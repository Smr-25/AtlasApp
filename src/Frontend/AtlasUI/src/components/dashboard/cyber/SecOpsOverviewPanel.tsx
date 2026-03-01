import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import {
  Shield, ShieldAlert, ShieldCheck, Clock, Flame,
  Activity, ArrowUpRight, Zap, Eye, Terminal,
  Skull, Lock, Wifi,
} from "lucide-react";
import { useAuth } from "@/context/AuthContext";
import {
  greetingApi, secOpsInsightsApi, GreetingDto,
  ThreatsBlockedDto, SecurityScoreDto, ZeroIncidentDto,
} from "@/services/api";

interface Props { onTabChange: (tab: string) => void; }

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

const SecOpsOverviewPanel = ({ onTabChange }: Props) => {
  const { user } = useAuth();
  const [greeting, setGreeting] = useState<GreetingDto | null>(null);
  const [threats, setThreats] = useState<ThreatsBlockedDto | null>(null);
  const [score, setScore] = useState<SecurityScoreDto | null>(null);
  const [streak, setStreak] = useState<ZeroIncidentDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = async () => {
      const [gR, tR, sR, zR] = await Promise.allSettled([
        greetingApi.get(user?.userName),
        secOpsInsightsApi.threatsBlocked(),
        secOpsInsightsApi.securityScore(),
        secOpsInsightsApi.zeroIncidentStreak(),
      ]);
      if (gR.status === "fulfilled" && gR.value.data.isSuccess) setGreeting(gR.value.data.data);
      if (tR.status === "fulfilled" && tR.value.data.isSuccess) setThreats(tR.value.data.data);
      if (sR.status === "fulfilled" && sR.value.data.isSuccess) setScore(sR.value.data.data);
      if (zR.status === "fulfilled" && zR.value.data.isSuccess) setStreak(zR.value.data.data);
      setLoading(false);
    };
    load();
  }, [user?.userName]);

  const displayName = user?.fullName?.split(" ")[0] || "Operator";
  const greetText = greeting?.greeting || `Welcome back, ${displayName}`;

  const stats = [
    {
      label: "Threats Blocked", value: threats?.totalBlocked?.toLocaleString() || "0",
      sub: `${threats?.ddosBlocked || 0} DDoS · ${threats?.malwareBlocked || 0} Malware`,
      icon: ShieldAlert, color: "text-red-400",
      bg: "from-red-500/15 to-red-500/3", border: "border-red-500/15",
    },
    {
      label: "Security Score", value: score ? `${score.grade}` : "—",
      sub: score ? `${score.score}/100` : "calculating...",
      icon: ShieldCheck, color: "text-emerald-400",
      bg: "from-emerald-500/15 to-emerald-500/3", border: "border-emerald-500/15",
    },
    {
      label: "0-Incident Streak", value: streak ? `${streak.days}d` : "—",
      sub: streak?.lastIncidentDate ? `since ${new Date(streak.lastIncidentDate).toLocaleDateString()}` : "no incidents",
      icon: Flame, color: "text-amber-400",
      bg: "from-amber-500/15 to-amber-500/3", border: "border-amber-500/15",
    },
  ];

  const quickActions = [
    { id: "secops-insights", label: "Threat Intel", desc: "Blocked threats, vulns, response time", icon: Activity, gradient: "from-red-500/12 to-red-500/3" },
    { id: "secops-utilities", label: "Security Tools", desc: "Hash, SSL, port scan, IP lookup", icon: Terminal, gradient: "from-cyan-500/12 to-cyan-500/3" },
    { id: "secops-agents", label: "AI Agents", desc: "Rogue ports, leaked keys, patches", icon: Eye, gradient: "from-violet-500/12 to-violet-500/3" },
    { id: "secops-scripts", label: "Scripts", desc: "Quick scan, panic button, firewall", icon: Zap, gradient: "from-amber-500/12 to-amber-500/3" },
  ];

  if (loading) {
    return (
      <div className="flex items-center justify-center py-20">
        <div className="flex flex-col items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-red-500/10 flex items-center justify-center animate-pulse">
            <Shield className="w-5 h-5 text-red-400" />
          </div>
          <p className="text-xs text-muted-foreground font-mono">Initializing secure dashboard...</p>
        </div>
      </div>
    );
  }

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-8">
      {/* Greeting */}
      <motion.div variants={item}>
        <h1 className="text-2xl font-bold text-foreground tracking-tight">{greetText}</h1>
        <p className="text-sm text-muted-foreground mt-1 font-mono">SecOps Command Center — All systems monitored</p>
      </motion.div>

      {/* Score Banner */}
      {score && (
        <motion.div variants={item} className="p-4 rounded-2xl bg-gradient-to-r from-emerald-500/10 via-emerald-500/5 to-transparent border border-emerald-500/15">
          <div className="flex items-center gap-4">
            <div className="w-14 h-14 rounded-2xl bg-emerald-500/15 border border-emerald-500/20 flex items-center justify-center">
              <span className="text-xl font-black text-emerald-400">{score.grade}</span>
            </div>
            <div className="flex-1">
              <p className="text-sm font-bold text-foreground">Security Score: {score.score}/100</p>
              {score.recommendations.length > 0 && (
                <p className="text-xs text-muted-foreground mt-0.5 font-mono">{score.recommendations[0]}</p>
              )}
            </div>
          </div>
        </motion.div>
      )}

      {/* Stats */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        {stats.map((s, i) => (
          <motion.div key={i} variants={item}
            className={`p-5 rounded-2xl bg-gradient-to-br ${s.bg} border ${s.border} group hover:shadow-lg transition-all duration-300`}>
            <div className="flex items-center gap-3 mb-3">
              <div className={`w-9 h-9 rounded-xl bg-gradient-to-br ${s.bg} flex items-center justify-center`}>
                <s.icon className={`w-4.5 h-4.5 ${s.color}`} />
              </div>
              <span className="text-xs font-medium text-muted-foreground">{s.label}</span>
            </div>
            <p className="text-2xl font-bold text-foreground tracking-tight font-mono">{s.value}</p>
            <p className="text-xs text-muted-foreground mt-1 font-mono">{s.sub}</p>
          </motion.div>
        ))}
      </div>

      {/* Quick Actions */}
      <motion.div variants={item}>
        <h2 className="text-sm font-bold text-foreground mb-4">Operations</h2>
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
          {quickActions.map((a) => (
            <motion.button key={a.id} variants={item} whileHover={{ y: -2, scale: 1.01 }} whileTap={{ scale: 0.99 }}
              onClick={() => onTabChange(a.id)}
              className={`group p-4 rounded-2xl bg-gradient-to-br ${a.gradient} border border-border/30 hover:border-primary/20 text-left transition-all duration-300 hover:shadow-lg`}>
              <div className="flex items-start justify-between mb-3">
                <div className="w-9 h-9 rounded-xl bg-card/60 border border-border/20 flex items-center justify-center">
                  <a.icon className="w-4 h-4 text-primary" />
                </div>
                <ArrowUpRight className="w-4 h-4 text-muted-foreground/30 group-hover:text-primary transition-all" />
              </div>
              <p className="text-sm font-semibold text-foreground">{a.label}</p>
              <p className="text-xs text-muted-foreground mt-0.5 font-mono">{a.desc}</p>
            </motion.button>
          ))}
        </div>
      </motion.div>
    </motion.div>
  );
};

export default SecOpsOverviewPanel;

