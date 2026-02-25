import * as React from "react";
import { useState } from "react";
import { accounts } from "@/lib/api";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { useLocation, useNavigate } from "react-router-dom";

export default function ResetPassword() {
  const loc: any = useLocation();
  const navigate = useNavigate();
  const [email, setEmail] = useState(loc?.state?.email || "");
  const [resetToken, setResetToken] = useState(loc?.state?.resetToken || "");
  const [password, setPassword] = useState("");
  const [confirm, setConfirm] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const strong = (p: string) => /(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}/.test(p);

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    if (!strong(password)) return setError('Password must be at least 8 chars, include upper, lower, digit and special char');
    if (password !== confirm) return setError('Passwords do not match');
    setLoading(true);
    try {
      await accounts.resetPassword({ Email: email, ResetToken: resetToken, NewPassword: password, ConfirmPassword: confirm });
      navigate('/login');
    } catch (err: any) {
      setError(err?.data?.errors?.map((x: any) => x.message).join('\n') || 'Reset failed');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-background">
      <div className="w-full max-w-md p-8 rounded-lg shadow-lg bg-card">
        <h1 className="text-2xl font-semibold mb-4">Choose a new password</h1>
        <p className="text-sm text-muted-foreground mb-6">Provide the reset token and choose a strong password.</p>
        {error && <div className="mb-4 text-sm text-destructive">{error}</div>}
        <form onSubmit={submit} className="space-y-4">
          <Input name="Email" placeholder="Email" value={email} onChange={(e) => setEmail(e.target.value)} />
          <Input name="ResetToken" placeholder="Reset token" value={resetToken} onChange={(e) => setResetToken(e.target.value)} />
          <Input type="password" name="NewPassword" placeholder="New password" value={password} onChange={(e) => setPassword(e.target.value)} />
          <Input type="password" name="ConfirmPassword" placeholder="Confirm password" value={confirm} onChange={(e) => setConfirm(e.target.value)} />
          <Button className="w-full" type="submit" disabled={loading}>{loading ? 'Resetting...' : 'Reset password'}</Button>
        </form>
      </div>
    </div>
  );
}

