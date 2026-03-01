import { useState } from "react";
import { motion } from "framer-motion";
import {
  Wrench, ImageDown, FileCode2, Eye, Palette, Ruler,
  Copy, Check, Loader2, Download,
} from "lucide-react";
import { designUtilitiesApi, ContrastCheckDto, AspectRatioDto, PaletteDto } from "@/services/api";
import { useEffect } from "react";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

type ActiveTool = "contrast" | "aspect" | "svg" | "css" | "palettes" | null;

const DesignUtilitiesPanel = () => {
  const [activeTool, setActiveTool] = useState<ActiveTool>(null);

  const tools = [
    { id: "contrast" as const, label: "Contrast Check", desc: "WCAG AA/AAA compliance", icon: Eye, color: "text-violet-400", gradient: "from-violet-500/12 to-violet-500/3" },
    { id: "aspect" as const, label: "Aspect Ratio", desc: "Calculate proportions", icon: Ruler, color: "text-cyan-400", gradient: "from-cyan-500/12 to-cyan-500/3" },
    { id: "svg" as const, label: "Optimize SVG", desc: "Clean & compress SVGs", icon: FileCode2, color: "text-emerald-400", gradient: "from-emerald-500/12 to-emerald-500/3" },
    { id: "css" as const, label: "Extract CSS", desc: "Colors → CSS variables", icon: Palette, color: "text-pink-400", gradient: "from-pink-500/12 to-pink-500/3" },
    { id: "palettes" as const, label: "My Palettes", desc: "Save & manage color palettes", icon: Palette, color: "text-amber-400", gradient: "from-amber-500/12 to-amber-500/3" },
  ];

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-primary/20 to-primary/5 border border-primary/10 flex items-center justify-center">
          <Wrench className="w-5 h-5 text-primary" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">Design Utilities</h2>
          <p className="text-xs text-muted-foreground">Everyday tools for designers</p>
        </div>
      </motion.div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
        {tools.map((t) => (
          <motion.button key={t.id} variants={item} whileHover={{ y: -2 }} whileTap={{ scale: 0.98 }}
            onClick={() => setActiveTool(activeTool === t.id ? null : t.id)}
            className={`group p-4 rounded-2xl bg-gradient-to-br ${t.gradient} border text-left transition-all duration-200 ${activeTool === t.id ? "border-primary/30 shadow-lg shadow-primary/5" : "border-border/20 hover:border-primary/15"}`}>
            <div className="flex items-center gap-3 mb-2">
              <t.icon className={`w-5 h-5 ${t.color}`} />
              <span className="text-sm font-semibold text-foreground">{t.label}</span>
            </div>
            <p className="text-xs text-muted-foreground">{t.desc}</p>
          </motion.button>
        ))}
      </div>

      {activeTool === "contrast" && <ContrastTool />}
      {activeTool === "aspect" && <AspectRatioTool />}
      {activeTool === "svg" && <SvgOptimizeTool />}
      {activeTool === "css" && <CssExtractTool />}
      {activeTool === "palettes" && <PalettesTool />}
    </motion.div>
  );
};

