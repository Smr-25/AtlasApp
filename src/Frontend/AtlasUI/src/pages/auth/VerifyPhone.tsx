import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { motion } from "framer-motion";
import { Phone } from "lucide-react";
import AuthLayout from "@/components/auth/AuthLayout";
import { useAuth } from "@/context/AuthContext";
import { useToast } from '@/hooks/use-toast'
import { ApiError } from '@/lib/apiClient'

const VerifyPhone = () => {
  const navigate = useNavigate();
  const { user, verifyPhone, resendPhoneVerification } = useAuth();
  const { toast } = useToast();
  const [code, setCode] = useState(["", "", "", "", "", ""]);
  const [error, setError] = useState("");
  const [resendCooldown, setResendCooldown] = useState<number>(0)

  useEffect(() => {
    if (resendCooldown <= 0) return
    const id = setInterval(() => {
      setResendCooldown(s => {
        if (s <= 1) { clearInterval(id); return 0 }
        return s - 1
      })
    }, 1000)
    return () => clearInterval(id)
  }, [resendCooldown])

  const handleChange = (index: number, value: string) => {
    if (value.length > 1) return;
    const newCode = [...code];
    newCode[index] = value;
    setCode(newCode);
    if (value && index < 5) {
      document.getElementById(`phone-code-${index + 1}`)?.focus();
    }
  };

  const handleKeyDown = (index: number, e: React.KeyboardEvent) => {
    if (e.key === "Backspace" && !code[index] && index > 0) {
      document.getElementById(`phone-code-${index - 1}`)?.focus();
    }
  };

  const handleVerify = async () => {
    const fullCode = code.join("");
    if (fullCode.length < 6) {
      setError("Please enter the full 6-digit code");
      return;
    }
    try {
      const success = await verifyPhone(fullCode);
      if (success) {
        toast({ title: 'Phone verified', description: 'Verified — redirecting to onboarding...' })
        navigate("/onboarding");
      } else {
        setError("Invalid verification code");
        toast({ title: 'Verification failed', description: 'Invalid verification code', variant: 'destructive' })
      }
    } catch (e: any) {
      if (e instanceof ApiError) {
        const msg = e.errors && e.errors.length ? e.errors.join(', ') : e.message
        setError(msg)
        toast({ title: 'Verification failed', description: msg, variant: 'destructive' })
      } else {
        setError('Invalid verification code')
        toast({ title: 'Verification failed', description: 'Invalid verification code', variant: 'destructive' })
      }
    }
  };

  const handleResend = async () => {
    try {
      if (!user?.phone) return setError('No phone available')
      // validate E.164-ish format
      if (!/^\+\d{7,15}$/.test(user.phone)) return setError('Phone number must be in E.164 format')
      if (resendCooldown > 0) return
      const ok = await resendPhoneVerification()
      if (ok) {
        toast({ title: 'Verification sent', description: `A new verification code was sent via ${user.phoneContact === 'telegram' ? 'Telegram' : 'SMS'}` })
        setResendCooldown(60)
      } else {
        setError('Failed to resend code.')
        toast({ title: 'Resend failed', description: 'Failed to resend code.', variant: 'destructive' })
      }
    } catch (e) {
      if (e instanceof ApiError) {
        const msg = e.errors && e.errors.length ? e.errors.join(', ') : e.message
        setError(msg)
        toast({ title: 'Resend failed', description: msg, variant: 'destructive' })
      } else {
        setError('Failed to resend code.')
        toast({ title: 'Resend failed', description: 'Failed to resend code.', variant: 'destructive' })
      }
    }
  }

  const contactMethod = user?.phoneContact === "telegram" ? "Telegram" : "SMS";

  return (
    <AuthLayout title="Verify your phone" subtitle={`We've sent a 6-digit code via ${contactMethod} to ${user?.phone || "your phone"}`}>
      <div className="space-y-6">
        <motion.div
          initial={{ scale: 0 }}
          animate={{ scale: 1 }}
          className="w-16 h-16 rounded-2xl bg-primary/10 flex items-center justify-center mx-auto"
        >
          <Phone className="w-8 h-8 text-primary" />
        </motion.div>

        {error && (
          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="p-3 rounded-xl bg-destructive/10 border border-destructive/20 text-destructive text-sm text-center">
            {error}
          </motion.div>
        )}

        <div className="flex justify-center gap-3">
          {code.map((digit, i) => (
            <motion.input
              key={i}
              initial={{ opacity: 0, y: 10 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: i * 0.05 }}
              id={`phone-code-${i}`}
              type="text"
              inputMode="numeric"
              maxLength={1}
              value={digit}
              onChange={(e) => handleChange(i, e.target.value.replace(/[^0-9]/g, ""))}
              onKeyDown={(e) => handleKeyDown(i, e)}
              className="w-12 h-14 text-center text-xl font-semibold rounded-xl bg-muted/50 border border-border text-foreground focus:outline-none focus:ring-2 focus:ring-primary/30 focus:border-primary/50 transition-all"
            />
          ))}
        </div>

        <motion.button
          whileHover={{ scale: 1.01 }}
          whileTap={{ scale: 0.99 }}
          onClick={handleVerify}
          disabled={code.join("").length < 6}
          className="w-full h-11 rounded-xl bg-primary text-primary-foreground font-medium text-sm shadow-lg shadow-primary/25 transition-all disabled:opacity-50"
        >
          Verify Phone
        </motion.button>

        <p className="text-center text-sm text-muted-foreground">
          Didn't receive the code?{' '}
          <button onClick={handleResend} disabled={resendCooldown > 0} className={`text-primary font-medium hover:underline ${resendCooldown > 0 ? 'opacity-50 cursor-not-allowed' : ''}`}>
            {resendCooldown > 0 ? `Resend (${resendCooldown}s)` : `Resend via ${contactMethod}`}
          </button>
        </p>
      </div>
    </AuthLayout>
  );
};

export default VerifyPhone;
