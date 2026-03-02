import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import {
  Crown, TrendingUp, ArrowUpRight,
  BarChart3, Wrench, Bot, Zap, Smile, AlertTriangle, Trophy,
} from "lucide-react";
import { useAuth } from "@/context/AuthContext";
import {
  greetingApi, leaderInsightsApi, leaderAgentsApi, teamsApi,
  GreetingDto, SprintVelocityDto, TeamMoodDto, MilestoneDto,
} from "@/services/api";

interface Props { onTabChange: (tab: string) => void; }

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

const LeaderOverviewPanel = ({ onTabChange }: Props) => {
  const { user } = useAuth();
  const [greeting, setGreeting] = useState<GreetingDto | null>(null);
  const [velocity, setVelocity] = useState<SprintVelocityDto | null>(null);
  const [mood, setMood] = useState<TeamMoodDto | null>(null);
  const [milestone, setMilestone] = useState<MilestoneDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = async () => {
      // Get greeting
      greetingApi.get(user?.userName).then(r => { if (r.data.isSuccess) setGreeting(r.data.data); }).catch(() => {});
      // Get first team
      try {
        const tR = await teamsApi.getMyTeams();
        if (tR.data.isSuccess && tR.data.data?.length > 0) {
          const tid = tR.data.data[0].id;
          setTeamId(tid);
          const [vR, mR, msR] = await Promise.allSettled([
            leaderInsightsApi.sprintVelocity(tid),
            leaderInsightsApi.teamMood(tid),
            leaderAgentsApi.milestone(tid),
          ]);
          if (vR.status === "fulfilled" && vR.value.data.isSuccess) setVelocity(vR.value.data.data);
          if (mR.status === "fulfilled" && mR.value.data.isSuccess) setMood(mR.value.data.data);
          if (msR.status === "fulfilled" && msR.value.data.isSuccess) setMilestone(msR.value.data.data);
        }
      } catch {}
      setLoading(false);
    };
    load();
  }, [user?.userName]);

  const displayName = user?.fullName?.split(" ")[0] || "Leader";
  const greetText = greeting?.greeting || `Welcome back, ${displayName}`;

  const stats = [
    { label: "Sprint Velocity", value: velocity ? `${velocity.averagePerSprint} pts` : "—", sub: velocity ? `${velocity.totalPoints} total points` : "loading...", icon: TrendingUp, color: "text-blue-400", bg: "from-blue-500/15 to-blue-500/3" },
    { label: "Team Mood", value: mood?.overallMood || "—", sub: mood ? `Happiness: ${mood.happinessLevel}%` : "", icon: Smile, color: mood?.overallMood === "Positive" ? "text-emerald-400" : "text-amber-400", bg: mood?.overallMood === "Positive" ? "from-emerald-500/15 to-emerald-500/3" : "from-amber-500/15 to-amber-500/3" },
    { label: "Stress Level", value: mood ? `${mood.stressLevel}%` : "—", sub: mood?.stressLevel && mood.stressLevel > 50 ? "⚠️ High stress" : "Manageable", icon: AlertTriangle, color: mood?.stressLevel && mood.stressLevel > 50 ? "text-red-400" : "text-emerald-400", bg: mood?.stressLevel && mood.stressLevel > 50 ? "from-red-500/15 to-red-500/3" : "from-emerald-500/15 to-emerald-500/3" },
  ];

  const quickActions = [
    { id: "leader-insights", label: "Team Analytics", desc: "Velocity, costs, reviews, mood", icon: BarChart3, gradient: "from-blue-500/12 to-blue-500/3" },
    { id: "leader-utilities", label: "PM Tools", desc: "Timezone, capacity, risk matrix", icon: Wrench, gradient: "from-violet-500/12 to-violet-500/3" },
    { id: "leader-agents", label: "AI Agents", desc: "Bottleneck, burnout, scope creep", icon: Bot, gradient: "from-amber-500/12 to-amber-500/3" },
    { id: "leader-scripts", label: "Automation", desc: "Sprint start, release notes, standup", icon: Zap, gradient: "from-emerald-500/12 to-emerald-500/3" },
  ];

  if (loading) {
    return (
      <div className="flex items-center justify-center py-20">
        <div className="flex flex-col items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-blue-500/10 flex items-center justify-center animate-pulse">
            <Crown className="w-5 h-5 text-blue-400" />
          </div>
          <p className="text-xs text-muted-foreground">Loading leadership dashboard...</p>
        </div>
      </div>
    );
  }

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-8">
      {/* Greeting */}
      <motion.div variants={item}>
        <h1 className="text-2xl font-bold text-foreground tracking-tight">{greetText}</h1>
        <p className="text-sm text-muted-foreground mt-1">Team Command Center — Lead with clarity</p>
      </motion.div>

      {/* Milestone Banner */}
      {milestone?.hasMilestone && (
        <motion.div variants={item} className="p-4 rounded-2xl bg-gradient-to-r from-amber-500/10 via-amber-500/5 to-transparent border border-amber-500/15">
          <div className="flex items-center gap-4">
            <div className="w-12 h-12 rounded-2xl bg-amber-500/15 border border-amber-500/20 flex items-center justify-center">
              <Trophy className="w-5 h-5 text-amber-400" />
            </div>
            <div className="flex-1">
              <p className="text-sm font-bold text-foreground">{milestone.milestoneName}</p>
              <p className="text-xs text-muted-foreground mt-0.5">{milestone.celebrationMessage}</p>
            </div>
          </div>
        </motion.div>
      )}

      {/* Stats */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        {stats.map((s, i) => (
          <motion.div key={i} variants={item}
            className={`p-5 rounded-2xl bg-gradient-to-br ${s.bg} border border-border/15 group hover:shadow-lg transition-all duration-300`}>
            <div className="flex items-center gap-3 mb-3">
              <div className={`w-9 h-9 rounded-xl bg-gradient-to-br ${s.bg} flex items-center justify-center`}>
                <s.icon className={`w-4.5 h-4.5 ${s.color}`} />
              </div>
              <span className="text-xs font-medium text-muted-foreground">{s.label}</span>
            </div>
            <p className="text-2xl font-bold text-foreground tracking-tight">{s.value}</p>
            <p className="text-xs text-muted-foreground mt-1">{s.sub}</p>
          </motion.div>
        ))}
      </div>

      {/* Sprint Velocity Chart */}
      {velocity && velocity.dataPoints.length > 0 && (
        <motion.div variants={item} className="p-5 rounded-2xl bg-card/50 border border-border/30">
          <h3 className="text-sm font-bold text-foreground mb-4 flex items-center gap-2">
            <TrendingUp className="w-4 h-4 text-blue-400" /> Sprint Velocity Trend
          </h3>
          <div className="flex items-end gap-3 h-24">
            {velocity.dataPoints.map((dp, i) => {
              const maxPts = Math.max(...velocity.dataPoints.map(d => d.points));
              return (
                <div key={i} className="flex-1 flex flex-col items-center gap-1">
                  <span className="text-[10px] text-muted-foreground font-medium">{dp.points}</span>
                  <motion.div initial={{ height: 0 }} animate={{ height: `${(dp.points / maxPts) * 100}%` }}
                    transition={{ duration: 0.5, delay: i * 0.08 }}
                    className="w-full bg-gradient-to-t from-blue-500/70 to-blue-400/30 rounded-t-lg min-h-[4px]" />
                  <span className="text-[9px] text-muted-foreground truncate w-full text-center">{dp.sprintName.replace("Sprint ", "S")}</span>
                </div>
              );
            })}
          </div>
        </motion.div>
      )}

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
              <p className="text-xs text-muted-foreground mt-0.5">{a.desc}</p>
            </motion.button>
          ))}
        </div>
      </motion.div>
    </motion.div>
  );
};

export default LeaderOverviewPanel;

