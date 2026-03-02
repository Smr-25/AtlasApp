import { useState } from "react";
import { motion } from "framer-motion";
import {
  Zap, PauseCircle, Share2, FileText, Link2,
  Globe, Cookie, MailCheck, Loader2, Terminal,
} from "lucide-react";
import {
  marketerScriptsApi, MktScriptOutputDto, MktReportDto,
  UtmResultDto, EmailVerifyResultDto,
} from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

type ActiveScript = "pause" | "social" | "report" | "utm" | "scrape" | "cookies" | "emails" | null;

const MarketerScriptsPanel = () => {
  const [activeScript, setActiveScript] = useState<ActiveScript>(null);
  const [outputs, setOutputs] = useState<Record<string, string>>({});
  const [loading, setLoading] = useState<Record<string, boolean>>({});

  const scripts = [
    { id: "pause" as const, label: "Pause Campaigns", desc: "Stop bleeding campaigns", icon: PauseCircle, color: "text-red-400", gradient: "from-red-500/12 to-red-500/3" },
    { id: "social" as const, label: "Social Blast", desc: "Post to multiple platforms", icon: Share2, color: "text-blue-400", gradient: "from-blue-500/12 to-blue-500/3" },
    { id: "report" as const, label: "Weekly Report", desc: "Generate marketing report", icon: FileText, color: "text-emerald-400", gradient: "from-emerald-500/12 to-emerald-500/3" },
    { id: "utm" as const, label: "UTM Link Builder", desc: "Create tracked URLs", icon: Link2, color: "text-violet-400", gradient: "from-violet-500/12 to-violet-500/3" },
    { id: "scrape" as const, label: "Competitor Scrape", desc: "Scrape competitor data", icon: Globe, color: "text-amber-400", gradient: "from-amber-500/12 to-amber-500/3" },
    { id: "cookies" as const, label: "Clear Cookies", desc: "Browser cookie cleanup", icon: Cookie, color: "text-cyan-400", gradient: "from-cyan-500/12 to-cyan-500/3" },
    { id: "emails" as const, label: "Verify Emails", desc: "Bulk email validation", icon: MailCheck, color: "text-pink-400", gradient: "from-pink-500/12 to-pink-500/3" },
  ];

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-amber-500/20 to-amber-500/5 border border-amber-500/10 flex items-center justify-center">
          <Zap className="w-5 h-5 text-amber-400" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">Marketing Automation</h2>
          <p className="text-xs text-muted-foreground">Campaign scripts & bulk operations</p>
        </div>
      </motion.div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
        {scripts.map((s) => (
          <motion.button key={s.id} variants={item} whileHover={{ y: -2 }} whileTap={{ scale: 0.98 }}
            onClick={() => setActiveScript(activeScript === s.id ? null : s.id)}
            className={`group p-4 rounded-2xl bg-gradient-to-br ${s.gradient} border text-left transition-all duration-200 ${activeScript === s.id ? "border-primary/30 shadow-lg" : "border-border/20 hover:border-primary/15"}`}>
            <div className="flex items-center gap-3 mb-2">
              <s.icon className={`w-5 h-5 ${s.color}`} />
              <span className="text-sm font-semibold text-foreground">{s.label}</span>
            </div>
            <p className="text-xs text-muted-foreground">{s.desc}</p>
          </motion.button>
        ))}
      </div>

      {activeScript === "pause" && <PauseCampaignsForm outputs={outputs} setOutputs={setOutputs} loading={loading} setLoading={setLoading} />}
      {activeScript === "social" && <SocialBlastForm outputs={outputs} setOutputs={setOutputs} loading={loading} setLoading={setLoading} />}
      {activeScript === "report" && <WeeklyReportForm outputs={outputs} setOutputs={setOutputs} loading={loading} setLoading={setLoading} />}
      {activeScript === "utm" && <UtmLinkForm outputs={outputs} setOutputs={setOutputs} loading={loading} setLoading={setLoading} />}
      {activeScript === "scrape" && <CompetitorScrapeForm outputs={outputs} setOutputs={setOutputs} loading={loading} setLoading={setLoading} />}
      {activeScript === "cookies" && <ClearCookiesForm outputs={outputs} setOutputs={setOutputs} loading={loading} setLoading={setLoading} />}
      {activeScript === "emails" && <VerifyEmailsForm outputs={outputs} setOutputs={setOutputs} loading={loading} setLoading={setLoading} />}
    </motion.div>
  );
};

