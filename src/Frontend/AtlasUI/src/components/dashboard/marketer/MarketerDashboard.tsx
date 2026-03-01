import MarketerTopNav from "./MarketerTopNav";
import { motion } from "framer-motion";
import { ArrowUpRight, Users, Briefcase, FolderOpen, Play, Pause, Clock, ChevronDown, Check } from "lucide-react";
import { useAuth } from "@/context/AuthContext";

const topStats = [
  { label: "Employee", value: "78", icon: Users },
  { label: "Hirings", value: "56", icon: Briefcase },
  { label: "Projects", value: "203", icon: FolderOpen },
];

const barLabels = ["Interviews", "Hired", "Project time", "Output"];
const barWidths = ["15%", "15%", "60%", "10%"];

const weekDays = ["S", "M", "T", "W", "T", "F", "S"];
const progressBars = [30, 55, 80, 45, 65, 40, 50];

const onboardingTasks = [
  { icon: "💻", label: "Interview", time: "Sep 13, 08:30", done: true },
  { icon: "⚡", label: "Team Meeting", time: "Sep 13, 10:30", done: true },
  { icon: "💬", label: "Project Update", time: "Sep 13, 13:00", done: false },
  { icon: "📝", label: "Discuss Q3 Goals", time: "Sep 13, 14:45", done: false },
  { icon: "🔗", label: "HR Policy Review", time: "Sep 13, 16:30", done: false },
];

const calendarEvents = [
  { time: "9:00 am", title: "Weekly Team Sync", desc: "Discuss progress on projects", color: "bg-primary" },
  { time: "10:00 am", title: "Onboarding Session", desc: "Introduction for new hires", color: "bg-primary/60" },
];

