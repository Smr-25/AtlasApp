import { Search, Mail, Bell, Sun, Moon } from "lucide-react";
import { useTheme } from "@/context/ThemeContext";
import { useAuth } from "@/context/AuthContext";
import { motion } from "framer-motion";

const CyberTopNav = () => {
  const { theme, toggleTheme } = useTheme();
  const { user } = useAuth();

  return (
    <motion.header
      initial={{ opacity: 0, y: -10 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.4 }}
      className="h-16 border-b border-border bg-card flex items-center justify-between px-6 shrink-0"
    >
      {/* Search */}
      <div className="hidden md:flex flex-1 max-w-md">
        <div className="relative w-full group">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground group-focus-within:text-primary transition-colors" />
          <input
            type="text"
            placeholder="Search task"
            className="w-full h-10 pl-10 pr-12 rounded-xl bg-muted/50 border border-border text-sm text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/30 focus:border-primary/50 transition-all"
          />
          <span className="absolute right-3 top-1/2 -translate-y-1/2 text-[10px] text-muted-foreground bg-background border border-border rounded px-1.5 py-0.5">⌘ F</span>
        </div>
      </div>

      {/* Right */}
      <div className="flex items-center gap-2">
        <button className="w-9 h-9 rounded-xl flex items-center justify-center text-muted-foreground hover:bg-muted hover:text-foreground transition-colors">
          <Mail className="w-[18px] h-[18px]" />
        </button>
        <motion.button
          whileTap={{ rotate: 180 }}
          onClick={toggleTheme}
          className="w-9 h-9 rounded-xl flex items-center justify-center text-muted-foreground hover:bg-muted hover:text-primary transition-colors"
        >
          {theme === "light" ? <Moon className="w-[18px] h-[18px]" /> : <Sun className="w-[18px] h-[18px]" />}
        </motion.button>
        <button className="w-9 h-9 rounded-xl flex items-center justify-center text-muted-foreground hover:bg-muted hover:text-foreground transition-colors relative">
          <Bell className="w-[18px] h-[18px]" />
          <motion.span
            animate={{ scale: [1, 1.3, 1] }}
            transition={{ duration: 2, repeat: Infinity }}
            className="absolute top-2 right-2 w-2 h-2 bg-primary rounded-full"
          />
        </button>
        <div className="flex items-center gap-2.5 ml-2">
          <motion.div
            whileHover={{ scale: 1.05 }}
            className="w-9 h-9 rounded-full bg-primary/20 flex items-center justify-center cursor-pointer"
          >
            <span className="text-xs font-semibold text-primary">
              {user?.fullName?.charAt(0) || "T"}
            </span>
          </motion.div>
          <div className="hidden sm:block">
            <p className="text-sm font-semibold text-foreground leading-tight">{user?.fullName || "Totok Michael"}</p>
            <p className="text-[11px] text-muted-foreground leading-tight">{user?.email || "tmichael20@mail.com"}</p>
          </div>
        </div>
      </div>
    </motion.header>
  );
};

export default CyberTopNav;