// ─── Contrast Check ────────────────────────────────────────────
const ContrastTool = () => {
  const [fg, setFg] = useState("#FFFFFF");
  const [bg, setBg] = useState("#007AFF");
  const [result, setResult] = useState<ContrastCheckDto | null>(null);
  const [loading, setLoading] = useState(false);

  const check = async () => {
    setLoading(true);
    try {
      const res = await designUtilitiesApi.checkContrast(fg, bg);
      if (res.data.isSuccess) setResult(res.data.data);
    } catch { /* ignore */ }
    setLoading(false);
  };

  return (
    <motion.div initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} className="p-5 rounded-2xl bg-card/50 border border-border/30 space-y-4">
      <h3 className="text-sm font-bold text-foreground">WCAG Contrast Checker</h3>
      <div className="flex items-center gap-4">
        <div className="flex-1 space-y-2">
          <label className="text-xs text-muted-foreground">Foreground</label>
          <div className="flex items-center gap-2">
            <input type="color" value={fg} onChange={e => setFg(e.target.value)} className="w-8 h-8 rounded-lg cursor-pointer border-0" />
            <input type="text" value={fg} onChange={e => setFg(e.target.value)} className="flex-1 h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground font-mono" />
          </div>
        </div>
        <div className="flex-1 space-y-2">
          <label className="text-xs text-muted-foreground">Background</label>
          <div className="flex items-center gap-2">
            <input type="color" value={bg} onChange={e => setBg(e.target.value)} className="w-8 h-8 rounded-lg cursor-pointer border-0" />
            <input type="text" value={bg} onChange={e => setBg(e.target.value)} className="flex-1 h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground font-mono" />
          </div>
        </div>
      </div>
      <div className="flex items-center gap-3">
        <div className="h-16 flex-1 rounded-xl flex items-center justify-center text-sm font-bold" style={{ backgroundColor: bg, color: fg }}>
          Preview Text
        </div>
        <button onClick={check} disabled={loading}
          className="h-9 px-4 rounded-xl bg-primary text-primary-foreground text-xs font-semibold hover:bg-primary/90 disabled:opacity-50 transition-colors">
          {loading ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : "Check"}
        </button>
      </div>
      {result && (
        <div className="flex items-center gap-4 p-3 rounded-xl bg-muted/20 border border-border/20">
          <div className="text-center">
            <p className="text-xl font-bold text-foreground">{result.ratio.toFixed(2)}</p>
            <p className="text-[10px] text-muted-foreground">Ratio</p>
          </div>
          <div className="h-8 w-px bg-border/30" />
          <div className={`px-3 py-1 rounded-lg text-xs font-bold ${result.passesAA ? "bg-emerald-500/15 text-emerald-400" : "bg-red-500/15 text-red-400"}`}>
            AA: {result.passesAA ? "Pass ✓" : "Fail ✗"}
          </div>
          <div className={`px-3 py-1 rounded-lg text-xs font-bold ${result.passesAAA ? "bg-emerald-500/15 text-emerald-400" : "bg-red-500/15 text-red-400"}`}>
            AAA: {result.passesAAA ? "Pass ✓" : "Fail ✗"}
          </div>
        </div>
      )}
    </motion.div>
  );
};

// ─── Aspect Ratio ──────────────────────────────────────────────
const AspectRatioTool = () => {
  const [w, setW] = useState("1920");
  const [h, setH] = useState("1080");
  const [result, setResult] = useState<AspectRatioDto | null>(null);

  const calc = async () => {
    try {
      const res = await designUtilitiesApi.aspectRatio(Number(w), Number(h));
      if (res.data.isSuccess) setResult(res.data.data);
    } catch { /* ignore */ }
  };

  return (
    <motion.div initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} className="p-5 rounded-2xl bg-card/50 border border-border/30 space-y-4">
      <h3 className="text-sm font-bold text-foreground">Aspect Ratio Calculator</h3>
      <div className="flex items-end gap-3">
        <div className="flex-1 space-y-1">
          <label className="text-xs text-muted-foreground">Width</label>
          <input type="number" value={w} onChange={e => setW(e.target.value)} className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground" />
        </div>
        <span className="text-muted-foreground text-lg pb-1">×</span>
        <div className="flex-1 space-y-1">
          <label className="text-xs text-muted-foreground">Height</label>
          <input type="number" value={h} onChange={e => setH(e.target.value)} className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground" />
        </div>
        <button onClick={calc} className="h-9 px-4 rounded-xl bg-primary text-primary-foreground text-xs font-semibold hover:bg-primary/90 transition-colors">
          Calculate
        </button>
      </div>
      {result && (
        <div className="flex items-center gap-4 p-4 rounded-xl bg-muted/20 border border-border/20">
          <div className="w-20 h-14 rounded-lg bg-primary/10 border border-primary/20 flex items-center justify-center">
            <span className="text-xs font-bold text-primary">{result.simplifiedWidth}:{result.simplifiedHeight}</span>
          </div>
          <div>
            <p className="text-lg font-bold text-foreground">{result.ratio}</p>
            <p className="text-xs text-muted-foreground">{w} × {h} pixels</p>
          </div>
        </div>
      )}
    </motion.div>
  );
};

