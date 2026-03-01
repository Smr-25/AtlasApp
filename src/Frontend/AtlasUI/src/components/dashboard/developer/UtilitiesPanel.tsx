import { useState } from "react";
import { motion, AnimatePresence } from "framer-motion";
import { Terminal, Key, Regex, Clock, Binary, Shield, Code2, Globe, Search, Cpu, Trash2, Loader2, Copy, Check, ChevronRight } from "lucide-react";
import { devUtilitiesApi } from "@/services/api";

type UtilTool = { id: string; name: string; icon: typeof Terminal; desc: string; color: string };

const tools: UtilTool[] = [
  { id: "jwt", name: "JWT Decoder", icon: Key, desc: "Decode & inspect JWT tokens", color: "text-amber-400 bg-amber-500/10" },
  { id: "regex", name: "Regex Tester", icon: Regex, desc: "Test regular expressions", color: "text-green-400 bg-green-500/10" },
  { id: "cron", name: "Cron Generator", icon: Clock, desc: "Generate cron expressions with AI", color: "text-blue-400 bg-blue-500/10" },
  { id: "base64", name: "Base64", icon: Binary, desc: "Encode/decode base64 strings", color: "text-purple-400 bg-purple-500/10" },
  { id: "ssh", name: "SSH Key Gen", icon: Shield, desc: "Generate SSH key pairs", color: "text-red-400 bg-red-500/10" },
  { id: "json", name: "JSON Formatter", icon: Code2, desc: "Format & validate JSON", color: "text-cyan-400 bg-cyan-500/10" },
  { id: "http", name: "HTTP Client", icon: Globe, desc: "Send HTTP requests", color: "text-orange-400 bg-orange-500/10" },
  { id: "deps", name: "Dep Scanner", icon: Search, desc: "Scan for vulnerabilities", color: "text-rose-400 bg-rose-500/10" },
  { id: "port", name: "Port Checker", icon: Cpu, desc: "Check if a port is in use", color: "text-teal-400 bg-teal-500/10" },
  { id: "kill", name: "Kill Process", icon: Trash2, desc: "Kill a running process", color: "text-red-400 bg-red-500/10" },
];

