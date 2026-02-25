import { motion } from "framer-motion";
import { ReactNode } from "react";

interface AuthLayoutProps {
  children: ReactNode;
  title: string;
  subtitle: string;
}

const AuthLayout = ({ children, title, subtitle }: AuthLayoutProps) => {
  return (
    <div className="min-h-screen flex bg-background">
      {/* Left Branding Panel */}
      <div className="hidden lg:flex lg:w-[45%] relative overflow-hidden bg-foreground">
        {/* Animated gradient orbs */}
        <motion.div
          animate={{ scale: [1, 1.2, 1], rotate: [0, 45, 0] }}
          transition={{ duration: 20, repeat: Infinity, ease: "easeInOut" }}
          className="absolute top-1/4 left-1/4 w-96 h-96 rounded-full bg-primary/30 blur-[120px]"
        />
        <motion.div
          animate={{ scale: [1.2, 1, 1.2], rotate: [45, 0, 45] }}
          transition={{ duration: 15, repeat: Infinity, ease: "easeInOut" }}
          className="absolute bottom-1/4 right-1/4 w-80 h-80 rounded-full bg-primary/20 blur-[100px]"
        />
        <motion.div
          animate={{ y: [0, -20, 0] }}
          transition={{ duration: 8, repeat: Infinity, ease: "easeInOut" }}
          className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-64 h-64 rounded-full bg-primary/40 blur-[80px]"
        />

        {/* Content */}
        <div className="relative z-10 flex flex-col justify-between p-12 w-full">
          {/* Logo */}
          <motion.div
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ duration: 0.6 }}
            className="flex items-center gap-3"
          >
            <div className="w-10 h-10 rounded-xl bg-primary flex items-center justify-center shadow-lg shadow-primary/40">
              <span className="text-primary-foreground font-bold text-lg">M</span>
            </div>
            <span className="text-white font-semibold text-xl tracking-tight">Momentum</span>
          </motion.div>

          {/* Center text */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.8, delay: 0.3 }}
            className="space-y-4"
          >
            <h2 className="text-4xl font-bold text-white leading-tight">
              Maximize your<br />
              <span className="text-primary">team's potential</span>
            </h2>
            <p className="text-white/60 text-lg max-w-sm">
              Replace all your software. Every app, AI agent, and human in one place.
            </p>
          </motion.div>

          {/* Bottom testimonial */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.8, delay: 0.5 }}
            className="bg-white/5 backdrop-blur-xl border border-white/10 rounded-2xl p-6"
          >
            <p className="text-white/80 text-sm italic mb-4">
              "Momentum transformed how our team collaborates. We've seen a 40% increase in productivity since switching."
            </p>
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 rounded-full bg-primary/30 flex items-center justify-center">
                <span className="text-primary text-sm font-semibold">AK</span>
              </div>
              <div>
                <p className="text-white text-sm font-medium">Alex Karev</p>
                <p className="text-white/50 text-xs">CTO at TechFlow</p>
              </div>
            </div>
          </motion.div>
        </div>
      </div>

      {/* Right Form Panel */}
      <div className="flex-1 flex flex-col justify-center items-center px-6 py-12 lg:px-16">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5 }}
          className="w-full max-w-md"
        >
          {/* Mobile logo */}
          <div className="lg:hidden flex items-center gap-3 mb-10">
            <div className="w-9 h-9 rounded-lg bg-primary flex items-center justify-center shadow-md shadow-primary/30">
              <span className="text-primary-foreground font-semibold text-sm">M</span>
            </div>
            <span className="text-foreground font-semibold text-lg">Momentum</span>
          </div>

          <div className="mb-8">
            <h1 className="text-2xl font-bold text-foreground mb-2">{title}</h1>
            <p className="text-muted-foreground text-sm">{subtitle}</p>
          </div>

          {children}
        </motion.div>
      </div>
    </div>
  );
};

export default AuthLayout;
