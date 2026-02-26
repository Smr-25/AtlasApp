import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest'
import { accounts, getTokens, setTokens, clearTokens, ApiError } from '../lib/apiClient'

const TEST_BASE = 'http://localhost:5075'

describe('accounts API client', () => {
  let fetchMock: any
  beforeEach(() => {
    // reset localStorage spies
    vi.restoreAllMocks()
    clearTokens()
    fetchMock = vi.fn()
    ;(globalThis as any).fetch = fetchMock
    // avoid spying on built-in non-configurable methods; we'll check via getTokens()
    // vi.spyOn(localStorage, 'setItem')
    // vi.spyOn(localStorage, 'getItem')
    // vi.spyOn(localStorage, 'removeItem')
  })

  afterEach(() => {
    vi.resetAllMocks()
    clearTokens()
  })

  it('login: should return tokens and allow storing them', async () => {
    const fakeAuth = {
      AccessToken: 'access-xyz',
      RefreshToken: 'refresh-xyz',
      AccessTokenExpiration: '2026-02-25T12:34:56Z',
      RefreshTokenExpiration: '2026-03-25T12:34:56Z',
      UserId: 'user-1',
      UserName: 'aygun',
      Email: 'aygun@example.com',
      FullName: 'Aygun',
    }

    fetchMock.mockResolvedValue({
      status: 200,
      json: async () => ({ success: true, data: fakeAuth }),
    })

    const res = await accounts.login({ Email: 'aygun@example.com', Password: 'P@ssw0rd' })
    expect(res).toEqual(fakeAuth)

    // simulate UI storing tokens
    setTokens({ AccessToken: res.AccessToken, RefreshToken: res.RefreshToken })
    // assert tokens are persisted via getTokens helper instead of spying on localStorage methods
    const toks = getTokens()
    expect(toks.accessToken).toBe('access-xyz')
    expect(toks.refreshToken).toBe('refresh-xyz')
  })

  it('verifyEmail: should return true on success', async () => {
    fetchMock.mockResolvedValue({ status: 200, json: async () => ({ success: true, data: true }) })
    const ok = await accounts.verifyEmail({ Email: 'u@x.com', VerificationCode: '123456' })
    expect(ok).toBe(true)
  })

  it('register: should throw ApiError with validation messages on 400', async () => {
    fetchMock.mockResolvedValue({ status: 400, json: async () => ({ success: false, errors: ['UserName is required', 'Password too short'] }) })
    await expect(
      accounts.register({ FullName: 'A', UserName: 'a', Email: 'bad', Password: '123456' })
    ).rejects.toMatchObject({ status: 400, errors: ['UserName is required', 'Password too short'] })
  })

  it('refreshToken: should call refresh endpoint and return TokenDto when called directly', async () => {
    const tokenPayload = { AccessToken: 'new-a', RefreshToken: 'new-r', AccessTokenExpiration: 't', RefreshTokenExpiration: 't2' }
    fetchMock.mockResolvedValue({ status: 200, json: async () => ({ success: true, data: tokenPayload }) })

    const res = await accounts.refreshToken('oldrefresh')
    expect(res).toEqual(tokenPayload)
  })
})
