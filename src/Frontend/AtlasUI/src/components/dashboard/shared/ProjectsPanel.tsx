import { useState } from "react";
import { motion } from "framer-motion";
import {
  FolderGit2, Plus, Loader2, Database, ArrowUpCircle,
} from "lucide-react";
import { projectsApi } from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

const ProjectsPanel = () => {
  const [name, setName] = useState("");
  const [path, setPath] = useState("");
  const [framework, setFramework] = useState("");
  const [creating, setCreating] = useState(false);
  const [createdId, setCreatedId] = useState<string | null>(null);
  const [migrationName, setMigrationName] = useState("");
  const [migrationResult, setMigrationResult] = useState<string | null>(null);
  const [dbResult, setDbResult] = useState<string | null>(null);
  const [actionLoading, setActionLoading] = useState<string | null>(null);

  const handleCreate = async () => {
    if (!name.trim() || !path.trim()) return;
    setCreating(true);
    try {
      const r = await projectsApi.create({ name, path, framework: framework || undefined });
      if (r.data.isSuccess && r.data.data) setCreatedId(r.data.data.id);
      setName(""); setPath(""); setFramework("");
    } catch {}
    setCreating(false);
  };

  const handleMigration = async () => {
    if (!createdId) return;
    setActionLoading("mig");
    try {
      const r = await projectsApi.runMigration(createdId, { migrationName: migrationName || undefined });
      setMigrationResult(typeof r.data.data === "string" ? r.data.data : JSON.stringify(r.data.data));
    } catch (e: any) { setMigrationResult("Error: " + (e.response?.data?.errors?.[0] || e.message)); }
    setActionLoading(null);
  };

  const handleDbUpdate = async () => {
    if (!createdId) return;
    setActionLoading("db");
    try {
      const r = await projectsApi.databaseUpdate(createdId);
      setDbResult(typeof r.data.data === "string" ? r.data.data : JSON.stringify(r.data.data));
    } catch (e: any) { setDbResult("Error: " + (e.response?.data?.errors?.[0] || e.message)); }
    setActionLoading(null);
  };

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-violet-500/20 to-violet-500/5 border border-violet-500/10 flex items-center justify-center">
          <FolderGit2 className="w-5 h-5 text-violet-400" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">Projects</h2>
          <p className="text-xs text-muted-foreground">Create & manage project scaffolding</p>
        </div>
      </motion.div>

      {/* Create project */}
      <motion.div variants={item} className="p-4 rounded-xl bg-card/50 border border-border/20 space-y-3">
        <p className="text-xs font-bold text-foreground">Create Project</p>
        <input value={name} onChange={e => setName(e.target.value)} placeholder="Project name..."
          className="w-full h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground placeholder:text-muted-foreground/50" />
        <input value={path} onChange={e => setPath(e.target.value)} placeholder="Project path (e.g. /Users/dev/myapp)"
          className="w-full h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground placeholder:text-muted-foreground/50" />
        <input value={framework} onChange={e => setFramework(e.target.value)} placeholder="Framework (optional: dotnet, react...)"
          className="w-full h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground placeholder:text-muted-foreground/50" />
        <button onClick={handleCreate} disabled={creating || !name.trim() || !path.trim()}
          className="w-full h-8 rounded-lg bg-primary text-primary-foreground text-xs font-semibold flex items-center justify-center gap-1.5 hover:bg-primary/90 disabled:opacity-50 transition-colors">
          {creating ? <Loader2 className="w-3 h-3 animate-spin" /> : <Plus className="w-3 h-3" />} Create Project
        </button>
      </motion.div>

      {/* Migration & DB tools */}
      {createdId && (
        <motion.div variants={item} className="p-4 rounded-xl bg-card/50 border border-border/20 space-y-3">
          <p className="text-xs font-bold text-foreground">Project Tools — <span className="text-primary">{createdId.slice(0, 8)}...</span></p>
          <input value={migrationName} onChange={e => setMigrationName(e.target.value)} placeholder="Migration name (e.g. AddUserTable)"
            className="w-full h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground placeholder:text-muted-foreground/50" />
          <div className="flex gap-2">
            <button onClick={handleMigration} disabled={actionLoading === "mig"}
              className="flex-1 h-8 rounded-lg bg-violet-500/10 text-violet-400 text-xs font-semibold flex items-center justify-center gap-1.5 hover:bg-violet-500/20 disabled:opacity-50 transition-colors">
              {actionLoading === "mig" ? <Loader2 className="w-3 h-3 animate-spin" /> : <Database className="w-3 h-3" />} Add Migration
            </button>
            <button onClick={handleDbUpdate} disabled={actionLoading === "db"}
              className="flex-1 h-8 rounded-lg bg-emerald-500/10 text-emerald-400 text-xs font-semibold flex items-center justify-center gap-1.5 hover:bg-emerald-500/20 disabled:opacity-50 transition-colors">
              {actionLoading === "db" ? <Loader2 className="w-3 h-3 animate-spin" /> : <ArrowUpCircle className="w-3 h-3" />} DB Update
            </button>
          </div>
          {migrationResult && (
            <div className="p-2 rounded-lg bg-muted/20 border border-border/15 text-[10px] text-foreground whitespace-pre-wrap font-mono max-h-32 overflow-auto">
              {migrationResult}
            </div>
          )}
          {dbResult && (
            <div className="p-2 rounded-lg bg-muted/20 border border-border/15 text-[10px] text-foreground whitespace-pre-wrap font-mono max-h-32 overflow-auto">
              {dbResult}
            </div>
          )}
        </motion.div>
      )}
    </motion.div>
  );
};

export default ProjectsPanel;


