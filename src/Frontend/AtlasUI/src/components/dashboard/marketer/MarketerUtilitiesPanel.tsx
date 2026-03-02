import { useState } from "react";
import { motion } from "framer-motion";
import {
  Wrench, Search as SearchIcon, PenTool, FileCode, Hash,
  BookOpen, Smile, Loader2, Copy, Check,
} from "lucide-react";
import {
  marketerUtilitiesApi, SeoCheckDto, CopywritingDto,
  KeywordDensityDto, ReadabilityDto, EmojiDto, MdToHtmlDto,
} from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

type ActiveTool = "seo" | "copy" | "md" | "keyword" | "readability" | "emoji" | null;

const MarketerUtilitiesPanel = () => {
  const [activeTool, setActiveTool] = useState<ActiveTool>(null);

  const tools = [
    { id: "seo" as const, label: "SEO Checker", desc: "Meta title & description audit", icon: SearchIcon, color: "text-emerald-400", gradient: "from-emerald-500/12 to-emerald-500/3" },
    { id: "copy" as const, label: "AI Copywriting", desc: "Generate marketing copy", icon: PenTool, color: "text-blue-400", gradient: "from-blue-500/12 to-blue-500/3" },
    { id: "md" as const, label: "Markdown → HTML", desc: "Convert markdown to HTML", icon: FileCode, color: "text-violet-400", gradient: "from-violet-500/12 to-violet-500/3" },
    { id: "keyword" as const, label: "Keyword Density", desc: "Analyze keyword usage", icon: Hash, color: "text-amber-400", gradient: "from-amber-500/12 to-amber-500/3" },
    { id: "readability" as const, label: "Readability", desc: "Flesch score analysis", icon: BookOpen, color: "text-cyan-400", gradient: "from-cyan-500/12 to-cyan-500/3" },
    { id: "emoji" as const, label: "Emoji Search", desc: "Find the perfect emoji", icon: Smile, color: "text-pink-400", gradient: "from-pink-500/12 to-pink-500/3" },
  ];

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-blue-500/20 to-blue-500/5 border border-blue-500/10 flex items-center justify-center">
          <Wrench className="w-5 h-5 text-blue-400" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">Marketing Tools</h2>
          <p className="text-xs text-muted-foreground">Content optimization & creation utilities</p>
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

      {activeTool === "seo" && <SeoTool />}
      {activeTool === "copy" && <CopywritingTool />}
      {activeTool === "md" && <MarkdownTool />}
      {activeTool === "keyword" && <KeywordTool />}
      {activeTool === "readability" && <ReadabilityTool />}
      {activeTool === "emoji" && <EmojiTool />}
    </motion.div>
  );
};

// ─── SEO Checker ───────────────────────────────────────────────
const SeoTool = () => {
  const [title, setTitle] = useState("");
  const [desc, setDesc] = useState("");
  const [url, setUrl] = useState("");
  const [result, setResult] = useState<SeoCheckDto | null>(null);
  const [loading, setLoading] = useState(false);

  const run = async () => {
    if (!title.trim()) return;
    setLoading(true);
    try { const r = await marketerUtilitiesApi.seoCheck({ title, description: desc, url }); if (r.data.isSuccess) setResult(r.data.data); } catch {}
    setLoading(false);
  };

  return (
    <ToolCard title="SEO Meta Checker">
      <div className="space-y-2">
        <Input label="Page Title" value={title} onChange={setTitle} placeholder="Best Running Shoes 2026" />
        <Input label="Meta Description" value={desc} onChange={setDesc} placeholder="Discover the best..." />
        <Input label="URL" value={url} onChange={setUrl} placeholder="https://example.com/page" />
        <RunBtn loading={loading} onClick={run} disabled={!title.trim()} />
      </div>
      {result && (
        <div className="mt-3 space-y-2">
          <div className="flex items-center gap-3 text-xs">
            <span className={result.titleOk ? "text-emerald-400" : "text-red-400"}>{result.titleOk ? "✓" : "✗"} Title ({result.titleLength} chars)</span>
            <span className={result.descriptionOk ? "text-emerald-400" : "text-red-400"}>{result.descriptionOk ? "✓" : "✗"} Description ({result.descriptionLength} chars)</span>
          </div>
          <div className="p-3 rounded-xl bg-muted/15 border border-border/15">
            <p className="text-xs text-muted-foreground mb-1">SERP Preview:</p>
            <pre className="text-xs text-foreground whitespace-pre-wrap">{result.previewSnippet}</pre>
          </div>
        </div>
      )}
    </ToolCard>
  );
};