type FormProps = {
  outputs: Record<string, string>;
  setOutputs: React.Dispatch<React.SetStateAction<Record<string, string>>>;
  loading: Record<string, boolean>;
  setLoading: React.Dispatch<React.SetStateAction<Record<string, boolean>>>;
};

const ScriptCard = ({ title, children, output }: { title: string; children: React.ReactNode; output?: string }) => (
  <motion.div initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} className="p-5 rounded-2xl bg-card/50 border border-border/30 space-y-3">
    <h3 className="text-sm font-bold text-foreground flex items-center gap-2"><Terminal className="w-4 h-4 text-primary" />{title}</h3>
    {children}
    {output && (
      <pre className="p-3 rounded-xl bg-muted/15 border border-border/15 text-xs text-foreground whitespace-pre-wrap max-h-48 overflow-y-auto">
        {output}
      </pre>
    )}
  </motion.div>
);

const RunBtn = ({ loading, onClick, label, danger }: { loading: boolean; onClick: () => void; label?: string; danger?: boolean }) => (
  <button onClick={onClick} disabled={loading}
    className={`h-9 px-4 rounded-xl text-xs font-semibold disabled:opacity-50 transition-colors ${danger ? "bg-red-500 text-white hover:bg-red-600" : "bg-primary text-primary-foreground hover:bg-primary/90"}`}>
    {loading ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : label || "Execute"}
  </button>
);

// ─── Pause Campaigns ───────────────────────────────────────────
const PauseCampaignsForm = ({ outputs, setOutputs, loading, setLoading }: FormProps) => {
  const [ids, setIds] = useState("camp-1, camp-2");
  const [reason, setReason] = useState("Budget exceeded");
  const run = async () => {
    const campaignIds = ids.split(",").map(s => s.trim()).filter(Boolean);
    if (campaignIds.length === 0) return;
    setLoading(p => ({ ...p, pause: true }));
    try { const r = await marketerScriptsApi.pauseCampaigns({ campaignIds, reason }); if (r.data.isSuccess) setOutputs(p => ({ ...p, pause: r.data.data.output })); } catch {}
    setLoading(p => ({ ...p, pause: false }));
  };
  return (
    <ScriptCard title="Pause Campaigns" output={outputs.pause}>
      <div className="space-y-2">
        <div className="space-y-1">
          <label className="text-[10px] text-muted-foreground">Campaign IDs (comma separated)</label>
          <input type="text" value={ids} onChange={e => setIds(e.target.value)}
            className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground" />
        </div>
        <div className="space-y-1">
          <label className="text-[10px] text-muted-foreground">Reason</label>
          <input type="text" value={reason} onChange={e => setReason(e.target.value)}
            className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground" />
        </div>
        <RunBtn loading={loading.pause} onClick={run} label="Pause" danger />
      </div>
    </ScriptCard>
  );
};

// ─── Social Blast ──────────────────────────────────────────────
const SocialBlastForm = ({ outputs, setOutputs, loading, setLoading }: FormProps) => {
  const [content, setContent] = useState("");
  const [platforms, setPlatforms] = useState(["twitter", "instagram", "linkedin"]);
  const toggle = (p: string) => setPlatforms(prev => prev.includes(p) ? prev.filter(x => x !== p) : [...prev, p]);
  const run = async () => {
    if (!content.trim() || platforms.length === 0) return;
    setLoading(p => ({ ...p, social: true }));
    try { const r = await marketerScriptsApi.socialBlast({ content, platforms }); if (r.data.isSuccess) setOutputs(p => ({ ...p, social: r.data.data.output })); } catch {}
    setLoading(p => ({ ...p, social: false }));
  };
  return (
    <ScriptCard title="Social Media Blast" output={outputs.social}>
      <textarea value={content} onChange={e => setContent(e.target.value)} rows={3} placeholder="🚀 Your post content..."
        className="w-full px-3 py-2 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground placeholder:text-muted-foreground/40 resize-none" />
      <div className="flex items-center gap-2 flex-wrap">
        {["twitter", "instagram", "linkedin", "facebook", "tiktok"].map(p => (
          <button key={p} onClick={() => toggle(p)}
            className={`px-3 py-1.5 rounded-lg text-[10px] font-semibold transition-colors ${platforms.includes(p) ? "bg-primary text-primary-foreground" : "bg-muted/30 text-muted-foreground border border-border/30"}`}>
            {p}
          </button>
        ))}
      </div>
      <RunBtn loading={loading.social} onClick={run} label="Blast" />
    </ScriptCard>
  );
};

