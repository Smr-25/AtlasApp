import { createContext, useContext, useState, ReactNode, useEffect } from "react";
import api from '@/lib/apiClient'

export type UserRole = "developer" | "designer" | "cybersecurity" | "marketer" | "team-leader";

interface User {
  fullName: string;
  username: string;
  email: string;
  phone?: string;
  phoneContact?: "sms" | "telegram";
  role?: UserRole;
  onboardingComplete: boolean;
  onboardingAnswers?: Record<string, string>;
}

interface AuthContextType {
  user: User | null;
  isAuthenticated: boolean;
  register: (data: Omit<User, "onboardingComplete"> & { password: string }) => Promise<boolean>;
  login: (identifier: string, password: string) => Promise<boolean>;
  logout: () => void;
  setUserRole: (role: UserRole) => void;
  completeOnboarding: (answers: Record<string, string>) => Promise<boolean>;
  verifyEmail: (code: string) => Promise<boolean>;
  verifyPhone: (code: string) => Promise<boolean>;
  resendEmailVerification: () => Promise<boolean>;
  resendPhoneVerification: () => Promise<boolean>;
  emailVerified: boolean;
  phoneVerified: boolean;
  setEmailVerified: (v: boolean) => void;
  setPhoneVerified: (v: boolean) => void;
}

const AuthContext = createContext<AuthContextType>({} as AuthContextType);

export const useAuth = () => useContext(AuthContext);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<User | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [emailVerified, setEmailVerified] = useState(false);
  const [phoneVerified, setPhoneVerified] = useState(false);

  const register = async (data: Omit<User, 'onboardingComplete'> & { password: string }) => {
    try {
      // map UI user shape to backend RegisterRequest
      const payload = {
        FullName: data.fullName,
        UserName: data.username,
        Email: data.email,
        Password: data.password,
      }

      const res = await api.accounts.register(payload)
      // res is AuthResponseDto
      api.setTokens({ AccessToken: res.AccessToken, RefreshToken: res.RefreshToken })

      // load full profile from backend
      try {
        const profile = await api.accounts.getProfile()
        setUser({
          fullName: profile.FullName,
          username: profile.UserName,
          email: profile.Email,
          phone: profile.PhoneNumber || undefined,
          role: (profile.Status as any) || undefined,
          onboardingComplete: false,
        })
        setEmailVerified(profile.EmailConfirmed)
        setPhoneVerified(Boolean(profile.PhoneNumberConfirmed))
      } catch (e) {
        // if profile fetch fails, still set minimal user from token payload
        setUser({ fullName: res.FullName, username: res.UserName, email: res.Email, onboardingComplete: false })
        setEmailVerified(false)
        setPhoneVerified(false)
      }

      // after register we auto-login the user (token already stored)
      setIsAuthenticated(true)

      return true
    } catch (e) {
      return false
    }
  };

  const login = async (_identifier: string, _password: string) => {
    try {
      const payload: any = {}
      if (_identifier.includes('@')) payload.Email = _identifier
      else payload.UserName = _identifier
      payload.Password = _password

      const res = await api.accounts.login(payload)
      // res is AuthResponseDto
      api.setTokens({ AccessToken: res.AccessToken, RefreshToken: res.RefreshToken })
      // load full profile from backend
      try {
        const profile = await api.accounts.getProfile()
        setUser({
          fullName: profile.FullName,
          username: profile.UserName,
          email: profile.Email,
          phone: profile.PhoneNumber || undefined,
          role: (profile.Status as any) || 'team-leader', // fallback mapping; UI role enums are different, may adjust
          onboardingComplete: true,
        })
        setEmailVerified(profile.EmailConfirmed)
        setPhoneVerified(Boolean(profile.PhoneNumberConfirmed))
      } catch (e) {
        // if profile fetch fails, still set minimal user from token payload
        setUser({ fullName: res.FullName, username: res.UserName, email: res.Email, onboardingComplete: true, role: 'team-leader' })
      }
      setIsAuthenticated(true)
      setEmailVerified(true)
      setPhoneVerified(true)
      return true
    } catch (e) {
      return false
    }
  };

  // On mount: if tokens exist try to load profile
  useEffect(() => {
    const tryLoad = async () => {
      const { accessToken } = api.getTokens()
      if (!accessToken) return
      try {
        const profile = await api.accounts.getProfile()
        setUser({
          fullName: profile.FullName,
          username: profile.UserName,
          email: profile.Email,
          phone: profile.PhoneNumber || undefined,
          role: (profile.Status as any) || 'team-leader',
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
      }
    }
    tryLoad()
  }, [])

  const logout = () => {
    api.clearTokens()
    setUser(null);
    setIsAuthenticated(false);
    setEmailVerified(false);
    setPhoneVerified(false);
  };

  const setUserRole = (role: UserRole) => {
    if (user) setUser({ ...user, role });
  };

  const completeOnboarding = async (answers: Record<string, string>) => {
    try {
      // transform flat answers (questionId->optionId) into backend shape
      const payload: any = { Profession: 1, JobTitle: null, Answers: [] }
      Object.entries(answers).forEach(([k, v]) => {
        if (k.toLowerCase() === 'role' || k.toLowerCase() === 'profession') {
          payload.Profession = Number(v) || 1
        } else {
          payload.Answers.push({ QuestionId: k, OptionId: String(v) })
        }
      })
      await api.onboarding.complete(payload)
      if (user) setUser({ ...user, onboardingComplete: true, onboardingAnswers: answers })
      setIsAuthenticated(true)
      return true
    } catch (e) {
      return false
    }
  };

  const verifyEmail = async (code: string) => {
    if (code.length !== 6) return false
    try {
      if (!user?.email) return false
      await api.accounts.verifyEmail({ Email: user.email, VerificationCode: code })
      setEmailVerified(true)
      return true
    } catch (e) {
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
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};
