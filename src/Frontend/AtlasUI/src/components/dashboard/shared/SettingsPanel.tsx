import { useState, useEffect, useCallback } from "react";
import {AnimatePresence, motion} from "framer-motion";
import {
  User, Shield, Key, Webhook, CreditCard, HelpCircle, Trash2, Loader2, Save, AlertTriangle,
  Moon, Sun, Monitor, Bell, Mail, Smartphone, Clock, Copy, Check, Plus,
  RefreshCw, ExternalLink, FileText, Settings2,
} from "lucide-react";
import { useAuth } from "@/context/AuthContext";
import {
  preferencesApi, type PreferencesDto,
  auditLogsApi, type AuditLogDto, type SessionDto,
  personalTokensApi, type PersonalTokenDto,
  webhooksApi, type WebhookDto,
  subscriptionApi, type SubscriptionDto, type UsageDto, type InvoiceDto,
  supportApi, type SupportTicketDto,
  authApi, profileApi, type ProfileDto,
} from "@/services/api";

const tabs = [
  { id: "profile", label: "Profile", icon: User },
  { id: "preferences", label: "Preferences", icon: Settings2 },
  { id: "security", label: "Security", icon: Shield },
  { id: "tokens", label: "API Tokens", icon: Key },
  { id: "webhooks", label: "Webhooks", icon: Webhook },
  { id: "billing", label: "Billing", icon: CreditCard },
  { id: "support", label: "Support", icon: HelpCircle },
  { id: "danger", label: "Danger Zone", icon: AlertTriangle },
];

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 8 }, show: { opacity: 1, y: 0 } };

// ─── Profile Tab ─────────────────────────────────────────────────
function ProfileTab() {
  const { user } = useAuth();
  const [profile, setProfile] = useState<ProfileDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [form, setForm] = useState({ jobTitle: "", bio: "" });

  useEffect(() => {
    profileApi.getMe().then((r) => {
      if (r.data.isSuccess && r.data.data) {
        setProfile(r.data.data);
        setForm({ jobTitle: r.data.data.jobTitle || "", bio: r.data.data.bio || "" });
      }
    }).finally(() => setLoading(false));
  }, []);

  const save = async () => {
    setSaving(true);
    try { await profileApi.updateMe(form); } catch { /* empty */ }
    setSaving(false);
  };

  if (loading) return <div className="flex justify-center py-12"><Loader2 className="w-5 h-5 animate-spin text-primary" /></div>;

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="grid gap-4 sm:grid-cols-2">
        <div className="p-4 rounded-xl bg-card/50 border border-border/20">
          <label className="text-[10px] font-bold uppercase tracking-wider text-muted-foreground/50 mb-1 block">Full Name</label>
          <p className="text-sm font-semibold text-foreground">{user?.fullName}</p>
        </div>
        <div className="p-4 rounded-xl bg-card/50 border border-border/20">
          <label className="text-[10px] font-bold uppercase tracking-wider text-muted-foreground/50 mb-1 block">Email</label>
          <p className="text-sm font-semibold text-foreground">{user?.email}</p>
        </div>
        <div className="p-4 rounded-xl bg-card/50 border border-border/20">
          <label className="text-[10px] font-bold uppercase tracking-wider text-muted-foreground/50 mb-1 block">Profession</label>
          <p className="text-sm font-semibold text-foreground capitalize">{profile?.profession || user?.role || "—"}</p>
        </div>
        <div className="p-4 rounded-xl bg-card/50 border border-border/20">
          <label className="text-[10px] font-bold uppercase tracking-wider text-muted-foreground/50 mb-1 block">Tags</label>
          <div className="flex flex-wrap gap-1 mt-1">
            {(profile?.tags || []).map((t) => (
              <span key={t} className="px-2 py-0.5 rounded-full bg-primary/10 text-primary text-[10px] font-semibold">{t}</span>
            ))}
            {(!profile?.tags || profile.tags.length === 0) && <span className="text-xs text-muted-foreground/50">—</span>}
          </div>
        </div>
      </motion.div>
      <motion.div variants={item} className="space-y-3">
        <div>
          <label className="text-xs font-semibold text-foreground mb-1 block">Job Title</label>
          <input value={form.jobTitle} onChange={(e) => setForm(p => ({ ...p, jobTitle: e.target.value }))}
            className="w-full px-3 py-2 rounded-lg bg-background border border-border/30 text-sm focus:outline-none focus:ring-1 focus:ring-primary/40" />
        </div>
        <div>
          <label className="text-xs font-semibold text-foreground mb-1 block">Bio</label>
          <textarea value={form.bio} onChange={(e) => setForm(p => ({ ...p, bio: e.target.value }))} rows={3}
            className="w-full px-3 py-2 rounded-lg bg-background border border-border/30 text-sm focus:outline-none focus:ring-1 focus:ring-primary/40 resize-none" />
        </div>
        <button onClick={save} disabled={saving}
          className="flex items-center gap-2 px-4 py-2 rounded-lg bg-primary text-primary-foreground text-sm font-semibold hover:bg-primary/90 transition-colors disabled:opacity-50">
          {saving ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : <Save className="w-3.5 h-3.5" />} Save Profile
        </button>
      </motion.div>
    </motion.div>
  );
}

