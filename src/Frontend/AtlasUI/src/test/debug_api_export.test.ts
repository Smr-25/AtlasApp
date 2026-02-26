import { it } from 'vitest'
import { onboarding } from '../lib/apiClient'

it('debug onboarding export', () => {
  // This test logs the onboarding value to inspect client exports during tests
  // eslint-disable-next-line no-console
  console.log('DEBUG onboarding export:', typeof onboarding, Object.keys(onboarding || {}))
})

