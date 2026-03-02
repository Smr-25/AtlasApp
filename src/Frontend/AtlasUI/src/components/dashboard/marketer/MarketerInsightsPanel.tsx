import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import {
  BarChart3, DollarSign, Users, Skull, FlaskConical,
  Clock, TrendingUp, Heart, Timer,
} from "lucide-react";
import {
  marketerInsightsApi, RoasDto, LeadsDto, ZombieAdsDto,
  AbTestDto, PeakEngagementDto, SentimentDto, TimeSavedReportingDto,
} from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.05 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

const MarketerInsightsPanel = () => {
  const [roas, setRoas] = useState<RoasDto | null>(null);
  const [leads, setLeads] = useState<LeadsDto | null>(null);
  const [zombies, setZombies] = useState<ZombieAdsDto | null>(null);
  const [abTest, setAbTest] = useState<AbTestDto | null>(null);
  const [peak, setPeak] = useState<PeakEngagementDto | null>(null);
  const [sentiment, setSentiment] = useState<SentimentDto | null>(null);
  const [timeSaved, setTimeSaved] = useState<TimeSavedReportingDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = async () => {
      const results = await Promise.allSettled([
        marketerInsightsApi.totalRoas(),
        marketerInsightsApi.leadsGenerated(),
        marketerInsightsApi.zombieAdsKilled(),
        marketerInsightsApi.abTestWinRate(),
        marketerInsightsApi.peakEngagement(),
        marketerInsightsApi.audienceSentiment(),
        marketerInsightsApi.timeSavedReporting(),
      ]);
      if (results[0].status === "fulfilled" && results[0].value.data.isSuccess) setRoas(results[0].value.data.data);
      if (results[1].status === "fulfilled" && results[1].value.data.isSuccess) setLeads(results[1].value.data.data);
      if (results[2].status === "fulfilled" && results[2].value.data.isSuccess) setZombies(results[2].value.data.data);
      if (results[3].status === "fulfilled" && results[3].value.data.isSuccess) setAbTest(results[3].value.data.data);
      if (results[4].status === "fulfilled" && results[4].value.data.isSuccess) setPeak(results[4].value.data.data);
      if (results[5].status === "fulfilled" && results[5].value.data.isSuccess) setSentiment(results[5].value.data.data);
      if (results[6].status === "fulfilled" && results[6].value.data.isSuccess) setTimeSaved(results[6].value.data.data);
      setLoading(false);
    };
    load();
  }, []);

  if (loading) {
    return (
      <div className="flex items-center justify-center py-20">
        <div className="flex flex-col items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-emerald-500/10 flex items-center justify-center animate-pulse">
            <BarChart3 className="w-5 h-5 text-emerald-400" />
          </div>
          <p className="text-xs text-muted-foreground">Loading marketing analytics...</p>
        </div>
      </div>
    );
  }

  const kpiCards = [
    { label: "ROAS", value: roas ? `${roas.roas}x` : "—", sub: roas ? `$${roas.totalRevenue.toLocaleString()} rev` : "", icon: DollarSign, color: "text-emerald-400", bg: "from-emerald-500/15 to-emerald-500/3" },
    { label: "Leads", value: leads?.totalLeads?.toLocaleString() || "0", sub: `${leads?.paidLeads || 0} paid`, icon: Users, color: "text-blue-400", bg: "from-blue-500/15 to-blue-500/3" },
    { label: "Zombie Ads", value: zombies?.totalKilled?.toString() || "0", sub: zombies ? `$${zombies.moneySaved.toFixed(0)} saved` : "", icon: Skull, color: "text-red-400", bg: "from-red-500/15 to-red-500/3" },
    { label: "A/B Win Rate", value: abTest ? `${abTest.winRate}%` : "—", sub: abTest ? `${abTest.wins}/${abTest.totalTests} tests` : "", icon: FlaskConical, color: "text-violet-400", bg: "from-violet-500/15 to-violet-500/3" },
    { label: "Time Saved", value: timeSaved ? `${timeSaved.hoursSaved}h` : "—", sub: timeSaved ? `${timeSaved.reportsGenerated} reports` : "", icon: Timer, color: "text-amber-400", bg: "from-amber-500/15 to-amber-500/3" },
    { label: "Sentiment", value: sentiment ? `${sentiment.positivePercent}%` : "—", sub: sentiment ? `${sentiment.totalMentions.toLocaleString()} mentions` : "", icon: Heart, color: "text-pink-400", bg: "from-pink-500/15 to-pink-500/3" },
  ];

  const peakEntries = peak?.hourlyEngagement ? Object.entries(peak.hourlyEngagement).sort(([a], [b]) => Number(a) - Number(b)) : [];
  const maxEngagement = peakEntries.length > 0 ? Math.max(...peakEntries.map(([, v]) => v)) : 1;

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-emerald-500/20 to-emerald-500/5 border border-emerald-500/10 flex items-center justify-center">
          <BarChart3 className="w-5 h-5 text-emerald-400" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">Marketing Analytics</h2>
          <p className="text-xs text-muted-foreground">Campaign performance, leads, and ROI metrics</p>
        </div>
      </motion.div>

      {/* KPI Cards */}
      <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-6 gap-3">
        {kpiCards.map((k, i) => (
          <motion.div key={i} variants={item} className={`p-4 rounded-2xl bg-gradient-to-br ${k.bg} border border-border/15`}>
            <k.icon className={`w-4 h-4 ${k.color} mb-2`} />
            <p className="text-xl font-bold text-foreground">{k.value}</p>
            <p className="text-[10px] text-muted-foreground mt-0.5">{k.label}</p>
            {k.sub && <p className="text-[10px] text-muted-foreground/60">{k.sub}</p>}
          </motion.div>
        ))}
      </div>

      {/* ROAS Breakdown + Leads Split */}
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        {roas && (
          <motion.div variants={item} className="p-5 rounded-2xl bg-card/50 border border-border/30">
            <h3 className="text-sm font-bold text-foreground mb-4 flex items-center gap-2">
              <DollarSign className="w-4 h-4 text-emerald-400" /> Revenue vs Spend
            </h3>
            <div className="space-y-3">
              <div>
                <div className="flex justify-between text-xs mb-1">
                  <span className="text-muted-foreground">Revenue</span>
                  <span className="font-bold text-emerald-400">${roas.totalRevenue.toLocaleString()}</span>
                </div>
                <div className="h-3 bg-muted/20 rounded-full overflow-hidden">
                  <motion.div initial={{ width: 0 }} animate={{ width: "100%" }} transition={{ duration: 0.8 }}
                    className="h-full rounded-full bg-gradient-to-r from-emerald-500 to-emerald-400" />
                </div>
              </div>
              <div>
                <div className="flex justify-between text-xs mb-1">
                  <span className="text-muted-foreground">Spend</span>
                  <span className="font-bold text-red-400">${roas.totalSpend.toLocaleString()}</span>
                </div>
                <div className="h-3 bg-muted/20 rounded-full overflow-hidden">
                  <motion.div initial={{ width: 0 }} animate={{ width: `${(roas.totalSpend / roas.totalRevenue) * 100}%` }}
                    transition={{ duration: 0.8, delay: 0.1 }}
                    className="h-full rounded-full bg-gradient-to-r from-red-500 to-red-400" />
                </div>
              </div>
              <p className="text-center text-lg font-bold text-foreground mt-2">ROAS: {roas.roas}x</p>
            </div>
          </motion.div>
        )}

        {leads && (
          <motion.div variants={item} className="p-5 rounded-2xl bg-card/50 border border-border/30">
            <h3 className="text-sm font-bold text-foreground mb-4 flex items-center gap-2">
              <Users className="w-4 h-4 text-blue-400" /> Lead Sources
            </h3>
            <div className="flex items-center justify-center gap-6">
              <div className="relative w-24 h-24">
                <svg viewBox="0 0 36 36" className="w-full h-full -rotate-90">
                  <circle cx="18" cy="18" r="15" fill="none" stroke="currentColor" strokeWidth="3" className="text-muted/20" />
                  <circle cx="18" cy="18" r="15" fill="none" strokeWidth="3"
                    strokeDasharray={`${(leads.organicLeads / leads.totalLeads) * 94} 94`}
                    className="text-blue-400" strokeLinecap="round" />
                  <circle cx="18" cy="18" r="15" fill="none" strokeWidth="3"
                    strokeDasharray={`${(leads.paidLeads / leads.totalLeads) * 94} 94`}
                    strokeDashoffset={`-${(leads.organicLeads / leads.totalLeads) * 94}`}
                    className="text-emerald-400" strokeLinecap="round" />
                </svg>
                <div className="absolute inset-0 flex items-center justify-center">
                  <span className="text-lg font-bold text-foreground">{leads.totalLeads}</span>
                </div>
              </div>
              <div className="space-y-2">
                <div className="flex items-center gap-2">
                  <div className="w-3 h-3 rounded-full bg-blue-400" />
                  <span className="text-xs text-muted-foreground">Organic: {leads.organicLeads}</span>
                </div>
                <div className="flex items-center gap-2">
                  <div className="w-3 h-3 rounded-full bg-emerald-400" />
                  <span className="text-xs text-muted-foreground">Paid: {leads.paidLeads}</span>
                </div>
              </div>
            </div>
          </motion.div>
        )}
      </div>

      {/* Peak Engagement Hours */}
      {peakEntries.length > 0 && (
        <motion.div variants={item} className="p-5 rounded-2xl bg-card/50 border border-border/30">
          <h3 className="text-sm font-bold text-foreground mb-4 flex items-center gap-2">
            <Clock className="w-4 h-4 text-amber-400" /> Peak Engagement Hours
          </h3>
          <div className="flex items-end gap-1.5 h-28">
            {peakEntries.map(([hour, val], i) => (
              <div key={i} className="flex-1 flex flex-col items-center gap-1">
                <span className="text-[9px] text-muted-foreground font-medium">{val.toFixed(0)}</span>
                <motion.div initial={{ height: 0 }} animate={{ height: `${(val / maxEngagement) * 100}%` }}
                  transition={{ duration: 0.5, delay: i * 0.05 }}
                  className="w-full bg-gradient-to-t from-amber-500/70 to-amber-400/30 rounded-t-lg min-h-[4px]" />
                <span className="text-[9px] text-muted-foreground">{hour}h</span>
              </div>
            ))}
          </div>
        </motion.div>
      )}

      {/* Sentiment Bar */}
      {sentiment && (
        <motion.div variants={item} className="p-4 rounded-2xl bg-card/50 border border-border/30">
          <h3 className="text-sm font-bold text-foreground mb-3 flex items-center gap-2">
            <Heart className="w-4 h-4 text-pink-400" /> Audience Sentiment — {sentiment.totalMentions.toLocaleString()} mentions
          </h3>
          <div className="h-4 rounded-full overflow-hidden flex">
            <motion.div initial={{ width: 0 }} animate={{ width: `${sentiment.positivePercent}%` }}
              transition={{ duration: 0.6 }} className="bg-emerald-500/80 h-full" />
            <motion.div initial={{ width: 0 }} animate={{ width: `${sentiment.neutralPercent}%` }}
              transition={{ duration: 0.6, delay: 0.1 }} className="bg-muted-foreground/30 h-full" />
            <motion.div initial={{ width: 0 }} animate={{ width: `${sentiment.negativePercent}%` }}
              transition={{ duration: 0.6, delay: 0.2 }} className="bg-red-500/80 h-full" />
          </div>
          <div className="flex justify-between mt-2 text-xs text-muted-foreground">
            <span className="text-emerald-400">{sentiment.positivePercent}% Positive</span>
            <span>{sentiment.neutralPercent}% Neutral</span>
            <span className="text-red-400">{sentiment.negativePercent}% Negative</span>
          </div>
        </motion.div>
      )}
    </motion.div>
  );
};

export default MarketerInsightsPanel;

