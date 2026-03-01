import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import {
  Clock, Zap, Target, TrendingUp, Coffee,
  Terminal, GitBranch, Bug, Activity, ArrowRight,
  Flame, Code2, Timer, Sparkles,
} from "lucide-react";
import { useAuth } from "@/context/AuthContext";
import { greetingApi, devInsightsApi, focusApi, GreetingDto, TimeSavedDto, FocusStatsDto } from "@/services/api";

interface DevOverviewPanelProps {
  onTabChange: (tab: string) => void;
}

const DevOverviewPanel = ({ onTabChange }: DevOverviewPanelProps) => {
  const { user } = useAuth();
  const [greeting, setGreeting] = useState<GreetingDto | null>(null);
  const [timeSaved, setTimeSaved] = useState<TimeSavedDto | null>(null);
  const [focusStats, setFocusStats] = useState<FocusStatsDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = async () => {
      setLoading(true);
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
      color: "text-blue-400 bg-blue-500/10",
    },
    {
      label: "Focus Sessions",
      value: focusStats?.todaySessions?.toString() || "0",
      sub: "today",
      icon: Target,
      color: "text-emerald-400 bg-emerald-500/10",
    },
    {
      label: "Focus Time",
      value: focusStats ? `${focusStats.todayMinutes}m` : "0m",
      sub: "deep work today",
      icon: Timer,
      color: "text-violet-400 bg-violet-500/10",
    },
    {
      label: "Streak",
      value: focusStats?.streak?.toString() || "0",
      sub: "day streak",
      icon: Flame,
      color: "text-orange-400 bg-orange-500/10",
    },
  ];

  const quickActions = [
    { label: "Dev Utilities", desc: "JWT, Regex, Base64...", icon: Terminal, tab: "utilities" },
    { label: "AI Agents", desc: "Error explain, PR review", icon: Sparkles, tab: "ai-agents" },
    { label: "Focus Timer", desc: "Start a pomodoro", icon: Coffee, tab: "focus" },
    { label: "Scripts", desc: "Run automations", icon: Code2, tab: "scripts" },
    { label: "Snippets", desc: "Your code snippets", icon: GitBranch, tab: "snippets" },
    { label: "Monitoring", desc: "Sentry & SonarQube", icon: Bug, tab: "monitoring" },
  ];

  return (
    <div className="space-y-6">
      {/* Hero Greeting */}
      <motion.div
        initial={{ opacity: 0, y: 12 }}
        animate={{ opacity: 1, y: 0 }}
        className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-primary/10 via-primary/5 to-transparent border border-primary/10 p-6"
      >
        <div className="absolute top-0 right-0 w-72 h-72 bg-primary/5 rounded-full blur-3xl -translate-y-1/2 translate-x-1/3" />
        <div className="absolute bottom-0 left-0 w-48 h-48 bg-blue-500/5 rounded-full blur-2xl translate-y-1/3 -translate-x-1/4" />
        <div className="relative">
          <h1 className="text-2xl font-bold text-foreground mb-1">
            {greetText} {greeting?.emoji || "👋"}
          </h1>
          <p className="text-sm text-muted-foreground max-w-lg">
            {greeting?.tip || "Your developer command center is ready. Ship great code today."}
          </p>
        </div>
      </motion.div>

      {/* Stats Grid */}
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-3">
        {stats.map((stat, i) => (
          <motion.div
            key={stat.label}
            initial={{ opacity: 0, y: 15 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.1 + i * 0.05 }}
            whileHover={{ y: -2, boxShadow: "0 8px 30px -12px hsl(var(--primary) / 0.15)" }}
            className="bg-card rounded-xl border border-border p-4"
          >
            <div className="flex items-center justify-between mb-3">
              <div className={`w-8 h-8 rounded-lg flex items-center justify-center ${stat.color}`}>
                <stat.icon className="w-4 h-4" />
              </div>
              <Activity className="w-3.5 h-3.5 text-muted-foreground/30" />
            </div>
            <p className="text-2xl font-bold text-foreground">{stat.value}</p>
            <p className="text-[11px] text-muted-foreground mt-0.5">{stat.sub}</p>
          </motion.div>
        ))}
      </div>

      {/* Quick Actions */}
      <div>
        <h3 className="text-sm font-semibold text-foreground mb-3 flex items-center gap-2">
          <Zap className="w-4 h-4 text-primary" />
          Quick Actions
        </h3>
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
          {quickActions.map((qa, i) => (
            <motion.button
              key={qa.label}
              initial={{ opacity: 0, y: 10 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: 0.2 + i * 0.04 }}
              whileHover={{ y: -2, boxShadow: "0 8px 30px -12px hsl(var(--primary) / 0.1)" }}
              whileTap={{ scale: 0.98 }}
              onClick={() => onTabChange(qa.tab)}
              className="flex items-center gap-3 p-4 rounded-xl bg-card border border-border hover:border-primary/20 transition-all text-left group"
            >
              <div className="w-10 h-10 rounded-lg bg-primary/10 flex items-center justify-center shrink-0">
                <qa.icon className="w-5 h-5 text-primary" />
              </div>
              <div className="flex-1 min-w-0">
                <p className="text-sm font-medium text-foreground">{qa.label}</p>
                <p className="text-[11px] text-muted-foreground">{qa.desc}</p>
              </div>
              <ArrowRight className="w-4 h-4 text-muted-foreground/30 group-hover:text-primary transition-colors" />
            </motion.button>
          ))}
        </div>
      </div>

      {/* Insights Preview */}
      <motion.div
        initial={{ opacity: 0, y: 15 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ delay: 0.4 }}
        className="bg-card rounded-xl border border-border p-5"
      >
        <div className="flex items-center justify-between mb-4">
          <div className="flex items-center gap-2">
            <TrendingUp className="w-4 h-4 text-primary" />
            <h3 className="text-sm font-semibold text-foreground">Insights Preview</h3>
          </div>
          <button
            onClick={() => onTabChange("insights")}
            className="text-xs text-primary hover:underline flex items-center gap-1"
          >
            View all <ArrowRight className="w-3 h-3" />
          </button>
        </div>
        {timeSaved && timeSaved.trend && timeSaved.trend.length > 0 ? (
          <div className="flex items-end gap-1.5 h-24">
            {timeSaved.trend.slice(-14).map((d, i) => {
              const max = Math.max(...timeSaved.trend.map((t) => t.minutes), 1);
              const h = Math.max((d.minutes / max) * 100, 4);
              return (
                <motion.div
                  key={d.date}
                  initial={{ height: 0 }}
                  animate={{ height: `${h}%` }}
                  transition={{ delay: 0.5 + i * 0.03, type: "spring", stiffness: 200 }}
                  className="flex-1 rounded-t bg-primary/30 hover:bg-primary/50 transition-colors min-w-[6px]"
                  title={`${d.date}: ${d.minutes}m saved`}
                />
              );
            })}
          </div>
        ) : (
          <div className="h-24 flex items-center justify-center">
            <p className="text-xs text-muted-foreground">No insights data yet. Start coding to see your trends!</p>
          </div>
        )}
        <p className="text-[10px] text-muted-foreground mt-2">Time saved by AI — last 14 days</p>
      </motion.div>
    </div>
  );
};

export default DevOverviewPanel;