// ─── Weekly Report ─────────────────────────────────────────────
const WeeklyReportForm = ({ outputs, setOutputs, loading, setLoading }: FormProps) => {
  const today = new Date();
  const weekAgo = new Date(today.getTime() - 7 * 24 * 60 * 60 * 1000);
  const [from, setFrom] = useState(weekAgo.toISOString().slice(0, 10));
  const [to, setTo] = useState(today.toISOString().slice(0, 10));
  const run = async () => {
    setLoading(p => ({ ...p, report: true }));
    try {
      const r = await marketerScriptsApi.weeklyReport({ from: `${from}T00:00:00Z`, to: `${to}T23:59:59Z` });
      if (r.data.isSuccess) setOutputs(p => ({ ...p, report: r.data.data.report }));
    } catch {}
    setLoading(p => ({ ...p, report: false }));
  };
  return (
    <ScriptCard title="Weekly Marketing Report" output={outputs.report}>
      <div className="flex items-end gap-3">
        <div className="flex-1 space-y-1">
          <label className="text-[10px] text-muted-foreground">From</label>
          <input type="date" value={from} onChange={e => setFrom(e.target.value)}
            className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground" />
        </div>
        <div className="flex-1 space-y-1">
          <label className="text-[10px] text-muted-foreground">To</label>
          <input type="date" value={to} onChange={e => setTo(e.target.value)}
            className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground" />
        </div>
        <RunBtn loading={loading.report} onClick={run} label="Generate" />
      </div>
    </ScriptCard>
  );
};

// ─── UTM Link Builder ──────────────────────────────────────────
const UtmLinkForm = ({ outputs, setOutputs, loading, setLoading }: FormProps) => {
  const [form, setForm] = useState({ baseUrl: "", source: "newsletter", medium: "email", campaign: "march_promo" });
  const run = async () => {
    if (!form.baseUrl.trim()) return;
    setLoading(p => ({ ...p, utm: true }));
    try {
      const r = await marketerScriptsApi.utmLink(form);
      if (r.data.isSuccess) setOutputs(p => ({ ...p, utm: r.data.data.utmUrl }));
    } catch {}
    setLoading(p => ({ ...p, utm: false }));
  };
  return (
    <ScriptCard title="UTM Link Builder" output={outputs.utm}>
      <div className="grid grid-cols-2 gap-2">
        <div className="col-span-2 space-y-1">
          <label className="text-[10px] text-muted-foreground">Base URL</label>
          <input type="text" value={form.baseUrl} onChange={e => setForm(p => ({ ...p, baseUrl: e.target.value }))} placeholder="https://example.com/sale"
            className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground placeholder:text-muted-foreground/40" />
        </div>
        <div className="space-y-1">
          <label className="text-[10px] text-muted-foreground">Source</label>
          <input type="text" value={form.source} onChange={e => setForm(p => ({ ...p, source: e.target.value }))}
            className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground" />
        </div>
        <div className="space-y-1">
          <label className="text-[10px] text-muted-foreground">Medium</label>
          <input type="text" value={form.medium} onChange={e => setForm(p => ({ ...p, medium: e.target.value }))}
            className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground" />
        </div>
        <div className="col-span-2 space-y-1">
          <label className="text-[10px] text-muted-foreground">Campaign</label>
          <input type="text" value={form.campaign} onChange={e => setForm(p => ({ ...p, campaign: e.target.value }))}
            className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground" />
        </div>
      </div>
      <RunBtn loading={loading.utm} onClick={run} label="Build Link" />
    </ScriptCard>
  );
};

