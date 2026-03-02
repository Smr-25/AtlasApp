import { useState, useEffect, useCallback } from "react";
import { motion, AnimatePresence } from "framer-motion";
import {
  Bell, Shield, GitPullRequest, AtSign, Sparkles,
  Check, CheckCheck, Trash2, Loader2, Play, Filter,
} from "lucide-react";
import { notificationsApi, type NotificationDto, type UnreadCountDto } from "@/services/api";

const categories = [
  { id: null as string | null, label: "All", icon: Bell },
  { id: "AlertsSecOps", label: "Alerts", icon: Shield, color: "text-red-400" },
  { id: "ApprovalsGit", label: "Approvals", icon: GitPullRequest, color: "text-green-400" },
  { id: "MentionsSocial", label: "Mentions", icon: AtSign, color: "text-blue-400" },
  { id: "SystemInsights", label: "Insights", icon: Sparkles, color: "text-purple-400" },
];

const priorityColors: Record<string, string> = {
  Critical: "border-l-red-500 bg-red-500/5",
  High: "border-l-orange-500 bg-orange-500/5",
  Normal: "border-l-blue-500/30 bg-card/50",
  Low: "border-l-border/30 bg-card/30",
};

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.03 } } };
const item = { hidden: { opacity: 0, y: 8 }, show: { opacity: 1, y: 0 } };

