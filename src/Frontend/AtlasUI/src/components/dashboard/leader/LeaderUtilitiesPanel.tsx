import { useState } from "react";
import { motion } from "framer-motion";
import {
  Wrench, Globe, BarChart, Users, DollarSign,
  AlertTriangle, BookOpen, FileCode, Loader2, Copy, Check,
} from "lucide-react";
import {
  leaderUtilitiesApi, TimezonesDto, QuickPollDto, CapacityDto,
  CostEstimateDto, RiskMatrixDto, DecisionLogDto, MdHtmlDto,
} from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

type ActiveTool = "tz" | "poll" | "capacity" | "cost" | "risk" | "decision" | "md" | null;

const LeaderUtilitiesPanel = () => {
  const [activeTool, setActiveTool] = useState<ActiveTool>(null);

  const tools = [
    { id: "tz" as const, label: "Timezone Converter", desc: "Team member local times", icon: Globe, color: "text-blue-400", gradient: "from-blue-500/12 to-blue-500/3" },
    { id: "poll" as const, label: "Quick Poll", desc: "Create team polls", icon: BarChart, color: "text-violet-400", gradient: "from-violet-500/12 to-violet-500/3" },
    { id: "capacity" as const, label: "Team Capacity", desc: "Calculate available hours", icon: Users, color: "text-emerald-400", gradient: "from-emerald-500/12 to-emerald-500/3" },
    { id: "cost" as const, label: "Cost Estimator", desc: "Estimate project costs", icon: DollarSign, color: "text-amber-400", gradient: "from-amber-500/12 to-amber-500/3" },
    { id: "risk" as const, label: "Risk Matrix", desc: "Prioritize risks", icon: AlertTriangle, color: "text-red-400", gradient: "from-red-500/12 to-red-500/3" },
    { id: "decision" as const, label: "Decision Log", desc: "Record key decisions", icon: BookOpen, color: "text-cyan-400", gradient: "from-cyan-500/12 to-cyan-500/3" },
    { id: "md" as const, label: "Markdown Render", desc: "Markdown → HTML", icon: FileCode, color: "text-pink-400", gradient: "from-pink-500/12 to-pink-500/3" },
  ];

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-violet-500/20 to-violet-500/5 border border-violet-500/10 flex items-center justify-center">
          <Wrench className="w-5 h-5 text-violet-400" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">PM Tools</h2>
          <p className="text-xs text-muted-foreground">Project management utilities & calculators</p>
        </div>
      </motion.div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
        {tools.map((t) => (
          <motion.button key={t.id} variants={item} whileHover={{ y: -2 }} whileTap={{ scale: 0.98 }}
            onClick={() => setActiveTool(activeTool === t.id ? null : t.id)}
            className={`group p-4 rounded-2xl bg-gradient-to-br ${t.gradient} border text-left transition-all duration-200 ${activeTool === t.id ? "border-primary/30 shadow-lg" : "border-border/20 hover:border-primary/15"}`}>
            <div className="flex items-center gap-3 mb-2">
              <t.icon className={`w-5 h-5 ${t.color}`} />
              <span className="text-sm font-semibold text-foreground">{t.label}</span>
            </div>
            <p className="text-xs text-muted-foreground">{t.desc}</p>
          </motion.button>
        ))}
      </div>

      {activeTool === "tz" && <TimezoneTool />}
      {activeTool === "poll" && <PollTool />}
      {activeTool === "capacity" && <CapacityTool />}
      {activeTool === "cost" && <CostTool />}
      {activeTool === "risk" && <RiskTool />}
      {activeTool === "decision" && <DecisionTool />}
      {activeTool === "md" && <MarkdownTool />}
    </motion.div>
  );
};

// ─── Shared UI ─────────────────────────────────────────────────
const ToolCard = ({ title, children }: { title: string; children: React.ReactNode }) => (
  <motion.div initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} className="p-5 rounded-2xl bg-card/50 border border-border/30 space-y-3">
    <h3 className="text-sm font-bold text-foreground">{title}</h3>
    {children}
  </motion.div>
);
const Input = ({ label, value, onChange, placeholder, type }: { label: string; value: string; onChange: (v: string) => void; placeholder?: string; type?: string }) => (
  <div className="space-y-1">
    <label className="text-[10px] text-muted-foreground">{label}</label>
    <input type={type || "text"} value={value} onChange={e => onChange(e.target.value)} placeholder={placeholder}
      className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground placeholder:text-muted-foreground/40" />
  </div>
);
const RunBtn = ({ loading, onClick, label }: { loading: boolean; onClick: () => void; label?: string }) => (
  <button onClick={onClick} disabled={loading}
    className="h-9 px-4 rounded-xl bg-primary text-primary-foreground text-xs font-semibold hover:bg-primary/90 disabled:opacity-50 transition-colors">
    {loading ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : label || "Run"}
  </button>
);

