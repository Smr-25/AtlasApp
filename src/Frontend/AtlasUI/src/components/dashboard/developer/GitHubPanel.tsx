import { useState, useEffect } from "react";
import { motion, AnimatePresence } from "framer-motion";
import {
  GitBranch, GitPullRequest, GitCommit, Star,
  Check, X as XIcon, GitMerge, Loader2, AlertCircle,
  ArrowUpRight,
} from "lucide-react";
import { gitApi, GitDashboardDto, IntegrationDto } from "@/services/api";

interface GitHubPanelProps {
  integrations: IntegrationDto[];
}

const stateConfig: Record<string, { color: string; bg: string; label: string }> = {
  open: { color: "text-emerald-400", bg: "bg-emerald-500/10 border-emerald-500/20", label: "Open" },
  merged: { color: "text-violet-400", bg: "bg-violet-500/10 border-violet-500/20", label: "Merged" },
  closed: { color: "text-red-400", bg: "bg-red-500/10 border-red-500/20", label: "Closed" },
};

const langColors: Record<string, string> = {
  TypeScript: "bg-blue-500", JavaScript: "bg-yellow-400", Python: "bg-emerald-500",
  "C#": "bg-violet-500", Go: "bg-cyan-400", Rust: "bg-orange-500",
  Java: "bg-red-500", Ruby: "bg-red-400", Swift: "bg-orange-400",
  Kotlin: "bg-purple-500", Dart: "bg-teal-400",
};

