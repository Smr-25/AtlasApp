import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import {
  Palette, ImageDown, ArrowRight, Layers, PenTool,
  Sparkles, Eye, Ruler, ArrowUpRight,
} from "lucide-react";
import { useAuth } from "@/context/AuthContext";
import { greetingApi, designInsightsApi, GreetingDto, AssetsOptimizedDto, HandoffsDto, DesignDebtDto } from "@/services/api";

interface Props { onTabChange: (tab: string) => void; }

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

const DesignerOverviewPanel = ({ onTabChange }: Props) => {
  const { user } = useAuth();
  const [greeting, setGreeting] = useState<GreetingDto | null>(null);
  const [assets, setAssets] = useState<AssetsOptimizedDto | null>(null);
  const [handoffs, setHandoffs] = useState<HandoffsDto | null>(null);
  const [designDebt, setDesignDebt] = useState<DesignDebtDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = async () => {
      const [gRes, aRes, hRes, dRes] = await Promise.allSettled([
        greetingApi.get(user?.userName),
        designInsightsApi.assetsOptimized(),
        designInsightsApi.handoffs(),
        designInsightsApi.designDebt(),
      ]);
      if (gRes.status === "fulfilled" && gRes.value.data.isSuccess) setGreeting(gRes.value.data.data);
      if (aRes.status === "fulfilled" && aRes.value.data.isSuccess) setAssets(aRes.value.data.data);
      if (hRes.status === "fulfilled" && hRes.value.data.isSuccess) setHandoffs(hRes.value.data.data);
      if (dRes.status === "fulfilled" && dRes.value.data.isSuccess) setDesignDebt(dRes.value.data.data);
      setLoading(false);
    };
    load();
  }, [user?.userName]);

  const displayName = user?.fullName?.split(" ")[0] || "Designer";
  const greetText = greeting?.greeting || `Welcome back, ${displayName}`;

  const stats = [
    {
      label: "Assets Optimized", value: assets?.totalOptimized?.toString() || "0",
      sub: `${assets?.totalSavedMb?.toFixed(1) || 0} MB saved`,
      icon: ImageDown, color: "text-pink-400",
      bgFrom: "from-pink-500/12", bgTo: "to-pink-500/3", borderColor: "border-pink-500/10",
    },
    {
      label: "Handoffs Done", value: handoffs?.count?.toString() || "0",
      sub: "designs handed off",
      icon: Layers, color: "text-violet-400",
      bgFrom: "from-violet-500/12", bgTo: "to-violet-500/3", borderColor: "border-violet-500/10",
    },
    {
      label: "Design Debt", value: designDebt?.count?.toString() || "0",
      sub: "inconsistencies found",
      icon: PenTool, color: "text-amber-400",
      bgFrom: "from-amber-500/12", bgTo: "to-amber-500/3", borderColor: "border-amber-500/10",
    },
  ];

  const quickActions = [
    { id: "design-utilities", label: "Design Tools", desc: "Compress, convert, contrast check", icon: Sparkles, gradient: "from-pink-500/15 to-rose-500/5" },
    { id: "figma", label: "Figma", desc: "Comments & collaboration", icon: PenTool, gradient: "from-violet-500/15 to-purple-500/5" },
    { id: "miro", label: "Miro Boards", desc: "Whiteboard & brainstorm", icon: Layers, gradient: "from-amber-500/15 to-yellow-500/5" },
    { id: "zeplin", label: "Zeplin", desc: "Handoff & style guides", icon: Ruler, gradient: "from-cyan-500/15 to-teal-500/5" },
    { id: "dribbble", label: "Dribbble", desc: "Inspiration & trends", icon: Eye, gradient: "from-pink-500/15 to-red-500/5" },
    { id: "lottie", label: "LottieFiles", desc: "Animations library", icon: Sparkles, gradient: "from-emerald-500/15 to-green-500/5" },
  ];

  if (loading) {
    return (
      <div className="flex items-center justify-center py-20">
        <div className="flex flex-col items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center animate-pulse">
            <Palette className="w-5 h-5 text-primary" />
          </div>
          <p className="text-xs text-muted-foreground">Loading your creative space...</p>
        </div>
      </div>
    );
  }

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-8">
      {/* Greeting */}
      <motion.div variants={item}>
        <h1 className="text-2xl font-bold text-foreground tracking-tight">{greetText}</h1>
        <p className="text-sm text-muted-foreground mt-1">Here's your creative dashboard overview</p>
      </motion.div>

      {/* Stats */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        {stats.map((s, i) => (
          <motion.div key={i} variants={item}
            className={`p-5 rounded-2xl bg-gradient-to-br ${s.bgFrom} ${s.bgTo} border ${s.borderColor} group hover:shadow-lg transition-all duration-300`}>
            <div className="flex items-center gap-3 mb-3">
              <div className={`w-9 h-9 rounded-xl bg-gradient-to-br ${s.bgFrom} ${s.bgTo} flex items-center justify-center`}>
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
        <h2 className="text-sm font-bold text-foreground mb-4">Your Design Tools</h2>
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
          {quickActions.map((action) => (
            <motion.button key={action.id} variants={item} whileHover={{ y: -2, scale: 1.01 }} whileTap={{ scale: 0.99 }}
              onClick={() => onTabChange(action.id)}
              className={`group p-4 rounded-2xl bg-gradient-to-br ${action.gradient} border border-border/30 hover:border-primary/20 text-left transition-all duration-300 hover:shadow-lg`}>
              <div className="flex items-start justify-between mb-3">
                <div className="w-9 h-9 rounded-xl bg-card/60 border border-border/20 flex items-center justify-center">
                  <action.icon className="w-4 h-4 text-primary" />
                </div>
                <ArrowUpRight className="w-4 h-4 text-muted-foreground/30 group-hover:text-primary group-hover:translate-x-0.5 group-hover:-translate-y-0.5 transition-all" />
              </div>
              <p className="text-sm font-semibold text-foreground">{action.label}</p>
              <p className="text-xs text-muted-foreground mt-0.5">{action.desc}</p>
            </motion.button>
          ))}
        </div>
      </motion.div>
    </motion.div>
  );
};

export default DesignerOverviewPanel;

