/*
  Type-safe API client for Accounts and Onboarding endpoints.
  - Base URL: import.meta.env.VITE_API_BASE || http://localhost:5075
  - Envelope-aware: { success: boolean, data, errors?: string[] }
  - Token helpers: localStorage keys 'accessToken' and 'refreshToken'
  - Auto-refresh on 401: calls /api/accounts/refresh-token once and retries original request

  Safety/performance additions in this file:
  - fetch timeout using AbortController (default 15s)
  - GET request deduplication (in-flight requests) to avoid duplicate network calls
  - Simple POST/PUT throttling to avoid accidental repeated heavy operations (default 1s)
  - Per-request overrides: _timeout, _dedupe, _throttleMs, _allowHeavy
*/

const API_BASE = (import.meta as any).env?.VITE_API_BASE || 'http://localhost:5075'

// -------------------- Types --------------------

type ApiEnvelope<T> = { success: boolean; data: T; errors?: string[] }

// Accounts DTOs
export type RegisterRequest = {
  FullName: string
  UserName: string
  Email: string
  PhoneNumber?: string | null
  Password: string
  ConfirmPassword: string
  Role: number
  PhoneVerificationChannel?: number | null
}

export type RegisterResponse = {
  Success: boolean
  RequiresEmailVerification: boolean
  RequiresPhoneVerification: boolean
  TelegramBotLink?: string | null
}

export type LoginRequest = { Email?: string | null; UserName?: string | null; Password: string }

export type AuthResponseDto = {
  AccessToken: string
  RefreshToken: string
  AccessTokenExpiration: string
  RefreshTokenExpiration: string
  UserId: string
  UserName: string
  Email: string
  FullName: string
}

export type TokenDto = {
  AccessToken: string
  RefreshToken: string
  AccessTokenExpiration: string
  RefreshTokenExpiration: string
}

export type ExternalLoginRequest = { Provider: 'google' | 'github'; IdToken: string; AccessToken?: string | null; AuthorizationCode?: string | null }
export type ExternalLoginResponse = { AccessToken: string; RefreshToken: string; RefreshTokenExpiration: string; IsNewUser: boolean; UserId: string; Email?: string | null; FullName?: string | null }

export type ForgotPasswordRequest = { Email: string }
export type VerifyResetCodeRequest = { Email: string; VerificationCode: string }
export type VerifyResetCodeResponse = { ResetToken: string; ExpiresAt: string }
export type ResetPasswordRequest = { Email: string; ResetToken: string; NewPassword: string; ConfirmPassword: string }

export type VerifyEmailRequest = { Email: string; VerificationCode: string }
export type VerifyPhoneRequest = { PhoneNumber: string; VerificationCode: string }
export type ResendEmailRequest = { Email: string }
export type ResendPhoneRequest = { PhoneNumber: string; Channel: number }

export type AccountDto = {
  Id: string
  UserName: string
  Email: string
  FullName: string
  PhoneNumber?: string | null
  EmailConfirmed: boolean
  PhoneNumberConfirmed: boolean
  CreatedAt: string
  Status?: number | null
  LastLoginAt?: string | null
}

export type UpdateProfileRequest = { FullName?: string | null; UserName?: string | null }
export type ChangePasswordRequest = { CurrentPassword?: string; NewPassword: string; ConfirmPassword: string }
export type AddPhoneNumberRequest = { PhoneNumber: string }

// Onboarding DTOs
export type OnboardingOptionDto = { Id: string; Text: string; RecommendedIntegration?: string | null }
export type OnboardingQuestionDto = { Id: string; Text: string; Order: number; IsMultiSelect: boolean; TargetProfession?: number | null; Options: OnboardingOptionDto[] }
export type ProfessionQuestionDto = OnboardingQuestionDto
export type CreateOnboardingQuestionRequest = { Text: string; Order: number; IsMultiSelect: boolean; TargetProfession?: number | null }
export type CreateOnboardingQuestionResponse = string // created GUID
export type AddOptionToQuestionRequest = { QuestionId: string; Text: string; RecommendedIntegration?: string | null; RecommendedTemplate?: string | null }
export type AddOptionToQuestionResponse = string // created GUID
export type OnboardingAnswerDto = { QuestionId: string; OptionId: string }
export type CompleteOnboardingRequest = { UserId?: string; Profession: number; JobTitle?: string | null; Answers: OnboardingAnswerDto[] }
export type CompleteOnboardingResponse = { ProfileId: string }

// -------------------- Token helpers --------------------
const ACCESS_KEY = 'accessToken'
const REFRESH_KEY = 'refreshToken'

