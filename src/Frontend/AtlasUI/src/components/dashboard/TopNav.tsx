import { Search, CalendarDays, Check, Settings, Bell, Sun, Moon } from "lucide-react";
import { useTheme } from "@/context/ThemeContext";
import { motion } from "framer-motion";

const TopNav = () => {
  const { theme, toggleTheme } = useTheme();

  return (
    <motion.header
      initial={{ opacity: 0, y: -10 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.4 }}
      className="h-16 border-b border-border bg-card flex items-center justify-between px-6 shrink-0"
    >
      {/* Left - Logo */}
      <div className="flex items-center gap-3">
        <motion.div
          whileHover={{ rotate: 6, scale: 1.05 }}
          className="w-8 h-8 rounded-lg bg-primary flex items-center justify-center shadow-md shadow-primary/30"
        >
          <span className="text-primary-foreground font-semibold text-sm">A</span>
        </motion.div>
        <div>
          <h1 className="text-sm font-semibold text-foreground leading-tight">Atlas</h1>
          <p className="text-[11px] text-muted-foreground leading-tight">Team's workspace</p>
        </div>
      </div>

      {/* Center - Search */}
      <div className="hidden md:flex flex-1 max-w-md mx-8">
        <div className="relative w-full group">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground group-focus-within:text-primary transition-colors" />
          <input
            type="text"
            placeholder="Preparation of technical specifications..."
            className="w-full h-9 pl-9 pr-4 rounded-xl bg-muted/50 border border-border text-xs text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/30 focus:border-primary/50 transition-all"
          />
        </div>
      </div>

      {/* Right - Actions */}
      <div className="flex items-center gap-1">
        <button className="flex items-center gap-1.5 h-8 px-3 rounded-lg text-xs text-muted-foreground hover:bg-muted hover:text-foreground transition-colors">
          <CalendarDays className="w-3.5 h-3.5" />
          <span className="hidden sm:inline">Monthly</span>
        </button>
        <button className="w-8 h-8 rounded-lg flex items-center justify-center text-muted-foreground hover:bg-muted hover:text-foreground transition-colors">
          <Check className="w-4 h-4" />
        </button>
        <motion.button
          whileTap={{ rotate: 180 }}
          onClick={toggleTheme}
          className="w-8 h-8 rounded-lg flex items-center justify-center text-muted-foreground hover:bg-muted hover:text-primary transition-colors"
        >
          {theme === "light" ? <Moon className="w-4 h-4" /> : <Sun className="w-4 h-4" />}
        </motion.button>
        <button className="w-8 h-8 rounded-lg flex items-center justify-center text-muted-foreground hover:bg-muted hover:text-foreground transition-colors">
          <Settings className="w-4 h-4" />
        </button>
        <button className="w-8 h-8 rounded-lg flex items-center justify-center text-muted-foreground hover:bg-muted hover:text-foreground transition-colors relative">
          <Bell className="w-4 h-4" />
          <motion.span
            animate={{ scale: [1, 1.3, 1] }}
            transition={{ duration: 2, repeat: Infinity }}
            className="absolute top-1.5 right-1.5 w-2 h-2 bg-momentum-red rounded-full"
          />
        </button>
        <motion.div
          whileHover={{ scale: 1.1 }}
          className="w-8 h-8 rounded-full bg-primary/20 ml-1 flex items-center justify-center overflow-hidden cursor-pointer"
        >
          <span className="text-xs font-medium text-primary">O</span>
        </motion.div>
      </div>
    </motion.header>
  );
};

export default TopNav;