// ─── Preferences Tab ─────────────────────────────────────────────
function PreferencesTab() {
  const [prefs, setPrefs] = useState<PreferencesDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    preferencesApi.get().then((r) => { if (r.data.isSuccess) setPrefs(r.data.data); }).finally(() => setLoading(false));
  }, []);

  const save = async () => {
    if (!prefs) return;
    setSaving(true);
    try { await preferencesApi.update(prefs); } catch { /* empty */ }
    setSaving(false);
  };

  const toggle = (key: keyof PreferencesDto) => {
    if (!prefs) return;
    setPrefs({ ...prefs, [key]: !prefs[key] });
  };

  if (loading) return <div className="flex justify-center py-12"><Loader2 className="w-5 h-5 animate-spin text-primary" /></div>;
  if (!prefs) return <p className="text-sm text-muted-foreground text-center py-8">Failed to load preferences</p>;

  const themes = [
    { id: "system", label: "System", icon: Monitor },
    { id: "dark", label: "Dark", icon: Moon },
    { id: "light", label: "Light", icon: Sun },
  ];

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item}>
        <h3 className="text-sm font-bold text-foreground mb-3">Theme</h3>
        <div className="flex gap-2">
          {themes.map((t) => (
            <button key={t.id} onClick={() => setPrefs({ ...prefs, theme: t.id })}
              className={`flex items-center gap-2 px-4 py-2.5 rounded-xl border text-sm font-medium transition-all ${prefs.theme === t.id ? "border-primary bg-primary/10 text-primary" : "border-border/20 bg-card/30 text-muted-foreground hover:text-foreground"}`}>
              <t.icon className="w-4 h-4" /> {t.label}
            </button>
          ))}
        </div>
      </motion.div>
      <motion.div variants={item}>
        <h3 className="text-sm font-bold text-foreground mb-3">Language & Timezone</h3>
        <div className="grid gap-3 sm:grid-cols-2">
          <div>
            <label className="text-xs text-muted-foreground mb-1 block">Language</label>
            <select value={prefs.language} onChange={(e) => setPrefs({ ...prefs, language: e.target.value })}
              className="w-full px-3 py-2 rounded-lg bg-background border border-border/30 text-sm">
              <option value="en">English</option><option value="az">Azərbaycan</option>
              <option value="tr">Türkçe</option><option value="ru">Русский</option>
            </select>
          </div>
          <div>
            <label className="text-xs text-muted-foreground mb-1 block">Timezone</label>
            <input value={prefs.timezone} onChange={(e) => setPrefs({ ...prefs, timezone: e.target.value })}
              className="w-full px-3 py-2 rounded-lg bg-background border border-border/30 text-sm" placeholder="Asia/Baku" />
          </div>
        </div>
      </motion.div>
      <motion.div variants={item}>
        <h3 className="text-sm font-bold text-foreground mb-3">Notifications</h3>
        <div className="space-y-2">
          {([
            { key: "emailNotifications" as const, label: "Email Notifications", icon: Mail },
            { key: "pushNotifications" as const, label: "Push Notifications", icon: Smartphone },
            { key: "inboxAlerts" as const, label: "Inbox: Alerts (SecOps)", icon: Shield },
            { key: "inboxApprovals" as const, label: "Inbox: Approvals (Git)", icon: Bell },
            { key: "inboxMentions" as const, label: "Inbox: Mentions (Social)", icon: Bell },
            { key: "inboxSystem" as const, label: "Inbox: System Insights", icon: Bell },
            { key: "weeklyDigest" as const, label: "Weekly Digest Email", icon: Clock },
          ] as const).map(({ key, label, icon: Icon }) => (
            <button key={key} onClick={() => toggle(key)}
              className="w-full flex items-center justify-between px-4 py-3 rounded-xl border border-border/20 bg-card/30 hover:bg-card/50 transition-all">
              <div className="flex items-center gap-3">
                <Icon className="w-4 h-4 text-muted-foreground" />
                <span className="text-sm font-medium text-foreground">{label}</span>
              </div>
              <div className={`w-10 h-5 rounded-full transition-colors flex items-center ${prefs[key] ? "bg-primary justify-end" : "bg-muted/40 justify-start"}`}>
                <div className="w-4 h-4 rounded-full bg-white shadow-sm mx-0.5" />
              </div>
            </button>
          ))}
        </div>
      </motion.div>
      <motion.div variants={item}>
        <button onClick={save} disabled={saving}
          className="flex items-center gap-2 px-4 py-2 rounded-lg bg-primary text-primary-foreground text-sm font-semibold hover:bg-primary/90 transition-colors disabled:opacity-50">
          {saving ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : <Save className="w-3.5 h-3.5" />} Save Preferences
        </button>
      </motion.div>
    </motion.div>
  );
}

