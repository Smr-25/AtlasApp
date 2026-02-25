import { TrendingUp, TrendingDown } from "lucide-react";
import { motion } from "framer-motion";

const stats = [
  { label: "Iterations", value: "282", change: "+38.12%", positive: true, sub: "from previous weeks" },
  { label: "KPI", value: "3.78", change: "-5.6%", positive: false, sub: "from previous weeks" },
  { label: "Meetings", value: "4.8h", change: "+28.3%", positive: true, sub: "from previous weeks" },
  { label: "Finished", value: "94%", change: "+3.1%", positive: true, sub: "from previous weeks" },
];

const StatsCards = () => {
  return (
    <div className="grid grid-cols-2 lg:grid-cols-4 gap-3">
      {stats.map((stat, index) => (
        <motion.div
          key={stat.label}
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.4, delay: 0.3 + index * 0.1, ease: "easeOut" }}
          whileHover={{ y: -3, boxShadow: "0 8px 25px -8px hsl(var(--primary) / 0.12)" }}
          className="bg-card rounded-2xl border border-border p-4 flex flex-col cursor-pointer transition-colors"
        >
          <p className="text-[11px] text-muted-foreground mb-2">{stat.label}</p>
          <motion.p
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{ delay: 0.5 + index * 0.1 }}
            className="text-2xl font-semibold text-foreground"
          >
            {stat.value}
          </motion.p>
          <div className="flex items-center gap-1 mt-1.5">
            {stat.positive ? (
              <TrendingUp className="w-3 h-3 text-momentum-green" />
            ) : (
              <TrendingDown className="w-3 h-3 text-momentum-red" />
            )}
            <span
              className={`text-[10px] font-medium ${
                stat.positive ? "text-momentum-green" : "text-momentum-red"
              }`}
            >
              {stat.change}
            </span>
            <span className="text-[10px] text-muted-foreground">{stat.sub}</span>
          </div>
        </motion.div>
      ))}
    </div>
  );
};

export default StatsCards;
