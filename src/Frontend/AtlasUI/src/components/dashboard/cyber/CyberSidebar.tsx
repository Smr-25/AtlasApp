import { motion } from "framer-motion";
import {
  LayoutDashboard,
  ListTodo,
  CalendarDays,
  BarChart3,
  Users,
  Settings,
  HelpCircle,
  LogOut,
  Smartphone,
  Download,
} from "lucide-react";

const menuItems = [
  { icon: LayoutDashboard, label: "Dashboard", active: true },
  { icon: ListTodo, label: "Tasks", badge: "12+" },
  { icon: CalendarDays, label: "Calendar" },
  { icon: BarChart3, label: "Analytics" },
  { icon: Users, label: "Team" },
];

const generalItems = [
  { icon: Settings, label: "Settings" },
  { icon: HelpCircle, label: "Help" },
  { icon: LogOut, label: "Logout" },
];

const CyberSidebar = () => {
  return (
    <aside className="w-56 shrink-0 border-r border-border bg-card flex flex-col h-full overflow-y-auto">
      <div className="p-5 flex-1">
        {/* Logo */}
        <div className="flex items-center gap-2.5 mb-8">
          <motion.div
            whileHover={{ rotate: 6, scale: 1.05 }}
            className="w-8 h-8 rounded-full bg-primary flex items-center justify-center"
          >
            <span className="text-primary-foreground font-bold text-sm">A</span>
          </motion.div>
          <span className="text-base font-semibold text-foreground">Atlas</span>
        </div>

        {/* MENU */}
        <p className="text-[10px] font-semibold text-muted-foreground tracking-widest uppercase mb-3">Menu</p>
        <nav className="space-y-0.5 mb-8">
          {menuItems.map((item, index) => (
            <motion.button
              key={item.label}
              initial={{ opacity: 0, x: -15 }}
              animate={{ opacity: 1, x: 0 }}
              transition={{ delay: index * 0.05 }}
              whileHover={{ x: 3 }}
              className={`w-full flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm transition-colors ${
                item.active
                  ? "text-foreground font-semibold"
                  : "text-muted-foreground hover:text-foreground hover:bg-muted"
              }`}
            >
              <item.icon className={`w-[18px] h-[18px] ${item.active ? "text-primary" : ""}`} />
              <span className="flex-1 text-left">{item.label}</span>
              {item.badge && (
                <span className="text-[10px] bg-primary/15 text-primary px-2 py-0.5 rounded-full font-medium">
                  {item.badge}
                </span>
              )}
            </motion.button>
          ))}
        </nav>

        {/* GENERAL */}
        <p className="text-[10px] font-semibold text-muted-foreground tracking-widest uppercase mb-3">General</p>
        <nav className="space-y-0.5">
          {generalItems.map((item) => (
            <motion.button
              key={item.label}
              whileHover={{ x: 3 }}
              className="w-full flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm text-muted-foreground hover:text-foreground hover:bg-muted transition-colors"
            >
              <item.icon className="w-[18px] h-[18px]" />
              <span>{item.label}</span>
            </motion.button>
          ))}
        </nav>
      </div>

      {/* Mobile App CTA */}
      <div className="p-4">
        <motion.div
          whileHover={{ scale: 1.02 }}
          className="bg-primary rounded-2xl p-4 text-primary-foreground"
        >
          <div className="w-8 h-8 rounded-lg bg-primary-foreground/20 flex items-center justify-center mb-3">
            <Smartphone className="w-4 h-4" />
          </div>
          <p className="text-sm font-semibold mb-0.5">Download our Mobile App</p>
          <p className="text-[11px] opacity-70 mb-3">Get easy in another way</p>
          <button className="w-full h-9 rounded-xl bg-primary-foreground text-primary text-xs font-semibold flex items-center justify-center gap-1.5 hover:opacity-90 transition-opacity">
            <Download className="w-3.5 h-3.5" />
            Download
          </button>
        </motion.div>
      </div>
    </aside>
  );
};

export default CyberSidebar;
