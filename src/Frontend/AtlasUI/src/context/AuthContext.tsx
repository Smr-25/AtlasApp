import {
  createContext,
  useContext,
  useState,
  useEffect,
  useCallback,
  ReactNode,
} from "react";
import {
  authApi,
  onboardingApi,
  profileApi,
  TokenService,
  AuthResponseDto,
  ExternalLoginResponseDto,
  AccountDto,
} from "@/services/api";
import { AxiosError } from "axios";

// ─── Types ──────────────────────────────────────────────────────────
export type UserRole =
  | "developer"
  | "designer"
  | "cybersecurity"
  | "team-leader";

/** Maps backend enum (UserProfession) → frontend role key */
export const professionToRole: Record<number, UserRole> = {
  0: "developer",
  1: "designer",
  2: "cybersecurity",
  4: "team-leader",
};

/** Maps backend string profession → frontend role key */
export const professionStringToRole: Record<string, UserRole> = {
  developer: "developer",
  designer: "designer",
  cybersecurity: "cybersecurity",
  productmanager: "team-leader",
  // Also handle role names returned by login endpoint
  secops: "cybersecurity",
  teamleader: "team-leader",
};

export const roleToProfession: Record<UserRole, number> = {
  developer: 0,
  designer: 1,
  cybersecurity: 2,
  "team-leader": 4,
};

export interface User {
  userId: string;
  fullName: string;
  userName: string;
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
  isLoading: boolean;

  register: (data: {
    fullName: string;
    userName: string;
    email: string;
    password: string;
  }) => Promise<string[]>;

  login: (identifier: string, password: string) => Promise<string[]>;
  externalLogin: (
    provider: string,
    accessToken: string,
    authorizationCode: string
  ) => Promise<{ errors: string[]; isNewUser?: boolean }>;
  finalizeAuthFromTokens: (tokens: {
    AccessToken: string;
    RefreshToken: string;
  }, isNewUser?: boolean) => Promise<void>;
  logout: () => Promise<void>;

  setUserRole: (role: UserRole) => void;
  completeOnboarding: (
    answers: Array<{
      questionId: string;
      optionId: string;
      customValue?: string;
    }>
  ) => Promise<string[]>;

  forgotPassword: (email: string) => Promise<string[]>;
  verifyResetCode: (email: string, code: string) => Promise<{ errors: string[]; resetToken?: string }>;
  resetPassword: (
    email: string,
    resetToken: string,
    newPassword: string,
    confirmPassword: string
  ) => Promise<string[]>;

  verifyEmail: (code: string) => Promise<string[]>;
  verifyPhone: (code: string) => Promise<string[]>;
  resendEmailCode: () => Promise<string[]>;
  resendPhoneCode: () => Promise<string[]>;

  emailVerified: boolean;
  phoneVerified: boolean;
  setEmailVerified: (v: boolean) => void;
  setPhoneVerified: (v: boolean) => void;

  /** Temporary email kept between pages (forgot password flow) */
  tempEmail: string;
  setTempEmail: (e: string) => void;
}

// ─── Helper: extract errors from API or Axios ───────────────────────
function extractErrors(err: unknown): string[] {
  if (err instanceof AxiosError) {
    const body = err.response?.data as
      | { errors?: string[] | Record<string, string[]>; isSuccess?: boolean }
      | undefined;

    if (body?.errors) {
      // Standard ResponseModel format: errors is string[]
      if (Array.isArray(body.errors) && body.errors.length > 0) {
        return body.errors;
      }
      // ASP.NET ModelState validation format: errors is { FieldName: ["msg"] }
      if (typeof body.errors === "object" && !Array.isArray(body.errors)) {
        const msgs: string[] = [];
        for (const field of Object.values(body.errors)) {
          if (Array.isArray(field)) msgs.push(...field);
        }
        if (msgs.length > 0) return msgs;
      }
    }

    if (err.response?.status === 429)
      return ["Too many requests. Please try again later."];

    if (err.response?.status === 401)
      return ["Invalid username or password. Please try again."];

    if (err.response?.status === 409)
      return ["This email or username is already in use."];

    if (err.response?.status === 400)
      return ["Invalid request. Please check your inputs."];

    if (err.response?.status === 404)
      return ["Service not found. Please try again later."];

    return [err.message || "Network error"];
  }
  if (err instanceof Error) return [err.message];
  return ["An unexpected error occurred"];
}

// ─── Context ────────────────────────────────────────────────────────
const AuthContext = createContext<AuthContextType>({} as AuthContextType);