// ─── Security Tab ─────────────────────────────────────────────
function SecurityTab() {
  const [logs, setLogs] = useState<AuditLogDto[]>([]);
  const [sessions, setSessions] = useState<SessionDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([
      auditLogsApi.getAll({ page: 1, pageSize: 20 }),
      auditLogsApi.getSessions(),
    ]).then(([lRes, sRes]) => {
      if (lRes.data.isSuccess) setLogs(lRes.data.data || []);
      if (sRes.data.isSuccess) setSessions(sRes.data.data || []);
    }).finally(() => setLoading(false));
  }, []);

  if (loading) return <div className="flex justify-center py-12"><Loader2 className="w-5 h-5 animate-spin text-primary" /></div>;

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item}>
        <h3 className="text-sm font-bold text-foreground mb-3">Active Sessions</h3>
        <div className="space-y-2">
          {sessions.map((s, i) => (
            <div key={i} className={`p-4 rounded-xl border ${s.isCurrent ? "border-primary/30 bg-primary/5" : "border-border/20 bg-card/30"}`}>
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-xs font-semibold text-foreground">{s.ipAddress} {s.isCurrent && <span className="text-primary text-[10px]">(current)</span>}</p>
                  <p className="text-[10px] text-muted-foreground mt-0.5 line-clamp-1">{s.userAgent}</p>
                </div>
                <p className="text-[10px] text-muted-foreground">{new Date(s.lastLoginAt).toLocaleDateString()}</p>
              </div>
            </div>
          ))}
          {sessions.length === 0 && <p className="text-xs text-muted-foreground text-center py-4">No sessions found</p>}
        </div>
      </motion.div>
      <motion.div variants={item}>
        <h3 className="text-sm font-bold text-foreground mb-3">Audit Log</h3>
        <div className="border border-border/20 rounded-xl overflow-hidden">
          <table className="w-full text-xs">
            <thead><tr className="bg-muted/10 border-b border-border/20">
              <th className="text-left px-4 py-2 font-semibold text-muted-foreground">Action</th>
              <th className="text-left px-4 py-2 font-semibold text-muted-foreground">IP</th>
              <th className="text-left px-4 py-2 font-semibold text-muted-foreground">Date</th>
            </tr></thead>
            <tbody>
              {logs.map((l) => (
                <tr key={l.id} className="border-b border-border/10 hover:bg-muted/5">
                  <td className="px-4 py-2.5 font-medium text-foreground">{l.action}</td>
                  <td className="px-4 py-2.5 text-muted-foreground">{l.ipAddress || "—"}</td>
                  <td className="px-4 py-2.5 text-muted-foreground">{new Date(l.createdAt).toLocaleString()}</td>
                </tr>
              ))}
            </tbody>
          </table>
          {logs.length === 0 && <p className="text-xs text-muted-foreground text-center py-6">No audit logs</p>}
        </div>
      </motion.div>
    </motion.div>
  );
}