const MarketerDashboard = () => {
  const { user } = useAuth();
  const displayName = user?.fullName?.split(" ")[0] || "Nixtio";

  return (
    <div className="flex flex-col h-screen overflow-hidden bg-background">
      <MarketerTopNav />
      <main className="flex-1 overflow-y-auto p-6">
        {/* Welcome */}
        <motion.h1
          initial={{ opacity: 0, y: 10 }}
          animate={{ opacity: 1, y: 0 }}
          className="text-3xl font-bold text-foreground mb-6"
        >
          Welcome in, {displayName}
        </motion.h1>

        {/* Top Bar: Tags + Stats */}
        <motion.div
          initial={{ opacity: 0, y: 10 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.1 }}
          className="flex items-center justify-between mb-6"
        >
          <div className="flex items-center gap-3">
            {barLabels.map((label, i) => (
              <div key={label} className="flex flex-col items-start">
                <span className="text-[11px] text-muted-foreground mb-1">{label}</span>
                <div className="flex items-center gap-2">
                  <motion.div
                    initial={{ width: 0 }}
                    animate={{ width: 80 }}
                    transition={{ delay: 0.2 + i * 0.1, duration: 0.6 }}
                    className="h-8 rounded-full overflow-hidden bg-muted"
                  >
                    <motion.div
                      initial={{ width: 0 }}
                      animate={{ width: barWidths[i] === "60%" ? "100%" : barWidths[i] === "15%" ? "100%" : "66%" }}
                      transition={{ delay: 0.4 + i * 0.1, duration: 0.8 }}
                      className={`h-full rounded-full ${i < 2 ? "bg-foreground" : i === 2 ? "bg-primary" : "bg-muted-foreground/30"} flex items-center justify-center`}
                    >
                      <span className="text-[10px] font-medium text-primary-foreground px-2">{barWidths[i]}</span>
                    </motion.div>
                  </motion.div>
                </div>
              </div>
            ))}
          </div>
          
          {/* Line chart placeholder */}
          <div className="hidden xl:flex items-center gap-1 h-10 flex-1 mx-8">
            <svg viewBox="0 0 200 40" className="w-full h-full">
              <motion.path
                d="M0 35 Q20 30 40 25 T80 20 T120 15 T160 18 T200 10"
                fill="none"
                stroke="hsl(var(--muted-foreground))"
                strokeWidth="1.5"
                strokeDasharray="4 3"
                initial={{ pathLength: 0 }}
                animate={{ pathLength: 1 }}
                transition={{ delay: 0.3, duration: 1.5 }}
              />
            </svg>
          </div>

          <div className="flex items-center gap-6">
            {topStats.map((s, i) => (
              <motion.div
                key={s.label}
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 0.3 + i * 0.08 }}
                className="flex items-center gap-2"
              >
                <s.icon className="w-4 h-4 text-muted-foreground" />
                <span className="text-3xl font-bold text-foreground">{s.value}</span>
                <span className="text-xs text-muted-foreground">{s.label}</span>
              </motion.div>
            ))}
          </div>
        </motion.div>

        {/* Main Grid */}
        <div className="grid grid-cols-12 gap-4">
          {/* Profile Card */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.2 }}
            className="col-span-3 bg-card rounded-2xl border border-border p-5 relative overflow-hidden"
          >
            <div className="w-full h-40 rounded-xl bg-gradient-to-br from-primary/20 to-primary/5 mb-4 flex items-center justify-center">
              <div className="w-20 h-20 rounded-full bg-primary/20 flex items-center justify-center">
                <span className="text-2xl font-bold text-primary">{displayName.charAt(0)}</span>
              </div>
            </div>
            <h3 className="text-lg font-bold text-foreground">{user?.fullName || "Lora Piterson"}</h3>
            <p className="text-xs text-muted-foreground mb-3">{user?.role || "Marketer"}</p>
            <div className="inline-flex px-3 py-1.5 rounded-full bg-primary text-primary-foreground text-xs font-semibold">
              $1,200
            </div>
          </motion.div>

          {/* Progress */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.25 }}
            className="col-span-3 bg-card rounded-2xl border border-border p-5"
          >
            <div className="flex items-center justify-between mb-2">
              <h3 className="text-sm font-semibold text-foreground">Progress</h3>
              <ArrowUpRight className="w-4 h-4 text-muted-foreground" />
            </div>
            <div className="flex items-baseline gap-2 mb-4">
              <span className="text-3xl font-bold text-foreground">6.1</span>
              <span className="text-lg font-semibold text-foreground">h</span>
              <span className="text-xs text-muted-foreground">Work Time this week</span>
            </div>
            <div className="flex items-end gap-1.5 h-24">
              {weekDays.map((d, i) => (
                <div key={i} className="flex-1 flex flex-col items-center gap-1">
                  <motion.div
                    initial={{ height: 0 }}
                    animate={{ height: progressBars[i] }}
                    transition={{ delay: 0.4 + i * 0.05, duration: 0.5 }}
                    className={`w-full rounded-full ${i === 3 ? "bg-primary" : "bg-primary/25"}`}
                    style={{ maxHeight: progressBars[i] }}
                  />
                  <span className="text-[9px] text-muted-foreground">{d}</span>
                </div>
              ))}
            </div>
          </motion.div>

          {/* Time Tracker */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.3 }}
            className="col-span-3 bg-card rounded-2xl border border-border p-5 flex flex-col items-center"
          >
            <div className="flex items-center justify-between w-full mb-4">
              <h3 className="text-sm font-semibold text-foreground">Time tracker</h3>
              <ArrowUpRight className="w-4 h-4 text-muted-foreground" />
            </div>
            <div className="relative w-32 h-32 mb-4">
              <svg viewBox="0 0 120 120" className="w-full h-full -rotate-90">
                <circle cx="60" cy="60" r="50" fill="none" stroke="hsl(var(--muted))" strokeWidth="8" />
                <motion.circle
                  cx="60" cy="60" r="50" fill="none"
                  stroke="hsl(var(--primary))"
                  strokeWidth="8"
                  strokeLinecap="round"
                  strokeDasharray={314}
                  initial={{ strokeDashoffset: 314 }}
                  animate={{ strokeDashoffset: 314 * 0.35 }}
                  transition={{ delay: 0.5, duration: 1.2, ease: "easeOut" }}
                />
              </svg>
              <div className="absolute inset-0 flex flex-col items-center justify-center">
                <span className="text-2xl font-bold text-foreground">02:35</span>
                <span className="text-[10px] text-muted-foreground">Work Time</span>
              </div>
            </div>
            <div className="flex gap-2">
              <button className="w-10 h-10 rounded-full bg-muted flex items-center justify-center text-foreground hover:bg-primary/10 transition-colors">
                <Play className="w-4 h-4" />
              </button>
              <button className="w-10 h-10 rounded-full bg-muted flex items-center justify-center text-foreground hover:bg-primary/10 transition-colors">
                <Pause className="w-4 h-4" />
              </button>
              <button className="w-10 h-10 rounded-full bg-muted flex items-center justify-center text-foreground hover:bg-primary/10 transition-colors">
                <Clock className="w-4 h-4" />
              </button>
            </div>
          </motion.div>

          {/* Onboarding */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.35 }}
            className="col-span-3 row-span-2 bg-muted/30 rounded-2xl border border-border p-5"
          >
            <div className="flex items-center justify-between mb-2">
              <h3 className="text-sm font-semibold text-foreground">Onboarding</h3>
              <span className="text-xl font-bold text-foreground">18%</span>
            </div>
            <div className="flex gap-1 mb-4">
              {[30, 25, 0].map((w, i) => (
                <motion.div
                  key={i}
                  initial={{ width: 0 }}
                  animate={{ width: `${w || 5}%` }}
                  transition={{ delay: 0.4 + i * 0.1 }}
                  className={`h-7 rounded-lg flex items-center justify-center text-[10px] font-medium ${
                    i === 0 ? "bg-primary text-primary-foreground" : i === 1 ? "bg-foreground text-background" : "bg-muted text-muted-foreground"
                  }`}
                  style={{ minWidth: 30 }}
                >
                  {w}%
                </motion.div>
              ))}
            </div>

            {/* Onboarding Task */}
            <div className="bg-card rounded-xl p-4 border border-border">
              <div className="flex items-center justify-between mb-3">
                <h4 className="text-sm font-semibold text-foreground">Onboarding Task</h4>
                <span className="text-lg font-bold text-foreground">2/8</span>
              </div>
              <div className="space-y-2.5">
                {onboardingTasks.map((t, i) => (
                  <motion.div
                    key={t.label}
                    initial={{ opacity: 0, x: 10 }}
                    animate={{ opacity: 1, x: 0 }}
                    transition={{ delay: 0.5 + i * 0.05 }}
                    className="flex items-center gap-3"
                  >
                    <div className="w-7 h-7 rounded-lg bg-muted flex items-center justify-center text-sm shrink-0">
                      {t.icon}
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="text-xs font-medium text-foreground">{t.label}</p>
                      <p className="text-[10px] text-muted-foreground">{t.time}</p>
                    </div>
                    <div className={`w-5 h-5 rounded-full flex items-center justify-center ${
                      t.done ? "bg-primary text-primary-foreground" : "border border-muted-foreground/30"
                    }`}>
                      {t.done && <Check className="w-3 h-3" />}
                    </div>
                  </motion.div>
                ))}
              </div>
            </div>
          </motion.div>

          {/* Bottom section: Accordion + Calendar */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.4 }}
            className="col-span-3 bg-card rounded-2xl border border-border p-5"
          >
            <div className="space-y-3">
              {["Pension contributions", "Devices", "Compensation Summary", "Employee Benefits"].map((item, i) => (
                <button key={item} className="w-full flex items-center justify-between py-2.5 border-b border-border last:border-0 text-sm text-foreground hover:text-primary transition-colors">
                  <span className={i === 1 ? "font-semibold" : ""}>{item}</span>
                  <ChevronDown className={`w-4 h-4 text-muted-foreground ${i === 1 ? "rotate-180" : ""}`} />
                </button>
              ))}
              {/* Device detail */}
              <div className="flex items-center gap-3 pl-2 py-1">
                <div className="w-10 h-10 rounded-lg bg-muted flex items-center justify-center text-lg">💻</div>
                <div>
                  <p className="text-sm font-medium text-foreground">MacBook Air</p>
                  <p className="text-[11px] text-muted-foreground">Version M1</p>
                </div>
              </div>
            </div>
          </motion.div>

          {/* Calendar */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.45 }}
            className="col-span-6 bg-card rounded-2xl border border-border p-5"
          >
            <div className="flex items-center justify-between mb-4">
              <div className="flex gap-2">
                <button className="px-3 py-1.5 rounded-lg bg-primary text-primary-foreground text-xs font-medium">August</button>
                <span className="text-sm font-semibold text-foreground self-center">September 2024</span>
                <button className="px-3 py-1.5 rounded-lg border border-border text-xs text-muted-foreground">October</button>
              </div>
            </div>
            {/* Week header */}
            <div className="grid grid-cols-6 gap-3 mb-3">
              {["Mon", "Tue", "Wed", "Thu", "Fri", "Sat"].map(d => (
                <div key={d} className="text-center">
                  <p className="text-xs text-muted-foreground">{d}</p>
                  <p className="text-sm font-semibold text-foreground">{22 + ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat"].indexOf(d)}</p>
                </div>
              ))}
            </div>
            {/* Time slots */}
            <div className="space-y-2">
              {["8:00 am", "9:00 am", "10:00 am", "11:00 am"].map((time, i) => (
                <div key={time} className="grid grid-cols-[70px_1fr] gap-3 min-h-[40px]">
                  <span className="text-[11px] text-muted-foreground pt-1">{time}</span>
                  <div>
                    {calendarEvents[i] && (
                      <motion.div
                        initial={{ opacity: 0, scale: 0.95 }}
                        animate={{ opacity: 1, scale: 1 }}
                        transition={{ delay: 0.6 + i * 0.1 }}
                        className={`${calendarEvents[i].color} rounded-xl p-3 text-primary-foreground`}
                      >
                        <p className="text-xs font-semibold">{calendarEvents[i].title}</p>
                        <p className="text-[10px] opacity-80">{calendarEvents[i].desc}</p>
                      </motion.div>
                    )}
                  </div>
                </div>
              ))}
            </div>
          </motion.div>
        </div>
      </main>
    </div>
  );
};

export default MarketerDashboard;