export const useAuth = () => useContext(AuthContext);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<User | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [emailVerified, setEmailVerified] = useState(false);
  const [phoneVerified, setPhoneVerified] = useState(false);
  const [tempEmail, setTempEmail] = useState("");

  // ── Restore session on mount ────────────────────────────────────
  useEffect(() => {
    const restoreSession = async () => {
      const stored = TokenService.getUser();
      const token = TokenService.getAccessToken();
      if (stored && token) {
        const storedUser = stored as User;

        // Validate session by fetching profile
        try {
          const profileRes = await authApi.getProfile();
          if (profileRes.data.isSuccess && profileRes.data.data) {
            const p = profileRes.data.data;
            let role = p.profession
              ? professionToRole[p.profession]
              : storedUser.role;

            // If still no role, try /api/profiles/me
            if (!role) {
              try {
                const profMeRes = await profileApi.getMe();
                if (profMeRes.data.isSuccess && profMeRes.data.data?.profession) {
                  const lower = String(profMeRes.data.data.profession).toLowerCase().replace(/[\s_-]/g, "");
                  role = professionStringToRole[lower] || storedUser.role;
                }
              } catch {
                role = storedUser.role;
              }
            }

            // Profile fetch successful = active user
            // Onboarding is complete if user has a role (profession set) or was previously marked complete
            const onboardingComplete = !!role || storedUser.onboardingComplete;
            const u: User = {
              userId: p.id || storedUser.userId,
              fullName: p.fullName || storedUser.fullName,
              userName: p.userName || storedUser.userName,
              email: p.email || storedUser.email,
              phone: p.phoneNumber || undefined,
              role,
              onboardingComplete,
            };
            setUser(u);
            setIsAuthenticated(true);
            setEmailVerified(p.emailConfirmed);
            setPhoneVerified(p.phoneNumberConfirmed);
          } else {
            // Profile not successful — don't authenticate
            // Keep tokens in case user needs to verify email
            setUser(storedUser);
          }
        } catch {
          // Profile fetch failed (401 = email not verified, or network error)
          // Keep user info for verify-email page but don't mark as authenticated
          setUser(storedUser);
          setTempEmail(storedUser.email);
        }
      }
      setIsLoading(false);
    };
    restoreSession();
  }, []);

  // ── Persist user whenever it changes ─────────────────────────────
  useEffect(() => {
    if (user) {
      TokenService.saveUser(user as unknown as Record<string, unknown>);
    }
  }, [user]);

  // ── Auth helpers ────────────────────────────────────────────────
  const handleAuthResponse = useCallback(
    (dto: AuthResponseDto, onboardingDone = false) => {
      TokenService.setTokens(dto);
      const u: User = {
        userId: dto.userId,
        fullName: dto.fullName,
        userName: dto.userName,
        email: dto.email,
        onboardingComplete: onboardingDone,
      };
      setUser(u);
      setIsAuthenticated(true);
      return u;
    },
    []
  );

  // ── REGISTER ────────────────────────────────────────────────────
  const register = async (data: {
    fullName: string;
    userName: string;
    email: string;
    password: string;
  }): Promise<string[]> => {
    try {
      setIsLoading(true);
      const res = await authApi.register(data);
      if (res.data.isSuccess && res.data.data) {
        const dto = res.data.data;
        TokenService.setTokens(dto);
        const u: User = {
          userId: dto.userId,
          fullName: dto.fullName,
          userName: dto.userName,
          email: dto.email,
          onboardingComplete: false,
        };
        setUser(u);
        setTempEmail(data.email);
        // Don't set isAuthenticated yet — needs email verification + onboarding
        return [];
      }
      return res.data.errors || ["Registration failed"];
    } catch (err) {
      return extractErrors(err);
    } finally {
      setIsLoading(false);
    }
  };

  // ── Helper: resolve role from profession (number or string) ───────
  const resolveRole = useCallback(
    (profession: number | string | null | undefined): UserRole | undefined => {
      if (!profession) return undefined;
      if (typeof profession === "number") {
        return professionToRole[profession];
      }
      // String profession from /api/profiles/me
      const lower = String(profession).toLowerCase().replace(/[\s_-]/g, "");
      return professionStringToRole[lower] || undefined;
    },
    []
  );

  // ── Helper: build user from profile ──────────────────────────────
  const buildUserFromProfile = useCallback(
    (profile: AccountDto, fallbackRole?: UserRole): User => {
      const role = resolveRole(profile.profession) || fallbackRole;
      // Onboarding is complete if profession is set (bio may still be null/generating)
      const onboardingComplete = !!profile.profession || !!role;
      return {
        userId: profile.id,
        fullName: profile.fullName,
        userName: profile.userName || "",
        email: profile.email,
        phone: profile.phoneNumber || undefined,
        role,
        onboardingComplete,
      };
    },
    [resolveRole]
  );

  // ── Helper: fetch role from /api/profiles/me ─────────────────────
  const fetchRoleFromProfilesMe = useCallback(async (): Promise<UserRole | undefined> => {
    try {
      const res = await profileApi.getMe();
      if (res.data.isSuccess && res.data.data) {
        const prof = res.data.data;
        return resolveRole(prof.profession) || undefined;
      }
    } catch {
      // profiles/me may not be available
    }
    return undefined;
  }, [resolveRole]);

  // ── LOGIN ───────────────────────────────────────────────────────
  const login = async (
    identifier: string,
    password: string
  ): Promise<string[]> => {
    try {
      setIsLoading(true);
      const isEmail = identifier.includes("@");
      const res = await authApi.login({
        email: isEmail ? identifier : null,
        userName: isEmail ? null : identifier,
        password,
      });
      if (res.data.isSuccess && res.data.data) {
        const dto = res.data.data;
        TokenService.setTokens(dto);

        // Try to get role directly from login response first (backend returns it)
        let roleFromLogin = dto.role ? resolveRole(dto.role) : undefined;

        // Fetch full profile for complete user data
        try {
          const profileRes = await authApi.getProfile();
          if (profileRes.data.isSuccess && profileRes.data.data) {
            // If login didn't return role, try from profile
            if (!roleFromLogin) {
              roleFromLogin = resolveRole(profileRes.data.data.profession);
            }
            // If still no role, try /api/profiles/me
            if (!roleFromLogin) {
              roleFromLogin = await fetchRoleFromProfilesMe();
            }

            const u = buildUserFromProfile(profileRes.data.data, roleFromLogin);
            // Login success = user already passed onboarding
            u.onboardingComplete = true;
            setUser(u);
            setIsAuthenticated(true);
            setEmailVerified(true);
            setPhoneVerified(profileRes.data.data.phoneNumberConfirmed);
            return [];
          }
        } catch {
          // Profile fetch failed — use token data + role from login
        }

        // Fallback: use auth response data — login success means onboarding is done
        const u = handleAuthResponse(dto, true);
        if (roleFromLogin && u) {
          setUser(prev => prev ? { ...prev, role: roleFromLogin } : prev);
        }
        setEmailVerified(true);
        setPhoneVerified(true);
        return [];
      }
      return res.data.errors || ["Login failed"];
    } catch (err) {
      // Detect "Email not verified" 401 response
      if (err instanceof AxiosError && err.response?.status === 401) {
        const body = err.response?.data as
          | { errors?: string[]; isSuccess?: boolean }
          | undefined;
        const msgs = body?.errors || [];
        const notVerified = msgs.some(
          (m) => m.toLowerCase().includes("email") && m.toLowerCase().includes("not verified")
        );
        if (notVerified) {
          setTempEmail(identifier);
          return ["EMAIL_NOT_VERIFIED"];
        }
        // Check if backend sent specific error messages
        if (msgs.length > 0) {
          return msgs;
        }
        // Generic invalid credentials
        return ["Invalid username or password. Please check your credentials and try again."];
      }
      return extractErrors(err);
    } finally {
      setIsLoading(false);
    }
  };

  // ── EXTERNAL LOGIN ──────────────────────────────────────────────
  const externalLogin = async (
    provider: string,
    accessToken: string,
    authorizationCode: string
  ): Promise<{ errors: string[]; isNewUser?: boolean }> => {
    try {
      setIsLoading(true);
      const res = await authApi.externalLogin({
        provider,
        idToken: "",
        accessToken,
        authorizationCode,
      });
      if (res.data.isSuccess && res.data.data) {
        const dto = res.data.data as ExternalLoginResponseDto;
        TokenService.setTokens({
          accessToken: dto.accessToken,
          refreshToken: dto.refreshToken,
          refreshTokenExpiration: dto.refreshTokenExpiration,
          userId: dto.userId,
        });
        const u: User = {
          userId: dto.userId,
          fullName: dto.fullName,
          userName: dto.email.split("@")[0],
          email: dto.email,
          onboardingComplete: !dto.isNewUser,
        };

        if (!dto.isNewUser) {
          // Existing user — try to get role from profile
          try {
            const profMeRes = await profileApi.getMe();
            if (profMeRes.data.isSuccess && profMeRes.data.data?.profession) {
              const lower = String(profMeRes.data.data.profession).toLowerCase().replace(/[\s_-]/g, "");
              u.role = professionStringToRole[lower] || undefined;
            }
          } catch {
            // profiles/me not available
          }
          setIsAuthenticated(true);
          setEmailVerified(true);
        }
        setUser(u);
        return { errors: [], isNewUser: dto.isNewUser };
      }
      return {
        errors: res.data.errors || ["External login failed"],
      };
    } catch (err) {
      return { errors: extractErrors(err) };
    } finally {
      setIsLoading(false);
    }
  };

  // ── FINALIZE AUTH FROM TOKENS (OAuth callback redirect flow) ────
  const finalizeAuthFromTokens = useCallback(
    async (tokens: { AccessToken: string; RefreshToken: string }, isNewUser?: boolean) => {
      TokenService.setTokens({
        accessToken: tokens.AccessToken,
        refreshToken: tokens.RefreshToken,
        refreshTokenExpiration: "",
        userId: "",
      });

      // New user — skip profile fetch, needs onboarding
      if (isNewUser) {
        const u: User = {
          userId: "",
          fullName: "",
          userName: "",
          email: "",
          onboardingComplete: false,
        };
        setUser(u);
        setIsAuthenticated(true);
        setEmailVerified(true);
        return;
      }

      // Existing user — fetch profile to get full info
      try {
        const profileRes = await authApi.getProfile();
        if (profileRes.data.isSuccess && profileRes.data.data) {
          const p = profileRes.data.data;
          let role = p.profession
            ? professionToRole[p.profession]
            : undefined;

          // If accounts/profile doesn't have profession, try /api/profiles/me
          if (!role) {
            try {
              const profMeRes = await profileApi.getMe();
              if (profMeRes.data.isSuccess && profMeRes.data.data?.profession) {
                const lower = String(profMeRes.data.data.profession).toLowerCase().replace(/[\s_-]/g, "");
                role = professionStringToRole[lower] || undefined;
              }
            } catch {
              // profiles/me not available
            }
          }

          // Existing OAuth user (isNewUser=false) = onboarding already done
          const u: User = {
            userId: p.id || "",
            fullName: p.fullName || "",
            userName: p.userName || "",
            email: p.email || "",
            phone: p.phoneNumber || undefined,
            role,
            onboardingComplete: true,
          };
          setUser(u);
          setIsAuthenticated(true);
          setEmailVerified(p.emailConfirmed);
          setPhoneVerified(p.phoneNumberConfirmed);
          return;
        }
      } catch {
        // Profile fetch failed — set minimal user from token
      }
      // Fallback: existing user — set onboarding complete
      setIsAuthenticated(true);
      setUser({ userId: "", fullName: "", userName: "", email: "", onboardingComplete: true });
    },
    []
  );

  // ── LOGOUT ──────────────────────────────────────────────────────
  const logout = async () => {
    try {
      const rt = TokenService.getRefreshToken();
      if (rt) await authApi.logout(rt).catch(() => {});
    } finally {
      TokenService.clear();
      setUser(null);
      setIsAuthenticated(false);
      setEmailVerified(false);
      setPhoneVerified(false);
    }
  };

  // ── VERIFY EMAIL ────────────────────────────────────────────────
  const verifyEmailFn = async (code: string): Promise<string[]> => {
    try {
      setIsLoading(true);
      const email = user?.email || tempEmail;
      if (!email) return ["Email not found"];
      const res = await authApi.verifyEmail(email, code);
      if (res.data.isSuccess) {
        setEmailVerified(true);
        // After email verification, user should be authenticated to proceed to onboarding
        setIsAuthenticated(true);
        return [];
      }
      return res.data.errors || ["Verification failed"];
    } catch (err) {
      return extractErrors(err);
    } finally {
      setIsLoading(false);
    }
  };

  // ── VERIFY PHONE ────────────────────────────────────────────────
  const verifyPhoneFn = async (code: string): Promise<string[]> => {
    try {
      setIsLoading(true);
      const phone = user?.phone;
      if (!phone) return ["Phone number not found"];
      const res = await authApi.verifyPhone(phone, code);
      if (res.data.isSuccess) {
        setPhoneVerified(true);
        return [];
      }
      return res.data.errors || ["Verification failed"];
    } catch (err) {
      return extractErrors(err);
    } finally {
      setIsLoading(false);
    }
  };

  // ── RESEND CODES ────────────────────────────────────────────────
  const resendEmailCode = async (): Promise<string[]> => {
    try {
      const email = user?.email || tempEmail;
      if (!email) return ["Email not found"];
      const res = await authApi.resendEmailVerificationCode(email);
      if (res.data.isSuccess) return [];
      return res.data.errors || ["Failed to resend code"];
    } catch (err) {
      return extractErrors(err);
    }
  };

  const resendPhoneCode = async (): Promise<string[]> => {
    try {
      const phone = user?.phone;
      if (!phone) return ["Phone number not found"];
      const res = await authApi.resendPhoneVerificationCode(phone);
      if (res.data.isSuccess) return [];
      return res.data.errors || ["Failed to resend code"];
    } catch (err) {
      return extractErrors(err);
    }
  };

  // ── FORGOT / RESET PASSWORD ─────────────────────────────────────
  const forgotPassword = async (email: string): Promise<string[]> => {
    try {
      setIsLoading(true);
      const res = await authApi.forgotPassword(email);
      if (res.data.isSuccess) {
        setTempEmail(email);
        return [];
      }
      return res.data.errors || ["Failed to send reset code"];
    } catch (err) {
      return extractErrors(err);
    } finally {
      setIsLoading(false);
    }
  };

  const verifyResetCode = async (
    email: string,
    code: string
  ): Promise<{ errors: string[]; resetToken?: string }> => {
    try {
      setIsLoading(true);
      const res = await authApi.verifyResetCode(email, code);
      if (res.data.isSuccess && res.data.data) {
        return { errors: [], resetToken: res.data.data.resetToken };
      }
      return { errors: res.data.errors || ["Invalid reset code"] };
    } catch (err) {
      return { errors: extractErrors(err) };
    } finally {
      setIsLoading(false);
    }
  };

  const resetPasswordFn = async (
    email: string,
    resetToken: string,
    newPassword: string,
    confirmPassword: string
  ): Promise<string[]> => {
    try {
      setIsLoading(true);
      const res = await authApi.resetPassword(email, resetToken, newPassword, confirmPassword);
      if (res.data.isSuccess) return [];
      return res.data.errors || ["Failed to reset password"];
    } catch (err) {
      return extractErrors(err);
    } finally {
      setIsLoading(false);
    }
  };

  // ── ROLE / ONBOARDING ──────────────────────────────────────────
  const setUserRole = (role: UserRole) => {
    if (user) setUser({ ...user, role });
  };

  const completeOnboarding = async (
    answers: Array<{
      questionId: string;
      optionId: string;
      customValue?: string;
    }>
  ): Promise<string[]> => {
    try {
      setIsLoading(true);
      // Map frontend role key to backend profession enum
      const profession = roleToProfession[user?.role || "developer"] ?? 0;
      const jobTitle = user?.role === "designer" ? "UI/UX Designer"
        : user?.role === "cybersecurity" ? "Security Engineer"
        : user?.role === "team-leader" ? "Product Manager"
        : "Software Developer";

      const res = await onboardingApi.complete({ profession, jobTitle, answers });
      if (res.data.isSuccess) {
        // After onboarding, fetch profile to get the assigned role
        try {
          const profRes = await profileApi.getMe();
          if (profRes.data.isSuccess && profRes.data.data) {
            const role = resolveRole(profRes.data.data.profession);
            if (user) {
              setUser({ ...user, onboardingComplete: true, role: role || "developer" });
            }
          } else if (user) {
            setUser({ ...user, onboardingComplete: true });
          }
        } catch {
          if (user) {
            setUser({ ...user, onboardingComplete: true });
          }
        }
        setIsAuthenticated(true);
        return [];
      }
      return res.data.errors || ["Onboarding failed"];
    } catch (err) {
      return extractErrors(err);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated,
        isLoading,
        register,
        login,
        externalLogin,
        finalizeAuthFromTokens,
        logout,
        setUserRole,
        completeOnboarding,
        verifyEmail: verifyEmailFn,
        verifyPhone: verifyPhoneFn,
        resendEmailCode,
        resendPhoneCode,
        forgotPassword,
        verifyResetCode,
        resetPassword: resetPasswordFn,
        emailVerified,
        phoneVerified,
        setEmailVerified,
        setPhoneVerified,
        tempEmail,
        setTempEmail,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};
