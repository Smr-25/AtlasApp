import { createContext, useContext, useState, ReactNode } from "react";

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
  register: (data: Omit<User, "onboardingComplete">) => void;
  login: (identifier: string, password: string) => boolean;
  logout: () => void;
  setUserRole: (role: UserRole) => void;
  completeOnboarding: (answers: Record<string, string>) => void;
  verifyEmail: (code: string) => boolean;
  verifyPhone: (code: string) => boolean;
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

  const register = (data: Omit<User, "onboardingComplete">) => {
    setUser({ ...data, onboardingComplete: false });
  };

  const login = (_identifier: string, _password: string) => {
    // Mock login - accept any credentials
    setUser({
      fullName: "Oliver Smith",
      username: _identifier,
      email: _identifier.includes("@") ? _identifier : `${_identifier}@momentum.io`,
      onboardingComplete: true,
      role: "team-leader",
    });
    setIsAuthenticated(true);
    setEmailVerified(true);
    setPhoneVerified(true);
    return true;
  };

  const logout = () => {
    setUser(null);
    setIsAuthenticated(false);
    setEmailVerified(false);
    setPhoneVerified(false);
  };

  const setUserRole = (role: UserRole) => {
    if (user) setUser({ ...user, role });
  };

  const completeOnboarding = (answers: Record<string, string>) => {
    if (user) {
      setUser({ ...user, onboardingComplete: true, onboardingAnswers: answers });
      setIsAuthenticated(true);
    }
  };

  const verifyEmail = (code: string) => {
    if (code.length === 6) {
      setEmailVerified(true);
      return true;
    }
    return false;
  };

  const verifyPhone = (code: string) => {
    if (code.length === 6) {
      setPhoneVerified(true);
      return true;
    }
    return false;
  };

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