export default function NotificationsPanel() {
  const [activeCategory, setActiveCategory] = useState<string | null>(null);
  const [unreadOnly, setUnreadOnly] = useState(false);
  const [notifications, setNotifications] = useState<NotificationDto[]>([]);
  const [counts, setCounts] = useState<UnreadCountDto | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchData = useCallback(async () => {
    setLoading(true);
    try {
      const [nRes, cRes] = await Promise.all([
        notificationsApi.getAll({ category: activeCategory || undefined, unreadOnly, page: 1, pageSize: 50 }),
        notificationsApi.getUnreadCount(),
      ]);
      if (nRes.data.isSuccess) setNotifications(nRes.data.data || []);
      if (cRes.data.isSuccess) setCounts(cRes.data.data);
    } catch { /* empty */ }
    setLoading(false);
  }, [activeCategory, unreadOnly]);

  useEffect(() => { fetchData(); }, [fetchData]);

  const markRead = async (id: string) => {
    await notificationsApi.markAsRead(id);
    fetchData();
  };

  const markAllRead = async () => {
    await notificationsApi.markAllAsRead(activeCategory || undefined);
    fetchData();
  };

  const executeAction = async (n: NotificationDto) => {
    try {
      await notificationsApi.execute(n.id);
      fetchData();
    } catch { /* empty */ }
  };

  const remove = async (id: string) => {
    await notificationsApi.remove(id);
    fetchData();
  };

  const getCategoryCount = (cat: string | null): number => {
    if (!counts) return 0;
    if (!cat) return counts.total;
    const map: Record<string, number> = {
      AlertsSecOps: counts.alertsSecOps,
      ApprovalsGit: counts.approvalsGit,
      MentionsSocial: counts.mentionsSocial,
      SystemInsights: counts.systemInsights,
    };
    return map[cat] || 0;
  };

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      {/* Header */}
      <motion.div variants={item} className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-primary/20 to-primary/5 border border-primary/10 flex items-center justify-center">
            <Bell className="w-5 h-5 text-primary" />
          </div>
          <div>
            <h2 className="text-lg font-bold text-foreground">Smart Inbox</h2>
            <p className="text-xs text-muted-foreground">{counts?.total || 0} unread notifications</p>
          </div>
        </div>
        <div className="flex items-center gap-2">
          <button onClick={() => setUnreadOnly(!unreadOnly)}
            className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-medium transition-all ${unreadOnly ? "bg-primary/10 text-primary" : "bg-muted/30 text-muted-foreground hover:text-foreground"}`}>
            <Filter className="w-3 h-3" /> {unreadOnly ? "Unread" : "All"}
          </button>
          <button onClick={markAllRead}
            className="flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-muted/30 text-xs font-medium text-muted-foreground hover:text-foreground transition-all">
            <CheckCheck className="w-3 h-3" /> Mark all read
          </button>
        </div>
      </motion.div>

      {/* Category Tabs */}
      <motion.div variants={item} className="flex gap-1.5 border-b border-border/20 pb-3">
        {categories.map((c) => {
          const count = getCategoryCount(c.id);
          const active = activeCategory === c.id;
          return (
            <button key={c.id || "all"} onClick={() => setActiveCategory(c.id)}
              className={`flex items-center gap-1.5 px-3 py-2 rounded-lg text-xs font-semibold transition-all ${active ? "bg-primary/10 text-primary" : "text-muted-foreground hover:text-foreground hover:bg-muted/20"}`}>
              <c.icon className={`w-3.5 h-3.5 ${c.color || ""}`} />
              {c.label}
              {count > 0 && (
                <span className={`px-1.5 py-0.5 rounded-full text-[10px] font-bold ${active ? "bg-primary/20 text-primary" : "bg-muted/50 text-muted-foreground"}`}>
                  {count}
                </span>
              )}
            </button>
          );
        })}
      </motion.div>

      {/* Notification List */}
      {loading ? (
        <div className="flex items-center justify-center py-16"><Loader2 className="w-5 h-5 animate-spin text-primary" /></div>
      ) : notifications.length === 0 ? (
        <motion.div variants={item} className="text-center py-16">
          <Bell className="w-8 h-8 text-muted-foreground/30 mx-auto mb-3" />
          <p className="text-sm text-muted-foreground">No notifications</p>
        </motion.div>
      ) : (
        <motion.div variants={container} initial="hidden" animate="show" className="space-y-2">
          <AnimatePresence>
            {notifications.map((n) => (
              <motion.div key={n.id} variants={item} layout
                className={`group relative p-4 rounded-xl border-l-2 border border-border/20 transition-all hover:border-border/40 ${priorityColors[n.priority] || priorityColors.Normal} ${n.isRead ? "opacity-60" : ""}`}>
                <div className="flex items-start justify-between gap-3">
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center gap-2 mb-1">
                      <span className="text-[10px] font-bold uppercase tracking-wider text-muted-foreground/60">{n.category.replace(/([A-Z])/g, " $1").trim()}</span>
                      {n.priority === "Critical" && <span className="px-1.5 py-0.5 rounded text-[9px] font-bold bg-red-500/20 text-red-400">CRITICAL</span>}
                      {n.priority === "High" && <span className="px-1.5 py-0.5 rounded text-[9px] font-bold bg-orange-500/20 text-orange-400">HIGH</span>}
                    </div>
                    <h4 className="text-sm font-semibold text-foreground mb-0.5">{n.title}</h4>
                    <p className="text-xs text-muted-foreground line-clamp-2">{n.body}</p>
                    <p className="text-[10px] text-muted-foreground/50 mt-1.5">{new Date(n.createdAt).toLocaleString()}</p>
                  </div>
                  <div className="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                    {n.actionType && (
                      <button onClick={() => executeAction(n)} title="Execute action"
                        className="w-7 h-7 rounded-lg flex items-center justify-center text-primary hover:bg-primary/10 transition-colors">
                        <Play className="w-3.5 h-3.5" />
                      </button>
                    )}
                    {!n.isRead && (
                      <button onClick={() => markRead(n.id)} title="Mark as read"
                        className="w-7 h-7 rounded-lg flex items-center justify-center text-muted-foreground hover:text-foreground hover:bg-muted/30 transition-colors">
                        <Check className="w-3.5 h-3.5" />
                      </button>
                    )}
                    <button onClick={() => remove(n.id)} title="Delete"
                      className="w-7 h-7 rounded-lg flex items-center justify-center text-muted-foreground hover:text-red-400 hover:bg-red-400/10 transition-colors">
                      <Trash2 className="w-3.5 h-3.5" />
                    </button>
                  </div>
                </div>
              </motion.div>
            ))}
          </AnimatePresence>
        </motion.div>
      )}
    </motion.div>
  );
}

