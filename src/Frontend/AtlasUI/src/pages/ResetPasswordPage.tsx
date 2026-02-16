import React, { useState, useRef } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Lock, Eye, EyeOff } from 'lucide-react';
import AtlasLogo from '@/components/AtlasLogo';
import AuthInput from '@/components/auth/AuthInput';
import ClosedEye from "@/components/icons/ClosedEye";

function parseApiError(result: any, response?: Response) {
  if (!result) return response?.statusText || 'An unknown error occurred.';
  const errs = result.errors ?? result.Errors ?? null;
  if (Array.isArray(errs) && errs.length > 0) return (errs as string[]).join('\n');
  if (errs && typeof errs === 'object') {
    try {
      const parts: string[] = [];
      for (const key of Object.keys(errs)) {
        const v = errs[key];
        if (Array.isArray(v)) parts.push(`${key}: ${v.join(', ')}`);
        else parts.push(`${key}: ${String(v)}`);
      }
      if (parts.length) return parts.join('\n');
    } catch (e) {}
  }
  if (result.message) return String(result.message);
  if (result.error) return String(result.error);
  if (response) return `${response.status} ${response.statusText}`;
  return 'An unexpected server error occurred.';
}

const ResetPasswordPage: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const state = location.state as { email?: string } | null;
  const [step, setStep] = useState<'code' | 'password'>('code');
  const [code, setCode] = useState(['', '', '', '', '', '']);
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [resendCooldown, setResendCooldown] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [showCode, setShowCode] = useState(false);
  const [showNewPassword, setShowNewPassword] = useState(false);
  const [showConfirmNewPassword, setShowConfirmNewPassword] = useState(false);
  const inputRefs = useRef<(HTMLInputElement | null)[]>([]);
  const [emailLocal, setEmailLocal] = useState(state?.email ?? '');

  const handleCodeChange = (index: number, value: string) => {
    if (!/^\d*$/.test(value)) return;
    const newCodeArr = [...code];
    newCodeArr[index] = value.slice(-1);
    setCode(newCodeArr);
    if (value && index < 5) inputRefs.current[index + 1]?.focus();
  };

  const handleKeyDown = (index: number, e: React.KeyboardEvent) => {
    if (e.key === 'Backspace' && !code[index] && index > 0) inputRefs.current[index - 1]?.focus();
  };

  const validatePassword = (pwd: string) => {
    if (!pwd) return 'Password is required.';
    if (pwd.length < 8) return 'Password must be at least 8 characters long.';
    if (!/[A-Z]/.test(pwd)) return 'Password must contain at least one uppercase letter.';
    if (!/[a-z]/.test(pwd)) return 'Password must contain at least one lowercase letter.';
    if (!/[0-9]/.test(pwd)) return 'Password must contain at least one digit.';
    if (!/[^a-zA-Z0-9]/.test(pwd)) return 'Password must contain at least one special character.';
    return null;
  };

  const handleVerifyCode = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccess(null);
    const verificationCode = code.join('');
    if (verificationCode.length !== 6) {
      setError('Verification code is required.');
      return;
    }
    setLoading(true);
    try {
      const emailToUse = (state?.email ?? emailLocal).trim();
      if (!emailToUse) {
        setError('Email is missing. Enter your email above or go to Forgot Password.');
        setLoading(false);
        return;
      }
      const res = await fetch('http://localhost:5075/api/accounts/reset-password', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email: emailToUse, verificationCode }),
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
        setSuccess('Code verified. You can now set a new password.');
        setStep('password');
      } else {
        setError(parseApiError(result, res));
      }
    } catch (err: any) {
      setError(err?.message || 'Network error. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const handleResetPassword = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccess(null);
    const pwdErr = validatePassword(newPassword);
    if (pwdErr) { setError(pwdErr); return; }
    if (newPassword !== confirmPassword) { setError('Passwords do not match.'); return; }
    setLoading(true);
    try {
      const emailToUse = (state?.email ?? emailLocal).trim();
      if (!emailToUse) {
        setError('Email is missing. Enter your email above or go to Forgot Password.');
        setLoading(false);
        return;
      }
      const res = await fetch('http://localhost:5075/api/accounts/reset-password', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          email: emailToUse,
          verificationCode: code.join(''),
          newPassword,
          confirmPassword,
        }),
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
        setSuccess('Password reset successful. You can now sign in.');
        setTimeout(() => navigate('/login'), 1500);
      } else {
        setError(parseApiError(result, res));
      }
    } catch (err: any) {
      setError(err?.message || 'Network error. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const handleResend = async () => {
    setResendCooldown(60);
    const timer = setInterval(() => { setResendCooldown(prev => { if (prev <= 1) { clearInterval(timer); return 0; } return prev - 1; }); }, 1000);

    setError(null);
    setSuccess(null);

    const email = (state?.email ?? emailLocal).trim();
     if (!email) {
       setError('Email is missing.');
       return;
     }
     const emailRe = /\S+@\S+\.\S+/;
     if (!emailRe.test(email)) {
       setError('A valid email is required.');
       return;
     }

    try {
      const res = await fetch('http://localhost:5075/api/accounts/forgot-password', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email }),
      });
      let result: any = null;
      try { result = await res.json(); } catch (err) {}
      if (!res.ok) {
        setError(parseApiError(result, res));
        return;
      }
      const successFlag = result?.success ?? result?.isSuccess ?? false;
      if (successFlag) {
        setSuccess('Verification code resent. Check your email.');
      } else {
        setError(parseApiError(result, res));
      }
    } catch (err: any) {
      setError(err?.message || 'Network error. Please try again.');
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
          {step === 'code' ? (
            <>
              <div className="text-center">
                <h2 className="text-xl font-semibold text-foreground">Verification code</h2>
                <p className="text-sm text-muted-foreground mt-1">Enter the code sent to {state?.email || emailLocal || 'your email'}</p>
              </div>
              {!state?.email && (
                <div>
                  <AuthInput label="Email" type="email" placeholder="email@example.com" value={emailLocal} onChange={e => setEmailLocal(e.target.value)} />
                </div>
              )}
              <form onSubmit={handleVerifyCode} className="space-y-6">
                <div className="relative">
                  <div className="flex justify-center gap-3">
                    {code.map((digit, i) => (
                      <input
                        key={i}
                        ref={el => { inputRefs.current[i] = el; }}
                        type={showCode ? 'text' : 'password'}
                        inputMode="numeric"
                        maxLength={1}
                        value={digit}
                        onChange={(e) => handleCodeChange(i, e.target.value)}
                        onKeyDown={(e) => handleKeyDown(i, e)}
                        className="w-12 h-14 rounded-lg bg-secondary border border-border text-center text-lg font-semibold text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all duration-200"
                      />
                    ))}
                  </div>
                  <button type="button" onClick={() => setShowCode(s => !s)} className="absolute right-2 top-1/2 -translate-y-1/2 text-muted-foreground">
                    {showCode ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
                  </button>
                </div>
                {error && <p className="text-sm text-destructive whitespace-pre-wrap">{error}</p>}
                {success && <p className="text-sm text-primary">{success}</p>}
                <button type="submit" disabled={loading} className="w-full h-11 rounded-lg font-medium text-sm text-primary-foreground transition-all duration-200 hover:opacity-90" style={{ background: 'var(--gradient-primary)' }}>
                  {loading ? 'Verifying...' : 'Verify'}
                </button>
              </form>
              <div className="text-center">
                <button onClick={handleResend} disabled={resendCooldown > 0} className="text-sm text-primary hover:underline disabled:text-muted-foreground">
                  {resendCooldown > 0 ? `Resend (${resendCooldown}s)` : 'Resend code'}
                </button>
                <div className="mt-2">
                 <a href="/forgot-password" className="text-xs text-muted-foreground hover:underline">Back to Forgot Password</a>
               </div>
              </div>
            </>
          ) : (
            <>
              <div className="text-center">
                <h2 className="text-xl font-semibold text-foreground">New password</h2>
                <p className="text-sm text-muted-foreground mt-1">Enter your new password</p>
              </div>
              <form onSubmit={handleResetPassword} className="space-y-4">
                <AuthInput label="New password" icon={Lock} type={showNewPassword ? 'text' : 'password'} placeholder="••••••••" value={newPassword} onChange={e => setNewPassword(e.target.value)} suffix={
                  <button type="button" onClick={() => setShowNewPassword(s => !s)} className="p-1 text-muted-foreground">
                    {showNewPassword ? <ClosedEye className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                  </button>
                } />
                <AuthInput label="Confirm password" icon={Lock} type={showConfirmNewPassword ? 'text' : 'password'} placeholder="••••••••" value={confirmPassword} onChange={e => setConfirmPassword(e.target.value)} suffix={
                  <button type="button" onClick={() => setShowConfirmNewPassword(s => !s)} className="p-1 text-muted-foreground">
                    {showConfirmNewPassword ? <ClosedEye className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                  </button>
                } />

                {error && <p className="text-sm text-destructive whitespace-pre-wrap">{error}</p>}
                {success && <p className="text-sm text-primary">{success}</p>}

                <button type="submit" disabled={loading} className="w-full h-11 rounded-lg font-medium text-sm text-primary-foreground transition-all duration-200 hover:opacity-90" style={{ background: 'var(--gradient-primary)' }}>
                  {loading ? 'Resetting...' : 'Reset password'}
                </button>
              </form>
            </>
          )}
        </div>
      </div>
    </div>
  );
};

export default ResetPasswordPage;
