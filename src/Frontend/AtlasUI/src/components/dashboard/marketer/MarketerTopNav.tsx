import { Settings, Bell, Sun, Moon } from "lucide-react";
import { useTheme } from "@/context/ThemeContext";
import { useAuth } from "@/context/AuthContext";
import { motion } from "framer-motion";

const navItems = ["Dashboard", "People", "Hiring", "Devices", "Apps", "Salary", "Calendar", "Reviews"];

const MarketerTopNav = () => {
  const { theme, toggleTheme } = useTheme();
  const { user } = useAuth();

  return (
    <motion.header
      initial={{ opacity: 0, y: -10 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.4 }}
      className="h-16 border-b border-border bg-card flex items-center justify-between px-6 shrink-0"
    >
      {/* Logo */}
      <div className="flex items-center gap-2.5">
        <motion.div
          whileHover={{ rotate: 6, scale: 1.05 }}
          className="px-3 py-1.5 rounded-full border border-border"
        >
          <span className="text-sm font-semibold text-foreground">Atlas</span>
        </motion.div>
      </div>

      {/* Center Nav */}
      <nav className="hidden lg:flex items-center bg-muted/50 rounded-full p-1">
        {navItems.map((item, i) => (
          <button
            key={item}
            className={`px-4 py-2 rounded-full text-sm transition-colors ${
              i === 0
                ? "bg-foreground text-background font-medium"
                : "text-muted-foreground hover:text-foreground"
            }`}
          >
            {item}
          </button>
        ))}
      </nav>

      {/* Right */}
      <div className="flex items-center gap-2">
        <button className="flex items-center gap-1.5 px-3 py-2 rounded-full border border-border text-sm text-muted-foreground hover:bg-muted transition-colors">
          <Settings className="w-4 h-4" />
          <span className="hidden sm:inline">Setting</span>
        </button>
        <motion.button
          whileTap={{ rotate: 180 }}
          onClick={toggleTheme}
          className="w-9 h-9 rounded-full flex items-center justify-center text-muted-foreground hover:bg-muted hover:text-primary transition-colors"
        >
          {theme === "light" ? <Moon className="w-4 h-4" /> : <Sun className="w-4 h-4" />}
        </motion.button>
        <button className="w-9 h-9 rounded-full flex items-center justify-center text-muted-foreground hover:bg-muted transition-colors relative">
          <Bell className="w-4 h-4" />
        </button>
        <motion.div
          whileHover={{ scale: 1.05 }}
          className="w-9 h-9 rounded-full bg-primary/20 flex items-center justify-center cursor-pointer ml-1"
        >
          <span className="text-xs font-semibold text-primary">
            {user?.fullName?.charAt(0) || "N"}
          </span>
        </motion.div>
      </div>
    </motion.header>
  );
};

export default MarketerTopNav;