const UtilitiesPanel = () => {
  const [activeTool, setActiveTool] = useState<string>("jwt");
  const [input, setInput] = useState("");
  const [input2, setInput2] = useState("");
  const [result, setResult] = useState<any>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [copied, setCopied] = useState(false);

  const resetState = () => { setInput(""); setInput2(""); setResult(null); setError(""); };
  const handleToolChange = (id: string) => { setActiveTool(id); resetState(); };

  const copyToClipboard = (text: string) => {
    navigator.clipboard.writeText(text);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  const handleRun = async () => {
    setLoading(true); setError(""); setResult(null);
    try {
      let res: any;
      switch (activeTool) {
        case "jwt": res = await devUtilitiesApi.decodeJwt(input); break;
        case "regex": res = await devUtilitiesApi.testRegex({ pattern: input, input: input2 }); break;
        case "cron": res = await devUtilitiesApi.generateCron({ description: input }); break;
        case "base64": res = await devUtilitiesApi.base64({ input, encode: input2 !== "decode" }); break;
        case "ssh": res = await devUtilitiesApi.sshKey({ type: input || "rsa", bits: 4096 }); break;
        case "json": res = await devUtilitiesApi.jsonFormat({ json: input }); break;
        case "http": res = await devUtilitiesApi.sendRequest({ method: input2 || "GET", url: input }); break;
        case "deps": res = await devUtilitiesApi.scanDependencies({ projectPath: input }); break;
        case "port": res = await devUtilitiesApi.checkPort(parseInt(input) || 3000); break;
        case "kill": res = await devUtilitiesApi.killProcess(parseInt(input) || 0); break;
      }
      if (res?.data?.isSuccess) setResult(res.data.data);
      else setError(res?.data?.errors?.[0] || "Operation failed");
    } catch (err: any) {
      setError(err?.response?.data?.errors?.[0] || err.message || "Failed");
    } finally {
      setLoading(false);
    }
  };

  const currentTool = tools.find((t) => t.id === activeTool)!;
  const resultText = result ? (typeof result === "string" ? result : JSON.stringify(result, null, 2)) : "";

  const inputClass = "w-full rounded-lg bg-muted/40 border border-border text-sm text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/30 focus:border-primary/40 transition-all font-mono";

  return (
    <div className="space-y-5">
      <div>
        <h2 className="text-lg font-bold text-foreground flex items-center gap-2">
          <Terminal className="w-5 h-5 text-primary" /> Developer Utilities
        </h2>
        <p className="text-sm text-muted-foreground">Essential dev tools at your fingertips</p>
      </div>

      <div className="flex gap-4">
        {/* Tool Sidebar */}
        <div className="w-48 shrink-0 space-y-0.5">
          {tools.map((tool) => (
            <motion.button
              key={tool.id}
              whileHover={{ x: 2 }}
              onClick={() => handleToolChange(tool.id)}
              className={`w-full flex items-center gap-2.5 px-3 py-2 rounded-lg text-xs transition-all ${
                activeTool === tool.id ? "bg-primary/10 text-primary font-medium" : "text-muted-foreground hover:bg-muted hover:text-foreground"
              }`}
            >
              <tool.icon className="w-3.5 h-3.5 shrink-0" />
              <span className="truncate">{tool.name}</span>
            </motion.button>
          ))}
        </div>

        {/* Tool Content */}
        <div className="flex-1 min-w-0">
          <AnimatePresence mode="wait">
            <motion.div
              key={activeTool}
              initial={{ opacity: 0, x: 10 }}
              animate={{ opacity: 1, x: 0 }}
              exit={{ opacity: 0, x: -10 }}
              className="bg-card rounded-xl border border-border overflow-hidden"
            >
              {/* Header */}
              <div className="flex items-center gap-3 p-4 border-b border-border">
                <div className={`w-9 h-9 rounded-lg flex items-center justify-center ${currentTool.color}`}>
                  <currentTool.icon className="w-4.5 h-4.5" />
                </div>
                <div>
                  <h3 className="text-sm font-semibold text-foreground">{currentTool.name}</h3>
                  <p className="text-[11px] text-muted-foreground">{currentTool.desc}</p>
                </div>
              </div>

              {/* Input */}
              <div className="p-4 space-y-3">
                {activeTool === "regex" ? (
                  <>
                    <div>
                      <label className="text-xs font-medium text-foreground mb-1 block">Pattern</label>
                      <input value={input} onChange={(e) => setInput(e.target.value)} placeholder="/your-regex/gi" className={`${inputClass} h-10 px-3`} />
                    </div>
                    <div>
                      <label className="text-xs font-medium text-foreground mb-1 block">Test String</label>
                      <textarea value={input2} onChange={(e) => setInput2(e.target.value)} placeholder="String to test against..." rows={3} className={`${inputClass} p-3`} />
                    </div>
                  </>
                ) : activeTool === "http" ? (
                  <>
                    <div className="flex gap-2">
                      <select value={input2} onChange={(e) => setInput2(e.target.value)} className={`${inputClass} h-10 px-3 w-28`}>
                        <option value="GET">GET</option>
                        <option value="POST">POST</option>
                        <option value="PUT">PUT</option>
                        <option value="DELETE">DELETE</option>
                        <option value="PATCH">PATCH</option>
                      </select>
                      <input value={input} onChange={(e) => setInput(e.target.value)} placeholder="https://api.example.com/data" className={`${inputClass} h-10 px-3 flex-1`} />
                    </div>
                  </>
                ) : activeTool === "base64" ? (
                  <>
                    <div className="flex gap-2 mb-2">
                      <button onClick={() => setInput2("encode")} className={`px-3 py-1.5 rounded-md text-xs font-medium transition-all ${input2 !== "decode" ? "bg-primary/10 text-primary" : "text-muted-foreground hover:bg-muted"}`}>Encode</button>
                      <button onClick={() => setInput2("decode")} className={`px-3 py-1.5 rounded-md text-xs font-medium transition-all ${input2 === "decode" ? "bg-primary/10 text-primary" : "text-muted-foreground hover:bg-muted"}`}>Decode</button>
                    </div>
                    <textarea value={input} onChange={(e) => setInput(e.target.value)} placeholder="Enter text..." rows={4} className={`${inputClass} p-3`} />
                  </>
                ) : activeTool === "port" || activeTool === "kill" ? (
                  <input type="number" value={input} onChange={(e) => setInput(e.target.value)} placeholder={activeTool === "port" ? "Port number (e.g. 3000)" : "Process ID (PID)"} className={`${inputClass} h-10 px-3`} />
                ) : (
                  <textarea
                    value={input}
                    onChange={(e) => setInput(e.target.value)}
                    placeholder={
                      activeTool === "jwt" ? "Paste your JWT token here..." :
                      activeTool === "cron" ? "Describe the schedule (e.g. every weekday at 9am)..." :
                      activeTool === "json" ? '{"key": "value"}' :
                      activeTool === "deps" ? "/path/to/project" :
                      activeTool === "ssh" ? "Key type (rsa, ed25519)" :
                      "Enter input..."
                    }
                    rows={activeTool === "json" ? 6 : 4}
                    className={`${inputClass} p-3`}
                  />
                )}

                {/* Run Button */}
                <motion.button
                  whileTap={{ scale: 0.98 }}
                  onClick={handleRun}
                  disabled={loading || !input.trim()}
                  className="w-full h-10 rounded-lg bg-primary text-primary-foreground text-sm font-medium shadow-md shadow-primary/20 disabled:opacity-50 flex items-center justify-center gap-2"
                >
                  {loading ? <><Loader2 className="w-4 h-4 animate-spin" />Processing...</> : <><ChevronRight className="w-4 h-4" />Run</>}
                </motion.button>

                {/* Error */}
                {error && (
                  <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="p-3 rounded-lg bg-destructive/10 border border-destructive/20 text-destructive text-xs">
                    {error}
                  </motion.div>
                )}

                {/* Result */}
                {result !== null && (
                  <motion.div initial={{ opacity: 0, y: 5 }} animate={{ opacity: 1, y: 0 }}>
                    <div className="flex items-center justify-between mb-1.5">
                      <label className="text-xs font-medium text-foreground">Result</label>
                      <button onClick={() => copyToClipboard(resultText)} className="flex items-center gap-1 text-[10px] text-primary hover:underline">
                        {copied ? <Check className="w-3 h-3" /> : <Copy className="w-3 h-3" />}
                        {copied ? "Copied" : "Copy"}
                      </button>
                    </div>
                    <pre className="p-3 rounded-lg bg-muted/50 border border-border text-xs text-foreground font-mono overflow-x-auto max-h-64 whitespace-pre-wrap">
                      {resultText}
                    </pre>
                  </motion.div>
                )}
              </div>
            </motion.div>
          </AnimatePresence>
        </div>
      </div>
    </div>
  );
};

export default UtilitiesPanel;

