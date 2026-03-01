import CyberSidebar from "./CyberSidebar";
import CyberTopNav from "./CyberTopNav";
import { motion } from "framer-motion";
import { ArrowUpRight, Plus, Import, Video, TrendingUp } from "lucide-react";
import { useAuth } from "@/context/AuthContext";

const statsCards = [
  { label: "Total Projects", value: "24", sub: "Increased from last month", highlight: true, positive: true },
  { label: "Ended Projects", value: "10", sub: "Increased from last month", positive: true },
  { label: "Running Projects", value: "12", sub: "Increased from last month", positive: true },
  { label: "Pending Project", value: "2", sub: "On Discuss", positive: false },
];

const weekDays = ["S", "M", "T", "W", "T", "F", "S"];
const barHeights = [40, 55, 85, 60, 45, 70, 50];

const teamMembers = [
  { name: "Alexandra Deff", task: "Github Project Repository", status: "Completed", color: "bg-primary/20 text-primary" },
  { name: "Edwin Adenike", task: "Integrate User Authentication System", status: "In Progress", color: "bg-yellow-100 text-yellow-700 dark:bg-yellow-900/30 dark:text-yellow-400" },
  { name: "Isaac Oluwatemilorun", task: "Develop Search and Filter Functionality", status: "Pending", color: "bg-red-100 text-red-600 dark:bg-red-900/30 dark:text-red-400" },
  { name: "David Oshodi", task: "Responsive Layout for Homepage", status: "In Progress", color: "bg-yellow-100 text-yellow-700 dark:bg-yellow-900/30 dark:text-yellow-400" },
];

const projects = [
  { icon: "⚡", name: "Develop API Endpoints", date: "Nov 26, 2024", color: "text-blue-500" },
  { icon: "🔄", name: "Onboarding Flow", date: "Nov 28, 2024", color: "text-purple-500" },
  { icon: "📊", name: "Build Dashboard", date: "Nov 30, 2024", color: "text-orange-500" },
  { icon: "🚀", name: "Optimize Page Load", date: "Dec 5, 2024", color: "text-yellow-500" },
  { icon: "🧪", name: "Cross-Browser Testing", date: "Dec 6, 2024", color: "text-pink-500" },
];