// ─── AI Copywriting ────────────────────────────────────────────
const CopywritingTool = () => {
  const [product, setProduct] = useState("");
  const [tone, setTone] = useState("professional");
  const [result, setResult] = useState<CopywritingDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [copied, setCopied] = useState(false);

  const run = async () => {
    if (!product.trim()) return;
    setLoading(true);
    try { const r = await marketerUtilitiesApi.copywriting({ productName: product, tone }); if (r.data.isSuccess) setResult(r.data.data); } catch {}
    setLoading(false);
  };

  return (
    <ToolCard title="AI Copywriting">
      <div className="flex items-end gap-3">
        <div className="flex-1">
          <Input label="Product Name" value={product} onChange={setProduct} placeholder="Atlas SaaS Platform" />
        </div>
        <select value={tone} onChange={e => setTone(e.target.value)} className="h-9 px-2 rounded-xl bg-muted/30 border border-border/30 text-xs text-foreground">
          {["professional", "casual", "fun", "urgency"].map(t => <option key={t} value={t}>{t}</option>)}
        </select>
        <RunBtn loading={loading} onClick={run} disabled={!product.trim()} />
      </div>
      {result && (
        <div className="relative mt-3">
          <pre className="p-3 rounded-xl bg-muted/15 border border-border/15 text-sm text-foreground whitespace-pre-wrap">{result.copy}</pre>
          <CopyButton text={result.copy} />
        </div>
      )}
    </ToolCard>
  );
};

// ─── Markdown → HTML ───────────────────────────────────────────
const MarkdownTool = () => {
  const [md, setMd] = useState("");
  const [result, setResult] = useState<MdToHtmlDto | null>(null);
  const [loading, setLoading] = useState(false);

  const run = async () => {
    if (!md.trim()) return;
    setLoading(true);
    try { const r = await marketerUtilitiesApi.markdownToHtml({ markdown: md }); if (r.data.isSuccess) setResult(r.data.data); } catch {}
    setLoading(false);
  };

  return (
    <ToolCard title="Markdown → HTML">
      <textarea value={md} onChange={e => setMd(e.target.value)} rows={4} placeholder="# Heading\n\n**Bold** text..."
        className="w-full px-3 py-2 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground placeholder:text-muted-foreground/40 resize-none" />
      <RunBtn loading={loading} onClick={run} disabled={!md.trim()} />
      {result && (
        <div className="relative mt-3">
          <pre className="p-3 rounded-xl bg-muted/15 border border-border/15 text-xs text-foreground whitespace-pre-wrap break-all">{result.html}</pre>
          <CopyButton text={result.html} />
        </div>
      )}
    </ToolCard>
  );
};

// ─── Keyword Density ───────────────────────────────────────────
const KeywordTool = () => {
  const [content, setContent] = useState("");
  const [keyword, setKeyword] = useState("");
  const [result, setResult] = useState<KeywordDensityDto | null>(null);
  const [loading, setLoading] = useState(false);

  const run = async () => {
    if (!content.trim() || !keyword.trim()) return;
    setLoading(true);
    try { const r = await marketerUtilitiesApi.keywordDensity({ content, keyword }); if (r.data.isSuccess) setResult(r.data.data); } catch {}
    setLoading(false);
  };

  return (
    <ToolCard title="Keyword Density Analyzer">
      <textarea value={content} onChange={e => setContent(e.target.value)} rows={3} placeholder="Paste your content..."
        className="w-full px-3 py-2 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground placeholder:text-muted-foreground/40 resize-none" />
      <div className="flex items-end gap-3 mt-2">
        <div className="flex-1">
          <Input label="Target Keyword" value={keyword} onChange={setKeyword} placeholder="atlas" />
        </div>
        <RunBtn loading={loading} onClick={run} disabled={!content.trim() || !keyword.trim()} />
      </div>
      {result && (
        <div className="mt-3 p-3 rounded-xl bg-muted/15 border border-border/15 flex items-center gap-4">
          <div className="text-center">
            <p className="text-xl font-bold text-foreground">{result.density.toFixed(1)}%</p>
            <p className="text-[10px] text-muted-foreground">density</p>
          </div>
          <div className="h-8 w-px bg-border/30" />
          <div>
            <p className="text-xs text-foreground">"{result.keyword}" found <span className="font-bold">{result.count}</span> times</p>
            <p className="text-[10px] text-muted-foreground mt-0.5">{result.recommendation}</p>
          </div>
        </div>
      )}
    </ToolCard>
  );
};

