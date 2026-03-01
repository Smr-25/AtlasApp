import { useState } from "react";
import { motion } from "framer-motion";
import { PenTool, MessageCircle, Check, Loader2, ExternalLink } from "lucide-react";
import { figmaApi, FigmaCommentDto, IntegrationDto } from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

interface Props { integrations: IntegrationDto[]; }

const FigmaPanel = ({ integrations }: Props) => {
  const figmaInt = integrations.find(i => i.provider === "Figma" && i.status === "Active");
  const [fileKey, setFileKey] = useState("");
  const [comments, setComments] = useState<FigmaCommentDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [resolving, setResolving] = useState<string | null>(null);

  const loadComments = async () => {
    if (!figmaInt || !fileKey.trim()) return;
    setLoading(true);
    try {
      const res = await figmaApi.getComments(figmaInt.id, fileKey);
      if (res.data.isSuccess && res.data.data) setComments(res.data.data);
    } catch { /* ignore */ }
    setLoading(false);
  };

  const resolveComment = async (commentId: string) => {
    if (!figmaInt) return;
    setResolving(commentId);
    try {
      await figmaApi.resolveComment({ integrationId: figmaInt.id, fileKey, commentId });
      setComments(comments.map(c => c.id === commentId ? { ...c, isResolved: true } : c));
    } catch { /* ignore */ }
    setResolving(null);
  };

  if (!figmaInt) {
    return (
      <div className="flex flex-col items-center justify-center py-20 gap-4">
        <div className="w-14 h-14 rounded-2xl bg-violet-500/10 border border-violet-500/15 flex items-center justify-center">
          <PenTool className="w-6 h-6 text-violet-400" />
        </div>
        <div className="text-center">
          <p className="text-sm font-semibold text-foreground">Figma not connected</p>
          <p className="text-xs text-muted-foreground mt-1">Connect your Figma account in Integrations to see comments</p>
        </div>
      </div>
    );
  }

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-violet-500/20 to-violet-500/5 border border-violet-500/10 flex items-center justify-center">
          <PenTool className="w-5 h-5 text-violet-400" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">Figma</h2>
          <p className="text-xs text-muted-foreground">View and manage file comments</p>
        </div>
      </motion.div>

      {/* File Key Input */}
      <motion.div variants={item} className="flex items-end gap-3">
        <div className="flex-1 space-y-1">
          <label className="text-xs text-muted-foreground">Figma File Key</label>
          <input type="text" value={fileKey} onChange={e => setFileKey(e.target.value)} placeholder="e.g., abc123xyz"
            className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground placeholder:text-muted-foreground/40"
            onKeyDown={e => e.key === "Enter" && loadComments()} />
        </div>
        <button onClick={loadComments} disabled={loading || !fileKey.trim()}
          className="h-9 px-4 rounded-xl bg-primary text-primary-foreground text-xs font-semibold hover:bg-primary/90 disabled:opacity-50 transition-colors">
          {loading ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : "Load Comments"}
        </button>
      </motion.div>

      {/* Comments List */}
      {comments.length > 0 ? (
        <div className="space-y-2">
          {comments.map((c) => (
            <motion.div key={c.id} variants={item}
              className={`p-4 rounded-2xl border transition-all ${c.isResolved ? "bg-emerald-500/5 border-emerald-500/10" : "bg-card/50 border-border/20"}`}>
              <div className="flex items-start gap-3">
                {c.authorAvatarUrl ? (
                  <img src={c.authorAvatarUrl} alt="" className="w-8 h-8 rounded-full" />
                ) : (
                  <div className="w-8 h-8 rounded-full bg-primary/10 flex items-center justify-center">
                    <span className="text-[10px] font-bold text-primary">{c.authorName?.[0]}</span>
                  </div>
                )}
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2">
                    <span className="text-xs font-semibold text-foreground">{c.authorName}</span>
                    <span className="text-[10px] text-muted-foreground/50">{new Date(c.createdAt).toLocaleDateString()}</span>
                    {c.isResolved && <span className="text-[9px] bg-emerald-500/15 text-emerald-400 px-1.5 py-0.5 rounded-md font-semibold">Resolved</span>}
                  </div>
                  <p className="text-sm text-foreground mt-1">{c.message}</p>
                </div>
                {!c.isResolved && (
                  <button onClick={() => resolveComment(c.id)} disabled={resolving === c.id}
                    className="w-8 h-8 rounded-lg bg-emerald-500/10 border border-emerald-500/15 flex items-center justify-center text-emerald-400 hover:bg-emerald-500/20 transition-colors shrink-0">
                    {resolving === c.id ? <Loader2 className="w-3 h-3 animate-spin" /> : <Check className="w-3.5 h-3.5" />}
                  </button>
                )}
              </div>
            </motion.div>
          ))}
        </div>
      ) : !loading && fileKey && (
        <div className="text-center py-8">
          <MessageCircle className="w-8 h-8 text-muted-foreground/20 mx-auto mb-2" />
          <p className="text-xs text-muted-foreground">No comments found for this file</p>
        </div>
      )}
    </motion.div>
  );
};

export default FigmaPanel;

