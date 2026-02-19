import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { User, Mail, Lock, Phone, AtSign, Eye } from 'lucide-react';
import ClosedEye from '@/components/icons/ClosedEye';
import AtlasLogo from '@/components/AtlasLogo';
import AuthInput from '@/components/auth/AuthInput';
import OAuthButton from '@/components/auth/OAuthButton';

function parseApiError(result: any, response?: Response) {
  if (!result) return response?.statusText || 'An unknown error occurred.';

  if (Array.isArray(result.errors) && result.errors.length > 0) {
    return result.errors.join('\n');
  }

  if (result.errors && typeof result.errors === 'object') {
    try {
      const parts: string[] = [];
      for (const key of Object.keys(result.errors)) {
        const v = result.errors[key];
        const arr = Array.isArray(v) ? v : [String(v)];
        parts.push(arr.join('\n'));
      }
      if (parts.length) return parts.join('\n');
    } catch (e) { }
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
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const update = (field: string, value: string) => {
    setForm(prev => ({ ...prev, [field]: value }));
    setFieldErrors(prev => {
      if (!prev || !prev[field]) return prev;
      const copy = { ...prev } as Record<string, string>;
      delete copy[field];
      return copy;
    });
    if (field === 'phone') {
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

    if (result.fieldErrors && typeof result.fieldErrors === 'object') {
      for (const key of Object.keys(result.fieldErrors)) {
        const v = result.fieldErrors[key];
        fields[key] = Array.isArray(v) ? v.join(', ') : String(v);
      }
      return { fields, globalMsg };
    }

    if (Array.isArray(result.errorCodes) && result.errorCodes.length) {
      for (const code of result.errorCodes) {
        const c = String(code).toUpperCase();
        if (c === 'EMAIL_REQUIRED' || c === 'INVALID_EMAIL') fields.email = 'A valid email is required.';
        if (c === 'USERNAME_TAKEN' || c === 'USERNAME_EXISTS') fields.userName = 'This username is already taken. Please choose a different username.';
        if (c === 'EMAIL_EXISTS' || c === 'EMAIL_TAKEN') fields.email = "This email is already registered. If it's yours, try logging in or reset your password.";
      }
      return { fields, globalMsg };
    }

    if (response?.status === 409) {
      const msgs: string[] = [];
      if (result.errors && typeof result.errors === 'object') {
        for (const k of Object.keys(result.errors)) {
          const v = result.errors[k];
          if (Array.isArray(v)) msgs.push(...v.map(String));
          else msgs.push(String(v));
        }
      } else if (Array.isArray(result.errors) && result.errors.length) {
        msgs.push(...result.errors.map(String));
      } else if (result.message) {
        msgs.push(String(result.message));
      } else if (result.error) {
        msgs.push(String(result.error));
      }

      const joined = msgs.join('\n');
      if (/email/i.test(joined)) fields.email = "This email is already registered. If it's yours, try logging in or reset your password.";
      if (/user|username/i.test(joined)) fields.userName = 'This username is already taken. Please choose a different username.';
      if (Object.keys(fields).length) return { fields, globalMsg };
    }

    if (result.errors && typeof result.errors === 'object') {
      for (const key of Object.keys(result.errors)) {
        const v = result.errors[key];
        fields[key] = Array.isArray(v) ? v.join(', ') : String(v);
      }
      return { fields, globalMsg };
    }

    if (Array.isArray(result.errors) && result.errors.length > 0) {
      const arr = result.errors as string[];
      for (const s of arr) {
        const low = s.toLowerCase();
        if (low.includes('email')) fields.email = /already|exist|taken|in use/i.test(low) ? "This email is already registered. If it's yours, try logging in or reset your password." : s;
        else if (low.includes('user') || low.includes('username')) fields.userName = /already|exist|taken|in use/i.test(low) ? 'This username is already taken. Please choose a different username.' : s;
        else globalMsg = globalMsg ? `${globalMsg}\n${s}` : s;
      }
      return { fields, globalMsg };
    }

    if (result.error && typeof result.error === 'string') {
      const errStr = String(result.error);
      const low = errStr.toLowerCase();
      if (/email/.test(low) && /already|exist|taken|in use/.test(low)) {
        fields.email = "This email is already registered. If it's yours, try logging in or reset your password.";
        return { fields, globalMsg };
      }
      if (/(user|username)/.test(low) && /already|exist|taken|in use/.test(low)) {
        fields.userName = 'This username is already taken. Please choose a different username.';
        return { fields, globalMsg };
      }
      return { fields, globalMsg: errStr };
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
      if (!emailRe.test(form.email.trim())) errors.email = 'A valid email is required.';
    }

    if (form.phone) {
      const phoneRe = /^\+?[1-9]\d{1,14}$/;
      if (!phoneRe.test(form.phone)) errors.phone = 'A valid phone number is required.';
    }

    if (!form.password) errors.password = 'Password is required.';
    else if (form.password.length < 6) errors.password = 'Password must be at least 6 characters long.';

    if (form.confirmPassword !== form.password) errors.confirmPassword = 'Passwords do not match.';

    return errors;
  };

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccessMessage(null);

    const clientFieldErrors = validateFields();
    if (Object.keys(clientFieldErrors).length) {
      setFieldErrors(clientFieldErrors);
      return;
    }

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

      const topSuccess = result?.success ?? result?.isSuccess ?? false;
      const data = result?.data ?? null;
      let registerSucceeded = false;
      if (topSuccess) {
        if (data === true) registerSucceeded = true;
        else if (data && typeof data === 'object' && data.success === true) registerSucceeded = true;
        else if (data && typeof data === 'object' && (data.requiresEmailVerification || data.requiresPhoneVerification || data.telegramBotLink)) registerSucceeded = true;
        else registerSucceeded = true;
      }

      if (registerSucceeded) {
        if (data && typeof data === 'object' && data.telegramBotLink) {
          navigate('/verify-email', { state: { email: form.email, phone: form.phone, telegramBotLink: data.telegramBotLink } });
        } else {
          navigate('/verify-email', { state: { email: form.email, phone: form.phone } });
        }
      } else {
        const mapped = mapApiErrorsToFields(result, res);
        if (Object.keys(mapped.fields).length) setFieldErrors(mapped.fields);
        if (mapped.globalMsg) setError(mapped.globalMsg);
        else setError(parseApiError(result, res));
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
          <div>
            <h2 className="text-xl font-semibold text-foreground">Create account</h2>
            <p className="text-sm text-muted-foreground mt-1">Sign up and get started with Atlas</p>
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

            <AuthInput
              label="Phone (optional)"
              icon={Phone}
              placeholder="+994501234567"
              value={form.phone}
              onChange={(e) => update('phone', e.target.value)}
              error={fieldErrors.phone}
            />

            <div className="flex gap-2">
              <button type="button" onClick={() => setContactMethod('sms')} className={`px-3 py-1 rounded-md ${contactMethod === 'sms' ? 'bg-primary text-primary-foreground' : 'bg-secondary'}`}>SMS</button>
              <button type="button" onClick={() => setContactMethod('telegram')} className={`px-3 py-1 rounded-md ${contactMethod === 'telegram' ? 'bg-primary text-primary-foreground' : 'bg-secondary'}`}>Telegram</button>
            </div>

            <AuthInput
              label="Password"
              icon={Lock}
              type={showPassword ? 'text' : 'password'}
              placeholder="••••••••"
              value={form.password}
              onChange={(e) => update('password', e.target.value)}
              error={fieldErrors.password}
              suffix={<button type="button" onClick={() => setShowPassword(s => !s)} className="p-1 text-muted-foreground">{showPassword ? <ClosedEye className="w-4 h-4" /> : <Eye className="w-4 h-4" />}</button>}
            />

            <AuthInput
              label="Confirm password"
              icon={Lock}
              type={showConfirmPassword ? 'text' : 'password'}
              placeholder="••••••••"
              value={form.confirmPassword}
              onChange={(e) => update('confirmPassword', e.target.value)}
              error={fieldErrors.confirmPassword}
              suffix={<button type="button" onClick={() => setShowConfirmPassword(s => !s)} className="p-1 text-muted-foreground">{showConfirmPassword ? <ClosedEye className="w-4 h-4" /> : <Eye className="w-4 h-4" />}</button>}
            />

            {error && <p className="text-sm text-destructive whitespace-pre-wrap">{error}</p>}
            {successMessage && <p className="text-sm text-primary">{successMessage}</p>}

            <button type="submit" disabled={loading} className="w-full h-11 rounded-lg font-medium text-sm text-primary-foreground transition-all duration-200 hover:opacity-90 active:scale-[0.98]" style={{ background: 'var(--gradient-primary)' }}>
              {loading ? 'Creating account...' : 'Create account'}
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
