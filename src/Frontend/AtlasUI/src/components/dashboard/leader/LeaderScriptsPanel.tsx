import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import {
  Zap, Play, AlertTriangle, FileText, BellOff,
  BarChart3, UserMinus, MessageSquare, Loader2, Terminal, Users,
} from "lucide-react";
import {
  leaderScriptsApi, teamsApi,
  SprintStarterDto, BlockedTaskBlasterDto,
  WeekSummaryDto, BulkReassignDto,
} from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

type ActiveScript = "sprint" | "blocked" | "release" | "meeting" | "summary" | "reassign" | "standup" | null;

const LeaderScriptsPanel = () => {
  const [activeScript, setActiveScript] = useState<ActiveScript>(null);
  const [teamId, setTeamId] = useState<string | null>(null);

  useEffect(() => {
    teamsApi.getMyTeams().then(r => {
      if (r.data.isSuccess && r.data.data?.length > 0) setTeamId(r.data.data[0].id);
    }).catch(() => {});
  }, []);

  const scripts = [
    { id: "sprint" as const, label: "Sprint Starter", desc: "Initialize a new sprint", icon: Play, color: "text-emerald-400", gradient: "from-emerald-500/12 to-emerald-500/3" },
    { id: "blocked" as const, label: "Blocked Task Blaster", desc: "Resolve blocked tasks", icon: AlertTriangle, color: "text-red-400", gradient: "from-red-500/12 to-red-500/3" },
    { id: "release" as const, label: "Release Notes", desc: "Generate release notes", icon: FileText, color: "text-blue-400", gradient: "from-blue-500/12 to-blue-500/3" },
    { id: "meeting" as const, label: "Meeting Mode", desc: "Mute all notifications", icon: BellOff, color: "text-violet-400", gradient: "from-violet-500/12 to-violet-500/3" },
    { id: "summary" as const, label: "Week Summary", desc: "Team weekly report", icon: BarChart3, color: "text-amber-400", gradient: "from-amber-500/12 to-amber-500/3" },
    { id: "reassign" as const, label: "Bulk Reassign", desc: "Reassign absent member tasks", icon: UserMinus, color: "text-cyan-400", gradient: "from-cyan-500/12 to-cyan-500/3" },
    { id: "standup" as const, label: "Standup Ping", desc: "Send standup reminder", icon: MessageSquare, color: "text-pink-400", gradient: "from-pink-500/12 to-pink-500/3" },
  ];

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-emerald-500/20 to-emerald-500/5 border border-emerald-500/10 flex items-center justify-center">
          <Zap className="w-5 h-5 text-emerald-400" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">Leadership Automation</h2>
          <p className="text-xs text-muted-foreground">Sprint management & team operations</p>
        </div>
      </motion.div>

      {!teamId && (
        <motion.div variants={item} className="p-3 rounded-xl bg-amber-500/5 border border-amber-500/15 text-xs text-amber-400 flex items-center gap-2">
          <Users className="w-4 h-4" /> No team found. Create a team first.
        </motion.div>
      )}

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
        {scripts.map((s) => (
          <motion.button key={s.id} variants={item} whileHover={{ y: -2 }} whileTap={{ scale: 0.98 }}
            onClick={() => setActiveScript(activeScript === s.id ? null : s.id)}
            className={`group p-4 rounded-2xl bg-gradient-to-br ${s.gradient} border text-left transition-all duration-200 ${activeScript === s.id ? "border-primary/30 shadow-lg" : "border-border/20 hover:border-primary/15"}`}>
            <div className="flex items-center gap-3 mb-2">
              <s.icon className={`w-5 h-5 ${s.color}`} />
              <span className="text-sm font-semibold text-foreground">{s.label}</span>
            </div>
            <p className="text-xs text-muted-foreground">{s.desc}</p>
          </motion.button>
        ))}
      </div>

      {activeScript === "sprint" && <SprintStarterForm teamId={teamId} />}
      {activeScript === "blocked" && <BlockedBlasterForm teamId={teamId} />}
      {activeScript === "release" && <ReleaseNotesForm />}
      {activeScript === "meeting" && <MeetingModeForm />}
      {activeScript === "summary" && <WeekSummaryForm teamId={teamId} />}
      {activeScript === "reassign" && <BulkReassignForm teamId={teamId} />}
      {activeScript === "standup" && <StandupPingForm teamId={teamId} />}
    </motion.div>
  );
};

