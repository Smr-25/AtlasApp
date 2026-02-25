import { useState, useEffect, useRef } from "react";
import { accounts } from "@/lib/api";

interface AuthState {
  accessToken?: string | null;
  refreshToken?: string | null;
  accessTokenExpiration?: string | null;
  refreshTokenExpiration?: string | null;
  user?: any;
}

export function useAuth() {
  const [state, setState] = useState<AuthState>({});
  const inMemoryRef = useRef<AuthState>(state);

  useEffect(() => {
    inMemoryRef.current = state;
  }, [state]);

  const setAuth = (s: AuthState) => setState((_) => ({ ...s }));

  const getAuthHeader = () => {
    const token = inMemoryRef.current?.accessToken;
    return token ? { Authorization: `Bearer ${token}` } : {};
  };

  const login = async (payload: { Email?: string | null; UserName?: string | null; Password: string }) => {
    const data = await accounts.login(payload);
    const newState: AuthState = {
      accessToken: data.AccessToken,
      refreshToken: data.RefreshToken,
      accessTokenExpiration: data.AccessTokenExpiration,
      refreshTokenExpiration: data.RefreshTokenExpiration,
      user: { id: data.UserId, userName: data.UserName, email: data.Email, fullName: data.FullName },
    };
    setAuth(newState);
    return data;
  };

  const register = async (payload: any) => {
    return accounts.register(payload);
  };

  const externalLogin = async (provider: 'google' | 'apple' | 'github', idToken: string, accessToken?: string | null, authorizationCode?: string | null) => {
    const data = await accounts.externalLogin({ Provider: provider, IdToken: idToken, AccessToken: accessToken ?? null, AuthorizationCode: authorizationCode ?? null });
    // If backend returns tokens, save them
    if (data?.AccessToken) {
      setAuth({
        accessToken: data.AccessToken,
        refreshToken: data.RefreshToken,
        accessTokenExpiration: data.AccessTokenExpiration,
        refreshTokenExpiration: data.RefreshTokenExpiration,
        user: { id: data.UserId, email: data.Email, fullName: data.FullName },
      });
    }
    return data;
  };

  const refresh = async () => {
    if (!inMemoryRef.current?.refreshToken) throw new Error('No refresh token');
    const data = await accounts.refreshToken({ RefreshToken: inMemoryRef.current.refreshToken });
    setAuth({
      ...inMemoryRef.current,
      accessToken: data.AccessToken,
      refreshToken: data.RefreshToken,
      accessTokenExpiration: data.AccessTokenExpiration,
      refreshTokenExpiration: data.RefreshTokenExpiration,
    });
    return data;
  };

  const revoke = async () => {
    try {
      await accounts.revokeRefreshToken(inMemoryRef.current?.accessToken);
    } catch {}
    setAuth({});
  };

  const logout = () => {
    setAuth({});
  };

  return { state, login, register, externalLogin, refresh, revoke, logout, setAuth, getAuthHeader };
}
