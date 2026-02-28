import { Toaster } from "@/components/ui/toaster";
import { Toaster as Sonner } from "@/components/ui/sonner";
import { TooltipProvider } from "@/components/ui/tooltip";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import React, { lazy, Suspense } from "react";
import { AuthProvider } from '@/context/AuthContext';
import { ThemeProvider } from '@/context/ThemeContext';
import { SignalRProvider } from '@/context/SignalRContext';
import { RoleProvider } from '@/context/RoleContext'
import NotFound from "./pages/NotFound";
import Register from "./pages/auth/Register";
import Login from "./pages/auth/Login";
import ForgotPassword from "./pages/auth/ForgotPassword";
import ResetPassword from "./pages/auth/ResetPassword";
import VerifyEmail from "./pages/auth/VerifyEmail";
import VerifyPhone from "./pages/auth/VerifyPhone";
import Onboarding from "./pages/onboarding/Onboarding";
import RequireAuth from "@/components/auth/RequireAuth";
import RoleRouter from "@/components/auth/RoleRouter";
import Workspaces from "./pages/workspaces/Workspaces";
import Integrations from "./pages/integrations/Integrations";
import GitHubCallback from "./pages/auth/GitHubCallback";
import { GoogleOAuthProvider } from '@react-oauth/google'
import Welcome from "./pages/Welcome";
import ErrorBoundary from "@/components/ErrorBoundary";

// Lazy-load role dashboards for optimal bundle splitting
const LeaderDashboard: React.LazyExoticComponent<React.FC> = lazy(() => import("./pages/leader/LeaderDashboard").then(m => ({ default: m.default as React.FC })));
const DeveloperDashboard: React.LazyExoticComponent<React.FC> = lazy(() => import("./pages/developer/DeveloperDashboard").then(m => ({ default: m.default as React.FC })));
const DesignerDashboard: React.LazyExoticComponent<React.FC> = lazy(() => import("./pages/designer/DesignerDashboard").then(m => ({ default: m.default as React.FC })));
const SecOpsDashboard: React.LazyExoticComponent<React.FC> = lazy(() => import("./pages/secops/SecOpsDashboard").then(m => ({ default: m.default as React.FC })));
const MarketerDashboard: React.LazyExoticComponent<React.FC> = lazy(() => import("./pages/marketer/MarketerDashboard").then(m => ({ default: m.default as React.FC })));
const SubscriptionPage: React.LazyExoticComponent<React.FC> = lazy(() => import("./pages/subscription/SubscriptionPage").then(m => ({ default: m.default as React.FC })));

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: 1,
    },
  },
})

const googleClientId = (import.meta.env.VITE_GOOGLE_CLIENT_ID as string) || ''

function AppRoutes() {
  return (
    <Suspense fallback={
      <div className="flex items-center justify-center h-screen bg-background">
        <div className="w-8 h-8 rounded-full border-2 border-primary border-t-transparent animate-spin" />
      </div>
    }>
      <Routes>
      {/* Show login page at root so app opens to sign-in by default */}
      <Route path="/" element={<Login />} />
      <Route path="/register" element={<Register />} />
      <Route path="/login" element={<Login />} />
      <Route path="/forgot-password" element={<ForgotPassword />} />
      <Route path="/reset-password" element={<ResetPassword />} />
      <Route path="/verify-email" element={<VerifyEmail />} />
      <Route path="/verify-phone" element={<VerifyPhone />} />
      <Route path="/onboarding" element={<Onboarding />} />
      <Route path="/auth/github/callback" element={<GitHubCallback />} />

      {/* Role router — redirects to correct dashboard */}
      <Route path="/dashboard" element={<RequireAuth><RoleRouter /></RequireAuth>} />

      {/* Role-specific dashboards */}
      <Route path="/leader/*" element={<RequireAuth><LeaderDashboard /></RequireAuth>} />
      <Route path="/developer/*" element={<RequireAuth><DeveloperDashboard /></RequireAuth>} />
      <Route path="/designer/*" element={<RequireAuth><DesignerDashboard /></RequireAuth>} />
      <Route path="/secops/*" element={<RequireAuth><SecOpsDashboard /></RequireAuth>} />
      <Route path="/marketer/*" element={<RequireAuth><MarketerDashboard /></RequireAuth>} />

      {/* Common pages */}
      <Route path="/workspaces" element={<RequireAuth><Workspaces /></RequireAuth>} />
      <Route path="/integrations" element={<RequireAuth><Integrations /></RequireAuth>} />
      <Route path="/subscription" element={<RequireAuth><SubscriptionPage /></RequireAuth>} />

      <Route path="*" element={<NotFound />} />
      </Routes>
    </Suspense>
  )
}

const App = () => {
  const inner = (
    <AuthProvider>
      <ThemeProvider>
        <SignalRProvider>
          <ErrorBoundary>
            <RoleProvider>
              <BrowserRouter>
                <AppRoutes />
              </BrowserRouter>
            </RoleProvider>
          </ErrorBoundary>
        </SignalRProvider>
      </ThemeProvider>
    </AuthProvider>
  )

  return (
    <QueryClientProvider client={queryClient}>
      <TooltipProvider>
        <Toaster />
        <Sonner />
        {googleClientId ? (
          <GoogleOAuthProvider clientId={googleClientId}>
            {inner}
          </GoogleOAuthProvider>
        ) : (
          inner
        )}
      </TooltipProvider>
    </QueryClientProvider>
  )
}

export default App;
