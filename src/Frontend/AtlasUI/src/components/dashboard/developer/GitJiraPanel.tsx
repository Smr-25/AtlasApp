import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import { GitBranch, GitPullRequest, GitCommit, Star, ExternalLink, Check, X as XIcon, GitMerge, Loader2, AlertCircle } from "lucide-react";
import { gitApi, GitDashboardDto, IntegrationDto } from "@/services/api";

interface GitJiraPanelProps {
  integrations: IntegrationDto[];
}

const GitJiraPanel = ({ integrations }: GitJiraPanelProps) => {
  const [dashboard, setDashboard] = useState<GitDashboardDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [actionLoading, setActionLoading] = useState<string | null>(null);
  const [error, setError] = useState("");

  const githubIntegration = integrations.find((i) => i.provider === "GitHub" && i.status === "Active");

  useEffect(() => {
    if (!githubIntegration) { setLoading(false); return; }
    const load = async () => {
      setLoading(true);
      try {
        const res = await gitApi.dashboard(githubIntegration.id);
        if (res.data.isSuccess) setDashboard(res.data.data);
        else setError(res.data.errors?.[0] || "Failed to load");
      } catch (err: any) {
        setError(err?.response?.data?.errors?.[0] || "Failed to load GitHub data");
      }
      setLoading(false);
    };
    load();
  }, [githubIntegration?.id]);

  const handlePrAction = async (action: "approve" | "reject" | "merge", pr: any) => {
    if (!githubIntegration) return;
    const key = `${action}-${pr.id}`;
    setActionLoading(key);
    try {
      const body = { integrationId: githubIntegration.id, owner: pr.repo?.split("/")[0] || "", repo: pr.repo?.split("/")[1] || pr.repo, prNumber: pr.id };
      if (action === "approve") await gitApi.approve(body);
      else if (action === "reject") await gitApi.reject({ ...body, reason: "Needs changes" });
      else await gitApi.merge(body);
    } catch {}
    setActionLoading(null);
  };

  if (!githubIntegration) {
    return (
      <div className="space-y-5">
        <div>
          <h2 className="text-lg font-bold text-foreground flex items-center gap-2"><GitBranch className="w-5 h-5 text-primary" /> Git & Jira</h2>
          <p className="text-sm text-muted-foreground">Connect your GitHub integration first</p>
        </div>
        <div className="py-16 text-center">
          <GitBranch className="w-12 h-12 text-muted-foreground/20 mx-auto mb-3" />
          <p className="text-sm font-medium text-foreground mb-1">No GitHub integration active</p>
          <p className="text-xs text-muted-foreground">Go to Integrations tab and connect your GitHub account</p>
        </div>
      </div>
    );
  }

  if (loading) return <div className="py-20 flex justify-center"><Loader2 className="w-6 h-6 animate-spin text-primary" /></div>;

  if (error) {
    return (
      <div className="space-y-5">
        <div><h2 className="text-lg font-bold text-foreground flex items-center gap-2"><GitBranch className="w-5 h-5 text-primary" /> Git & Jira</h2></div>
        <div className="p-4 rounded-xl bg-destructive/10 border border-destructive/20 text-destructive text-sm flex items-center gap-2"><AlertCircle className="w-5 h-5" />{error}</div>
      </div>
    );
  }

  return (
    <div className="space-y-5">
      <div>
        <h2 className="text-lg font-bold text-foreground flex items-center gap-2"><GitBranch className="w-5 h-5 text-primary" /> Git & Jira</h2>
        <p className="text-sm text-muted-foreground">Your GitHub overview and PR management</p>
      </div>

      {/* Repos */}
      {dashboard?.repos && dashboard.repos.length > 0 && (
        <div>
          <h3 className="text-xs font-semibold text-muted-foreground uppercase tracking-wider mb-2">Repositories</h3>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-3">
            {dashboard.repos.slice(0, 6).map((repo, i) => (
              <motion.div key={repo.name} initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: i * 0.03 }} className="bg-card rounded-xl border border-border p-4 hover:border-primary/15 transition-all">
                <div className="flex items-center justify-between mb-2">
                  <p className="text-sm font-medium text-foreground truncate">{repo.name}</p>
                  <ExternalLink className="w-3.5 h-3.5 text-muted-foreground/40 shrink-0" />
                </div>
                <div className="flex items-center gap-3 text-[11px] text-muted-foreground">
                  {repo.language && <span className="bg-primary/10 text-primary px-1.5 py-0.5 rounded text-[9px]">{repo.language}</span>}
                  <span className="flex items-center gap-1"><Star className="w-3 h-3" />{repo.stars}</span>
                  <span className="flex items-center gap-1"><GitBranch className="w-3 h-3" />{repo.forks}</span>
                </div>
              </motion.div>
            ))}
          </div>
        </div>
      )}

      {/* Pull Requests */}
      {dashboard?.pullRequests && dashboard.pullRequests.length > 0 && (
        <div>
          <h3 className="text-xs font-semibold text-muted-foreground uppercase tracking-wider mb-2 flex items-center gap-1.5"><GitPullRequest className="w-3.5 h-3.5" /> Pull Requests</h3>
          <div className="space-y-2">
            {dashboard.pullRequests.map((pr, i) => (
              <motion.div key={pr.id} initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: i * 0.03 }} className="flex items-center gap-3 p-3 bg-card rounded-xl border border-border hover:border-primary/15 transition-all group">
                <GitPullRequest className={`w-4 h-4 shrink-0 ${pr.state === "open" ? "text-emerald-500" : pr.state === "merged" ? "text-purple-500" : "text-red-500"}`} />
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium text-foreground truncate">{pr.title}</p>
                  <p className="text-[11px] text-muted-foreground">#{pr.id} · {pr.author} · {pr.repo}</p>
                </div>
                {pr.state === "open" && (
                  <div className="flex items-center gap-1 shrink-0 opacity-0 group-hover:opacity-100 transition-opacity">
                    <button onClick={() => handlePrAction("approve", pr)} disabled={!!actionLoading} className="w-7 h-7 rounded-lg flex items-center justify-center text-emerald-500 hover:bg-emerald-500/10 transition-colors" title="Approve">
                      {actionLoading === `approve-${pr.id}` ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : <Check className="w-3.5 h-3.5" />}
                    </button>
                    <button onClick={() => handlePrAction("reject", pr)} disabled={!!actionLoading} className="w-7 h-7 rounded-lg flex items-center justify-center text-red-500 hover:bg-red-500/10 transition-colors" title="Request Changes">
                      <XIcon className="w-3.5 h-3.5" />
                    </button>
                    <button onClick={() => handlePrAction("merge", pr)} disabled={!!actionLoading} className="w-7 h-7 rounded-lg flex items-center justify-center text-purple-500 hover:bg-purple-500/10 transition-colors" title="Merge">
                      {actionLoading === `merge-${pr.id}` ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : <GitMerge className="w-3.5 h-3.5" />}
                    </button>
                  </div>
                )}
              </motion.div>
            ))}
          </div>
        </div>
      )}

      {/* Recent Commits */}
      {dashboard?.recentCommits && dashboard.recentCommits.length > 0 && (
        <div>
          <h3 className="text-xs font-semibold text-muted-foreground uppercase tracking-wider mb-2 flex items-center gap-1.5"><GitCommit className="w-3.5 h-3.5" /> Recent Commits</h3>
          <div className="space-y-1">
            {dashboard.recentCommits.slice(0, 8).map((c) => (
              <div key={c.sha} className="flex items-center gap-3 p-2.5 rounded-lg hover:bg-muted/30 transition-colors">
                <code className="text-[10px] text-primary font-mono shrink-0">{c.sha.slice(0, 7)}</code>
                <p className="text-xs text-foreground truncate flex-1">{c.message}</p>
                <span className="text-[10px] text-muted-foreground shrink-0">{c.repo}</span>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
};

export default GitJiraPanel;

