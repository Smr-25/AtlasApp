import { useState, useEffect } from "react";
import { motion, AnimatePresence } from "framer-motion";
import { Code2, Plus, Star, Trash2, Edit3, Loader2, X, Search, Copy, Check, BookOpen } from "lucide-react";
import { snippetsApi, SnippetDto } from "@/services/api";

const SnippetsPanel = () => {
  const [snippets, setSnippets] = useState<SnippetDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [showCreate, setShowCreate] = useState(false);
  const [editTarget, setEditTarget] = useState<SnippetDto | null>(null);
  const [formTitle, setFormTitle] = useState("");
  const [formLang, setFormLang] = useState("typescript");
  const [formContent, setFormContent] = useState("");
  const [formTags, setFormTags] = useState("");
  const [saving, setSaving] = useState(false);
  const [actionLoading, setActionLoading] = useState<string | null>(null);
  const [copied, setCopied] = useState<string | null>(null);

  const fetchSnippets = async () => {
    setLoading(true);
    try {
      const res = await snippetsApi.getAll();
      if (res.data.isSuccess && Array.isArray(res.data.data)) setSnippets(res.data.data);
    } catch {}
    setLoading(false);
  };

  useEffect(() => { fetchSnippets(); }, []);

  const handleSave = async () => {
    if (!formTitle.trim() || !formContent.trim()) return;
    setSaving(true);
    try {
      const body = { title: formTitle, language: formLang, code: formContent, tags: formTags.split(",").map((t) => t.trim()).filter(Boolean) };
      if (editTarget) {
        await snippetsApi.update(editTarget.id, body);
      } else {
        await snippetsApi.create(body);
      }
      await fetchSnippets();
      closeForm();
    } catch {}
    setSaving(false);
  };

  const handleDelete = async (id: string) => {
    setActionLoading(id);
    await snippetsApi.remove(id);
    await fetchSnippets();
    setActionLoading(null);
  };

  const handleFavorite = async (id: string) => {
    setActionLoading(id);
    await snippetsApi.toggleFavorite(id);
    await fetchSnippets();
    setActionLoading(null);
  };

  const openEdit = (s: SnippetDto) => {
    setEditTarget(s);
    setFormTitle(s.title);
    setFormLang(s.language);
    setFormContent(s.content);
    setFormTags(s.tags?.join(", ") || "");
    setShowCreate(true);
  };

  const closeForm = () => {
    setShowCreate(false);
    setEditTarget(null);
    setFormTitle("");
    setFormLang("typescript");
    setFormContent("");
    setFormTags("");
  };

  const copySnippet = (id: string, content: string) => {
    navigator.clipboard.writeText(content);
    setCopied(id);
    setTimeout(() => setCopied(null), 2000);
  };

  const filtered = snippets.filter((s) =>
    !search || s.title.toLowerCase().includes(search.toLowerCase()) || s.language.toLowerCase().includes(search.toLowerCase()) || s.tags?.some((t) => t.toLowerCase().includes(search.toLowerCase()))
  );
  const favorites = filtered.filter((s) => s.isFavorite);
  const others = filtered.filter((s) => !s.isFavorite);

  const inputClass = "w-full h-10 px-3.5 rounded-lg bg-muted/40 border border-border text-sm text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/30 transition-all";

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-cyan-500/20 to-cyan-500/5 border border-cyan-500/10 flex items-center justify-center">
            <Code2 className="w-5 h-5 text-cyan-400" />
          </div>
          <div>
            <h2 className="text-lg font-bold text-foreground tracking-tight">Code Snippets</h2>
            <p className="text-xs text-muted-foreground">{snippets.length} snippets saved</p>
          </div>
        </div>
        <motion.button whileTap={{ scale: 0.98 }} onClick={() => { closeForm(); setShowCreate(true); }} className="flex items-center gap-2 px-4 h-9 rounded-lg bg-primary text-primary-foreground text-sm font-medium shadow-md shadow-primary/20 hover:shadow-lg hover:shadow-primary/25 transition-shadow">
          <Plus className="w-4 h-4" /> New Snippet
        </motion.button>
      </div>

      {/* Search */}
      <div className="relative">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
        <input value={search} onChange={(e) => setSearch(e.target.value)} placeholder="Search snippets..." className="w-full h-10 pl-10 pr-4 rounded-lg bg-muted/40 border border-border text-sm text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/30 transition-all" />
      </div>

      {loading ? (
        <div className="py-16 flex justify-center"><Loader2 className="w-6 h-6 animate-spin text-primary" /></div>
      ) : filtered.length === 0 ? (
        <div className="py-16 text-center">
          <BookOpen className="w-12 h-12 text-muted-foreground/20 mx-auto mb-3" />
          <p className="text-sm font-medium text-foreground mb-1">{search ? "No matching snippets" : "No snippets yet"}</p>
          <p className="text-xs text-muted-foreground mb-4">Create your first code snippet to get started</p>
        </div>
      ) : (
        <div className="space-y-4">
          {favorites.length > 0 && (
            <div>
              <p className="text-xs font-semibold text-muted-foreground uppercase tracking-wider mb-2 flex items-center gap-1.5"><Star className="w-3 h-3 text-amber-500" /> Favorites</p>
              <div className="grid grid-cols-1 lg:grid-cols-2 gap-3">
                {favorites.map((s, i) => <SnippetCard key={s.id} snippet={s} index={i} onEdit={openEdit} onDelete={handleDelete} onFavorite={handleFavorite} onCopy={copySnippet} actionLoading={actionLoading} copied={copied} />)}
              </div>
            </div>
          )}
          {others.length > 0 && (
            <div>
              {favorites.length > 0 && <p className="text-xs font-semibold text-muted-foreground uppercase tracking-wider mb-2">All Snippets</p>}
              <div className="grid grid-cols-1 lg:grid-cols-2 gap-3">
                {others.map((s, i) => <SnippetCard key={s.id} snippet={s} index={i} onEdit={openEdit} onDelete={handleDelete} onFavorite={handleFavorite} onCopy={copySnippet} actionLoading={actionLoading} copied={copied} />)}
              </div>
            </div>
          )}
        </div>
      )}

      {/* Create/Edit Dialog */}
      <AnimatePresence>
        {showCreate && (
          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} className="fixed inset-0 z-50 flex items-center justify-center">
            <div className="absolute inset-0 bg-black/40 backdrop-blur-sm" onClick={closeForm} />
            <motion.div initial={{ opacity: 0, scale: 0.95 }} animate={{ opacity: 1, scale: 1 }} exit={{ opacity: 0, scale: 0.95 }} className="relative w-full max-w-lg bg-card border border-border rounded-2xl shadow-2xl overflow-hidden">
              <div className="flex items-center justify-between p-5 border-b border-border">
                <h3 className="text-sm font-semibold text-foreground">{editTarget ? "Edit Snippet" : "New Snippet"}</h3>
                <button onClick={closeForm} className="w-8 h-8 rounded-lg flex items-center justify-center text-muted-foreground hover:bg-muted transition-colors"><X className="w-4 h-4" /></button>
              </div>
              <div className="p-5 space-y-3">
                <div><label className="text-xs font-medium text-foreground mb-1 block">Title</label><input value={formTitle} onChange={(e) => setFormTitle(e.target.value)} placeholder="My useful snippet" className={inputClass} autoFocus /></div>
                <div className="flex gap-3">
                  <div className="flex-1"><label className="text-xs font-medium text-foreground mb-1 block">Language</label><input value={formLang} onChange={(e) => setFormLang(e.target.value)} placeholder="typescript" className={inputClass} /></div>
                  <div className="flex-1"><label className="text-xs font-medium text-foreground mb-1 block">Tags</label><input value={formTags} onChange={(e) => setFormTags(e.target.value)} placeholder="react, hooks, api" className={inputClass} /></div>
                </div>
                <div><label className="text-xs font-medium text-foreground mb-1 block">Code</label><textarea value={formContent} onChange={(e) => setFormContent(e.target.value)} placeholder="// Your code here..." rows={8} className="w-full rounded-lg bg-muted/40 border border-border text-sm text-foreground p-3 font-mono focus:outline-none focus:ring-2 focus:ring-primary/30 transition-all resize-none" /></div>
                <div className="flex gap-2 pt-2">
                  <button onClick={closeForm} className="flex-1 h-10 rounded-lg border border-border text-sm text-foreground hover:bg-muted transition-colors">Cancel</button>
                  <motion.button whileTap={{ scale: 0.98 }} onClick={handleSave} disabled={saving || !formTitle.trim() || !formContent.trim()} className="flex-1 h-10 rounded-lg bg-primary text-primary-foreground text-sm font-medium shadow-md shadow-primary/20 disabled:opacity-50 flex items-center justify-center gap-2">
                    {saving ? <Loader2 className="w-4 h-4 animate-spin" /> : editTarget ? "Update" : "Create"}
                  </motion.button>
                </div>
              </div>
            </motion.div>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  );
};

