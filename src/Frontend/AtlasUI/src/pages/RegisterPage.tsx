import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { User, Mail, Lock, Phone, AtSign } from 'lucide-react';
import AtlasLogo from '@/components/AtlasLogo';
import AuthInput from '@/components/auth/AuthInput';
import OAuthButton from '@/components/auth/OAuthButton';

function parseApiError(result: any, response?: Response) {
  if (!result) return response?.statusText || 'An unknown error occurred.';

  const friendlyForField = (key: string, msgs: string[] | string) => {
    const k = key.toLowerCase();
    const arr = Array.isArray(msgs) ? msgs : [String(msgs)];
    const joined = arr.join('\n');

    if (k.includes('email')) {
      if (/already|exist|taken|in use/i.test(joined))
        return 'This email is already registered. If it\'s yours, try logging in or reset your password; otherwise use a different email.';
      return joined;
    }
    if (k.includes('user') || k.includes('username') || k.includes('username')) {
      if (/already|exist|taken|in use/i.test(joined))
        return 'This username is already taken. Please choose a different username.';
      return joined;
    }

    return joined;
  };

  if (Array.isArray(result.errors) && result.errors.length > 0) {
    return result.errors.join('\n');
  }

  if (result.errors && typeof result.errors === 'object') {
    try {
      const parts: string[] = [];
      for (const key of Object.keys(result.errors)) {
        const v = result.errors[key];
        const friendly = friendlyForField(key, v);
        parts.push(friendly);
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

const RegisterPage: React.FC = () => {
  const navigate = useNavigate();
  const [form, setForm] = useState({
    fullName: '',
    userName: '',
    email: '',
    phone: '',
    password: '',
    confirmPassword: '',
  });
  const [contactMethod, setContactMethod] = useState<'sms' | 'telegram' | null>(null);
  const [showContactChoice, setShowContactChoice] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const update = (field: string, value: string) => {
    setForm(prev => ({ ...prev, [field]: value }));
    setFieldErrors(prev => {
      if (!prev || !prev[field]) return prev;
      const copy = { ...prev } as Record<string, string>;
      delete copy[field];
      return copy;
    });

    if (field === 'phone') {
      setShowContactChoice(value.length > 0);
      if (!value) setContactMethod(null);
    }
  };

  const generateUsernameSuggestions = (base: string) => {
    const clean = (base || 'user').trim().toLowerCase().replace(/[^a-z0-9]/g, '');
    const suggestions = new Set<string>();
    if (clean && clean.length > 0) suggestions.add(clean);
    suggestions.add(`${clean}01`);
    suggestions.add(`${clean}${Math.floor(Math.random() * 90 + 10)}`);
    suggestions.add(`${clean}_dev`);
    const arr = Array.from(suggestions).filter(s => s && s !== clean);
    return arr.slice(0, 3);
  };

  const mapApiErrorsToFields = (result: any, response?: Response) => {
    const fields: Record<string, string> = {};
    let globalMsg: string | null = null;

    if (!result) return { fields, globalMsg };

    if (result.errors && typeof result.errors === 'object') {
      for (const key of Object.keys(result.errors)) {
        const v = result.errors[key];
        const msgs = Array.isArray(v) ? v.join(', ') : String(v);
        const k = key.toLowerCase();
        if (k.includes('email')) {
          fields.email = /already|exist|taken|in use/i.test(msgs)
            ? "This email is already registered. If it's yours, try logging in or reset your password."
            : msgs;
        } else if (k.includes('user') || k.includes('username')) {
          fields.userName = /already|exist|taken|in use/i.test(msgs)
            ? 'This username is already taken. Please choose a different username.'
            : msgs;
        } else {
          fields[key] = msgs;
        }
      }
      return { fields, globalMsg };
    }

    if (Array.isArray(result.errors) && result.errors.length > 0) {
      const arr = result.errors as string[];
      for (const s of arr) {
        const low = s.toLowerCase();
        if (low.includes('email')) fields.email = /already|exist|taken|in use/i.test(low)
          ? "This email is already registered. If it's yours, try logging in or reset your password."
          : s;
        else if (low.includes('user') || low.includes('username')) fields.userName = /already|exist|taken|in use/i.test(low)
          ? 'This username is already taken. Please choose a different username.'
          : s;
        else globalMsg = globalMsg ? `${globalMsg}\n${s}` : s;
      }
      return { fields, globalMsg };
    }

    if (result.message) globalMsg = String(result.message);
    else if (result.error) globalMsg = String(result.error);
    else if (response) globalMsg = `${response.status} ${response.statusText}`;

    return { fields, globalMsg };
  };

  const validateFields = (): Record<string, string> => {
    const errors: Record<string, string> = {};

    if (!form.userName || form.userName.trim() === '') {
      errors.userName = 'Username is required.';
    } else if (form.userName.trim().length < 3) {
      errors.userName = 'Username must be at least 3 characters long.';
    } else if (form.userName.trim().length > 20) {
      errors.userName = 'Username must not exceed 20 characters.';
    }

    if (!form.fullName || form.fullName.trim() === '') {
      errors.fullName = 'Full name is required.';
    } else if (form.fullName.trim().length < 3) {
      errors.fullName = 'Full name must be at least 3 characters long.';
    } else if (form.fullName.trim().length > 20) {
      errors.fullName = 'Full name must not exceed 20 characters.';
    }

    if (!form.email || form.email.trim() === '') {
      errors.email = 'Email is required.';
    } else {
      const emailRe = /\S+@\S+\.\S+/;
      if (!emailRe.test(form.email.trim())) {
        errors.email = 'A valid email is required.';
      }
    }

    if (form.phone && form.phone.trim() !== '') {
      const phoneRe = /^\+?[1-9]\d{1,14}$/;
      if (!phoneRe.test(form.phone.trim())) {
        errors.phone = 'A valid phone number is required.';
      }
    }

    if (!form.password) {
      errors.password = 'Password is required.';
    } else if (form.password.length < 6) {
      errors.password = 'Password must be at least 6 characters long.';
    }

    if (form.confirmPassword !== form.password) {
      errors.confirmPassword = 'Passwords do not match.';
    }

    return errors;
  };

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccessMessage(null);

    setLoading(true);

    const payload = {
      fullName: form.fullName,
      userName: form.userName,
      email: form.email,
      phoneNumber: form.phone ? form.phone : null,
      password: form.password,
      confirmPassword: form.confirmPassword,
      phoneVerificationChannel: contactMethod === 'sms' ? 1 : 2,
    } as any;

    try {
      const clientFieldErrors = validateFields();
      if (Object.keys(clientFieldErrors).length) {
        setFieldErrors(clientFieldErrors);
        setLoading(false);
        return;
      }

      const res = await fetch('http://localhost:5075/api/Accounts/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
      });

      let result: any = null;
      try { result = await res.json(); } catch (err) { }

      if (!res.ok) {
        const mapped = mapApiErrorsToFields(result, res);
        if (Object.keys(mapped.fields).length) {
          setFieldErrors(mapped.fields);
        }
        if (mapped.globalMsg) setError(mapped.globalMsg);
        else if (!Object.keys(mapped.fields).length) setError(parseApiError(result, res));
        setLoading(false);
        return;
      }

      if (result?.isSuccess) {
        setSuccessMessage('Registration successful. Please verify your email or login.');
        navigate('/verify-email', { state: { email: form.email, phone: form.phone, contactMethod } });
      } else {
        const mapped = mapApiErrorsToFields(result, res);
        if (Object.keys(mapped.fields).length) setFieldErrors(mapped.fields);
        if (mapped.globalMsg) setError(mapped.globalMsg);
        else if (!Object.keys(mapped.fields).length) setError(parseApiError(result, res));
      }
    } catch (err: any) {
      setError(err?.message || 'Network error. Please try again.');
    } finally {
      setLoading(false);
    }
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
            <h2 className="text-xl font-semibold text-foreground">Register</h2>
            <p className="text-sm text-muted-foreground mt-1">Create a new account</p>
          </div>

          <form onSubmit={handleRegister} className="space-y-4">
            <AuthInput
              label="Full name"
              icon={User}
              placeholder="John Doe"
              value={form.fullName}
              onChange={(e) => update('fullName', e.target.value)}
              error={fieldErrors.fullName}
            />
            <AuthInput
              label="Username"
              icon={AtSign}
              placeholder="username"
              value={form.userName}
              onChange={(e) => update('userName', e.target.value)}
              error={fieldErrors.userName}
            />
            {fieldErrors.userName && /taken|already|exist|in use/i.test(fieldErrors.userName) && (
              <div className="mt-2 text-sm">
                <p className="text-xs text-muted-foreground">Suggestions:</p>
                <div className="flex gap-2 mt-2">
                  {generateUsernameSuggestions(form.userName).map(s => (
                    <button
                      type="button"
                      key={s}
                      onClick={() => update('userName', s)}
                      className="px-3 py-1 rounded-md bg-secondary text-sm hover:bg-secondary/80"
                    >
                      {s}
                    </button>
                  ))}
                </div>
              </div>
            )}
            <AuthInput
              label="Email"
              icon={Mail}
              type="email"
              placeholder="email@example.com"
              value={form.email}
              onChange={(e) => update('email', e.target.value)}
              error={fieldErrors.email}
            />
            {fieldErrors.email && /already|registered|exist|in use/i.test(fieldErrors.email) && (
              <div className="mt-2 text-sm">
                <p className="text-xs text-muted-foreground">If this is your email:</p>
                <div className="flex gap-2 mt-2">
                  <button
                    type="button"
                    onClick={() => navigate('/login', { state: { email: form.email } })}
                    className="px-3 py-1 rounded-md bg-secondary text-sm hover:bg-secondary/80"
                  >
                    Go to login
                  </button>
                  <button
                    type="button"
                    onClick={() => navigate('/forgot-password', { state: { email: form.email } })}
                    className="px-3 py-1 rounded-md bg-secondary text-sm hover:bg-secondary/80"
                  >
                    Reset password
                  </button>
                </div>
              </div>
            )}
            <AuthInput
              label="Phone (optional)"
              icon={Phone}
              type="tel"
              placeholder="+1 555 555 5555"
              value={form.phone}
              onChange={(e) => update('phone', e.target.value)}
            />

            {showContactChoice && (
              <div className="space-y-2 animate-fade-in">
                <label className="text-sm font-medium text-muted-foreground">Contact method</label>
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
              label="Password"
              icon={Lock}
              type="password"
              placeholder="••••••••"
              value={form.password}
              onChange={(e) => update('password', e.target.value)}
            />
            <AuthInput
              label="Confirm password"
              icon={Lock}
              type="password"
              placeholder="••••••••"
              value={form.confirmPassword}
              onChange={(e) => update('confirmPassword', e.target.value)}
            />

            {error && <p className="text-sm text-destructive whitespace-pre-wrap">{error}</p>}
            {successMessage && <p className="text-sm text-primary">{successMessage}</p>}

            <button
              type="submit"
              disabled={loading}
              className="w-full h-11 rounded-lg font-medium text-sm text-primary-foreground transition-all duration-200 hover:opacity-90 active:scale-[0.98]"
              style={{ background: 'var(--gradient-primary)' }}
            >
              {loading ? 'Registering...' : 'Register'}
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
            Already have an account?{' '}
            <Link to="/login" className="text-primary hover:underline font-medium">
              Sign in
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default RegisterPage;
