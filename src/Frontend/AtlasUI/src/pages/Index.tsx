import TopNav from "@/components/dashboard/TopNav";
import Sidebar from "@/components/dashboard/Sidebar";
import UpdatesCard from "@/components/dashboard/UpdatesCard";
import HeroBanner from "@/components/dashboard/HeroBanner";
import CalendarWidget from "@/components/dashboard/CalendarWidget";
import StatsCards from "@/components/dashboard/StatsCards";
import HeatmapWidget from "@/components/dashboard/HeatmapWidget";
import { ThemeProvider } from "@/context/ThemeContext";
import { motion } from "framer-motion";

const Dashboard = () => {
  return (
    <div className="flex flex-col h-screen overflow-hidden">
      <TopNav />
      <div className="flex flex-1 overflow-hidden">
        <Sidebar />
        <main className="flex-1 overflow-y-auto p-6">
          {/* Greeting */}
          <motion.div
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.4 }}
            className="mb-6"
          >
            <h2 className="text-xl font-semibold text-foreground">
              Hi, Oliver! <span className="font-normal text-muted-foreground">Let's customize your workspace!</span>
            </h2>
          </motion.div>

          {/* Top row: Updates + Hero Banner */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-4 mb-4">
            <UpdatesCard />
            <HeroBanner />
          </div>

          {/* Middle row: Calendar + Stats */}
          <div className="grid grid-cols-1 lg:grid-cols-[1fr_2fr] gap-4 mb-4">
            <CalendarWidget />
            <div className="flex flex-col gap-4">
              <StatsCards />
              <HeatmapWidget />
            </div>
          </div>
        </main>
      </div>
    </div>
  );
};

const Index = () => {
  return (
    <ThemeProvider>
      <Dashboard />
    </ThemeProvider>
  );
};

export default Index;
