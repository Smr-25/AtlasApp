import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import { Layers, ExternalLink, StickyNote, Loader2, Send } from "lucide-react";
import { miroApi, MiroBoardDto, IntegrationDto } from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

interface Props { integrations: IntegrationDto[]; }

const MiroPanel = ({ integrations }: Props) => {
  const miroInt = integrations.find(i => i.provider === "Miro" && i.status === "Active");
  const [boards, setBoards] = useState<MiroBoardDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [stickyContent, setStickyContent] = useState("");
  const [selectedBoard, setSelectedBoard] = useState<string>("");
  const [sending, setSending] = useState(false);

  useEffect(() => {
    if (!miroInt) { setLoading(false); return; }
    miroApi.getBoards(miroInt.id).then(r => {
      if (r.data.isSuccess && r.data.data) { setBoards(r.data.data); if (r.data.data.length > 0) setSelectedBoard(r.data.data[0].id); }
    }).catch(() => {}).finally(() => setLoading(false));
  }, [miroInt]);

  const sendSticky = async () => {
    if (!miroInt || !selectedBoard || !stickyContent.trim()) return;
    setSending(true);
    try {
      await miroApi.createSticky({ integrationId: miroInt.id, boardId: selectedBoard, content: stickyContent });
      setStickyContent("");
    } catch { /* ignore */ }
    setSending(false);
  };

  if (!miroInt) {
    return (
      <div className="flex flex-col items-center justify-center py-20 gap-4">
        <div className="w-14 h-14 rounded-2xl bg-amber-500/10 border border-amber-500/15 flex items-center justify-center">
          <Layers className="w-6 h-6 text-amber-400" />
        </div>
        <div className="text-center">
          <p className="text-sm font-semibold text-foreground">Miro not connected</p>
          <p className="text-xs text-muted-foreground mt-1">Connect Miro in Integrations to see boards</p>
        </div>
      </div>
    );
  }

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-amber-500/20 to-amber-500/5 border border-amber-500/10 flex items-center justify-center">
          <Layers className="w-5 h-5 text-amber-400" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">Miro Boards</h2>
          <p className="text-xs text-muted-foreground">Whiteboard & brainstorming</p>
        </div>
      </motion.div>

      {loading ? (
        <div className="flex items-center justify-center py-12"><Loader2 className="w-5 h-5 animate-spin text-muted-foreground" /></div>
      ) : (
        <>
          {/* Board List */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            {boards.map((b) => (
              <motion.div key={b.id} variants={item}
                className="p-4 rounded-2xl bg-card/50 border border-border/20 hover:border-amber-500/20 hover:shadow-lg transition-all group">
                <div className="flex items-start justify-between mb-3">
                  <div>
                    <p className="text-sm font-semibold text-foreground">{b.name}</p>
                    {b.description && <p className="text-xs text-muted-foreground mt-0.5">{b.description}</p>}
                  </div>
                  <a href={b.viewLink} target="_blank" rel="noopener noreferrer"
                    className="w-7 h-7 rounded-lg bg-muted/20 flex items-center justify-center text-muted-foreground hover:text-primary transition-colors">
                    <ExternalLink className="w-3.5 h-3.5" />
                  </a>
                </div>
                <div className="flex items-center gap-3 text-xs text-muted-foreground">
                  <span className="flex items-center gap-1"><StickyNote className="w-3 h-3" />{b.stickyNoteCount} stickies</span>
                  <span>Updated {new Date(b.modifiedAt).toLocaleDateString()}</span>
                </div>
              </motion.div>
            ))}
          </div>

          {/* Quick Sticky Note */}
          {boards.length > 0 && (
            <motion.div variants={item} className="p-4 rounded-2xl bg-card/50 border border-border/20">
              <h3 className="text-xs font-bold text-foreground mb-3">Quick Sticky Note</h3>
              <div className="flex items-end gap-3">
                <select value={selectedBoard} onChange={e => setSelectedBoard(e.target.value)}
                  className="h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-xs text-foreground">
                  {boards.map(b => <option key={b.id} value={b.id}>{b.name}</option>)}
                </select>
                <input type="text" value={stickyContent} onChange={e => setStickyContent(e.target.value)} placeholder="Type a note..."
                  className="flex-1 h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground placeholder:text-muted-foreground/40"
                  onKeyDown={e => e.key === "Enter" && sendSticky()} />
                <button onClick={sendSticky} disabled={sending || !stickyContent.trim()}
                  className="h-9 w-9 rounded-xl bg-amber-500 text-white flex items-center justify-center hover:bg-amber-500/90 disabled:opacity-50 transition-colors">
                  {sending ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : <Send className="w-3.5 h-3.5" />}
                </button>
              </div>
            </motion.div>
          )}
        </>
      )}
    </motion.div>
  );
};

export default MiroPanel;

