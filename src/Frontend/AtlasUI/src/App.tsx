import { Toaster } from "@/components/ui/toaster";
import { Toaster as Sonner } from "@/components/ui/sonner";
import { TooltipProvider } from "@/components/ui/tooltip";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { AuthProvider, useAuth, UserRole, professionStringToRole } from "@/context/AuthContext";
import { ThemeProvider } from "@/context/ThemeContext";
import Login from "./pages/auth/Login";
import Register from "./pages/auth/Register";
import ForgotPassword from "./pages/auth/ForgotPassword";
import ResetPassword from "./pages/auth/ResetPassword";
import VerifyEmail from "./pages/auth/VerifyEmail";
import VerifyPhone from "./pages/auth/VerifyPhone";
import Onboarding from "./pages/onboarding/Onboarding";
import Dashboard from "./pages/Dashboard";
import NotFound from "./pages/NotFound";
import OAuthCallback from "./pages/auth/OAuthCallback";
import { Loader2 } from "lucide-react";
import { profileApi } from "@/services/api";
import { useState, useEffect } from "react";

const queryClient = new QueryClient();

/** Role → Dashboard route mapping */
const roleToDashboardPath: Record<UserRole, string> = {
  developer: "/developer",
  designer: "/designer",
  cybersecurity: "/secops",
  marketer: "/marketer",
  "team-leader": "/leader",
};

/**
 * PublicRoute — login / register kimi səhifələr.
 * YALNIZ tam authenticated + onboarding bitmiş user-ları redirect edir.
 * Onboarding bitməmişsə login/register-ə giriş icazə verilir (logout edə bilsin).
 */
const PublicRoute = ({ children }: { children: React.ReactNode }) => {
  const { isAuthenticated, isLoading, user } = useAuth();
  if (isLoading) return <FullLoader />;
  if (isAuthenticated && user?.onboardingComplete) {
    return <Navigate to="/dashboard" replace />;
  }
  return <>{children}</>;
};

/**
 * ProtectedRoute — dashboard səhifələri.
 * Authenticated + onboarding complete tələb edir.
 */
const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const { isAuthenticated, isLoading, user } = useAuth();
  if (isLoading) return <FullLoader />;
  if (!isAuthenticated) return <Navigate to="/login" replace />;
  if (!user?.onboardingComplete) return <Navigate to="/onboarding" replace />;
  return <>{children}</>;
};

/**
 * OnboardingRoute — onboarding səhifəsi.
 * Token/user olmalıdır. Artıq tamamlanıbsa dashboard-a redirect edir.
 */
const OnboardingRoute = ({ children }: { children: React.ReactNode }) => {
  const { isAuthenticated, isLoading, user } = useAuth();
  if (isLoading) return <FullLoader />;
  // Heç bir session yoxdur — login-ə
  if (!isAuthenticated && !user) return <Navigate to="/login" replace />;
  // Artıq tamamlanıb — dashboard-a
  if (isAuthenticated && user?.onboardingComplete) return <Navigate to="/dashboard" replace />;
  return <>{children}</>;
};

/** RoleRouter — redirect /dashboard to role-specific path */
const RoleRouter = () => {
  const { user, isLoading, setUserRole } = useAuth();
  const [fetching, setFetching] = useState(false);
  const [resolved, setResolved] = useState(false);

  useEffect(() => {
    // If user exists but has no role, try to fetch it from profiles/me
    if (user && !user.role && !fetching && !resolved) {
      setFetching(true);
      profileApi.getMe().then((res) => {
        if (res.data.isSuccess && res.data.data?.profession) {
          const lower = String(res.data.data.profession).toLowerCase().replace(/[\s_-]/g, "");
          const role = professionStringToRole[lower];
          if (role) {
            setUserRole(role);
          }
        }
      }).catch(() => {}).finally(() => {
        setFetching(false);
        setResolved(true);
      });
    }
  }, [user, fetching, resolved, setUserRole]);

  if (isLoading || fetching) return <FullLoader />;
  if (!user) return <Navigate to="/login" replace />;
  if (!user.onboardingComplete) return <Navigate to="/onboarding" replace />;

  const path = user.role ? roleToDashboardPath[user.role] : "/developer";
  return <Navigate to={path} replace />;
};

const FullLoader = () => (
  <div className="min-h-screen flex items-center justify-center bg-background">
    <div className="flex flex-col items-center gap-3">
      <Loader2 className="w-8 h-8 animate-spin text-primary" />
      <span className="text-sm text-muted-foreground">Loading...</span>
    </div>
  </div>
);

const AppRoutes = () => (
  <Routes>
    <Route path="/" element={<Navigate to="/login" replace />} />
    <Route path="/login" element={<PublicRoute><Login /></PublicRoute>} />
    <Route path="/register" element={<PublicRoute><Register /></PublicRoute>} />
    <Route path="/forgot-password" element={<ForgotPassword />} />
    <Route path="/reset-password" element={<ResetPassword />} />
    <Route path="/verify-email" element={<VerifyEmail />} />
    <Route path="/verify-phone" element={<VerifyPhone />} />
    <Route path="/auth/callback" element={<OAuthCallback />} />
    <Route path="/auth/callback/:provider" element={<OAuthCallback />} />
    <Route path="/onboarding" element={<OnboardingRoute><Onboarding /></OnboardingRoute>} />

    {/* RoleRouter — redirects /dashboard to role-specific path */}
    <Route path="/dashboard" element={<ProtectedRoute><RoleRouter /></ProtectedRoute>} />

    {/* Role-specific dashboard routes */}
    <Route path="/developer/*" element={<ProtectedRoute><Dashboard /></ProtectedRoute>} />
    <Route path="/designer/*" element={<ProtectedRoute><Dashboard /></ProtectedRoute>} />
    <Route path="/secops/*" element={<ProtectedRoute><Dashboard /></ProtectedRoute>} />
    <Route path="/marketer/*" element={<ProtectedRoute><Dashboard /></ProtectedRoute>} />
    <Route path="/leader/*" element={<ProtectedRoute><Dashboard /></ProtectedRoute>} />

    <Route path="*" element={<NotFound />} />
  </Routes>
);

const App = () => (
  <QueryClientProvider client={queryClient}>
    <TooltipProvider>
      <Toaster />
      <Sonner />
      <AuthProvider>
        <ThemeProvider>
          <BrowserRouter>
            <AppRoutes />
          </BrowserRouter>
        </ThemeProvider>
      </AuthProvider>
    </TooltipProvider>
  </QueryClientProvider>
);

export default App;