// ─── Competitor Scrape ─────────────────────────────────────────
const CompetitorScrapeForm = ({ outputs, setOutputs, loading, setLoading }: FormProps) => {
  const [url, setUrl] = useState("");
  const run = async () => {
    if (!url.trim()) return;
    setLoading(p => ({ ...p, scrape: true }));
    try { const r = await marketerScriptsApi.competitorScrape({ competitorUrl: url }); if (r.data.isSuccess) setOutputs(p => ({ ...p, scrape: r.data.data.output })); } catch {}
    setLoading(p => ({ ...p, scrape: false }));
  };
  return (
    <ScriptCard title="Competitor Scrape" output={outputs.scrape}>
      <div className="flex items-end gap-3">
        <div className="flex-1 space-y-1">
          <label className="text-[10px] text-muted-foreground">Competitor URL</label>
          <input type="text" value={url} onChange={e => setUrl(e.target.value)} placeholder="https://competitor.com"
            className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground placeholder:text-muted-foreground/40"
            onKeyDown={e => e.key === "Enter" && run()} />
        </div>
        <RunBtn loading={loading.scrape} onClick={run} label="Scrape" />
      </div>
    </ScriptCard>
  );
};

// ─── Clear Cookies ─────────────────────────────────────────────
const ClearCookiesForm = ({ outputs, setOutputs, loading, setLoading }: FormProps) => {
  const [browser, setBrowser] = useState("chrome");
  const run = async () => {
    setLoading(p => ({ ...p, cookies: true }));
    try { const r = await marketerScriptsApi.clearCookies({ browser }); if (r.data.isSuccess) setOutputs(p => ({ ...p, cookies: r.data.data.output })); } catch {}
    setLoading(p => ({ ...p, cookies: false }));
  };
  return (
    <ScriptCard title="Clear Browser Cookies" output={outputs.cookies}>
      <div className="flex items-end gap-3">
        <select value={browser} onChange={e => setBrowser(e.target.value)}
          className="h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground">
          {["chrome", "firefox", "safari"].map(b => <option key={b} value={b}>{b}</option>)}
        </select>
        <RunBtn loading={loading.cookies} onClick={run} label="Clear" />
      </div>
    </ScriptCard>
  );
};

// ─── Verify Emails ─────────────────────────────────────────────
const VerifyEmailsForm = ({ outputs, setOutputs, loading, setLoading }: FormProps) => {
  const [emails, setEmails] = useState("");
  const [result, setResult] = useState<{ total: number; valid: number; invalid: number; invalidEmails: string[] } | null>(null);
  const run = async () => {
    const list = emails.split(/[\n,]/).map(e => e.trim()).filter(Boolean);
    if (list.length === 0) return;
    setLoading(p => ({ ...p, emails: true }));
    try {
      const r = await marketerScriptsApi.verifyEmails({ emails: list });
      if (r.data.isSuccess) {
        setResult(r.data.data);
        setOutputs(p => ({ ...p, emails: `Total: ${r.data.data.total} | Valid: ${r.data.data.valid} | Invalid: ${r.data.data.invalid}` }));
      }
    } catch {}
    setLoading(p => ({ ...p, emails: false }));
  };
  return (
    <ScriptCard title="Bulk Email Verification">
      <textarea value={emails} onChange={e => setEmails(e.target.value)} rows={4} placeholder="user1@example.com&#10;user2@gmail.com&#10;bad@invalid"
        className="w-full px-3 py-2 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground placeholder:text-muted-foreground/40 resize-none" />
      <RunBtn loading={loading.emails} onClick={run} label="Verify All" />
      {result && (
        <div className="mt-2 space-y-2">
          <div className="flex items-center gap-4 p-3 rounded-xl bg-muted/15 border border-border/15">
            <div className="text-center">
              <p className="text-lg font-bold text-foreground">{result.total}</p>
              <p className="text-[10px] text-muted-foreground">Total</p>
            </div>
            <div className="h-8 w-px bg-border/30" />
            <div className="text-center">
              <p className="text-lg font-bold text-emerald-400">{result.valid}</p>
              <p className="text-[10px] text-muted-foreground">Valid</p>
            </div>
            <div className="h-8 w-px bg-border/30" />
            <div className="text-center">
              <p className="text-lg font-bold text-red-400">{result.invalid}</p>
              <p className="text-[10px] text-muted-foreground">Invalid</p>
            </div>
          </div>
          {result.invalidEmails.length > 0 && (
            <div className="space-y-1">
              {result.invalidEmails.map((e, i) => (
                <div key={i} className="text-xs text-red-400 flex items-center gap-2 p-1.5 rounded-lg bg-red-500/5 border border-red-500/10">
                  <span>✗</span> {e}
                </div>
              ))}
            </div>
          )}
        </div>
      )}
    </ScriptCard>
  );
};

export default MarketerScriptsPanel;

