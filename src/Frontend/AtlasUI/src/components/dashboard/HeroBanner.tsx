import { motion } from "framer-motion";

const HeroBanner = () => {
  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5, delay: 0.1, ease: "easeOut" }}
      whileHover={{ y: -2, boxShadow: "0 12px 40px -12px hsl(var(--momentum-orange) / 0.3)" }}
      className="relative rounded-2xl overflow-hidden bg-momentum-black min-h-[180px] flex flex-col justify-end p-6 cursor-pointer group"
    >
      {/* Animated decorative gradient swirl */}
      <div className="absolute inset-0">
        <motion.div
          animate={{
            scale: [1, 1.15, 1],
            opacity: [0.2, 0.3, 0.2],
          }}
          transition={{ duration: 6, repeat: Infinity, ease: "easeInOut" }}
          className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-64 h-64 rounded-full bg-primary/20 blur-[80px]"
        />
        <motion.div
          animate={{
            x: [0, 20, 0],
            y: [0, -15, 0],
            opacity: [0.1, 0.2, 0.1],
          }}
          transition={{ duration: 8, repeat: Infinity, ease: "easeInOut" }}
          className="absolute top-1/3 right-1/4 w-32 h-32 rounded-full bg-primary/10 blur-[60px]"
        />
        <div
          className="absolute inset-0"
          style={{
            background:
              "radial-gradient(ellipse at 60% 50%, hsla(19,100%,50%,0.15) 0%, transparent 60%), linear-gradient(180deg, transparent 40%, hsla(18,100%,4%,0.9) 100%)",
          }}
        />
      </div>

      {/* Content */}
      <div className="relative z-10">
        <motion.h2
          initial={{ opacity: 0, x: -20 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ delay: 0.4, duration: 0.5 }}
          className="text-lg font-semibold text-white mb-1 group-hover:text-primary transition-colors duration-300"
        >
          Maximize human productivity
        </motion.h2>
        <motion.p
          initial={{ opacity: 0, x: -20 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ delay: 0.5, duration: 0.5 }}
          className="text-xs text-white/60 max-w-xs"
        >
          Replace all your software. Every app, AI agent, and human in one place.
        </motion.p>
      </div>
    </motion.div>
  );
};

export default HeroBanner;
