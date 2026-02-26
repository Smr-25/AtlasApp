import { useEffect } from 'react'
import { useNavigate, useLocation } from 'react-router-dom'
import api from '@/lib/apiClient'
import { useToast } from '@/hooks/use-toast'
import { useAuth } from '@/context/AuthContext'
import { formatApiError } from '@/lib/errorUtils'

export default function GitHubCallback() {
  const navigate = useNavigate()
  const location = useLocation()
  const { toast } = useToast()
  const { finalizeAuthFromTokens } = useAuth()

  useEffect(() => {
    const params = new URLSearchParams(location.search)
    const code = params.get('code')
    const state = params.get('state')
    if (!code) {
      toast({ title: 'GitHub login failed', description: 'Missing authorization code' })
      navigate('/login')
      return
    }

    ;(async () => {
      try {
        // Send authorization code to backend to exchange for tokens
        const res = await api.accounts.externalLogin({ Provider: 'github', AuthorizationCode: code, IdToken: 'github-flow' })
        // Expect backend to return AccessToken/RefreshToken
        if (res?.AccessToken && res?.RefreshToken) {
          await finalizeAuthFromTokens({ AccessToken: res.AccessToken, RefreshToken: res.RefreshToken })
          // If backend indicates new user, it may return IsNewUser in response; apiClient currently returns the envelope data as res
          const isNew = (res as any).IsNewUser || (res as any).isNewUser || false
          if (isNew) navigate('/onboarding')
          else navigate('/')
          toast({ title: 'Signed in', description: 'Signed in with GitHub' })
          return
        }

        toast({ title: 'GitHub login failed', description: 'Invalid token response from server' })
        navigate('/login')
      } catch (e) {
        const formatted = formatApiError(e, 'GitHub sign in failed')
        toast({ title: formatted.title, description: formatted.message })
        navigate('/login')
      }
    })()
  }, [location.search])

  return (
    <div className="min-h-screen flex items-center justify-center">
      <div className="text-center">
        <h2 className="text-lg font-semibold">Signing you in with GitHub…</h2>
        <p className="text-sm text-muted-foreground mt-2">If you are not redirected automatically, please close this window and continue in the app.</p>
      </div>
    </div>
  )
}