// ─── Readability ───────────────────────────────────────────────
const ReadabilityTool = () => {
  const [text, setText] = useState("");
  const [result, setResult] = useState<ReadabilityDto | null>(null);
  const [loading, setLoading] = useState(false);

  const run = async () => {
    if (!text.trim()) return;
    setLoading(true);
    try { const r = await marketerUtilitiesApi.readability({ text }); if (r.data.isSuccess) setResult(r.data.data); } catch {}
    setLoading(false);
  };

  const levelColors: Record<string, string> = { Easy: "text-emerald-400", Medium: "text-amber-400", Hard: "text-red-400" };

  return (
    <ToolCard title="Readability Analyzer">
      <textarea value={text} onChange={e => setText(e.target.value)} rows={4} placeholder="Paste your text..."
        className="w-full px-3 py-2 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground placeholder:text-muted-foreground/40 resize-none" />
      <RunBtn loading={loading} onClick={run} disabled={!text.trim()} />
      {result && (
        <div className="mt-3 p-3 rounded-xl bg-muted/15 border border-border/15 flex items-center gap-4">
          <div className="text-center">
            <p className="text-2xl font-bold text-foreground">{result.fleschScore.toFixed(0)}</p>
            <p className={`text-xs font-bold ${levelColors[result.level] || "text-foreground"}`}>{result.level}</p>
          </div>
          <div className="h-8 w-px bg-border/30" />
          <div className="text-xs text-muted-foreground space-y-0.5">
            <p>Avg sentence: <span className="text-foreground font-medium">{result.avgSentenceLength.toFixed(1)} words</span></p>
            <p>Avg word: <span className="text-foreground font-medium">{result.avgWordLength.toFixed(1)} chars</span></p>
          </div>
        </div>
      )}
    </ToolCard>
  );
};

// ─── Emoji Search ──────────────────────────────────────────────
const EmojiTool = () => {
  const [query, setQuery] = useState("");
  const [results, setResults] = useState<EmojiDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [copiedEmoji, setCopiedEmoji] = useState<string | null>(null);

  const run = async () => {
    if (!query.trim()) return;
    setLoading(true);
    try { const r = await marketerUtilitiesApi.emojis({ query }); if (r.data.isSuccess && r.data.data) setResults(r.data.data); } catch {}
    setLoading(false);
  };

  return (
    <ToolCard title="Emoji Search">
      <div className="flex items-end gap-3">
        <div className="flex-1">
          <Input label="Search" value={query} onChange={setQuery} placeholder="fire, heart, star..." onEnter={run} />
        </div>
        <RunBtn loading={loading} onClick={run} disabled={!query.trim()} />
      </div>
      {results.length > 0 && (
        <div className="mt-3 grid grid-cols-4 sm:grid-cols-6 gap-2">
          {results.map((e, i) => (
            <motion.button key={i} whileHover={{ scale: 1.1 }} whileTap={{ scale: 0.95 }}
              onClick={() => { navigator.clipboard.writeText(e.emoji); setCopiedEmoji(e.emoji); setTimeout(() => setCopiedEmoji(null), 1500); }}
              className={`p-3 rounded-xl bg-muted/15 border border-border/15 hover:border-primary/20 flex flex-col items-center gap-1 transition-all ${copiedEmoji === e.emoji ? "border-emerald-400/40 bg-emerald-500/5" : ""}`}>
              <span className="text-2xl">{e.emoji}</span>
              <span className="text-[9px] text-muted-foreground truncate w-full text-center">{e.name}</span>
            </motion.button>
          ))}
        </div>
      )}
    </ToolCard>
  );
};

// ─── Shared UI ─────────────────────────────────────────────────
const ToolCard = ({ title, children }: { title: string; children: React.ReactNode }) => (
  <motion.div initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} className="p-5 rounded-2xl bg-card/50 border border-border/30 space-y-3">
    <h3 className="text-sm font-bold text-foreground">{title}</h3>
    {children}
  </motion.div>
);

const Input = ({ label, value, onChange, placeholder, onEnter }: { label: string; value: string; onChange: (v: string) => void; placeholder?: string; onEnter?: () => void }) => (
  <div className="space-y-1">
    <label className="text-[10px] text-muted-foreground">{label}</label>
    <input type="text" value={value} onChange={e => onChange(e.target.value)} placeholder={placeholder}
      className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground placeholder:text-muted-foreground/40"
      onKeyDown={e => e.key === "Enter" && onEnter?.()} />
  </div>
);

const RunBtn = ({ loading, onClick, disabled }: { loading: boolean; onClick: () => void; disabled?: boolean }) => (
  <button onClick={onClick} disabled={loading || disabled}
    className="h-9 px-4 rounded-xl bg-primary text-primary-foreground text-xs font-semibold hover:bg-primary/90 disabled:opacity-50 transition-colors">
    {loading ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : "Run"}
  </button>
);

const CopyButton = ({ text }: { text: string }) => {
  const [copied, setCopied] = useState(false);
  return (
    <button onClick={() => { navigator.clipboard.writeText(text); setCopied(true); setTimeout(() => setCopied(false), 2000); }}
      className="absolute top-2 right-2 w-7 h-7 rounded-lg bg-card border border-border/30 flex items-center justify-center text-muted-foreground hover:text-foreground transition-colors">
      {copied ? <Check className="w-3 h-3 text-emerald-400" /> : <Copy className="w-3 h-3" />}
    </button>
  );
};

export default MarketerUtilitiesPanel;

