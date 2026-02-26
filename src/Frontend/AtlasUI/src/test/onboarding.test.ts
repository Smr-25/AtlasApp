import {describe, it, expect, beforeEach, vi} from 'vitest'
import {onboarding} from '../lib/apiClient'

describe('onboarding API client', () => {
    let fetchMock: any
    beforeEach(() => {
        vi.restoreAllMocks()
        fetchMock = vi.fn()
        ;(globalThis as any).fetch = fetchMock
    })

    it('complete: should map answer object to expected payload when given flat object', async () => {
        // server expects Answers array
        fetchMock.mockResolvedValue({status: 200, json: async () => ({success: true, data: {ProfileId: 'p1'}})})
        const payload = {Profession: 1, JobTitle: 'Dev', 'q1': 'o1', 'q2': 'o2'}
        const res = await onboarding.complete(payload as any)
        expect(res).toEqual({ProfileId: 'p1'})
    })

    it('complete: should accept already structured payload', async () => {
        fetchMock.mockResolvedValue({status: 200, json: async () => ({success: true, data: {ProfileId: 'p2'}})})
        const payload = {Profession: 1, Answers: [{QuestionId: 'q1', OptionId: 'o1'}]}
        const res = await onboarding.complete(payload as any)
        expect(res).toEqual({ProfileId: 'p2'})
    })
})