// ─── Shared UI ─────────────────────────────────────────────────
const ScriptCard = ({ title, children, output }: { title: string; children: React.ReactNode; output?: string }) => (
  <motion.div initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} className="p-5 rounded-2xl bg-card/50 border border-border/30 space-y-3">
    <h3 className="text-sm font-bold text-foreground flex items-center gap-2"><Terminal className="w-4 h-4 text-primary" />{title}</h3>
    {children}
    {output && <pre className="p-3 rounded-xl bg-muted/15 border border-border/15 text-xs text-foreground whitespace-pre-wrap max-h-48 overflow-y-auto">{output}</pre>}
  </motion.div>
);
const RunBtn = ({ loading, onClick, label, danger }: { loading: boolean; onClick: () => void; label?: string; danger?: boolean }) => (
  <button onClick={onClick} disabled={loading}
    className={`h-9 px-4 rounded-xl text-xs font-semibold disabled:opacity-50 transition-colors ${danger ? "bg-red-500 text-white hover:bg-red-600" : "bg-primary text-primary-foreground hover:bg-primary/90"}`}>
    {loading ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : label || "Execute"}
  </button>
);

// ─── Sprint Starter ────────────────────────────────────────────
const SprintStarterForm = ({ teamId }: { teamId: string | null }) => {
  const [name, setName] = useState("Sprint 25");
  const [tasks, setTasks] = useState("Setup CI/CD, Auth refactor, Dashboard UI");
  const [result, setResult] = useState<SprintStarterDto | null>(null);
  const [loading, setLoading] = useState(false);
  const run = async () => {
    if (!teamId || !name.trim()) return;
    setLoading(true);
    try { const r = await leaderScriptsApi.sprintStarter({ sprintName: name, initialTasks: tasks.split(",").map(s => s.trim()).filter(Boolean), teamId }); if (r.data.isSuccess) setResult(r.data.data); } catch {}
    setLoading(false);
  };
  return (
    <ScriptCard title="Sprint Starter" output={result?.slackNotification}>
      <div className="space-y-2">
        <div className="space-y-1">
          <label className="text-[10px] text-muted-foreground">Sprint Name</label>
          <input value={name} onChange={e => setName(e.target.value)} className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground" />
        </div>
        <div className="space-y-1">
          <label className="text-[10px] text-muted-foreground">Initial Tasks (comma separated)</label>
          <input value={tasks} onChange={e => setTasks(e.target.value)} className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground" />
        </div>
        <RunBtn loading={loading} onClick={run} label="Start Sprint 🚀" />
      </div>
      {result && !result.slackNotification && (
        <div className="text-xs text-emerald-400 mt-2">✓ {result.sprintName} started — {result.tasksCreated} tasks created</div>
      )}
    </ScriptCard>
  );
};

// ─── Blocked Task Blaster ──────────────────────────────────────
const BlockedBlasterForm = ({ teamId }: { teamId: string | null }) => {
  const [result, setResult] = useState<BlockedTaskBlasterDto | null>(null);
  const [loading, setLoading] = useState(false);
  const run = async () => { if (!teamId) return; setLoading(true); try { const r = await leaderScriptsApi.blockedTaskBlaster({ teamId }); if (r.data.isSuccess) setResult(r.data.data); } catch {} setLoading(false); };
  return (
    <ScriptCard title="Blocked Task Blaster">
      <RunBtn loading={loading} onClick={run} label="Blast Blockers" danger />
      {result && (
        <div className="mt-2 space-y-1.5">
          <p className="text-xs text-foreground">{result.blockedTasksFound} blocked tasks found, {result.messagesSent} messages sent</p>
          {result.tasks.map((t, i) => (
            <div key={i} className="p-2 rounded-lg bg-red-500/5 border border-red-500/10 text-xs flex items-center gap-3">
              <span className="text-muted-foreground">{t.taskKey}</span>
              <span className="text-foreground flex-1">{t.summary}</span>
              <span className="text-muted-foreground">{t.assignee}</span>
              <span className="text-red-400 font-bold">{t.daysBlocked}d</span>
            </div>
          ))}
        </div>
      )}
    </ScriptCard>
  );
};

// ─── Release Notes ─────────────────────────────────────────────
const ReleaseNotesForm = () => {
  const [repo, setRepo] = useState("ecommerce-backend");
  const [from, setFrom] = useState("v2.4.0");
  const [to, setTo] = useState("v2.5.0");
  const [result, setResult] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const run = async () => { setLoading(true); try { const r = await leaderScriptsApi.releaseNotes({ repoName: repo, fromTag: from, toTag: to }); if (r.data.isSuccess) setResult(r.data.data.notes); } catch {} setLoading(false); };
  return (
    <ScriptCard title="Release Notes Generator" output={result || undefined}>
      <div className="grid grid-cols-3 gap-2">
        <div className="space-y-1"><label className="text-[10px] text-muted-foreground">Repo</label><input value={repo} onChange={e => setRepo(e.target.value)} className="w-full h-8 px-2 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground" /></div>
        <div className="space-y-1"><label className="text-[10px] text-muted-foreground">From Tag</label><input value={from} onChange={e => setFrom(e.target.value)} className="w-full h-8 px-2 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground" /></div>
        <div className="space-y-1"><label className="text-[10px] text-muted-foreground">To Tag</label><input value={to} onChange={e => setTo(e.target.value)} className="w-full h-8 px-2 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground" /></div>
      </div>
      <RunBtn loading={loading} onClick={run} label="Generate" />
    </ScriptCard>
  );
};

