import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { Mail } from 'lucide-react';
import AtlasLogo from '@/components/AtlasLogo';
import AuthInput from '@/components/auth/AuthInput';

const ForgotPasswordPage: React.FC = () => {
  const navigate = useNavigate();
  const [email, setEmail] = useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    navigate('/reset-password', { state: { email } });
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
            <h2 className="text-xl font-semibold text-foreground">Şifrəni bərpa et</h2>
            <p className="text-sm text-muted-foreground mt-1">Email ünvanınızı daxil edin</p>
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

            <button
              type="submit"
              className="w-full h-11 rounded-lg font-medium text-sm text-primary-foreground transition-all duration-200 hover:opacity-90 active:scale-[0.98]"
              style={{ background: 'var(--gradient-primary)' }}
            >
              Kodu göndər
            </button>
          </form>

          <p className="text-center text-sm text-muted-foreground">
            <Link to="/login" className="text-primary hover:underline font-medium">
              ← Geri qayıt
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default ForgotPasswordPage;
