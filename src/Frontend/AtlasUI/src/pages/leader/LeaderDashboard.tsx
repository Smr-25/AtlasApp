import { useEffect, useState } from 'react'
import api from '@/lib/apiClient'
import { useToast } from '@/hooks/use-toast'
import { useNavigate } from 'react-router-dom'

export default function LeaderDashboard() {
  const [teams, setTeams] = useState<any[]>([])
  const [bottlenecks, setBottlenecks] = useState<any[]>([])
  const [unassigned, setUnassigned] = useState<any[]>([])
  const [loading, setLoading] = useState(false)
  const { toast } = useToast()
  const navigate = useNavigate()

  useEffect(() => {
    let mounted = true
    const load = async () => {
      setLoading(true)
      try {
        const t = await api.teams.my()
        if (mounted) setTeams(t || [])
      } catch (e) {
        // ignore
      }

      try {
        if (teams && teams.length) {
          const firstId = teams[0]?.Id || teams[0]?.id
          if (firstId) {
            const b = await api.leaderagents.bottleneck(firstId)
            if (mounted) setBottlenecks(b?.Members || [])
            const u = await api.leaderagents.unassignedBugs(firstId)
            if (mounted) setUnassigned(u?.Bugs || [])
          }
        }
      } catch (e) {
        // ignore
      }

      setLoading(false)
    }
    load()
    return () => { mounted = false }
  }, [toast, teams.length])

  return (
    <div className="p-6">
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-2xl font-semibold">Team Leader Dashboard</h2>
        <div>
          <button className="btn btn-primary" onClick={() => navigate('/teams')}>Manage Teams</button>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
        <div className="p-4 rounded-lg bg-white border shadow-sm">
          <div className="text-sm text-muted-foreground">Your teams</div>
          <div className="text-2xl font-semibold mt-2">{teams.length}</div>
          <div className="mt-3">
            {teams.slice(0,3).map(t => <div key={t.Id || t.id} className="text-sm">{t.Name || t.name}</div>)}
          </div>
        </div>

        <div className="p-4 rounded-lg bg-white border shadow-sm">
          <div className="text-sm text-muted-foreground">Bottlenecks</div>
          <div className="text-2xl font-semibold mt-2">{bottlenecks.length}</div>
          <div className="mt-3 space-y-2">
            {bottlenecks.slice(0,3).map((b,i) => (
              <div key={i} className="text-sm">{b.MemberName} — {b.TaskKey} ({b.DaysStuck}d)</div>
            ))}
          </div>
        </div>

        <div className="p-4 rounded-lg bg-white border shadow-sm">
          <div className="text-sm text-muted-foreground">Unassigned bugs</div>
          <div className="text-2xl font-semibold mt-2">{unassigned.length}</div>
          <div className="mt-3 space-y-2">
            {unassigned.slice(0,3).map((u,i) => (
              <div key={i} className="text-sm">{u.IssueKey} — {u.Title}</div>
            ))}
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
        <div className="p-4 rounded-lg bg-white border shadow-sm">
          <h3 className="font-semibold mb-2">Top Recommendations</h3>
          {bottlenecks.length === 0 && <div className="text-sm text-muted-foreground">No recommendations yet.</div>}
          {bottlenecks.map((b,i) => (
            <div key={i} className="mb-3">
              <div className="text-sm font-medium">{b.MemberName} — {b.TaskKey}</div>
              <div className="text-xs text-muted-foreground">{b.Recommendation}</div>
            </div>
          ))}
        </div>

        <div className="p-4 rounded-lg bg-white border shadow-sm">
          <h3 className="font-semibold mb-2">Unassigned Bugs</h3>
          {unassigned.length === 0 && <div className="text-sm text-muted-foreground">No unassigned bugs.</div>}
          {unassigned.map((u,i) => (
            <div key={i} className="mb-3">
              <div className="text-sm font-medium">{u.IssueKey} — {u.Title}</div>
              <div className="text-xs text-muted-foreground">Severity: {u.Severity} • Reported: {u.ReportedAt}</div>
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}