// ─── Tokens Tab ─────────────────────────────────────────────
function TokensTab() {
  const [tokens, setTokens] = useState<PersonalTokenDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [creating, setCreating] = useState(false);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState({ name: "", scopes: "read:workspaces" });
  const [createdToken, setCreatedToken] = useState<string | null>(null);
  const [copied, setCopied] = useState(false);

  const fetch = useCallback(async () => {
    setLoading(true);
    try { const r = await personalTokensApi.getAll(); if (r.data.isSuccess) setTokens(r.data.data || []); } catch { /* empty */ }
    setLoading(false);
  }, []);

  useEffect(() => { fetch(); }, [fetch]);

  const create = async () => {
    setCreating(true);
    try {
      const r = await personalTokensApi.create({ name: form.name, scopes: form.scopes.split(",").map(s => s.trim()) });
      if (r.data.isSuccess && r.data.data?.token) {
        setCreatedToken(r.data.data.token);
        setShowForm(false);
        fetch();
      }
    } catch { /* empty */ }
    setCreating(false);
  };

  const revoke = async (id: string) => {
    await personalTokensApi.revoke(id);
    fetch();
  };

  const copyToken = () => {
    if (createdToken) { navigator.clipboard.writeText(createdToken); setCopied(true); setTimeout(() => setCopied(false), 2000); }
  };

  if (loading) return <div className="flex justify-center py-12"><Loader2 className="w-5 h-5 animate-spin text-primary" /></div>;

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-4">
      {createdToken && (
        <motion.div variants={item} className="p-4 rounded-xl border border-green-500/30 bg-green-500/5">
          <p className="text-xs font-semibold text-green-400 mb-2">⚠️ Copy this token now — you won't see it again!</p>
          <div className="flex items-center gap-2">
            <code className="flex-1 px-3 py-2 rounded bg-background border border-border/30 text-xs font-mono text-foreground break-all">{createdToken}</code>
            <button onClick={copyToken} className="px-3 py-2 rounded-lg bg-primary/10 text-primary text-xs font-semibold hover:bg-primary/20">
              {copied ? <Check className="w-3.5 h-3.5" /> : <Copy className="w-3.5 h-3.5" />}
            </button>
          </div>
          <button onClick={() => setCreatedToken(null)} className="text-[10px] text-muted-foreground mt-2 hover:text-foreground">Dismiss</button>
        </motion.div>
      )}
      <motion.div variants={item} className="flex items-center justify-between">
        <h3 className="text-sm font-bold text-foreground">Personal Access Tokens</h3>
        <button onClick={() => setShowForm(!showForm)}
          className="flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-primary text-primary-foreground text-xs font-semibold">
          <Plus className="w-3 h-3" /> New Token
        </button>
      </motion.div>
      {showForm && (
        <motion.div variants={item} className="p-4 rounded-xl border border-border/20 bg-card/30 space-y-3">
          <input value={form.name} onChange={(e) => setForm(p => ({ ...p, name: e.target.value }))} placeholder="Token name"
            className="w-full px-3 py-2 rounded-lg bg-background border border-border/30 text-sm" />
          <input value={form.scopes} onChange={(e) => setForm(p => ({ ...p, scopes: e.target.value }))} placeholder="Scopes (comma separated)"
            className="w-full px-3 py-2 rounded-lg bg-background border border-border/30 text-sm" />
          <div className="flex gap-2">
            <button onClick={create} disabled={creating || !form.name}
              className="px-3 py-1.5 rounded-lg bg-primary text-primary-foreground text-xs font-semibold disabled:opacity-50">
              {creating ? <Loader2 className="w-3 h-3 animate-spin" /> : "Create"}
            </button>
            <button onClick={() => setShowForm(false)} className="px-3 py-1.5 rounded-lg bg-muted/30 text-muted-foreground text-xs">Cancel</button>
          </div>
        </motion.div>
      )}
      <div className="space-y-2">
        {tokens.map((t) => (
          <motion.div key={t.id} variants={item} className={`p-4 rounded-xl border border-border/20 bg-card/30 ${t.isRevoked ? "opacity-40" : ""}`}>
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-semibold text-foreground">{t.name}</p>
                <p className="text-[10px] text-muted-foreground mt-0.5">{t.tokenPrefix}••• · {t.scopes.join(", ")}</p>
                {t.lastUsedAt && <p className="text-[10px] text-muted-foreground/50">Last used: {new Date(t.lastUsedAt).toLocaleDateString()}</p>}
              </div>
              {!t.isRevoked && (
                <button onClick={() => revoke(t.id)} className="px-3 py-1.5 rounded-lg text-xs text-red-400 hover:bg-red-400/10 font-semibold transition-colors">
                  Revoke
                </button>
              )}
            </div>
          </motion.div>
        ))}
        {tokens.length === 0 && <p className="text-xs text-muted-foreground text-center py-8">No tokens created yet</p>}
      </div>
    </motion.div>
  );
}

