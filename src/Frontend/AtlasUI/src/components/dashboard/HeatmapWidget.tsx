import { motion } from "framer-motion";

const rows = ["FEB", "FTI", "FKG", "FKH", "FKS"];
const cols = [18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29];

const heatData: number[][] = [
  [1, 2, 3, 2, 1, 0, 0, 2, 3, 1, 2, 1],
  [0, 1, 2, 3, 2, 1, 0, 1, 2, 3, 1, 0],
  [2, 3, 1, 0, 1, 2, 3, 2, 1, 0, 1, 2],
  [3, 2, 1, 2, 3, 0, 1, 3, 2, 1, 0, 1],
  [1, 0, 2, 1, 0, 3, 2, 1, 3, 2, 1, 0],
];

const levelColors = [
  "bg-primary/10",
  "bg-primary/30",
  "bg-primary/60",
  "bg-primary",
];

const legends = [
  { label: "Low", level: 0 },
  { label: "Medium", level: 1 },
  { label: "High", level: 2 },
  { label: "Fully Occupied", level: 3 },
];

const HeatmapWidget = () => {
  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5, delay: 0.5, ease: "easeOut" }}
      className="bg-card rounded-2xl border border-border p-5"
    >
      <div className="flex items-center justify-between mb-4">
        <h3 className="text-sm font-semibold text-foreground">Weekly Workload</h3>
        <div className="flex items-center gap-3">
          {legends.map((l) => (
            <div key={l.label} className="flex items-center gap-1">
              <div className={`w-3 h-3 rounded-sm ${levelColors[l.level]}`} />
              <span className="text-[10px] text-muted-foreground">{l.label}</span>
            </div>
          ))}
        </div>
      </div>

      {/* Grid */}
      <div className="overflow-x-auto">
        <div className="min-w-[500px]">
          {/* Column headers */}
          <div className="grid grid-cols-[48px_repeat(12,1fr)] gap-1 mb-1">
            <div />
            {cols.map((c) => (
              <div key={c} className="text-center text-[10px] text-muted-foreground">
                {c}
              </div>
            ))}
          </div>

          {/* Rows */}
          {rows.map((row, ri) => (
            <div key={row} className="grid grid-cols-[48px_repeat(12,1fr)] gap-1 mb-1">
              <div className="text-[10px] text-muted-foreground flex items-center">{row}</div>
              {heatData[ri].map((level, ci) => (
                <motion.div
                  key={ci}
                  initial={{ opacity: 0, scale: 0.5 }}
                  animate={{ opacity: 1, scale: 1 }}
                  transition={{ delay: 0.6 + ri * 0.05 + ci * 0.02, duration: 0.3 }}
                  whileHover={{ scale: 1.15, zIndex: 10 }}
                  className={`h-7 rounded-md ${levelColors[level]} cursor-pointer transition-shadow hover:shadow-md hover:shadow-primary/20`}
                />
              ))}
            </div>
          ))}
        </div>
      </div>
    </motion.div>
  );
};

export default HeatmapWidget;
