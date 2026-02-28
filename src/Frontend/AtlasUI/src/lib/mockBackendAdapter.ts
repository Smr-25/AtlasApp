// Simple in-memory mock adapter for dev/offline mode. Not full MSW but minimal fetch interception.
// It monkeypatches window.fetch when run. Use only in development.

interface MockUser {
  id: string
  fullName: string
  email: string
  role: string
}

const mockUsers: Record<string, MockUser> = {
  'alice@dev': { id: 'u-1', fullName: 'Alice Dev', email: 'alice@dev', role: 'developer' },
  'bob@design': { id: 'u-2', fullName: 'Bob Designer', email: 'bob@design', role: 'designer' },
  'carol@sec': { id: 'u-3', fullName: 'Carol Sec', email: 'carol@sec', role: 'cybersecurity' },
  'dave@market': { id: 'u-4', fullName: 'Dave Market', email: 'dave@market', role: 'marketer' },
  'ellen@lead': { id: 'u-5', fullName: 'Ellen Lead', email: 'ellen@lead', role: 'team-leader' },
}

let storedRefreshToken = 'refresh-token-1'
let storedAccessToken = 'access-token-1'

function jsonResponse(body: any, status = 200) {
  return Promise.resolve(new Response(JSON.stringify(body), { status, headers: { 'Content-Type': 'application/json' } }))
}

function parseBody(req: Request) {
  return req.text().then(t => {
    try { return JSON.parse(t || '{}') } catch { return {} }
  })
}

export function enableMockBackend() {
  if (typeof window === 'undefined') return
  if ((window as any).__mockBackendEnabled) return
  (window as any).__mockBackendEnabled = true

  const originalFetch = window.fetch.bind(window)

  window.fetch = async (input: RequestInfo, init?: RequestInit) => {
    try {
      const url = typeof input === 'string' ? input : input.url
      const path = url.replace(window.location.origin, '')
      // Only intercept API calls to /api
      if (!path.startsWith('/api') && !path.includes('/api/')) return originalFetch(input, init)

      const req = new Request(input as any, init)
      // simulate latency
      await new Promise(r => setTimeout(r, 200))

      // POST /api/accounts/login
      if (path.endsWith('/api/accounts/login') && req.method === 'POST') {
        const body = await parseBody(req)
        const identifier = body.Email || body.UserName
        const user = mockUsers[identifier]
        if (!user || body.Password !== 'password') {
          return jsonResponse({ success: false, errors: ['Invalid credentials'] }, 401)
        }
        storedAccessToken = `access-${user.id}-${Date.now()}`
        storedRefreshToken = `refresh-${user.id}-${Date.now()}`
        return jsonResponse({ AccessToken: storedAccessToken, RefreshToken: storedRefreshToken, UserId: user.id, UserName: user.email, Email: user.email, FullName: user.fullName })
      }

      if (path.endsWith('/api/accounts/refresh-token') && req.method === 'POST') {
        const body = await parseBody(req)
        if (!body.RefreshToken || body.RefreshToken !== storedRefreshToken) {
          return jsonResponse({ success: false, errors: ['Invalid refresh token'] }, 401)
        }
        storedAccessToken = `access-refreshed-${Date.now()}`
        storedRefreshToken = `refresh-refreshed-${Date.now()}`
        return jsonResponse({ AccessToken: storedAccessToken, RefreshToken: storedRefreshToken })
      }

      if (path.endsWith('/api/accounts/profile') && req.method === 'GET') {
        const auth = req.headers.get('authorization') || ''
        if (!auth.includes(storedAccessToken)) return jsonResponse({ success: false, errors: ['Unauthorized'] }, 401)
        // find user by token
        const uidMatch = storedAccessToken.split('-')[1]
        const user = Object.values(mockUsers).find(u => storedAccessToken.includes(u.id)) || Object.values(mockUsers)[0]
        return jsonResponse({ FullName: user.fullName, UserName: user.email, Email: user.email, EmailConfirmed: true, PhoneNumber: null, Profession: user.role })
      }

      // GET /api/dashboard/widgets
      if (path.endsWith('/api/dashboard/widgets') && req.method === 'GET') {
        return jsonResponse({ success: true, data: [
          { id: 'w-1', displayName: 'Focus Timer', endpoint: '/api/widgets/focus', requiredRoles: [] },
          { id: 'w-2', displayName: 'GitHub PRs', endpoint: '/api/widgets/github-prs', requiredRoles: ['developer'] },
          { id: 'w-3', displayName: 'Figma Recent', endpoint: '/api/widgets/figma', requiredRoles: ['designer'] },
        ] })
      }

      if (path.startsWith('/api/widgets/') && req.method === 'GET') {
        const parts = path.split('/')
        const id = parts[parts.length - 1]
        if (id === 'focus') return jsonResponse({ success: true, data: { remaining: 15, sessionsToday: 3 } })
        if (id === 'github-prs') return jsonResponse({ success: true, data: { open: 5, reviewsRequested: 2 } })
        if (id === 'figma') return jsonResponse({ success: true, data: { recent: [{ name: 'Homepage' }, { name: 'Dashboard' }] } })
        return jsonResponse({ success: false, errors: ['Unknown widget'] }, 404)
      }

      // fallback to original fetch for anything else
      return originalFetch(input, init)
    } catch (e) {
      return Promise.reject(e)
    }
  }
}

export function disableMockBackend() {
  if (typeof window === 'undefined') return
  const w = window as any
  if (!w.__mockBackendEnabled) return
  if (w.__originalFetch) window.fetch = w.__originalFetch
  w.__mockBackendEnabled = false
}

