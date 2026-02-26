import React, { Suspense, useEffect, useState } from "react";
import { motion } from "framer-motion";
import { useNavigate } from 'react-router-dom'

import TopNav from "@/components/dashboard/TopNav";
import Sidebar from "@/components/dashboard/Sidebar";
import { useAuth } from "@/context/AuthContext";
import { useTheme } from "@/context/ThemeContext";
import api, { ApiError } from '@/lib/apiClient'
import { useToast } from '@/hooks/use-toast'
import LeaderDashboard from './leader/LeaderDashboard'

const UpdatesCard = React.lazy(() => import("@/components/dashboard/UpdatesCard"));
const HeroBanner = React.lazy(() => import("@/components/dashboard/HeroBanner"));
const CalendarWidget = React.lazy(() => import("@/components/dashboard/CalendarWidget"));
const StatsCards = React.lazy(() => import("@/components/dashboard/StatsCards"));
const HeatmapWidget = React.lazy(() => import("@/components/dashboard/HeatmapWidget"));

const DashboardContent = () => {
  const { user } = useAuth();
  // If team-leader, render leader dashboard
  if (user?.role === 'team-leader') return <LeaderDashboard />

  const { setRole } = useTheme();
  const navigate = useNavigate()
  const { toast } = useToast()

  const [workspacesCount, setWorkspacesCount] = useState<number | null>(null)
  const [integrationsCount, setIntegrationsCount] = useState<number | null>(null)
  const [pendingIntegrationsCount, setPendingIntegrationsCount] = useState<number | null>(null)

  useEffect(() => {
    if (user?.role) {
      setRole(user.role)
    }
  }, [user?.role, setRole])

  useEffect(() => {
    let mounted = true
    const load = async () => {
      try {
        const ws = await api.workspaces.list()
        if (mounted) setWorkspacesCount(Array.isArray(ws) ? ws.length : 0)
      } catch (e) {
        if (e instanceof ApiError) toast({ title: 'Workspaces failed', description: e.errors?.join(', ') || e.message })
        else toast({ title: 'Workspaces failed', description: 'Unknown error' })
      }

      try {
        const ints = await api.integrations.list()
        if (mounted) setIntegrationsCount(Array.isArray(ints) ? ints.length : 0)
      } catch (e) {
        if (e instanceof ApiError) toast({ title: 'Integrations failed', description: e.errors?.join(', ') || e.message })
        else toast({ title: 'Integrations failed', description: 'Unknown error' })
      }

      try {
        const pending = await api.integrations.listPending()
        if (mounted) setPendingIntegrationsCount(Array.isArray(pending) ? pending.length : 0)
      } catch (e) {
        // silent if no pending endpoint on backend
        if (e instanceof ApiError) {
          // not critical
        }
      }
    }

    load()
    return () => { mounted = false }
  }, [toast])

  const displayName = user?.fullName?.split(" ")[0] || user?.username || "User";

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
              Hi, {displayName}! <span className="font-normal text-muted-foreground">Here's your dashboard</span>
            </h2>
          </motion.div>

          {/* Summary widgets */}
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-6">
            <div className="p-4 rounded-lg bg-white border shadow-sm">
              <div className="text-sm text-muted-foreground">Workspaces</div>
              <div className="text-2xl font-semibold mt-2">{workspacesCount === null ? '—' : workspacesCount}</div>
              <div className="mt-3 flex items-center gap-2">
                <button className="btn btn-sm" onClick={() => navigate('/workspaces')}>Manage</button>
              </div>
            </div>

            <div className="p-4 rounded-lg bg-white border shadow-sm">
              <div className="text-sm text-muted-foreground">Integrations</div>
              <div className="text-2xl font-semibold mt-2">{integrationsCount === null ? '—' : integrationsCount}</div>
              <div className="mt-3 flex items-center gap-2">
                <button className="btn btn-sm" onClick={() => navigate('/integrations')}>Manage</button>
                <button className="btn btn-ghost btn-sm" onClick={() => navigate('/integrations')}>Pending: {pendingIntegrationsCount ?? 0}</button>
              </div>
            </div>

            <div className="p-4 rounded-lg bg-white border shadow-sm">
              <div className="text-sm text-muted-foreground">Quick Actions</div>
              <div className="mt-2 flex flex-col gap-2">
                <button className="btn btn-sm" onClick={() => navigate('/workspaces')}>Create Workspace</button>
                <button className="btn btn-sm" onClick={() => navigate('/integrations')}>Connect Integration</button>
              </div>
            </div>
          </div>

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
  )
}

const Landing = () => {
  const navigate = useNavigate()
  // Auto-redirect to login after 2s
  useEffect(() => {
    const t = setTimeout(() => navigate('/login'), 2000)
    return () => clearTimeout(t)
  }, [navigate])

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-b from-slate-50 to-white">
      <div className="max-w-2xl p-8 text-center">
        <h1 className="text-4xl font-bold mb-4 animate-fade-in-up">Welcome to Atlas</h1>
        <p className="text-muted-foreground mb-6">Sign in to see your dashboard with Workspaces and Integrations.</p>
        <div className="flex items-center justify-center gap-3">
          <button className="btn btn-primary" onClick={() => navigate('/login')}>Sign in</button>
          <button className="btn" onClick={() => navigate('/register')}>Create account</button>
        </div>
      </div>
    </div>
  )
}

const Index = () => {
  const { isAuthenticated } = useAuth()
  return isAuthenticated ? <DashboardContent /> : <Landing />
}

export default Index;
