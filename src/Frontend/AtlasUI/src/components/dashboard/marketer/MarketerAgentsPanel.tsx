import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import {
  Bot, DollarSign, Link2, TrendingUp, Tag, BarChart3,
  Mail, ShoppingCart, Loader2, AlertTriangle, CheckCircle2,
  Copy, Check, ExternalLink,
} from "lucide-react";
import {
  marketerAgentsApi, BudgetBleedDto, BrokenLinkDto,
  ViralTrendDto, CompetitorPriceDto, UtmResultDto, CartAbandonmentDto,
} from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

const MarketerAgentsPanel = () => {
  const [budgetBleed, setBudgetBleed] = useState<BudgetBleedDto | null>(null);
  const [brokenLinks, setBrokenLinks] = useState<BrokenLinkDto[] | null>(null);
  const [trends, setTrends] = useState<ViralTrendDto[] | null>(null);
  const [prices, setPrices] = useState<CompetitorPriceDto[] | null>(null);
  const [utm, setUtm] = useState<UtmResultDto | null>(null);
  const [cart, setCart] = useState<CartAbandonmentDto | null>(null);
  const [loading, setLoading] = useState<Record<string, boolean>>({});

  // Form states
  const [linkUrl, setLinkUrl] = useState("");
  const [industry, setIndustry] = useState("e-commerce");
  const [compUrl, setCompUrl] = useState("");
  const [utmForm, setUtmForm] = useState({ url: "", source: "facebook", medium: "cpc", campaign: "" });
  const [resendForm, setResendForm] = useState({ campaignId: "", newSubject: "" });
  const [resendResult, setResendResult] = useState<string | null>(null);
  const [copied, setCopied] = useState(false);

  // Auto-load cart abandonment
  useEffect(() => {
    marketerAgentsApi.cartAbandonment().then(r => { if (r.data.isSuccess) setCart(r.data.data); }).catch(() => {});
  }, []);

  const setL = (id: string, v: boolean) => setLoading(p => ({ ...p, [id]: v }));

  const runBudget = async () => { setL("budget", true); try { const r = await marketerAgentsApi.budgetBleed(); if (r.data.isSuccess) setBudgetBleed(r.data.data); } catch {} setL("budget", false); };
  const runLinks = async () => { if (!linkUrl.trim()) return; setL("links", true); try { const r = await marketerAgentsApi.brokenLinks({ baseUrl: linkUrl }); if (r.data.isSuccess) setBrokenLinks(r.data.data); } catch {} setL("links", false); };
  const runTrends = async () => { setL("trends", true); try { const r = await marketerAgentsApi.viralTrends({ industry }); if (r.data.isSuccess) setTrends(r.data.data); } catch {} setL("trends", false); };
  const runPrices = async () => { if (!compUrl.trim()) return; setL("prices", true); try { const r = await marketerAgentsApi.competitorPriceDrop({ competitorUrl: compUrl }); if (r.data.isSuccess) setPrices(r.data.data); } catch {} setL("prices", false); };
  const runUtm = async () => { if (!utmForm.url.trim()) return; setL("utm", true); try { const r = await marketerAgentsApi.autoUtm(utmForm); if (r.data.isSuccess) setUtm(r.data.data); } catch {} setL("utm", false); };
  const runResend = async () => { if (!resendForm.campaignId.trim()) return; setL("resend", true); try { const r = await marketerAgentsApi.resendLowOpen(resendForm); if (r.data.isSuccess) setResendResult(r.data.data.result); } catch {} setL("resend", false); };

  const agents = [
    { id: "budget", label: "Budget Bleed", desc: "Find money-wasting campaigns", icon: DollarSign, color: "text-red-400", gradient: "from-red-500/12 to-red-500/3" },
    { id: "links", label: "Broken Links", desc: "Crawl for dead URLs", icon: Link2, color: "text-amber-400", gradient: "from-amber-500/12 to-amber-500/3" },
    { id: "trends", label: "Viral Trends", desc: "Discover trending hashtags", icon: TrendingUp, color: "text-violet-400", gradient: "from-violet-500/12 to-violet-500/3" },
    { id: "prices", label: "Competitor Prices", desc: "Monitor price drops", icon: Tag, color: "text-blue-400", gradient: "from-blue-500/12 to-blue-500/3" },
    { id: "utm", label: "Auto UTM", desc: "Generate tracking URLs", icon: BarChart3, color: "text-emerald-400", gradient: "from-emerald-500/12 to-emerald-500/3" },
    { id: "resend", label: "Resend Low Open", desc: "Re-send to non-openers", icon: Mail, color: "text-cyan-400", gradient: "from-cyan-500/12 to-cyan-500/3" },
  ];

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-violet-500/20 to-violet-500/5 border border-violet-500/10 flex items-center justify-center">
          <Bot className="w-5 h-5 text-violet-400" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">AI Marketing Agents</h2>
          <p className="text-xs text-muted-foreground">Smart campaign analysis & automation</p>
        </div>
      </motion.div>

      {/* Cart Abandonment Banner */}
      {cart && (
        <motion.div variants={item} className="p-3 rounded-xl bg-amber-500/5 border border-amber-500/15 flex items-center gap-3">
          <ShoppingCart className="w-4 h-4 text-amber-400" />
          <div className="flex-1">
            <p className="text-xs font-bold text-foreground">{cart.abandonedCount} abandoned carts ({cart.abandonmentRate}%)</p>
            <p className="text-[10px] text-muted-foreground">{cart.recommendation}</p>
          </div>
        </motion.div>
      )}

      {/* Agent Buttons */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
        {agents.map((a) => (
          <motion.div key={a.id} variants={item}>
            <AgentCard agent={a} loading={loading[a.id]}>
              {a.id === "budget" && <Btn loading={loading.budget} onClick={runBudget} label="Detect" />}
              {a.id === "links" && (
                <div className="flex items-end gap-2 mt-2">
                  <input type="text" value={linkUrl} onChange={e => setLinkUrl(e.target.value)} placeholder="https://example.com"
                    className="flex-1 h-8 px-2 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground placeholder:text-muted-foreground/40" />
                  <Btn loading={loading.links} onClick={runLinks} label="Crawl" />
                </div>
              )}
              {a.id === "trends" && (
                <div className="flex items-end gap-2 mt-2">
                  <select value={industry} onChange={e => setIndustry(e.target.value)}
                    className="flex-1 h-8 px-2 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground">
                    {["e-commerce", "saas", "fintech", "health", "education"].map(v => <option key={v} value={v}>{v}</option>)}
                  </select>
                  <Btn loading={loading.trends} onClick={runTrends} label="Search" />
                </div>
              )}
              {a.id === "prices" && (
                <div className="flex items-end gap-2 mt-2">
                  <input type="text" value={compUrl} onChange={e => setCompUrl(e.target.value)} placeholder="https://competitor.com"
                    className="flex-1 h-8 px-2 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground placeholder:text-muted-foreground/40" />
                  <Btn loading={loading.prices} onClick={runPrices} label="Check" />
                </div>
              )}
              {a.id === "utm" && (
                <div className="space-y-1.5 mt-2">
                  <input type="text" value={utmForm.url} onChange={e => setUtmForm(p => ({ ...p, url: e.target.value }))} placeholder="https://example.com/sale"
                    className="w-full h-8 px-2 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground placeholder:text-muted-foreground/40" />
                  <div className="flex gap-1.5">
                    <input type="text" value={utmForm.source} onChange={e => setUtmForm(p => ({ ...p, source: e.target.value }))} placeholder="source"
                      className="flex-1 h-8 px-2 rounded-lg bg-muted/30 border border-border/30 text-[10px] text-foreground" />
                    <input type="text" value={utmForm.medium} onChange={e => setUtmForm(p => ({ ...p, medium: e.target.value }))} placeholder="medium"
                      className="flex-1 h-8 px-2 rounded-lg bg-muted/30 border border-border/30 text-[10px] text-foreground" />
                    <input type="text" value={utmForm.campaign} onChange={e => setUtmForm(p => ({ ...p, campaign: e.target.value }))} placeholder="campaign"
                      className="flex-1 h-8 px-2 rounded-lg bg-muted/30 border border-border/30 text-[10px] text-foreground" />
                  </div>
                  <Btn loading={loading.utm} onClick={runUtm} label="Generate" />
                </div>
              )}
              {a.id === "resend" && (
                <div className="space-y-1.5 mt-2">
                  <input type="text" value={resendForm.campaignId} onChange={e => setResendForm(p => ({ ...p, campaignId: e.target.value }))} placeholder="Campaign ID"
                    className="w-full h-8 px-2 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground placeholder:text-muted-foreground/40" />
                  <input type="text" value={resendForm.newSubject} onChange={e => setResendForm(p => ({ ...p, newSubject: e.target.value }))} placeholder="New subject line"
                    className="w-full h-8 px-2 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground placeholder:text-muted-foreground/40" />
                  <Btn loading={loading.resend} onClick={runResend} label="Resend" />
                </div>
              )}
            </AgentCard>
          </motion.div>
        ))}
      </div>

      {/* Results */}
      {budgetBleed && (
        <ResultCard title="Budget Bleed Detection" icon={<DollarSign className="w-4 h-4 text-red-400" />}>
          {!budgetBleed.hasBleed ? <p className="text-xs text-emerald-400">✓ No budget bleed detected</p> : (
            <div className="space-y-1.5">
              {budgetBleed.campaigns.map((c, i) => (
                <div key={i} className="flex items-center gap-3 p-2 rounded-lg bg-red-500/5 border border-red-500/10 text-xs">
                  <span className="font-bold text-foreground">{c.name}</span>
                  <span className="text-muted-foreground">Spend: ${c.spend.toLocaleString()}</span>
                  <span className="text-muted-foreground">Rev: ${c.revenue.toLocaleString()}</span>
                  <span className="text-red-400 font-bold ml-auto">ROAS: {c.roas}x</span>
                </div>
              ))}
            </div>
          )}
        </ResultCard>
      )}

      {brokenLinks && (
        <ResultCard title="Broken Links" icon={<Link2 className="w-4 h-4 text-amber-400" />}>
          {brokenLinks.length === 0 ? <p className="text-xs text-emerald-400">✓ All links are healthy</p> : (
            <div className="space-y-1">
              {brokenLinks.map((l, i) => (
                <div key={i} className="flex items-center gap-3 p-2 rounded-lg bg-amber-500/5 border border-amber-500/10 text-xs">
                  <span className="text-red-400 font-bold">{l.statusCode}</span>
                  <span className="text-foreground truncate flex-1">{l.url}</span>
                  <span className="text-muted-foreground">{l.errorMessage}</span>
                </div>
              ))}
            </div>
          )}
        </ResultCard>
      )}

      {trends && trends.length > 0 && (
        <ResultCard title="Viral Trends" icon={<TrendingUp className="w-4 h-4 text-violet-400" />}>
          <div className="space-y-1.5">
            {trends.map((t, i) => (
              <div key={i} className="flex items-center gap-3 p-2 rounded-lg bg-violet-500/5 border border-violet-500/10 text-xs">
                <span className="text-violet-400 font-bold">{t.hashtag}</span>
                <span className="text-muted-foreground">{t.platform}</span>
                <span className="text-foreground">{t.volume.toLocaleString()} vol</span>
                <span className={`ml-auto font-bold ${t.sentiment === "Positive" ? "text-emerald-400" : t.sentiment === "Negative" ? "text-red-400" : "text-muted-foreground"}`}>{t.sentiment}</span>
              </div>
            ))}
          </div>
        </ResultCard>
      )}

      {prices && prices.length > 0 && (
        <ResultCard title="Competitor Price Drops" icon={<Tag className="w-4 h-4 text-blue-400" />}>
          <div className="space-y-1.5">
            {prices.map((p, i) => (
              <div key={i} className="flex items-center gap-3 p-2 rounded-lg bg-blue-500/5 border border-blue-500/10 text-xs">
                <span className="text-foreground font-bold">{p.productName}</span>
                <span className="text-muted-foreground line-through">${p.oldPrice}</span>
                <span className="text-emerald-400 font-bold">${p.newPrice}</span>
                <span className="text-red-400 ml-auto font-bold">-{p.discountPercent}%</span>
              </div>
            ))}
          </div>
        </ResultCard>
      )}

      {utm && (
        <ResultCard title="Generated UTM URL" icon={<BarChart3 className="w-4 h-4 text-emerald-400" />}>
          <div className="flex items-center gap-2 p-2 rounded-lg bg-emerald-500/5 border border-emerald-500/10">
            <code className="text-xs text-foreground break-all flex-1">{utm.utmUrl}</code>
            <button onClick={() => { navigator.clipboard.writeText(utm.utmUrl); setCopied(true); setTimeout(() => setCopied(false), 2000); }}
              className="w-7 h-7 rounded-lg bg-card border border-border/30 flex items-center justify-center text-muted-foreground hover:text-foreground">
              {copied ? <Check className="w-3 h-3 text-emerald-400" /> : <Copy className="w-3 h-3" />}
            </button>
          </div>
        </ResultCard>
      )}

      {resendResult && (
        <ResultCard title="Resend Result" icon={<Mail className="w-4 h-4 text-cyan-400" />}>
          <p className="text-xs text-foreground">{resendResult}</p>
        </ResultCard>
      )}
    </motion.div>
  );
};

