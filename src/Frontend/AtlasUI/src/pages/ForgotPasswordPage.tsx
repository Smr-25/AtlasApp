import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { Mail } from 'lucide-react';
import AtlasLogo from '@/components/AtlasLogo';
import AuthInput from '@/components/auth/AuthInput';

function parseApiError(result: any, response?: Response) {
  if (!result) return response?.statusText || 'An unknown error occurred.';
  const errs = result.errors ?? result.Errors ?? null;
  if (Array.isArray(errs) && errs.length > 0) return errs.join('\n');
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

const ForgotPasswordPage: React.FC = () => {
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const validateEmail = (v: string) => {
    if (!v || v.trim() === '') return 'Email is required.';
    const emailRe = /\S+@\S+\.\S+/;
    if (!emailRe.test(v.trim())) return 'A valid email address is required.';
    return null;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccess(null);
    const vErr = validateEmail(email);
    if (vErr) {
      setError(vErr);
      return;
    }
    setLoading(true);
    try {
      const res = await fetch('http://localhost:5075/api/accounts/forgot-password', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email }),
      });
      let result: any = null;
      try { result = await res.json(); } catch (err) { }
      if (!res.ok) {
        setError(parseApiError(result, res));
        setLoading(false);
        return;
      }
      const successFlag = result?.success ?? result?.isSuccess ?? false;
      if (successFlag) {
        setSuccess('Verification code sent. Check your email.');
        navigate('/reset-password', { state: { email } });
      } else {
        setError(parseApiError(result, res));
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
        </div>

        <div className="glass rounded-2xl p-8 space-y-6">
          <div className="text-center">
            <h2 className="text-xl font-semibold text-foreground">Reset password</h2>
            <p className="text-sm text-muted-foreground mt-1">Enter your account email to receive a verification code</p>
          </div>

          <form onSubmit={handleSubmit} className="space-y-4">
            <AuthInput
              label="Email"
              icon={Mail}
              type="email"
              placeholder="email@example.com"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />

            {error && <p className="text-sm text-destructive whitespace-pre-wrap">{error}</p>}
            {success && <p className="text-sm text-primary">{success}</p>}

            <button
              type="submit"
              disabled={loading}
              className="w-full h-11 rounded-lg font-medium text-sm text-primary-foreground transition-all duration-200 hover:opacity-90 active:scale-[0.98]"
              style={{ background: 'var(--gradient-primary)' }}
            >
              {loading ? 'Sending...' : 'Send code'}
            </button>
          </form>

          <p className="text-center text-sm text-muted-foreground">
            <Link to="/login" className="text-primary hover:underline font-medium">
              ← Back to login
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default ForgotPasswordPage;
