import * as React from "react";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "@/hooks/useAuth";
import SocialButtons from "@/components/ui/SocialButtons";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

const roles = ["Developer", "Designer", "SecOps", "Marketer", "TeamLeader"];

export default function RegisterPage() {
  const navigate = useNavigate();
  const { register } = useAuth();
  const [form, setForm] = useState({ FullName: "", UserName: "", Email: "", PhoneNumber: "", Password: "", ConfirmPassword: "", Role: roles[0], PhoneVerificationChannel: "" });
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
    if (!roles.includes(form.Role)) return "Invalid role.";
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
        Role: form.Role,
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
    window.location.href = `${import.meta.env.VITE_API_BASE || 'http://localhost:5000'}/api/accounts/external/${provider}`;
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

          <Input name="PhoneNumber" placeholder="Phone (optional)" value={form.PhoneNumber} onChange={handleChange} />
          {form.PhoneNumber && (
            <div className="flex gap-2 mt-2">
              <button type="button" onClick={() => setForm(s => ({ ...s, PhoneVerificationChannel: 'Sms' }))} className={`px-3 py-1 rounded-md ${form.PhoneVerificationChannel === 'Sms' ? 'bg-primary text-primary-foreground' : 'bg-secondary'}`}>SMS</button>
              <button type="button" onClick={() => setForm(s => ({ ...s, PhoneVerificationChannel: 'Telegram' }))} className={`px-3 py-1 rounded-md ${form.PhoneVerificationChannel === 'Telegram' ? 'bg-primary text-primary-foreground' : 'bg-secondary'}`}>Telegram</button>
            </div>
          )}
          {fieldErrors.PhoneNumber && <div className="text-sm text-destructive">{fieldErrors.PhoneNumber}</div>}

          <Input type="password" name="Password" placeholder="Password" value={form.Password} onChange={handleChange} />
          {fieldErrors.Password && <div className="text-sm text-destructive">{fieldErrors.Password}</div>}
          <Input type="password" name="ConfirmPassword" placeholder="Confirm password" value={form.ConfirmPassword} onChange={handleChange} />

          <div>
            <label className="block text-sm mb-1">Role</label>
            <select name="Role" value={form.Role} onChange={handleChange} className="w-full rounded-md border px-3 py-2">
              {roles.map(r => <option key={r} value={r}>{r}</option>)}
            </select>
          </div>

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
