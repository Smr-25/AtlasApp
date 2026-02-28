import React from 'react'
import { render, screen, fireEvent, within } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import LeaderDashboard from '@/pages/leader/LeaderDashboard'

describe('Integrations modal', () => {
  it('opens integrations modal when button clicked', async () => {
    render(
      <MemoryRouter>
        <LeaderDashboard />
      </MemoryRouter>
    )
    const btn = screen.getByRole('button', { name: /Open Integrations/i })
    fireEvent.click(btn)
    const dialog = screen.getByRole('dialog')
    expect(dialog).toBeInTheDocument()
    // inside modal we should see a unique integration entry
    expect(within(dialog).getByText(/GitHub - Atlas/i)).toBeInTheDocument()
  })
})