// ─── Webhooks Tab ─────────────────────────────────────────────
function WebhooksTab() {
  const [webhooks, setWebhooks] = useState<WebhookDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState({ name: "", url: "", events: "AlertFired" });

  const fetch = useCallback(async () => {
    setLoading(true);
    try { const r = await webhooksApi.getAll(); if (r.data.isSuccess) setWebhooks(r.data.data || []); } catch { /* empty */ }
    setLoading(false);
  }, []);

  useEffect(() => { fetch(); }, [fetch]);

  const create = async () => {
    await webhooksApi.create({ name: form.name, url: form.url, events: form.events.split(",").map(s => s.trim()) });
    setShowForm(false);
    fetch();
  };

  const toggle = async (id: string, active: boolean) => { await webhooksApi.toggle(id, !active); fetch(); };
  const remove = async (id: string) => { await webhooksApi.remove(id); fetch(); };

  if (loading) return <div className="flex justify-center py-12"><Loader2 className="w-5 h-5 animate-spin text-primary" /></div>;

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-4">
      <motion.div variants={item} className="flex items-center justify-between">
        <h3 className="text-sm font-bold text-foreground">Webhooks</h3>
        <button onClick={() => setShowForm(!showForm)}
          className="flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-primary text-primary-foreground text-xs font-semibold">
          <Plus className="w-3 h-3" /> Add Webhook
        </button>
      </motion.div>
      {showForm && (
        <motion.div variants={item} className="p-4 rounded-xl border border-border/20 bg-card/30 space-y-3">
          <input value={form.name} onChange={(e) => setForm(p => ({ ...p, name: e.target.value }))} placeholder="Webhook name"
            className="w-full px-3 py-2 rounded-lg bg-background border border-border/30 text-sm" />
          <input value={form.url} onChange={(e) => setForm(p => ({ ...p, url: e.target.value }))} placeholder="https://hooks.slack.com/..."
            className="w-full px-3 py-2 rounded-lg bg-background border border-border/30 text-sm" />
          <input value={form.events} onChange={(e) => setForm(p => ({ ...p, events: e.target.value }))} placeholder="Events (comma separated)"
            className="w-full px-3 py-2 rounded-lg bg-background border border-border/30 text-sm" />
          <button onClick={create} disabled={!form.name || !form.url}
            className="px-3 py-1.5 rounded-lg bg-primary text-primary-foreground text-xs font-semibold disabled:opacity-50">Create</button>
        </motion.div>
      )}
      <div className="space-y-2">
        {webhooks.map((w) => (
          <motion.div key={w.id} variants={item} className="p-4 rounded-xl border border-border/20 bg-card/30">
            <div className="flex items-center justify-between">
              <div className="flex-1 min-w-0">
                <div className="flex items-center gap-2">
                  <p className="text-sm font-semibold text-foreground">{w.name}</p>
                  <span className={`px-1.5 py-0.5 rounded text-[9px] font-bold ${w.active ? "bg-green-500/20 text-green-400" : "bg-muted/30 text-muted-foreground"}`}>
                    {w.active ? "ACTIVE" : "PAUSED"}
                  </span>
                  {w.failCount >= 5 && <span className="px-1.5 py-0.5 rounded text-[9px] font-bold bg-red-500/20 text-red-400">⚠ {w.failCount} FAILS</span>}
                </div>
                <p className="text-[10px] text-muted-foreground mt-0.5 truncate">{w.url}</p>
                <p className="text-[10px] text-muted-foreground/50 mt-0.5">{w.events.join(", ")}</p>
              </div>
              <div className="flex items-center gap-1">
                <button onClick={() => toggle(w.id, w.active)} className="w-7 h-7 rounded flex items-center justify-center text-muted-foreground hover:text-foreground">
                  <RefreshCw className="w-3.5 h-3.5" />
                </button>
                <button onClick={() => remove(w.id)} className="w-7 h-7 rounded flex items-center justify-center text-muted-foreground hover:text-red-400">
                  <Trash2 className="w-3.5 h-3.5" />
                </button>
              </div>
            </div>
          </motion.div>
        ))}
        {webhooks.length === 0 && <p className="text-xs text-muted-foreground text-center py-8">No webhooks configured</p>}
      </div>
    </motion.div>
  );
}

