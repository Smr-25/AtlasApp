import React, { useState, useRef } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Lock } from 'lucide-react';
import AtlasLogo from '@/components/AtlasLogo';
import AuthInput from '@/components/auth/AuthInput';

const ResetPasswordPage: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const state = location.state as { email?: string } | null;
  const [step, setStep] = useState<'code' | 'password'>('code');
  const [code, setCode] = useState(['', '', '', '', '', '']);
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [resendCooldown, setResendCooldown] = useState(0);
  const inputRefs = useRef<(HTMLInputElement | null)[]>([]);

  const handleCodeChange = (index: number, value: string) => {
    if (!/^\d*$/.test(value)) return;
    const newCode = [...code];
    newCode[index] = value.slice(-1);
    setCode(newCode);
    if (value && index < 5) inputRefs.current[index + 1]?.focus();
  };

  const handleKeyDown = (index: number, e: React.KeyboardEvent) => {
    if (e.key === 'Backspace' && !code[index] && index > 0) inputRefs.current[index - 1]?.focus();
  };

  const handleVerifyCode = (e: React.FormEvent) => {
    e.preventDefault();
    setStep('password');
  };

  const handleResetPassword = (e: React.FormEvent) => {
    e.preventDefault();
    navigate('/login');
  };

  const handleResend = () => {
    setResendCooldown(60);
    const timer = setInterval(() => {
      setResendCooldown(prev => { if (prev <= 1) { clearInterval(timer); return 0; } return prev - 1; });
    }, 1000);
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
                <h2 className="text-xl font-semibold text-foreground">Təsdiq kodu</h2>
                <p className="text-sm text-muted-foreground mt-1">{state?.email} ünvanına göndərilən kodu daxil edin</p>
              </div>
              <form onSubmit={handleVerifyCode} className="space-y-6">
                <div className="flex justify-center gap-3">
                  {code.map((digit, i) => (
                    <input
                      key={i}
                      ref={el => { inputRefs.current[i] = el; }}
                      type="text"
                      inputMode="numeric"
                      maxLength={1}
                      value={digit}
                      onChange={(e) => handleCodeChange(i, e.target.value)}
                      onKeyDown={(e) => handleKeyDown(i, e)}
                      className="w-12 h-14 rounded-lg bg-secondary border border-border text-center text-lg font-semibold text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all duration-200"
                    />
                  ))}
                </div>
                <button type="submit" className="w-full h-11 rounded-lg font-medium text-sm text-primary-foreground transition-all duration-200 hover:opacity-90" style={{ background: 'var(--gradient-primary)' }}>
                  Təsdiqlə
                </button>
              </form>
              <div className="text-center">
                <button onClick={handleResend} disabled={resendCooldown > 0} className="text-sm text-primary hover:underline disabled:text-muted-foreground">
                  {resendCooldown > 0 ? `Yenidən göndər (${resendCooldown}s)` : 'Kodu yenidən göndər'}
                </button>
              </div>
            </>
          ) : (
            <>
              <div className="text-center">
                <h2 className="text-xl font-semibold text-foreground">Yeni şifrə</h2>
                <p className="text-sm text-muted-foreground mt-1">Yeni şifrənizi daxil edin</p>
              </div>
              <form onSubmit={handleResetPassword} className="space-y-4">
                <AuthInput label="Yeni şifrə" icon={Lock} type="password" placeholder="••••••••" value={newPassword} onChange={e => setNewPassword(e.target.value)} />
                <AuthInput label="Şifrəni təsdiqlə" icon={Lock} type="password" placeholder="••••••••" value={confirmPassword} onChange={e => setConfirmPassword(e.target.value)} />
                <button type="submit" className="w-full h-11 rounded-lg font-medium text-sm text-primary-foreground transition-all duration-200 hover:opacity-90" style={{ background: 'var(--gradient-primary)' }}>
                  Şifrəni yenilə
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