export function getTokens(): { accessToken?: string | null; refreshToken?: string | null } {
  try {
    return { accessToken: localStorage.getItem(ACCESS_KEY), refreshToken: localStorage.getItem(REFRESH_KEY) }
  } catch (e) {
    return { accessToken: undefined, refreshToken: undefined }
  }
}

export function setTokens(tokens: { AccessToken: string; RefreshToken: string }) {
  try {
    localStorage.setItem(ACCESS_KEY, tokens.AccessToken)
    localStorage.setItem(REFRESH_KEY, tokens.RefreshToken)
  } catch (e) {
    // ignore
  }
}

export function clearTokens() {
  try {
    localStorage.removeItem(ACCESS_KEY)
    localStorage.removeItem(REFRESH_KEY)
  } catch (e) {}
}

// -------------------- API Error --------------------

export class ApiError extends Error {
  status: number
  errors: string[]
  constructor(status: number, errors: string[] = []) {
    super(errors?.join(', ') || 'ApiError')
    this.name = 'ApiError'
    this.status = status
    this.errors = errors
  }
}

// -------------------- Request helper --------------------

type RequestOptions = RequestInit & { skipAuth?: boolean; _retry?: boolean; _timeout?: number; _dedupe?: boolean; _throttleMs?: number; _allowHeavy?: boolean }

const DEFAULT_TIMEOUT = 15000 // 15s
const DEFAULT_POST_THROTTLE_MS = 1000 // 1 second between POST/PUT to prevent accidental duplicates

const inFlightRequests = new Map<string, Promise<Response>>()
const lastRequestTimestamps = new Map<string, number>()

function makeRequestKey(method: string, url: string, body?: any) {
  // for GET requests body is ignored; for others include body to distinguish
  return `${method.toUpperCase()}::${url}::${body ? JSON.stringify(body) : ''}`
}

async function fetchWithTimeout(url: string, opts: RequestInit = {}, timeout = DEFAULT_TIMEOUT) {
  const controller = new AbortController()
  const timer = setTimeout(() => controller.abort(), timeout)
  try {
    const finalOpts = { ...opts, signal: controller.signal }
    const res = await fetch(url, finalOpts)
    return res
  } finally {
    clearTimeout(timer)
  }
}

async function rawFetch(path: string, opts: RequestInit = {}, timeout?: number) {
  const url = path.startsWith('http') ? path : `${API_BASE}${path}`
  // default to timeout if not provided
  return fetchWithTimeout(url, opts, typeof timeout === 'number' ? timeout : DEFAULT_TIMEOUT)
}

async function refreshTokensIfPossible(): Promise<TokenDto> {
  const { refreshToken } = getTokens()
  if (!refreshToken) throw new ApiError(401, ['No refresh token available'])

  const res = await rawFetch('/api/accounts/refresh-token', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ RefreshToken: refreshToken }),
  })

  if (res.status === 204) throw new ApiError(401, ['Refresh failed'])

  let json: ApiEnvelope<TokenDto>
  try {
    json = await res.json()
  } catch (e) {
    throw new ApiError(res.status, ['Invalid JSON from refresh endpoint'])
  }

  if (!json?.success) {
    clearTokens()
    throw new ApiError(res.status, json?.errors || ['Failed to refresh token'])
  }

  setTokens({ AccessToken: json.data.AccessToken, RefreshToken: json.data.RefreshToken })
  return json.data
}

