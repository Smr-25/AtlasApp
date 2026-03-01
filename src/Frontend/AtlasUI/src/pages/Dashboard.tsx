import { useState, useEffect } from "react";
import { Navigate } from "react-router-dom";
import { motion } from "framer-motion";
import { Loader2, Sparkles } from "lucide-react";
import { useAuth } from "@/context/AuthContext";
import { useTheme } from "@/context/ThemeContext";
import { useWorkspaces } from "@/hooks/use-workspace";
import DashboardTopNav from "@/components/dashboard/DashboardTopNav";
import DashboardSidebar from "@/components/dashboard/DashboardSidebar";
import OverviewPanel from "@/components/dashboard/OverviewPanel";
import WorkspacesPanel from "@/components/dashboard/WorkspacesPanel";
import IntegrationsPanel from "@/components/dashboard/IntegrationsPanel";
import CreateWorkspaceDialog from "@/components/dashboard/CreateWorkspaceDialog";

const AiPlaceholder = () => (
  <div className="flex flex-col items-center justify-center py-20 text-center">
    <div className="w-16 h-16 rounded-2xl bg-primary/10 flex items-center justify-center mb-4">
      <Sparkles className="w-8 h-8 text-primary" />
    </div>
    <h2 className="text-lg font-bold text-foreground mb-1">Atlas AI Assistant</h2>
    <p className="text-sm text-muted-foreground max-w-sm">
      Your intelligent workspace companion is coming soon. Stay tuned for AI-powered insights, automation and more.
    </p>
  </div>
);

const Dashboard = () => {
  const { user } = useAuth();
  const { setRole } = useTheme();
  const [activeTab, setActiveTab] = useState("overview");
  const [createDialogOpen, setCreateDialogOpen] = useState(false);

  const {
    workspaces,
    integrations,
    pendingIntegrations,
    activeWorkspace,
    switchWorkspace,
    createWorkspace,
    deleteWorkspace,
    setDefaultWorkspace,
    loading,
    refresh,
  } = useWorkspaces();

  useEffect(() => {
    if (user?.role) setRole(user.role);
  }, [user?.role, setRole]);

  if (user && !user.onboardingComplete) return <Navigate to="/onboarding" replace />;

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-background">
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          className="flex flex-col items-center gap-3"
        >
          <Loader2 className="w-8 h-8 animate-spin text-primary" />
          <span className="text-sm text-muted-foreground">Loading your workspace...</span>
        </motion.div>
      </div>
    );
  }

  return (
    <div className="flex flex-col h-screen overflow-hidden bg-background">
      <DashboardTopNav
        activeWorkspace={activeWorkspace}
        workspaces={workspaces}
        onSwitchWorkspace={switchWorkspace}
      />
      <div className="flex flex-1 overflow-hidden">
        <DashboardSidebar
          workspaces={workspaces}
          integrations={integrations}
          pendingIntegrations={pendingIntegrations}
          activeWorkspace={activeWorkspace}
          onSwitchWorkspace={switchWorkspace}
          onCreateWorkspace={() => setCreateDialogOpen(true)}
          activeTab={activeTab}
          onTabChange={setActiveTab}
        />
        <main className="flex-1 overflow-y-auto">
          <div className="max-w-5xl mx-auto p-6">
            {activeTab === "overview" && (
              <OverviewPanel
                workspaces={workspaces}
                integrations={integrations}
                pendingIntegrations={pendingIntegrations}
                activeWorkspace={activeWorkspace}
                onTabChange={setActiveTab}
                onCreateWorkspace={() => setCreateDialogOpen(true)}
              />
            )}
            {activeTab === "workspaces" && (
              <WorkspacesPanel
                workspaces={workspaces}
                activeWorkspace={activeWorkspace}
                onSwitchWorkspace={switchWorkspace}
                onCreateWorkspace={() => setCreateDialogOpen(true)}
                onDeleteWorkspace={deleteWorkspace}
                onSetDefault={setDefaultWorkspace}
              />
            )}
            {activeTab === "integrations" && (
              <IntegrationsPanel
                integrations={integrations}
                pendingIntegrations={pendingIntegrations}
                onRefresh={refresh}
              />
            )}
            {activeTab === "ai" && <AiPlaceholder />}
          </div>
        </main>
      </div>

      <CreateWorkspaceDialog
        open={createDialogOpen}
        onClose={() => setCreateDialogOpen(false)}
        onCreate={createWorkspace}
      />
    </div>
  );
};


export default Dashboard;
