import { createContext, useContext, useState, ReactNode, useEffect } from "react";
import api, { ApiError } from '@/lib/apiClient'

export type UserRole = "developer" | "designer" | "cybersecurity" | "marketer" | "team-leader";

// Map backend profession integer to frontend role string
function professionToRole(profession: any): UserRole {
  const map: Record<number | string, UserRole> = {
    1: 'developer',
    2: 'designer',
    3: 'cybersecurity',
    4: 'marketer',
    5: 'team-leader',
    Developer: 'developer',
    designer: 'designer',
    Designer: 'designer',
    CyberSecurity: 'cybersecurity',
    cybersecurity: 'cybersecurity',
    DigitalMarketing: 'marketer',
    marketer: 'marketer',
    ProductManager: 'team-leader',
    'team-leader': 'team-leader',
    TeamLeader: 'team-leader',
  }
  if (profession && map[profession]) return map[profession]
  if (typeof profession === 'string') {
    const lower = profession.toLowerCase()
    if (lower.includes('developer') || lower === '1') return 'developer'
    if (lower.includes('designer') || lower === '2') return 'designer'
    if (lower.includes('cyber') || lower.includes('security') || lower.includes('secops') || lower === '3') return 'cybersecurity'
    if (lower.includes('market') || lower === '4') return 'marketer'
    if (lower.includes('leader') || lower.includes('manager') || lower === '5') return 'team-leader'
  }
  return 'developer'
}

interface User {
  fullName: string;
  username: string;
  email: string;
  phone?: string;
  phoneContact?: "sms" | "telegram";
  role?: UserRole;
  onboardingComplete: boolean;
  onboardingAnswers?: Record<string, string>;
  tags?: string[];
  bio?: string | null;
}

interface AuthContextType {
  user: User | null;
  isAuthenticated: boolean;
  // new flag: auth restoration in progress
  initializing: boolean;
  register: (data: Omit<User, "onboardingComplete"> & { password: string; confirmPassword: string }) => Promise<boolean>;
  // login now returns a detailed result so UI can react (email verification required, locked, etc.)
  login: (identifier: string, password: string) => Promise<{ ok: boolean; reason?: 'email_not_verified' | 'locked' | 'invalid_credentials' | 'profile_fetch_failed' | 'other'; message?: string }>;
  externalLogin: (provider: 'google' | 'github', idToken: string) => Promise<boolean>;
  // Explicit refresh helper: attempts to exchange stored refresh token for new tokens and finalize auth
  refreshTokens: () => Promise<boolean>;
  logout: () => void;
  setUserRole: (role: UserRole) => void;
  completeOnboarding: (answers: Record<string, string>) => Promise<boolean>;
  verifyEmail: (code: string, email?: string) => Promise<boolean>;
  verifyPhone: (code: string) => Promise<boolean>;
  resendEmailVerification: () => Promise<boolean>;
  resendPhoneVerification: () => Promise<boolean>;
  emailVerified: boolean;
  phoneVerified: boolean;
  setEmailVerified: (v: boolean) => void;
  setPhoneVerified: (v: boolean) => void;
  // allow other components to finalize tokens received from external login flows
  finalizeAuthFromTokens: (tokenPayload: { AccessToken: string; RefreshToken: string } | null) => Promise<boolean>;
}

const AuthContext = createContext<AuthContextType>({} as AuthContextType);

