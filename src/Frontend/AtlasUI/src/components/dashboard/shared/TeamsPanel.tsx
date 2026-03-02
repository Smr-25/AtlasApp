import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import {
  Users, Plus, Loader2, UserPlus, Crown,
  Target, Link2, Shield, Briefcase,
} from "lucide-react";
import { teamsApi, teamInfoApi, TeamDto, TeamInfoDto } from "@/services/api";
import { useAuth } from "@/context/AuthContext";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

const TeamsPanel = () => {
  const { user } = useAuth();
  const isTeamLeader = user?.role === "team-leader";
  const [teams, setTeams] = useState<TeamDto[]>([]);
  const [selectedTeam, setSelectedTeam] = useState<TeamDto | null>(null);
  const [teamInfo, setTeamInfo] = useState<TeamInfoDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [creating, setCreating] = useState(false);
  const [newName, setNewName] = useState("");
  const [newDesc, setNewDesc] = useState("");
  const [inviteEmail, setInviteEmail] = useState("");
  const [inviting, setInviting] = useState(false);
  const [activeSection, setActiveSection] = useState<"overview" | "members" | "vault">("overview");

  const fetchTeams = async () => {
    setLoading(true);
    try {
      const r = await teamsApi.getMyTeams();
      if (r.data.isSuccess && r.data.data) {
        setTeams(r.data.data);
        if (r.data.data.length > 0 && !selectedTeam) setSelectedTeam(r.data.data[0]);
      }
    } catch {}
    setLoading(false);
  };

  useEffect(() => { fetchTeams(); }, []);

  useEffect(() => {
    if (!selectedTeam) return;
    teamInfoApi.getInfo(selectedTeam.id).then(r => {
      if (r.data.isSuccess) setTeamInfo(r.data.data);
    }).catch(() => {});
  }, [selectedTeam]);

  const handleCreate = async () => {
    if (!newName.trim()) return;
    setCreating(true);
    try {
      await teamsApi.create({ name: newName, description: newDesc });
      setNewName(""); setNewDesc("");
      await fetchTeams();
    } catch {}
    setCreating(false);
  };

  const handleInvite = async () => {
    if (!selectedTeam || !inviteEmail.trim()) return;
    setInviting(true);
    try {
      await teamsApi.addMember(selectedTeam.id, { userId: inviteEmail });
      setInviteEmail("");
      // Refresh team info
      const r = await teamInfoApi.getInfo(selectedTeam.id);
      if (r.data.isSuccess) setTeamInfo(r.data.data);
    } catch {}
    setInviting(false);
  };

  const sections = [
    { id: "overview" as const, label: "Overview", icon: Target },
    { id: "members" as const, label: "Members", icon: Users },
    { id: "vault" as const, label: "Vault", icon: Link2 },
  ];

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      {/* Header */}
      <motion.div variants={item} className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-primary/20 to-primary/5 border border-primary/10 flex items-center justify-center">
            <Users className="w-5 h-5 text-primary" />
          </div>
          <div>
            <h2 className="text-lg font-bold text-foreground tracking-tight">Teams</h2>
            <p className="text-xs text-muted-foreground">{teams.length} team{teams.length !== 1 ? "s" : ""}</p>
          </div>
        </div>
      </motion.div>

      {loading ? (
        <div className="flex items-center justify-center py-12"><Loader2 className="w-5 h-5 animate-spin text-primary" /></div>
      ) : (
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">
          {/* Left: Team List + Create */}
          <motion.div variants={item} className="space-y-3">
            {teams.map(t => (
              <button key={t.id} onClick={() => setSelectedTeam(t)}
                className={`w-full p-3 rounded-xl border text-left transition-all ${selectedTeam?.id === t.id ? "bg-primary/10 border-primary/30" : "bg-card/50 border-border/30 hover:border-primary/15"}`}>
                <div className="flex items-center gap-2">
                  <Crown className="w-4 h-4 text-primary" />
                  <span className="text-sm font-semibold text-foreground">{t.name}</span>
                </div>
                {t.description && <p className="text-[10px] text-muted-foreground mt-1 truncate">{t.description}</p>}
              </button>
            ))}
            {/* Create Form — only Team Leader can create */}
            {isTeamLeader && (
            <div className="p-3 rounded-xl bg-card/30 border border-dashed border-border/30 space-y-2">
              <input value={newName} onChange={e => setNewName(e.target.value)} placeholder="Team name..."
                className="w-full h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground placeholder:text-muted-foreground/50" />
              <input value={newDesc} onChange={e => setNewDesc(e.target.value)} placeholder="Description (optional)"
                className="w-full h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground placeholder:text-muted-foreground/50" />
              <button onClick={handleCreate} disabled={creating || !newName.trim()}
                className="w-full h-8 rounded-lg bg-primary text-primary-foreground text-xs font-semibold flex items-center justify-center gap-1.5 hover:bg-primary/90 disabled:opacity-50 transition-colors">
                {creating ? <Loader2 className="w-3 h-3 animate-spin" /> : <Plus className="w-3 h-3" />} Create Team
              </button>
            </div>
            )}
          </motion.div>

          {/* Right: Team Detail */}
          {selectedTeam ? (
            <motion.div variants={item} className="lg:col-span-2 space-y-4">
              <div className="flex items-center gap-2 border-b border-border/20 pb-3">
                {sections.map(s => (
                  <button key={s.id} onClick={() => setActiveSection(s.id)}
                    className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-semibold transition-all ${activeSection === s.id ? "bg-primary/10 text-primary" : "text-muted-foreground hover:text-foreground"}`}>
                    <s.icon className="w-3.5 h-3.5" /> {s.label}
                  </button>
                ))}
              </div>

              {activeSection === "overview" && teamInfo && (
                <div className="space-y-3">
                  {teamInfo.objective && (
                    <div className="p-4 rounded-xl bg-primary/5 border border-primary/10">
                      <div className="flex items-center gap-2 mb-2">
                        <Target className="w-4 h-4 text-primary" />
                        <span className="text-xs font-bold text-foreground">Team Objective</span>
                      </div>
                      <p className="text-sm text-foreground">{typeof teamInfo.objective === "string" ? teamInfo.objective : JSON.stringify(teamInfo.objective)}</p>
                    </div>
                  )}
                  {teamInfo.armory && typeof teamInfo.armory === "object" && Object.keys(teamInfo.armory).length > 0 && (
                    <div className="p-4 rounded-xl bg-card/50 border border-border/20">
                      <div className="flex items-center gap-2 mb-2">
                        <Shield className="w-4 h-4 text-amber-400" />
                        <span className="text-xs font-bold text-foreground">Armory</span>
                      </div>
                      <div className="grid grid-cols-2 gap-2">
                        {Object.entries(teamInfo.armory as Record<string, string>).map(([k, v]) => (
                          <div key={k} className="text-[10px]">
                            <span className="text-muted-foreground">{k}:</span>{" "}
                            <span className="text-foreground font-medium">{v}</span>
                          </div>
                        ))}
                      </div>
                    </div>
                  )}
                </div>
              )}

              {activeSection === "members" && (
                <div className="space-y-3">
                  <div className="flex gap-2">
                    <input value={inviteEmail} onChange={e => setInviteEmail(e.target.value)} placeholder="User ID to invite..."
                      className="flex-1 h-8 px-3 rounded-lg bg-muted/30 border border-border/30 text-xs text-foreground placeholder:text-muted-foreground/50" />
                    <button onClick={handleInvite} disabled={inviting || !inviteEmail.trim()}
                      className="h-8 px-3 rounded-lg bg-primary text-primary-foreground text-xs font-semibold flex items-center gap-1.5 hover:bg-primary/90 disabled:opacity-50 transition-colors">
                      {inviting ? <Loader2 className="w-3 h-3 animate-spin" /> : <UserPlus className="w-3 h-3" />} Invite
                    </button>
                  </div>
                  {teamInfo?.members?.map((m: any, i: number) => (
                    <div key={i} className="p-3 rounded-xl bg-card/50 border border-border/20 flex items-center gap-3">
                      <div className="w-8 h-8 rounded-full bg-primary/10 flex items-center justify-center text-xs font-bold text-primary">
                        {(m.fullName || m.userName || "U").split(" ").map((n: string) => n[0]).join("").slice(0, 2)}
                      </div>
                      <div className="flex-1 min-w-0">
                        <p className="text-xs font-semibold text-foreground">{m.fullName || m.userName || "Member"}</p>
                        {m.focus && <p className="text-[10px] text-muted-foreground truncate">{m.focus}</p>}
                      </div>
                    </div>
                  ))}
                  {(!teamInfo?.members || teamInfo.members.length === 0) && (
                    <p className="text-xs text-muted-foreground text-center py-4">No members yet. Invite someone!</p>
                  )}
                </div>
              )}

              {activeSection === "vault" && (
                <div className="space-y-2">
                  {teamInfo?.vaultLinks?.map((link: any, i: number) => (
                    <a key={link.id || i} href={link.url} target="_blank" rel="noopener"
                      className="p-3 rounded-xl bg-card/50 border border-border/20 flex items-center gap-3 hover:border-primary/20 transition-colors">
                      <span className="text-lg">{link.icon || "🔗"}</span>
                      <span className="text-xs font-semibold text-foreground">{link.label}</span>
                      <Link2 className="w-3 h-3 text-muted-foreground ml-auto" />
                    </a>
                  ))}
                  {(!teamInfo?.vaultLinks || teamInfo.vaultLinks.length === 0) && (
                    <p className="text-xs text-muted-foreground text-center py-4">No vault links yet.</p>
                  )}
                </div>
              )}
            </motion.div>
          ) : (
            <div className="lg:col-span-2 flex items-center justify-center text-xs text-muted-foreground py-12">
              <Briefcase className="w-4 h-4 mr-2" /> Select or create a team to get started
            </div>
          )}
        </div>
      )}
    </motion.div>
  );
};

export default TeamsPanel;

