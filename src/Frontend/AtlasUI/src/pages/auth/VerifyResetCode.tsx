import * as React from "react";
import { useState } from "react";
import { accounts } from "@/lib/api";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { useNavigate } from "react-router-dom";

export default function VerifyResetCode() {
  const [email, setEmail] = useState("");
  const [code, setCode] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    if (!/^[0-9]{6}$/.test(code)) return setError('Code must be 6 digits');
    setLoading(true);
    try {
      const data = await accounts.verifyResetCode({ Email: email, VerificationCode: code });
      // redirect to reset-password with token
      navigate('/reset-password', { state: { email, resetToken: data.ResetToken } });
    } catch (err: any) {
      setError(err?.data?.errors?.map((x: any) => x.message).join('\n') || 'Verification failed');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-background">
      <div className="w-full max-w-md p-8 rounded-lg shadow-lg bg-card">
        <h1 className="text-2xl font-semibold mb-4">Verify reset code</h1>
        <p className="text-sm text-muted-foreground mb-6">Enter the 6-digit code sent to your email.</p>
        {error && <div className="mb-4 text-sm text-destructive">{error}</div>}
        <form onSubmit={submit} className="space-y-4">
          <Input name="Email" placeholder="Email" value={email} onChange={(e) => setEmail(e.target.value)} />
          <Input name="VerificationCode" placeholder="000000" value={code} onChange={(e) => setCode(e.target.value)} />
          <Button className="w-full" type="submit" disabled={loading}>{loading ? 'Verifying...' : 'Verify'}</Button>
        </form>
      </div>
    </div>
  );
}

