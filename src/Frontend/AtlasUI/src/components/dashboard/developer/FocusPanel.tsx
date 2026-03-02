import { useState, useEffect, useRef, useCallback } from "react";
import { motion } from "framer-motion";
import { Timer, Play, Pause, Square, SkipForward, Flame, Target, Clock, Loader2, Coffee, History } from "lucide-react";
import { focusApi, FocusSessionDto, FocusStatsDto } from "@/services/api";

const FocusPanel = () => {
  const [stats, setStats] = useState<FocusStatsDto | null>(null);
  const [activeSession, setActiveSession] = useState<FocusSessionDto | null>(null);
  const [history, setHistory] = useState<FocusSessionDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [actionLoading, setActionLoading] = useState(false);
  const [task, setTask] = useState("");
  const [duration, setDuration] = useState(25);
  const [elapsed, setElapsed] = useState(0);
  const [isPaused, setIsPaused] = useState(false);
  const timerRef = useRef<NodeJS.Timeout | null>(null);

  const fetchData = useCallback(async () => {
    const [sRes, aRes, hRes] = await Promise.allSettled([
      focusApi.getStats(),
      focusApi.getActive(),
      focusApi.history(7),
    ]);
    if (sRes.status === "fulfilled" && sRes.value.data.isSuccess) setStats(sRes.value.data.data);
    if (aRes.status === "fulfilled" && aRes.value.data.isSuccess && aRes.value.data.data) {
      const s = aRes.value.data.data;
      setActiveSession(s);
      const start = new Date(s.startedAt).getTime();
      setElapsed(Math.floor((Date.now() - start) / 1000));
      setIsPaused(s.status === "Paused");
    }
    if (hRes.status === "fulfilled" && hRes.value.data.isSuccess && Array.isArray(hRes.value.data.data)) setHistory(hRes.value.data.data);
    setLoading(false);
  }, []);

  useEffect(() => { fetchData(); }, [fetchData]);

  // Timer tick
  useEffect(() => {
    if (activeSession && !isPaused) {
      timerRef.current = setInterval(() => setElapsed((e) => e + 1), 1000);
    }
    return () => { if (timerRef.current) clearInterval(timerRef.current); };
  }, [activeSession, isPaused]);

  const totalSeconds = (activeSession?.duration || duration) * 60;
  const remaining = Math.max(totalSeconds - elapsed, 0);
  const progress = totalSeconds > 0 ? Math.min(elapsed / totalSeconds, 1) : 0;
  const minutes = Math.floor(remaining / 60);
  const seconds = remaining % 60;

  const handleStart = async () => {
    if (!task.trim()) return;
    setActionLoading(true);
    try {
      const res = await focusApi.start({ durationMinutes: duration, taskDescription: task, sessionType: "DeepWork" });
      if (res.data.isSuccess && res.data.data) {
        setActiveSession(res.data.data);
        setElapsed(0);
        setIsPaused(false);
      }
    } catch {}
    setActionLoading(false);
  };

  const handlePause = async () => {
    if (!activeSession) return;
    setActionLoading(true);
    try { await focusApi.pause(activeSession.id); setIsPaused(true); } catch {}
    setActionLoading(false);
  };

  const handleResume = async () => {
    if (!activeSession) return;
    setActionLoading(true);
    try { await focusApi.resume(activeSession.id); setIsPaused(false); } catch {}
    setActionLoading(false);
  };

  const handleComplete = async () => {
    if (!activeSession) return;
    setActionLoading(true);
    try {
      await focusApi.complete(activeSession.id);
      setActiveSession(null);
      setElapsed(0);
      await fetchData();
    } catch {}
    setActionLoading(false);
  };

  const handleInterrupt = async () => {
    if (!activeSession) return;
    setActionLoading(true);
    try {
      await focusApi.interrupt(activeSession.id);
      setActiveSession(null);
      setElapsed(0);
      await fetchData();
    } catch {}
    setActionLoading(false);
  };

  if (loading) return <div className="py-20 flex justify-center"><Loader2 className="w-6 h-6 animate-spin text-primary" /></div>;

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-emerald-500/20 to-emerald-500/5 border border-emerald-500/10 flex items-center justify-center">
          <Timer className="w-5 h-5 text-emerald-400" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">Focus Sessions</h2>
          <p className="text-xs text-muted-foreground">Deep work with pomodoro technique</p>
        </div>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-3">
        {[
          { label: "Today", value: `${stats?.todaySessions || 0} sessions`, icon: Target, gradient: "from-blue-500/15 to-cyan-500/5", iconColor: "text-blue-400" },
          { label: "Today Focus", value: `${stats?.todayMinutes || 0}m`, icon: Clock, gradient: "from-emerald-500/15 to-green-500/5", iconColor: "text-emerald-400" },
          { label: "Avg Duration", value: `${stats?.averageDuration || 0}m`, icon: Coffee, gradient: "from-amber-500/15 to-orange-500/5", iconColor: "text-amber-400" },
          { label: "Streak", value: `${stats?.streak || 0} days`, icon: Flame, gradient: "from-orange-500/15 to-red-500/5", iconColor: "text-orange-400" },
        ].map((s) => (
          <motion.div
            key={s.label}
            whileHover={{ y: -3, transition: { duration: 0.2 } }}
            className={`relative overflow-hidden rounded-xl border border-border bg-gradient-to-br ${s.gradient} p-4 group`}
          >
            <div className="absolute top-0 right-0 w-16 h-16 bg-primary/[0.03] rounded-full -translate-y-1/2 translate-x-1/3 group-hover:bg-primary/[0.05] transition-colors duration-500" />
            <s.icon className={`w-4 h-4 ${s.iconColor} mb-3`} />
            <p className="text-xl font-bold text-foreground tracking-tight">{s.value}</p>
            <p className="text-[10px] text-muted-foreground mt-0.5">{s.label}</p>
          </motion.div>
        ))}
      </div>

      {/* Timer */}
      <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} className="relative overflow-hidden rounded-2xl border border-border p-8 text-center bg-gradient-to-br from-card to-card/60">
        <div className="absolute top-0 right-0 w-64 h-64 bg-primary/[0.02] rounded-full -translate-y-1/2 translate-x-1/3" />
        {/* Circular Progress */}
        <div className="relative w-48 h-48 mx-auto mb-6">
          <svg className="w-full h-full -rotate-90" viewBox="0 0 100 100">
            <circle cx="50" cy="50" r="45" fill="none" stroke="hsl(var(--muted))" strokeWidth="4" />
            <motion.circle
              cx="50" cy="50" r="45" fill="none" stroke="hsl(var(--primary))" strokeWidth="4" strokeLinecap="round"
              strokeDasharray={`${2 * Math.PI * 45}`}
              strokeDashoffset={`${2 * Math.PI * 45 * (1 - progress)}`}
              transition={{ duration: 0.5 }}
            />
          </svg>
          <div className="absolute inset-0 flex flex-col items-center justify-center">
            <span className="text-4xl font-bold text-foreground font-mono">{String(minutes).padStart(2, "0")}:{String(seconds).padStart(2, "0")}</span>
            {activeSession && <span className="text-xs text-muted-foreground mt-1 truncate max-w-[120px]">{activeSession.task}</span>}
          </div>
        </div>

        {!activeSession ? (
          /* Start Form */
          <div className="max-w-sm mx-auto space-y-3">
            <input value={task} onChange={(e) => setTask(e.target.value)} placeholder="What are you working on?" className="w-full h-10 px-3.5 rounded-lg bg-muted/40 border border-border text-sm text-center text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/30 transition-all" />
            <div className="flex items-center justify-center gap-2">
              {[15, 25, 45, 60].map((d) => (
                <button key={d} onClick={() => setDuration(d)} className={`px-3 py-1.5 rounded-lg text-xs font-medium transition-all ${duration === d ? "bg-primary/10 text-primary" : "text-muted-foreground hover:bg-muted"}`}>{d}m</button>
              ))}
            </div>
            <motion.button whileTap={{ scale: 0.98 }} onClick={handleStart} disabled={actionLoading || !task.trim()} className="w-full h-11 rounded-xl bg-primary text-primary-foreground text-sm font-medium shadow-lg shadow-primary/25 disabled:opacity-50 flex items-center justify-center gap-2">
              {actionLoading ? <Loader2 className="w-4 h-4 animate-spin" /> : <Play className="w-4 h-4" />} Start Focus
            </motion.button>
          </div>
        ) : (
          /* Session Controls */
          <div className="flex items-center justify-center gap-3">
            {isPaused ? (
              <motion.button whileTap={{ scale: 0.95 }} onClick={handleResume} disabled={actionLoading} className="w-12 h-12 rounded-xl bg-primary text-primary-foreground flex items-center justify-center shadow-lg shadow-primary/25"><Play className="w-5 h-5" /></motion.button>
            ) : (
              <motion.button whileTap={{ scale: 0.95 }} onClick={handlePause} disabled={actionLoading} className="w-12 h-12 rounded-xl bg-amber-500 text-white flex items-center justify-center shadow-lg shadow-amber-500/25"><Pause className="w-5 h-5" /></motion.button>
            )}
            <motion.button whileTap={{ scale: 0.95 }} onClick={handleComplete} disabled={actionLoading} className="w-12 h-12 rounded-xl bg-emerald-500 text-white flex items-center justify-center shadow-lg shadow-emerald-500/25"><Square className="w-5 h-5" /></motion.button>
            <motion.button whileTap={{ scale: 0.95 }} onClick={handleInterrupt} disabled={actionLoading} className="w-12 h-12 rounded-xl bg-red-500 text-white flex items-center justify-center shadow-lg shadow-red-500/25"><SkipForward className="w-5 h-5" /></motion.button>
          </div>
        )}
      </motion.div>

      {/* History */}
      {history.length > 0 && (
        <div>
          <h3 className="text-sm font-semibold text-foreground flex items-center gap-2 mb-3"><History className="w-4 h-4 text-primary" /> Recent Sessions</h3>
          <div className="space-y-1.5">
            {history.slice(0, 10).map((s) => (
              <div key={s.id} className="flex items-center gap-3 p-3 bg-card rounded-lg border border-border">
                <div className={`w-2 h-2 rounded-full shrink-0 ${s.status === "Completed" ? "bg-emerald-500" : s.status === "Interrupted" ? "bg-red-500" : "bg-amber-500"}`} />
                <span className="text-sm text-foreground flex-1 truncate">{s.task}</span>
                <span className="text-xs text-muted-foreground">{s.duration}m</span>
                <span className="text-[10px] text-muted-foreground/60">{new Date(s.startedAt).toLocaleDateString()}</span>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
};

export default FocusPanel;