// ─── Timezone Converter ────────────────────────────────────────
const TimezoneTool = () => {
  const [members, setMembers] = useState([{ memberName: "Alice", timezoneId: "America/New_York" }, { memberName: "Bob", timezoneId: "Asia/Tokyo" }]);
  const [result, setResult] = useState<TimezonesDto | null>(null);
  const [loading, setLoading] = useState(false);
  const addMember = () => setMembers(p => [...p, { memberName: "", timezoneId: "" }]);
  const updateMember = (i: number, key: string, val: string) => setMembers(p => p.map((m, idx) => idx === i ? { ...m, [key]: val } : m));
  const run = async () => { setLoading(true); try { const r = await leaderUtilitiesApi.timezones({ members }); if (r.data.isSuccess) setResult(r.data.data); } catch {} setLoading(false); };
  return (
    <ToolCard title="Timezone Converter">
      {members.map((m, i) => (
        <div key={i} className="flex gap-2">
          <input value={m.memberName} onChange={e => updateMember(i, "memberName", e.target.value)} placeholder="Name"
            className="flex-1 h-8 px-2 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground" />
          <input value={m.timezoneId} onChange={e => updateMember(i, "timezoneId", e.target.value)} placeholder="Asia/Baku"
            className="flex-1 h-8 px-2 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground" />
        </div>
      ))}
      <div className="flex gap-2">
        <button onClick={addMember} className="h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-muted-foreground hover:text-foreground">+ Add</button>
        <RunBtn loading={loading} onClick={run} label="Convert" />
      </div>
      {result && (
        <div className="mt-2 space-y-1">
          {result.memberTimes.map((m, i) => (
            <div key={i} className="flex items-center gap-3 p-2 rounded-lg bg-muted/10 border border-border/15 text-xs">
              <span className="font-bold text-foreground">{m.memberName}</span>
              <span className="text-muted-foreground">{m.timezoneId}</span>
              <span className="ml-auto font-bold text-blue-400">{m.localTime} (UTC{m.offset})</span>
            </div>
          ))}
        </div>
      )}
    </ToolCard>
  );
};

// ─── Quick Poll ────────────────────────────────────────────────
const PollTool = () => {
  const [question, setQuestion] = useState("");
  const [options, setOptions] = useState(["Option A", "Option B"]);
  const [result, setResult] = useState<QuickPollDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [copied, setCopied] = useState(false);
  const run = async () => { if (!question.trim()) return; setLoading(true); try { const r = await leaderUtilitiesApi.quickPoll({ question, options: options.filter(Boolean) }); if (r.data.isSuccess) setResult(r.data.data); } catch {} setLoading(false); };
  return (
    <ToolCard title="Quick Poll">
      <Input label="Question" value={question} onChange={setQuestion} placeholder="Sprint 25 hansı gün başlasın?" />
      {options.map((o, i) => (
        <input key={i} value={o} onChange={e => setOptions(p => p.map((x, idx) => idx === i ? e.target.value : x))} placeholder={`Option ${i + 1}`}
          className="w-full h-8 px-2 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground" />
      ))}
      <div className="flex gap-2">
        <button onClick={() => setOptions(p => [...p, ""])} className="h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-muted-foreground">+ Add Option</button>
        <RunBtn loading={loading} onClick={run} label="Create Poll" />
      </div>
      {result && (
        <div className="relative mt-2">
          <pre className="p-3 rounded-xl bg-muted/15 border border-border/15 text-xs text-foreground whitespace-pre-wrap">{result.formattedMessage}</pre>
          <button onClick={() => { navigator.clipboard.writeText(result.formattedMessage); setCopied(true); setTimeout(() => setCopied(false), 2000); }}
            className="absolute top-2 right-2 w-7 h-7 rounded-lg bg-card border border-border/30 flex items-center justify-center">
            {copied ? <Check className="w-3 h-3 text-emerald-400" /> : <Copy className="w-3 h-3 text-muted-foreground" />}
          </button>
        </div>
      )}
    </ToolCard>
  );
};

