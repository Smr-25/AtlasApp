import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { User, Mail, Lock, Phone, AtSign } from 'lucide-react';
import AtlasLogo from '@/components/AtlasLogo';
import AuthInput from '@/components/auth/AuthInput';
import OAuthButton from '@/components/auth/OAuthButton';

const RegisterPage: React.FC = () => {
  const navigate = useNavigate();
  const [form, setForm] = useState({
    fullname: '',
    username: '',
    email: '',
    phone: '',
    password: '',
    confirmPassword: '',
  });
  const [contactMethod, setContactMethod] = useState<'sms' | 'telegram' | null>(null);
  const [showContactChoice, setShowContactChoice] = useState(false);

  const update = (field: string, value: string) => {
    setForm(prev => ({ ...prev, [field]: value }));
    if (field === 'phone') {
      setShowContactChoice(value.length > 0);
      if (!value) setContactMethod(null);
    }
  };

  const handleRegister = (e: React.FormEvent) => {
    e.preventDefault();
    // Navigate to email verification first
    navigate('/verify-email', { state: { email: form.email, phone: form.phone, contactMethod } });
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-background relative overflow-hidden py-8">
      <div className="absolute top-0 left-1/2 -translate-x-1/2 w-[600px] h-[400px] rounded-full opacity-30" style={{ background: 'var(--gradient-glow)' }} />

      <div className="w-full max-w-md mx-4 animate-fade-in">
        <div className="text-center mb-8">
          <AtlasLogo size="lg" />
          <p className="mt-3 text-muted-foreground text-sm">Command Center for Tech Professionals</p>
        </div>

        <div className="glass rounded-2xl p-8 space-y-6">
          <div>
            <h2 className="text-xl font-semibold text-foreground">Qeydiyyat</h2>
            <p className="text-sm text-muted-foreground mt-1">Yeni hesab yaradın</p>
          </div>

          <form onSubmit={handleRegister} className="space-y-4">
            <AuthInput
              label="Ad Soyad"
              icon={User}
              placeholder="Ad Soyad"
              value={form.fullname}
              onChange={(e) => update('fullname', e.target.value)}
            />
            <AuthInput
              label="İstifadəçi adı"
              icon={AtSign}
              placeholder="username"
              value={form.username}
              onChange={(e) => update('username', e.target.value)}
            />
            <AuthInput
              label="Email"
              icon={Mail}
              type="email"
              placeholder="email@example.com"
              value={form.email}
              onChange={(e) => update('email', e.target.value)}
            />
            <AuthInput
              label="Telefon nömrəsi (istəyə bağlı)"
              icon={Phone}
              type="tel"
              placeholder="+994 XX XXX XX XX"
              value={form.phone}
              onChange={(e) => update('phone', e.target.value)}
            />

            {showContactChoice && (
              <div className="space-y-2 animate-fade-in">
                <label className="text-sm font-medium text-muted-foreground">Əlaqə üsulu</label>
                <div className="flex gap-3">
                  <button
                    type="button"
                    onClick={() => setContactMethod('sms')}
                    className={`flex-1 h-10 rounded-lg border text-sm font-medium transition-all duration-200 ${
                      contactMethod === 'sms'
                        ? 'bg-primary/10 border-primary text-primary'
                        : 'bg-secondary border-border text-muted-foreground hover:text-foreground'
                    }`}
                  >
                    📱 SMS
                  </button>
                  <button
                    type="button"
                    onClick={() => setContactMethod('telegram')}
                    className={`flex-1 h-10 rounded-lg border text-sm font-medium transition-all duration-200 ${
                      contactMethod === 'telegram'
                        ? 'bg-primary/10 border-primary text-primary'
                        : 'bg-secondary border-border text-muted-foreground hover:text-foreground'
                    }`}
                  >
                    ✈️ Telegram
                  </button>
                </div>
              </div>
            )}

            <AuthInput
              label="Şifrə"
              icon={Lock}
              type="password"
              placeholder="••••••••"
              value={form.password}
              onChange={(e) => update('password', e.target.value)}
            />
            <AuthInput
              label="Şifrəni təsdiqlə"
              icon={Lock}
              type="password"
              placeholder="••••••••"
              value={form.confirmPassword}
              onChange={(e) => update('confirmPassword', e.target.value)}
            />

            <button
              type="submit"
              className="w-full h-11 rounded-lg font-medium text-sm text-primary-foreground transition-all duration-200 hover:opacity-90 active:scale-[0.98]"
              style={{ background: 'var(--gradient-primary)' }}
            >
              Qeydiyyatdan keç
            </button>
          </form>

          <div className="relative">
            <div className="absolute inset-0 flex items-center">
              <div className="w-full border-t border-border" />
            </div>
            <div className="relative flex justify-center text-xs">
              <span className="bg-card px-3 text-muted-foreground">və ya</span>
            </div>
          </div>

          <div className="flex gap-3">
            <OAuthButton provider="google" />
            <OAuthButton provider="apple" />
            <OAuthButton provider="github" />
          </div>

          <p className="text-center text-sm text-muted-foreground">
            Artıq hesabınız var?{' '}
            <Link to="/login" className="text-primary hover:underline font-medium">
              Daxil ol
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default RegisterPage;