async function request<T = any>(path: string, opts: RequestOptions = {}): Promise<T> {
  const method = (opts.method || 'GET').toUpperCase()
  const headers: Record<string, string> = { 'Content-Type': 'application/json', ...(opts.headers as Record<string, string> || {}) }
  const finalOpts: RequestInit = { ...opts, headers }

  // Throttle check for non-GET requests (prevent accidental heavy operations)
  const keyForThrottle = makeRequestKey(method, path, opts.body)
  if (method !== 'GET' && !opts._allowHeavy) {
    const last = lastRequestTimestamps.get(keyForThrottle) || 0
    const throttleMs = typeof opts._throttleMs === 'number' ? opts._throttleMs : DEFAULT_POST_THROTTLE_MS
    const now = Date.now()
    if (now - last < throttleMs) {
      throw new ApiError(429, ['Too many requests. Please wait and try again.'])
    }
    // update timestamp to now (we'll allow this request to proceed)
    lastRequestTimestamps.set(keyForThrottle, now)
  }

  // Authentication header
  if (!opts.skipAuth) {
    const { accessToken } = getTokens()
    if (accessToken) headers['Authorization'] = `Bearer ${accessToken}`
  }

  const url = path.startsWith('http') ? path : `${API_BASE}${path}`

  // Deduplication for GET requests (and if _dedupe requested)
  const dedupeKey = makeRequestKey(method, url, opts.body)
  if (method === 'GET' && (opts._dedupe ?? true)) {
    const existing = inFlightRequests.get(dedupeKey)
    if (existing) {
      // reuse the ongoing fetch promise, then parse JSON + envelope like normal
      const reuseRes = await existing
      if (reuseRes.status === 204) return undefined as unknown as T
      let envelope: ApiEnvelope<any>
      try {
        envelope = await reuseRes.clone().json()
      } catch (e) {
        throw new ApiError(reuseRes.status, ['Invalid JSON response'])
      }
      if (!envelope?.success) throw new ApiError(reuseRes.status, envelope?.errors || ['Unknown error'])
      return envelope.data as T
    }
  }

  // Perform fetch and add to inFlight map if deduping
  let fetchPromise: Promise<Response>
  try {
    fetchPromise = rawFetch(path, finalOpts, opts._timeout)
    if (method === 'GET' && (opts._dedupe ?? true)) inFlightRequests.set(dedupeKey, fetchPromise)
    const res = await fetchPromise

    if (method === 'GET' && (opts._dedupe ?? true)) inFlightRequests.delete(dedupeKey)

    // 204 No Content
    if (res.status === 204) return undefined as unknown as T

    let envelope: ApiEnvelope<any>
    try {
      envelope = await res.json()
    } catch (e) {
      throw new ApiError(res.status, ['Invalid JSON response'])
    }

    if (res.status === 401 && !opts._retry) {
      // try refresh once
      try {
        await refreshTokensIfPossible()
        // retry original request with _retry flag
        const retryOpts: RequestOptions = { ...opts, _retry: true }
        // ensure Authorization header updated
        const { accessToken } = getTokens()
        retryOpts.headers = { ...(retryOpts.headers as Record<string, string> || {}), 'Content-Type': 'application/json' }
        if (accessToken) (retryOpts.headers as Record<string, string>)['Authorization'] = `Bearer ${accessToken}`
        const retryRes = await rawFetch(path, retryOpts, opts._timeout)
        if (retryRes.status === 204) return undefined as unknown as T
        try {
          const retryEnvelope = await retryRes.json()
          if (!retryEnvelope.success) throw new ApiError(retryRes.status, retryEnvelope.errors || ['Request failed'])
          return retryEnvelope.data as T
        } catch (e) {
          if (e instanceof ApiError) throw e
          throw new ApiError(retryRes.status, ['Invalid JSON on retry'])
        }
      } catch (e) {
        // refresh failed
        clearTokens()
        if (e instanceof ApiError) throw e
        throw new ApiError(401, ['Unauthorized'])
      }
    }

    if (!envelope?.success) {
      throw new ApiError(res.status, envelope?.errors || ['Unknown error'])
    }

    return envelope.data as T
  } catch (e) {
    // clean up inFlight map on error
    if (method === 'GET' && (opts._dedupe ?? true)) inFlightRequests.delete(dedupeKey)

    // If the error was an AbortError, convert to ApiError with 408
    if (e instanceof Error && (e as any).name === 'AbortError') {
      throw new ApiError(408, ['Request timeout'])
    }

    if (e instanceof ApiError) throw e
    throw new ApiError(500, [e instanceof Error ? e.message : String(e)])
  }
}

// safeRequest: wrapper to enforce non-heavy defaults
export async function safeRequest<T = any>(path: string, opts: RequestOptions = {}): Promise<T> {
  // By default, disallow heavy operations (POST/PUT/DELETE) unless _allowHeavy is explicitly passed true.
  const method = (opts.method || 'GET').toUpperCase()
  if (['POST', 'PUT', 'DELETE'].includes(method) && !opts._allowHeavy) {
    // If caller didn't opt-in, we reject with a clear error so UI cannot accidentally perform heavy ops.
    throw new ApiError(403, ['Heavy operations are disabled by default. Set _allowHeavy: true to opt-in explicitly.'])
  }
  // Keep other behavior same as request
  return request<T>(path, opts)
}

// Note: existing exported endpoints continue to use `request` for backward compatibility. If you want the UI to be safer by default,
// switch endpoint exports to use `safeRequest` for POST/PUT/DELETE. Example below (commented):

