import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import {
  BookOpen, Plus, Loader2, Pin, Trash2, ExternalLink,
  FileText, Wrench, GraduationCap, Newspaper, Filter,
} from "lucide-react";
import { resourceHubApi, teamsApi, ResourceDto, TeamDto } from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

const catIcons: Record<string, typeof FileText> = {
  Documentation: FileText, Tutorial: GraduationCap, Tool: Wrench, Article: Newspaper,
};
const catColors: Record<string, string> = {
  Documentation: "text-blue-400 bg-blue-500/10", Tutorial: "text-emerald-400 bg-emerald-500/10",
  Tool: "text-amber-400 bg-amber-500/10", Article: "text-violet-400 bg-violet-500/10",
};

const ResourceHubPanel = () => {
  const [teams, setTeams] = useState<TeamDto[]>([]);
  const [teamId, setTeamId] = useState<string | null>(null);
  const [resources, setResources] = useState<ResourceDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [catFilter, setCatFilter] = useState("");
  const [title, setTitle] = useState("");
  const [url, setUrl] = useState("");
  const [desc, setDesc] = useState("");
  const [category, setCategory] = useState("Documentation");
  const [adding, setAdding] = useState(false);
  const [actionLoading, setActionLoading] = useState<string | null>(null);

  useEffect(() => {
    teamsApi.getMyTeams().then(r => {
      if (r.data.isSuccess && r.data.data?.length > 0) {
        setTeams(r.data.data);
        setTeamId(r.data.data[0].id);
      }
    }).catch(() => {});
  }, []);

  useEffect(() => {
    if (!teamId) { setLoading(false); return; }
    setLoading(true);
    resourceHubApi.getResources(teamId, catFilter || undefined).then(r => {
      if (r.data.isSuccess) setResources(r.data.data || []);
    }).catch(() => {}).finally(() => setLoading(false));
  }, [teamId, catFilter]);

  const handleAdd = async () => {
    if (!teamId || !title.trim() || !url.trim()) return;
    setAdding(true);
    try {
      await resourceHubApi.create({ teamId, title, url, category, description: desc });
      setTitle(""); setUrl(""); setDesc("");
      const r = await resourceHubApi.getResources(teamId, catFilter || undefined);
      if (r.data.isSuccess) setResources(r.data.data || []);
    } catch {}
    setAdding(false);
  };

  const handlePin = async (id: string) => {
    setActionLoading(id);
    try {
      await resourceHubApi.togglePin(id);
      if (teamId) {
        const r = await resourceHubApi.getResources(teamId, catFilter || undefined);
        if (r.data.isSuccess) setResources(r.data.data || []);
      }
    } catch {}
    setActionLoading(null);
  };

  const handleDelete = async (id: string) => {
    setActionLoading(id);
    try {
      await resourceHubApi.remove(id);
      setResources(prev => prev.filter(r => r.id !== id));
    } catch {}
    setActionLoading(null);
  };

  const categories = ["", "Documentation", "Tutorial", "Tool", "Article"];

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-emerald-500/20 to-emerald-500/5 border border-emerald-500/10 flex items-center justify-center">
            <BookOpen className="w-5 h-5 text-emerald-400" />
          </div>
          <div>
            <h2 className="text-lg font-bold text-foreground tracking-tight">Resource Hub</h2>
            <p className="text-xs text-muted-foreground">Shared team resources</p>
          </div>
        </div>
        {teams.length > 1 && (
          <select value={teamId || ""} onChange={e => setTeamId(e.target.value)}
            className="h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground">
            {teams.map(t => <option key={t.id} value={t.id}>{t.name}</option>)}
          </select>
        )}
      </motion.div>

      {/* Category filter */}
      <motion.div variants={item} className="flex gap-1.5 flex-wrap">
        {categories.map(c => (
          <button key={c} onClick={() => setCatFilter(c)}
            className={`px-3 py-1.5 rounded-lg text-[10px] font-semibold transition-all ${catFilter === c ? "bg-primary/15 text-primary" : "bg-muted/20 text-muted-foreground hover:text-foreground"}`}>
            <Filter className="w-3 h-3 inline mr-1" />{c || "All"}
          </button>
        ))}
      </motion.div>

      {/* Add resource */}
      <motion.div variants={item} className="p-3 rounded-xl bg-card/30 border border-dashed border-border/30 space-y-2">
        <div className="flex gap-2">
          <input value={title} onChange={e => setTitle(e.target.value)} placeholder="Resource title..."
            className="flex-1 h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground placeholder:text-muted-foreground/50" />
          <select value={category} onChange={e => setCategory(e.target.value)}
            className="h-8 px-2 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground">
            {["Documentation", "Tutorial", "Tool", "Article"].map(c => <option key={c}>{c}</option>)}
          </select>
        </div>
        <input value={url} onChange={e => setUrl(e.target.value)} placeholder="URL (https://...)"
          className="w-full h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground placeholder:text-muted-foreground/50" />
        <input value={desc} onChange={e => setDesc(e.target.value)} placeholder="Description (optional)"
          className="w-full h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground placeholder:text-muted-foreground/50" />
        <button onClick={handleAdd} disabled={adding || !title.trim() || !url.trim()}
          className="w-full h-8 rounded-lg bg-primary text-primary-foreground text-xs font-semibold flex items-center justify-center gap-1.5 hover:bg-primary/90 disabled:opacity-50 transition-colors">
          {adding ? <Loader2 className="w-3 h-3 animate-spin" /> : <Plus className="w-3 h-3" />} Add Resource
        </button>
      </motion.div>

      {/* Resources list */}
      {!teamId ? (
        <p className="text-xs text-muted-foreground text-center py-8">No team found.</p>
      ) : loading ? (
        <div className="flex justify-center py-8"><Loader2 className="w-5 h-5 animate-spin text-primary" /></div>
      ) : resources.length === 0 ? (
        <p className="text-xs text-muted-foreground text-center py-8">No resources yet.</p>
      ) : (
        <div className="space-y-2">
          {resources.map((r, i) => {
            const Icon = catIcons[r.category] || FileText;
            const color = catColors[r.category] || "text-muted-foreground bg-muted/10";
            return (
              <motion.div key={r.id || i} variants={item}
                className={`p-3 rounded-xl bg-card/50 border border-border/15 flex items-start gap-3 ${r.isPinned ? "ring-1 ring-amber-500/30" : ""}`}>
                <div className={`w-8 h-8 rounded-lg ${color} flex items-center justify-center shrink-0`}>
                  <Icon className="w-4 h-4" />
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-xs font-semibold text-foreground">{r.title}</p>
                  {r.description && <p className="text-[10px] text-muted-foreground truncate">{r.description}</p>}
                  <span className="text-[10px] text-muted-foreground/50">{r.category}</span>
                </div>
                <div className="flex items-center gap-1 shrink-0">
                  <button onClick={() => handlePin(r.id)} disabled={actionLoading === r.id}
                    className={`p-1.5 rounded-lg hover:bg-muted/20 transition-colors ${r.isPinned ? "text-amber-400" : "text-muted-foreground"}`}>
                    <Pin className="w-3 h-3" />
                  </button>
                  <a href={r.url} target="_blank" rel="noopener" className="p-1.5 rounded-lg text-muted-foreground hover:text-foreground hover:bg-muted/20 transition-colors">
                    <ExternalLink className="w-3 h-3" />
                  </a>
                  <button onClick={() => handleDelete(r.id)} disabled={actionLoading === r.id}
                    className="p-1.5 rounded-lg text-muted-foreground hover:text-red-400 hover:bg-red-500/10 transition-colors">
                    <Trash2 className="w-3 h-3" />
                  </button>
                </div>
              </motion.div>
            );
          })}
        </div>
      )}
    </motion.div>
  );
};

export default ResourceHubPanel;

