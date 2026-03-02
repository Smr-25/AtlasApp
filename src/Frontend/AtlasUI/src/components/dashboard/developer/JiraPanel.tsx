import { useState, useEffect } from "react";
import { motion, AnimatePresence } from "framer-motion";
import {
  SquareKanban, Play, Clock, CheckCircle, AlertCircle,
  Loader2, Coffee, ListFilter,
} from "lucide-react";
import { gitApi, IntegrationDto } from "@/services/api";

interface JiraPanelProps {
  integrations: IntegrationDto[];
}

const priorityConfig: Record<string, { color: string; bg: string }> = {
  highest: { color: "text-red-400", bg: "bg-red-500/10 border-red-500/15" },
  high: { color: "text-orange-400", bg: "bg-orange-500/10 border-orange-500/15" },
  medium: { color: "text-amber-400", bg: "bg-amber-500/10 border-amber-500/15" },
  low: { color: "text-blue-400", bg: "bg-blue-500/10 border-blue-500/15" },
  lowest: { color: "text-slate-400", bg: "bg-slate-500/10 border-slate-500/15" },
};

const statusConfig: Record<string, { icon: typeof Play; color: string }> = {
  "To Do": { icon: ListFilter, color: "text-muted-foreground" },
  "In Progress": { icon: Play, color: "text-blue-400" },
  "In Review": { icon: Clock, color: "text-amber-400" },
  Done: { icon: CheckCircle, color: "text-emerald-400" },
};

// Tasks loaded dynamically - see fallbackTasks and useEffect below

interface JiraTask {
  key: string;
  summary: string;
  status: string;
  priority: string;
  assignee: string;
  storyPoints?: number;
}

const fallbackTasks: JiraTask[] = [
  { key: "ATLAS-142", summary: "Implement OAuth refresh token rotation", status: "In Progress", priority: "high", assignee: "You", storyPoints: 5 },
  { key: "ATLAS-138", summary: "Fix Docker container memory leak on staging", status: "To Do", priority: "highest", assignee: "You", storyPoints: 8 },
  { key: "ATLAS-155", summary: "Add rate limiting to WebSocket connections", status: "In Review", priority: "medium", assignee: "You", storyPoints: 3 },
  { key: "ATLAS-160", summary: "Write unit tests for CQRS handlers", status: "To Do", priority: "medium", assignee: "You", storyPoints: 5 },
  { key: "ATLAS-163", summary: "Update Swagger/Scalar API documentation", status: "Done", priority: "low", assignee: "You", storyPoints: 2 },
  { key: "ATLAS-170", summary: "Optimize EF Core query for team dashboard", status: "In Progress", priority: "high", assignee: "You", storyPoints: 5 },
];

