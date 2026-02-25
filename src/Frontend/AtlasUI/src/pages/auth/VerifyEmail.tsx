import * as React from "react";
import { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { accounts } from "@/lib/api";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

export default function VerifyEmail() {
  const location = useLocation();
  const navigate = useNavigate();
  const prefilledEmail = (location.state as any)?.email || '';
  const [email] = useState(prefilledEmail);
  const [code, setCode] = useState("");
  const [message, setMessage] = useState<string | null>(null);
  const [resendLoading, setResendLoading] = useState(false);

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await accounts.verifyEmail({ Email: email, VerificationCode: code });
      setMessage('Email verified successfully. Redirecting...');
      setTimeout(() => navigate('/'), 800);
    } catch (err: any) {
      setMessage(err?.data?.errors?.map((x: any) => x.message).join('\n') || 'Verification failed');
    }
  };

  const resend = async () => {
    if (!email) {
      setMessage('Email not available. Please go back to register and provide your email.');
      return;
    }
    setResendLoading(true);
    try {
      await accounts.resendEmailVerificationCode({ Email: email });
      setMessage('Verification code resent to your email.');
    } catch (err: any) {
      if (err?.status === 429) setMessage('Too many requests. Please wait before trying again.');
      else setMessage(err?.data?.message || 'Could not resend verification code');
    } finally {
      setResendLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-background">
      <div className="w-full max-w-md p-8 rounded-lg shadow-lg bg-card">
        <h1 className="text-2xl font-semibold mb-4">Verify your email</h1>
        <p className="text-sm text-muted-foreground mb-6">Enter the verification code we sent to your email.</p>
        {message && <div className="mb-4 text-sm text-muted-foreground">{message}</div>}
        <form onSubmit={submit} className="space-y-4">
          <div>
            <div className="text-sm text-muted-foreground mb-2">Email</div>
            <div className="p-3 bg-muted rounded-md">{email || '(not available)'}</div>
          </div>
          <Input name="VerificationCode" placeholder="000000" value={code} onChange={(e) => setCode(e.target.value)} />
          <Button className="w-full" type="submit">Verify</Button>
        </form>
        <div className="mt-4 flex gap-2">
          <Button onClick={resend} disabled={resendLoading}>{resendLoading ? 'Resending...' : 'Resend code'}</Button>
          <Button variant="ghost" onClick={() => navigate('/register')}>Back</Button>
        </div>
      </div>
    </div>
  );
}
