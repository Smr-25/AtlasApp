import { useState } from "react";
import { motion } from "framer-motion";
import { Sparkles, Search, Heart, Download, Loader2, ExternalLink } from "lucide-react";
import { lottieFilesApi, LottieAnimDto, IntegrationDto } from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

interface Props { integrations: IntegrationDto[]; }

const LottiePanel = ({ integrations }: Props) => {
  const lottieInt = integrations.find(i => i.provider === "LottieFiles" && i.status === "Active");
  const [query, setQuery] = useState("");
  const [results, setResults] = useState<LottieAnimDto[]>([]);
  const [loading, setLoading] = useState(false);

  const search = async () => {
    if (!lottieInt || !query.trim()) return;
    setLoading(true);
    try {
      const res = await lottieFilesApi.search(lottieInt.id, query);
      if (res.data.isSuccess && res.data.data) setResults(res.data.data);
    } catch { /* ignore */ }
    setLoading(false);
  };

  if (!lottieInt) {
    return (
      <div className="flex flex-col items-center justify-center py-20 gap-4">
        <div className="w-14 h-14 rounded-2xl bg-emerald-500/10 border border-emerald-500/15 flex items-center justify-center">
          <Sparkles className="w-6 h-6 text-emerald-400" />
        </div>
        <div className="text-center">
          <p className="text-sm font-semibold text-foreground">LottieFiles not connected</p>
          <p className="text-xs text-muted-foreground mt-1">Connect LottieFiles in Integrations to search animations</p>
        </div>
      </div>
    );
  }

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-emerald-500/20 to-emerald-500/5 border border-emerald-500/10 flex items-center justify-center">
          <Sparkles className="w-5 h-5 text-emerald-400" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">LottieFiles</h2>
          <p className="text-xs text-muted-foreground">Search & discover animations</p>
        </div>
      </motion.div>

      {/* Search */}
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="flex-1 relative">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
          <input type="text" value={query} onChange={e => setQuery(e.target.value)} placeholder="Search animations... (e.g., loading, success)"
            className="w-full h-10 pl-10 pr-4 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground placeholder:text-muted-foreground/40"
            onKeyDown={e => e.key === "Enter" && search()} />
        </div>
        <button onClick={search} disabled={loading || !query.trim()}
          className="h-10 px-5 rounded-xl bg-primary text-primary-foreground text-xs font-semibold hover:bg-primary/90 disabled:opacity-50 transition-colors">
          {loading ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : "Search"}
        </button>
      </motion.div>

      {/* Results Grid */}
      {results.length > 0 && (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
          {results.map((anim) => (
            <motion.div key={anim.id} variants={item}
              className="rounded-2xl bg-card/50 border border-border/20 overflow-hidden hover:border-emerald-500/20 hover:shadow-lg transition-all group">
              <div className="aspect-square bg-muted/10 flex items-center justify-center p-4">
                <img src={anim.previewUrl} alt={anim.name} className="max-w-full max-h-full object-contain" loading="lazy" />
              </div>
              <div className="p-3">
                <p className="text-sm font-medium text-foreground truncate">{anim.name}</p>
                <p className="text-xs text-muted-foreground mt-0.5">by {anim.authorName}</p>
                <div className="flex items-center justify-between mt-2">
                  <span className="flex items-center gap-1 text-xs text-muted-foreground"><Heart className="w-3 h-3" />{anim.likesCount}</span>
                  <a href={anim.downloadUrl} target="_blank" rel="noopener noreferrer"
                    className="flex items-center gap-1 text-xs text-primary hover:underline">
                    <Download className="w-3 h-3" /> Download
                  </a>
                </div>
              </div>
            </motion.div>
          ))}
        </div>
      )}

      {!loading && results.length === 0 && query && (
        <div className="text-center py-8">
          <Sparkles className="w-8 h-8 text-muted-foreground/20 mx-auto mb-2" />
          <p className="text-xs text-muted-foreground">No animations found. Try a different search.</p>
        </div>
      )}
    </motion.div>
  );
};

export default LottiePanel;