// ─── Billing Tab ─────────────────────────────────────────────
function BillingTab() {
  const [sub, setSub] = useState<SubscriptionDto | null>(null);
  const [usage, setUsage] = useState<UsageDto | null>(null);
  const [invoices, setInvoices] = useState<InvoiceDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([
      subscriptionApi.getCurrent(),
      subscriptionApi.getUsage(),
      subscriptionApi.getInvoices(),
    ]).then(([sRes, uRes, iRes]) => {
      if (sRes.data.isSuccess) setSub(sRes.data.data);
      if (uRes.data.isSuccess) setUsage(uRes.data.data);
      if (iRes.data.isSuccess) setInvoices(iRes.data.data || []);
    }).finally(() => setLoading(false));
  }, []);

  const openPortal = async () => {
    try {
      const r = await subscriptionApi.portal({ returnUrl: window.location.href });
      if (r.data.isSuccess && r.data.data?.url) window.open(r.data.data.url, "_blank");
    } catch { /* empty */ }
  };

  if (loading) return <div className="flex justify-center py-12"><Loader2 className="w-5 h-5 animate-spin text-primary" /></div>;

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="grid gap-4 sm:grid-cols-2">
        <div className="p-5 rounded-xl border border-border/20 bg-card/30">
          <p className="text-[10px] font-bold uppercase tracking-wider text-muted-foreground/50 mb-1">Plan</p>
          <p className="text-2xl font-black text-foreground">{sub?.tier || "Free"}</p>
          <p className="text-xs text-muted-foreground mt-1">{sub?.status || "active"}</p>
        </div>
        <div className="p-5 rounded-xl border border-border/20 bg-card/30">
          <p className="text-[10px] font-bold uppercase tracking-wider text-muted-foreground/50 mb-1">Usage</p>
          <p className="text-sm text-foreground mt-1">Workspaces: <span className="font-bold">{usage?.workspacesUsed || 0}</span> / {usage?.workspacesLimit || "∞"}</p>
          <p className="text-sm text-foreground">Integrations: <span className="font-bold">{usage?.integrationsUsed || 0}</span> / {usage?.integrationsLimit || "∞"}</p>
        </div>
      </motion.div>
      <motion.div variants={item}>
        <button onClick={openPortal}
          className="flex items-center gap-2 px-4 py-2 rounded-lg bg-primary text-primary-foreground text-sm font-semibold hover:bg-primary/90">
          <ExternalLink className="w-3.5 h-3.5" /> Manage Subscription
        </button>
      </motion.div>
      {invoices.length > 0 && (
        <motion.div variants={item}>
          <h3 className="text-sm font-bold text-foreground mb-3">Invoices</h3>
          <div className="space-y-2">
            {invoices.map((inv) => (
              <div key={inv.id} className="flex items-center justify-between p-3 rounded-xl border border-border/20 bg-card/30">
                <div className="flex items-center gap-3">
                  <FileText className="w-4 h-4 text-muted-foreground" />
                  <div>
                    <p className="text-xs font-semibold text-foreground">{new Date(inv.date).toLocaleDateString()}</p>
                    <p className="text-[10px] text-muted-foreground">${(inv.amountPaid / 100).toFixed(2)} {inv.currency.toUpperCase()}</p>
                  </div>
                </div>
                <div className="flex items-center gap-2">
                  <span className={`px-1.5 py-0.5 rounded text-[9px] font-bold ${inv.status === "paid" ? "bg-green-500/20 text-green-400" : "bg-muted/30 text-muted-foreground"}`}>{inv.status}</span>
                  {inv.pdfUrl && <a href={inv.pdfUrl} target="_blank" rel="noreferrer" className="text-primary text-xs hover:underline">PDF</a>}
                </div>
              </div>
            ))}
          </div>
        </motion.div>
      )}
    </motion.div>
  );
}

