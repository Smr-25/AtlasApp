import React, { Suspense, useEffect } from "react";
import { motion } from "framer-motion";

import TopNav from "@/components/dashboard/TopNav";
import Sidebar from "@/components/dashboard/Sidebar";
import { useAuth } from "@/context/AuthContext";
import { useTheme } from "@/context/ThemeContext";

const UpdatesCard = React.lazy(() => import("@/components/dashboard/UpdatesCard"));
const HeroBanner = React.lazy(() => import("@/components/dashboard/HeroBanner"));
const CalendarWidget = React.lazy(() => import("@/components/dashboard/CalendarWidget"));
const StatsCards = React.lazy(() => import("@/components/dashboard/StatsCards"));
const HeatmapWidget = React.lazy(() => import("@/components/dashboard/HeatmapWidget"));

const Dashboard = () => {
  const { user } = useAuth();
  const { setRole } = useTheme();

  useEffect(() => {
    if (user?.role) {
      setRole(user.role);
    }
  }, [user?.role, setRole]);

  const displayName = user?.fullName?.split(" ")[0] || "Oliver";

  return (
    <div className="flex flex-col h-screen overflow-hidden">
      <TopNav />
      <div className="flex flex-1 overflow-hidden">
        <Sidebar />
        <main className="flex-1 overflow-y-auto p-6">
          <motion.div
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.4 }}
            className="mb-6"
          >
            <h2 className="text-xl font-semibold text-foreground">
              Hi, {displayName}! <span className="font-normal text-muted-foreground">Let's customize your workspace!</span>
            </h2>
          </motion.div>

          <Suspense fallback={<div className="mb-4">Loading widgets...</div>}>
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-4 mb-4">
              <UpdatesCard />
              <HeroBanner />
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-[1fr_2fr] gap-4 mb-4">
              <CalendarWidget />
              <div className="flex flex-col gap-4">
                <StatsCards />
                <HeatmapWidget />
              </div>
            </div>
          </Suspense>
        </main>
      </div>
    </div>
  );
};

const Index = () => {
  return <Dashboard />;
};

export default Index;
