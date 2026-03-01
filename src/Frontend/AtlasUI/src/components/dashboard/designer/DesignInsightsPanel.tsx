import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import { BarChart3, ImageDown, Layers, Palette, PenTool } from "lucide-react";
import { designInsightsApi, AssetsOptimizedDto, HandoffsDto, DesignDebtDto } from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.05 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

const DesignInsightsPanel = () => {
  const [assets, setAssets] = useState<AssetsOptimizedDto | null>(null);
  const [handoffs, setHandoffs] = useState<HandoffsDto | null>(null);
  const [colorTrends, setColorTrends] = useState<Record<string, number> | null>(null);
  const [debt, setDebt] = useState<DesignDebtDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = async () => {
      const [aR, hR, cR, dR] = await Promise.allSettled([
        designInsightsApi.assetsOptimized(),
        designInsightsApi.handoffs(),
        designInsightsApi.colorTrends(),
        designInsightsApi.designDebt(),
      ]);
      if (aR.status === "fulfilled" && aR.value.data.isSuccess) setAssets(aR.value.data.data);
      if (hR.status === "fulfilled" && hR.value.data.isSuccess) setHandoffs(hR.value.data.data);
      if (cR.status === "fulfilled" && cR.value.data.isSuccess) setColorTrends(cR.value.data.data);
      if (dR.status === "fulfilled" && dR.value.data.isSuccess) setDebt(dR.value.data.data);
      setLoading(false);
    };
    load();
  }, []);

  if (loading) {
    return (
      <div className="flex items-center justify-center py-20">
        <div className="flex flex-col items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center animate-pulse">
            <BarChart3 className="w-5 h-5 text-primary" />
          </div>
          <p className="text-xs text-muted-foreground">Loading design insights...</p>
        </div>
      </div>
    );
  }

  const colorEntries = colorTrends ? Object.entries(colorTrends).sort((a, b) => b[1] - a[1]) : [];
  const maxColorCount = colorEntries.length > 0 ? colorEntries[0][1] : 1;

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-primary/20 to-primary/5 border border-primary/10 flex items-center justify-center">
          <BarChart3 className="w-5 h-5 text-primary" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">Design Insights</h2>
          <p className="text-xs text-muted-foreground">Track your design productivity and quality metrics</p>
        </div>
      </motion.div>

      {/* Summary Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        {[
          { label: "Assets Optimized", value: assets?.totalOptimized?.toString() || "0", sub: `${assets?.totalSavedMb?.toFixed(1) || 0} MB saved`, icon: ImageDown, gradient: "from-pink-500/15 to-rose-500/5", color: "text-pink-400" },
          { label: "Handoffs Completed", value: handoffs?.count?.toString() || "0", sub: "designs delivered", icon: Layers, gradient: "from-violet-500/15 to-purple-500/5", color: "text-violet-400" },
          { label: "Design Debt", value: debt?.count?.toString() || "0", sub: "inconsistencies", icon: PenTool, gradient: "from-amber-500/15 to-yellow-500/5", color: "text-amber-400" },
        ].map((s, i) => (
          <motion.div key={i} variants={item}
            className={`p-5 rounded-2xl bg-gradient-to-br ${s.gradient} border border-border/20`}>
            <div className="flex items-center gap-2 mb-2">
              <s.icon className={`w-4 h-4 ${s.color}`} />
              <span className="text-xs font-medium text-muted-foreground">{s.label}</span>
            </div>
            <p className="text-2xl font-bold text-foreground">{s.value}</p>
            <p className="text-xs text-muted-foreground mt-1">{s.sub}</p>
          </motion.div>
        ))}
      </div>

      {/* Color Trends */}
      <motion.div variants={item} className="p-5 rounded-2xl bg-card/50 border border-border/30">
        <div className="flex items-center gap-2 mb-4">
          <Palette className="w-4 h-4 text-primary" />
          <h3 className="text-sm font-bold text-foreground">Color Trends</h3>
        </div>
        {colorEntries.length > 0 ? (
          <div className="space-y-3">
            {colorEntries.slice(0, 8).map(([hex, count]) => (
              <div key={hex} className="flex items-center gap-3">
                <div className="w-6 h-6 rounded-lg border border-border/30 shrink-0" style={{ backgroundColor: hex }} />
                <span className="text-xs font-mono text-muted-foreground w-20 shrink-0">{hex}</span>
                <div className="flex-1 h-2 bg-muted/30 rounded-full overflow-hidden">
                  <motion.div initial={{ width: 0 }} animate={{ width: `${(count / maxColorCount) * 100}%` }}
                    transition={{ duration: 0.6, delay: 0.2 }}
                    className="h-full rounded-full" style={{ backgroundColor: hex, opacity: 0.7 }} />
                </div>
                <span className="text-xs text-muted-foreground w-10 text-right">{count}</span>
              </div>
            ))}
          </div>
        ) : (
          <p className="text-xs text-muted-foreground text-center py-6">No color data available yet</p>
        )}
      </motion.div>
    </motion.div>
  );
};

export default DesignInsightsPanel;