// ─── Support Tab ─────────────────────────────────────────────
function SupportTab() {
  const [tickets, setTickets] = useState<SupportTicketDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState({ type: "Bug", subject: "", body: "" });

  const fetch = useCallback(async () => {
    setLoading(true);
    try { const r = await supportApi.getTickets(); if (r.data.isSuccess) setTickets(r.data.data || []); } catch { /* empty */ }
    setLoading(false);
  }, []);

  useEffect(() => { fetch(); }, [fetch]);

  const create = async () => {
    await supportApi.create({ ...form, pageUrl: window.location.pathname, browserInfo: navigator.userAgent });
    setShowForm(false);
    setForm({ type: "Bug", subject: "", body: "" });
    fetch();
  };

  if (loading) return <div className="flex justify-center py-12"><Loader2 className="w-5 h-5 animate-spin text-primary" /></div>;

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-4">
      <motion.div variants={item} className="flex items-center justify-between">
        <h3 className="text-sm font-bold text-foreground">Support Tickets</h3>
        <button onClick={() => setShowForm(!showForm)}
          className="flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-primary text-primary-foreground text-xs font-semibold">
          <Plus className="w-3 h-3" /> New Ticket
        </button>
      </motion.div>
      {showForm && (
        <motion.div variants={item} className="p-4 rounded-xl border border-border/20 bg-card/30 space-y-3">
          <select value={form.type} onChange={(e) => setForm(p => ({ ...p, type: e.target.value }))}
            className="w-full px-3 py-2 rounded-lg bg-background border border-border/30 text-sm">
            <option value="Bug">Bug</option><option value="FeatureRequest">Feature Request</option>
            <option value="Question">Question</option><option value="General">General</option>
          </select>
          <input value={form.subject} onChange={(e) => setForm(p => ({ ...p, subject: e.target.value }))} placeholder="Subject"
            className="w-full px-3 py-2 rounded-lg bg-background border border-border/30 text-sm" />
          <textarea value={form.body} onChange={(e) => setForm(p => ({ ...p, body: e.target.value }))} placeholder="Describe the issue..." rows={3}
            className="w-full px-3 py-2 rounded-lg bg-background border border-border/30 text-sm resize-none" />
          <button onClick={create} disabled={!form.subject || !form.body}
            className="px-3 py-1.5 rounded-lg bg-primary text-primary-foreground text-xs font-semibold disabled:opacity-50">Submit</button>
        </motion.div>
      )}
      <div className="space-y-2">
        {tickets.map((t) => (
          <motion.div key={t.id} variants={item} className="p-4 rounded-xl border border-border/20 bg-card/30">
            <div className="flex items-center justify-between">
              <div>
                <div className="flex items-center gap-2">
                  <span className="text-[10px] font-bold uppercase text-muted-foreground/50">{t.type}</span>
                  <span className={`px-1.5 py-0.5 rounded text-[9px] font-bold ${t.status === "Open" ? "bg-blue-500/20 text-blue-400" : t.status === "Resolved" ? "bg-green-500/20 text-green-400" : "bg-muted/30 text-muted-foreground"}`}>{t.status}</span>
                </div>
                <p className="text-sm font-semibold text-foreground mt-0.5">{t.subject}</p>
                <p className="text-[10px] text-muted-foreground mt-0.5">{new Date(t.createdAt).toLocaleDateString()}</p>
              </div>
              {t.status === "Open" && (
                <button onClick={() => supportApi.close(t.id).then(fetch)} className="text-xs text-muted-foreground hover:text-foreground">Close</button>
              )}
            </div>
          </motion.div>
        ))}
        {tickets.length === 0 && <p className="text-xs text-muted-foreground text-center py-8">No tickets yet</p>}
      </div>
    </motion.div>
  );
}

