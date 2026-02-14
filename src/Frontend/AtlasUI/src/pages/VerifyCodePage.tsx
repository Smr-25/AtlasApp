import React, { useState, useRef } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import AtlasLogo from '@/components/AtlasLogo';

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

  const handleVerify = (e: React.FormEvent) => {
    e.preventDefault();
    if (type === 'email' && state?.phone) {
      navigate('/verify-phone', { state });
    } else {
      navigate('/dashboard');
    }
  };

  const startCooldown = () => {
    setResendCooldown(60);
    const timer = setInterval(() => {
      setResendCooldown(prev => {
        if (prev <= 1) { clearInterval(timer); return 0; }
        return prev - 1;
      });
    }, 1000);
  };

  const handleResend = (method?: string) => {
    setShowPhoneResendChoice(false);
    startCooldown();
    // API call by user
  };

  const title = type === 'email' ? 'Email təsdiqləmə' : 'Telefon təsdiqləmə';
  const desc = type === 'email'
    ? `${state?.email || 'email'} ünvanına göndərilən kodu daxil edin`
    : `${state?.phone || 'telefon'} nömrəsinə göndərilən kodu daxil edin`;

  return (
    <div className="min-h-screen flex items-center justify-center bg-background relative overflow-hidden">
      <div className="absolute top-0 left-1/2 -translate-x-1/2 w-[600px] h-[400px] rounded-full opacity-30" style={{ background: 'var(--gradient-glow)' }} />

      <div className="w-full max-w-md mx-4 animate-fade-in">
        <div className="text-center mb-8">
          <AtlasLogo size="lg" />
        </div>

        <div className="glass rounded-2xl p-8 space-y-6">
          <div className="text-center">
            <div className="mx-auto w-16 h-16 rounded-2xl bg-primary/10 flex items-center justify-center mb-4">
              <span className="text-3xl">{type === 'email' ? '📧' : '📱'}</span>
            </div>
            <h2 className="text-xl font-semibold text-foreground">{title}</h2>
            <p className="text-sm text-muted-foreground mt-1">{desc}</p>
          </div>

          <form onSubmit={handleVerify} className="space-y-6">
            <div className="flex justify-center gap-3" onPaste={handlePaste}>
              {code.map((digit, i) => (
                <input
                  key={i}
                  ref={el => { inputRefs.current[i] = el; }}
                  type="text"
                  inputMode="numeric"
                  maxLength={1}
                  value={digit}
                  onChange={(e) => handleChange(i, e.target.value)}
                  onKeyDown={(e) => handleKeyDown(i, e)}
                  className="w-12 h-14 rounded-lg bg-secondary border border-border text-center text-lg font-semibold text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all duration-200"
                />
              ))}
            </div>

            <button
              type="submit"
              className="w-full h-11 rounded-lg font-medium text-sm text-primary-foreground transition-all duration-200 hover:opacity-90 active:scale-[0.98]"
              style={{ background: 'var(--gradient-primary)' }}
            >
              Təsdiqlə
            </button>
          </form>

          <div className="text-center">
            {type === 'phone' && !showPhoneResendChoice ? (
              <button
                onClick={() => resendCooldown === 0 && setShowPhoneResendChoice(true)}
                disabled={resendCooldown > 0}
                className="text-sm text-primary hover:underline disabled:text-muted-foreground disabled:no-underline"
              >
                {resendCooldown > 0 ? `Yenidən göndər (${resendCooldown}s)` : 'Kodu yenidən göndər'}
              </button>
            ) : type === 'phone' && showPhoneResendChoice ? (
              <div className="flex gap-3 justify-center animate-fade-in">
                <button
                  onClick={() => handleResend('sms')}
                  className="px-4 py-2 rounded-lg bg-secondary border border-border text-sm text-foreground hover:bg-muted transition-all"
                >
                  📱 SMS ilə
                </button>
                <button
                  onClick={() => handleResend('telegram')}
                  className="px-4 py-2 rounded-lg bg-secondary border border-border text-sm text-foreground hover:bg-muted transition-all"
                >
                  ✈️ Telegram ilə
                </button>
              </div>
            ) : (
              <button
                onClick={() => handleResend()}
                disabled={resendCooldown > 0}
                className="text-sm text-primary hover:underline disabled:text-muted-foreground disabled:no-underline"
              >
                {resendCooldown > 0 ? `Yenidən göndər (${resendCooldown}s)` : 'Kodu yenidən göndər'}
              </button>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default VerifyCodePage;
