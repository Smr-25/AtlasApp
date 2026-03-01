import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import { BarChart3, Clock, Flame, TrendingUp, Zap, Activity, Loader2 } from "lucide-react";
import { devInsightsApi, TimeSavedDto, PeakHoursDto, DeploySuccessDto } from "@/services/api";

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
        <Loader2 className="w-6 h-6 animate-spin text-primary" />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-lg font-bold text-foreground flex items-center gap-2">
          <BarChart3 className="w-5 h-5 text-primary" /> Developer Insights
        </h2>
        <p className="text-sm text-muted-foreground">Track your productivity metrics and trends</p>
      </div>

      {/* Summary Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
        <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} className="bg-card rounded-xl border border-border p-4">
          <div className="flex items-center gap-2 mb-2">
            <div className="w-8 h-8 rounded-lg bg-blue-500/10 flex items-center justify-center"><Clock className="w-4 h-4 text-blue-400" /></div>
            <span className="text-xs text-muted-foreground">Time Saved by AI</span>
          </div>
          <p className="text-2xl font-bold text-foreground">{timeSaved ? `${Math.round(timeSaved.totalMinutes)}m` : "—"}</p>
        </motion.div>
        <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.05 }} className="bg-card rounded-xl border border-border p-4">
          <div className="flex items-center gap-2 mb-2">
            <div className="w-8 h-8 rounded-lg bg-emerald-500/10 flex items-center justify-center"><TrendingUp className="w-4 h-4 text-emerald-400" /></div>
            <span className="text-xs text-muted-foreground">Deploy Success Rate</span>
          </div>
          <p className="text-2xl font-bold text-foreground">{deployRate ? `${Math.round(deployRate.rate)}%` : "—"}</p>
        </motion.div>
        <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.1 }} className="bg-card rounded-xl border border-border p-4">
          <div className="flex items-center gap-2 mb-2">
            <div className="w-8 h-8 rounded-lg bg-orange-500/10 flex items-center justify-center"><Flame className="w-4 h-4 text-orange-400" /></div>
            <span className="text-xs text-muted-foreground">Peak Productivity Hour</span>
          </div>
          <p className="text-2xl font-bold text-foreground">{peakHours ? `${peakHours.bestHour}:00` : "—"}</p>
        </motion.div>
      </div>

      {/* Time Saved Chart */}
      <motion.div initial={{ opacity: 0, y: 15 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.15 }} className="bg-card rounded-xl border border-border p-5">
        <div className="flex items-center gap-2 mb-4">
          <Clock className="w-4 h-4 text-primary" />
          <h3 className="text-sm font-semibold text-foreground">Time Saved Trend</h3>
        </div>
        {timeSaved?.trend && timeSaved.trend.length > 0 ? (
          <div className="flex items-end gap-1 h-32">
            {timeSaved.trend.map((d, i) => {
              const max = Math.max(...timeSaved.trend.map((t) => t.minutes), 1);
              const h = Math.max((d.minutes / max) * 100, 3);
              return (
                <motion.div
                  key={d.date}
                  initial={{ height: 0 }}
                  animate={{ height: `${h}%` }}
                  transition={{ delay: 0.2 + i * 0.02 }}
                  className="flex-1 rounded-t bg-primary/40 hover:bg-primary/60 transition-colors cursor-pointer min-w-[4px] group relative"
                >
                  <div className="absolute -top-6 left-1/2 -translate-x-1/2 opacity-0 group-hover:opacity-100 transition-opacity text-[9px] text-foreground bg-card border border-border px-1.5 py-0.5 rounded whitespace-nowrap shadow-lg">
                    {d.minutes}m
                  </div>
                </motion.div>
              );
            })}
          </div>
        ) : (
          <div className="h-32 flex items-center justify-center text-xs text-muted-foreground">No data yet</div>
        )}
      </motion.div>

      {/* Peak Hours */}
      <motion.div initial={{ opacity: 0, y: 15 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.2 }} className="bg-card rounded-xl border border-border p-5">
        <div className="flex items-center gap-2 mb-4">
          <Zap className="w-4 h-4 text-primary" />
          <h3 className="text-sm font-semibold text-foreground">Peak Productivity Hours</h3>
        </div>
        {peakHours?.hours && peakHours.hours.length > 0 ? (
          <div className="flex items-end gap-0.5 h-28">
            {peakHours.hours.map((h, i) => {
              const max = Math.max(...peakHours.hours.map((x) => x.productivity), 1);
              const height = Math.max((h.productivity / max) * 100, 2);
              const isBest = h.hour === peakHours.bestHour;
              return (
                <div key={h.hour} className="flex-1 flex flex-col items-center gap-1">
                  <motion.div
                    initial={{ height: 0 }}
                    animate={{ height: `${height}%` }}
                    transition={{ delay: 0.25 + i * 0.02 }}
                    className={`w-full rounded-t min-h-[2px] transition-colors ${isBest ? "bg-primary" : "bg-primary/25 hover:bg-primary/40"}`}
                  />
                  <span className={`text-[8px] ${isBest ? "text-primary font-bold" : "text-muted-foreground/50"}`}>{h.hour}</span>
                </div>
              );
            })}
          </div>
        ) : (
          <div className="h-28 flex items-center justify-center text-xs text-muted-foreground">No productivity data yet</div>
        )}
      </motion.div>

      {/* Deploy Success Rate */}
      {deployRate?.trend && deployRate.trend.length > 0 && (
        <motion.div initial={{ opacity: 0, y: 15 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.25 }} className="bg-card rounded-xl border border-border p-5">
          <div className="flex items-center gap-2 mb-4">
            <Activity className="w-4 h-4 text-primary" />
            <h3 className="text-sm font-semibold text-foreground">Deployment Success Rate</h3>
          </div>
          <div className="flex items-end gap-1 h-24">
            {deployRate.trend.map((d, i) => {
              const h = Math.max(d.rate, 3);
              return (
                <motion.div
                  key={d.date}
                  initial={{ height: 0 }}
                  animate={{ height: `${h}%` }}
                  transition={{ delay: 0.3 + i * 0.02 }}
                  className={`flex-1 rounded-t min-w-[4px] transition-colors ${d.rate >= 90 ? "bg-emerald-500/50 hover:bg-emerald-500/70" : d.rate >= 70 ? "bg-amber-500/50 hover:bg-amber-500/70" : "bg-red-500/50 hover:bg-red-500/70"}`}
                  title={`${d.date}: ${d.rate}%`}
                />
              );
            })}
          </div>
        </motion.div>
      )}
    </div>
  );
};

export default InsightsPanel;

