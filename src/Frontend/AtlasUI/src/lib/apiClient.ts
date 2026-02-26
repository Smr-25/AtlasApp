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
  Password: string
}

export type RegisterResponse = AuthResponseDto

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
export type CompleteOnboardingRequest = { Profession: number; JobTitle?: string | null; Answers: OnboardingAnswerDto[] }
export type CompleteOnboardingResponse = { ProfileId: string }

// -------------------- Workspaces & Integrations DTOs --------------------
export type WorkspaceDto = {
  Id: string
  Name: string
  Description?: string | null
  IsDefault: boolean
  LocalFolderPath?: string | null
  IsShared?: boolean
  CreatedAt?: string
  ActiveIntegrations?: WorkspaceIntegrationDto[]
}
export type WorkspaceIntegrationDto = { IntegrationId: string; IntegrationName: string; Provider: string; Enabled: boolean; ConnectedAt?: string }
export type ListWorkspacesResponse = WorkspaceDto[]
export type CreateWorkspaceRequest = { Name: string; Description?: string | null; LocalFolderPath?: string | null; IsShared?: boolean }
export type CreateWorkspaceResponse = { Id: string }

export type IntegrationDto = { Id: string; Name: string; Provider: string; Status: string; MetadataJson?: string | null; IsActive: boolean; TokenExpiresAt?: string | null }
export type IntegrationDetailDto = IntegrationDto & { WorkspaceCount?: number; CreatedAt?: string; ModifiedAt?: string }
export type ListIntegrationsResponse = IntegrationDto[]
export type ConnectIntegrationRequest = { Provider: string; Name: string; AccessToken: string; RefreshToken?: string | null; ExpiresAt?: string | null; MetadataJson?: string | null }

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

// single-flight refresh promise to avoid multiple concurrent refresh calls
let refreshPromise: Promise<TokenDto> | null = null

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
  // If there's already an in-flight refresh, return it (single-flight)
  if (refreshPromise) return refreshPromise

  refreshPromise = (async () => {
    const { refreshToken } = getTokens()
    if (!refreshToken) {
      refreshPromise = null
      throw new ApiError(401, ['No refresh token available'])
    }

    const res = await rawFetch('/api/accounts/refresh-token', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ RefreshToken: refreshToken }),
    })

    if (res.status === 204) {
      refreshPromise = null
      throw new ApiError(401, ['Refresh failed'])
    }

    let json: ApiEnvelope<TokenDto> | any
    try {
      json = await res.json()
    } catch (e) {
      refreshPromise = null
      throw new ApiError(res.status, ['Invalid JSON from refresh endpoint'])
    }

    // tolerate both envelope and direct TokenDto
    if (json && typeof json === 'object' && json.success === undefined) {
      // assume direct TokenDto
      if (!json?.AccessToken || !json?.RefreshToken) {
        refreshPromise = null
        clearTokens()
        throw new ApiError(res.status, ['Invalid token payload'])
      }
      setTokens({ AccessToken: json.AccessToken, RefreshToken: json.RefreshToken })
      const out: TokenDto = { AccessToken: json.AccessToken, RefreshToken: json.RefreshToken, AccessTokenExpiration: json.AccessTokenExpiration, RefreshTokenExpiration: json.RefreshTokenExpiration }
      refreshPromise = null
      return out
    }

    if (!json?.success) {
      refreshPromise = null
      clearTokens()
      throw new ApiError(res.status, json?.errors || ['Failed to refresh token'])
    }

    setTokens({ AccessToken: json.data.AccessToken, RefreshToken: json.data.RefreshToken })
    const result = json.data as TokenDto
    refreshPromise = null
    return result
  })()

  return refreshPromise
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
      let envelope: ApiEnvelope<any> | any
      try {
        envelope = await reuseRes.clone().json()
      } catch (e) {
        throw new ApiError(reuseRes.status, ['Invalid JSON response'])
      }
      // tolerate non-envelope responses
      if (envelope && typeof envelope === 'object' && envelope.success === undefined) return envelope as T
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

    let envelope: ApiEnvelope<any> | any
    try {
      envelope = await res.json()
    } catch (e) {
      throw new ApiError(res.status, ['Invalid JSON response'])
    }

    // If server returned a direct DTO (no envelope), tolerate it by wrapping as success
    if (envelope && typeof envelope === 'object' && envelope.success === undefined) {
      return envelope as T
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
          // tolerate direct DTO on retry as well
          if (retryEnvelope && typeof retryEnvelope === 'object' && retryEnvelope.success === undefined) return retryEnvelope as T
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
  register: (payload: RegisterRequest) => request<AuthResponseDto>('/api/accounts/register', { method: 'POST', body: JSON.stringify(payload), skipAuth: true }),
  login: (payload: LoginRequest) => request<AuthResponseDto>('/api/accounts/login', { method: 'POST', body: JSON.stringify(payload), skipAuth: true }),
  refreshToken: (refreshToken: string) => request<TokenDto>('/api/accounts/refresh-token', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ RefreshToken: refreshToken }), skipAuth: true }),
  revokeRefreshToken: () => request<void>('/api/accounts/revoke-refresh-token', { method: 'POST', body: JSON.stringify({}) }),
  externalLogin: (payload: ExternalLoginRequest) => request<ExternalLoginResponse>('/api/accounts/external-login', { method: 'POST', body: JSON.stringify(payload), skipAuth: true }),
  forgotPassword: (payload: ForgotPasswordRequest) => request<boolean>('/api/accounts/forgot-password', { method: 'POST', body: JSON.stringify(payload), skipAuth: true }),
  verifyResetCode: (payload: VerifyResetCodeRequest) => request<VerifyResetCodeResponse>('/api/accounts/verify-reset-code', { method: 'POST', body: JSON.stringify(payload), skipAuth: true }),
  resetPassword: (payload: ResetPasswordRequest) => request<boolean>('/api/accounts/reset-password', { method: 'POST', body: JSON.stringify(payload), skipAuth: true }),
  verifyEmail: (payload: VerifyEmailRequest) => request<boolean>('/api/accounts/verify-email', { method: 'POST', body: JSON.stringify(payload) }),
  verifyPhone: (payload: VerifyPhoneRequest) => request<boolean>('/api/accounts/verify-phone', { method: 'POST', body: JSON.stringify(payload) }),
  resendEmailVerification: (payload: ResendEmailRequest) => request<boolean>('/api/accounts/resend-email-verification-code', { method: 'POST', body: JSON.stringify(payload), skipAuth: true }),
  resendPhoneVerification: (payload: ResendPhoneRequest) => request<boolean>('/api/accounts/resend-phone-verification-code', { method: 'POST', body: JSON.stringify(payload), skipAuth: true }),
  getProfile: () => request<AccountDto>('/api/accounts/profile', { method: 'GET' }),
  updateProfile: (payload: UpdateProfileRequest) => request<AccountDto>('/api/accounts/profile', { method: 'PUT', body: JSON.stringify(payload) }),
  changePassword: (payload: ChangePasswordRequest) => request<boolean>('/api/accounts/change-password', { method: 'PUT', body: JSON.stringify(payload) }),
  addPhoneNumber: (payload: AddPhoneNumberRequest) => request<boolean>('/api/accounts/add-phone-number', { method: 'POST', body: JSON.stringify(payload) }),
}

