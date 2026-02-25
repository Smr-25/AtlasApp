import * as React from "react";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "@/hooks/useAuth";
import SocialButtons from "@/components/ui/SocialButtons";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

export default function LoginPage() {
  const navigate = useNavigate();
  const { login } = useAuth();
  const [form, setForm] = useState({ Identifier: "", Password: "" });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm((s) => ({ ...s, [e.target.name]: e.target.value }));
  };

  const isEmail = (s: string) => /^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(s);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    if (!form.Password || !form.Identifier) {
      setError("Email/USERNAME and password are required.");
      return;
    }
    setLoading(true);
    try {
      const payload: any = { Password: form.Password };
      if (isEmail(form.Identifier)) payload.Email = form.Identifier;
      else payload.UserName = form.Identifier;

      const data = await login(payload);
      if (data?.IsNewUser) {
        navigate('/onboarding');
      } else {
        navigate('/');
      }
    } catch (err: any) {
      setError(err?.data?.errors?.map((x: any) => x.message).join(" \n") || err.message || "Login failed");
    } finally {
      setLoading(false);
    }
  };

  const handleExternal = (provider: 'google' | 'github') => {
    window.location.href = `${import.meta.env.VITE_API_BASE || 'http://localhost:5000'}/api/accounts/external/${provider}`;
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-background">
      <div className="w-full max-w-md p-8 rounded-lg shadow-lg bg-card">
        <h1 className="text-2xl font-semibold mb-4">Sign in to your account</h1>
        <p className="text-sm text-muted-foreground mb-6">Welcome back — please enter your details.</p>

        {error && <div className="mb-4 text-sm text-destructive">{error}</div>}

        <form onSubmit={handleSubmit} className="space-y-4">
          <Input name="Identifier" placeholder="Email address or USERNAME" value={form.Identifier} onChange={handleChange} />
          <Input type="password" name="Password" placeholder="Password" value={form.Password} onChange={handleChange} />

          <div className="flex items-center justify-between">
            <div />
            <a className="text-sm text-primary hover:underline" href="/forgot-password">Forgot password?</a>
          </div>

          <Button type="submit" className="w-full" disabled={loading}>{loading ? 'Signing in...' : 'Sign in'}</Button>
        </form>

        <div className="my-4 text-center text-sm text-muted-foreground">Or continue with</div>
        <SocialButtons
          onGoogle={() => handleExternal('google')}
          onGithub={() => handleExternal('github')}
        />

        <div className="mt-6 text-center">
          <span className="text-sm">Don’t have an account? </span>
          <a className="text-sm text-primary hover:underline" onClick={() => navigate('/register')}>Sign up</a>
        </div>
      </div>
    </div>
  );
}
