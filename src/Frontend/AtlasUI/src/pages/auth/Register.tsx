import * as React from "react";
import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "@/hooks/useAuth";
import SocialButtons from "@/components/ui/SocialButtons";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import PhoneInput from "@/components/auth/PhoneInput";
import { Phone } from 'lucide-react';

export default function RegisterPage() {
  const navigate = useNavigate();
  const { register } = useAuth();
  const [form, setForm] = useState({ FullName: "", UserName: "", Email: "", PhoneNumber: "", Password: "", ConfirmPassword: "", PhoneVerificationChannel: "" });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    setForm((s) => ({ ...s, [e.target.name]: e.target.value }));
    // clear field error for this field
    setFieldErrors(prev => {
      if (!prev || !prev[e.target.name]) return prev;
      const copy = { ...prev } as Record<string, string>;
      delete copy[e.target.name];
      return copy;
    });
  };

  const validate = () => {
    if (!form.FullName || form.FullName.length < 3) return "Full name must be at least 3 characters.";
    if (!form.UserName || form.UserName.length < 3) return "Username must be at least 3 characters.";
    if (!form.Email || !/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(form.Email)) return "Invalid email.";
    if (!form.Password || form.Password.length < 6) return "Password must be at least 6 characters.";
    if (form.Password !== form.ConfirmPassword) return "Passwords do not match.";
    if (form.PhoneNumber && !/^\+?[1-9]\d{1,14}$/.test(form.PhoneNumber)) return "Invalid phone number.";
    return null;
  };

  const mapApiErrorsToFields = (err: any) => {
    const fields: Record<string, string> = {};
    if (!err) return fields;
    if (err.errors && typeof err.errors === 'object') {
      for (const k of Object.keys(err.errors)) {
        const v = err.errors[k];
        fields[k] = Array.isArray(v) ? v.join(', ') : String(v);
      }
    }
    if (err.message && typeof err.message === 'string') {
      const msg = err.message;
      if (/email/i.test(msg) && /already|exist|taken|in use/i.test(msg)) fields.Email = "This email is already registered.";
      if (/user|username/i.test(msg) && /already|exist|taken|in use/i.test(msg)) fields.UserName = "This username is already taken.";
    }
    return fields;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setFieldErrors({});
    const v = validate();
    if (v) {
      setError(v);
      return;
    }
    setLoading(true);
    try {
      const payload: any = {
        FullName: form.FullName,
        UserName: form.UserName,
        Email: form.Email,
        PhoneNumber: form.PhoneNumber || null,
        Password: form.Password,
        ConfirmPassword: form.ConfirmPassword,
        PhoneVerificationChannel: form.PhoneNumber ? (form.PhoneVerificationChannel || 'Sms') : null,
      };

      const data = await register(payload);

      // backend may return shape with success flags or RegisterResponseDto
      const requiresEmail = data?.requiresEmailVerification ?? data?.RequiresEmailVerification ?? false;
      const requiresPhone = data?.requiresPhoneVerification ?? data?.RequiresPhoneVerification ?? false;
      const telegramBotLink = data?.telegramBotLink ?? data?.TelegramBotLink ?? null;

      if (requiresEmail) {
        navigate('/verify-email', { state: { email: form.Email } });
        return;
      }
      if (requiresPhone) {
        navigate('/verify-phone', { state: { phone: form.PhoneNumber, telegramBotLink } });
        return;
      }

      // fallback: if new user -> onboarding, else go to login
      if (data?.isNewUser ?? data?.IsNewUser) {
        navigate('/onboarding');
      } else {
        navigate('/login');
      }
    } catch (err: any) {
      const mapped = err?.data ?? err;
      const fields = mapApiErrorsToFields(mapped);
      if (Object.keys(fields).length) setFieldErrors(fields);
      setError(err?.data?.message || err?.message || 'Registration failed');
    } finally {
      setLoading(false);
    }
  };

  const handleExternal = (provider: 'google' | 'github') => {
    // redirect to backend external endpoint (keeps existing behaviour)
    window.location.href = `${import.meta.env.VITE_API_BASE || 'http://localhost:5075'}/api/accounts/external/${provider}`;
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-background">
      <div className="w-full max-w-md p-8 rounded-lg shadow-lg bg-card">
        <h1 className="text-2xl font-semibold mb-4">Create your account</h1>
        <p className="text-sm text-muted-foreground mb-6">Start your journey with Momentum.</p>

        {error && <div className="mb-4 text-sm text-destructive">{error}</div>}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
            <Input name="FullName" placeholder="Full name" value={form.FullName} onChange={handleChange} />
            <Input name="UserName" placeholder="Username" value={form.UserName} onChange={handleChange} />
          </div>
          {fieldErrors.FullName && <div className="text-sm text-destructive">{fieldErrors.FullName}</div>}
          {fieldErrors.UserName && <div className="text-sm text-destructive">{fieldErrors.UserName}</div>}

          <Input name="Email" placeholder="Email" value={form.Email} onChange={handleChange} />
          {fieldErrors.Email && <div className="text-sm text-destructive">{fieldErrors.Email}</div>}

          <PhoneInput value={form.PhoneNumber} onChange={(v) => setForm(s => ({ ...s, PhoneNumber: v }))} placeholder="501234567" />
          {/* channel buttons shown directly under phone input (each half width of input) */}
          {form.PhoneNumber && (
            <div className="mt-3 flex gap-3">
              <button type="button" onClick={() => setForm(s => ({ ...s, PhoneVerificationChannel: 'Sms' }))} className={`flex-1 flex items-center justify-center gap-3 px-4 py-3 rounded-md border ${form.PhoneVerificationChannel === 'Sms' ? 'bg-primary text-primary-foreground' : 'bg-secondary'}`}>
                <Phone className="w-5 h-5" /> Send via SMS
              </button>
              <button type="button" onClick={() => setForm(s => ({ ...s, PhoneVerificationChannel: 'Telegram' }))} className={`flex-1 flex items-center justify-center gap-3 px-4 py-3 rounded-md border ${form.PhoneVerificationChannel === 'Telegram' ? 'bg-primary text-primary-foreground' : 'bg-secondary'}`}>
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 240 240" className="w-5 h-5"><circle cx="120" cy="120" r="120" fill="#37AEE2" /><path d="M84 124l65-27c2-1 4 0 3 2l-18 69c-1 4-5 5-9 3l-23-16-11 11c-1 1-3 1-4 0l-1-26-26-6c-4-1-4-5 2-7l99-39c3-1 6 1 5 4l-37 119c-1 3-5 4-8 3l-69-31c-3-1-5-3-4-6l23-93c1-3 4-4 6-3z" fill="#fff"/></svg>
                Send via Telegram
              </button>
            </div>
          )}
          {fieldErrors.PhoneNumber && <div className="text-sm text-destructive">{fieldErrors.PhoneNumber}</div>}

          <Input type="password" name="Password" placeholder="Password" value={form.Password} onChange={handleChange} />
          {fieldErrors.Password && <div className="text-sm text-destructive">{fieldErrors.Password}</div>}
          <Input type="password" name="ConfirmPassword" placeholder="Confirm password" value={form.ConfirmPassword} onChange={handleChange} />

          <Button type="submit" className="w-full" disabled={loading}>{loading ? 'Creating account...' : 'Create account'}</Button>
        </form>

        <div className="my-4 text-center text-sm text-muted-foreground">Or continue with</div>
        <SocialButtons
          onGoogle={() => handleExternal('google')}
          onGithub={() => handleExternal('github')}
        />

        <div className="mt-6 text-center">
          <span className="text-sm">Already have an account? </span>
          <a className="text-sm text-primary hover:underline" onClick={() => navigate('/login')}>Sign in</a>
        </div>
      </div>
    </div>
  );
}
