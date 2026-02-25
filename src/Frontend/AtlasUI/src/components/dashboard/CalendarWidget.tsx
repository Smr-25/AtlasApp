import { Plus, Clock } from "lucide-react";
import { motion } from "framer-motion";

const days = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];
const dates = [16, 17, 18, 19, 20, 21, 22];
const today = 19;

const tasks = [
  {
    title: "Business Analysis",
    time: "09:30 AM",
    avatars: ["DP", "FA"],
    colors: ["bg-blue-500", "bg-purple-500"],
  },
  {
    title: "Preparation of the MVP",
    time: "07:15 AM",
    avatars: ["MC", "DP"],
    colors: ["bg-pink-500", "bg-blue-500"],
  },
];

const CalendarWidget = () => {
  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5, delay: 0.2, ease: "easeOut" }}
      className="bg-card rounded-2xl border border-border p-5 flex flex-col"
    >
      {/* Header */}
      <div className="flex items-center justify-between mb-4">
        <h3 className="text-sm font-semibold text-foreground">February 2026</h3>
      </div>

      {/* Week view */}
      <div className="grid grid-cols-7 gap-1 mb-4">
        {days.map((day, i) => (
          <motion.div
            key={day}
            initial={{ opacity: 0, y: -10 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.3 + i * 0.05 }}
            className="flex flex-col items-center gap-1"
          >
            <span className="text-[10px] text-muted-foreground">{day}</span>
            <motion.div
              whileHover={{ scale: 1.15 }}
              whileTap={{ scale: 0.95 }}
              className={`w-8 h-8 rounded-full flex items-center justify-center text-xs font-medium cursor-pointer transition-colors ${
                dates[i] === today
                  ? "bg-primary text-primary-foreground shadow-md shadow-primary/30"
                  : "text-foreground hover:bg-muted"
              }`}
            >
              {dates[i]}
            </motion.div>
          </motion.div>
        ))}
      </div>

      {/* Tasks */}
      <div className="space-y-2 flex-1">
        {tasks.map((task, index) => (
          <motion.div
            key={task.title}
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ delay: 0.5 + index * 0.15 }}
            whileHover={{ x: 4, backgroundColor: "hsl(var(--muted) / 0.8)" }}
            className="flex items-center gap-3 p-3 rounded-xl bg-muted/50 border border-border cursor-pointer transition-colors"
          >
            <div className="flex-1 min-w-0">
              <p className="text-xs font-medium text-foreground truncate">{task.title}</p>
              <div className="flex items-center gap-1 mt-1">
                <Clock className="w-3 h-3 text-muted-foreground" />
                <span className="text-[10px] text-muted-foreground">{task.time}</span>
              </div>
            </div>
            <div className="flex -space-x-1.5">
              {task.avatars.map((av, i) => (
                <motion.div
                  key={i}
                  whileHover={{ scale: 1.2, zIndex: 10 }}
                  className={`w-6 h-6 rounded-full ${task.colors[i]} flex items-center justify-center ring-2 ring-card`}
                >
                  <span className="text-[8px] font-medium text-white">{av}</span>
                </motion.div>
              ))}
            </div>
          </motion.div>
        ))}
      </div>

      {/* Add appointment */}
      <motion.button
        whileHover={{ scale: 1.02, borderColor: "hsl(var(--primary))" }}
        whileTap={{ scale: 0.98 }}
        className="mt-3 flex items-center justify-center gap-1.5 h-9 w-full rounded-xl border border-primary/30 text-primary text-xs font-medium hover:bg-primary/5 transition-colors"
      >
        <Plus className="w-3.5 h-3.5" />
        Make an appointment
      </motion.button>
    </motion.div>
  );
};

export default CalendarWidget;
