import React, { useState, useCallback } from "react";
import { motion } from "framer-motion";
import {
  LayoutDashboard,
  Copy,
  Package,
  FileText,
  MessageSquare,
  ListTodo,
  Sparkles,
  ChevronDown,
  Crown,
} from "lucide-react";

const menuGeneral = [
  { icon: LayoutDashboard, label: "Dashboard", active: true },
  { icon: Copy, label: "Templates" },
  { icon: Package, label: "Products", badge: "13" },
  { icon: FileText, label: "Docs", badge: "56" },
  { icon: MessageSquare, label: "Messages", badge: "4" },
];

const menuMore = [
  { icon: ListTodo, label: "To do lists" },
  { icon: Sparkles, label: "AI Assistants", highlight: true },
];

const interactions = [
  { name: "Dann Petty", initials: "DP", color: "bg-blue-500" },
  { name: "Flux Academy", initials: "FA", color: "bg-purple-500" },
  { name: "Michelle Choi", initials: "MC", color: "bg-pink-500" },
];

const Sidebar = () => {
  const [showMore, setShowMore] = useState(false);

  const toggleShowMore = useCallback(() => setShowMore((s) => !s), []);

  return (
    <aside className="w-56 shrink-0 border-r border-border bg-card flex flex-col h-full overflow-y-auto">
      <div className="p-4 flex-1">
        {/* GENERAL */}
        <p className="text-[10px] font-semibold text-muted-foreground tracking-widest uppercase mb-2">General</p>
        <nav className="space-y-0.5 mb-6">
          {menuGeneral.map((item, index) => (
            <motion.button
              key={item.label}
              initial={{ opacity: 0, x: -20 }}
              animate={{ opacity: 1, x: 0 }}
              transition={{ delay: index * 0.05 }}
              whileHover={{ x: 3 }}
              className={`w-full flex items-center gap-2.5 px-3 py-2 rounded-lg text-sm transition-colors ${
                item.active
                  ? "bg-primary/10 text-primary font-medium"
                  : "text-muted-foreground hover:bg-muted hover:text-foreground"
              }`}
            >
              <item.icon className="w-4 h-4" />
              <span className="flex-1 text-left">{item.label}</span>
              {item.badge && (
                <span className="text-[10px] bg-muted text-muted-foreground px-1.5 py-0.5 rounded-md font-medium">
                  {item.badge}
                </span>
              )}
            </motion.button>
          ))}
        </nav>

        {/* MORE */}
        <p className="text-[10px] font-semibold text-muted-foreground tracking-widest uppercase mb-2">More</p>
        <nav className="space-y-0.5 mb-6">
          {menuMore.map((item) => (
            <motion.button
              key={item.label}
              whileHover={{ x: 3 }}
              className={`w-full flex items-center gap-2.5 px-3 py-2 rounded-lg text-sm transition-colors ${
                item.highlight
                  ? "text-primary hover:bg-primary/10"
                  : "text-muted-foreground hover:bg-muted hover:text-foreground"
              }`}
            >
              <item.icon className="w-4 h-4" />
              <span className="flex-1 text-left">{item.label}</span>
            </motion.button>
          ))}
        </nav>

        {/* INTERACTIONS */}
        <p className="text-[10px] font-semibold text-muted-foreground tracking-widest uppercase mb-2">Interactions</p>
        <div className="space-y-1 mb-3">
          {interactions.map((user) => (
            <motion.button
              key={user.name}
              whileHover={{ x: 3 }}
              className="w-full flex items-center gap-2.5 px-3 py-1.5 rounded-lg text-sm text-muted-foreground hover:bg-muted hover:text-foreground transition-colors"
            >
              <div className={`w-6 h-6 rounded-full ${user.color} flex items-center justify-center`}>
                <span className="text-[10px] font-medium text-white">{user.initials}</span>
              </div>
              <span>{user.name}</span>
            </motion.button>
          ))}
        </div>
        <button
          onClick={toggleShowMore}
          className="flex items-center gap-1 px-3 text-xs text-muted-foreground hover:text-foreground transition-colors"
        >
          <motion.div animate={{ rotate: showMore ? 180 : 0 }} transition={{ duration: 0.2 }}>
            <ChevronDown className="w-3 h-3" />
          </motion.div>
          <span>Show more (14)</span>
        </button>
      </div>

      {/* Upgrade */}
      <div className="p-4">
        <motion.button
          whileHover={{ scale: 1.02, boxShadow: "0 4px 20px -4px hsl(var(--primary) / 0.4)" }}
          whileTap={{ scale: 0.98 }}
          className="w-full flex items-center justify-center gap-2 h-10 rounded-xl bg-primary text-primary-foreground text-sm font-medium hover:bg-primary/90 transition-colors"
        >
          <Crown className="w-4 h-4" />
          Upgrade to PRO
        </motion.button>
      </div>
    </aside>
  );
};

const MemoizedSidebar = React.memo(Sidebar);

export default MemoizedSidebar;