// -------------------- Onboarding endpoints --------------------

export const onboarding = {

  complete: (payload: CompleteOnboardingRequest | Record<string, any>) => {
    const hasAnswers = (payload as any).Answers && Array.isArray((payload as any).Answers)
    let finalPayload: CompleteOnboardingRequest
    if (hasAnswers) {
      finalPayload = payload as CompleteOnboardingRequest
    } else {
      const obj = payload as Record<string, any>
      const skipKeys = new Set(['Profession', 'JobTitle'])
      const answers: OnboardingAnswerDto[] = Object.entries(obj)
        .filter(([k, v]) => !skipKeys.has(k) && v !== undefined && v !== null)
        .map(([k, v]) => ({ QuestionId: k, OptionId: String(v) }))

      finalPayload = {
        Profession: obj.Profession ?? (obj.profession as any) ?? 1,
        JobTitle: obj.JobTitle ?? obj.jobTitle ?? null,
        Answers: answers,
      }
    }

    return request<CompleteOnboardingResponse>('/api/onboarding/complete', { method: 'POST', body: JSON.stringify(finalPayload) })
  },
}

// -------------------- Workspaces endpoints --------------------
export const workspaces = {
  list: () => request<ListWorkspacesResponse>('/api/workspaces', { method: 'GET' }),
  get: (id: string) => request<WorkspaceDto>(`/api/workspaces/${id}`, { method: 'GET' }),
  create: (payload: CreateWorkspaceRequest) => request<CreateWorkspaceResponse>('/api/workspaces', { method: 'POST', body: JSON.stringify(payload) }),
  update: (id: string, payload: CreateWorkspaceRequest) => request<void>(`/api/workspaces/${id}`, { method: 'PUT', body: JSON.stringify(payload) }),
  delete: (id: string) => request<void>(`/api/workspaces/${id}`, { method: 'DELETE' }),
  setDefault: (id: string) => request<void>(`/api/workspaces/${id}/set-default`, { method: 'PATCH' }),
  validateFolder: (payload: { FolderPath: string }) => request<{ IsValid: boolean; Reason?: string | null }>('/api/workspaces/validate-folder', { method: 'POST', body: JSON.stringify(payload) }),
}

// -------------------- Integrations endpoints --------------------
export const integrations = {
  list: () => request<ListIntegrationsResponse>('/api/integrations', { method: 'GET' }),
  listPending: () => request<ListIntegrationsResponse>('/api/integrations/pending', { method: 'GET' }),
  get: (id: string) => request<IntegrationDetailDto>(`/api/integrations/${id}`, { method: 'GET' }),
  create: (payload: ConnectIntegrationRequest) => request<IntegrationDto>('/api/integrations', { method: 'POST', body: JSON.stringify(payload) }),
  reconnect: (id: string, payload?: { AccessToken: string; RefreshToken?: string | null; ExpiresAt?: string | null }) => request<void>(`/api/integrations/${id}/reconnect`, { method: 'POST', body: JSON.stringify(payload || {}) }),
  delete: (id: string) => request<void>(`/api/integrations/${id}`, { method: 'DELETE' }),
  markExpired: (id: string) => request<void>(`/api/integrations/${id}/mark-expired`, { method: 'POST' }),
}

