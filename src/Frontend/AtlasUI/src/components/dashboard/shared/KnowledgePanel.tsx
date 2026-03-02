import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import { BookMarked, Loader2, ExternalLink, FileText } from "lucide-react";
import { knowledgeApi } from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

interface NotionDoc { id: string; title: string; url?: string; lastEdited?: string; icon?: string; }

const KnowledgePanel = () => {
  const [docs, setDocs] = useState<NotionDoc[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    knowledgeApi.getNotionDocs().then(r => {
      if (r.data.isSuccess && r.data.data) setDocs(Array.isArray(r.data.data) ? r.data.data : []);
    }).catch(() => {}).finally(() => setLoading(false));
  }, []);

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-blue-500/20 to-blue-500/5 border border-blue-500/10 flex items-center justify-center">
          <BookMarked className="w-5 h-5 text-blue-400" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">Knowledge Base</h2>
          <p className="text-xs text-muted-foreground">Notion documents & wikis</p>
        </div>
      </motion.div>

      {loading ? (
        <div className="flex justify-center py-12"><Loader2 className="w-5 h-5 animate-spin text-primary" /></div>
      ) : docs.length === 0 ? (
        <motion.div variants={item} className="text-center py-12 rounded-2xl bg-card/30 border border-dashed border-border/30">
          <FileText className="w-8 h-8 text-muted-foreground/40 mx-auto mb-2" />
          <p className="text-xs text-muted-foreground">No knowledge base documents found.</p>
          <p className="text-[10px] text-muted-foreground/50 mt-1">Connect Notion to see your docs here.</p>
        </motion.div>
      ) : (
        <div className="space-y-2">
          {docs.map((d, i) => (
            <motion.a key={d.id || i} variants={item} href={d.url || "#"} target="_blank" rel="noopener"
              className="p-3 rounded-xl bg-card/50 border border-border/15 flex items-center gap-3 hover:border-primary/20 transition-colors group">
              <span className="text-lg">{d.icon || "📄"}</span>
              <div className="flex-1 min-w-0">
                <p className="text-xs font-semibold text-foreground group-hover:text-primary transition-colors">{d.title}</p>
                {d.lastEdited && <p className="text-[10px] text-muted-foreground/60">Last edited: {new Date(d.lastEdited).toLocaleDateString()}</p>}
              </div>
              <ExternalLink className="w-3 h-3 text-muted-foreground/40 group-hover:text-primary shrink-0" />
            </motion.a>
          ))}
        </div>
      )}
    </motion.div>
  );
};

export default KnowledgePanel;



