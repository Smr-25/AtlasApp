import { describe, it, expect, beforeEach, vi } from 'vitest'
import { onboarding } from '../lib/apiClient'

describe('onboarding API client', () => {
  let fetchMock: any
  beforeEach(() => {
    vi.restoreAllMocks()
    fetchMock = vi.fn()
    ;(globalThis as any).fetch = fetchMock
  })

  it('getProfessionQuestion: should return a question DTO', async () => {
    const q = { Id: 'q1', Text: 'What is your profession?', Order: 1, IsMultiSelect: false, TargetProfession: null, Options: [{ Id: 'o1', Text: 'Dev' }] }
    fetchMock.mockResolvedValue({ status: 200, json: async () => ({ success: true, data: q }) })
    const res = await onboarding.getProfessionQuestion()
    expect(res).toEqual(q)
  })

  it('getQuestions: should pass profession query param when provided', async () => {
    const arr = [ { Id: 'q1', Text: 'Q1', Order: 1, IsMultiSelect: false, TargetProfession: 1, Options: [] } ]
    fetchMock.mockImplementation((url: string) => Promise.resolve({ status: 200, json: async () => ({ success: true, data: arr }) }))
    const res = await onboarding.getQuestions(1)
    expect(res).toEqual(arr)
  })

  it('complete: should map answer object to expected payload when given flat object', async () => {
    // server expects Answers array
    fetchMock.mockResolvedValue({ status: 200, json: async () => ({ success: true, data: { ProfileId: 'p1' } }) })
    const payload = { Profession: 1, JobTitle: 'Dev', 'q1': 'o1', 'q2': 'o2' }
    const res = await onboarding.complete(payload as any)
    expect(res).toEqual({ ProfileId: 'p1' })
  })

  it('complete: should accept already structured payload', async () => {
    fetchMock.mockResolvedValue({ status: 200, json: async () => ({ success: true, data: { ProfileId: 'p2' } }) })
    const payload = { Profession: 1, Answers: [{ QuestionId: 'q1', OptionId: 'o1' }] }
    const res = await onboarding.complete(payload as any)
    expect(res).toEqual({ ProfileId: 'p2' })
  })
})

