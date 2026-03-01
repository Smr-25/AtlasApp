import { useState } from "react";
import { motion } from "framer-motion";
import { Ruler, Image, Palette, Type, Loader2 } from "lucide-react";
import { zeplinApi, ZeplinScreenDto, ZeplinStyleGuideDto, IntegrationDto } from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

interface Props { integrations: IntegrationDto[]; }

const ZeplinPanel = ({ integrations }: Props) => {
  const zeplinInt = integrations.find(i => i.provider === "Zeplin" && i.status === "Active");
  const [projectId, setProjectId] = useState("");
  const [screens, setScreens] = useState<ZeplinScreenDto[]>([]);
  const [styleGuide, setStyleGuide] = useState<ZeplinStyleGuideDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [activeView, setActiveView] = useState<"screens" | "styleguide">("screens");

  const load = async () => {
    if (!zeplinInt || !projectId.trim()) return;
    setLoading(true);
    try {
      const [sRes, gRes] = await Promise.allSettled([
        zeplinApi.getScreens(zeplinInt.id, projectId),
        zeplinApi.getStyleGuide(zeplinInt.id, projectId),
      ]);
      if (sRes.status === "fulfilled" && sRes.value.data.isSuccess && sRes.value.data.data) setScreens(sRes.value.data.data);
      if (gRes.status === "fulfilled" && gRes.value.data.isSuccess && gRes.value.data.data) setStyleGuide(gRes.value.data.data);
    } catch { /* ignore */ }
    setLoading(false);
  };

  if (!zeplinInt) {
    return (
      <div className="flex flex-col items-center justify-center py-20 gap-4">
        <div className="w-14 h-14 rounded-2xl bg-cyan-500/10 border border-cyan-500/15 flex items-center justify-center">
          <Ruler className="w-6 h-6 text-cyan-400" />
        </div>
        <div className="text-center">
          <p className="text-sm font-semibold text-foreground">Zeplin not connected</p>
          <p className="text-xs text-muted-foreground mt-1">Connect Zeplin in Integrations for design handoff</p>
        </div>
      </div>
    );
  }

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-cyan-500/20 to-cyan-500/5 border border-cyan-500/10 flex items-center justify-center">
          <Ruler className="w-5 h-5 text-cyan-400" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">Zeplin</h2>
          <p className="text-xs text-muted-foreground">Design handoff & style guides</p>
        </div>
      </motion.div>

      {/* Project ID Input */}
      <motion.div variants={item} className="flex items-end gap-3">
        <div className="flex-1 space-y-1">
          <label className="text-xs text-muted-foreground">Zeplin Project ID</label>
          <input type="text" value={projectId} onChange={e => setProjectId(e.target.value)} placeholder="e.g., proj-1"
            className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground placeholder:text-muted-foreground/40"
            onKeyDown={e => e.key === "Enter" && load()} />
        </div>
        <button onClick={load} disabled={loading || !projectId.trim()}
          className="h-9 px-4 rounded-xl bg-primary text-primary-foreground text-xs font-semibold hover:bg-primary/90 disabled:opacity-50 transition-colors">
          {loading ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : "Load Project"}
        </button>
      </motion.div>

      {/* View Toggle */}
      {(screens.length > 0 || styleGuide) && (
        <motion.div variants={item} className="flex items-center gap-1 bg-muted/20 p-1 rounded-xl w-fit">
          {(["screens", "styleguide"] as const).map(v => (
            <button key={v} onClick={() => setActiveView(v)}
              className={`px-4 py-1.5 rounded-lg text-xs font-medium transition-all ${activeView === v ? "bg-card text-foreground shadow-sm" : "text-muted-foreground hover:text-foreground"}`}>
              {v === "screens" ? "Screens" : "Style Guide"}
            </button>
          ))}
        </motion.div>
      )}

      {/* Screens Grid */}
      {activeView === "screens" && screens.length > 0 && (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
          {screens.map((s) => (
            <motion.div key={s.id} variants={item}
              className="rounded-2xl bg-card/50 border border-border/20 overflow-hidden hover:border-cyan-500/20 hover:shadow-lg transition-all group">
              <div className="aspect-[16/10] bg-muted/10 overflow-hidden">
                <img src={s.imageUrl} alt={s.name} className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500" loading="lazy" />
              </div>
              <div className="p-3">
                <p className="text-sm font-medium text-foreground truncate">{s.name}</p>
                <div className="flex items-center gap-2 mt-1 text-xs text-muted-foreground">
                  <Image className="w-3 h-3" />
                  <span>{s.width} × {s.height}</span>
                  <span className="ml-auto">{new Date(s.updatedAt).toLocaleDateString()}</span>
                </div>
              </div>
            </motion.div>
          ))}
        </div>
      )}

      {/* Style Guide */}
      {activeView === "styleguide" && styleGuide && (
        <div className="space-y-5">
          {/* Colors */}
          {styleGuide.colors && styleGuide.colors.length > 0 && (
            <motion.div variants={item} className="p-4 rounded-2xl bg-card/50 border border-border/20">
              <div className="flex items-center gap-2 mb-3">
                <Palette className="w-4 h-4 text-primary" />
                <h3 className="text-xs font-bold text-foreground">Colors</h3>
              </div>
              <div className="grid grid-cols-2 sm:grid-cols-4 lg:grid-cols-6 gap-2">
                {styleGuide.colors.map((c, i) => (
                  <div key={i} className="text-center">
                    <div className="w-full aspect-square rounded-xl border border-border/20" style={{ backgroundColor: c.hexCode, opacity: c.opacity }} />
                    <p className="text-[10px] font-medium text-foreground mt-1.5">{c.name}</p>
                    <p className="text-[10px] font-mono text-muted-foreground">{c.hexCode}</p>
                  </div>
                ))}
              </div>
            </motion.div>
          )}

          {/* Fonts */}
          {styleGuide.fonts && styleGuide.fonts.length > 0 && (
            <motion.div variants={item} className="p-4 rounded-2xl bg-card/50 border border-border/20">
              <div className="flex items-center gap-2 mb-3">
                <Type className="w-4 h-4 text-primary" />
                <h3 className="text-xs font-bold text-foreground">Typography</h3>
              </div>
              <div className="space-y-2">
                {styleGuide.fonts.map((f, i) => (
                  <div key={i} className="flex items-center gap-4 px-3 py-2 rounded-xl bg-muted/10 border border-border/10">
                    <span className="text-sm text-foreground" style={{ fontFamily: f.family, fontSize: Math.min(f.size, 24), fontWeight: f.weight === "Bold" ? 700 : 400 }}>
                      Aa
                    </span>
                    <div className="flex-1">
                      <p className="text-xs font-medium text-foreground">{f.family}</p>
                      <p className="text-[10px] text-muted-foreground">{f.weight} — {f.size}px</p>
                    </div>
                  </div>
                ))}
              </div>
            </motion.div>
          )}

          {/* Spacings */}
          {styleGuide.spacings && styleGuide.spacings.length > 0 && (
            <motion.div variants={item} className="p-4 rounded-2xl bg-card/50 border border-border/20">
              <div className="flex items-center gap-2 mb-3">
                <Ruler className="w-4 h-4 text-primary" />
                <h3 className="text-xs font-bold text-foreground">Spacing</h3>
              </div>
              <div className="flex items-end gap-3">
                {styleGuide.spacings.map((s, i) => (
                  <div key={i} className="text-center">
                    <div className="bg-primary/15 rounded" style={{ width: Math.max(s.value, 8), height: Math.max(s.value, 8), maxWidth: 64, maxHeight: 64 }} />
                    <p className="text-[10px] font-medium text-foreground mt-1">{s.name}</p>
                    <p className="text-[10px] font-mono text-muted-foreground">{s.value}px</p>
                  </div>
                ))}
              </div>
            </motion.div>
          )}
        </div>
      )}
    </motion.div>
  );
};

export default ZeplinPanel;

