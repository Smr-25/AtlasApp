import { useState, useEffect, useCallback } from "react";
import { motion } from "framer-motion";
import {
  Activity, Send, Loader2, Filter,
} from "lucide-react";
import { omniFeedApi, teamsApi, OmniFeedItemDto, TeamDto } from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

const sourceIcons: Record<string, string> = { GitHub: "🐙", Jira: "📋", Slack: "💬", Figma: "🎨", Manual: "📝" };
const sourceColors: Record<string, string> = {
  GitHub: "border-l-violet-500", Jira: "border-l-blue-500", Slack: "border-l-emerald-500",
  Figma: "border-l-pink-500", Manual: "border-l-amber-500",
};

const OmniFeedPanel = () => {
  const [teams, setTeams] = useState<TeamDto[]>([]);
  const [teamId, setTeamId] = useState<string | null>(null);
  const [feedItems, setFeedItems] = useState<OmniFeedItemDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [sourceFilter, setSourceFilter] = useState<string>("");
  const [page, setPage] = useState(1);
  const [newContent, setNewContent] = useState("");
  const [publishing, setPublishing] = useState(false);

  useEffect(() => {
    teamsApi.getMyTeams().then(r => {
      if (r.data.isSuccess && r.data.data?.length > 0) {
        setTeams(r.data.data);
        setTeamId(r.data.data[0].id);
      }
    }).catch(() => {});
  }, []);

  const fetchFeed = useCallback(async () => {
    if (!teamId) return;
    setLoading(true);
    try {
      const r = await omniFeedApi.getFeed(teamId, { source: sourceFilter || undefined, page, pageSize: 20 });
      if (r.data.isSuccess) setFeedItems(r.data.data || []);
    } catch {}
    setLoading(false);
  }, [teamId, sourceFilter, page]);

  useEffect(() => { fetchFeed(); }, [fetchFeed]);

  const handlePublish = async () => {
    if (!teamId || !newContent.trim()) return;
    setPublishing(true);
    try {
      await omniFeedApi.publish({ teamId, content: newContent, source: "Manual" });
      setNewContent("");
      await fetchFeed();
    } catch {}
    setPublishing(false);
  };

  const handleEmoji = async (itemId: string, emoji: string) => {
    try {
      await omniFeedApi.addEmoji(itemId, { emoji });
      await fetchFeed();
    } catch {}
  };

  const sources = ["", "GitHub", "Jira", "Slack", "Figma", "Manual"];

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-violet-500/20 to-violet-500/5 border border-violet-500/10 flex items-center justify-center">
            <Activity className="w-5 h-5 text-violet-400" />
          </div>
          <div>
            <h2 className="text-lg font-bold text-foreground tracking-tight">OmniFeed</h2>
            <p className="text-xs text-muted-foreground">Team activity stream</p>
          </div>
        </div>
        {teams.length > 1 && (
          <select value={teamId || ""} onChange={e => { setTeamId(e.target.value); setPage(1); }}
            className="h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground">
            {teams.map(t => <option key={t.id} value={t.id}>{t.name}</option>)}
          </select>
        )}
      </motion.div>

      {/* Source filter */}
      <motion.div variants={item} className="flex gap-1.5 flex-wrap">
        {sources.map(s => (
          <button key={s} onClick={() => { setSourceFilter(s); setPage(1); }}
            className={`px-3 py-1.5 rounded-lg text-[10px] font-semibold transition-all ${sourceFilter === s ? "bg-primary/15 text-primary" : "bg-muted/20 text-muted-foreground hover:text-foreground"}`}>
            <Filter className="w-3 h-3 inline mr-1" />{s || "All"}
          </button>
        ))}
      </motion.div>

      {/* Publish */}
      <motion.div variants={item} className="flex gap-2">
        <input value={newContent} onChange={e => setNewContent(e.target.value)} placeholder="Share an update with your team..."
          onKeyDown={e => e.key === "Enter" && handlePublish()}
          className="flex-1 h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-xs text-foreground placeholder:text-muted-foreground/50" />
        <button onClick={handlePublish} disabled={publishing || !newContent.trim() || !teamId}
          className="h-9 px-4 rounded-xl bg-primary text-primary-foreground text-xs font-semibold flex items-center gap-1.5 hover:bg-primary/90 disabled:opacity-50 transition-colors">
          {publishing ? <Loader2 className="w-3 h-3 animate-spin" /> : <Send className="w-3 h-3" />} Post
        </button>
      </motion.div>

      {/* Feed */}
      {!teamId ? (
        <p className="text-xs text-muted-foreground text-center py-8">No team found. Create a team first.</p>
      ) : loading ? (
        <div className="flex justify-center py-8"><Loader2 className="w-5 h-5 animate-spin text-primary" /></div>
      ) : feedItems.length === 0 ? (
        <p className="text-xs text-muted-foreground text-center py-8">No activity yet. Share an update!</p>
      ) : (
        <div className="space-y-2">
          {feedItems.map((fi, i) => (
            <motion.div key={fi.id || i} variants={item}
              className={`p-3 rounded-xl bg-card/50 border-l-2 border border-border/20 ${sourceColors[fi.source] || "border-l-muted"} hover:bg-card/70 transition-colors`}>
              <div className="flex items-start gap-3">
                <span className="text-sm">{sourceIcons[fi.source] || "📌"}</span>
                <div className="flex-1 min-w-0">
                  <p className="text-xs text-foreground leading-relaxed">{fi.content}</p>
                  <div className="flex items-center gap-3 mt-1.5">
                    <span className="text-[10px] text-muted-foreground/60">{new Date(fi.timestamp).toLocaleString()}</span>
                    <span className="text-[10px] text-muted-foreground/40">{fi.source}</span>
                    <div className="flex gap-1 ml-auto">
                      {["🎉", "👍", "🔥"].map(emoji => (
                        <button key={emoji} onClick={() => handleEmoji(fi.id, emoji)}
                          className="text-xs hover:scale-125 transition-transform">{emoji}</button>
                      ))}
                    </div>
                  </div>
                </div>
              </div>
            </motion.div>
          ))}
          {/* Pagination */}
          <div className="flex justify-center gap-2 pt-2">
            <button onClick={() => setPage(p => Math.max(1, p - 1))} disabled={page === 1}
              className="px-3 py-1 rounded-lg bg-muted/20 text-xs text-muted-foreground hover:text-foreground disabled:opacity-40">← Prev</button>
            <span className="text-xs text-muted-foreground px-2 py-1">Page {page}</span>
            <button onClick={() => setPage(p => p + 1)} disabled={feedItems.length < 20}
              className="px-3 py-1 rounded-lg bg-muted/20 text-xs text-muted-foreground hover:text-foreground disabled:opacity-40">Next →</button>
          </div>
        </div>
      )}
    </motion.div>
  );
};

export default OmniFeedPanel;

