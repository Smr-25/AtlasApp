import React, { useState, useRef } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import AtlasLogo from '@/components/AtlasLogo';
import { Mail, Phone, Eye, EyeOff } from 'lucide-react';

interface VerifyCodePageProps {
  type: 'email' | 'phone';
}

const VerifyCodePage: React.FC<VerifyCodePageProps> = ({ type }) => {
  const navigate = useNavigate();
  const location = useLocation();
  const state = location.state as { email?: string; phone?: string; contactMethod?: string } | null;
  const [code, setCode] = useState(['', '', '', '', '', '']);
  const [resendCooldown, setResendCooldown] = useState(0);
  const [showPhoneResendChoice, setShowPhoneResendChoice] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [showCode, setShowCode] = useState(false);
  const inputRefs = useRef<(HTMLInputElement | null)[]>([]);

  const handleChange = (index: number, value: string) => {
    if (!/^\d*$/.test(value)) return;
    const newCode = [...code];
    newCode[index] = value.slice(-1);
    setCode(newCode);
    if (value && index < 5) {
      inputRefs.current[index + 1]?.focus();
    }
  };

  const handleKeyDown = (index: number, e: React.KeyboardEvent) => {
    if (e.key === 'Backspace' && !code[index] && index > 0) {
      inputRefs.current[index - 1]?.focus();
    }
  };

  const handlePaste = (e: React.ClipboardEvent) => {
    e.preventDefault();
    const pasted = e.clipboardData.getData('text').replace(/\D/g, '').slice(0, 6);
    const newCode = [...code];
    pasted.split('').forEach((char, i) => { newCode[i] = char; });
    setCode(newCode);
    inputRefs.current[Math.min(pasted.length, 5)]?.focus();
  };

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
      } catch (e) {}
    }
    if (result.message) return String(result.message);
    if (result.error) return String(result.error);
    if (response) return `${response.status} ${response.statusText}`;
    return 'An unexpected server error occurred.';
  }

  const startCooldown = () => {
    setResendCooldown(60);
    const timer = setInterval(() => {
      setResendCooldown(prev => {
        if (prev <= 1) { clearInterval(timer); return 0; }
        return prev - 1;
      });
    }, 1000);
  };

  const handleVerify = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccessMessage(null);
    const verificationCode = code.join('');
    if (!/^\d{6}$/.test(verificationCode)) {
      if (verificationCode.length !== 6) setError('Verification code must be 6 digits.');
      else setError('Verification code must contain only digits.');
      return;
    }

    if (type === 'email') {
      if (!state?.email) {
        setError('Email is required.');
        return;
      }
      const emailRe = /\S+@\S+\.\S+/;
      if (!emailRe.test(state.email)) {
        setError('A valid email address is required.');
        return;
      }

      setLoading(true);
      try {
        const res = await fetch('http://localhost:5075/api/accounts/verify-email', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ email: state.email, verificationCode }),
        });
        let result: any = null;
        try { result = await res.json(); } catch (err) {}
        if (!res.ok) {
          setError(parseApiError(result, res));
          return;
        }
        const successFlag = result?.success ?? result?.isSuccess ?? false;
        if (successFlag) {
          setSuccessMessage('Email verified successfully.');
          if (state?.phone) {
            navigate('/verify-phone', { state: { phone: state.phone, telegramBotLink: (state as any).telegramBotLink } });
          } else {
            navigate('/onboarding');
          }
        } else {
          setError(parseApiError(result, res));
        }
      } catch (err: any) {
        setError(err?.message || 'Network error. Please try again.');
      } finally {
        setLoading(false);
      }
    } else {
      if (!state?.phone) { setError('Phone number is required.'); return; }
      const phoneRe = /^\+\d{1,3}\d{4,14}(?:x.+)?$/;
      if (!phoneRe.test(state.phone)) {
        setError('Phone number must be in valid international format (e.g., +994501234567).');
        return;
      }

      setLoading(true);
      try {
        const res = await fetch('http://localhost:5075/api/accounts/verify-phone', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ phoneNumber: state.phone, verificationCode }),
        });
        let result: any = null;
        try { result = await res.json(); } catch (err) {}
        if (!res.ok) {
          setError(parseApiError(result, res));
          return;
        }
        const successFlag = result?.success ?? result?.isSuccess ?? false;
        if (successFlag) {
          setSuccessMessage('Phone verified successfully.');
          navigate('/onboarding');
        } else {
          setError(parseApiError(result, res));
        }
      } catch (err: any) {
        setError(err?.message || 'Network error. Please try again.');
      } finally {
        setLoading(false);
      }
    }
  };

  const handleResend = async (channel?: number) => {
    setShowPhoneResendChoice(false);
    setError(null);
    setSuccessMessage(null);
    startCooldown();

    if (type === 'email') {
      if (!state?.email) { setError('Email is required.'); return; }
      const emailRe = /\S+@\S+\.\S+/;
      if (!emailRe.test(state.email)) { setError('A valid email address is required.'); return; }
      try {
        const res = await fetch('http://localhost:5075/api/accounts/resend-email-verification-code', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ email: state.email }),
        });
        let result: any = null;
        try { result = await res.json(); } catch (err) {}
        if (!res.ok) { setError(parseApiError(result, res)); return; }
        const successFlag = result?.success ?? result?.isSuccess ?? false;
        if (successFlag) setSuccessMessage('Verification code resent. Check your email.');
        else setError(parseApiError(result, res));
      } catch (err: any) {
        setError(err?.message || 'Network error. Please try again.');
      }
    } else {
      if (!state?.phone) { setError('Phone number is required.'); return; }
      const phoneRe = /^\+\d{1,3}\d{4,14}(?:x.+)?$/;
      if (!phoneRe.test(state.phone)) { setError('Phone number must be in valid international format (e.g., +994501234567).'); return; }
      const payload: any = { phoneNumber: state.phone };
      if (channel) payload.channel = channel;
      try {
        const res = await fetch('http://localhost:5075/api/accounts/resend-phone-verification-code', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(payload),
        });
        let result: any = null;
        try { result = await res.json(); } catch (err) {}
        if (!res.ok) { setError(parseApiError(result, res)); return; }
        const successFlag = result?.success ?? result?.isSuccess ?? false;
        if (successFlag) setSuccessMessage('Verification code resent.');
        else setError(parseApiError(result, res));
      } catch (err: any) {
        setError(err?.message || 'Network error. Please try again.');
      }
    }
  };

  const title = type === 'email' ? 'Email verification' : 'Phone verification';
  const desc = type === 'email'
    ? `Enter the code sent to ${state?.email || 'your email'}`
    : `Enter the code sent to ${state?.phone || 'your phone'}`;

  const telegramBotLink = (state as any)?.telegramBotLink ?? null;

  return (
    <div className="min-h-screen flex items-center justify-center bg-background relative overflow-hidden">
      <div className="absolute top-0 left-1/2 -translate-x-1/2 w-[600px] h-[400px] rounded-full opacity-30" style={{ background: 'var(--gradient-glow)' }} />

      <div className="w-full max-w-md mx-4 animate-fade-in">
        <div className="text-center mb-8">
          <AtlasLogo size="lg" />
        </div>

        <div className="glass rounded-2xl p-8 space-y-6">

          {telegramBotLink && type === 'email' && (
            <div className="p-3 rounded-md bg-primary/10 text-primary">
              <p className="font-medium">Telegram verification</p>
              <p className="text-sm mt-1">We sent a link for Telegram verification. Open the bot and send /start to link your chat.</p>
              <a href={telegramBotLink} target="_blank" rel="noopener noreferrer" className="mt-2 inline-block text-sm px-3 py-2 rounded-md bg-primary text-primary-foreground">Open Telegram Bot</a>
            </div>
          )}

          <div className="text-center">
            <div className="mx-auto w-16 h-16 rounded-2xl bg-primary/10 flex items-center justify-center mb-4">
              <span className="text-3xl">{type === 'email' ? <Mail className="w-6 h-6" /> : <Phone className="w-6 h-6" />}</span>
            </div>
            <h2 className="text-xl font-semibold text-foreground">{title}</h2>
            <p className="text-sm text-muted-foreground mt-1">{desc}</p>
          </div>

          <form onSubmit={handleVerify} className="space-y-6">
            <div className="relative">
              <div className="flex justify-center gap-3" onPaste={handlePaste}>
                {code.map((digit, i) => (
                  <input
                    key={i}
                    ref={el => { inputRefs.current[i] = el; }}
                    type={showCode ? 'text' : 'password'}
                    inputMode="numeric"
                    maxLength={1}
                    value={digit}
                    onChange={(e) => handleChange(i, e.target.value)}
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
            {successMessage && <p className="text-sm text-primary">{successMessage}</p>}

            <button
              type="submit"
              disabled={loading}
              className="w-full h-11 rounded-lg font-medium text-sm text-primary-foreground transition-all duration-200 hover:opacity-90 active:scale-[0.98]"
              style={{ background: 'var(--gradient-primary)' }}
            >
              {loading ? 'Verifying...' : 'Verify'}
            </button>
          </form>

          <div className="text-center">
            {type === 'phone' && !showPhoneResendChoice ? (
              <button
                onClick={() => resendCooldown === 0 && setShowPhoneResendChoice(true)}
                disabled={resendCooldown > 0}
                className="text-sm text-primary hover:underline disabled:text-muted-foreground disabled:no-underline"
              >
                {resendCooldown > 0 ? `Resend (${resendCooldown}s)` : 'Resend code'}
              </button>
            ) : type === 'phone' && showPhoneResendChoice ? (
              <div className="flex gap-3 justify-center animate-fade-in">
                <button
                  onClick={() => handleResend(1)}
                  className="px-4 py-2 rounded-lg bg-secondary border border-border text-sm text-foreground hover:bg-muted transition-all"
                >
                  <span className="inline-flex items-center gap-2">
                    <Phone className="w-4 h-4" />
                    SMS
                  </span>
                </button>
                <button
                  onClick={() => handleResend(2)}
                  className="px-4 py-2 rounded-lg bg-secondary border border-border text-sm text-foreground hover:bg-muted transition-all"
                >
                  <span className="inline-flex items-center gap-2">
                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 240 240" className="w-4 h-4" aria-hidden>
                      <path fill="currentColor" d="M120 0C53.7 0 0 53.7 0 120s53.7 120 120 120 120-53.7 120-120S186.3 0 120 0zm54.8 83.9l-20.7 97.5c-1.6 6.9-5.9 8.6-11.9 5.4l-33-24.3-15.9 15.3c-1.8 1.8-3.3 3.3-6.7 3.3l2.4-34.4 62.6-56.3c2.7-2.4-.6-3.7-4.2-1.3L70.5 124l-33.9-10.6c-7.3-2.3-7.4-7.3 1.5-10.8L173 69.2c6.5-2.2 12.2 1.5 11.8 14.7z" />
                    </svg>
                    Telegram
                  </span>
                </button>
              </div>
            ) : (
              <button
                onClick={() => handleResend()}
                disabled={resendCooldown > 0}
                className="text-sm text-primary hover:underline disabled:text-muted-foreground disabled:no-underline"
              >
                {resendCooldown > 0 ? `Resend (${resendCooldown}s)` : 'Resend code'}
              </button>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default VerifyCodePage;