// ─── Capacity ──────────────────────────────────────────────────
const CapacityTool = () => {
  const [members, setMembers] = useState([{ memberName: "Tərlan", hoursPerDay: 8, daysOff: 2, meetingHoursPerWeek: 5 }]);
  const [result, setResult] = useState<CapacityDto | null>(null);
  const [loading, setLoading] = useState(false);
  const addMember = () => setMembers(p => [...p, { memberName: "", hoursPerDay: 8, daysOff: 0, meetingHoursPerWeek: 3 }]);
  const updateM = (i: number, key: string, val: string | number) => setMembers(p => p.map((m, idx) => idx === i ? { ...m, [key]: val } : m));
  const run = async () => { setLoading(true); try { const r = await leaderUtilitiesApi.capacity({ members }); if (r.data.isSuccess) setResult(r.data.data); } catch {} setLoading(false); };
  return (
    <ToolCard title="Team Capacity Calculator">
      {members.map((m, i) => (
        <div key={i} className="flex gap-1.5">
          <input value={m.memberName} onChange={e => updateM(i, "memberName", e.target.value)} placeholder="Name"
            className="flex-1 h-8 px-2 rounded-lg bg-muted/30 border border-border/30 text-[10px] text-foreground" />
          <input type="number" value={m.hoursPerDay} onChange={e => updateM(i, "hoursPerDay", Number(e.target.value))} placeholder="h/day"
            className="w-14 h-8 px-1 rounded-lg bg-muted/30 border border-border/30 text-[10px] text-foreground text-center" />
          <input type="number" value={m.daysOff} onChange={e => updateM(i, "daysOff", Number(e.target.value))} placeholder="off"
            className="w-14 h-8 px-1 rounded-lg bg-muted/30 border border-border/30 text-[10px] text-foreground text-center" />
          <input type="number" value={m.meetingHoursPerWeek} onChange={e => updateM(i, "meetingHoursPerWeek", Number(e.target.value))} placeholder="mtg"
            className="w-14 h-8 px-1 rounded-lg bg-muted/30 border border-border/30 text-[10px] text-foreground text-center" />
        </div>
      ))}
      <div className="flex gap-2">
        <button onClick={addMember} className="h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-muted-foreground">+ Add</button>
        <RunBtn loading={loading} onClick={run} label="Calculate" />
      </div>
      {result && (
        <div className="mt-2 p-3 rounded-xl bg-muted/15 border border-border/15">
          <p className="text-lg font-bold text-foreground text-center">{result.totalAvailableHours}h total capacity</p>
          <div className="flex flex-wrap gap-3 justify-center mt-2">
            {result.members.map((m, i) => (
              <span key={i} className="text-xs text-muted-foreground">{m.memberName}: <span className="text-foreground font-bold">{m.availableHours}h</span></span>
            ))}
          </div>
        </div>
      )}
    </ToolCard>
  );
};

// ─── Cost Estimator ────────────────────────────────────────────
const CostTool = () => {
  const [hours, setHours] = useState("200");
  const [rate, setRate] = useState("80");
  const [server, setServer] = useState("500");
  const [months, setMonths] = useState("3");
  const [result, setResult] = useState<CostEstimateDto | null>(null);
  const [loading, setLoading] = useState(false);
  const run = async () => { setLoading(true); try { const r = await leaderUtilitiesApi.costEstimate({ hoursEstimated: Number(hours), hourlyRate: Number(rate), serverMonthlyCost: Number(server), estimatedMonths: Number(months) }); if (r.data.isSuccess) setResult(r.data.data); } catch {} setLoading(false); };
  return (
    <ToolCard title="Cost Estimator">
      <div className="grid grid-cols-2 gap-2">
        <Input label="Hours" value={hours} onChange={setHours} type="number" />
        <Input label="Rate ($/h)" value={rate} onChange={setRate} type="number" />
        <Input label="Server ($/mo)" value={server} onChange={setServer} type="number" />
        <Input label="Months" value={months} onChange={setMonths} type="number" />
      </div>
      <RunBtn loading={loading} onClick={run} label="Estimate" />
      {result && (
        <pre className="mt-2 p-3 rounded-xl bg-muted/15 border border-border/15 text-xs text-foreground whitespace-pre-wrap">{result.breakdown}</pre>
      )}
    </ToolCard>
  );
};

