import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import {
  TrendingUp, DollarSign, Users, Skull, FlaskConical,
  ArrowUpRight, BarChart3, Activity, Heart, Clock,
} from "lucide-react";
import { useAuth } from "@/context/AuthContext";
import {
  greetingApi, marketerInsightsApi, GreetingDto,
  RoasDto, LeadsDto, ZombieAdsDto, SentimentDto,
} from "@/services/api";

interface Props { onTabChange: (tab: string) => void; }

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

const MarketerOverviewPanel = ({ onTabChange }: Props) => {
  const { user } = useAuth();
  const [greeting, setGreeting] = useState<GreetingDto | null>(null);
  const [roas, setRoas] = useState<RoasDto | null>(null);
  const [leads, setLeads] = useState<LeadsDto | null>(null);
  const [zombies, setZombies] = useState<ZombieAdsDto | null>(null);
  const [sentiment, setSentiment] = useState<SentimentDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = async () => {
      const [gR, rR, lR, zR, sR] = await Promise.allSettled([
        greetingApi.get(user?.userName),
        marketerInsightsApi.totalRoas(),
        marketerInsightsApi.leadsGenerated(),
        marketerInsightsApi.zombieAdsKilled(),
        marketerInsightsApi.audienceSentiment(),
      ]);
      if (gR.status === "fulfilled" && gR.value.data.isSuccess) setGreeting(gR.value.data.data);
      if (rR.status === "fulfilled" && rR.value.data.isSuccess) setRoas(rR.value.data.data);
      if (lR.status === "fulfilled" && lR.value.data.isSuccess) setLeads(lR.value.data.data);
      if (zR.status === "fulfilled" && zR.value.data.isSuccess) setZombies(zR.value.data.data);
      if (sR.status === "fulfilled" && sR.value.data.isSuccess) setSentiment(sR.value.data.data);
      setLoading(false);
    };
    load();
  }, [user?.userName]);

  const displayName = user?.fullName?.split(" ")[0] || "Marketer";
  const greetText = greeting?.greeting || `Welcome back, ${displayName}`;

  const stats = [
    {
      label: "ROAS", value: roas ? `${roas.roas}x` : "—",
      sub: roas ? `$${roas.totalRevenue.toLocaleString()} / $${roas.totalSpend.toLocaleString()}` : "loading...",
      icon: DollarSign, color: "text-emerald-400", bg: "from-emerald-500/15 to-emerald-500/3", border: "border-emerald-500/15",
    },
    {
      label: "Leads Generated", value: leads?.totalLeads?.toLocaleString() || "0",
      sub: `${leads?.organicLeads || 0} organic · ${leads?.paidLeads || 0} paid`,
      icon: Users, color: "text-blue-400", bg: "from-blue-500/15 to-blue-500/3", border: "border-blue-500/15",
    },
    {
      label: "Zombie Ads Killed", value: zombies?.totalKilled?.toString() || "0",
      sub: zombies ? `$${zombies.moneySaved.toLocaleString()} saved` : "—",
      icon: Skull, color: "text-red-400", bg: "from-red-500/15 to-red-500/3", border: "border-red-500/15",
    },
  ];

  const quickActions = [
    { id: "marketer-insights", label: "Analytics", desc: "ROAS, leads, A/B tests, engagement", icon: BarChart3, gradient: "from-emerald-500/12 to-emerald-500/3" },
    { id: "marketer-utilities", label: "Marketing Tools", desc: "SEO, copywriting, readability", icon: Activity, gradient: "from-blue-500/12 to-blue-500/3" },
    { id: "marketer-agents", label: "AI Agents", desc: "Budget bleed, trends, competitors", icon: FlaskConical, gradient: "from-violet-500/12 to-violet-500/3" },
    { id: "marketer-scripts", label: "Automation", desc: "Campaigns, reports, social blast", icon: TrendingUp, gradient: "from-amber-500/12 to-amber-500/3" },
  ];

  if (loading) {
    return (
      <div className="flex items-center justify-center py-20">
        <div className="flex flex-col items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-emerald-500/10 flex items-center justify-center animate-pulse">
            <TrendingUp className="w-5 h-5 text-emerald-400" />
          </div>
          <p className="text-xs text-muted-foreground">Loading marketing dashboard...</p>
        </div>
      </div>
    );
  }

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-8">
      {/* Greeting */}
      <motion.div variants={item}>
        <h1 className="text-2xl font-bold text-foreground tracking-tight">{greetText}</h1>
        <p className="text-sm text-muted-foreground mt-1">Marketing Command Center — Track every dollar</p>
      </motion.div>

      {/* Sentiment Banner */}
      {sentiment && (
        <motion.div variants={item} className="p-4 rounded-2xl bg-gradient-to-r from-blue-500/10 via-blue-500/5 to-transparent border border-blue-500/15">
          <div className="flex items-center gap-4">
            <div className="w-12 h-12 rounded-2xl bg-blue-500/15 border border-blue-500/20 flex items-center justify-center">
              <Heart className="w-5 h-5 text-blue-400" />
            </div>
            <div className="flex-1">
              <p className="text-sm font-bold text-foreground">Audience Sentiment</p>
              <p className="text-xs text-muted-foreground mt-0.5">
                {sentiment.totalMentions.toLocaleString()} mentions —
                <span className="text-emerald-400 font-semibold"> {sentiment.positivePercent}% positive</span>,
                <span className="text-red-400 font-semibold"> {sentiment.negativePercent}% negative</span>,
                <span className="text-muted-foreground"> {sentiment.neutralPercent}% neutral</span>
              </p>
            </div>
            {/* Mini sentiment bar */}
            <div className="w-32 h-3 rounded-full overflow-hidden flex bg-muted/20">
              <div className="bg-emerald-500/70 h-full" style={{ width: `${sentiment.positivePercent}%` }} />
              <div className="bg-muted-foreground/30 h-full" style={{ width: `${sentiment.neutralPercent}%` }} />
              <div className="bg-red-500/70 h-full" style={{ width: `${sentiment.negativePercent}%` }} />
            </div>
          </div>
        </motion.div>
      )}

      {/* Stats */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        {stats.map((s, i) => (
          <motion.div key={i} variants={item}
            className={`p-5 rounded-2xl bg-gradient-to-br ${s.bg} border ${s.border} group hover:shadow-lg transition-all duration-300`}>
            <div className="flex items-center gap-3 mb-3">
              <div className={`w-9 h-9 rounded-xl bg-gradient-to-br ${s.bg} flex items-center justify-center`}>
                <s.icon className={`w-4.5 h-4.5 ${s.color}`} />
              </div>
              <span className="text-xs font-medium text-muted-foreground">{s.label}</span>
            </div>
            <p className="text-2xl font-bold text-foreground tracking-tight">{s.value}</p>
            <p className="text-xs text-muted-foreground mt-1">{s.sub}</p>
          </motion.div>
        ))}
      </div>

      {/* Quick Actions */}
      <motion.div variants={item}>
        <h2 className="text-sm font-bold text-foreground mb-4">Operations</h2>
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
          {quickActions.map((a) => (
            <motion.button key={a.id} variants={item} whileHover={{ y: -2, scale: 1.01 }} whileTap={{ scale: 0.99 }}
              onClick={() => onTabChange(a.id)}
              className={`group p-4 rounded-2xl bg-gradient-to-br ${a.gradient} border border-border/30 hover:border-primary/20 text-left transition-all duration-300 hover:shadow-lg`}>
              <div className="flex items-start justify-between mb-3">
                <div className="w-9 h-9 rounded-xl bg-card/60 border border-border/20 flex items-center justify-center">
                  <a.icon className="w-4 h-4 text-primary" />
                </div>
                <ArrowUpRight className="w-4 h-4 text-muted-foreground/30 group-hover:text-primary transition-all" />
              </div>
              <p className="text-sm font-semibold text-foreground">{a.label}</p>
              <p className="text-xs text-muted-foreground mt-0.5">{a.desc}</p>
            </motion.button>
          ))}
        </div>
      </motion.div>
    </motion.div>
  );
};

export default MarketerOverviewPanel;

