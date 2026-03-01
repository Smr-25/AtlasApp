import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import { BarChart3, Clock, Flame, TrendingUp, Zap, Activity } from "lucide-react";
import { devInsightsApi, TimeSavedDto, PeakHoursDto, DeploySuccessDto } from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.05 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

const InsightsPanel = () => {
  const [timeSaved, setTimeSaved] = useState<TimeSavedDto | null>(null);
  const [peakHours, setPeakHours] = useState<PeakHoursDto | null>(null);
  const [deployRate, setDeployRate] = useState<DeploySuccessDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      const [tRes, pRes, dRes] = await Promise.allSettled([
        devInsightsApi.timeSaved(),
        devInsightsApi.peakHours(),
        devInsightsApi.deploySuccessRate(),
      ]);
      if (tRes.status === "fulfilled" && tRes.value.data.isSuccess) setTimeSaved(tRes.value.data.data);
      if (pRes.status === "fulfilled" && pRes.value.data.isSuccess) setPeakHours(pRes.value.data.data);
      if (dRes.status === "fulfilled" && dRes.value.data.isSuccess) setDeployRate(dRes.value.data.data);
      setLoading(false);
    };
    load();
  }, []);

  if (loading) {
    return (
      <div className="flex items-center justify-center py-20">
        <div className="flex flex-col items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center animate-pulse">
            <BarChart3 className="w-5 h-5 text-primary" />
          </div>
          <p className="text-xs text-muted-foreground">Loading insights...</p>
        </div>
      </div>
    );
  }

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      {/* Header */}
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-primary/20 to-primary/5 border border-primary/10 flex items-center justify-center">
          <BarChart3 className="w-5 h-5 text-primary" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">Developer Insights</h2>
          <p className="text-xs text-muted-foreground">Track your productivity metrics and trends</p>
        </div>
      </motion.div>

      {/* Summary Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
        {[
          { label: "Time Saved by AI", value: timeSaved ? `${Math.round(timeSaved.totalMinutes)}m` : "—", icon: Clock, gradient: "from-blue-500/15 to-cyan-500/5", iconColor: "text-blue-400" },
          { label: "Deploy Success Rate", value: deployRate ? `${Math.round(deployRate.rate)}%` : "—", icon: TrendingUp, gradient: "from-emerald-500/15 to-green-500/5", iconColor: "text-emerald-400" },
          { label: "Peak Productivity", value: peakHours ? `${peakHours.bestHour}:00` : "—", icon: Flame, gradient: "from-orange-500/15 to-amber-500/5", iconColor: "text-orange-400" },
        ].map((stat, i) => (
          <motion.div
            key={stat.label}
            variants={item}
            whileHover={{ y: -3, transition: { duration: 0.2 } }}
            className={`relative overflow-hidden rounded-xl border border-border bg-gradient-to-br ${stat.gradient} p-4 group`}
          >
            <div className="absolute top-0 right-0 w-16 h-16 bg-primary/[0.03] rounded-full -translate-y-1/2 translate-x-1/3 group-hover:bg-primary/[0.05] transition-colors duration-500" />
            <stat.icon className={`w-4 h-4 ${stat.iconColor} mb-3`} />
            <p className="text-2xl font-bold text-foreground tracking-tight">{stat.value}</p>
            <p className="text-[10px] text-muted-foreground mt-0.5">{stat.label}</p>
          </motion.div>
        ))}
      </div>

      {/* Time Saved Chart */}
      <motion.div variants={item} className="relative overflow-hidden rounded-xl border border-border p-5 bg-gradient-to-br from-card to-card/60">
        <div className="absolute top-0 right-0 w-40 h-40 bg-primary/[0.02] rounded-full -translate-y-1/2 translate-x-1/3" />
        <div className="flex items-center gap-2 mb-4 relative">
          <Clock className="w-4 h-4 text-primary" />
          <h3 className="text-sm font-semibold text-foreground tracking-tight">Time Saved Trend</h3>
        </div>
        {timeSaved?.trend && timeSaved.trend.length > 0 ? (
          <div className="flex items-end gap-1.5 h-32 relative">
            {timeSaved.trend.map((d, i) => {
              const max = Math.max(...timeSaved.trend.map((t) => t.minutes), 1);
              const h = Math.max((d.minutes / max) * 100, 3);
              return (
                <motion.div
                  key={d.date}
                  initial={{ height: 0 }}
                  animate={{ height: `${h}%` }}
                  transition={{ delay: 0.2 + i * 0.025, type: "spring", stiffness: 150, damping: 15 }}
                  className="flex-1 rounded-t-[3px] bg-primary/30 hover:bg-primary/50 transition-colors cursor-pointer min-w-[4px] group relative"
                >
                  <div className="absolute -top-7 left-1/2 -translate-x-1/2 opacity-0 group-hover:opacity-100 transition-opacity text-[8px] text-foreground bg-card border border-border px-1.5 py-0.5 rounded shadow-lg whitespace-nowrap pointer-events-none">
                    {d.minutes}m
                  </div>
                </motion.div>
              );
            })}
          </div>
        ) : (
          <div className="h-32 flex items-center justify-center text-xs text-muted-foreground/60">No data yet — start coding to see trends</div>
        )}
      </motion.div>

      {/* Peak Hours */}
      <motion.div variants={item} className="relative overflow-hidden rounded-xl border border-border p-5 bg-gradient-to-br from-card to-card/60">
        <div className="absolute top-0 right-0 w-40 h-40 bg-amber-500/[0.02] rounded-full -translate-y-1/2 translate-x-1/3" />
        <div className="flex items-center gap-2 mb-4 relative">
          <Zap className="w-4 h-4 text-primary" />
          <h3 className="text-sm font-semibold text-foreground tracking-tight">Peak Productivity Hours</h3>
        </div>
        {peakHours?.hours && peakHours.hours.length > 0 ? (
          <div className="flex items-end gap-0.5 h-28 relative">
            {peakHours.hours.map((h, i) => {
              const max = Math.max(...peakHours.hours.map((x) => x.productivity), 1);
              const height = Math.max((h.productivity / max) * 100, 2);
              const isBest = h.hour === peakHours.bestHour;
              return (
                <div key={h.hour} className="flex-1 flex flex-col items-center gap-1">
                  <motion.div
                    initial={{ height: 0 }}
                    animate={{ height: `${height}%` }}
                    transition={{ delay: 0.25 + i * 0.02, type: "spring", stiffness: 120, damping: 12 }}
                    className={`w-full rounded-t min-h-[2px] transition-colors ${isBest ? "bg-primary shadow-sm shadow-primary/20" : "bg-primary/20 hover:bg-primary/35"}`}
                  />
                  <span className={`text-[8px] ${isBest ? "text-primary font-bold" : "text-muted-foreground/40"}`}>{h.hour}</span>
                </div>
              );
            })}
          </div>
        ) : (
          <div className="h-28 flex items-center justify-center text-xs text-muted-foreground/60">No productivity data yet</div>
        )}
      </motion.div>

      {/* Deploy Success Rate */}
      {deployRate?.trend && deployRate.trend.length > 0 && (
        <motion.div variants={item} className="relative overflow-hidden rounded-xl border border-border p-5 bg-gradient-to-br from-card to-card/60">
          <div className="absolute top-0 right-0 w-40 h-40 bg-emerald-500/[0.02] rounded-full -translate-y-1/2 translate-x-1/3" />
          <div className="flex items-center gap-2 mb-4 relative">
            <Activity className="w-4 h-4 text-primary" />
            <h3 className="text-sm font-semibold text-foreground tracking-tight">Deployment Success Rate</h3>
          </div>
          <div className="flex items-end gap-1 h-24 relative">
            {deployRate.trend.map((d, i) => {
              const h = Math.max(d.rate, 3);
              return (
                <motion.div
                  key={d.date}
                  initial={{ height: 0 }}
                  animate={{ height: `${h}%` }}
                  transition={{ delay: 0.3 + i * 0.025, type: "spring", stiffness: 120, damping: 12 }}
                  className={`flex-1 rounded-t-[3px] min-w-[4px] transition-colors cursor-pointer group relative ${d.rate >= 90 ? "bg-emerald-500/40 hover:bg-emerald-500/60" : d.rate >= 70 ? "bg-amber-500/40 hover:bg-amber-500/60" : "bg-red-500/40 hover:bg-red-500/60"}`}
                >
                  <div className="absolute -top-7 left-1/2 -translate-x-1/2 opacity-0 group-hover:opacity-100 transition-opacity text-[8px] text-foreground bg-card border border-border px-1.5 py-0.5 rounded shadow-lg whitespace-nowrap pointer-events-none">
                    {d.rate}%
                  </div>
                </motion.div>
              );
            })}
          </div>
        </motion.div>
      )}
    </motion.div>
  );
};

export default InsightsPanel;

