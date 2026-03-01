import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { motion } from "framer-motion";
import { Mail, Loader2 } from "lucide-react";
import AuthLayout from "@/components/auth/AuthLayout";
import { useAuth } from "@/context/AuthContext";

const VerifyEmail = () => {
  const navigate = useNavigate();
  const { user, verifyEmail, resendEmailCode, isLoading, tempEmail } = useAuth();
  const displayEmail = user?.email || tempEmail || "your email";
  const [code, setCode] = useState(["", "", "", "", "", ""]);
  const [error, setError] = useState("");
  const [resendMsg, setResendMsg] = useState("");

  const handleChange = (index: number, value: string) => {
    if (value.length > 1) return;
    const newCode = [...code];
    newCode[index] = value;
    setCode(newCode);

    if (value && index < 5) {
      document.getElementById(`email-code-${index + 1}`)?.focus();
    }
  };

  const handleKeyDown = (index: number, e: React.KeyboardEvent) => {
    if (e.key === "Backspace" && !code[index] && index > 0) {
      document.getElementById(`email-code-${index - 1}`)?.focus();
    }
  };

  const handlePaste = (e: React.ClipboardEvent) => {
    e.preventDefault();
    const pasted = e.clipboardData.getData("text").replace(/[^0-9]/g, "").slice(0, 6);
    if (pasted.length > 0) {
      const newCode = [...code];
      for (let i = 0; i < pasted.length && i < 6; i++) {
        newCode[i] = pasted[i];
      }
      setCode(newCode);
      const focusIndex = Math.min(pasted.length, 5);
      document.getElementById(`email-code-${focusIndex}`)?.focus();
    }
  };

  const handleVerify = async () => {
    const fullCode = code.join("");
    if (fullCode.length < 6) {
      setError("Please enter the full 6-digit code");
      return;
    }
    setError("");
    const errs = await verifyEmail(fullCode);
    if (errs.length === 0) {
      if (user?.phone) {
        navigate("/verify-phone");
      } else {
        navigate("/onboarding");
      }
    } else {
      setError(errs[0]);
    }
  };

  const handleResend = async () => {
    setResendMsg("");
    setError("");
    const errs = await resendEmailCode();
    if (errs.length === 0) {
      setResendMsg("Verification code resent successfully!");
      setTimeout(() => setResendMsg(""), 3000);
    } else {
      setError(errs[0]);
    }
  };

  return (
    <AuthLayout title="Verify your email" subtitle={`We've sent a 6-digit code to ${displayEmail}`}>
      <div className="space-y-6">
        <motion.div
          initial={{ scale: 0 }}
          animate={{ scale: 1 }}
          className="w-16 h-16 rounded-2xl bg-primary/10 flex items-center justify-center mx-auto"
        >
          <Mail className="w-8 h-8 text-primary" />
        </motion.div>

        {error && (
          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="p-3 rounded-xl bg-destructive/10 border border-destructive/20 text-destructive text-sm text-center">
            {error}
          </motion.div>
        )}

        {resendMsg && (
          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="p-3 rounded-xl bg-green-500/10 border border-green-500/20 text-green-600 text-sm text-center">
            {resendMsg}
          </motion.div>
        )}

        <div className="flex justify-center gap-3" onPaste={handlePaste}>
          {code.map((digit, i) => (
            <motion.input
              key={i}
              initial={{ opacity: 0, y: 10 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: i * 0.05 }}
              id={`email-code-${i}`}
              type="text"
              inputMode="numeric"
              maxLength={1}
              value={digit}
              onChange={(e) => handleChange(i, e.target.value.replace(/[^0-9]/g, ""))}
              onKeyDown={(e) => handleKeyDown(i, e)}
              disabled={isLoading}
              className="w-12 h-14 text-center text-xl font-semibold rounded-xl bg-muted/50 border border-border text-foreground focus:outline-none focus:ring-2 focus:ring-primary/30 focus:border-primary/50 transition-all disabled:opacity-50"
            />
          ))}
        </div>

        <motion.button
          whileHover={{ scale: isLoading ? 1 : 1.01 }}
          whileTap={{ scale: isLoading ? 1 : 0.99 }}
          onClick={handleVerify}
          disabled={code.join("").length < 6 || isLoading}
          className="w-full h-11 rounded-xl bg-primary text-primary-foreground font-medium text-sm shadow-lg shadow-primary/25 transition-all disabled:opacity-50 flex items-center justify-center gap-2"
        >
          {isLoading ? (
            <>
              <Loader2 className="w-4 h-4 animate-spin" />
              Verifying...
            </>
          ) : (
            "Verify Email"
          )}
        </motion.button>

        <p className="text-center text-sm text-muted-foreground">
          Didn't receive the code?{" "}
          <button
            onClick={handleResend}
            disabled={isLoading}
            className="text-primary font-medium hover:underline disabled:opacity-50"
          >
            Resend
          </button>
        </p>
      </div>
    </AuthLayout>
  );
};

export default VerifyEmail;