const GitHubPanel = ({ integrations }: GitHubPanelProps) => {
  const [dashboard, setDashboard] = useState<GitDashboardDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [actionLoading, setActionLoading] = useState<string | null>(null);
  const [error, setError] = useState("");
  const [activeSection, setActiveSection] = useState<"repos" | "prs" | "commits">("prs");

  const githubInt = integrations.find((i) => i.provider === "GitHub" && i.status === "Active");

  useEffect(() => {
    if (!githubInt) { setLoading(false); return; }
    const load = async () => {
      setLoading(true);
      try {
        const res = await gitApi.dashboard(githubInt.id);
        if (res.data.isSuccess) setDashboard(res.data.data);
        else setError(res.data.errors?.[0] || "Failed to load");
      } catch (err: any) {
        setError(err?.response?.data?.errors?.[0] || "Failed to load GitHub data");
      }
      setLoading(false);
    };
    load();
  }, [githubInt?.id]);

  const handlePrAction = async (action: "approve" | "reject" | "merge", pr: any) => {
    if (!githubInt) return;
    setActionLoading(`${action}-${pr.id}`);
    try {
      const body = { integrationId: githubInt.id, owner: pr.repo?.split("/")[0] || "", repo: pr.repo?.split("/")[1] || pr.repo, prNumber: pr.id };
      if (action === "approve") await gitApi.approve(body);
      else if (action === "reject") await gitApi.reject({ ...body, reason: "Needs changes" });
      else await gitApi.merge(body);
    } catch { /* toast error */ }
    setActionLoading(null);
  };

  // No integration
  if (!githubInt) {
    return (
      <div className="space-y-6">
        <Header />
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="py-20 text-center"
        >
          <div className="w-16 h-16 rounded-2xl bg-muted/40 border border-border flex items-center justify-center mx-auto mb-4">
            <GitBranch className="w-8 h-8 text-muted-foreground/30" />
          </div>
          <p className="text-sm font-semibold text-foreground mb-1">Connect GitHub</p>
          <p className="text-xs text-muted-foreground max-w-xs mx-auto">
            Go to the Integrations tab and connect your GitHub account to see repos, PRs and commits.
          </p>
        </motion.div>
      </div>
    );
  }

  if (loading) {
    return (
      <div className="space-y-6">
        <Header />
        <div className="flex items-center justify-center py-20">
          <div className="flex flex-col items-center gap-3">
            <div className="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center animate-pulse">
              <GitBranch className="w-5 h-5 text-primary" />
            </div>
            <p className="text-xs text-muted-foreground">Loading GitHub data...</p>
          </div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="space-y-6">
        <Header />
        <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="p-4 rounded-xl bg-destructive/5 border border-destructive/15 flex items-center gap-3">
          <AlertCircle className="w-5 h-5 text-destructive shrink-0" />
          <div>
            <p className="text-sm font-medium text-foreground">Failed to load</p>
            <p className="text-xs text-muted-foreground">{error}</p>
          </div>
        </motion.div>
      </div>
    );
  }

  const sections = [
    { id: "prs" as const, label: "Pull Requests", count: dashboard?.pullRequests?.length || 0, icon: GitPullRequest },
    { id: "repos" as const, label: "Repositories", count: dashboard?.repos?.length || 0, icon: GitBranch },
    { id: "commits" as const, label: "Commits", count: dashboard?.recentCommits?.length || 0, icon: GitCommit },
  ];

  return (
    <div className="space-y-6">
      <Header />

      {/* Stats Bar */}
      <div className="grid grid-cols-3 gap-3">
        {[
          { label: "Repositories", value: dashboard?.repos?.length || 0, icon: GitBranch, gradient: "from-blue-500/10 to-blue-600/5" },
          { label: "Open PRs", value: dashboard?.pullRequests?.filter((p) => p.state === "open").length || 0, icon: GitPullRequest, gradient: "from-emerald-500/10 to-emerald-600/5" },
          { label: "Recent Commits", value: dashboard?.recentCommits?.length || 0, icon: GitCommit, gradient: "from-violet-500/10 to-violet-600/5" },
        ].map((stat, i) => (
          <motion.div
            key={stat.label}
            initial={{ opacity: 0, y: 12 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: i * 0.06 }}
            className={`relative overflow-hidden rounded-xl border border-border p-4 bg-gradient-to-br ${stat.gradient}`}
          >
            <div className="absolute top-0 right-0 w-20 h-20 bg-primary/[0.03] rounded-full -translate-y-1/2 translate-x-1/3" />
            <stat.icon className="w-4 h-4 text-muted-foreground mb-2" />
            <p className="text-2xl font-bold text-foreground">{stat.value}</p>
            <p className="text-[10px] text-muted-foreground mt-0.5">{stat.label}</p>
          </motion.div>
        ))}
      </div>

      {/* Section Tabs */}
      <div className="flex items-center gap-1 p-1 rounded-xl bg-muted/30 border border-border">
        {sections.map((sec) => (
          <button
            key={sec.id}
            onClick={() => setActiveSection(sec.id)}
            className={`flex-1 flex items-center justify-center gap-2 py-2 px-3 rounded-lg text-xs font-medium transition-all ${
              activeSection === sec.id
                ? "bg-card text-foreground shadow-sm border border-border/50"
                : "text-muted-foreground hover:text-foreground"
            }`}
          >
            <sec.icon className="w-3.5 h-3.5" />
            {sec.label}
            {sec.count > 0 && (
              <span className={`text-[9px] px-1.5 py-0.5 rounded-full ${
                activeSection === sec.id ? "bg-primary/10 text-primary" : "bg-muted text-muted-foreground"
              }`}>{sec.count}</span>
            )}
          </button>
        ))}
      </div>

      {/* Content */}
      <AnimatePresence mode="wait">
        {activeSection === "prs" && (
          <motion.div key="prs" initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -8 }} className="space-y-2">
            {dashboard?.pullRequests && dashboard.pullRequests.length > 0 ? dashboard.pullRequests.map((pr, i) => {
              const cfg = stateConfig[pr.state] || stateConfig.open;
              return (
                <motion.div
                  key={pr.id}
                  initial={{ opacity: 0, y: 8 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: i * 0.04 }}
                  className="group relative overflow-hidden rounded-xl border border-border bg-card/60 hover:bg-card hover:border-primary/10 transition-all duration-200"
                >
                  <div className="flex items-start gap-3.5 p-4">
                    <div className={`w-8 h-8 rounded-lg flex items-center justify-center shrink-0 ${cfg.bg} border`}>
                      <GitPullRequest className={`w-4 h-4 ${cfg.color}`} />
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="text-sm font-medium text-foreground leading-snug">{pr.title}</p>
                      <div className="flex items-center gap-2 mt-1.5">
                        <span className="text-[10px] text-muted-foreground font-mono">#{pr.id}</span>
                        <span className="text-[10px] text-muted-foreground">•</span>
                        <span className="text-[10px] text-muted-foreground">{pr.author}</span>
                        <span className="text-[10px] text-muted-foreground">•</span>
                        <span className="text-[10px] text-muted-foreground">{pr.repo}</span>
                      </div>
                    </div>
                    {pr.state === "open" && (
                      <div className="flex items-center gap-1 shrink-0 opacity-0 group-hover:opacity-100 transition-all duration-200">
                        <motion.button whileTap={{ scale: 0.9 }} onClick={() => handlePrAction("approve", pr)} disabled={!!actionLoading}
                          className="w-8 h-8 rounded-lg flex items-center justify-center text-emerald-400 bg-emerald-500/10 hover:bg-emerald-500/20 border border-emerald-500/10 transition-all" title="Approve">
                          {actionLoading === `approve-${pr.id}` ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : <Check className="w-3.5 h-3.5" />}
                        </motion.button>
                        <motion.button whileTap={{ scale: 0.9 }} onClick={() => handlePrAction("reject", pr)} disabled={!!actionLoading}
                          className="w-8 h-8 rounded-lg flex items-center justify-center text-red-400 bg-red-500/10 hover:bg-red-500/20 border border-red-500/10 transition-all" title="Request Changes">
                          <XIcon className="w-3.5 h-3.5" />
                        </motion.button>
                        <motion.button whileTap={{ scale: 0.9 }} onClick={() => handlePrAction("merge", pr)} disabled={!!actionLoading}
                          className="w-8 h-8 rounded-lg flex items-center justify-center text-violet-400 bg-violet-500/10 hover:bg-violet-500/20 border border-violet-500/10 transition-all" title="Merge">
                          {actionLoading === `merge-${pr.id}` ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : <GitMerge className="w-3.5 h-3.5" />}
                        </motion.button>
                      </div>
                    )}
                    {pr.state !== "open" && (
                      <span className={`text-[9px] font-medium px-2 py-0.5 rounded-full ${cfg.bg} ${cfg.color} border`}>{cfg.label}</span>
                    )}
                  </div>
                </motion.div>
              );
            }) : <EmptyState icon={GitPullRequest} text="No pull requests" />}
          </motion.div>
        )}

        {activeSection === "repos" && (
          <motion.div key="repos" initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -8 }} className="grid grid-cols-1 md:grid-cols-2 gap-3">
            {dashboard?.repos && dashboard.repos.length > 0 ? dashboard.repos.map((repo, i) => (
              <motion.div
                key={repo.name}
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: i * 0.04 }}
                whileHover={{ y: -2 }}
                className="relative overflow-hidden rounded-xl border border-border bg-card/60 hover:bg-card hover:border-primary/10 p-4 transition-all duration-200 group cursor-pointer"
              >
                <div className="absolute top-0 right-0 w-24 h-24 bg-primary/[0.02] rounded-full -translate-y-1/2 translate-x-1/3" />
                <div className="flex items-start justify-between mb-3">
                  <div className="flex items-center gap-2 min-w-0">
                    <GitBranch className="w-4 h-4 text-primary shrink-0" />
                    <p className="text-sm font-semibold text-foreground truncate">{repo.name}</p>
                  </div>
                  <ArrowUpRight className="w-3.5 h-3.5 text-muted-foreground/30 group-hover:text-primary transition-colors shrink-0" />
                </div>
                <div className="flex items-center gap-3 text-[11px] text-muted-foreground">
                  {repo.language && (
                    <span className="flex items-center gap-1.5">
                      <span className={`w-2 h-2 rounded-full ${langColors[repo.language] || "bg-muted-foreground"}`} />
                      {repo.language}
                    </span>
                  )}
                  <span className="flex items-center gap-1"><Star className="w-3 h-3 text-amber-500/60" />{repo.stars}</span>
                  <span className="flex items-center gap-1"><GitBranch className="w-3 h-3" />{repo.forks}</span>
                </div>
              </motion.div>
            )) : <EmptyState icon={GitBranch} text="No repositories found" />}
          </motion.div>
        )}

        {activeSection === "commits" && (
          <motion.div key="commits" initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -8 }} className="space-y-1">
            {dashboard?.recentCommits && dashboard.recentCommits.length > 0 ? (
              <div className="rounded-xl border border-border overflow-hidden">
                {dashboard.recentCommits.slice(0, 12).map((c, i) => (
                  <motion.div
                    key={c.sha}
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    transition={{ delay: i * 0.03 }}
                    className={`flex items-center gap-3 px-4 py-3 hover:bg-muted/20 transition-colors ${i !== 0 ? "border-t border-border/50" : ""}`}
                  >
                    <div className="w-6 h-6 rounded-md bg-primary/8 flex items-center justify-center shrink-0">
                      <GitCommit className="w-3 h-3 text-primary/60" />
                    </div>
                    <code className="text-[10px] text-primary font-mono shrink-0 bg-primary/5 px-1.5 py-0.5 rounded">{c.sha.slice(0, 7)}</code>
                    <p className="text-xs text-foreground truncate flex-1">{c.message}</p>
                    <span className="text-[10px] text-muted-foreground shrink-0 font-mono">{c.repo}</span>
                  </motion.div>
                ))}
              </div>
            ) : <EmptyState icon={GitCommit} text="No recent commits" />}
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  );
};

