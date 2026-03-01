import { useState } from "react";
import { motion, AnimatePresence } from "framer-motion";
import { Code2, Play, Database, Trash2, FileCode, GitMerge, RefreshCw, Braces, Server, Loader2, X, Check, AlertCircle } from "lucide-react";
import { scriptsApi, ScriptRunResult } from "@/services/api";

type QuickAction = { id: string; name: string; icon: typeof Play; desc: string; color: string; action: (input: string) => Promise<any> };

const quickActions: QuickAction[] = [
  { id: "spin", name: "Spin Environment", icon: Server, desc: "Bootstrap a dev environment", color: "text-blue-400 bg-blue-500/10", action: (p) => scriptsApi.spinEnvironment({ projectPath: p }) },
  { id: "nuke", name: "Nuke & Migrate", icon: Database, desc: "Drop DB + run migrations", color: "text-red-400 bg-red-500/10", action: (p) => scriptsApi.nukeMigrate({ projectPath: p }) },
  { id: "conflicts", name: "Resolve Conflicts", icon: GitMerge, desc: "Auto-resolve git conflicts", color: "text-amber-400 bg-amber-500/10", action: (p) => scriptsApi.resolveConflicts({ projectPath: p }) },
  { id: "flush", name: "Flush Cache", icon: RefreshCw, desc: "Clear all caches", color: "text-cyan-400 bg-cyan-500/10", action: () => scriptsApi.flushCache() },
  { id: "format", name: "Format & Lint", icon: Braces, desc: "Run formatters & linters", color: "text-green-400 bg-green-500/10", action: (p) => scriptsApi.formatLint({ projectPath: p }) },
  { id: "kill", name: "Kill Node.js", icon: Trash2, desc: "Kill all Node processes", color: "text-orange-400 bg-orange-500/10", action: () => scriptsApi.killNodes() },
  { id: "boilerplate", name: "Boilerplate", icon: FileCode, desc: "Generate project scaffold", color: "text-purple-400 bg-purple-500/10", action: (p) => scriptsApi.generateBoilerplate({ template: "react", name: "new-project", outputPath: p }) },
];

const ScriptsPanel = () => {
  const [runningId, setRunningId] = useState<string | null>(null);
  const [result, setResult] = useState<ScriptRunResult | null>(null);
  const [error, setError] = useState("");
  const [projectPath, setProjectPath] = useState("");
  const [showResult, setShowResult] = useState(false);

  const handleQuickAction = async (action: QuickAction) => {
    setRunningId(action.id);
    setResult(null);
    setError("");
    try {
      const res = await action.action(projectPath || ".");
      if (res?.data?.isSuccess) {
        setResult(res.data.data);
        setShowResult(true);
      } else {
        setError(res?.data?.errors?.[0] || "Script failed");
        setShowResult(true);
      }
    } catch (err: any) {
      setError(err?.response?.data?.errors?.[0] || err.message || "Script execution failed");
      setShowResult(true);
    } finally {
      setRunningId(null);
    }
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-amber-500/20 to-amber-500/5 border border-amber-500/10 flex items-center justify-center">
          <Code2 className="w-5 h-5 text-amber-400" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">Scripts & Automation</h2>
          <p className="text-xs text-muted-foreground">One-click developer automations</p>
        </div>
      </div>

      {/* Project Path */}
      <div className="relative overflow-hidden rounded-xl border border-border p-4 bg-gradient-to-br from-card to-card/60">
        <div className="absolute top-0 right-0 w-32 h-32 bg-primary/[0.02] rounded-full -translate-y-1/2 translate-x-1/3" />
        <label className="text-xs font-medium text-foreground mb-1.5 block relative">Project Path</label>
        <input
          type="text"
          value={projectPath}
          onChange={(e) => setProjectPath(e.target.value)}
          placeholder="/path/to/your/project (optional — defaults to current dir)"
          className="w-full h-10 px-3.5 rounded-lg bg-muted/30 border border-border text-sm text-foreground placeholder:text-muted-foreground/50 focus:outline-none focus:ring-2 focus:ring-primary/30 focus:border-primary/20 font-mono transition-all relative"
        />
      </div>

      {/* Quick Actions */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-3">
        {quickActions.map((action, i) => (
          <motion.button
            key={action.id}
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: i * 0.03 }}
            whileHover={{ y: -2, transition: { duration: 0.2 } }}
            whileTap={{ scale: 0.97 }}
            onClick={() => handleQuickAction(action)}
            disabled={runningId !== null}
            className="relative overflow-hidden flex items-start gap-3 p-4 rounded-xl bg-gradient-to-br from-card to-card/60 border border-border hover:border-primary/15 transition-all text-left disabled:opacity-60 group"
          >
            <div className="absolute top-0 right-0 w-16 h-16 bg-primary/[0.02] rounded-full -translate-y-1/2 translate-x-1/3 group-hover:bg-primary/[0.04] transition-colors duration-500" />
            <div className={`w-10 h-10 rounded-lg flex items-center justify-center shrink-0 ${action.color} border border-current/10`}>
              {runningId === action.id ? <Loader2 className="w-5 h-5 animate-spin" /> : <action.icon className="w-5 h-5" />}
            </div>
            <div className="min-w-0 relative">
              <p className="text-sm font-medium text-foreground">{action.name}</p>
              <p className="text-[10px] text-muted-foreground">{action.desc}</p>
            </div>
            <Play className="w-3.5 h-3.5 text-muted-foreground/20 group-hover:text-primary/50 transition-colors shrink-0 mt-1 ml-auto" />
          </motion.button>
        ))}
      </div>

      {/* Result Dialog */}
      <AnimatePresence>
        {showResult && (
          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} className="fixed inset-0 z-50 flex items-center justify-center">
            <div className="absolute inset-0 bg-black/40 backdrop-blur-sm" onClick={() => setShowResult(false)} />
            <motion.div
              initial={{ opacity: 0, scale: 0.95, y: 10 }}
              animate={{ opacity: 1, scale: 1, y: 0 }}
              exit={{ opacity: 0, scale: 0.95 }}
              className="relative w-full max-w-lg bg-card border border-border rounded-2xl shadow-2xl overflow-hidden"
            >
              <div className="flex items-center justify-between p-4 border-b border-border">
                <div className="flex items-center gap-2">
                  {error ? <AlertCircle className="w-5 h-5 text-destructive" /> : <Check className="w-5 h-5 text-emerald-500" />}
                  <h3 className="text-sm font-semibold text-foreground">{error ? "Script Failed" : "Script Output"}</h3>
                </div>
                <button onClick={() => setShowResult(false)} className="w-8 h-8 rounded-lg flex items-center justify-center text-muted-foreground hover:bg-muted transition-colors">
                  <X className="w-4 h-4" />
                </button>
              </div>
              <div className="p-4">
                {error ? (
                  <div className="p-3 rounded-lg bg-destructive/10 border border-destructive/20 text-destructive text-xs">{error}</div>
                ) : result ? (
                  <div className="space-y-3">
                    <div className="flex items-center gap-4 text-xs text-muted-foreground">
                      <span>Exit code: <span className={result.exitCode === 0 ? "text-emerald-500" : "text-red-500"}>{result.exitCode}</span></span>
                      <span>Duration: {result.duration}ms</span>
                    </div>
                    <pre className="p-3 rounded-lg bg-muted/50 border border-border text-xs text-foreground font-mono overflow-x-auto max-h-64 whitespace-pre-wrap">{result.output || "No output"}</pre>
                  </div>
                ) : null}
              </div>
            </motion.div>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  );
};

export default ScriptsPanel;