// ─── SVG Optimize ──────────────────────────────────────────────
const SvgOptimizeTool = () => {
  const [svg, setSvg] = useState("");
  const [optimized, setOptimized] = useState<string | null>(null);
  const [savings, setSavings] = useState<{ original: number; optimized: number } | null>(null);
  const [loading, setLoading] = useState(false);
  const [copied, setCopied] = useState(false);

  const optimize = async () => {
    if (!svg.trim()) return;
    setLoading(true);
    try {
      const res = await designUtilitiesApi.optimizeSvg(svg);
      if (res.data.isSuccess && res.data.data) {
        setOptimized(res.data.data.optimizedSvg);
        setSavings({ original: res.data.data.originalLength, optimized: res.data.data.optimizedLength });
      }
    } catch { /* ignore */ }
    setLoading(false);
  };

  const copyResult = () => {
    if (optimized) { navigator.clipboard.writeText(optimized); setCopied(true); setTimeout(() => setCopied(false), 2000); }
  };

  return (
    <motion.div initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} className="p-5 rounded-2xl bg-card/50 border border-border/30 space-y-4">
      <h3 className="text-sm font-bold text-foreground">SVG Optimizer</h3>
      <textarea value={svg} onChange={e => setSvg(e.target.value)} placeholder="Paste your SVG code here..." rows={5}
        className="w-full px-4 py-3 rounded-xl bg-muted/30 border border-border/30 text-sm font-mono text-foreground placeholder:text-muted-foreground/40 resize-none" />
      <div className="flex items-center gap-3">
        <button onClick={optimize} disabled={loading || !svg.trim()}
          className="h-9 px-4 rounded-xl bg-primary text-primary-foreground text-xs font-semibold hover:bg-primary/90 disabled:opacity-50 transition-colors">
          {loading ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : "Optimize"}
        </button>
        {savings && (
          <span className="text-xs text-emerald-400 font-medium">
            {Math.round((1 - savings.optimized / savings.original) * 100)}% smaller ({savings.original} → {savings.optimized} chars)
          </span>
        )}
      </div>
      {optimized && (
        <div className="relative">
          <pre className="p-4 rounded-xl bg-muted/20 border border-border/20 text-xs font-mono text-foreground overflow-x-auto max-h-40">{optimized}</pre>
          <button onClick={copyResult} className="absolute top-2 right-2 w-7 h-7 rounded-lg bg-card border border-border/30 flex items-center justify-center text-muted-foreground hover:text-foreground transition-colors">
            {copied ? <Check className="w-3 h-3 text-emerald-400" /> : <Copy className="w-3 h-3" />}
          </button>
        </div>
      )}
    </motion.div>
  );
};

