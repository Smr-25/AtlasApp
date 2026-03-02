import { useState, useEffect, useRef, useCallback } from "react";
import { motion, AnimatePresence } from "framer-motion";
import { Search, ArrowRight, Command, Loader2 } from "lucide-react";
import { searchApi, type SearchResultItem } from "@/services/api";

interface CommandPaletteProps {
  open: boolean;
  onClose: () => void;
  onNavigate: (route: string) => void;
}

export default function CommandPalette({ open, onClose, onNavigate }: CommandPaletteProps) {
  const [query, setQuery] = useState("");
  const [results, setResults] = useState<Record<string, SearchResultItem[]>>({});
  const [loading, setLoading] = useState(false);
  const [selectedIdx, setSelectedIdx] = useState(0);
  const inputRef = useRef<HTMLInputElement>(null);
  const debounceRef = useRef<ReturnType<typeof setTimeout>>();

  // Focus input when opened
  useEffect(() => {
    if (open) {
      setTimeout(() => inputRef.current?.focus(), 50);
      setQuery("");
      setResults({});
      setSelectedIdx(0);
    }
  }, [open]);

  // Global Cmd+K listener
  useEffect(() => {
    const handler = (e: KeyboardEvent) => {
      if ((e.metaKey || e.ctrlKey) && e.key === "k") {
        e.preventDefault();
        if (open) onClose();
        // parent handles open toggle
      }
      if (e.key === "Escape" && open) {
        onClose();
      }
    };
    window.addEventListener("keydown", handler);
    return () => window.removeEventListener("keydown", handler);
  }, [open, onClose]);

  // Debounced search
  const doSearch = useCallback(async (q: string) => {
    if (q.length < 2) { setResults({}); setLoading(false); return; }
    setLoading(true);
    try {
      const r = await searchApi.search(q);
      if (r.data.isSuccess && r.data.data) {
        const d = r.data.data;
        const groups: Record<string, SearchResultItem[]> = {};
        if (d.commands?.length) groups["Commands"] = d.commands;
        if (d.workspaces?.length) groups["Workspaces"] = d.workspaces;
        if (d.integrations?.length) groups["Integrations"] = d.integrations;
        if (d.scripts?.length) groups["Scripts"] = d.scripts;
        if (d.snippets?.length) groups["Snippets"] = d.snippets;
        if (d.projects?.length) groups["Projects"] = d.projects;
        if (d.teams?.length) groups["Teams"] = d.teams;
        setResults(groups);
      }
    } catch {
      setResults({});
    }
    setLoading(false);
  }, []);

  useEffect(() => {
    if (debounceRef.current) clearTimeout(debounceRef.current);
    debounceRef.current = setTimeout(() => doSearch(query), 300);
    return () => { if (debounceRef.current) clearTimeout(debounceRef.current); };
  }, [query, doSearch]);

  // Flatten results for keyboard nav
  const allItems = Object.values(results).flat();

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === "ArrowDown") { e.preventDefault(); setSelectedIdx((p) => Math.min(p + 1, allItems.length - 1)); }
    if (e.key === "ArrowUp") { e.preventDefault(); setSelectedIdx((p) => Math.max(p - 1, 0)); }
    if (e.key === "Enter" && allItems[selectedIdx]) {
      e.preventDefault();
      onNavigate(allItems[selectedIdx].route);
      onClose();
    }
  };

  if (!open) return null;

  return (
    <AnimatePresence>
      <motion.div
        initial={{ opacity: 0 }}
        animate={{ opacity: 1 }}
        exit={{ opacity: 0 }}
        className="fixed inset-0 z-[100] flex items-start justify-center pt-[15vh]"
        onClick={onClose}
      >
        {/* Backdrop */}
        <div className="absolute inset-0 bg-background/60 backdrop-blur-sm" />

        {/* Modal */}
        <motion.div
          initial={{ opacity: 0, scale: 0.95, y: -10 }}
          animate={{ opacity: 1, scale: 1, y: 0 }}
          exit={{ opacity: 0, scale: 0.95, y: -10 }}
          transition={{ type: "spring", damping: 25, stiffness: 300 }}
          className="relative w-full max-w-lg rounded-2xl border border-border/30 bg-card shadow-2xl shadow-black/20 overflow-hidden"
          onClick={(e) => e.stopPropagation()}
        >
          {/* Input */}
          <div className="flex items-center gap-3 px-4 py-3 border-b border-border/20">
            <Search className="w-4 h-4 text-muted-foreground shrink-0" />
            <input
              ref={inputRef}
              value={query}
              onChange={(e) => { setQuery(e.target.value); setSelectedIdx(0); }}
              onKeyDown={handleKeyDown}
              placeholder="Search workspaces, commands, scripts..."
              className="flex-1 bg-transparent text-sm text-foreground placeholder:text-muted-foreground/50 outline-none"
            />
            {loading && <Loader2 className="w-3.5 h-3.5 animate-spin text-primary shrink-0" />}
            <kbd className="hidden sm:flex items-center gap-0.5 px-1.5 py-0.5 rounded bg-muted/30 text-[10px] font-mono text-muted-foreground">
              ESC
            </kbd>
          </div>

          {/* Results */}
          <div className="max-h-[50vh] overflow-y-auto py-2">
            {query.length < 2 ? (
              <div className="px-4 py-8 text-center">
                <div className="flex items-center justify-center gap-1 text-muted-foreground/40 mb-2">
                  <Command className="w-4 h-4" /> <span className="text-xs font-mono">K</span>
                </div>
                <p className="text-xs text-muted-foreground/40">Type at least 2 characters to search</p>
              </div>
            ) : Object.keys(results).length === 0 && !loading ? (
              <div className="px-4 py-8 text-center">
                <p className="text-xs text-muted-foreground">No results for "{query}"</p>
              </div>
            ) : (
              <>
                {Object.entries(results).map(([group, items]) => {
                  const groupStartIdx = allItems.indexOf(items[0]);
                  return (
                    <div key={group}>
                      <p className="px-4 py-1.5 text-[10px] font-bold uppercase tracking-wider text-muted-foreground/40">{group}</p>
                      {items.map((r, idx) => {
                        const globalIdx = groupStartIdx + idx;
                        const selected = globalIdx === selectedIdx;
                        return (
                          <button
                            key={r.id + r.route}
                            onClick={() => { onNavigate(r.route); onClose(); }}
                            onMouseEnter={() => setSelectedIdx(globalIdx)}
                            className={`w-full flex items-center gap-3 px-4 py-2.5 text-left transition-colors ${selected ? "bg-primary/10" : "hover:bg-muted/10"}`}
                          >
                            <span className="text-base shrink-0">{r.icon || "📄"}</span>
                            <div className="flex-1 min-w-0">
                              <p className={`text-sm font-medium truncate ${selected ? "text-primary" : "text-foreground"}`}>{r.title}</p>
                              {r.subtitle && <p className="text-[10px] text-muted-foreground truncate">{r.subtitle}</p>}
                            </div>
                            {selected && <ArrowRight className="w-3.5 h-3.5 text-primary shrink-0" />}
                          </button>
                        );
                      })}
                    </div>
                  );
                })}
              </>
            )}
          </div>

          {/* Footer */}
          <div className="px-4 py-2 border-t border-border/10 flex items-center gap-4 text-[10px] text-muted-foreground/40">
            <span>↑↓ Navigate</span>
            <span>↵ Select</span>
            <span>esc Close</span>
          </div>
        </motion.div>
      </motion.div>
    </AnimatePresence>
  );
}