// -------------------- Teams endpoints --------------------
export const teams = {
  my: () => request<any[]>('/api/teams/my', { method: 'GET' }),
  get: (teamId: string) => request<any>(`/api/teams/${teamId}`, { method: 'GET' }),
  create: (payload: { Name: string; Description?: string | null }) => request<{ Id: string }>('/api/teams', { method: 'POST', body: JSON.stringify(payload) }),
  inviteMember: (teamId: string, payload: { UserId: string }) => request<any>(`/api/teams/${teamId}/members`, { method: 'POST', body: JSON.stringify(payload) }),
}

// -------------------- LeaderAgents endpoints --------------------
export const leaderagents = {
  bottleneck: (teamId: string) => request<any>(`/api/leaderagents/bottleneck/${teamId}`, { method: 'GET' }),
  burnoutRisk: (teamId: string) => request<any>(`/api/leaderagents/burnout-risk/${teamId}`, { method: 'GET' }),
  unassignedBugs: (teamId: string) => request<any>(`/api/leaderagents/unassigned-bugs/${teamId}`, { method: 'GET' }),
  prReviewNag: (payload: { TeamId: string; ThresholdHours: number }) => request<any>('/api/leaderagents/pr-review-nag', { method: 'POST', body: JSON.stringify(payload) }),
}

// Add leader-related namespaces for UI integration
export const omnifeed = {
  list: (teamId: string, source?: string, page = 1, pageSize = 20) => request<any>(`/api/omnifeed/${teamId}?source=${encodeURIComponent(source || '')}&page=${page}&pageSize=${pageSize}`, { method: 'GET' }),
  publish: (payload: { TeamId: string; Title: string; Body?: string | null }) => request<void>('/api/omnifeed/publish', { method: 'POST', body: JSON.stringify(payload) }),
  markRead: (itemId: string) => request<void>(`/api/omnifeed/${itemId}/read`, { method: 'POST' }),
  addEmoji: (itemId: string, payload: { Emoji: string }) => request<void>(`/api/omnifeed/${itemId}/emoji`, { method: 'POST', body: JSON.stringify(payload) }),
}

export const squadarena = {
  leaderboard: (teamId: string) => request<any>(`/api/squadarena/leaderboard/${teamId}`, { method: 'GET' }),
  createBounty: (payload: any) => request<any>('/api/squadarena/bounty', { method: 'POST', body: JSON.stringify(payload) }),
  claimBounty: (bountyId: string) => request<void>(`/api/squadarena/bounty/${bountyId}/claim`, { method: 'POST' }),
}

export const squadradar = {
  get: (teamId: string) => request<any>(`/api/squadradar/${teamId}`, { method: 'GET' }),
  updatePresence: (payload: any) => request<void>('/api/squadradar/presence', { method: 'PUT', body: JSON.stringify(payload) }),
}

export const resourcehub = {
  list: (teamId: string, category?: string) => request<any>(`/api/resourcehub/${teamId}${category ? `?category=${encodeURIComponent(category)}` : ''}`, { method: 'GET' }),
  add: (payload: any) => request<any>('/api/resourcehub', { method: 'POST', body: JSON.stringify(payload) }),
  update: (payload: any) => request<void>('/api/resourcehub', { method: 'PUT', body: JSON.stringify(payload) }),
  delete: (resourceId: string) => request<void>(`/api/resourcehub/${resourceId}`, { method: 'DELETE' }),
  pin: (resourceId: string) => request<void>(`/api/resourcehub/${resourceId}/pin`, { method: 'POST' }),
}

export const leadermodals = {
  list: () => request<any>('/api/leadermodals', { method: 'GET' }),
  getPayload: (modalId: string) => request<any>(`/api/leadermodals/${modalId}/payload`, { method: 'GET' }),
  open: (payload: { ModalType: string; TeamId?: string | null; PayloadJson?: string | null }) => request<any>('/api/leadermodals', { method: 'POST', body: JSON.stringify(payload) }),
  dismiss: (modalId: string) => request<void>(`/api/leadermodals/${modalId}/dismiss`, { method: 'POST' }),
}

export const leaderinsights = {
  sprintVelocity: (teamId: string, from?: string, to?: string) => request<any>(`/api/leaderinsights/sprint-velocity?teamId=${encodeURIComponent(teamId)}${from ? `&from=${encodeURIComponent(from)}` : ''}${to ? `&to=${encodeURIComponent(to)}` : ''}`, { method: 'GET' }),
  // generic GET helper for other insights can be called directly by path
}

export default { accounts, onboarding, getTokens, setTokens, clearTokens, workspaces, integrations, teams, leaderagents, omnifeed, squadarena, squadradar, resourcehub, leadermodals, leaderinsights }
