import React, { useState, useRef } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import AtlasLogo from '@/components/AtlasLogo';
import TelegramIcon from '@/components/icons/Telegram';
import { apiFetch } from '@/lib/api';

interface VerifyCodePageProps {
  type: 'email' | 'phone';
}

const VerifyCodePage: React.FC<VerifyCodePageProps> = ({ type }) => {
  const navigate = useNavigate();
  const location = useLocation();
  const state = location.state as { email?: string; phone?: string; telegramBotLink?: string } | null;
  const [code, setCode] = useState(['', '', '', '', '', '']);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const inputRefs = useRef<(HTMLInputElement | null)[]>([]);

  const handleChange = (index: number, value: string) => {
    if (!/^[0-9]*$/.test(value)) return;
    const newArr = [...code];
    newArr[index] = value.slice(-1);
    setCode(newArr);
    if (value && index < 5) inputRefs.current[index + 1]?.focus();
  };

  const handleKeyDown = (index: number, e: React.KeyboardEvent) => {
    if (e.key === 'Backspace' && !code[index] && index > 0) inputRefs.current[index - 1]?.focus();
  };

  const handleVerify = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccessMessage(null);
    const verificationCode = code.join('');
    if (!/^\d{6}$/.test(verificationCode)) { setError('Verification code must be 6 digits.'); return; }
    setLoading(true);
    try {
      const payload = type === 'email' ? { email: state?.email ?? '' , verificationCode } : { phoneNumber: state?.phone ?? '', verificationCode };
      const path = type === 'email' ? '/api/accounts/verify-email' : '/api/accounts/verify-phone';
      const res = await apiFetch(path, { method: 'POST', body: JSON.stringify(payload) });
      const text = await res.text();
      let json: any; try { json = text ? JSON.parse(text) : null; } catch (e) { json = null; }
      const successFlag = json?.success ?? json?.isSuccess ?? false;
      if (successFlag) {
        setSuccessMessage('Verified successfully.');
        setTimeout(() => navigate('/dashboard'), 1000);
      } else {
        setError(json?.message ?? 'Verification failed.');
      }
    } catch (err: any) {
      setError(err?.message || 'Network error.');
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
            <h2 className="text-xl font-semibold text-foreground">Verification code</h2>
            <p className="text-sm text-muted-foreground mt-1">Enter the 6-digit code sent to {type === 'email' ? (state?.email ?? 'your email') : (state?.phone ?? 'your phone')}</p>
          </div>

          {state?.telegramBotLink && type === 'phone' && (
            <div className="p-3 rounded-md bg-secondary flex items-center gap-3">
              <TelegramIcon className="w-6 h-6 text-primary" />
              <div>
                <div className="font-medium">Telegram verification</div>
                <div className="text-xs text-muted-foreground">Open the bot and send /start to link your chat</div>
                <a href={state.telegramBotLink} target="_blank" rel="noreferrer" className="mt-2 inline-block text-sm px-3 py-2 rounded-md bg-primary text-primary-foreground">Open Telegram Bot</a>
              </div>
            </div>
          )}

          <form onSubmit={handleVerify} className="space-y-4">
            <div className="relative">
              <div className="flex justify-center gap-3">
                {code.map((digit, i) => (
                  <input key={i} ref={el => inputRefs.current[i] = el} type="text" inputMode="numeric" maxLength={1} value={digit} onChange={(e) => handleChange(i, e.target.value)} onKeyDown={(e) => handleKeyDown(i, e)} className="w-12 h-14 rounded-lg bg-secondary border border-border text-center text-lg font-semibold text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50" />
                ))}
              </div>
            </div>

            {error && <p className="text-sm text-destructive whitespace-pre-wrap">{error}</p>}
            {successMessage && <p className="text-sm text-primary">{successMessage}</p>}

            <button type="submit" disabled={loading} className="w-full h-11 rounded-lg font-medium text-sm text-primary-foreground transition-all duration-200 hover:opacity-90" style={{ background: 'var(--gradient-primary)' }}>
              {loading ? 'Verifying...' : 'Verify'}
            </button>

          </form>

          <div className="text-center">
            <button disabled className="text-sm text-muted-foreground">Resend disabled in demo</button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default VerifyCodePage;
