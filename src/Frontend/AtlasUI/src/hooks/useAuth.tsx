import React from 'react';
import { postJson } from '@/lib/api';
import { setTokens, getAccessToken } from '@/lib/api';
import { logout as apiLogout } from '@/lib/api';

type User = { id?: string; userName?: string; email?: string; fullName?: string } | null;

type LoginDto = { email?: string; userName?: string; password: string };

const AuthContext = React.createContext<{
  user: User;
  ready: boolean;
  login: (dto: LoginDto) => Promise<void>;
  logout: () => Promise<void>;
} | null>(null);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = React.useState<User>(() => {
    try {
      const raw = localStorage.getItem('atlas_user');
      return raw ? JSON.parse(raw) : null;
    } catch (e) {
      return null;
    }
  });
  const [ready, setReady] = React.useState(true);

  const login = React.useCallback(async (dto: LoginDto) => {
    // expects backend to return AuthResponseDto inside data
    const data = await postJson<any>('/api/accounts/login', dto);
    // data may contain accessToken, refreshToken and user info
    if (data) {
      setTokens({
        accessToken: data.accessToken,
        refreshToken: data.refreshToken,
        accessTokenExpiration: data.accessTokenExpiration,
        refreshTokenExpiration: data.refreshTokenExpiration,
      });
      const u = {
        id: data.userId ?? data.id,
        userName: data.userName ?? data.user?.userName ?? undefined,
        email: data.email ?? data.user?.email ?? undefined,
        fullName: data.fullName ?? data.user?.fullName ?? undefined,
      };
      localStorage.setItem('atlas_user', JSON.stringify(u));
      setUser(u);
    }
  }, []);

  const logout = React.useCallback(async () => {
    await apiLogout();
    localStorage.removeItem('atlas_user');
    setUser(null);
  }, []);

  const value = React.useMemo(() => ({ user, ready, login, logout }), [user, ready, login, logout]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = React.useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}