const SnippetCard = ({ snippet, index, onEdit, onDelete, onFavorite, onCopy, actionLoading, copied }: {
  snippet: SnippetDto; index: number; onEdit: (s: SnippetDto) => void; onDelete: (id: string) => void; onFavorite: (id: string) => void; onCopy: (id: string, content: string) => void; actionLoading: string | null; copied: string | null;
}) => (
  <motion.div
    initial={{ opacity: 0, y: 10 }}
    animate={{ opacity: 1, y: 0 }}
    transition={{ delay: index * 0.03 }}
    className="relative overflow-hidden bg-gradient-to-br from-card to-card/60 rounded-xl border border-border hover:border-primary/15 transition-all group"
  >
    <div className="absolute top-0 right-0 w-16 h-16 bg-primary/[0.015] rounded-full -translate-y-1/2 translate-x-1/3" />
    <div className="flex items-center justify-between p-3 pb-0">
      <div className="flex items-center gap-2 min-w-0">
        <p className="text-sm font-medium text-foreground truncate">{snippet.title}</p>
        <span className="text-[9px] bg-primary/10 text-primary px-1.5 py-0.5 rounded font-mono">{snippet.language}</span>
      </div>
      <div className="flex items-center gap-0.5 shrink-0">
        <button onClick={() => onFavorite(snippet.id)} disabled={actionLoading === snippet.id} className="w-7 h-7 rounded flex items-center justify-center transition-colors hover:bg-muted">
          <Star className={`w-3.5 h-3.5 ${snippet.isFavorite ? "text-amber-500 fill-amber-500" : "text-muted-foreground/40"}`} />
        </button>
        <button onClick={() => onCopy(snippet.id, snippet.content)} className="w-7 h-7 rounded flex items-center justify-center transition-colors hover:bg-muted">
          {copied === snippet.id ? <Check className="w-3.5 h-3.5 text-emerald-500" /> : <Copy className="w-3.5 h-3.5 text-muted-foreground/40" />}
        </button>
        <button onClick={() => onEdit(snippet)} className="w-7 h-7 rounded flex items-center justify-center transition-colors hover:bg-muted opacity-0 group-hover:opacity-100"><Edit3 className="w-3.5 h-3.5 text-muted-foreground" /></button>
        <button onClick={() => onDelete(snippet.id)} disabled={actionLoading === snippet.id} className="w-7 h-7 rounded flex items-center justify-center transition-colors hover:bg-red-500/10 opacity-0 group-hover:opacity-100">
          {actionLoading === snippet.id ? <Loader2 className="w-3.5 h-3.5 animate-spin text-muted-foreground" /> : <Trash2 className="w-3.5 h-3.5 text-red-500/60" />}
        </button>
      </div>
    </div>
    <pre className="px-3 py-2 text-[11px] text-muted-foreground font-mono overflow-hidden max-h-20 line-clamp-4">{snippet.content}</pre>
    {snippet.tags && snippet.tags.length > 0 && (
      <div className="flex gap-1 px-3 pb-3 flex-wrap">{snippet.tags.map((t) => <span key={t} className="text-[8px] bg-muted/60 text-muted-foreground px-1.5 py-0.5 rounded">{t}</span>)}</div>
    )}
  </motion.div>
);

export default SnippetsPanel;

