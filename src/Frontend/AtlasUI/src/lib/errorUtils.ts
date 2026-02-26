import { ApiError } from '@/lib/apiClient'

type FormattedError = { title: string; message: string }

function joinErrors(arr: any[]): string {
  return arr
    .map((x) => {
      if (!x) return ''
      if (typeof x === 'string') return x
      // If server returned field-specific errors like { field: ['msg'] }
      if (typeof x === 'object') {
        try {
          return Object.entries(x)
            .map(([k, v]) => `${k}: ${Array.isArray(v) ? v.join(', ') : String(v)}`)
            .join('; ')
        } catch {
          return JSON.stringify(x)
        }
      }
      return String(x)
    })
    .filter(Boolean)
    .join('; ')
}

export function formatApiError(e: any, fallbackTitle = 'Error'): FormattedError {
  try {
    if (!e) return { title: fallbackTitle, message: 'Unknown error' }

    // If it's our ApiError
    if (e instanceof ApiError) {
      const errs = e.errors && e.errors.length ? joinErrors(e.errors) : e.message || String(e)
      const lower = errs.toLowerCase()
      if (lower.includes('email not verified') || lower.includes('email not confirmed')) {
        return { title: 'Email not verified', message: 'Your email address is not verified. Please check your inbox for a verification code or resend it.' }
      }
      if (lower.includes('invalid credentials') || lower.includes('invalid email') || lower.includes('invalid username') || lower.includes('incorrect password')) {
        return { title: 'Invalid credentials', message: 'Invalid email/username or password. Please try again.' }
      }
      if (lower.includes('locked') || e.status === 423) {
        return { title: 'Account locked', message: errs }
      }
      return { title: fallbackTitle, message: errs }
    }

    // If it's a plain Response-like or object, try to stringify helpful parts
    if (typeof e === 'object') {
      if (Array.isArray(e)) return { title: fallbackTitle, message: joinErrors(e) }
      if (e.errors) return { title: fallbackTitle, message: Array.isArray(e.errors) ? joinErrors(e.errors) : String(e.errors) }
      if (e.message) return { title: fallbackTitle, message: String(e.message) }
      // If it's a string-like object with nested data
      try {
        return { title: fallbackTitle, message: JSON.stringify(e) }
      } catch {
        return { title: fallbackTitle, message: String(e) }
      }
    }

    return { title: fallbackTitle, message: String(e) }
  } catch (err) {
    return { title: fallbackTitle, message: String(err) }
  }
}
