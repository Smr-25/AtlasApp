import { useState } from "react";
import { motion, AnimatePresence } from "framer-motion";
import { Sparkles, Bug, Anchor, Container, GitCommit, FileSearch, Eye, Search, Loader2, Copy, Check, Send } from "lucide-react";
import { proactiveAgentsApi } from "@/services/api";

type Agent = { id: string; name: string; icon: typeof Bug; desc: string; placeholder: string; color: string };

const agents: Agent[] = [
  { id: "error", name: "Error Explainer", icon: Bug, desc: "Explain stack traces in plain English", placeholder: "Paste your stack trace or error message...", color: "text-red-400 bg-red-500/10" },
  { id: "port", name: "Port Resolver", icon: Anchor, desc: "Resolve port conflicts automatically", placeholder: "Enter port number (e.g. 3000)", color: "text-blue-400 bg-blue-500/10" },
  { id: "containers", name: "Container Killer", icon: Container, desc: "Find and kill idle Docker containers", placeholder: "Click run to scan for idle containers", color: "text-cyan-400 bg-cyan-500/10" },
  { id: "commit", name: "Commit Suggester", icon: GitCommit, desc: "Generate meaningful commit messages", placeholder: "Paste your git diff output...", color: "text-green-400 bg-green-500/10" },
  { id: "pr", name: "PR Summarizer", icon: FileSearch, desc: "Summarize pull request changes", placeholder: "Enter PR URL (e.g. https://github.com/user/repo/pull/1)", color: "text-purple-400 bg-purple-500/10" },
  { id: "deps", name: "Dep Watcher", icon: Eye, desc: "Watch dependencies for updates & issues", placeholder: "Enter project path or click run...", color: "text-amber-400 bg-amber-500/10" },
  { id: "search", name: "AI Search", icon: Search, desc: "AI-powered search (Perplexity)", placeholder: "Ask anything about coding, APIs, libraries...", color: "text-violet-400 bg-violet-500/10" },
];

