import * as React from "react";
import { useState } from "react";
import { accounts } from "@/lib/api";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

export default function ForgotPassword() {
  const [email, setEmail] = useState("");
  const [message, setMessage] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setMessage(null);
    try {
      await accounts.forgotPassword({ Email: email });
      setMessage("If an account exists, we'll send reset instructions to the provided email.");
    } catch (err: any) {
      setMessage("If an account exists, we'll send reset instructions to the provided email.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-background">
      <div className="w-full max-w-md p-8 rounded-lg shadow-lg bg-card">
        <h1 className="text-2xl font-semibold mb-4">Reset your password</h1>
        <p className="text-sm text-muted-foreground mb-6">Enter your email and we'll send reset instructions if the account exists.</p>
        {message && <div className="mb-4 text-sm text-muted-foreground">{message}</div>}
        <form onSubmit={submit} className="space-y-4">
          <Input name="Email" placeholder="Email" value={email} onChange={(e) => setEmail(e.target.value)} />
          <Button className="w-full" type="submit" disabled={loading}>{loading ? 'Sending...' : 'Send reset instructions'}</Button>
        </form>
      </div>
    </div>
  );
}