// ─── Danger Zone Tab ─────────────────────────────────────────
function DangerZoneTab() {
  const { logout } = useAuth();
  const [confirm, setConfirm] = useState("");
  const [deleting, setDeleting] = useState(false);

  const deleteAccount = async () => {
    if (confirm !== "DELETE") return;
    setDeleting(true);
    try { await authApi.deleteAccount(); logout(); } catch { /* empty */ }
    setDeleting(false);
  };

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="p-6 rounded-xl border-2 border-red-500/20 bg-red-500/5">
        <div className="flex items-center gap-3 mb-4">
          <AlertTriangle className="w-5 h-5 text-red-400" />
          <h3 className="text-sm font-bold text-red-400">Delete Account</h3>
        </div>
        <p className="text-xs text-muted-foreground mb-4">
          This action is <strong>irreversible</strong>. All your data, workspaces, integrations, and settings will be permanently deleted.
        </p>
        <div className="space-y-3">
          <div>
            <label className="text-xs text-muted-foreground mb-1 block">Type <strong className="text-red-400">DELETE</strong> to confirm</label>
            <input value={confirm} onChange={(e) => setConfirm(e.target.value)} placeholder="DELETE"
              className="w-full max-w-xs px-3 py-2 rounded-lg bg-background border border-red-500/30 text-sm focus:ring-1 focus:ring-red-500/40" />
          </div>
          <button onClick={deleteAccount} disabled={confirm !== "DELETE" || deleting}
            className="flex items-center gap-2 px-4 py-2 rounded-lg bg-red-500 text-white text-sm font-semibold disabled:opacity-30 hover:bg-red-600 transition-colors">
            {deleting ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : <Trash2 className="w-3.5 h-3.5" />} Delete My Account
          </button>
        </div>
      </motion.div>
    </motion.div>
  );
}

// ─── Main Settings Panel ─────────────────────────────────────
export default function SettingsPanel() {
  const [activeTab, setActiveTab] = useState("profile");

  const renderTab = () => {
    switch (activeTab) {
      case "profile": return <ProfileTab />;
      case "preferences": return <PreferencesTab />;
      case "security": return <SecurityTab />;
      case "tokens": return <TokensTab />;
      case "webhooks": return <WebhooksTab />;
      case "billing": return <BillingTab />;
      case "support": return <SupportTab />;
      case "danger": return <DangerZoneTab />;
      default: return <ProfileTab />;
    }
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-primary/20 to-primary/5 border border-primary/10 flex items-center justify-center">
          <Settings2 className="w-5 h-5 text-primary" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground">Settings</h2>
          <p className="text-xs text-muted-foreground">Manage your account and preferences</p>
        </div>
      </div>

      {/* Tab Navigation */}
      <div className="flex flex-wrap gap-1 border-b border-border/20 pb-3">
        {tabs.map((t) => (
          <button key={t.id} onClick={() => setActiveTab(t.id)}
            className={`flex items-center gap-1.5 px-3 py-2 rounded-lg text-xs font-semibold transition-all ${activeTab === t.id ? "bg-primary/10 text-primary" : "text-muted-foreground hover:text-foreground hover:bg-muted/20"} ${t.id === "danger" ? "!text-red-400" : ""}`}>
            <t.icon className="w-3.5 h-3.5" /> {t.label}
          </button>
        ))}
      </div>

      {/* Tab Content */}
      <AnimatePresence mode="wait">
        <motion.div key={activeTab} initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -8 }} transition={{ duration: 0.2 }}>
          {renderTab()}
        </motion.div>
      </AnimatePresence>
    </div>
  );
}

