import { Toaster } from "@/components/ui/toaster";
import { Toaster as Sonner } from "@/components/ui/sonner";
import { TooltipProvider } from "@/components/ui/tooltip";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { AuthProvider } from "@/context/AuthContext";
import { ThemeProvider } from "@/context/ThemeContext";
import NotFound from "./pages/NotFound";
import Register from "./pages/auth/Register";
import Login from "./pages/auth/Login";
import ForgotPassword from "./pages/auth/ForgotPassword";
import ResetPassword from "./pages/auth/ResetPassword";
import VerifyEmail from "./pages/auth/VerifyEmail";
import VerifyPhone from "./pages/auth/VerifyPhone";
import Onboarding from "./pages/onboarding/Onboarding";
import RequireAuth from "@/components/auth/RequireAuth";
import Workspaces from "./pages/workspaces/Workspaces";
import Integrations from "./pages/integrations/Integrations";
import GitHubCallback from "./pages/auth/GitHubCallback";
import { GoogleOAuthProvider } from '@react-oauth/google'
import Welcome from "./pages/Welcome";
import ErrorBoundary from "@/components/ErrorBoundary";
import LeaderDashboard from "./pages/leader/LeaderDashboard";

const queryClient = new QueryClient();

const googleClientId = (import.meta.env.VITE_GOOGLE_CLIENT_ID as string) || ''

const App = () => (
  <QueryClientProvider client={queryClient}>
    <TooltipProvider>
      <Toaster />
      <Sonner />
      {googleClientId ? (
        <GoogleOAuthProvider clientId={googleClientId}>
          <AuthProvider>
            <ThemeProvider>
              <ErrorBoundary>
                <BrowserRouter>
                  <Routes>
                    {/* Root now shows a visual Welcome page which redirects to /login after 2s */}
                    <Route path="/" element={<Welcome />} />
                    <Route path="/register" element={<Register />} />
                    <Route path="/login" element={<Login />} />
                    <Route path="/forgot-password" element={<ForgotPassword />} />
                    <Route path="/reset-password" element={<ResetPassword />} />
                    <Route path="/verify-email" element={<VerifyEmail />} />
                    <Route path="/verify-phone" element={<VerifyPhone />} />
                    <Route path="/onboarding" element={<Onboarding />} />

                    {/* Leader dashboard route - requires auth */}
                    <Route path="/leader" element={<RequireAuth><LeaderDashboard /></RequireAuth>} />

                    <Route path="/workspaces" element={<RequireAuth><Workspaces /></RequireAuth>} />
                    <Route path="/integrations" element={<RequireAuth><Integrations /></RequireAuth>} />

                    <Route path="/auth/github/callback" element={<GitHubCallback />} />

                    <Route path="*" element={<NotFound />} />
                  </Routes>
                </BrowserRouter>
              </ErrorBoundary>
            </ThemeProvider>
          </AuthProvider>
        </GoogleOAuthProvider>
      ) : (
        <AuthProvider>
          <ThemeProvider>
            <ErrorBoundary>
              <BrowserRouter>
                <Routes>
                  {/* Root now shows a visual Welcome page which redirects to /login after 2s */}
                  <Route path="/" element={<Welcome />} />
                  <Route path="/register" element={<Register />} />
                  <Route path="/login" element={<Login />} />
                  <Route path="/forgot-password" element={<ForgotPassword />} />
                  <Route path="/reset-password" element={<ResetPassword />} />
                  <Route path="/verify-email" element={<VerifyEmail />} />
                  <Route path="/verify-phone" element={<VerifyPhone />} />
                  <Route path="/onboarding" element={<Onboarding />} />

                  {/* Leader dashboard route - requires auth */}
                  <Route path="/leader" element={<RequireAuth><LeaderDashboard /></RequireAuth>} />

                  <Route path="/workspaces" element={<RequireAuth><Workspaces /></RequireAuth>} />
                  <Route path="/integrations" element={<RequireAuth><Integrations /></RequireAuth>} />

                  <Route path="/auth/github/callback" element={<GitHubCallback />} />

                  <Route path="*" element={<NotFound />} />
                </Routes>
              </BrowserRouter>
            </ErrorBoundary>
          </ThemeProvider>
        </AuthProvider>
      )}
    </TooltipProvider>
  </QueryClientProvider>
);

export default App;