// ─── Shared UI ─────────────────────────────────────────────────
const AgentCard = ({ agent, loading, children }: { agent: { icon: typeof Bot; color: string; gradient: string; label: string; desc: string }; loading?: boolean; children: React.ReactNode }) => (
  <div className={`p-4 rounded-2xl bg-gradient-to-br ${agent.gradient} border border-border/20`}>
    <div className="flex items-center gap-3 mb-1">
      {loading ? <Loader2 className="w-5 h-5 animate-spin text-muted-foreground" /> : <agent.icon className={`w-5 h-5 ${agent.color}`} />}
      <span className="text-sm font-semibold text-foreground">{agent.label}</span>
    </div>
    <p className="text-xs text-muted-foreground mb-1">{agent.desc}</p>
    {children}
  </div>
);

const ResultCard = ({ title, icon, children }: { title: string; icon: React.ReactNode; children: React.ReactNode }) => (
  <motion.div initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} className="p-4 rounded-2xl bg-card/50 border border-border/30">
    <h3 className="text-xs font-bold text-foreground mb-3 flex items-center gap-2">{icon} {title}</h3>
    {children}
  </motion.div>
);

const Btn = ({ loading, onClick, label }: { loading?: boolean; onClick: () => void; label: string }) => (
  <button onClick={onClick} disabled={loading}
    className="h-8 px-3 rounded-lg bg-primary text-primary-foreground text-[10px] font-semibold hover:bg-primary/90 disabled:opacity-50 transition-colors">
    {loading ? <Loader2 className="w-3 h-3 animate-spin" /> : label}
  </button>
);

export default MarketerAgentsPanel;

