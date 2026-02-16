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
    } catch (e) {}
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
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [telegramBotLink, setTelegramBotLink] = useState<string | null>(null);
  const [registrationSuccessInfo, setRegistrationSuccessInfo] = useState<any | null>(null);

  const [phoneResendCooldown, setPhoneResendCooldown] = useState(0);
  const [phoneResendLoading, setPhoneResendLoading] = useState(false);
  const [phoneResendMessage, setPhoneResendMessage] = useState<string | null>(null);

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

    // If backend returns a single error string (no fieldErrors / errorCodes), parse it
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
      // fallback: return the error as global message
      return { fields, globalMsg: errStr };
    }

    // If backend provides explicit fieldErrors object, prefer it
    if (result.fieldErrors && typeof result.fieldErrors === 'object') {
      for (const key of Object.keys(result.fieldErrors)) {
        const v = result.fieldErrors[key];
        fields[key] = Array.isArray(v) ? v.join(', ') : String(v);
      }
      return { fields, globalMsg };
    }

    // If backend provides errorCodes, produce friendly messages when possible
    if (Array.isArray(result.errorCodes) && result.errorCodes.length) {
      for (const code of result.errorCodes) {
        const c = String(code).toUpperCase();
        if (c === 'EMAIL_REQUIRED' || c === 'INVALID_EMAIL') fields.email = 'A valid email is required.';
        if (c === 'USERNAME_TAKEN' || c === 'USERNAME_EXISTS') fields.userName = 'This username is already taken. Please choose a different username.';
        if (c === 'EMAIL_EXISTS' || c === 'EMAIL_TAKEN') fields.email = "This email is already registered. If it's yours, try logging in or reset your password.";
      }
      if (Object.keys(fields).length) return { fields, globalMsg };
    }

    // Special handling for 409 Conflict (unique constraint) where backend may return generic messages
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
      if (/email/i.test(joined)) {
        fields.email = "This email is already registered. If it's yours, try logging in or reset your password.";
      }
      if (/user|username/i.test(joined)) {
        fields.userName = 'This username is already taken. Please choose a different username.';
      }
      if (Object.keys(fields).length) return { fields, globalMsg };
      // fallback to put full message as global
      if (joined) return { fields, globalMsg: joined };
    }

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
    setTelegramBotLink(null);
    setRegistrationSuccessInfo(null);

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

      // Normalize success detection and payload
      const topSuccess = result?.success ?? result?.isSuccess ?? false;
      const data = result?.data ?? null;
      let registerSucceeded = false;
      if (topSuccess) {
        if (data === true) registerSucceeded = true;
        else if (data && typeof data === 'object' && data.success === true) registerSucceeded = true;
        else if (data && typeof data === 'object' && (data.requiresEmailVerification || data.requiresPhoneVerification || data.telegramBotLink)) registerSucceeded = true;
        else registerSucceeded = true; // fallback
      }

      if (registerSucceeded) {
        // If backend provided telegramBotLink (phone channel = telegram), show it
        if (data && typeof data === 'object' && data.telegramBotLink) {
          setTelegramBotLink(String(data.telegramBotLink));
          setRegistrationSuccessInfo(data);
          setSuccessMessage('Registration successful. Please complete verification via Telegram.');
          // keep user on page and show link UI
        } else {
          // For SMS or normal flows, navigate to verification page
          setSuccessMessage('Registration successful. Please verify your email or phone.');
          // Pass email and phone to verify page
          if (contactMethod === 'sms') {
            navigate('/verify-phone', { state: { phone: form.phone } });
          } else {
            navigate('/verify-email', { state: { email: form.email } });
          }
        }
      } else {
        // Not successful at application level
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

  const startPhoneResendCooldown = () => {
    setPhoneResendCooldown(60);
    const t = setInterval(() => {
      setPhoneResendCooldown(prev => {
        if (prev <= 1) { clearInterval(t); return 0; }
        return prev - 1;
      });
    }, 1000);
  };

  const handleResendPhoneFromRegister = async () => {
    setPhoneResendMessage(null);
    setPhoneResendLoading(true);
    const phone = form.phone?.trim();
    if (!phone) { setPhoneResendMessage('Phone number is missing.'); setPhoneResendLoading(false); return; }
    const phoneRe = /^\+\d{1,3}\d{4,14}(?:x.+)?$/;
    if (!phoneRe.test(phone)) { setPhoneResendMessage('Phone number must be in valid international format.'); setPhoneResendLoading(false); return; }
    try {
      startPhoneResendCooldown();
      const res = await fetch('http://localhost:5075/api/accounts/resend-phone-verification-code', {
        method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ phoneNumber: phone, channel: 2 })
      });
      let result: any = null;
      try { result = await res.json(); } catch (e) {}
      if (!res.ok) {
        setPhoneResendMessage(parseApiError(result, res));
      } else {
        const successFlag = result?.success ?? result?.isSuccess ?? false;
        if (successFlag) setPhoneResendMessage('Verification code resent via Telegram. Check your Telegram.');
        else setPhoneResendMessage(parseApiError(result, res));
      }
    } catch (err: any) {
      setPhoneResendMessage(err?.message || 'Network error.');
    } finally { setPhoneResendLoading(false); }
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
                    <span className="inline-flex items-center gap-2 justify-center">
                      <Phone className="w-4 h-4" />
                      SMS
                    </span>
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
                    <span className="inline-flex items-center gap-2 justify-center">
                      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 240 240" className="w-4 h-4" aria-hidden>
                        <path fill="currentColor" d="M120 0C53.7 0 0 53.7 0 120s53.7 120 120 120 120-53.7 120-120S186.3 0 120 0zm54.8 83.9l-20.7 97.5c-1.6 6.9-5.9 8.6-11.9 5.4l-33-24.3-15.9 15.3c-1.8 1.8-3.3 3.3-6.7 3.3l2.4-34.4 62.6-56.3c2.7-2.4-.6-3.7-4.2-1.3L70.5 124l-33.9-10.6c-7.3-2.3-7.4-7.3 1.5-10.8L173 69.2c6.5-2.2 12.2 1.5 11.8 14.7z" />
                      </svg>
                      Telegram
                    </span>
                  </button>
                </div>
              </div>
            )}

            <AuthInput
              label="Password"
              icon={Lock}
              type={showPassword ? 'text' : 'password'}
              placeholder="••••••••"
              value={form.password}
              onChange={(e) => update('password', e.target.value)}
              error={fieldErrors.password}
              suffix={
                <button type="button" onClick={() => setShowPassword(s => !s)} className="p-1 text-muted-foreground">
                  {showPassword ? <ClosedEye className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                </button>
              }
            />
            <AuthInput
              label="Confirm password"
              icon={Lock}
              type={showConfirmPassword ? 'text' : 'password'}
              placeholder="••••••••"
              value={form.confirmPassword}
              onChange={(e) => update('confirmPassword', e.target.value)}
              error={fieldErrors.confirmPassword}
              suffix={
                <button type="button" onClick={() => setShowConfirmPassword(s => !s)} className="p-1 text-muted-foreground">
                  {showConfirmPassword ? <ClosedEye className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                </button>
              }
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

        {/* Render Telegram link block when available */}
        {telegramBotLink && registrationSuccessInfo?.telegramBotLink && (
          <div className="mt-6 p-4 rounded-lg bg-primary/10 text-primary text-sm">
            <p className="font-medium">Almost done!</p>
            <p className="mt-1">Please complete your registration by chatting with our Telegram bot.</p>
            <a
              href={telegramBotLink}
              target="_blank"
              rel="noopener noreferrer"
              className="mt-2 inline-block text-center px-3 py-2 rounded-md bg-primary text-primary-foreground hover:bg-primary/90 transition-all duration-200"
            >
              Open Telegram Bot
            </a>
            <div className="mt-3 flex gap-2">
              <button
                type="button"
                onClick={() => navigate('/verify-phone', { state: { phone: form.phone } })}
                className="px-3 py-2 rounded-md bg-secondary"
              >
                I received the code
              </button>
              <button
                type="button"
                onClick={handleResendPhoneFromRegister}
                disabled={phoneResendCooldown > 0 || phoneResendLoading}
                className="px-3 py-2 rounded-md bg-primary text-primary-foreground disabled:opacity-60"
              >
                {phoneResendCooldown > 0 ? `Resend (${phoneResendCooldown}s)` : (phoneResendLoading ? 'Resending...' : 'Resend code')}
              </button>
            </div>
            {phoneResendMessage && <p className="mt-2 text-sm text-muted-foreground">{phoneResendMessage}</p>}
          </div>
        )}
      </div>
    </div>
  );
};

export default RegisterPage;
