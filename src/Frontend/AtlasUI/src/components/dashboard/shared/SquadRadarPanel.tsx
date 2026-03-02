import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import {
  Radar, Loader2, Wifi, WifiOff, Coffee,
  Code2, MessageCircle,
} from "lucide-react";
import { squadRadarApi, teamsApi, SquadRadarDto, TeamDto } from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

const statusConfig: Record<string, { icon: typeof Wifi; color: string; bg: string }> = {
  Online: { icon: Wifi, color: "text-emerald-400", bg: "bg-emerald-500/10" },
  Focused: { icon: Code2, color: "text-blue-400", bg: "bg-blue-500/10" },
  InMeeting: { icon: MessageCircle, color: "text-amber-400", bg: "bg-amber-500/10" },
  OnBreak: { icon: Coffee, color: "text-violet-400", bg: "bg-violet-500/10" },
  Offline: { icon: WifiOff, color: "text-muted-foreground", bg: "bg-muted/10" },
};

const SquadRadarPanel = () => {
  const [teams, setTeams] = useState<TeamDto[]>([]);
  const [teamId, setTeamId] = useState<string | null>(null);
  const [members, setMembers] = useState<SquadRadarDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [myStatus, setMyStatus] = useState("Online");
  const [myTask, setMyTask] = useState("");
  const [updating, setUpdating] = useState(false);

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
    squadRadarApi.getRadar(teamId).then(r => {
      if (r.data.isSuccess) setMembers(r.data.data || []);
    }).catch(() => {}).finally(() => setLoading(false));
  }, [teamId]);

  const updatePresence = async () => {
    if (!teamId) return;
    setUpdating(true);
    try {
      await squadRadarApi.updatePresence({ status: myStatus, currentTask: myTask || undefined });
      // Refresh
      const r = await squadRadarApi.getRadar(teamId);
      if (r.data.isSuccess) setMembers(r.data.data || []);
    } catch {}
    setUpdating(false);
  };

  const statuses = ["Online", "Focused", "InMeeting", "OnBreak", "Offline"];

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-cyan-500/20 to-cyan-500/5 border border-cyan-500/10 flex items-center justify-center">
            <Radar className="w-5 h-5 text-cyan-400" />
          </div>
          <div>
            <h2 className="text-lg font-bold text-foreground tracking-tight">Squad Radar</h2>
            <p className="text-xs text-muted-foreground">See who's doing what</p>
          </div>
        </div>
        {teams.length > 1 && (
          <select value={teamId || ""} onChange={e => setTeamId(e.target.value)}
            className="h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground">
            {teams.map(t => <option key={t.id} value={t.id}>{t.name}</option>)}
          </select>
        )}
      </motion.div>

      {/* My Presence */}
      <motion.div variants={item} className="p-4 rounded-xl bg-card/50 border border-border/20">
        <p className="text-[10px] font-bold text-muted-foreground/50 uppercase tracking-wider mb-3">Your Presence</p>
        <div className="flex gap-1.5 mb-3 flex-wrap">
          {statuses.map(s => {
            const cfg = statusConfig[s] || statusConfig.Online;
            return (
              <button key={s} onClick={() => setMyStatus(s)}
                className={`px-3 py-1.5 rounded-lg text-[10px] font-semibold flex items-center gap-1.5 transition-all ${myStatus === s ? `${cfg.bg} ${cfg.color}` : "bg-muted/10 text-muted-foreground hover:text-foreground"}`}>
                <cfg.icon className="w-3 h-3" /> {s}
              </button>
            );
          })}
        </div>
        <div className="flex gap-2">
          <input value={myTask} onChange={e => setMyTask(e.target.value)} placeholder="What are you working on?"
            className="flex-1 h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground placeholder:text-muted-foreground/50" />
          <button onClick={updatePresence} disabled={updating}
            className="h-8 px-4 rounded-lg bg-primary text-primary-foreground text-xs font-semibold hover:bg-primary/90 disabled:opacity-50 transition-colors">
            {updating ? <Loader2 className="w-3 h-3 animate-spin" /> : "Update"}
          </button>
        </div>
      </motion.div>

      {/* Team Members */}
      {!teamId ? (
        <p className="text-xs text-muted-foreground text-center py-8">No team found.</p>
      ) : loading ? (
        <div className="flex justify-center py-8"><Loader2 className="w-5 h-5 animate-spin text-primary" /></div>
      ) : members.length === 0 ? (
        <p className="text-xs text-muted-foreground text-center py-8">No team members online.</p>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-2">
          {members.map((m, i) => {
            const cfg = statusConfig[m.status] || statusConfig.Online;
            return (
              <motion.div key={i} variants={item}
                className={`p-3 rounded-xl ${cfg.bg} border border-border/15 flex items-center gap-3`}>
                <div className="w-8 h-8 rounded-full bg-foreground/5 flex items-center justify-center">
                  <cfg.icon className={`w-4 h-4 ${cfg.color}`} />
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-xs font-semibold text-foreground">{m.fullName}</p>
                  <p className="text-[10px] text-muted-foreground truncate">{m.currentTask || m.status}</p>
                </div>
                <div className={`w-2 h-2 rounded-full ${m.status === "Online" || m.status === "Focused" ? "bg-emerald-400" : "bg-muted-foreground/30"}`} />
              </motion.div>
            );
          })}
        </div>
      )}
    </motion.div>
  );
};

export default SquadRadarPanel;

