import { RefreshCw } from "lucide-react";
import { motion } from "framer-motion";

const UpdatesCard = () => {
  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5, ease: "easeOut" }}
      whileHover={{ y: -2, boxShadow: "0 8px 30px -12px hsl(var(--primary) / 0.15)" }}
      className="bg-card rounded-2xl border border-border p-5 flex flex-col transition-colors"
    >
      {/* Header */}
      <div className="flex items-center justify-between mb-4">
        <div className="flex items-center gap-2">
          <div className="w-8 h-8 rounded-lg bg-primary/10 flex items-center justify-center">
            <svg width="16" height="16" viewBox="0 0 16 16" fill="none">
              <path d="M2 4h12M2 8h8M2 12h10" stroke="hsl(var(--primary))" strokeWidth="1.5" strokeLinecap="round" />
            </svg>
          </div>
          <span className="text-sm font-medium text-foreground">Updates</span>
        </div>
        <div className="flex items-center gap-2">
          <button className="text-xs text-primary hover:underline transition-all">View all</button>
          <motion.button
            whileHover={{ rotate: 180 }}
            transition={{ duration: 0.4 }}
            className="w-7 h-7 rounded-lg flex items-center justify-center text-muted-foreground hover:bg-muted hover:text-primary transition-colors"
          >
            <RefreshCw className="w-3.5 h-3.5" />
          </motion.button>
        </div>
      </div>

      {/* Stats */}
      <div className="flex gap-6 flex-1">
        {/* Left - Total */}
        <div className="flex-1">
          <motion.p
            initial={{ opacity: 0, scale: 0.5 }}
            animate={{ opacity: 1, scale: 1 }}
            transition={{ delay: 0.3, type: "spring", stiffness: 100 }}
            className="text-3xl font-semibold text-foreground"
          >
            1,892
          </motion.p>
          <p className="text-xs text-muted-foreground mt-1">Total updates for the project</p>
        </div>

        {/* Right - Chart */}
        <div className="flex-1">
          <div className="flex items-center gap-2 mb-2">
            <span className="text-lg font-semibold text-foreground">1,302</span>
            <span className="text-[10px] text-muted-foreground">Development</span>
          </div>
          {/* Animated bar visualization */}
          <div className="space-y-1.5">
            {[
              { width: "90%", opacity: "", delay: 0.4 },
              { width: "6%", opacity: "/60", delay: 0.55 },
              { width: "4%", opacity: "/30", delay: 0.7 },
            ].map((bar, i) => (
              <div key={i} className="flex items-center gap-2">
                <div className="h-2 flex-1 rounded-full bg-muted overflow-hidden">
                  <motion.div
                    initial={{ width: 0 }}
                    animate={{ width: bar.width }}
                    transition={{ duration: 0.8, delay: bar.delay, ease: "easeOut" }}
                    className={`h-full rounded-full bg-primary${bar.opacity}`}
                  />
                </div>
                <span className="text-[10px] text-muted-foreground w-8">{bar.width.replace('%', '')}%</span>
              </div>
            ))}
          </div>
        </div>
      </div>
    </motion.div>
  );
};

export default UpdatesCard;
