import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { Lock, User } from 'lucide-react';
import AtlasLogo from '@/components/AtlasLogo';
import AuthInput from '@/components/auth/AuthInput';
import OAuthButton from '@/components/auth/OAuthButton';


function parseApiError(result: any, response?: Response) {
  if (!result) return response?.statusText || 'An unknown error occurred.';

  const errs = result.errors ?? result.Errors ?? null;

  if (Array.isArray(errs) && errs.length > 0) {
    return (errs as string[]).join('\n');
  }

  if (errs && typeof errs === 'object') {
    try {
      const parts: string[] = [];
      for (const key of Object.keys(errs)) {
        const v = errs[key];
        if (Array.isArray(v)) parts.push(`${key}: ${v.join(', ')}`);
        else parts.push(`${key}: ${String(v)}`);
      }
      if (parts.length) return parts.join('\n');
    } catch (e) {

    }
  }

  if (result.message) return String(result.message);
  if (result.error) return String(result.error);

  if (response) return `${response.status} ${response.statusText}`;

  return 'An unexpected server error occurred.';
}

const LoginPage: React.FC = () => {
  const navigate = useNavigate();
  const [emailOrUsername, setEmailOrUsername] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const validateInput = () => {

    if (!emailOrUsername) return 'Email or Username is required.';
    if (!password) return 'Password is required.';


    if (emailOrUsername.includes('@')) {
      const emailRe = /\S+@\S+\.\S+/;
      if (!emailRe.test(emailOrUsername)) return 'A valid email is required.';
    }

    return null;
  };

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    const validationError = validateInput();
    if (validationError) {
      setError(validationError);
      return;
    }

    setLoading(true);
    const isEmail = emailOrUsername.includes('@');
    const payload = {
      email: isEmail ? emailOrUsername : null,
      userName: isEmail ? null : emailOrUsername,
      password: password || '',
    } as Record<string, unknown>;

    try {
      const res = await fetch('http://localhost:5075/api/Accounts/login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(payload),
      });

      let result: any = null;
      try {
        result = await res.json();
      } catch (jsonErr) {

        if (!res.ok) {
          setError(`${res.status} ${res.statusText}`);
          setLoading(false);
          return;
        }
      }

      if (!res.ok) {
        const msg = parseApiError(result, res);
        setError(msg);
        setLoading(false);
        return;
      }

      if (result && result.isSuccess) {
        const data = result.data as any;

        if (data?.accessToken) localStorage.setItem('accessToken', data.accessToken);
        if (data?.refreshToken) localStorage.setItem('refreshToken', data.refreshToken);
        if (data?.accessTokenExpiration) localStorage.setItem('accessTokenExpiration', data.accessTokenExpiration);
        if (data?.refreshTokenExpiration) localStorage.setItem('refreshTokenExpiration', data.refreshTokenExpiration);
        if (data?.userId) localStorage.setItem('userId', data.userId);
        if (data?.userName) localStorage.setItem('userName', data.userName);
        if (data?.email) localStorage.setItem('email', data.email);
        if (data?.fullName) localStorage.setItem('fullName', data.fullName);

        navigate('/dashboard');
      } else {
        const msg = parseApiError(result, res);
        setError(msg);
      }
    } catch (err: any) {
      setError(err?.message || 'Network error. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-background relative overflow-hidden">
      <div className="absolute top-0 left-1/2 -translate-x-1/2 w-[600px] h-[400px] rounded-full opacity-30" style={{ background: 'var(--gradient-glow)' }} />

      <div className="w-full max-w-md mx-4 animate-fade-in">
        <div className="text-center mb-8">
          <AtlasLogo size="lg" />
          <p className="mt-3 text-muted-foreground text-sm">Command Center for Tech Professionals</p>
        </div>

        <div className="glass rounded-2xl p-8 space-y-6">
          <div>
            <h2 className="text-xl font-semibold text-foreground">Sign in</h2>
            <p className="text-sm text-muted-foreground mt-1">Enter your credentials to continue</p>
          </div>

          <form onSubmit={handleLogin} className="space-y-4">
            <AuthInput
              label="Email or Username"
              icon={User}
              placeholder="email@example.com"
              value={emailOrUsername}
              onChange={(e) => setEmailOrUsername(e.target.value)}
            />
            <AuthInput
              label="Password"
              icon={Lock}
              type="password"
              placeholder="••••••••"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />

            <div className="flex justify-end">
              <Link to="/forgot-password" className="text-xs text-primary hover:underline">
                Forgot password?
              </Link>
            </div>

            {error && <p className="text-sm text-destructive whitespace-pre-wrap">{error}</p>}

            <button
              type="submit"
              disabled={loading}
              className={`w-full h-11 rounded-lg font-medium text-sm text-primary-foreground transition-all duration-200 active:scale-[0.98] ${loading ? 'opacity-60 cursor-not-allowed' : ''}`}
              style={{ background: 'var(--gradient-primary)' }}
            >
              {loading ? 'Signing in...' : 'Sign in'}
            </button>
          </form>

          <div className="relative">
            <div className="absolute inset-0 flex items-center">
              <div className="w-full border-t border-border" />
            </div>
            <div className="relative flex justify-center text-xs">
              <span className="bg-card px-3 text-muted-foreground">or</span>
            </div>
          </div>

          <div className="flex gap-3">
            <OAuthButton provider="google" />
            <OAuthButton provider="apple" />
            <OAuthButton provider="github" />
          </div>

          <p className="text-center text-sm text-muted-foreground">
            Don't have an account?{' '}
            <Link to="/register" className="text-primary hover:underline font-medium">
              Sign up
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;
