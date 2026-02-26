import { useEffect, useState } from 'react'
import api, { ApiError } from '@/lib/apiClient'
import { useToast } from '@/hooks/use-toast'
import { formatApiError } from '@/lib/errorUtils'

export default function ConnectModal({ provider, onClose, onConnected }: any) {
  const [link, setLink] = useState<string | null>(null)
  const [manualToken, setManualToken] = useState('')
  const [loading, setLoading] = useState(false)
  const { toast } = useToast()

  useEffect(() => {
    // Try to ask backend for a link to start OAuth flow; backend can return 302 or provide link via ?response=link
    const fetchLink = async () => {
      try {
        const url = `/api/accounts/external/${String(provider?.provider || provider).toLowerCase()}?response=link`
        const res = await fetch(url, { method: 'GET' })
        if (res.ok) {
          try {
            const json = await res.json()
            // Backend may return { success: true, data: 'https://github.com/...' } or plain URL
            if (json && json.data) setLink(json.data)
            else if (typeof json === 'string') setLink(json)
          } catch (e) {
            // ignore
          }
        }
      } catch (e) {
        // ignore
      }
    }
    fetchLink()
  }, [provider])

  const handleManual = async () => {
    if (!manualToken) return toast({ title: 'Token required', description: 'Paste the access token obtained from provider' })
    setLoading(true)
    try {
      await api.integrations.create({ Provider: provider?.provider || provider, Name: `${provider?.provider || provider} - manual`, AccessToken: manualToken })
      toast({ title: 'Connected', description: 'Integration connected' })
      onConnected && onConnected()
    } catch (e) {
      const fe = formatApiError(e, 'Connect failed')
      toast({ title: fe.title, description: fe.message })
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded w-full max-w-lg">
        <h3 className="text-lg font-medium mb-4">Connect {provider?.provider || provider}</h3>
        {link ? (
          <div className="mb-4">
            <p className="mb-2">Click the button below to start OAuth flow:</p>
            <a className="btn btn-primary" href={link as string}>Open OAuth page</a>
            <p className="text-sm text-gray-500 mt-2">After finishing the OAuth flow, you should be redirected back and the integration will be created automatically.</p>
          </div>
        ) : (
          <div className="mb-4">
            <p className="mb-2">If you already have an access token from the provider, paste it here to connect manually:</p>
            <textarea className="input w-full" value={manualToken} onChange={(e) => setManualToken(e.target.value)} rows={3} />
            <div className="text-sm text-gray-500 mt-2">This method will store the token encrypted on the server.</div>
          </div>
        )}

        <div className="flex justify-end space-x-2">
          <button className="btn" onClick={onClose}>Close</button>
          {!link && <button className="btn btn-primary" onClick={handleManual} disabled={loading}>{loading ? 'Connecting...' : 'Connect'}</button>}
        </div>
      </div>
    </div>
  )
}
