import * as React from "react";
import { useState, useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { accounts } from "@/lib/api";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Phone } from 'lucide-react';

export default function VerifyPhone() {
  const location = useLocation();
  const navigate = useNavigate();
  const prefilledPhone = (location.state as any)?.phone || '';
  const telegramBotLink = (location.state as any)?.telegramBotLink || null;
  const [phone] = useState(prefilledPhone);
  const [code, setCode] = useState("");
  const [message, setMessage] = useState<string | null>(null);
  const [resendLoading, setResendLoading] = useState(false);
  const [cooldown, setCooldown] = useState<number>(0);

  useEffect(() => {
    let t: any;
    if (cooldown > 0) {
      t = setTimeout(() => setCooldown(cooldown - 1), 1000);
    }
    return () => clearTimeout(t);
  }, [cooldown]);

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await accounts.verifyPhone({ PhoneNumber: phone, VerificationCode: code });
      setMessage('Phone verified successfully. Redirecting...');
      setTimeout(() => navigate('/'), 800);
    } catch (err: any) {
      setMessage(err?.data?.errors?.map((x: any) => x.message).join('\n') || 'Verification failed');
    }
  };

  const resendVia = async (selected: 'Sms' | 'Telegram') => {
    if (!phone) {
      setMessage('Phone not available. Please go back to register and provide your phone.');
      return;
    }
    setResendLoading(true);
    try {
      await accounts.resendPhoneVerificationCode({ PhoneNumber: phone, Channel: selected });
      // If backend returns Retry-After header, backend wrapper would need to forward it. We'll start a default cooldown (60s) otherwise.
      setMessage(`Verification code resent via ${selected}.`);
      setCooldown(60);
    } catch (err: any) {
      if (err?.status === 429) {
        const retryAfter = err?.data?.retryAfter || 60;
        setMessage('Too many requests. Please wait before trying again.');
        setCooldown(Number(retryAfter) || 60);
      } else setMessage(err?.data?.message || 'Could not resend verification code');
    } finally {
      setResendLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-background">
      <div className="w-full max-w-md p-8 rounded-lg shadow-lg bg-card">
        <h1 className="text-2xl font-semibold mb-4">Verify your phone</h1>
        <p className="text-sm text-muted-foreground mb-6">Enter the verification code sent to your phone.</p>
        {message && <div role="status" aria-live="polite" className="mb-4 text-sm text-muted-foreground">{message}</div>}
        <form onSubmit={submit} className="space-y-4">
          <div>
            <div className="text-sm text-muted-foreground mb-2">Phone</div>
            <div className="p-3 bg-muted rounded-md">{phone || '(not available)'}</div>
          </div>
          {telegramBotLink && <div className="text-sm text-muted-foreground">Telegram bot: <a href={telegramBotLink} target="_blank" rel="noreferrer" className="text-primary">Open</a></div>}
          <Input name="VerificationCode" placeholder="000000" value={code} onChange={(e) => setCode(e.target.value)} />
          <Button className="w-full" type="submit">Verify</Button>
        </form>

        <div className="mt-4">
          {phone ? (
            <div className="flex gap-3">
              <button
                type="button"
                onClick={() => resendVia('Sms')}
                disabled={resendLoading || cooldown > 0}
                className={`flex-1 flex items-center justify-center gap-3 px-4 py-3 rounded-md border ${resendLoading || cooldown > 0 ? 'opacity-60 pointer-events-none' : ''}`}
              >
                <Phone className="w-5 h-5" />
                <span>Send via SMS</span>
              </button>

              <button
                type="button"
                onClick={() => resendVia('Telegram')}
                disabled={resendLoading || cooldown > 0}
                className={`flex-1 flex items-center justify-center gap-3 px-4 py-3 rounded-md border ${resendLoading || cooldown > 0 ? 'opacity-60 pointer-events-none' : ''}`}
              >
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 240 240" className="w-5 h-5"><circle cx="120" cy="120" r="120" fill="#37AEE2" /><path d="M84 124l65-27c2-1 4 0 3 2l-18 69c-1 4-5 5-9 3l-23-16-11 11c-1 1-3 1-4 0l-1-26-26-6c-4-1-4-5 2-7l99-39c3-1 6 1 5 4l-37 119c-1 3-5 4-8 3l-69-31c-3-1-5-3-4-6l23-93c1-3 4-4 6-3z" fill="#fff"/></svg>
                <span>Send via Telegram</span>
              </button>
            </div>
          ) : (
            <div className="flex items-center gap-2">
              <Button variant="ghost" onClick={() => navigate('/register')}>Back</Button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
