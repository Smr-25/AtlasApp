import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import {
  Bot, AlertTriangle, Flame, GitPullRequest, Bug,
  Ghost, Trophy, Users, Loader2, ExternalLink, TrendingUp,
} from "lucide-react";
import {
  leaderAgentsApi, teamsApi,
  BottleneckDto, BurnoutRiskDto, ScopeCreepDto,
  PrReviewNagDto, UnassignedBugsDto, GhostMembersDto, MilestoneDto,
} from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

const LeaderAgentsPanel = () => {
  const [teamId, setTeamId] = useState<string | null>(null);
  const [bottleneck, setBottleneck] = useState<BottleneckDto | null>(null);
  const [burnout, setBurnout] = useState<BurnoutRiskDto | null>(null);
  const [scope, setScope] = useState<ScopeCreepDto | null>(null);
  const [prNag, setPrNag] = useState<PrReviewNagDto | null>(null);
  const [bugs, setBugs] = useState<UnassignedBugsDto | null>(null);
  const [ghosts, setGhosts] = useState<GhostMembersDto | null>(null);
  const [milestone, setMilestone] = useState<MilestoneDto | null>(null);
  const [loading, setLoading] = useState<Record<string, boolean>>({});
  const [sprintId, setSprintId] = useState("sprint-24");
  const [threshold, setThreshold] = useState(24);

  useEffect(() => {
    teamsApi.getMyTeams().then(r => {
      if (r.data.isSuccess && r.data.data?.length > 0) setTeamId(r.data.data[0].id);
    }).catch(() => {});
  }, []);

  const setL = (id: string, v: boolean) => setLoading(p => ({ ...p, [id]: v }));

  const runBottleneck = async () => { if (!teamId) return; setL("bn", true); try { const r = await leaderAgentsApi.bottleneck(teamId); if (r.data.isSuccess) setBottleneck(r.data.data); } catch {} setL("bn", false); };
  const runBurnout = async () => { if (!teamId) return; setL("bo", true); try { const r = await leaderAgentsApi.burnoutRisk(teamId); if (r.data.isSuccess) setBurnout(r.data.data); } catch {} setL("bo", false); };
  const runScope = async () => { if (!teamId) return; setL("sc", true); try { const r = await leaderAgentsApi.scopeCreep(teamId, sprintId); if (r.data.isSuccess) setScope(r.data.data); } catch {} setL("sc", false); };
  const runPrNag = async () => { if (!teamId) return; setL("pr", true); try { const r = await leaderAgentsApi.prReviewNag({ teamId, thresholdHours: threshold }); if (r.data.isSuccess) setPrNag(r.data.data); } catch {} setL("pr", false); };
  const runBugs = async () => { if (!teamId) return; setL("bug", true); try { const r = await leaderAgentsApi.unassignedBugs(teamId); if (r.data.isSuccess) setBugs(r.data.data); } catch {} setL("bug", false); };
  const runGhosts = async () => { if (!teamId) return; setL("gh", true); try { const r = await leaderAgentsApi.ghostMembers({ teamId }); if (r.data.isSuccess) setGhosts(r.data.data); } catch {} setL("gh", false); };
  const runMilestone = async () => { if (!teamId) return; setL("ms", true); try { const r = await leaderAgentsApi.milestone(teamId); if (r.data.isSuccess) setMilestone(r.data.data); } catch {} setL("ms", false); };

  const riskLevelColor = (l: string) => l === "High" ? "text-red-400 bg-red-500/10" : l === "Medium" ? "text-amber-400 bg-amber-500/10" : "text-emerald-400 bg-emerald-500/10";
  const severityColor = (s: string) => s === "Critical" ? "text-red-400" : s === "High" ? "text-amber-400" : "text-muted-foreground";

  const agents = [
    { id: "bn", label: "Bottleneck Detector", desc: "Find stuck tasks & members", icon: AlertTriangle, color: "text-red-400", gradient: "from-red-500/12 to-red-500/3", run: runBottleneck },
    { id: "bo", label: "Burnout Risk", desc: "Monitor overtime & stress", icon: Flame, color: "text-amber-400", gradient: "from-amber-500/12 to-amber-500/3", run: runBurnout },
    { id: "sc", label: "Scope Creep", desc: "Track mid-sprint additions", icon: TrendingUp, color: "text-violet-400", gradient: "from-violet-500/12 to-violet-500/3", run: runScope },
    { id: "pr", label: "PR Review Nag", desc: "Stale PRs waiting for review", icon: GitPullRequest, color: "text-blue-400", gradient: "from-blue-500/12 to-blue-500/3", run: runPrNag },
    { id: "bug", label: "Unassigned Bugs", desc: "Bugs without owners", icon: Bug, color: "text-pink-400", gradient: "from-pink-500/12 to-pink-500/3", run: runBugs },
    { id: "gh", label: "Ghost Members", desc: "Inactive team members", icon: Ghost, color: "text-cyan-400", gradient: "from-cyan-500/12 to-cyan-500/3", run: runGhosts },
    { id: "ms", label: "Milestone Check", desc: "Team achievements", icon: Trophy, color: "text-emerald-400", gradient: "from-emerald-500/12 to-emerald-500/3", run: runMilestone },
  ];

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-amber-500/20 to-amber-500/5 border border-amber-500/10 flex items-center justify-center">
          <Bot className="w-5 h-5 text-amber-400" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">AI Leadership Agents</h2>
          <p className="text-xs text-muted-foreground">Smart team monitoring & risk detection</p>
        </div>
      </motion.div>

      {!teamId && (
        <motion.div variants={item} className="p-4 rounded-xl bg-amber-500/5 border border-amber-500/15 text-xs text-amber-400 flex items-center gap-2">
          <Users className="w-4 h-4" /> No team found. Create a team first to use leadership agents.
        </motion.div>
      )}

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
        {agents.map((a) => (
          <motion.button key={a.id} variants={item} whileHover={{ y: -2 }} whileTap={{ scale: 0.98 }}
            onClick={a.run} disabled={!teamId || loading[a.id]}
            className={`group p-4 rounded-2xl bg-gradient-to-br ${a.gradient} border border-border/20 text-left transition-all hover:border-primary/15 disabled:opacity-50`}>
            <div className="flex items-center gap-3 mb-2">
              {loading[a.id] ? <Loader2 className="w-5 h-5 animate-spin text-muted-foreground" /> : <a.icon className={`w-5 h-5 ${a.color}`} />}
              <span className="text-sm font-semibold text-foreground">{a.label}</span>
            </div>
            <p className="text-xs text-muted-foreground">{a.desc}</p>
          </motion.button>
        ))}
      </div>

      {/* Extra inputs for scope creep & PR nag */}
      <div className="flex gap-3 flex-wrap">
        <div className="flex items-center gap-2">
          <label className="text-[10px] text-muted-foreground">Sprint ID:</label>
          <input value={sprintId} onChange={e => setSprintId(e.target.value)}
            className="h-7 px-2 rounded-lg bg-muted/30 border border-border/30 text-[10px] text-foreground w-28" />
        </div>
        <div className="flex items-center gap-2">
          <label className="text-[10px] text-muted-foreground">PR threshold (h):</label>
          <input type="number" value={threshold} onChange={e => setThreshold(Number(e.target.value))}
            className="h-7 px-2 rounded-lg bg-muted/30 border border-border/30 text-[10px] text-foreground w-16 text-center" />
        </div>
      </div>

      {/* Results */}
      {bottleneck && bottleneck.members.length > 0 && (
        <ResultCard title="Bottleneck Detection" icon={<AlertTriangle className="w-4 h-4 text-red-400" />}>
          {bottleneck.members.map((m, i) => (
            <div key={i} className="p-2 rounded-lg bg-red-500/5 border border-red-500/10 text-xs flex items-start gap-3">
              <span className="font-bold text-foreground">{m.memberName}</span>
              <span className="text-muted-foreground">{m.taskKey} — {m.daysStuck} days stuck</span>
              <p className="text-emerald-400 ml-auto text-[10px]">{m.recommendation}</p>
            </div>
          ))}
        </ResultCard>
      )}

      {burnout && burnout.members.length > 0 && (
        <ResultCard title="Burnout Risk" icon={<Flame className="w-4 h-4 text-amber-400" />}>
          {burnout.members.map((m, i) => (
            <div key={i} className="p-2 rounded-lg bg-muted/10 border border-border/15 text-xs flex items-center gap-3">
              <span className="font-bold text-foreground">{m.memberName}</span>
              <span className="text-muted-foreground">{m.overtimeHours}h overtime, {m.lateNightCommits} late commits</span>
              <span className={`ml-auto px-2 py-0.5 rounded-full text-[10px] font-bold ${riskLevelColor(m.riskLevel)}`}>{m.riskLevel}</span>
            </div>
          ))}
        </ResultCard>
      )}

      {scope && (
        <ResultCard title="Scope Creep Analysis" icon={<TrendingUp className="w-4 h-4 text-violet-400" />}>
          <div className="flex items-center gap-4 text-xs">
            <div className="text-center"><p className="text-lg font-bold text-foreground">{scope.originalTaskCount}</p><p className="text-[10px] text-muted-foreground">Original</p></div>
            <span className="text-muted-foreground">→</span>
            <div className="text-center"><p className="text-lg font-bold text-foreground">{scope.currentTaskCount}</p><p className="text-[10px] text-muted-foreground">Current</p></div>
            <div className="text-center"><p className="text-lg font-bold text-red-400">+{scope.addedMidSprint}</p><p className="text-[10px] text-muted-foreground">Added</p></div>
            <span className={`ml-auto text-sm font-bold ${scope.creepPercentage > 30 ? "text-red-400" : "text-amber-400"}`}>{scope.creepPercentage.toFixed(1)}%</span>
          </div>
          {scope.warning && <p className="text-xs text-amber-400 mt-2">{scope.warning}</p>}
        </ResultCard>
      )}

      {prNag && prNag.stalePrs.length > 0 && (
        <ResultCard title={`Stale PRs (${prNag.totalStale})`} icon={<GitPullRequest className="w-4 h-4 text-blue-400" />}>
          {prNag.stalePrs.map((p, i) => (
            <div key={i} className="p-2 rounded-lg bg-blue-500/5 border border-blue-500/10 text-xs flex items-center gap-3">
              <span className="text-foreground font-bold truncate flex-1">{p.prTitle}</span>
              <span className="text-muted-foreground">{p.author}</span>
              <span className="text-amber-400 font-bold">{p.hoursPending}h</span>
              {p.url && <a href={p.url} target="_blank" rel="noopener" className="text-blue-400 hover:text-blue-300"><ExternalLink className="w-3 h-3" /></a>}
            </div>
          ))}
        </ResultCard>
      )}

      {bugs && bugs.bugs.length > 0 && (
        <ResultCard title={`Unassigned Bugs (${bugs.totalUnassigned})`} icon={<Bug className="w-4 h-4 text-pink-400" />}>
          {bugs.bugs.map((b, i) => (
            <div key={i} className="p-2 rounded-lg bg-pink-500/5 border border-pink-500/10 text-xs flex items-center gap-3">
              <span className="text-muted-foreground">{b.issueKey}</span>
              <span className="text-foreground flex-1">{b.title}</span>
              <span className={`font-bold ${severityColor(b.severity)}`}>{b.severity}</span>
            </div>
          ))}
        </ResultCard>
      )}

      {ghosts && ghosts.ghostMembers.length > 0 && (
        <ResultCard title="Ghost Members" icon={<Ghost className="w-4 h-4 text-cyan-400" />}>
          {ghosts.ghostMembers.map((g, i) => (
            <div key={i} className="p-2 rounded-lg bg-cyan-500/5 border border-cyan-500/10 text-xs flex items-center gap-3">
              <span className="font-bold text-foreground">{g.memberName}</span>
              <span className="text-muted-foreground">{g.hoursInactive}h inactive</span>
              <span className="text-[10px] text-muted-foreground/60 ml-auto">Last: {new Date(g.lastActiveAt).toLocaleDateString()}</span>
            </div>
          ))}
        </ResultCard>
      )}

      {milestone && milestone.hasMilestone && (
        <ResultCard title="Milestone" icon={<Trophy className="w-4 h-4 text-emerald-400" />}>
          <div className="text-center p-3">
            <p className="text-lg font-bold text-foreground">{milestone.milestoneName}</p>
            <p className="text-xs text-muted-foreground mt-1">{milestone.celebrationMessage}</p>
          </div>
        </ResultCard>
      )}
    </motion.div>
  );
};

const ResultCard = ({ title, icon, children }: { title: string; icon: React.ReactNode; children: React.ReactNode }) => (
  <motion.div initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} className="p-4 rounded-2xl bg-card/50 border border-border/30">
    <h3 className="text-xs font-bold text-foreground mb-3 flex items-center gap-2">{icon} {title}</h3>
    <div className="space-y-1.5">{children}</div>
  </motion.div>
);

export default LeaderAgentsPanel;

