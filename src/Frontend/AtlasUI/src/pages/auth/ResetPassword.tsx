import { useState } from "react";
import { useNavigate, Link, useLocation } from "react-router-dom";
import { motion } from "framer-motion";
import { ArrowLeft, Eye, EyeOff, CheckCircle } from "lucide-react";
import AuthLayout from "@/components/auth/AuthLayout";
import { useToast } from '@/hooks/use-toast'
import api, { ApiError } from '@/lib/apiClient'

const ResetPassword = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const params = new URLSearchParams(location.search);
  const prefillEmail = params.get('email') || '';

  const { toast } = useToast();

  const [step, setStep] = useState<"code" | "reset" | "success">(prefillEmail ? 'code' : 'code');
  const [code, setCode] = useState(["", "", "", "", "", ""]);
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);
  const [error, setError] = useState("");
  const [email, setEmail] = useState(prefillEmail);
  const [resetToken, setResetToken] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleCodeChange = (index: number, value: string) => {
    if (value.length > 1) return;
    const newCode = [...code];
    newCode[index] = value;
    setCode(newCode);

    if (value && index < 5) {
      const next = document.getElementById(`code-${index + 1}`);
      next?.focus();
    }
  };

  const handleCodeKeyDown = (index: number, e: React.KeyboardEvent) => {
    if (e.key === "Backspace" && !code[index] && index > 0) {
      const prev = document.getElementById(`code-${index - 1}`);
      prev?.focus();
    }
  };

  const verifyCode = async () => {
    setError('')
    if (email.trim() === "") {
      toast({ title: 'Email required', description: 'Please enter your email', variant: 'destructive' })
      return;
    }
    if (code.join("").length === 6) {
      try {
        // debug request
        try { console.debug('verifyResetCode: calling with', { Email: email, VerificationCode: code.join('') }) } catch {}
        const res = await api.accounts.verifyResetCode({ Email: email, VerificationCode: code.join("") });
        // unwrap common envelope shapes up to a few levels
        let raw: any = res as any
        for (let i = 0; i < 3; i++) {
          if (raw && raw.data && typeof raw.data === 'object') raw = raw.data
          else break
        }
        // verify response shape for ResetToken
        const token = raw?.ResetToken ?? raw?.resetToken ?? raw?.reset_token
        if (!token) {
          console.error('verifyResetCode: unexpected response (no ResetToken found)', res)
          setError('Unexpected server response. Please try again later.')
          toast({ title: 'Invalid code', description: 'Unexpected server response', variant: 'destructive' })
          return
        }
        console.debug('verifyResetCode: received token length', token?.length)
        setResetToken(token)
        setStep("reset");
        toast({ title: 'Code verified', description: 'You may now set a new password.' })
      } catch (e: any) {
        try { console.error('verifyResetCode error', e) } catch {}
        if (e instanceof ApiError) {
          const msg = e.errors && e.errors.length ? e.errors.join(', ') : e.message
          setError(msg)
          toast({ title: 'Invalid code', description: msg, variant: 'destructive' })
        } else {
          setError(e?.message || 'Invalid or expired code')
          toast({ title: 'Invalid code', description: e?.message || 'Invalid or expired code', variant: 'destructive' })
        }
      }
    }
  };

  const handleReset = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('')
    if (password.length < 8) {
      setError("Password must be at least 8 characters");
      toast({ title: 'Weak password', description: 'Password must be at least 8 characters', variant: 'destructive' })
      return;
    }
    // enforce complexity: uppercase, lowercase, digit, special
    if (!/[A-Z]/.test(password) || !/[a-z]/.test(password) || !/\d/.test(password) || !/[^A-Za-z0-9]/.test(password)) {
      setError('Password must contain uppercase, lowercase, digit and special character');
      toast({ title: 'Weak password', description: 'Password must contain uppercase, lowercase, digit and special character', variant: 'destructive' })
      return;
    }
    if (password !== confirmPassword) {
      setError("Passwords don't match");
      toast({ title: 'Mismatch', description: "Passwords don't match", variant: 'destructive' })
      return;
    }
    if (!resetToken) {
      setError('Missing reset token. Please verify your code first.')
      toast({ title: 'Missing token', description: 'Please verify your reset code before submitting a new password.', variant: 'destructive' })
      return
    }
    try {
      // log payload for debugging (do not include tokens in production logs)
      try { console.debug('ResetPassword: calling resetPassword with', { Email: email, /* ResetToken hidden */ NewPassword: '***' }) } catch {}
      setIsSubmitting(true)
      await api.accounts.resetPassword({ Email: email, ResetToken: resetToken, NewPassword: password, ConfirmPassword: confirmPassword });
      setStep('success');
      toast({ title: 'Password reset', description: 'Your password has been reset. You can now sign in.' })
    } catch (e: any) {
      try { console.error('resetPassword error', e) } catch {}
      if (e instanceof ApiError) {
        const msg = e.errors && e.errors.length ? e.errors.join(', ') : e.message
        setError(msg)
        toast({ title: 'Reset failed', description: msg, variant: 'destructive' })
      } else {
        setError(e?.message || 'Failed to reset password')
        toast({ title: 'Reset failed', description: e?.message || 'Failed to reset password', variant: 'destructive' })
      }
    } finally {
      setIsSubmitting(false)
    }
  };

  const inputClass =
    "w-full h-11 px-4 rounded-xl bg-muted/50 border border-border text-sm text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/30 focus:border-primary/50 transition-all";

  if (step === "success") {
    return (
      <AuthLayout title="Password reset!" subtitle="Your password has been successfully reset">
        <motion.div initial={{ scale: 0 }} animate={{ scale: 1 }} className="flex justify-center mb-6">
          <div className="w-20 h-20 rounded-full bg-green-500/10 flex items-center justify-center">
            <CheckCircle className="w-10 h-10 text-green-500" />
          </div>
        </motion.div>
        <motion.button
          whileHover={{ scale: 1.01 }}
          whileTap={{ scale: 0.99 }}
          onClick={() => navigate("/login")}
          className="w-full h-11 rounded-xl bg-primary text-primary-foreground font-medium text-sm shadow-lg shadow-primary/25 transition-all"
        >
          Continue to Sign In
        </motion.button>
      </AuthLayout>
    );
  }

  return (
    <AuthLayout
      title={step === "code" ? "Enter reset code" : "Set new password"}
      subtitle={step === "code" ? "Enter the 6-digit code sent to your email" : "Your new password must be different from previous passwords"}
    >
      {step === "code" ? (
        <div className="space-y-6">
          {/* If email was prefilled via query param, keep it hidden (don't show email on UI) */}
          {!prefillEmail && (
            <div>
              <label className="text-sm font-medium text-foreground mb-1.5 block">Email</label>
              <input
                type="email"
                placeholder="Enter your email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className={inputClass}
              />
            </div>
          )}

          <div className="flex justify-center gap-3">
            {code.map((digit, i) => (
              <input
                key={i}
                id={`code-${i}`}
                type="text"
                inputMode="numeric"
                maxLength={1}
                value={digit}
                onChange={(e) => handleCodeChange(i, e.target.value.replace(/[^0-9]/g, ""))}
                onKeyDown={(e) => handleCodeKeyDown(i, e)}
                className="w-12 h-14 text-center text-xl font-semibold rounded-xl bg-muted/50 border border-border text-foreground focus:outline-none focus:ring-2 focus:ring-primary/30 focus:border-primary/50 transition-all"
              />
            ))}
          </div>

          {error && (
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="p-3 rounded-xl bg-destructive/10 border border-destructive/20 text-destructive text-sm">
              {error}
            </motion.div>
          )}

          <motion.button
            whileHover={{ scale: 1.01 }}
            whileTap={{ scale: 0.99 }}
            onClick={verifyCode}
            disabled={code.join("").length < 6}
            className="w-full h-11 rounded-xl bg-primary text-primary-foreground font-medium text-sm shadow-lg shadow-primary/25 transition-all disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Verify Code
          </motion.button>

          <Link to="/login" className="flex items-center justify-center gap-2 text-sm text-muted-foreground hover:text-foreground transition-colors">
            <ArrowLeft className="w-4 h-4" />
            Back to sign in
          </Link>
        </div>
      ) : (
        <form onSubmit={handleReset} className="space-y-4">
          {/* Debug indicator: don't reveal token contents, only show presence/length */}
          {resetToken ? (
            <div className="text-xs text-muted-foreground">Reset token received (length: {resetToken.length})</div>
          ) : (
            <div className="text-xs text-destructive">No reset token present — verify code first</div>
          )}

          {/* Debug helper: allow pasting token manually for testing if verify step didn't return it */}
          {!resetToken && (
            <div className="space-y-2">
              <label className="text-xs font-medium text-muted-foreground block">(Debug) Paste reset token</label>
              <div className="flex gap-2">
                <input id="manual-reset-token" placeholder="paste reset token here" className="flex-1 h-10 px-3 rounded-xl bg-muted/50 border border-border text-sm" />
                <button type="button" onClick={() => {
                  const el = document.getElementById('manual-reset-token') as HTMLInputElement | null
                  if (!el) return
                  const v = el.value.trim()
                  if (!v) return
                  setResetToken(v)
                  toast({ title: 'Token set', description: 'Manual reset token applied for testing.' })
                }} className="h-10 px-3 rounded-xl bg-primary text-primary-foreground text-sm">Use</button>
              </div>
            </div>
          )}

          {error && (
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="p-3 rounded-xl bg-destructive/10 border border-destructive/20 text-destructive text-sm">
              {error}
            </motion.div>
          )}

          <div>
            <label className="text-sm font-medium text-foreground mb-1.5 block">New Password</label>
            <div className="relative">
              <input type={showPassword ? "text" : "password"} placeholder="Min. 8 characters" value={password} onChange={(e) => setPassword(e.target.value)} className={inputClass} />
              <button type="button" onClick={() => setShowPassword(!showPassword)} className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground">
                {showPassword ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
              </button>
            </div>
          </div>

          <div>
            <label className="text-sm font-medium text-foreground mb-1.5 block">Confirm New Password</label>
            <div className="relative">
              <input type={showConfirm ? "text" : "password"} placeholder="Confirm password" value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)} className={inputClass} />
              <button type="button" onClick={() => setShowConfirm(!showConfirm)} className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground">
                {showConfirm ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
              </button>
            </div>
          </div>

          <motion.button whileHover={{ scale: 1.01 }} whileTap={{ scale: 0.99 }} type="submit" disabled={isSubmitting} className={`w-full h-11 rounded-xl bg-primary text-primary-foreground font-medium text-sm shadow-lg shadow-primary/25 transition-all ${isSubmitting ? 'opacity-50 cursor-not-allowed' : ''}`}>
            {isSubmitting ? 'Resetting…' : 'Reset Password'}
          </motion.button>
        </form>
      )}
    </AuthLayout>
  );
};

export default ResetPassword;