// export const accounts = {
//   register: (payload: RegisterRequest) => safeRequest<RegisterResponse>('/api/accounts/register', { method: 'POST', body: JSON.stringify(payload) }),
//   // ...other endpoints
// }

// For now we keep the original `accounts` and `onboarding` exports unchanged to avoid breaking code, but recommend scanning and updating callers to pass `_allowHeavy: true` only where appropriate.

// -------------------- Accounts endpoints --------------------

export const accounts = {
  register: (payload: RegisterRequest) => request<RegisterResponse>('/api/accounts/register', { method: 'POST', body: JSON.stringify(payload) }),
  login: (payload: LoginRequest) => request<AuthResponseDto>('/api/accounts/login', { method: 'POST', body: JSON.stringify(payload) }),
  refreshToken: (refreshToken: string) => request<TokenDto>('/api/accounts/refresh-token', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ RefreshToken: refreshToken }), skipAuth: true }),
  revokeRefreshToken: () => rawFetch(`${API_BASE}/api/accounts/revoke-refresh-token`, { method: 'POST', headers: { 'Content-Type': 'application/json', ...(getTokens().accessToken ? { Authorization: `Bearer ${getTokens().accessToken}` } : {}) }, body: JSON.stringify({}) }).then(r => { if (r.status === 204) return; return r.json() }),
  externalLogin: (payload: ExternalLoginRequest) => request<ExternalLoginResponse>('/api/accounts/external-login', { method: 'POST', body: JSON.stringify(payload) }),
  forgotPassword: (payload: ForgotPasswordRequest) => request<boolean>('/api/accounts/forgot-password', { method: 'POST', body: JSON.stringify(payload) }),
  verifyResetCode: (payload: VerifyResetCodeRequest) => request<VerifyResetCodeResponse>('/api/accounts/verify-reset-code', { method: 'POST', body: JSON.stringify(payload) }),
  resetPassword: (payload: ResetPasswordRequest) => request<boolean>('/api/accounts/reset-password', { method: 'POST', body: JSON.stringify(payload) }),
  verifyEmail: (payload: VerifyEmailRequest) => request<boolean>('/api/accounts/verify-email', { method: 'POST', body: JSON.stringify(payload) }),
  verifyPhone: (payload: VerifyPhoneRequest) => request<boolean>('/api/accounts/verify-phone', { method: 'POST', body: JSON.stringify(payload) }),
  resendEmailVerification: (payload: ResendEmailRequest) => request<boolean>('/api/accounts/resend-email-verification-code', { method: 'POST', body: JSON.stringify(payload) }),
  resendPhoneVerification: (payload: ResendPhoneRequest) => request<boolean>('/api/accounts/resend-phone-verification-code', { method: 'POST', body: JSON.stringify(payload) }),
  getProfile: () => request<AccountDto>('/api/accounts/profile', { method: 'GET' }),
  updateProfile: (payload: UpdateProfileRequest) => request<AccountDto>('/api/accounts/profile', { method: 'PUT', body: JSON.stringify(payload) }),
  changePassword: (payload: ChangePasswordRequest) => request<boolean>('/api/accounts/change-password', { method: 'PUT', body: JSON.stringify(payload) }),
  addPhoneNumber: (payload: AddPhoneNumberRequest) => request<boolean>('/api/accounts/add-phone-number', { method: 'POST', body: JSON.stringify(payload) }),
}

// -------------------- Onboarding endpoints --------------------

export const onboarding = {
  getProfessionQuestion: () => request<ProfessionQuestionDto>('/api/onboarding/profession-question', { method: 'GET', skipAuth: true }),
  getQuestions: (profession?: number | string) => {
    const q = profession !== undefined && profession !== null ? `?profession=${encodeURIComponent(String(profession))}` : ''
    return request<OnboardingQuestionDto[]>(`/api/onboarding/questions${q}`, { method: 'GET', skipAuth: true })
  },
  createQuestion: (payload: CreateOnboardingQuestionRequest) => request<CreateOnboardingQuestionResponse>('/api/onboarding/questions', { method: 'POST', body: JSON.stringify(payload) }),
  addOptionToQuestion: (questionId: string, payload: AddOptionToQuestionRequest) => request<AddOptionToQuestionResponse>(`/api/onboarding/questions/${questionId}/options`, { method: 'POST', body: JSON.stringify(payload) }),
  complete: (payload: CompleteOnboardingRequest) => request<CompleteOnboardingResponse>('/api/onboarding/complete', { method: 'POST', body: JSON.stringify(payload) }),
}

export default { accounts, onboarding, getTokens, setTokens, clearTokens }