const CyberDashboard = () => {
  const { user } = useAuth();

  return (
    <div className="flex flex-col h-screen overflow-hidden">
      <CyberTopNav />
      <div className="flex flex-1 overflow-hidden">
        <CyberSidebar />
        <main className="flex-1 overflow-y-auto p-6 bg-background">
          {/* Header */}
          <motion.div
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            className="flex items-center justify-between mb-6"
          >
            <div>
              <h1 className="text-2xl font-bold text-foreground">Dashboard</h1>
              <p className="text-sm text-muted-foreground">Plan, prioritize, and accomplish your tasks with ease.</p>
            </div>
            <div className="flex items-center gap-3">
              <motion.button
                whileHover={{ scale: 1.02 }}
                whileTap={{ scale: 0.98 }}
                className="h-10 px-5 rounded-xl bg-primary text-primary-foreground text-sm font-medium flex items-center gap-2 hover:bg-primary/90 transition-colors"
              >
                <Plus className="w-4 h-4" />
                Add Project
              </motion.button>
              <motion.button
                whileHover={{ scale: 1.02 }}
                whileTap={{ scale: 0.98 }}
                className="h-10 px-5 rounded-xl border border-border text-foreground text-sm font-medium flex items-center gap-2 hover:bg-muted transition-colors"
              >
                <Import className="w-4 h-4" />
                Import Data
              </motion.button>
            </div>
          </motion.div>

          {/* Stats Cards */}
          <div className="grid grid-cols-4 gap-4 mb-6">
            {statsCards.map((stat, i) => (
              <motion.div
                key={stat.label}
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: i * 0.08 }}
                whileHover={{ y: -3, boxShadow: "0 8px 25px -8px hsl(var(--primary) / 0.12)" }}
                className={`rounded-2xl border p-5 cursor-pointer transition-colors ${
                  stat.highlight
                    ? "bg-primary text-primary-foreground border-primary"
                    : "bg-card border-border"
                }`}
              >
                <div className="flex items-center justify-between mb-3">
                  <p className={`text-sm font-medium ${stat.highlight ? "text-primary-foreground/80" : "text-muted-foreground"}`}>{stat.label}</p>
                  <ArrowUpRight className={`w-4 h-4 ${stat.highlight ? "text-primary-foreground/60" : "text-muted-foreground"}`} />
                </div>
                <motion.p
                  initial={{ opacity: 0 }}
                  animate={{ opacity: 1 }}
                  transition={{ delay: 0.3 + i * 0.08 }}
                  className={`text-4xl font-bold mb-2 ${stat.highlight ? "" : "text-foreground"}`}
                >
                  {stat.value}
                </motion.p>
                <div className="flex items-center gap-1.5">
                  {stat.positive && <TrendingUp className={`w-3 h-3 ${stat.highlight ? "text-primary-foreground/70" : "text-primary"}`} />}
                  <span className={`text-xs ${stat.highlight ? "text-primary-foreground/70" : "text-muted-foreground"}`}>{stat.sub}</span>
                </div>
              </motion.div>
            ))}
          </div>

          {/* Second Row: Analytics, Reminders, Projects */}
          <div className="grid grid-cols-3 gap-4 mb-6">
            {/* Project Analytics */}
            <motion.div
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: 0.3 }}
              className="bg-card rounded-2xl border border-border p-5"
            >
              <h3 className="text-sm font-semibold text-foreground mb-4">Project Analytics</h3>
              <div className="flex items-end gap-2 h-36">
                {weekDays.map((day, i) => (
                  <div key={i} className="flex-1 flex flex-col items-center gap-2">
                    <div className="w-full flex justify-center">
                      <motion.div
                        initial={{ height: 0 }}
                        animate={{ height: barHeights[i] }}
                        transition={{ delay: 0.4 + i * 0.06, duration: 0.6, ease: "easeOut" }}
                        className={`w-5 rounded-full ${i === 2 ? "bg-primary" : "bg-primary/30"}`}
                        style={{ maxHeight: barHeights[i] }}
                      />
                    </div>
                    <span className="text-[10px] text-muted-foreground">{day}</span>
                  </div>
                ))}
              </div>
            </motion.div>

            {/* Reminders */}
            <motion.div
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: 0.35 }}
              className="bg-card rounded-2xl border border-border p-5 flex flex-col"
            >
              <h3 className="text-sm font-semibold text-foreground mb-4">Reminders</h3>
              <div className="flex-1">
                <h4 className="text-lg font-bold text-foreground">Meeting with Arc Company</h4>
                <p className="text-sm text-muted-foreground mt-1">Time : 02.00 pm - 04.00 pm</p>
              </div>
              <motion.button
                whileHover={{ scale: 1.02 }}
                whileTap={{ scale: 0.98 }}
                className="w-full h-11 mt-4 rounded-full bg-primary text-primary-foreground text-sm font-medium flex items-center justify-center gap-2 hover:bg-primary/90 transition-colors"
              >
                <Video className="w-4 h-4" />
                Start Meeting
              </motion.button>
            </motion.div>

            {/* Projects List */}
            <motion.div
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: 0.4 }}
              className="bg-card rounded-2xl border border-border p-5"
            >
              <div className="flex items-center justify-between mb-4">
                <h3 className="text-sm font-semibold text-foreground">Project</h3>
                <button className="text-xs border border-border rounded-lg px-3 py-1.5 text-muted-foreground hover:bg-muted transition-colors">+ New</button>
              </div>
              <div className="space-y-3">
                {projects.map((p, i) => (
                  <motion.div
                    key={p.name}
                    initial={{ opacity: 0, x: 10 }}
                    animate={{ opacity: 1, x: 0 }}
                    transition={{ delay: 0.5 + i * 0.05 }}
                    className="flex items-center gap-3 cursor-pointer hover:bg-muted/50 rounded-lg p-1.5 -mx-1.5 transition-colors"
                  >
                    <span className="text-lg">{p.icon}</span>
                    <div className="flex-1 min-w-0">
                      <p className="text-sm font-medium text-foreground truncate">{p.name}</p>
                      <p className="text-[11px] text-muted-foreground">Due date: {p.date}</p>
                    </div>
                  </motion.div>
                ))}
              </div>
            </motion.div>
          </div>

          {/* Third Row: Team Collaboration, Progress, Time Tracker */}
          <div className="grid grid-cols-[1.2fr_1fr_0.8fr] gap-4">
            {/* Team Collaboration */}
            <motion.div
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: 0.45 }}
              className="bg-card rounded-2xl border border-border p-5"
            >
              <div className="flex items-center justify-between mb-4">
                <h3 className="text-sm font-semibold text-foreground">Team Collaboration</h3>
                <button className="text-xs border border-border rounded-lg px-3 py-1.5 text-muted-foreground hover:bg-muted transition-colors">+ Add Member</button>
              </div>
              <div className="space-y-3">
                {teamMembers.map((m, i) => (
                  <motion.div
                    key={m.name}
                    initial={{ opacity: 0, x: -10 }}
                    animate={{ opacity: 1, x: 0 }}
                    transition={{ delay: 0.55 + i * 0.05 }}
                    className="flex items-center gap-3"
                  >
                    <div className="w-8 h-8 rounded-full bg-primary/15 flex items-center justify-center shrink-0">
                      <span className="text-xs font-semibold text-primary">{m.name.split(" ").map(n => n[0]).join("")}</span>
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="text-sm font-medium text-foreground">{m.name}</p>
                      <p className="text-[11px] text-muted-foreground truncate">Working on <span className="font-medium text-foreground">{m.task}</span></p>
                    </div>
                    <span className={`text-[10px] font-medium px-2.5 py-1 rounded-md ${m.color}`}>{m.status}</span>
                  </motion.div>
                ))}
              </div>
            </motion.div>

            {/* Project Progress */}
            <motion.div
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: 0.5 }}
              className="bg-card rounded-2xl border border-border p-5 flex flex-col items-center justify-center"
            >
              <h3 className="text-sm font-semibold text-foreground mb-6 self-start">Project Progress</h3>
              <div className="relative w-36 h-36">
                <svg viewBox="0 0 120 120" className="w-full h-full -rotate-90">
                  <circle cx="60" cy="60" r="50" fill="none" stroke="hsl(var(--muted))" strokeWidth="10" />
                  <motion.circle
                    cx="60" cy="60" r="50" fill="none"
                    stroke="hsl(var(--primary))"
                    strokeWidth="10"
                    strokeLinecap="round"
                    strokeDasharray={314}
                    initial={{ strokeDashoffset: 314 }}
                    animate={{ strokeDashoffset: 314 * (1 - 0.41) }}
                    transition={{ delay: 0.6, duration: 1.2, ease: "easeOut" }}
                  />
                </svg>
                <div className="absolute inset-0 flex flex-col items-center justify-center">
                  <span className="text-3xl font-bold text-foreground">41%</span>
                  <span className="text-[11px] text-muted-foreground">Project Ended</span>
                </div>
              </div>
              <div className="flex items-center gap-4 mt-4">
                <div className="flex items-center gap-1.5"><div className="w-2.5 h-2.5 rounded-full bg-primary" /><span className="text-[11px] text-muted-foreground">Completed</span></div>
                <div className="flex items-center gap-1.5"><div className="w-2.5 h-2.5 rounded-full bg-primary/40" /><span className="text-[11px] text-muted-foreground">In Progress</span></div>
                <div className="flex items-center gap-1.5"><div className="w-2.5 h-2.5 rounded-full bg-muted" /><span className="text-[11px] text-muted-foreground">Pending</span></div>
              </div>
            </motion.div>

            {/* Time Tracker */}
            <motion.div
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: 0.55 }}
              whileHover={{ y: -2 }}
              className="bg-primary rounded-2xl p-5 text-primary-foreground relative overflow-hidden"
            >
              {/* Decorative pattern */}
              <div className="absolute inset-0 opacity-10">
                {[...Array(8)].map((_, i) => (
                  <div key={i} className="absolute w-full h-px bg-primary-foreground" style={{ top: `${12 + i * 12}%`, transform: `rotate(-45deg)` }} />
                ))}
              </div>
              <div className="relative z-10">
                <h3 className="text-sm font-semibold mb-4 opacity-90">Time Tracker</h3>
                <motion.p
                  initial={{ opacity: 0 }}
                  animate={{ opacity: 1 }}
                  transition={{ delay: 0.7 }}
                  className="text-4xl font-bold font-mono tracking-wider mb-6"
                >
                  01:24:08
                </motion.p>
                <div className="flex gap-2">
                  <button className="w-10 h-10 rounded-full bg-primary-foreground/20 flex items-center justify-center hover:bg-primary-foreground/30 transition-colors">
                    <div className="w-3 h-3 flex gap-0.5">
                      <div className="w-1 h-3 bg-primary-foreground rounded-sm" />
                      <div className="w-1 h-3 bg-primary-foreground rounded-sm" />
                    </div>
                  </button>
                  <button className="w-10 h-10 rounded-full bg-destructive/80 flex items-center justify-center hover:bg-destructive transition-colors">
                    <div className="w-3 h-3 bg-primary-foreground rounded-sm" />
                  </button>
                </div>
              </div>
            </motion.div>
          </div>
        </main>
      </div>
    </div>
  );
};

export default CyberDashboard;