// ─── Meeting Mode ──────────────────────────────────────────────
const MeetingModeForm = () => {
  const [duration, setDuration] = useState(60);
  const [output, setOutput] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const run = async () => { setLoading(true); try { const r = await leaderScriptsApi.meetingMode({ durationMinutes: duration }); if (r.data.isSuccess) setOutput(r.data.data.output); } catch {} setLoading(false); };
  return (
    <ScriptCard title="Meeting Mode" output={output || undefined}>
      <div className="flex items-end gap-3">
        <div className="space-y-1">
          <label className="text-[10px] text-muted-foreground">Duration (minutes)</label>
          <input type="number" value={duration} onChange={e => setDuration(Number(e.target.value))} className="w-24 h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground text-center" />
        </div>
        <RunBtn loading={loading} onClick={run} label="🔇 Activate" />
      </div>
    </ScriptCard>
  );
};

// ─── Week Summary ──────────────────────────────────────────────
const WeekSummaryForm = ({ teamId }: { teamId: string | null }) => {
  const [result, setResult] = useState<WeekSummaryDto | null>(null);
  const [loading, setLoading] = useState(false);
  const run = async () => { if (!teamId) return; setLoading(true); try { const r = await leaderScriptsApi.weekSummary({ teamId }); if (r.data.isSuccess) setResult(r.data.data); } catch {} setLoading(false); };
  return (
    <ScriptCard title="Week Summary" output={result?.summaryMarkdown}>
      <RunBtn loading={loading} onClick={run} label="Generate Summary" />
      {result && (
        <div className="mt-2 flex items-center gap-4 p-3 rounded-xl bg-muted/15 border border-border/15">
          <div className="text-center"><p className="text-lg font-bold text-foreground">{result.tasksCompleted}</p><p className="text-[10px] text-muted-foreground">Tasks</p></div>
          <div className="h-8 w-px bg-border/30" />
          <div className="text-center"><p className="text-lg font-bold text-foreground">{result.bugsFixed}</p><p className="text-[10px] text-muted-foreground">Bugs</p></div>
          <div className="h-8 w-px bg-border/30" />
          <div className="text-center"><p className="text-lg font-bold text-foreground">{result.prsMerged}</p><p className="text-[10px] text-muted-foreground">PRs</p></div>
          <div className="h-8 w-px bg-border/30" />
          <div className="text-center"><p className="text-lg font-bold text-blue-400">{result.velocityPoints}</p><p className="text-[10px] text-muted-foreground">Velocity</p></div>
        </div>
      )}
    </ScriptCard>
  );
};

// ─── Bulk Reassign ─────────────────────────────────────────────
const BulkReassignForm = ({ teamId }: { teamId: string | null }) => {
  const [memberId, setMemberId] = useState("");
  const [result, setResult] = useState<BulkReassignDto | null>(null);
  const [loading, setLoading] = useState(false);
  const run = async () => { if (!teamId || !memberId.trim()) return; setLoading(true); try { const r = await leaderScriptsApi.bulkReassign({ absentMemberId: memberId, teamId }); if (r.data.isSuccess) setResult(r.data.data); } catch {} setLoading(false); };
  return (
    <ScriptCard title="Bulk Task Reassignment">
      <div className="flex items-end gap-3">
        <div className="flex-1 space-y-1">
          <label className="text-[10px] text-muted-foreground">Absent Member ID</label>
          <input value={memberId} onChange={e => setMemberId(e.target.value)} placeholder="member-uuid" className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground placeholder:text-muted-foreground/40" />
        </div>
        <RunBtn loading={loading} onClick={run} label="Reassign" />
      </div>
      {result && (
        <div className="mt-2 space-y-1">
          <p className="text-xs text-foreground">{result.tasksReassigned} tasks reassigned</p>
          {result.tasks.map((t, i) => (
            <div key={i} className="p-2 rounded-lg bg-muted/10 border border-border/15 text-xs flex items-center gap-3">
              <span className="text-muted-foreground">{t.taskKey}</span>
              <span className="text-red-400 line-through">{t.fromUser}</span>
              <span className="text-muted-foreground">→</span>
              <span className="text-emerald-400 font-bold">{t.toUser}</span>
            </div>
          ))}
        </div>
      )}
    </ScriptCard>
  );
};

// ─── Standup Ping ──────────────────────────────────────────────
const StandupPingForm = ({ teamId }: { teamId: string | null }) => {
  const [output, setOutput] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const run = async () => { if (!teamId) return; setLoading(true); try { const r = await leaderScriptsApi.standupPing({ teamId }); if (r.data.isSuccess) setOutput(r.data.data.output); } catch {} setLoading(false); };
  return (
    <ScriptCard title="Standup Reminder" output={output || undefined}>
      <RunBtn loading={loading} onClick={run} label="📢 Send Ping" />
    </ScriptCard>
  );
};

export default LeaderScriptsPanel;