// ─── CSS Extract ───────────────────────────────────────────────
const CssExtractTool = () => {
  const [colors, setColors] = useState([{ name: "primary", hexCode: "#007AFF" }, { name: "danger", hexCode: "#FF3B30" }]);
  const [format, setFormat] = useState<"css" | "scss" | "less">("css");
  const [result, setResult] = useState<string | null>(null);
  const [copied, setCopied] = useState(false);

  const extract = async () => {
    try {
      const res = await designUtilitiesApi.extractCss(colors, format);
      if (res.data.isSuccess && res.data.data) setResult(res.data.data.css);
    } catch { /* ignore */ }
  };

  const addColor = () => setColors([...colors, { name: `color-${colors.length + 1}`, hexCode: "#000000" }]);

  return (
    <motion.div initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} className="p-5 rounded-2xl bg-card/50 border border-border/30 space-y-4">
      <h3 className="text-sm font-bold text-foreground">Extract CSS Variables</h3>
      <div className="space-y-2">
        {colors.map((c, i) => (
          <div key={i} className="flex items-center gap-2">
            <input type="color" value={c.hexCode} onChange={e => { const n = [...colors]; n[i].hexCode = e.target.value; setColors(n); }} className="w-8 h-8 rounded-lg cursor-pointer border-0" />
            <input type="text" value={c.name} onChange={e => { const n = [...colors]; n[i].name = e.target.value; setColors(n); }}
              className="flex-1 h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground" placeholder="Variable name" />
            <span className="text-xs font-mono text-muted-foreground w-20">{c.hexCode}</span>
          </div>
        ))}
      </div>
      <div className="flex items-center gap-2">
        <button onClick={addColor} className="text-xs text-primary hover:underline">+ Add Color</button>
        <div className="flex-1" />
        <select value={format} onChange={e => setFormat(e.target.value as any)} className="h-8 px-2 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground">
          <option value="css">CSS</option><option value="scss">SCSS</option><option value="less">LESS</option>
        </select>
        <button onClick={extract} className="h-8 px-4 rounded-lg bg-primary text-primary-foreground text-xs font-semibold hover:bg-primary/90 transition-colors">Generate</button>
      </div>
      {result && (
        <div className="relative">
          <pre className="p-4 rounded-xl bg-muted/20 border border-border/20 text-xs font-mono text-foreground">{result}</pre>
          <button onClick={() => { navigator.clipboard.writeText(result); setCopied(true); setTimeout(() => setCopied(false), 2000); }}
            className="absolute top-2 right-2 w-7 h-7 rounded-lg bg-card border border-border/30 flex items-center justify-center text-muted-foreground hover:text-foreground transition-colors">
            {copied ? <Check className="w-3 h-3 text-emerald-400" /> : <Copy className="w-3 h-3" />}
          </button>
        </div>
      )}
    </motion.div>
  );
};

// ─── Palettes ──────────────────────────────────────────────────
const PalettesTool = () => {
  const [palettes, setPalettes] = useState<PaletteDto[]>([]);
  const [newName, setNewName] = useState("");
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    designUtilitiesApi.getPalettes().then(r => {
      if (r.data.isSuccess && r.data.data) setPalettes(r.data.data);
    }).catch(() => {}).finally(() => setLoading(false));
  }, []);

  const createPalette = async () => {
    if (!newName.trim()) return;
    try {
      const res = await designUtilitiesApi.createPalette(newName);
      if (res.data.isSuccess) {
        setPalettes([...palettes, { id: res.data.data as unknown as string, name: newName, colors: [] }]);
        setNewName("");
      }
    } catch { /* ignore */ }
  };

  return (
    <motion.div initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} className="p-5 rounded-2xl bg-card/50 border border-border/30 space-y-4">
      <h3 className="text-sm font-bold text-foreground">My Color Palettes</h3>
      <div className="flex items-center gap-2">
        <input type="text" value={newName} onChange={e => setNewName(e.target.value)} placeholder="New palette name..."
          className="flex-1 h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground placeholder:text-muted-foreground/40"
          onKeyDown={e => e.key === "Enter" && createPalette()} />
        <button onClick={createPalette} className="h-9 px-4 rounded-xl bg-primary text-primary-foreground text-xs font-semibold hover:bg-primary/90 transition-colors">Create</button>
      </div>
      {loading ? (
        <div className="flex items-center justify-center py-8"><Loader2 className="w-5 h-5 animate-spin text-muted-foreground" /></div>
      ) : palettes.length > 0 ? (
        <div className="space-y-3">
          {palettes.map(p => (
            <div key={p.id} className="p-3 rounded-xl bg-muted/15 border border-border/15">
              <p className="text-xs font-semibold text-foreground mb-2">{p.name}</p>
              <div className="flex items-center gap-1.5">
                {p.colors?.length > 0 ? p.colors.map((c, i) => (
                  <div key={i} className="w-8 h-8 rounded-lg border border-border/20 cursor-pointer hover:scale-110 transition-transform" style={{ backgroundColor: c.hexCode }} title={`${c.name}: ${c.hexCode}`} />
                )) : <p className="text-[10px] text-muted-foreground/50">No colors yet</p>}
              </div>
            </div>
          ))}
        </div>
      ) : (
        <p className="text-xs text-muted-foreground text-center py-6">No palettes yet. Create one above!</p>
      )}
    </motion.div>
  );
};

export default DesignUtilitiesPanel;