const JiraPanel = ({ integrations }: JiraPanelProps) => {
  const [pomodoroLoading, setPomodoroLoading] = useState<string | null>(null);
  const [pomodoroResult, setPomodoroResult] = useState<any>(null);
  const [filter, setFilter] = useState<string>("all");
  const [tasks, setTasks] = useState<JiraTask[]>(fallbackTasks);

  const jiraInt = integrations.find((i) => i.provider === "Jira" && i.status === "Active");

  // Attempt to load real Jira data via Git dashboard when integration is active
  useEffect(() => {
    if (!jiraInt) { setTasks(fallbackTasks); return; }
    let cancelled = false;
    gitApi.dashboard(jiraInt.id).then((res) => {
      if (cancelled) return;
      if (res.data.isSuccess && res.data.data) {
        // Map PR data to task-like items if Jira board data is available
        const d = res.data.data;
        if (d.pullRequests && d.pullRequests.length > 0) {
          const mapped: JiraTask[] = d.pullRequests.map((pr) => ({
            key: `PR-${pr.id}`,
            summary: pr.title,
            status: pr.state === "open" ? "In Progress" : pr.state === "closed" ? "Done" : "In Review",
            priority: "medium",
            assignee: pr.author || "You",
            storyPoints: undefined,
          }));
          setTasks(mapped.length > 0 ? mapped : fallbackTasks);
        }
      }
    }).catch(() => { if (!cancelled) setTasks(fallbackTasks); });
    return () => { cancelled = true; };
  }, [jiraInt]);

  const handleStartPomodoro = async (issueKey: string) => {
    setPomodoroLoading(issueKey);
    try {
      const res = await gitApi.jiraPomodoro({ issueKey, duration: 25 });
      if (res.data.isSuccess) setPomodoroResult(res.data.data);
    } catch { /* silent */ }
    setPomodoroLoading(null);
  };

  const filteredTasks = filter === "all"
    ? tasks
    : tasks.filter((t) => t.status === filter);

  const statuses = ["all", "To Do", "In Progress", "In Review", "Done"];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-[#0052CC] to-[#2684FF] flex items-center justify-center shadow-lg shadow-blue-500/20">
          <SquareKanban className="w-5 h-5 text-white" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground">Jira</h2>
          <p className="text-xs text-muted-foreground">
            {jiraInt ? "Sprint tasks & issue tracking" : "Connect Jira to see your real tasks"}
          </p>
        </div>
      </div>

      {/* Sprint Stats */}
      <div className="grid grid-cols-4 gap-3">
        {[
          { label: "To Do", count: tasks.filter((t) => t.status === "To Do").length, color: "text-muted-foreground", bg: "from-slate-500/10 to-slate-500/5" },
          { label: "In Progress", count: tasks.filter((t) => t.status === "In Progress").length, color: "text-blue-400", bg: "from-blue-500/10 to-blue-500/5" },
          { label: "In Review", count: tasks.filter((t) => t.status === "In Review").length, color: "text-amber-400", bg: "from-amber-500/10 to-amber-500/5" },
          { label: "Done", count: tasks.filter((t) => t.status === "Done").length, color: "text-emerald-400", bg: "from-emerald-500/10 to-emerald-500/5" },
        ].map((s, i) => (
          <motion.div
            key={s.label}
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: i * 0.06 }}
            className={`rounded-xl border border-border p-3 bg-gradient-to-br ${s.bg} text-center`}
          >
            <p className={`text-xl font-bold ${s.color}`}>{s.count}</p>
            <p className="text-[10px] text-muted-foreground mt-0.5">{s.label}</p>
          </motion.div>
        ))}
      </div>

      {/* Filter Tabs */}
      <div className="flex items-center gap-1 p-1 rounded-xl bg-muted/30 border border-border overflow-x-auto">
        {statuses.map((s) => (
          <button
            key={s}
            onClick={() => setFilter(s)}
            className={`px-3 py-1.5 rounded-lg text-[11px] font-medium transition-all whitespace-nowrap ${
              filter === s ? "bg-card text-foreground shadow-sm border border-border/50" : "text-muted-foreground hover:text-foreground"
            }`}
          >
            {s === "all" ? "All Tasks" : s}
          </button>
        ))}
      </div>

      {/* Task List */}
      <AnimatePresence mode="wait">
        <motion.div key={filter} initial={{ opacity: 0, y: 6 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0 }} className="space-y-2">
          {filteredTasks.map((task, i) => {
            const pCfg = priorityConfig[task.priority] || priorityConfig.medium;
            const sCfg = statusConfig[task.status] || statusConfig["To Do"];
            const StatusIcon = sCfg.icon;
            return (
              <motion.div
                key={task.key}
                initial={{ opacity: 0, y: 8 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: i * 0.04 }}
                className="group relative overflow-hidden rounded-xl border border-border bg-card/60 hover:bg-card hover:border-primary/10 transition-all duration-200"
              >
                <div className="flex items-start gap-3.5 p-4">
                  <div className={`w-8 h-8 rounded-lg flex items-center justify-center shrink-0 border ${pCfg.bg}`}>
                    <StatusIcon className={`w-4 h-4 ${sCfg.color}`} />
                  </div>
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center gap-2 mb-1">
                      <code className="text-[10px] text-primary font-mono bg-primary/5 px-1.5 py-0.5 rounded">{task.key}</code>
                      <span className={`text-[9px] font-medium px-1.5 py-0.5 rounded border ${pCfg.bg} ${pCfg.color}`}>{task.priority}</span>
                      {task.storyPoints && <span className="text-[9px] text-muted-foreground bg-muted/50 px-1.5 py-0.5 rounded">{task.storyPoints} SP</span>}
                    </div>
                    <p className="text-sm text-foreground leading-snug">{task.summary}</p>
                  </div>
                  <div className="flex items-center gap-1.5 shrink-0 opacity-0 group-hover:opacity-100 transition-all">
                    <motion.button
                      whileTap={{ scale: 0.9 }}
                      onClick={() => handleStartPomodoro(task.key)}
                      disabled={!!pomodoroLoading}
                      className="flex items-center gap-1.5 px-2.5 py-1.5 rounded-lg text-[10px] font-medium text-primary bg-primary/10 hover:bg-primary/15 border border-primary/10 transition-all"
                      title="Start Focus Session"
                    >
                      {pomodoroLoading === task.key ? <Loader2 className="w-3 h-3 animate-spin" /> : <Coffee className="w-3 h-3" />}
                      Focus
                    </motion.button>
                  </div>
                </div>
              </motion.div>
            );
          })}
        </motion.div>
      </AnimatePresence>

      {!jiraInt && (
        <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="p-3 rounded-xl bg-amber-500/5 border border-amber-500/10 flex items-center gap-2.5">
          <AlertCircle className="w-4 h-4 text-amber-500 shrink-0" />
          <p className="text-[11px] text-muted-foreground">
            Showing demo data. Connect your Jira integration to see real sprint tasks.
          </p>
        </motion.div>
      )}
    </div>
  );
};

export default JiraPanel;

