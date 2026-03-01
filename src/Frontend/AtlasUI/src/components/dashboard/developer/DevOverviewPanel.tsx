import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import {
  Clock, Zap, Target, TrendingUp, Coffee,
  Terminal, GitBranch, Bug, ArrowRight, ArrowUpRight,
  Flame, Code2, Timer, Sparkles, SquareKanban,
  CalendarDays, CheckCircle2,
} from "lucide-react";
import { useAuth } from "@/context/AuthContext";
import { greetingApi, devInsightsApi, focusApi, GreetingDto, TimeSavedDto, FocusStatsDto } from "@/services/api";

interface DevOverviewPanelProps {
  onTabChange: (tab: string) => void;
}

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

const DevOverviewPanel = ({ onTabChange }: DevOverviewPanelProps) => {
  const { user } = useAuth();
  const [greeting, setGreeting] = useState<GreetingDto | null>(null);
  const [timeSaved, setTimeSaved] = useState<TimeSavedDto | null>(null);
  const [focusStats, setFocusStats] = useState<FocusStatsDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = async () => {
      const [gRes, tRes, fRes] = await Promise.allSettled([
        greetingApi.get(user?.userName),
        devInsightsApi.timeSaved(),
        focusApi.getStats(),
      ]);
      if (gRes.status === "fulfilled" && gRes.value.data.isSuccess) setGreeting(gRes.value.data.data);
      if (tRes.status === "fulfilled" && tRes.value.data.isSuccess) setTimeSaved(tRes.value.data.data);
      if (fRes.status === "fulfilled" && fRes.value.data.isSuccess) setFocusStats(fRes.value.data.data);
      setLoading(false);
    };
    load();
  }, [user?.userName]);

  const displayName = user?.fullName?.split(" ")[0] || "Developer";
  const greetText = greeting?.greeting || `Welcome back, ${displayName}`;

  const stats = [
    {
      label: "Time Saved",
      value: timeSaved ? `${Math.round(timeSaved.totalMinutes)}m` : "—",
      sub: "by AI this week",
      icon: Clock,
      color: "text-blue-400",
      bgFrom: "from-blue-500/12",
      bgTo: "to-blue-500/3",
      borderColor: "border-blue-500/10",
      trend: "+12%",
      trendUp: true,
    },
    {
      label: "Focus Sessions",
      value: focusStats?.todaySessions?.toString() || "0",
      sub: "completed today",
      icon: Target,
      color: "text-emerald-400",
      bgFrom: "from-emerald-500/12",
      bgTo: "to-emerald-500/3",
      borderColor: "border-emerald-500/10",
      trend: focusStats?.todaySessions ? `${focusStats.todaySessions} done` : "Start one",
      trendUp: true,
    },
    {
      label: "Deep Work",
      value: focusStats ? `${focusStats.todayMinutes}m` : "0m",
      sub: "focus time today",
      icon: Timer,
      color: "text-violet-400",
      bgFrom: "from-violet-500/12",
      bgTo: "to-violet-500/3",
      borderColor: "border-violet-500/10",
      trend: "today",
      trendUp: true,
    },
    {
      label: "Streak",
      value: focusStats?.streak?.toString() || "0",
      sub: "day streak 🔥",
      icon: Flame,
      color: "text-orange-400",
      bgFrom: "from-orange-500/12",
      bgTo: "to-orange-500/3",
      borderColor: "border-orange-500/10",
      trend: focusStats?.streak ? `${focusStats.streak} days` : "Start today",
      trendUp: !!focusStats?.streak,
    },
  ];

  const quickActions = [
    { label: "Utilities", desc: "JWT, Regex, Base64...", icon: Terminal, tab: "utilities", gradient: "from-blue-500/8 to-transparent" },
    { label: "AI Agents", desc: "Error explain, PR review", icon: Sparkles, tab: "ai-agents", gradient: "from-violet-500/8 to-transparent" },
    { label: "Focus Timer", desc: "Start a pomodoro", icon: Coffee, tab: "focus", gradient: "from-emerald-500/8 to-transparent" },
    { label: "Scripts", desc: "Run automations", icon: Code2, tab: "scripts", gradient: "from-amber-500/8 to-transparent" },
    { label: "GitHub", desc: "PRs, repos & commits", icon: GitBranch, tab: "github", gradient: "from-slate-500/8 to-transparent" },
    { label: "Jira", desc: "Sprint tasks & boards", icon: SquareKanban, tab: "jira", gradient: "from-blue-600/8 to-transparent" },
    { label: "Snippets", desc: "Your code snippets", icon: Code2, tab: "snippets", gradient: "from-cyan-500/8 to-transparent" },
    { label: "Monitoring", desc: "Sentry & SonarQube", icon: Bug, tab: "monitoring", gradient: "from-red-500/8 to-transparent" },
  ];

  if (loading) {
    return (
      <div className="space-y-5">
        {/* Skeleton for greeting */}
        <div className="rounded-2xl border border-border/50 p-6 bg-gradient-to-br from-primary/[0.03] to-transparent">
          <div className="h-7 w-64 bg-muted/40 rounded-lg animate-pulse mb-2" />
          <div className="h-4 w-96 bg-muted/30 rounded animate-pulse" />
        </div>
        {/* Skeleton for stats */}
        <div className="grid grid-cols-2 lg:grid-cols-4 gap-3">
          {[1,2,3,4].map(i => (
            <div key={i} className="rounded-xl border border-border/40 p-4 bg-card/40">
              <div className="w-4 h-4 bg-muted/40 rounded mb-3 animate-pulse" />
              <div className="h-7 w-16 bg-muted/40 rounded animate-pulse mb-1" />
              <div className="h-3 w-24 bg-muted/30 rounded animate-pulse" />
            </div>
          ))}
        </div>
      </div>
    );
  }

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-5">
      {/* Hero Greeting */}
      <motion.div
        variants={item}
        className="relative overflow-hidden rounded-2xl border border-primary/6 p-6"
      >
        <div className="absolute inset-0 bg-gradient-to-br from-primary/[0.04] via-transparent to-primary/[0.02]" />
        <div className="absolute top-0 right-0 w-72 h-72 bg-primary/[0.03] rounded-full blur-[80px] -translate-y-1/2 translate-x-1/4" />
        <div className="absolute bottom-0 left-0 w-40 h-40 bg-blue-500/[0.02] rounded-full blur-[60px] translate-y-1/3 -translate-x-1/4" />
        <div className="relative flex items-start justify-between">
          <div>
            <h1 className="text-xl font-bold text-foreground mb-1 tracking-tight">
              {greetText} {greeting?.emoji || "👋"}
            </h1>
            <p className="text-[13px] text-muted-foreground max-w-md leading-relaxed">
              {greeting?.tip || "Your developer command center is ready. Ship great code today."}
            </p>
          </div>
          <div className="hidden md:flex items-center gap-1.5 text-[10px] text-muted-foreground/50 bg-muted/20 px-2 py-1 rounded-lg border border-border/30">
            <span className="w-1.5 h-1.5 rounded-full bg-emerald-500 animate-pulse" />
            Online
          </div>
        </div>
      </motion.div>

      {/* Stats Grid — Donezo inspired cards */}
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-3">
        {stats.map((stat) => (
          <motion.div
            key={stat.label}
            variants={item}
            whileHover={{ y: -2, transition: { duration: 0.15 } }}
            className={`relative overflow-hidden rounded-xl border ${stat.borderColor} bg-gradient-to-br ${stat.bgFrom} ${stat.bgTo} p-4 group cursor-default`}
          >
            <div className="absolute top-0 right-0 w-20 h-20 bg-primary/[0.02] rounded-full -translate-y-1/2 translate-x-1/3 group-hover:bg-primary/[0.04] transition-colors duration-500" />
            <div className="flex items-center justify-between mb-3">
              <stat.icon className={`w-4 h-4 ${stat.color}`} />
              <div className={`flex items-center gap-0.5 text-[9px] ${stat.trendUp ? "text-emerald-400" : "text-muted-foreground/50"}`}>
                {stat.trendUp && <ArrowUpRight className="w-2.5 h-2.5" />}
                {stat.trend}
              </div>
            </div>
            <p className="text-2xl font-bold text-foreground tracking-tight leading-none">{stat.value}</p>
            <p className="text-[10px] text-muted-foreground mt-1">{stat.sub}</p>
          </motion.div>
        ))}
      </div>

      {/* Two-column: Quick Actions + AI Trend */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">
        {/* Quick Actions — 2 cols */}
        <div className="lg:col-span-2">
          <motion.div variants={item} className="flex items-center gap-2 mb-3">
            <Zap className="w-4 h-4 text-primary" />
            <h3 className="text-[13px] font-semibold text-foreground tracking-tight">Quick Actions</h3>
          </motion.div>
          <div className="grid grid-cols-2 gap-2">
            {quickActions.map((qa) => (
              <motion.button
                key={qa.label}
                variants={item}
                whileHover={{ y: -1, transition: { duration: 0.15 } }}
                whileTap={{ scale: 0.98 }}
                onClick={() => onTabChange(qa.tab)}
                className={`relative overflow-hidden flex items-center gap-2.5 p-3 rounded-xl border border-border/60 bg-gradient-to-br ${qa.gradient} hover:border-primary/12 transition-all duration-200 text-left group`}
              >
                <div className="w-8 h-8 rounded-lg bg-card/60 border border-border/30 flex items-center justify-center shrink-0 group-hover:border-primary/15 transition-colors">
                  <qa.icon className="w-3.5 h-3.5 text-foreground/60 group-hover:text-primary transition-colors" />
                </div>
                <div className="min-w-0 flex-1">
                  <p className="text-[12px] font-medium text-foreground leading-tight">{qa.label}</p>
                  <p className="text-[9px] text-muted-foreground/70">{qa.desc}</p>
                </div>
                <ArrowRight className="w-3 h-3 text-muted-foreground/15 group-hover:text-primary/40 transition-all ml-auto shrink-0" />
              </motion.button>
            ))}
          </div>
        </div>

        {/* Right column: AI Time Saved Mini Chart + Today Summary */}
        <div className="space-y-3">
          {/* Time Saved Mini Chart */}
          <motion.div variants={item} className="relative overflow-hidden rounded-xl border border-border/60 p-4 bg-gradient-to-br from-card to-card/50">
            <div className="flex items-center justify-between mb-3">
              <div className="flex items-center gap-1.5">
                <TrendingUp className="w-3.5 h-3.5 text-primary" />
                <h3 className="text-[11px] font-semibold text-foreground">AI Time Saved</h3>
              </div>
              <button onClick={() => onTabChange("insights")} className="text-[9px] text-primary/70 hover:text-primary flex items-center gap-0.5 transition-colors">
                View all <ArrowRight className="w-2.5 h-2.5" />
              </button>
            </div>
            {timeSaved?.trend && timeSaved.trend.length > 0 ? (
              <div className="flex items-end gap-1 h-20">
                {timeSaved.trend.slice(-14).map((d, i) => {
                  const max = Math.max(...timeSaved.trend.map((t) => t.minutes), 1);
                  const h = Math.max((d.minutes / max) * 100, 4);
                  return (
                    <motion.div
                      key={d.date}
                      initial={{ height: 0 }}
                      animate={{ height: `${h}%` }}
                      transition={{ delay: 0.3 + i * 0.03, type: "spring", stiffness: 150, damping: 15 }}
                      className="flex-1 rounded-t-[2px] bg-primary/25 hover:bg-primary/45 transition-colors min-w-[3px] cursor-pointer group relative"
                    >
                      <div className="absolute -top-6 left-1/2 -translate-x-1/2 opacity-0 group-hover:opacity-100 transition-opacity text-[7px] text-foreground bg-card border border-border px-1 py-0.5 rounded shadow-lg whitespace-nowrap pointer-events-none">
                        {d.minutes}m
                      </div>
                    </motion.div>
                  );
                })}
              </div>
            ) : (
              <div className="h-20 flex items-center justify-center">
                <p className="text-[10px] text-muted-foreground/50">Start coding to see trends</p>
              </div>
            )}
          </motion.div>

          {/* Today's Summary Card */}
          <motion.div variants={item} className="rounded-xl border border-border/60 p-4 bg-gradient-to-br from-card to-card/50">
            <div className="flex items-center gap-1.5 mb-3">
              <CalendarDays className="w-3.5 h-3.5 text-primary" />
              <h3 className="text-[11px] font-semibold text-foreground">Today</h3>
            </div>
            <div className="space-y-2">
              <div className="flex items-center gap-2">
                <CheckCircle2 className="w-3 h-3 text-emerald-400" />
                <span className="text-[10px] text-foreground">{focusStats?.todaySessions || 0} focus sessions</span>
              </div>
              <div className="flex items-center gap-2">
                <Timer className="w-3 h-3 text-blue-400" />
                <span className="text-[10px] text-foreground">{focusStats?.todayMinutes || 0}m deep work</span>
              </div>
              <div className="flex items-center gap-2">
                <Flame className="w-3 h-3 text-orange-400" />
                <span className="text-[10px] text-foreground">{focusStats?.streak || 0} day streak</span>
              </div>
            </div>
          </motion.div>
        </div>
      </div>
    </motion.div>
  );
};

export default DevOverviewPanel;

