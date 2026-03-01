import { useState } from "react";
import { motion } from "framer-motion";
import { Eye, Search, Heart, ExternalLink, Loader2 } from "lucide-react";
import { dribbbleApi, DribbbleShotDto, IntegrationDto } from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

interface Props { integrations: IntegrationDto[]; }

const DribbblePanel = ({ integrations }: Props) => {
  const dribbbleInt = integrations.find(i => i.provider === "Dribbble" && i.status === "Active");
  const [query, setQuery] = useState("");
  const [shots, setShots] = useState<DribbbleShotDto[]>([]);
  const [loading, setLoading] = useState(false);

  const search = async () => {
    if (!dribbbleInt || !query.trim()) return;
    setLoading(true);
    try {
      const res = await dribbbleApi.inspiration(dribbbleInt.id, query);
      if (res.data.isSuccess && res.data.data) setShots(res.data.data);
    } catch { /* ignore */ }
    setLoading(false);
  };

  if (!dribbbleInt) {
    return (
      <div className="flex flex-col items-center justify-center py-20 gap-4">
        <div className="w-14 h-14 rounded-2xl bg-pink-500/10 border border-pink-500/15 flex items-center justify-center">
          <Eye className="w-6 h-6 text-pink-400" />
        </div>
        <div className="text-center">
          <p className="text-sm font-semibold text-foreground">Dribbble not connected</p>
          <p className="text-xs text-muted-foreground mt-1">Connect Dribbble in Integrations to find inspiration</p>
        </div>
      </div>
    );
  }

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-pink-500/20 to-pink-500/5 border border-pink-500/10 flex items-center justify-center">
          <Eye className="w-5 h-5 text-pink-400" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">Dribbble</h2>
          <p className="text-xs text-muted-foreground">Design inspiration & trends</p>
        </div>
      </motion.div>

      {/* Search */}
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="flex-1 relative">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
          <input type="text" value={query} onChange={e => setQuery(e.target.value)} placeholder="Search shots... (e.g., dashboard, mobile)"
            className="w-full h-10 pl-10 pr-4 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground placeholder:text-muted-foreground/40"
            onKeyDown={e => e.key === "Enter" && search()} />
        </div>
        <button onClick={search} disabled={loading || !query.trim()}
          className="h-10 px-5 rounded-xl bg-primary text-primary-foreground text-xs font-semibold hover:bg-primary/90 disabled:opacity-50 transition-colors">
          {loading ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : "Search"}
        </button>
      </motion.div>

      {/* Results Grid */}
      {shots.length > 0 && (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {shots.map((shot) => (
            <motion.a key={shot.id} href={shot.htmlUrl} target="_blank" rel="noopener noreferrer" variants={item}
              className="rounded-2xl bg-card/50 border border-border/20 overflow-hidden hover:border-pink-500/20 hover:shadow-xl transition-all group block">
              <div className="aspect-[4/3] bg-muted/10 overflow-hidden">
                <img src={shot.imageUrl} alt={shot.title} className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500" loading="lazy" />
              </div>
              <div className="p-4">
                <p className="text-sm font-semibold text-foreground truncate">{shot.title}</p>
                <div className="flex items-center gap-2 mt-2">
                  {shot.authorAvatarUrl && <img src={shot.authorAvatarUrl} alt="" className="w-5 h-5 rounded-full" />}
                  <span className="text-xs text-muted-foreground">{shot.authorName}</span>
                </div>
                <div className="flex items-center gap-4 mt-3 text-xs text-muted-foreground">
                  <span className="flex items-center gap-1"><Heart className="w-3 h-3 text-pink-400" />{shot.likesCount.toLocaleString()}</span>
                  <span className="flex items-center gap-1"><Eye className="w-3 h-3" />{shot.viewsCount.toLocaleString()}</span>
                  <ExternalLink className="w-3 h-3 ml-auto text-muted-foreground/30 group-hover:text-primary transition-colors" />
                </div>
              </div>
            </motion.a>
          ))}
        </div>
      )}

      {!loading && shots.length === 0 && query && (
        <div className="text-center py-8">
          <Eye className="w-8 h-8 text-muted-foreground/20 mx-auto mb-2" />
          <p className="text-xs text-muted-foreground">No shots found. Try a different query.</p>
        </div>
      )}
    </motion.div>
  );
};

export default DribbblePanel;