const AIAgentsPanel = () => {
  const [activeAgent, setActiveAgent] = useState("error");
  const [input, setInput] = useState("");
  const [result, setResult] = useState<any>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [copied, setCopied] = useState(false);

  const current = agents.find((a) => a.id === activeAgent)!;

  const handleRun = async () => {
    setLoading(true); setError(""); setResult(null);
    try {
      let res: any;
      switch (activeAgent) {
        case "error": res = await proactiveAgentsApi.explainError({ stackTrace: input }); break;
        case "port": res = await proactiveAgentsApi.resolvePort({ port: parseInt(input) || 3000 }); break;
        case "containers": res = await proactiveAgentsApi.killIdleContainers(); break;
        case "commit": res = await proactiveAgentsApi.suggestCommit({ diff: input }); break;
        case "pr": res = await proactiveAgentsApi.summarizePr({ prUrl: input }); break;
        case "deps": res = await proactiveAgentsApi.watchDependencies({ projectPath: input || undefined }); break;
        case "search": res = await proactiveAgentsApi.search({ query: input }); break;
      }
      if (res?.data?.isSuccess) setResult(res.data.data);
      else setError(res?.data?.errors?.[0] || "Agent failed");
    } catch (err: any) {
      setError(err?.response?.data?.errors?.[0] || err.message || "Failed");
    } finally { setLoading(false); }
  };

  const copyResult = () => {
    const text = typeof result === "string" ? result : JSON.stringify(result, null, 2);
    navigator.clipboard.writeText(text);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-violet-500/20 to-violet-500/5 border border-violet-500/10 flex items-center justify-center shadow-lg shadow-violet-500/5">
          <Sparkles className="w-5 h-5 text-violet-400" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">AI Agents</h2>
          <p className="text-xs text-muted-foreground">Proactive AI assistants for your development workflow</p>
        </div>
      </div>

      {/* Agent Grid */}
      <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-2.5">
        {agents.map((agent) => (
          <motion.button
            key={agent.id}
            whileHover={{ y: -2, transition: { duration: 0.2 } }}
            whileTap={{ scale: 0.98 }}
            onClick={() => { setActiveAgent(agent.id); setInput(""); setResult(null); setError(""); }}
            className={`relative overflow-hidden flex items-center gap-2.5 p-3 rounded-xl border text-left transition-all ${
              activeAgent === agent.id ? "bg-primary/5 border-primary/20 shadow-sm" : "bg-gradient-to-br from-card to-card/60 border-border hover:border-primary/15"
            }`}
          >
            <div className="absolute top-0 right-0 w-12 h-12 bg-primary/[0.02] rounded-full -translate-y-1/2 translate-x-1/3" />
            <div className={`w-8 h-8 rounded-lg flex items-center justify-center shrink-0 ${agent.color} border border-current/10`}>
              <agent.icon className="w-4 h-4" />
            </div>
            <div className="min-w-0">
              <p className="text-xs font-medium text-foreground truncate">{agent.name}</p>
              <p className="text-[9px] text-muted-foreground truncate">{agent.desc}</p>
            </div>
          </motion.button>
        ))}
      </div>

      {/* Active Agent Panel */}
      <AnimatePresence mode="wait">
        <motion.div
          key={activeAgent}
          initial={{ opacity: 0, y: 10 }}
          animate={{ opacity: 1, y: 0 }}
          exit={{ opacity: 0, y: -10 }}
          className="bg-card rounded-xl border border-border overflow-hidden"
        >
          <div className="flex items-center gap-3 p-4 border-b border-border">
            <div className={`w-9 h-9 rounded-lg flex items-center justify-center ${current.color}`}>
              <current.icon className="w-5 h-5" />
            </div>
            <div>
              <h3 className="text-sm font-semibold text-foreground">{current.name}</h3>
              <p className="text-[11px] text-muted-foreground">{current.desc}</p>
            </div>
          </div>

          <div className="p-4 space-y-3">
            {activeAgent !== "containers" && (
              <div className="relative">
                <textarea
                  value={input}
                  onChange={(e) => setInput(e.target.value)}
                  placeholder={current.placeholder}
                  rows={activeAgent === "error" || activeAgent === "commit" ? 6 : 3}
                  className="w-full rounded-lg bg-muted/40 border border-border text-sm text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/30 p-3 font-mono resize-none transition-all"
                />
              </div>
            )}

            <motion.button
              whileTap={{ scale: 0.98 }}
              onClick={handleRun}
              disabled={loading || (activeAgent !== "containers" && !input.trim())}
              className="w-full h-10 rounded-lg bg-primary text-primary-foreground text-sm font-medium shadow-md shadow-primary/20 disabled:opacity-50 flex items-center justify-center gap-2"
            >
              {loading ? <><Loader2 className="w-4 h-4 animate-spin" />Analyzing...</> : <><Send className="w-4 h-4" />Run Agent</>}
            </motion.button>

            {error && (
              <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="p-3 rounded-lg bg-destructive/10 border border-destructive/20 text-destructive text-xs">{error}</motion.div>
            )}

            {result && (
              <motion.div initial={{ opacity: 0, y: 5 }} animate={{ opacity: 1, y: 0 }} className="space-y-2">
                <div className="flex items-center justify-between">
                  <span className="text-xs font-medium text-foreground flex items-center gap-1.5">
                    <Sparkles className="w-3.5 h-3.5 text-primary" /> AI Response
                  </span>
                  <button onClick={copyResult} className="flex items-center gap-1 text-[10px] text-primary hover:underline">
                    {copied ? <Check className="w-3 h-3" /> : <Copy className="w-3 h-3" />}{copied ? "Copied" : "Copy"}
                  </button>
                </div>
                <pre className="p-4 rounded-lg bg-muted/50 border border-border text-xs text-foreground font-mono overflow-x-auto max-h-80 whitespace-pre-wrap leading-relaxed">
                  {typeof result === "string" ? result : JSON.stringify(result, null, 2)}
                </pre>
              </motion.div>
            )}
          </div>
        </motion.div>
      </AnimatePresence>
    </div>
  );
};

export default AIAgentsPanel;

