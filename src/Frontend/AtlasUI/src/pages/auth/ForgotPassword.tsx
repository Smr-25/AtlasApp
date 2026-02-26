import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { motion } from "framer-motion";
import { ArrowLeft, Mail } from "lucide-react";
import AuthLayout from "@/components/auth/AuthLayout";
import { useToast } from '@/hooks/use-toast'
import api, { ApiError } from '@/lib/apiClient'

const ForgotPassword = () => {
  const navigate = useNavigate();
  const { toast } = useToast();
  const [email, setEmail] = useState("");
  const [sent, setSent] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    // client-side validation
    if (!email.trim()) {
      toast({ title: 'Email required', description: 'Please enter your email address', variant: 'destructive' })
      return
    }
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      toast({ title: 'Invalid email', description: 'Please enter a valid email address', variant: 'destructive' })
      return
    }

    try {
      await api.accounts.forgotPassword({ Email: email });
      setSent(true);
      toast({ title: 'Reset email sent', description: `If an account exists for ${email} we'll send instructions.` })
      // navigate to reset password flow with email prefilled
      navigate(`/reset-password?email=${encodeURIComponent(email)}`)
    } catch (err: any) {
      if (err instanceof ApiError) {
        const msg = err.errors && err.errors.length ? err.errors.join(', ') : err.message
        toast({ title: 'Failed to send reset email', description: msg, variant: 'destructive' })
      } else {
        toast({ title: 'Failed to send reset email', description: err?.message || 'Failed to send reset email.', variant: 'destructive' })
      }
    }
  };

  const inputClass =
    "w-full h-11 px-4 rounded-xl bg-muted/50 border border-border text-sm text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/30 focus:border-primary/50 transition-all";

  return (
    <AuthLayout title={sent ? "Check your email" : "Forgot password?"} subtitle={sent ? `We've sent a reset code to ${email}` : "No worries, we'll send you reset instructions"}>
      {!sent ? (
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="text-sm font-medium text-foreground mb-1.5 block">Email</label>
            <input
              type="email"
              placeholder="oliver@momentum.io"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className={inputClass}
            />
          </div>

          <motion.button
            whileHover={{ scale: 1.01 }}
            whileTap={{ scale: 0.99 }}
            type="submit"
            className="w-full h-11 rounded-xl bg-primary text-primary-foreground font-medium text-sm shadow-lg shadow-primary/25 transition-all"
          >
            Send Reset Code
          </motion.button>

          <Link to="/login" className="flex items-center justify-center gap-2 text-sm text-muted-foreground hover:text-foreground transition-colors mt-4">
            <ArrowLeft className="w-4 h-4" />
            Back to sign in
          </Link>
        </form>
      ) : (
        <div className="space-y-4">
          <motion.div
            initial={{ scale: 0 }}
            animate={{ scale: 1 }}
            className="w-16 h-16 rounded-2xl bg-primary/10 flex items-center justify-center mx-auto mb-4"
          >
            <Mail className="w-8 h-8 text-primary" />
          </motion.div>

          <motion.button
            whileHover={{ scale: 1.01 }}
            whileTap={{ scale: 0.99 }}
            onClick={() => navigate("/reset-password")}
            className="w-full h-11 rounded-xl bg-primary text-primary-foreground font-medium text-sm shadow-lg shadow-primary/25 transition-all"
          >
            Enter Reset Code
          </motion.button>

          <button
            onClick={() => setSent(false)}
            className="w-full text-center text-sm text-muted-foreground hover:text-foreground transition-colors"
          >
            Didn't receive the email? <span className="text-primary">Click to resend</span>
          </button>

          <Link to="/login" className="flex items-center justify-center gap-2 text-sm text-muted-foreground hover:text-foreground transition-colors">
            <ArrowLeft className="w-4 h-4" />
            Back to sign in
          </Link>
        </div>
      )}
    </AuthLayout>
  );
};

export default ForgotPassword;
