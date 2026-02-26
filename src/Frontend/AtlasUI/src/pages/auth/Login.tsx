import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { motion } from "framer-motion";
import { Eye, EyeOff } from "lucide-react";
import AuthLayout from "@/components/auth/AuthLayout";
import { useAuth } from "@/context/AuthContext";
import { useToast } from '@/hooks/use-toast'
import { formatApiError } from '@/lib/errorUtils'
import GoogleSignInButton from '@/components/auth/GoogleSignInButton'

const Login = () => {
  const navigate = useNavigate();
  const { login, externalLogin } = useAuth();
  const { toast } = useToast();
  const [identifier, setIdentifier] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('')
    // Validation per API rules:
    // - At least one of Email or UserName must be provided (identifier covers both)
    // - If identifier looks like an email validate format
    // - If identifier is a username validate length 3..20
    if (!identifier.trim()) { setError('Please enter email or username'); return }
    if (!password.trim()) { setError('Please enter your password'); return }
    const isEmail = identifier.includes('@')
    if (isEmail) {
      if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(identifier)) { setError('Invalid email address'); return }
    } else {
      if (identifier.length < 3) { setError('Username must be at least 3 characters'); return }
      if (identifier.length > 20) { setError('Username must be less than 20 characters'); return }
    }

    try {
      const result = await login(identifier, password as string);
      if (result.ok) {
        toast({ title: 'Signed in', description: 'Welcome back!' })
        navigate("/");
        return
      }

      // Non-ok but no exception -> map known reasons
      if (result.reason === 'email_not_verified') {
        toast({ title: 'Email not verified', description: 'Please verify your email first' })
        navigate('/verify-email')
        return
      }
      if (result.reason === 'locked') {
        const friendly = result.message || 'Your account is temporarily locked.'
        setError(friendly)
        toast({ title: 'Account locked', description: friendly })
        return
      }

      // Generic failure
      const friendly = result.message || 'Invalid email/username or password.'
      setError(friendly)
      toast({ title: 'Sign in failed', description: friendly })

    } catch (e) {
      // Exception path (ApiError or other)
      const formatted = formatApiError(e, 'Sign in failed')
      setError(formatted.message)
      toast({ title: formatted.title, description: formatted.message })
    }
  };

  // GitHub start: redirect to authorize URL
  const startGitHub = () => {
    const clientId = import.meta.env.VITE_GITHUB_CLIENT_ID as string | undefined
    const redirectUri = `${import.meta.env.VITE_API_BASE || ''}/auth/github/callback` || `${window.location.origin}/auth/github/callback`
    if (!clientId) {
      toast({ title: 'GitHub config missing', description: 'Set VITE_GITHUB_CLIENT_ID in your env' })
      return
    }
    const url = `https://github.com/login/oauth/authorize?client_id=${encodeURIComponent(clientId)}&redirect_uri=${encodeURIComponent(redirectUri)}&scope=read:user user:email`
    // open in same tab to comply with GitHub redirect behavior
    window.location.href = url
  }

  const inputClass =
    "w-full h-11 px-4 rounded-xl bg-muted/50 border border-border text-sm text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/30 focus:border-primary/50 transition-all";
  const labelClass = "text-sm font-medium text-foreground mb-1.5 block";

  return (
    <AuthLayout title="Welcome back" subtitle="Sign in to your Momentum workspace">
      <form onSubmit={handleSubmit} className="space-y-4">
        {error && (
          <motion.div
            initial={{ opacity: 0, y: -5 }}
            animate={{ opacity: 1, y: 0 }}
            className="p-3 rounded-xl bg-destructive/10 border border-destructive/20 text-destructive text-sm"
          >
            {error}
          </motion.div>
        )}

        <div>
          <label className={labelClass}>Email or Username</label>
          <input
            type="text"
            placeholder="oliver@momentum.io"
            value={identifier}
            onChange={(e) => setIdentifier(e.target.value)}
            className={inputClass}
          />
        </div>

        <div>
          <div className="flex justify-between items-center mb-1.5">
            <label className="text-sm font-medium text-foreground">Password</label>
            <Link to="/forgot-password" className="text-xs text-primary hover:underline">
              Forgot password?
            </Link>
          </div>
          <div className="relative">
            <input
              type={showPassword ? "text" : "password"}
              placeholder="Enter your password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className={inputClass}
            />
            <button
              type="button"
              onClick={() => setShowPassword(!showPassword)}
              className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground transition-colors"
            >
              {showPassword ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
            </button>
          </div>
        </div>

        <motion.button
          whileHover={{ scale: 1.01 }}
          whileTap={{ scale: 0.99 }}
          type="submit"
          className="w-full h-11 rounded-xl bg-primary text-primary-foreground font-medium text-sm shadow-lg shadow-primary/25 hover:shadow-primary/40 transition-all"
        >
          Sign In
        </motion.button>

        <div className="flex items-center gap-4 my-2">
          <div className="flex-1 h-px bg-border" />
          <span className="text-xs text-muted-foreground">or continue with</span>
          <div className="flex-1 h-px bg-border" />
        </div>

        <div className="flex gap-3">
          {/* Only render GoogleSignInButton if env config exists (to avoid hook init errors) */}
          {import.meta.env.VITE_GOOGLE_CLIENT_ID ? (
            <GoogleSignInButton />
          ) : (
            <div className="flex-1 h-11 rounded-xl border border-border flex items-center justify-center gap-2 text-sm text-foreground opacity-60">Google not configured</div>
          )}
          <button
            type="button"
            onClick={() => startGitHub()}
            className="flex-1 h-11 rounded-xl border border-border flex items-center justify-center gap-2 text-sm text-foreground hover:bg-muted/50 transition-colors"
          >
            <svg className="w-4 h-4" viewBox="0 0 24 24" fill="currentColor">
              <path d="M12 0c-6.626 0-12 5.373-12 12 0 5.302 3.438 9.8 8.207 11.387.599.111.793-.261.793-.577v-2.234c-3.338.726-4.033-1.416-4.033-1.416-.546-1.387-1.333-1.756-1.333-1.756-1.089-.745.083-.729.083-.729 1.205.084 1.839 1.237 1.839 1.237 1.07 1.834 2.807 1.304 3.492.997.107-.775.418-1.305.762-1.604-2.665-.305-5.467-1.334-5.467-5.931 0-1.311.469-2.381 1.236-3.221-.124-.303-.535-1.524.117-3.176 0 0 1.008-.322 3.301 1.23.957-.266 1.983-.399 3.003-.404 1.02.005 2.047.138 3.006.404 2.291-1.552 3.297-1.23 3.297-1.23.653 1.653.242 2.874.118 3.176.77.84 1.235 1.911 1.235 3.221 0 4.609-2.807 5.624-5.479 5.921.43.372.823 1.102.823 2.222v3.293c0 .319.192.694.801.576 4.765-1.589 8.199-6.086 8.199-11.386 0-6.627-5.373-12-12-12z" />
            </svg>
            GitHub
          </button>
        </div>

        <p className="text-center text-sm text-muted-foreground mt-4">
          Don't have an account?{" "}
          <Link to="/register" className="text-primary font-medium hover:underline">
            Sign up
          </Link>
        </p>
      </form>
    </AuthLayout>
  );
};

export default Login;