// ─── Risk Matrix ───────────────────────────────────────────────
const RiskTool = () => {
  const [items, setItems] = useState([{ title: "", impact: 5, probability: 5 }]);
  const [result, setResult] = useState<RiskMatrixDto | null>(null);
  const [loading, setLoading] = useState(false);
  const addItem = () => setItems(p => [...p, { title: "", impact: 5, probability: 5 }]);
  const updateI = (i: number, key: string, val: string | number) => setItems(p => p.map((x, idx) => idx === i ? { ...x, [key]: val } : x));
  const run = async () => { setLoading(true); try { const r = await leaderUtilitiesApi.riskMatrix({ items: items.filter(x => x.title.trim()) }); if (r.data.isSuccess) setResult(r.data.data); } catch {} setLoading(false); };
  const renderCategory = (label: string, items: { title: string; score: number }[], color: string) => items.length > 0 && (
    <div className="space-y-1">
      <p className={`text-[10px] font-bold ${color}`}>{label}</p>
      {items.map((r, i) => (
        <div key={i} className="flex items-center gap-2 text-xs p-1.5 rounded-lg bg-muted/10 border border-border/15">
          <span className="text-foreground flex-1">{r.title}</span>
          <span className={`font-bold ${color}`}>{r.score}</span>
        </div>
      ))}
    </div>
  );
  return (
    <ToolCard title="Risk Matrix">
      {items.map((it, i) => (
        <div key={i} className="flex gap-1.5">
          <input value={it.title} onChange={e => updateI(i, "title", e.target.value)} placeholder="Risk title"
            className="flex-1 h-8 px-2 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground" />
          <input type="number" value={it.impact} onChange={e => updateI(i, "impact", Number(e.target.value))} min={1} max={10}
            className="w-14 h-8 px-1 rounded-lg bg-muted/30 border border-border/30 text-[10px] text-foreground text-center" title="Impact (1-10)" />
          <input type="number" value={it.probability} onChange={e => updateI(i, "probability", Number(e.target.value))} min={1} max={10}
            className="w-14 h-8 px-1 rounded-lg bg-muted/30 border border-border/30 text-[10px] text-foreground text-center" title="Probability (1-10)" />
        </div>
      ))}
      <div className="flex gap-2">
        <button onClick={addItem} className="h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-muted-foreground">+ Add</button>
        <RunBtn loading={loading} onClick={run} label="Analyze" />
      </div>
      {result && (
        <div className="mt-2 space-y-2">
          {renderCategory("🔴 Urgent", result.urgent, "text-red-400")}
          {renderCategory("🟠 Important", result.important, "text-amber-400")}
          {renderCategory("🟢 Later", result.later, "text-emerald-400")}
        </div>
      )}
    </ToolCard>
  );
};

// ─── Decision Log ──────────────────────────────────────────────
const DecisionTool = () => {
  const [decision, setDecision] = useState("");
  const [rationale, setRationale] = useState("");
  const [decidedBy, setDecidedBy] = useState("");
  const [result, setResult] = useState<DecisionLogDto | null>(null);
  const [loading, setLoading] = useState(false);
  const run = async () => { if (!decision.trim()) return; setLoading(true); try { const r = await leaderUtilitiesApi.decisionLog({ decision, rationale, decidedBy }); if (r.data.isSuccess) setResult(r.data.data); } catch {} setLoading(false); };
  return (
    <ToolCard title="Decision Log">
      <Input label="Decision" value={decision} onChange={setDecision} placeholder="Monolith → microservice migration" />
      <Input label="Rationale" value={rationale} onChange={setRationale} placeholder="Scaling problems" />
      <Input label="Decided By" value={decidedBy} onChange={setDecidedBy} placeholder="Fərid Əliyev" />
      <RunBtn loading={loading} onClick={run} label="Record" />
      {result && (
        <div className="mt-2 p-3 rounded-xl bg-emerald-500/5 border border-emerald-500/15 text-xs">
          <p className="text-emerald-400 font-bold">✓ Decision recorded</p>
          <p className="text-muted-foreground mt-1">ID: {result.id} | {new Date(result.recordedAt).toLocaleString()}</p>
        </div>
      )}
    </ToolCard>
  );
};

// ─── Markdown Render ───────────────────────────────────────────
const MarkdownTool = () => {
  const [md, setMd] = useState("");
  const [result, setResult] = useState<MdHtmlDto | null>(null);
  const [loading, setLoading] = useState(false);
  const run = async () => { if (!md.trim()) return; setLoading(true); try { const r = await leaderUtilitiesApi.markdown({ markdown: md }); if (r.data.isSuccess) setResult(r.data.data); } catch {} setLoading(false); };
  return (
    <ToolCard title="Markdown → HTML">
      <textarea value={md} onChange={e => setMd(e.target.value)} rows={4} placeholder="# Sprint Summary&#10;- Tasks: 42..."
        className="w-full px-3 py-2 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground placeholder:text-muted-foreground/40 resize-none" />
      <RunBtn loading={loading} onClick={run} label="Render" />
      {result && <div className="mt-2 p-3 rounded-xl bg-muted/15 border border-border/15 prose prose-sm prose-invert max-w-none text-foreground" dangerouslySetInnerHTML={{ __html: result.html }} />}
    </ToolCard>
  );
};

export default LeaderUtilitiesPanel;

