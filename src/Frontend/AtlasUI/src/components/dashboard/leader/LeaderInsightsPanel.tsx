import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import {
  BarChart3, TrendingUp, Clock, DollarSign,
  GitPullRequest, Smile, AlertTriangle, Trophy,
} from "lucide-react";
import {
  leaderInsightsApi, teamsApi,
  SprintVelocityDto, MeetingsAvoidedDto, BlockedTimeDto,
  CostPerFeatureDto, ReviewTurnaroundDto, TopContributorDto, TeamMoodDto,
} from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.05 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

const LeaderInsightsPanel = () => {
  const [velocity, setVelocity] = useState<SprintVelocityDto | null>(null);
  const [meetings, setMeetings] = useState<MeetingsAvoidedDto | null>(null);
  const [blocked, setBlocked] = useState<BlockedTimeDto | null>(null);
  const [costs, setCosts] = useState<CostPerFeatureDto | null>(null);
  const [review, setReview] = useState<ReviewTurnaroundDto | null>(null);
  const [topContrib, setTopContrib] = useState<TopContributorDto | null>(null);
  const [mood, setMood] = useState<TeamMoodDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = async () => {
      try {
        const tR = await teamsApi.getMyTeams();
        if (tR.data.isSuccess && tR.data.data?.length > 0) {
          const tid = tR.data.data[0].id;
          const results = await Promise.allSettled([
            leaderInsightsApi.sprintVelocity(tid),
            leaderInsightsApi.meetingsAvoided(tid),
            leaderInsightsApi.blockedTime(tid),
            leaderInsightsApi.costPerFeature(tid),
            leaderInsightsApi.reviewTurnaround(tid),
            leaderInsightsApi.topContributor(tid),
            leaderInsightsApi.teamMood(tid),
          ]);
          if (results[0].status === "fulfilled" && results[0].value.data.isSuccess) setVelocity(results[0].value.data.data);
          if (results[1].status === "fulfilled" && results[1].value.data.isSuccess) setMeetings(results[1].value.data.data);
          if (results[2].status === "fulfilled" && results[2].value.data.isSuccess) setBlocked(results[2].value.data.data);
          if (results[3].status === "fulfilled" && results[3].value.data.isSuccess) setCosts(results[3].value.data.data);
          if (results[4].status === "fulfilled" && results[4].value.data.isSuccess) setReview(results[4].value.data.data);
          if (results[5].status === "fulfilled" && results[5].value.data.isSuccess) setTopContrib(results[5].value.data.data);
          if (results[6].status === "fulfilled" && results[6].value.data.isSuccess) setMood(results[6].value.data.data);
        }
      } catch {}
      setLoading(false);
    };
    load();
  }, []);

  if (loading) {
    return (
      <div className="flex items-center justify-center py-20">
        <div className="flex flex-col items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-blue-500/10 flex items-center justify-center animate-pulse">
            <BarChart3 className="w-5 h-5 text-blue-400" />
          </div>
          <p className="text-xs text-muted-foreground">Loading team analytics...</p>
        </div>
      </div>
    );
  }

  const kpiCards = [
    { label: "Velocity", value: velocity ? `${velocity.averagePerSprint} pts` : "—", sub: `${velocity?.dataPoints?.length || 0} sprints`, icon: TrendingUp, color: "text-blue-400", bg: "from-blue-500/15 to-blue-500/3" },
    { label: "Meetings Saved", value: meetings ? `${meetings.meetingsCancelled}` : "—", sub: meetings ? `${meetings.hoursSaved}h saved` : "", icon: Clock, color: "text-emerald-400", bg: "from-emerald-500/15 to-emerald-500/3" },
    { label: "Blocked Time", value: blocked ? `${blocked.totalBlockedHours}h` : "—", sub: `${blocked?.members?.length || 0} members`, icon: AlertTriangle, color: "text-red-400", bg: "from-red-500/15 to-red-500/3" },
    { label: "Avg Review", value: review ? `${review.averageHours}h` : "—", sub: review ? `${review.totalReviews} reviews` : "", icon: GitPullRequest, color: "text-violet-400", bg: "from-violet-500/15 to-violet-500/3" },
    { label: "Avg Feature Cost", value: costs ? `$${costs.averageCost.toLocaleString()}` : "—", sub: `${costs?.features?.length || 0} features`, icon: DollarSign, color: "text-amber-400", bg: "from-amber-500/15 to-amber-500/3" },
    { label: "Team Mood", value: mood?.overallMood || "—", sub: mood ? `${mood.happinessLevel}% happy` : "", icon: Smile, color: "text-pink-400", bg: "from-pink-500/15 to-pink-500/3" },
  ];

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-blue-500/20 to-blue-500/5 border border-blue-500/10 flex items-center justify-center">
          <BarChart3 className="w-5 h-5 text-blue-400" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">Team Analytics</h2>
          <p className="text-xs text-muted-foreground">Sprint velocity, reviews, mood & costs</p>
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

      {/* Sprint Velocity Chart */}
      {velocity && velocity.dataPoints.length > 0 && (
        <motion.div variants={item} className="p-5 rounded-2xl bg-card/50 border border-border/30">
          <h3 className="text-sm font-bold text-foreground mb-4 flex items-center gap-2">
            <TrendingUp className="w-4 h-4 text-blue-400" /> Sprint Velocity
          </h3>
          <div className="flex items-end gap-2 h-28">
            {velocity.dataPoints.map((dp, i) => {
              const maxPts = Math.max(...velocity.dataPoints.map(d => d.points));
              return (
                <div key={i} className="flex-1 flex flex-col items-center gap-1">
                  <span className="text-[9px] text-muted-foreground font-medium">{dp.points}</span>
                  <motion.div initial={{ height: 0 }} animate={{ height: `${(dp.points / maxPts) * 100}%` }}
                    transition={{ duration: 0.5, delay: i * 0.08 }}
                    className="w-full bg-gradient-to-t from-blue-500/70 to-blue-400/30 rounded-t-lg min-h-[4px]" />
                  <span className="text-[9px] text-muted-foreground truncate w-full text-center">{dp.sprintName.replace("Sprint ", "S")}</span>
                </div>
              );
            })}
          </div>
        </motion.div>
      )}

      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        {/* Cost per Feature */}
        {costs && costs.features.length > 0 && (
          <motion.div variants={item} className="p-5 rounded-2xl bg-card/50 border border-border/30">
            <h3 className="text-sm font-bold text-foreground mb-3 flex items-center gap-2">
              <DollarSign className="w-4 h-4 text-amber-400" /> Cost per Feature
            </h3>
            <div className="space-y-2">
              {costs.features.map((f, i) => (
                <div key={i} className="flex items-center gap-3">
                  <span className="text-xs text-foreground truncate flex-1">{f.featureName}</span>
                  <span className="text-[10px] text-muted-foreground">{f.estimatedHours}h</span>
                  <span className="text-xs font-bold text-foreground">${f.cost.toLocaleString()}</span>
                </div>
              ))}
            </div>
          </motion.div>
        )}

        {/* Top Contributor */}
        {topContrib && (
          <motion.div variants={item} className="p-5 rounded-2xl bg-card/50 border border-border/30">
            <h3 className="text-sm font-bold text-foreground mb-3 flex items-center gap-2">
              <Trophy className="w-4 h-4 text-amber-400" /> Top Contributor
            </h3>
            <div className="text-center">
              <div className="w-14 h-14 rounded-2xl bg-gradient-to-br from-amber-500/20 to-amber-400/5 border border-amber-500/20 flex items-center justify-center mx-auto mb-2">
                <span className="text-xl font-bold text-amber-400">{topContrib.memberName.charAt(0)}</span>
              </div>
              <p className="text-sm font-bold text-foreground">{topContrib.memberName}</p>
              <p className="text-lg font-bold text-amber-400 mt-1">{topContrib.totalScore} pts</p>
              <div className="flex justify-center gap-3 mt-2 text-[10px] text-muted-foreground">
                <span>{topContrib.tasksClosed} tasks</span>
                <span>{topContrib.prsMerged} PRs</span>
                <span>{topContrib.bugsFixed} bugs</span>
              </div>
            </div>
          </motion.div>
        )}
      </div>

      {/* Blocked Members */}
      {blocked && blocked.members.length > 0 && (
        <motion.div variants={item} className="p-5 rounded-2xl bg-card/50 border border-border/30">
          <h3 className="text-sm font-bold text-foreground mb-3 flex items-center gap-2">
            <AlertTriangle className="w-4 h-4 text-red-400" /> Blocked Members
          </h3>
          <div className="space-y-2">
            {blocked.members.map((m, i) => (
              <div key={i} className="flex items-center gap-3 p-2 rounded-xl bg-red-500/5 border border-red-500/10">
                <div className="w-8 h-8 rounded-lg bg-red-500/10 flex items-center justify-center">
                  <span className="text-xs font-bold text-red-400">{m.memberName.charAt(0)}</span>
                </div>
                <div className="flex-1">
                  <span className="text-xs font-bold text-foreground">{m.memberName}</span>
                  <p className="text-[10px] text-muted-foreground">{m.topBlocker}</p>
                </div>
                <span className="text-xs font-bold text-red-400">{m.blockedHours}h</span>
              </div>
            ))}
          </div>
        </motion.div>
      )}

      {/* Team Mood Factors */}
      {mood && mood.factors.length > 0 && (
        <motion.div variants={item} className="p-5 rounded-2xl bg-card/50 border border-border/30">
          <h3 className="text-sm font-bold text-foreground mb-3 flex items-center gap-2">
            <Smile className="w-4 h-4 text-pink-400" /> Mood Factors
          </h3>
          <div className="space-y-2">
            {mood.factors.map((f, i) => (
              <div key={i} className="flex items-center gap-3 text-xs">
                <span className={`w-2 h-2 rounded-full ${f.direction === "Positive" ? "bg-emerald-400" : "bg-red-400"}`} />
                <span className="text-foreground flex-1">{f.factor}</span>
                <span className={f.direction === "Positive" ? "text-emerald-400" : "text-red-400"}>{f.impact}% {f.direction}</span>
              </div>
            ))}
          </div>
        </motion.div>
      )}
    </motion.div>
  );
};

export default LeaderInsightsPanel;

