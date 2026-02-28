import React from 'react'
import { useGoogleLogin } from '@react-oauth/google'
import { useNavigate } from 'react-router-dom'
import { useToast } from '@/hooks/use-toast'
import { useAuth } from '@/context/AuthContext'
import { formatApiError } from '@/lib/errorUtils'

const GoogleSignInButton: React.FC = () => {
  const navigate = useNavigate()
  const { toast } = useToast()
  const { externalLogin } = useAuth()

  const googleLogin = useGoogleLogin({
    onSuccess: async (tokenResponse) => {
      try {
        const idToken = (tokenResponse as any).credential || (tokenResponse as any).id_token || (tokenResponse as any).access_token
        if (!idToken) throw new Error('No id token returned by Google')
        const res = await externalLogin('google', idToken)
        if (res) {
          toast({ title: 'Signed in', description: 'Signed in with Google' })
          navigate('/dashboard')
        } else {
          toast({ title: 'External sign in failed', description: 'Could not sign in with Google' })
        }
      } catch (e) {
        const formatted = formatApiError(e, 'Google sign in failed')
        toast({ title: formatted.title, description: formatted.message })
      }
    },
    onError: (err) => {
      const msg = (err && (err as any).message) || 'Google sign in failed'
      toast({ title: 'Google sign in failed', description: String(msg) })
    },
  })

  return (
    <button
      type="button"
      onClick={() => googleLogin()}
      className="flex-1 h-11 rounded-xl border border-border flex items-center justify-center gap-2 text-sm text-foreground hover:bg-muted/50 transition-colors"
    >
      <svg className="w-4 h-4" viewBox="0 0 24 24">
        <path fill="#4285F4" d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92a5.06 5.06 0 0 1-2.2 3.32v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.1z" />
        <path fill="#34A853" d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" />
        <path fill="#FBBC05" d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z" />
        <path fill="#EA4335" d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" />
      </svg>
      Google
    </button>
  )
}

export default GoogleSignInButton
