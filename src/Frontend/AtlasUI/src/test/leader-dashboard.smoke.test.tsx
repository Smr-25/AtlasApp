import React from 'react'
import { render, screen } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import LeaderDashboard from '@/pages/leader/LeaderDashboard'

describe('LeaderDashboard smoke', () => {
  it('renders the dashboard title and omni-feed', () => {
    render(
      <MemoryRouter>
        <LeaderDashboard />
      </MemoryRouter>
    )
    expect(screen.getByText(/Team Leader Dashboard/i)).toBeInTheDocument()
    // Omni-Feed should render (uses mock data)
    expect(screen.getByText(/Omni-Feed/i)).toBeInTheDocument()
  })
})
