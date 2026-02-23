export class ApiError extends Error {
  public status?: number;
  public details?: any;
  constructor(message: string, status?: number, details?: any) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.details = details;
  }
}

type BackendResponse<T> = {
  success?: boolean;
  isSuccess?: boolean;
  data?: T | null;
  message?: string | null;
  errors?: any;
  Error?: any;
};

const BASE = (window as any).__ATLAS_API_BASE__ ?? 'http://localhost:5075';

const ACCESS_KEY = 'atlas_access_token';
const REFRESH_KEY = 'atlas_refresh_token';
const ACCESS_EXP_KEY = 'atlas_access_token_exp';
const REFRESH_EXP_KEY = 'atlas_refresh_token_exp';

export function getAccessToken() {
  return localStorage.getItem(ACCESS_KEY);
}
export function getRefreshToken() {
  return localStorage.getItem(REFRESH_KEY);
}
export function setTokens(tokens: { accessToken?: string; refreshToken?: string; accessTokenExpiration?: string; refreshTokenExpiration?: string }) {
  if (tokens.accessToken) localStorage.setItem(ACCESS_KEY, tokens.accessToken);
  if (tokens.refreshToken) localStorage.setItem(REFRESH_KEY, tokens.refreshToken);
  if (tokens.accessTokenExpiration) localStorage.setItem(ACCESS_EXP_KEY, tokens.accessTokenExpiration);
  if (tokens.refreshTokenExpiration) localStorage.setItem(REFRESH_EXP_KEY, tokens.refreshTokenExpiration);
}
export function clearTokens() {
  localStorage.removeItem(ACCESS_KEY);
  localStorage.removeItem(REFRESH_KEY);
  localStorage.removeItem(ACCESS_EXP_KEY);
  localStorage.removeItem(REFRESH_EXP_KEY);
  localStorage.removeItem('atlas_user_id');
  localStorage.removeItem('atlas_user_name');
  localStorage.removeItem('atlas_user_email');
  localStorage.removeItem('atlas_user_fullname');
}

async function refreshToken(): Promise<boolean> {
  const refresh = getRefreshToken();
  if (!refresh) return false;

  try {
    const res = await fetch(`${BASE}/api/accounts/refresh-token`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken: refresh }),
    });
    if (!res.ok) return false;
    const json = (await res.json()) as BackendResponse<any>;
    const successFlag = json?.success ?? json?.isSuccess ?? false;
    const data = json?.data ?? null;
    if (successFlag && data) {
      setTokens({
        accessToken: data.accessToken,
        refreshToken: data.refreshToken,
        accessTokenExpiration: data.accessTokenExpiration,
        refreshTokenExpiration: data.refreshTokenExpiration,
      });
      return true;
    }
    return false;
  } catch (e) {
    return false;
  }
}

async function revokeRefreshToken(): Promise<void> {
  try {
    await fetch(`${BASE}/api/accounts/revoke-refresh-token`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: '{}' });
  } catch (e) {}
}

export async function logout(): Promise<void> {
  const refresh = getRefreshToken();
  try {
    if (refresh) {
      await fetch(`${BASE}/api/accounts/logout`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ refreshToken: refresh }),
      });
    }
  } catch (e) {}
  try {
    await revokeRefreshToken();
  } catch (e) {}
  clearTokens();
}

export async function apiFetch(input: string, init: RequestInit = {}, retry = true): Promise<Response> {
  const headers = new Headers(init.headers || {});
  headers.set('Accept', 'application/json');
  if (!headers.has('Content-Type')) headers.set('Content-Type', 'application/json');

  const token = getAccessToken();
  if (token) headers.set('Authorization', `Bearer ${token}`);

  const res = await fetch(input.startsWith('http') ? input : `${BASE}${input.startsWith('/') ? '' : '/'}${input}`, { ...init, headers });

  if (res.status === 401 && retry) {
    const refreshed = await refreshToken();
    if (refreshed) {
      const token2 = getAccessToken();
      if (token2) headers.set('Authorization', `Bearer ${token2}`);
      const retryRes = await fetch(input.startsWith('http') ? input : `${BASE}${input.startsWith('/') ? '' : '/'}${input}`, { ...init, headers });
      if (retryRes.status === 401) {
        clearTokens();
      }
      return retryRes;
    }
    clearTokens();
  }

  return res;
}

export async function postJson<T = any>(path: string, body: any): Promise<T> {
  const res = await apiFetch(path.startsWith('/') ? path : `/${path}`, { method: 'POST', body: JSON.stringify(body) });
  let json: any | null = null;
  try { json = await res.json(); } catch (e) { throw new ApiError(res.statusText || 'Invalid JSON response', res.status); }
  if (!res.ok) {
    const msg = json?.message ?? json?.Message ?? JSON.stringify(json?.errors ?? json?.Errors ?? json?.Error ?? json);
    throw new ApiError(msg || res.statusText, res.status, json);
  }
  const successFlag = json?.success ?? json?.isSuccess ?? json?.IsSuccess ?? false;
  if (!successFlag) {
    const msg = json?.message ?? json?.Message ?? JSON.stringify(json?.errors ?? json?.Errors ?? json?.Error ?? json);
    throw new ApiError(msg || 'Request failed', res.status, json);
  }
  return (json?.data ?? json?.Data) as T;
}

export async function putJson<T = any>(path: string, body: any): Promise<T> {
  const res = await apiFetch(path.startsWith('/') ? path : `/${path}`, { method: 'PUT', body: JSON.stringify(body) });
  let json: any | null = null;
  try { json = await res.json(); } catch (e) { throw new ApiError(res.statusText || 'Invalid JSON response', res.status); }
  if (!res.ok) {
    const msg = json?.message ?? json?.Message ?? JSON.stringify(json?.errors ?? json?.Errors ?? json?.Error ?? json);
    throw new ApiError(msg || res.statusText, res.status, json);
  }
  const successFlag = json?.success ?? json?.isSuccess ?? json?.IsSuccess ?? false;
  if (!successFlag) {
    const msg = json?.message ?? json?.Message ?? JSON.stringify(json?.errors ?? json?.Errors ?? json?.Error ?? json);
    throw new ApiError(msg || 'Request failed', res.status, json);
  }
  return (json?.data ?? json?.Data) as T;
}

export async function getJson<T = any>(path: string): Promise<T> {
  const res = await apiFetch(path.startsWith('/') ? path : `/${path}`, { method: 'GET' });
  let json: any | null = null;
  try { json = await res.json(); } catch (e) { throw new ApiError(res.statusText || 'Invalid JSON response', res.status); }
  if (!res.ok) {
    const msg = json?.message ?? json?.Message ?? JSON.stringify(json?.errors ?? json?.Errors ?? json?.Error ?? json);
    throw new ApiError(msg || res.statusText, res.status, json);
  }
  const successFlag = json?.success ?? json?.isSuccess ?? json?.IsSuccess ?? false;
  if (!successFlag) {
    const msg = json?.message ?? json?.Message ?? JSON.stringify(json?.errors ?? json?.Errors ?? json?.Error ?? json);
    throw new ApiError(msg || 'Request failed', res.status, json);
  }
  return (json?.data ?? json?.Data) as T;
}