export const useAuth = () => useContext(AuthContext);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<User | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [emailVerified, setEmailVerified] = useState(false);
  const [phoneVerified, setPhoneVerified] = useState(false);
  const [pendingPassword, setPendingPassword] = useState<string | null>(null);
  // new state to indicate initial auth restoration is running
  const [initializing, setInitializing] = useState(true);

  const register = async (data: Omit<User, 'onboardingComplete'> & { password: string; confirmPassword: string }) => {
    try {
      // map UI user shape to backend RegisterRequest
      const payload = {
        FullName: data.fullName,
        UserName: data.username,
        Email: data.email,
        Password: data.password,
      }

      // Call register and apply returned tokens so frontend treats user as authenticated
      const res = await api.accounts.register(payload)

      // If server returned tokens, finalize auth from tokens (this will store tokens, fetch profile and set authenticated state)
      if (res?.AccessToken && res?.RefreshToken) {
        const ok = await finalizeAuthFromTokens({ AccessToken: res.AccessToken, RefreshToken: res.RefreshToken })
        if (!ok) {
          // If finalizing failed, clear tokens and return false
          api.clearTokens()
          return false
        }
      }

      // Store minimal user info in case profile fetch didn't set it fully
      if (!user) setUser({ fullName: data.fullName, username: data.username, email: data.email, onboardingComplete: false })
      setEmailVerified(false)
      setPhoneVerified(false)

      return true
    } catch (e) {
      // If server returned ApiError, rethrow so UI can display server-side validation messages
      if (e instanceof ApiError) throw e
      return false
    }
  };

  // Helper to finish auth after tokens are set: fetch profile and only on success mark authenticated
  const finalizeAuthFromTokens = async (tokenPayload: { AccessToken: string; RefreshToken: string } | null) => {
    if (!tokenPayload || !tokenPayload.AccessToken) return false
    api.setTokens({ AccessToken: tokenPayload.AccessToken, RefreshToken: tokenPayload.RefreshToken })
    try {
      const profile = await api.accounts.getProfile()
      setUser({
        fullName: profile.FullName,
        username: profile.UserName,
        email: profile.Email,
        phone: profile.PhoneNumber || undefined,
        role: professionToRole((profile as any).Profession ?? (profile as any).profession ?? profile.Status),
        onboardingComplete: true,
      })
      setEmailVerified(profile.EmailConfirmed)
      setPhoneVerified(Boolean(profile.PhoneNumberConfirmed))
      setIsAuthenticated(true)
      return true
    } catch (e) {
      // profile fetch failed -> clear tokens and don't authenticate
      api.clearTokens()
      setUser(null)
      setIsAuthenticated(false)
      return false
    } finally {
      // ensure initializing flag is cleared after any finalize attempt
      try { setInitializing(false) } catch {}
    }
  }

  const login: AuthContextType['login'] = async (_identifier: string, _password: string) => {
    try {
      const payload: any = {}
      if (_identifier.includes('@')) payload.Email = _identifier
      else payload.UserName = _identifier
      payload.Password = _password

      const res = await api.accounts.login(payload)
      // Some backends may return the DTO directly, or may return a wrapper like { data: { ... } } or different casing.
      // Recursively unwrap nested `data` fields up to a few levels to tolerate different envelopes.
      let raw: any = res as any
      for (let i = 0; i < 3; i++) {
        if (raw && raw.data && typeof raw.data === 'object') raw = raw.data
        else break
      }
      // tolerate different casing and token key names from backend
      const accessToken = raw?.AccessToken ?? raw?.accessToken ?? raw?.access_token ?? raw?.token ?? raw?.jwt ?? raw?.idToken ?? raw?.id_token ?? raw?.access
      const refreshToken = raw?.RefreshToken ?? raw?.refreshToken ?? raw?.refresh_token ?? raw?.refresh ?? raw?.refreshTokenValue
      if (!accessToken || !refreshToken) {
        // Log the unexpected server response to console to help debugging.
        try {
          console.error('Login: server returned invalid token payload:', res)
        } catch (logErr) {}
        return { ok: false, reason: 'other', message: 'Invalid token payload (server returned unexpected response). Check backend logs or inspect network response.' }
      }

      // normalize tokens object for finalizeAuthFromTokens
      const normalized = { AccessToken: accessToken, RefreshToken: refreshToken }

      // finalize auth and directly return result
      if (!(await finalizeAuthFromTokens(normalized))) {
        return { ok: false, reason: 'profile_fetch_failed', message: 'Failed to fetch profile after login' }
      }
      return { ok: true }
    } catch (e) {
      if (e instanceof ApiError) {
        // Map common server-side statuses/messages to friendly reasons
        if (e.status === 401) {
          const rawMsg = e.errors?.join(', ') || e.message || ''
          const msg = rawMsg.toLowerCase()

          // Email not verified
          if (msg.includes('email not verified') || msg.includes('email not confirmed')) {
            try {
              const maybeEmail = _identifier.includes('@') ? _identifier : undefined
              const maybeUserName = !_identifier.includes('@') ? _identifier : ''
              if (maybeEmail) setUser({ fullName: '', username: maybeUserName ?? '', email: maybeEmail, onboardingComplete: false })
            } catch {}
            return {
              ok: false,
              reason: 'email_not_verified',
              message: 'Your email address is not verified. Please check your email for the verification code or resend the code.'
            }
          }

          // Deleted / unauthorized account
          if (msg.includes('deleted') || msg.includes('this account has been deleted')) {
            return {
              ok: false,
              reason: 'other',
              message: 'This account has been deleted. If you believe this is an error, please contact support.'
            }
          }

          // Locked mention in 401 message (fallback)
          if (msg.includes('locked') || msg.includes('too many failed')) {
            // Surface server message only in logs; show friendly message to user
            return {
              ok: false,
              reason: 'locked',
              message: 'Your account is temporarily locked due to multiple failed sign-in attempts. Please try again later or contact support.'
            }
          }

          // Invalid credentials — try to produce a friendly localized message.
          // If server provided remaining attempts info, include it when safe.
          let friendly = 'Invalid email/username or password. Please try again.'
          // attempt to extract a remaining attempts number from server message (e.g., "2 attempts remaining")
          const attemptsMatch = rawMsg.match(/(\d+)\s*(?:attempts?|cəhd|tries)/i)
          if (attemptsMatch && attemptsMatch[1]) {
            friendly = `Invalid email/username or password. ${attemptsMatch[1]} attempts remaining before account lockout.`
          }
          return { ok: false, reason: 'invalid_credentials', message: friendly }
        }
        // 423 Locked -> account locked
        if (e.status === 423) {
          // If backend provided a human-readable duration/minutes in errors, prefer it for logs; show friendly message to user
          const serverMsg = e.errors?.join(', ') || e.message || ''
          try { console.warn('Account locked details from server:', serverMsg) } catch {}
          return { ok: false, reason: 'locked', message: 'Your account is locked. Please try again later or contact support.' }
        }
        // Generic server error: log detailed server message, but return friendly message to user
        try { console.error('Login ApiError details:', e.status, e.errors) } catch {}
        return { ok: false, reason: 'other', message: 'Sign in failed due to a server error. Please try again later.' }
      }
      // Non-ApiError fallback
      return { ok: false, reason: 'other', message: e instanceof Error ? e.message : String(e) }
    }
  };

  const externalLogin = async (provider: 'google' | 'github', idToken: string) => {
    try {
      const res = await api.accounts.externalLogin({ Provider: provider, IdToken: idToken })
      if (!res?.AccessToken || !res?.RefreshToken) return false
      // If backend marks this as a new user, tokens are still provided — finalize auth the same way.
      return finalizeAuthFromTokens({ AccessToken: res.AccessToken, RefreshToken: res.RefreshToken })
    } catch (e) {
      // Rethrow ApiError so UI can display backend validation messages (e.g., 400 invalid token)
      if (e instanceof ApiError) throw e
      return false
    }
  }

  // Attempt to refresh tokens using the current stored refresh token.
  const refreshTokens = async () => {
    const { refreshToken } = api.getTokens()
    if (!refreshToken) throw new ApiError(401, ['No refresh token available'])
    const res = await api.accounts.refreshToken(refreshToken)
    // res should be TokenDto
    if (!res?.AccessToken || !res?.RefreshToken) throw new ApiError(500, ['Invalid token payload'])
    // finalizeAuthFromTokens will store tokens and fetch profile
    return finalizeAuthFromTokens({ AccessToken: res.AccessToken, RefreshToken: res.RefreshToken })
  }

  // On mount: if tokens exist try to load profile
  useEffect(() => {
    const tryLoad = async () => {
      const { accessToken } = api.getTokens()
      if (!accessToken) {
        // no token -> still mark initialization complete
        setInitializing(false)
        return
      }
      try {
        const profile = await api.accounts.getProfile()
        setUser({
          fullName: profile.FullName,
          username: profile.UserName,
          email: profile.Email,
          phone: profile.PhoneNumber || undefined,
          role: professionToRole((profile as any).Profession ?? (profile as any).profession ?? profile.Status),
          onboardingComplete: true,
        })
        setIsAuthenticated(true)
        setEmailVerified(profile.EmailConfirmed)
        setPhoneVerified(Boolean(profile.PhoneNumberConfirmed))
      } catch (e) {
        // tokens invalid or profile fetch failed: clear tokens
        api.clearTokens()
        setUser(null)
        setIsAuthenticated(false)
      } finally {
        // initialization done regardless of success
        setInitializing(false)
      }
    }
    tryLoad()
  }, [])

  const logout = () => {
    // Attempt to revoke refresh tokens on the server for the current user.
    // This endpoint requires Authorization header; apiClient will attach access token if present.
    (async () => {
      try {
        await api.accounts.revokeRefreshToken()
      } catch (e) {
        // If revoke fails (e.g., 401 unauthorized), continue with local cleanup.
        // Re-throwing is not desired for a logout action — we want logout to succeed client-side.
        // We could show a toast here, but keep silent to avoid blocking logout flow.
      } finally {
        api.clearTokens()
        setUser(null);
        setIsAuthenticated(false);
        setEmailVerified(false);
        setPhoneVerified(false);
      }
    })()
  };

  const setUserRole = (role: UserRole) => {
    if (user) setUser({ ...user, role });
  };

  const completeOnboarding = async (answers: Record<string, string>) => {
    try {
      // Build payload similar to apiClient.complete normalization
      const skipKeys = new Set(['Profession', 'JobTitle', 'role', 'Role'])
      const answerArray: any[] = []
      Object.entries(answers).forEach(([k, v]) => {
        if (!v || skipKeys.has(k)) return
        // comma-separated multi-select
        if (typeof v === 'string' && v.includes(',')) {
          v.split(',').map(s => s.trim()).filter(Boolean).forEach(val => answerArray.push({ QuestionId: k, OptionId: String(val) }))
        } else {
          answerArray.push({ QuestionId: k, OptionId: String(v) })
        }
      })

      // Profession mapping: support role strings like 'developer'
      let professionVal: any = answers['Profession'] ?? answers['profession'] ?? answers['role'] ?? answers['Role'] ?? 1
      if (typeof professionVal === 'string' && isNaN(Number(professionVal))) {
        const roleMap: Record<string, number> = { developer: 1, designer: 2, cybersecurity: 3, marketer: 4, 'team-leader': 5 }
        professionVal = roleMap[professionVal] ?? roleMap[professionVal?.toLowerCase?.()] ?? 1
      }

      const payload = {
        Profession: Number(professionVal) || 1,
        JobTitle: (answers['JobTitle'] ?? answers['jobTitle']) || null,
        Answers: answerArray,
      }

      await api.onboarding.complete(payload as any)

      // After successful onboarding, update local user: mark onboardingComplete and store answers
      if (user) {
        // generate tags from selected options: collect OptionId strings
        const tagsSet = new Set<string>()
        answerArray.forEach((a: any) => {
          if (a && a.OptionId) tagsSet.add(String(a.OptionId))
        })
        const tags = Array.from(tagsSet)

        // generate a lightweight bio: use role and experience if provided
        const roleStr = (answers['role'] ?? user.role) as string | undefined
        const experience = answers['experience'] || answers['Experience'] || undefined
        const jobTitle = answers['JobTitle'] || answers['jobTitle'] || undefined
        let bioParts: string[] = []
        if (jobTitle) bioParts.push(String(jobTitle))
        if (roleStr) bioParts.push(String(roleStr))
        if (experience) bioParts.push(String(experience))
        const bio = bioParts.length ? bioParts.join(' — ') : null

        setUser({ ...user, onboardingComplete: true, onboardingAnswers: answers, tags, bio })
      }

      setIsAuthenticated(true)
      return true
    } catch (e) {
      return false
    }
  };

  const verifyEmail = async (code: string, emailParam?: string) => {
    if (code.length !== 6) return false
    const emailToUse = emailParam ?? user?.email
    if (!emailToUse) return false
    try {
      await api.accounts.verifyEmail({ Email: emailToUse, VerificationCode: code })
      setEmailVerified(true)
      // after successful verification, if we have a pending password, attempt to login and fetch profile
      if (pendingPassword && emailToUse) {
        try {
          const loginRes = await api.accounts.login({ Email: emailToUse, Password: pendingPassword })
          if (loginRes?.AccessToken && loginRes?.RefreshToken) {
            if (!(await finalizeAuthFromTokens({ AccessToken: loginRes.AccessToken, RefreshToken: loginRes.RefreshToken }))) {
              // auto-login failed; clear pending and return true (verification itself succeeded)
            }
          }
        } catch (loginErr) {
          // ignore auto-login failure; user can manually login
        }
        setPendingPassword(null)
      }
      return true
    } catch (e) {
      // Rethrow ApiError so UI can show server-provided messages
      if (e instanceof ApiError) throw e
      return false
    }
  }

  const verifyPhone = async (code: string) => {
    if (code.length !== 6) return false
    try {
      if (!user?.phone) return false
      await api.accounts.verifyPhone({ PhoneNumber: user.phone, VerificationCode: code })
      setPhoneVerified(true)
      return true
    } catch (e) {
      // Rethrow ApiError so UI can show server-provided messages
      if (e instanceof ApiError) throw e
      return false
    }
  }

  const resendEmailVerification = async () => {
    try {
      if (!user?.email) return false
      await api.accounts.resendEmailVerification({ Email: user.email })
      return true
    } catch (e) { return false }
  }

  const resendPhoneVerification = async () => {
    try {
      if (!user?.phone) return false
      const channel = user.phoneContact === 'telegram' ? 2 : 1
      await api.accounts.resendPhoneVerification({ PhoneNumber: user.phone, Channel: channel })
      return true
    } catch (e) { return false }
  }

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated,
        register,
        login,
        externalLogin,
        logout,
        setUserRole,
        completeOnboarding,
        verifyEmail,
        verifyPhone,
        resendEmailVerification,
        resendPhoneVerification,
        emailVerified,
        phoneVerified,
        setEmailVerified,
        setPhoneVerified,
        refreshTokens,
        finalizeAuthFromTokens,
        // expose initialization flag
        initializing,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};
