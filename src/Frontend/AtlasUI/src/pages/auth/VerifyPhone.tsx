import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { motion } from "framer-motion";
import { Phone } from "lucide-react";
import AuthLayout from "@/components/auth/AuthLayout";
import { useAuth } from "@/context/AuthContext";

const VerifyPhone = () => {
  const navigate = useNavigate();
  const { user, verifyPhone } = useAuth();
  const [code, setCode] = useState(["", "", "", "", "", ""]);
  const [error, setError] = useState("");

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

  const handleVerify = () => {
    const fullCode = code.join("");
    if (fullCode.length < 6) {
      setError("Please enter the full 6-digit code");
      return;
    }
    const success = verifyPhone(fullCode);
    if (success) {
      navigate("/onboarding");
    } else {
      setError("Invalid verification code");
    }
  };

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
          Didn't receive the code?{" "}
          <button className="text-primary font-medium hover:underline">Resend via {contactMethod}</button>
        </p>
      </div>
    </AuthLayout>
  );
};

export default VerifyPhone;