const Header = () => (
  <div className="flex items-center gap-3">
    <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-[#333] to-[#1a1a1a] border border-white/10 flex items-center justify-center shadow-lg">
      <svg className="w-5 h-5" viewBox="0 0 24 24" fill="white"><path d="M12 0C5.37 0 0 5.37 0 12c0 5.31 3.435 9.795 8.205 11.385.6.105.825-.255.825-.57 0-.285-.015-1.23-.015-2.235-3.015.555-3.795-.735-4.035-1.41-.135-.345-.72-1.41-1.23-1.695-.42-.225-1.02-.78-.015-.795.945-.015 1.62.87 1.845 1.23 1.08 1.815 2.805 1.305 3.495.99.105-.78.42-1.305.765-1.605-2.67-.3-5.46-1.335-5.46-5.925 0-1.305.465-2.385 1.23-3.225-.12-.3-.54-1.53.12-3.18 0 0 1.005-.315 3.3 1.23.96-.27 1.98-.405 3-.405s2.04.135 3 .405c2.295-1.56 3.3-1.23 3.3-1.23.66 1.65.24 2.88.12 3.18.765.84 1.23 1.905 1.23 3.225 0 4.605-2.805 5.625-5.475 5.925.435.375.81 1.095.81 2.22 0 1.605-.015 2.895-.015 3.3 0 .315.225.69.825.57A12.02 12.02 0 0024 12c0-6.63-5.37-12-12-12z"/></svg>
    </div>
    <div>
      <h2 className="text-lg font-bold text-foreground">GitHub</h2>
      <p className="text-xs text-muted-foreground">Repositories, pull requests & commits</p>
    </div>
  </div>
);

const EmptyState = ({ icon: Icon, text }: { icon: typeof GitBranch; text: string }) => (
  <div className="py-12 text-center col-span-full">
    <Icon className="w-8 h-8 text-muted-foreground/20 mx-auto mb-2" />
    <p className="text-xs text-muted-foreground">{text}</p>
  </div>
);

export default GitHubPanel;

