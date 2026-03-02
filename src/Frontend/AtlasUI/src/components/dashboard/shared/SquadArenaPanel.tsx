import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import {
  Trophy, Medal, Target, Plus, Loader2, Star,
  Award, Flame, CheckCircle2, Hand,
} from "lucide-react";
import {
  squadArenaApi, teamsApi,
  LeaderboardEntryDto, BountyDto, TeamDto,
} from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

const SquadArenaPanel = () => {
  const [teams, setTeams] = useState<TeamDto[]>([]);
  const [teamId, setTeamId] = useState<string | null>(null);
  const [leaderboard, setLeaderboard] = useState<LeaderboardEntryDto[]>([]);
  const [bounties, setBounties] = useState<BountyDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [tab, setTab] = useState<"leaderboard" | "bounties" | "badge">("leaderboard");
  // Badge form
  const [badgeRecipient, setBadgeRecipient] = useState("");
  const [badgeType, setBadgeType] = useState("CodeNinja");
  const [badgeMessage, setBadgeMessage] = useState("");
  // Bounty form
  const [bountyTitle, setBountyTitle] = useState("");
  const [bountyDesc, setBountyDesc] = useState("");
  const [bountyXP, setBountyXP] = useState(50);
  const [actionLoading, setActionLoading] = useState<string | null>(null);

  useEffect(() => {
    teamsApi.getMyTeams().then(r => {
      if (r.data.isSuccess && r.data.data?.length > 0) {
        setTeams(r.data.data);
        setTeamId(r.data.data[0].id);
      }
    }).catch(() => {});
  }, []);

  useEffect(() => {
    if (!teamId) { setLoading(false); return; }
    setLoading(true);
    Promise.all([
      squadArenaApi.getLeaderboard(teamId),
      squadArenaApi.getBounties(teamId),
    ]).then(([lb, bn]) => {
      if (lb.data.isSuccess) setLeaderboard(lb.data.data || []);
      if (bn.data.isSuccess) setBounties(bn.data.data || []);
    }).catch(() => {}).finally(() => setLoading(false));
  }, [teamId]);

  const handleGiveBadge = async () => {
    if (!teamId || !badgeRecipient.trim()) return;
    setActionLoading("badge");
    try {
      await squadArenaApi.giveBadge({ teamId, recipientUserId: badgeRecipient, badgeType, message: badgeMessage });
      setBadgeRecipient(""); setBadgeMessage("");
    } catch {}
    setActionLoading(null);
  };

  const handleCreateBounty = async () => {
    if (!teamId || !bountyTitle.trim()) return;
    setActionLoading("bounty");
    try {
      await squadArenaApi.createBounty({ teamId, title: bountyTitle, description: bountyDesc, xpReward: bountyXP });
      setBountyTitle(""); setBountyDesc(""); setBountyXP(50);
      const r = await squadArenaApi.getBounties(teamId);
      if (r.data.isSuccess) setBounties(r.data.data || []);
    } catch {}
    setActionLoading(null);
  };

  const handleClaim = async (id: string) => {
    setActionLoading(id);
    try {
      await squadArenaApi.claimBounty(id);
      if (teamId) { const r = await squadArenaApi.getBounties(teamId); if (r.data.isSuccess) setBounties(r.data.data || []); }
    } catch {}
    setActionLoading(null);
  };

  const handleComplete = async (id: string) => {
    setActionLoading(id);
    try {
      await squadArenaApi.completeBounty(id);
      if (teamId) { const r = await squadArenaApi.getBounties(teamId); if (r.data.isSuccess) setBounties(r.data.data || []); }
    } catch {}
    setActionLoading(null);
  };

  const medalColors = ["text-amber-400", "text-zinc-300", "text-orange-400"];

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-amber-500/20 to-amber-500/5 border border-amber-500/10 flex items-center justify-center">
            <Trophy className="w-5 h-5 text-amber-400" />
          </div>
          <div>
            <h2 className="text-lg font-bold text-foreground tracking-tight">Squad Arena</h2>
            <p className="text-xs text-muted-foreground">Leaderboard, bounties & badges</p>
          </div>
        </div>
        {teams.length > 1 && (
          <select value={teamId || ""} onChange={e => setTeamId(e.target.value)}
            className="h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground">
            {teams.map(t => <option key={t.id} value={t.id}>{t.name}</option>)}
          </select>
        )}
      </motion.div>

      <motion.div variants={item} className="flex gap-1.5">
        {(["leaderboard", "bounties", "badge"] as const).map(t => (
          <button key={t} onClick={() => setTab(t)}
            className={`px-4 py-1.5 rounded-lg text-xs font-semibold capitalize transition-all ${tab === t ? "bg-primary/15 text-primary" : "bg-muted/10 text-muted-foreground hover:text-foreground"}`}>
            {t === "leaderboard" && <Medal className="w-3.5 h-3.5 inline mr-1" />}
            {t === "bounties" && <Target className="w-3.5 h-3.5 inline mr-1" />}
            {t === "badge" && <Award className="w-3.5 h-3.5 inline mr-1" />}
            {t}
          </button>
        ))}
      </motion.div>

      {!teamId ? (
        <p className="text-xs text-muted-foreground text-center py-8">No team found.</p>
      ) : loading ? (
        <div className="flex justify-center py-8"><Loader2 className="w-5 h-5 animate-spin text-primary" /></div>
      ) : (
        <>
          {tab === "leaderboard" && (
            <div className="space-y-1.5">
              {leaderboard.map((e, i) => (
                <motion.div key={i} variants={item}
                  className={`p-3 rounded-xl border border-border/15 flex items-center gap-3 ${i < 3 ? "bg-amber-500/5" : "bg-card/50"}`}>
                  <span className={`text-lg font-black ${medalColors[i] || "text-muted-foreground"}`}>#{i + 1}</span>
                  <div className="flex-1 min-w-0">
                    <p className="text-xs font-semibold text-foreground">{e.fullName}</p>
                  </div>
                  <div className="flex items-center gap-1">
                    <Flame className="w-3 h-3 text-amber-400" />
                    <span className="text-xs font-bold text-foreground">{e.xp} XP</span>
                  </div>
                </motion.div>
              ))}
              {leaderboard.length === 0 && <p className="text-xs text-muted-foreground text-center py-6">No leaderboard data yet.</p>}
            </div>
          )}

          {tab === "bounties" && (
            <div className="space-y-3">
              <div className="p-3 rounded-xl bg-card/30 border border-dashed border-border/30 space-y-2">
                <input value={bountyTitle} onChange={e => setBountyTitle(e.target.value)} placeholder="Bounty title..."
                  className="w-full h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground placeholder:text-muted-foreground/50" />
                <div className="flex gap-2">
                  <input value={bountyDesc} onChange={e => setBountyDesc(e.target.value)} placeholder="Description..."
                    className="flex-1 h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground placeholder:text-muted-foreground/50" />
                  <input type="number" value={bountyXP} onChange={e => setBountyXP(Number(e.target.value))} min={1}
                    className="w-20 h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground text-center" />
                  <span className="text-[10px] text-muted-foreground self-center">XP</span>
                </div>
                <button onClick={handleCreateBounty} disabled={actionLoading === "bounty" || !bountyTitle.trim()}
                  className="w-full h-8 rounded-lg bg-primary text-primary-foreground text-xs font-semibold flex items-center justify-center gap-1.5 hover:bg-primary/90 disabled:opacity-50 transition-colors">
                  {actionLoading === "bounty" ? <Loader2 className="w-3 h-3 animate-spin" /> : <Plus className="w-3 h-3" />} Create Bounty
                </button>
              </div>

              {bounties.map((b, i) => (
                <motion.div key={i} variants={item} className="p-3 rounded-xl bg-card/50 border border-border/15">
                  <div className="flex items-start justify-between mb-1">
                    <div>
                      <p className="text-xs font-semibold text-foreground">{b.title}</p>
                      {b.description && <p className="text-[10px] text-muted-foreground">{b.description}</p>}
                    </div>
                    <span className="flex items-center gap-1 text-xs font-bold text-amber-400"><Star className="w-3 h-3" />{b.xpReward} XP</span>
                  </div>
                  <div className="flex gap-1.5 mt-2">
                    <button onClick={() => handleClaim(b.id)} disabled={actionLoading === b.id}
                      className="h-7 px-3 rounded-lg bg-blue-500/10 text-blue-400 text-[10px] font-semibold flex items-center gap-1 hover:bg-blue-500/20 disabled:opacity-50">
                      <Hand className="w-3 h-3" /> Claim
                    </button>
                    <button onClick={() => handleComplete(b.id)} disabled={actionLoading === b.id}
                      className="h-7 px-3 rounded-lg bg-emerald-500/10 text-emerald-400 text-[10px] font-semibold flex items-center gap-1 hover:bg-emerald-500/20 disabled:opacity-50">
                      <CheckCircle2 className="w-3 h-3" /> Complete
                    </button>
                  </div>
                </motion.div>
              ))}
              {bounties.length === 0 && <p className="text-xs text-muted-foreground text-center py-4">No bounties yet.</p>}
            </div>
          )}

          {tab === "badge" && (
            <div className="p-4 rounded-xl bg-card/50 border border-border/20 space-y-3">
              <p className="text-xs font-bold text-foreground">Give a Badge</p>
              <input value={badgeRecipient} onChange={e => setBadgeRecipient(e.target.value)} placeholder="Recipient User ID..."
                className="w-full h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground placeholder:text-muted-foreground/50" />
              <select value={badgeType} onChange={e => setBadgeType(e.target.value)}
                className="w-full h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground">
                {["CodeNinja", "BugHunter", "TeamPlayer", "Speedster", "Mentor"].map(b => <option key={b} value={b}>{b}</option>)}
              </select>
              <input value={badgeMessage} onChange={e => setBadgeMessage(e.target.value)} placeholder="Message (optional)..."
                className="w-full h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground placeholder:text-muted-foreground/50" />
              <button onClick={handleGiveBadge} disabled={actionLoading === "badge" || !badgeRecipient.trim()}
                className="w-full h-8 rounded-lg bg-primary text-primary-foreground text-xs font-semibold flex items-center justify-center gap-1.5 hover:bg-primary/90 disabled:opacity-50">
                {actionLoading === "badge" ? <Loader2 className="w-3 h-3 animate-spin" /> : <Award className="w-3 h-3" />} Give Badge
              </button>
            </div>
          )}
        </>
      )}
    </motion.div>
  );
};

export default SquadArenaPanel;

