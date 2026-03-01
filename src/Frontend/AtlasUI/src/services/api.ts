import axios, { AxiosError, InternalAxiosRequestConfig } from "axios";

// ─── Base Config ────────────────────────────────────────────────────
const API_BASE_URL = "/api";

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: { "Content-Type": "application/json" },
});

// ─── Token helpers ──────────────────────────────────────────────────
export const TokenService = {
  getAccessToken: () => localStorage.getItem("atlas_access_token"),
  getRefreshToken: () => localStorage.getItem("atlas_refresh_token"),
  getUserId: () => localStorage.getItem("atlas_user_id"),

  setTokens(data: {
    accessToken: string;
    refreshToken: string;
    accessTokenExpiration?: string;
    refreshTokenExpiration?: string;
    userId?: string;
  }) {
    localStorage.setItem("atlas_access_token", data.accessToken);
    localStorage.setItem("atlas_refresh_token", data.refreshToken);
    if (data.accessTokenExpiration)
      localStorage.setItem("atlas_access_exp", data.accessTokenExpiration);
    if (data.refreshTokenExpiration)
      localStorage.setItem("atlas_refresh_exp", data.refreshTokenExpiration);
    if (data.userId) localStorage.setItem("atlas_user_id", data.userId);
  },

  clear() {
    localStorage.removeItem("atlas_access_token");
    localStorage.removeItem("atlas_refresh_token");
    localStorage.removeItem("atlas_access_exp");
    localStorage.removeItem("atlas_refresh_exp");
    localStorage.removeItem("atlas_user_id");
    localStorage.removeItem("atlas_user");
  },

  saveUser(user: Record<string, unknown>) {
    localStorage.setItem("atlas_user", JSON.stringify(user));
  },

  getUser() {
    const raw = localStorage.getItem("atlas_user");
    return raw ? JSON.parse(raw) : null;
  },
};

// ─── Request interceptor — attach token ─────────────────────────────
api.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  const token = TokenService.getAccessToken();
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// ─── Response interceptor — auto refresh on 401 ────────────────────
let isRefreshing = false;
let failedQueue: Array<{
  resolve: (v: unknown) => void;
  reject: (e: unknown) => void;
}> = [];

const processQueue = (error: unknown, token: string | null = null) => {
  failedQueue.forEach((prom) => {
    if (error) prom.reject(error);
    else prom.resolve(token);
  });
  failedQueue = [];
};

api.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as InternalAxiosRequestConfig & {
      _retry?: boolean;
    };

    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        }).then((token) => {
          if (originalRequest.headers) {
            originalRequest.headers.Authorization = `Bearer ${token}`;
          }
          return api(originalRequest);
        });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      const refreshToken = TokenService.getRefreshToken();
      if (!refreshToken) {
        TokenService.clear();
        window.location.href = "/login";
        return Promise.reject(error);
      }

      try {
        const { data } = await axios.post(`${API_BASE_URL}/accounts/refresh-token`, {
          refreshToken,
        });

        if (data.isSuccess && data.data) {
          TokenService.setTokens(data.data);
          processQueue(null, data.data.accessToken);
          if (originalRequest.headers) {
            originalRequest.headers.Authorization = `Bearer ${data.data.accessToken}`;
          }
          return api(originalRequest);
        } else {
          processQueue(error, null);
          TokenService.clear();
          window.location.href = "/login";
        }
      } catch (refreshError) {
        processQueue(refreshError, null);
        TokenService.clear();
        window.location.href = "/login";
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(error);
  }
);

// ─── Response helper types ──────────────────────────────────────────
export interface ApiResponse<T> {
  data: T;
  isSuccess: boolean;
  errors: string[] | null;
}

export interface AuthResponseDto {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiration: string;
  refreshTokenExpiration: string;
  userId: string;
  userName: string;
  email: string;
  fullName: string;
}

export interface ExternalLoginResponseDto {
  accessToken: string;
  refreshToken: string;
  refreshTokenExpiration: string;
  isNewUser: boolean;
  userId: string;
  email: string;
  fullName: string;
}

export interface AccountDto {
  id: string;
  userName: string | null;
  email: string;
  fullName: string;
  phoneNumber?: string | null;
  emailConfirmed: boolean;
  phoneNumberConfirmed: boolean;
  createdAt: string;
  status: "PendingVerification" | "Active" | "Suspended" | "Deactivated";
  lastLoginAt?: string | null;
  bio?: string | null;
  tags?: string[] | null;
  profession?: number | null;
}

export interface VerifyResetCodeResponseDto {
  resetToken: string;
  expiresAt: string;
}

// ─── Auth API ───────────────────────────────────────────────────────
export const authApi = {
  register(body: {
    fullName: string;
    userName: string;
    email: string;
    password: string;
  }) {
    return api.post<ApiResponse<AuthResponseDto>>("/accounts/register", body);
  },

  login(body: {
    email?: string | null;
    userName?: string | null;
    password: string;
  }) {
    return api.post<ApiResponse<AuthResponseDto>>("/accounts/login", body);
  },

  externalLogin(body: {
    provider: string;
    idToken: string;
    accessToken: string;
    authorizationCode: string;
  }) {
    return api.post<ApiResponse<ExternalLoginResponseDto>>(
      "/accounts/external-login",
      body
    );
  },

  logout(refreshToken: string) {
    return api.post("/accounts/logout", { refreshToken });
  },

  forgotPassword(email: string) {
    return api.post<ApiResponse<null>>("/accounts/forgot-password", { email });
  },

  verifyResetCode(email: string, code: string) {
    return api.post<ApiResponse<VerifyResetCodeResponseDto>>("/accounts/verify-reset-code", {
      email,
      verificationCode: code,
    });
  },

  resetPassword(email: string, resetToken: string, newPassword: string, confirmPassword: string) {
    return api.post<ApiResponse<null>>("/accounts/reset-password", {
      email,
      resetToken,
      newPassword,
      confirmPassword,
    });
  },

  verifyEmail(email: string, code: string) {
    return api.post<ApiResponse<null>>("/accounts/verify-email", {
      email,
      verificationCode: code,
    });
  },

  verifyPhone(phoneNumber: string, code: string) {
    return api.post<ApiResponse<null>>("/accounts/verify-phone", {
      phoneNumber,
      verificationCode: code,
    });
  },

  resendEmailVerificationCode(email: string) {
    return api.post<ApiResponse<null>>(
      "/accounts/resend-email-verification-code",
      { email }
    );
  },

  resendPhoneVerificationCode(phoneNumber: string) {
    return api.post<ApiResponse<null>>(
      "/accounts/resend-phone-verification-code",
      { phoneNumber }
    );
  },

  refreshToken(refreshToken: string) {
    return api.post<ApiResponse<AuthResponseDto>>("/accounts/refresh-token", {
      refreshToken,
    });
  },

  revokeRefreshToken(refreshToken: string) {
    return api.post("/accounts/revoke-refresh-token", { refreshToken });
  },

  getProfile() {
    return api.get<ApiResponse<AccountDto>>("/accounts/profile");
  },

  updateProfile(body: { fullName: string; userName: string }) {
    return api.put<ApiResponse<null>>("/accounts/profile", body);
  },

  changePassword(body: { currentPassword: string; newPassword: string }) {
    return api.put<ApiResponse<null>>("/accounts/change-password", body);
  },

  addPhoneNumber(phoneNumber: string) {
    return api.post<ApiResponse<null>>("/accounts/add-phone-number", {
      phoneNumber,
    });
  },

  deleteAccount() {
    return api.delete<ApiResponse<null>>("/accounts/delete-account");
  },

  setTelegramChatId(body: { linkCode: string; chatId: string }) {
    return api.post<ApiResponse<null>>("/accounts/set-telegram-chat-id", body);
  },

  generateTelegramLinkCode() {
    return api.post<
      ApiResponse<{ linkCode: string; expiresAt: string }>
    >("/accounts/generate-telegram-link-code");
  },
};

// ─── Onboarding API ─────────────────────────────────────────────────
export const onboardingApi = {
  complete(body: {
    profession: number;
    jobTitle: string;
    answers: Array<{
      questionId: string;
      optionId: string;
      customValue?: string;
    }>;
  }) {
    return api.post<ApiResponse<{ profileId: string }>>(
      "/onboarding/complete",
      body
    );
  },
};

// ─── Workspace & Integration Types ──────────────────────────────────
export interface WorkspaceIntegrationDto {
  integrationId: string;
  integrationName: string;
  provider: string;
  enabled: boolean;
  connectedAt?: string;
}

export interface WorkspaceDto {
  id: string;
  name: string;
  description?: string | null;
  isDefault: boolean;
  localFolderPath?: string | null;
  isShared?: boolean;
  activeIntegrations?: WorkspaceIntegrationDto[];
}

export interface IntegrationDto {
  id: string;
  name: string;
  provider: string;
  status: "PendingSetup" | "Active" | "Disconnected" | "Expired" | "Error";
  metadataJson?: string | null;
}

export interface FolderValidationDto {
  exists: boolean;
  path: string;
  sizeInBytes: number;
  subFolderCount: number;
  fileCount: number;
}

// ─── Workspace API ──────────────────────────────────────────────────
export const workspaceApi = {
  getAll() {
    return api.get<ApiResponse<WorkspaceDto[]>>("/workspaces");
  },
  getById(id: string) {
    return api.get<ApiResponse<WorkspaceDto>>(`/workspaces/${id}`);
  },
  create(body: { name: string; description?: string | null; localFolderPath?: string | null }) {
    return api.post<ApiResponse<string>>("/workspaces", body);
  },
  update(id: string, body: { workspaceId: string; name: string; description?: string | null; localFolderPath?: string | null }) {
    return api.put<ApiResponse<null>>(`/workspaces/${id}`, body);
  },
  remove(id: string) {
    return api.delete<ApiResponse<null>>(`/workspaces/${id}`);
  },
  setDefault(id: string) {
    return api.patch<ApiResponse<null>>(`/workspaces/${id}/set-default`);
  },
  toggleIntegration(workspaceId: string, integrationId: string, enable: boolean) {
    return api.post<ApiResponse<null>>(`/workspaces/${workspaceId}/integrations/toggle`, {
      integrationId,
      enable,
    });
  },
  validateFolder(folderPath: string) {
    return api.post<ApiResponse<FolderValidationDto>>("/workspaces/validate-folder", { folderPath });
  },
};

// ─── Integration API ────────────────────────────────────────────────
export const integrationApi = {
  getAll() {
    return api.get<ApiResponse<IntegrationDto[]>>("/integrations");
  },
  getPending() {
    return api.get<ApiResponse<IntegrationDto[]>>("/integrations/pending");
  },
  getById(id: string) {
    return api.get<ApiResponse<IntegrationDto>>(`/integrations/${id}`);
  },
  create(body: {
    provider: string;
    name: string;
    accessToken: string;
    refreshToken?: string | null;
    expiresAt?: string | null;
    metadataJson?: string | null;
  }) {
    return api.post<ApiResponse<IntegrationDto>>("/integrations", body);
  },
  update(id: string, name: string) {
    return api.put<ApiResponse<null>>(`/integrations/${id}`, { integrationId: id, name });
  },
  remove(id: string) {
    return api.delete<ApiResponse<null>>(`/integrations/${id}`);
  },
  reconnect(id: string, body: {
    integrationId: string;
    accessToken: string;
    refreshToken?: string | null;
    expiresAt?: string | null;
    metadataJson?: string | null;
  }) {
    return api.post<ApiResponse<null>>(`/integrations/${id}/reconnect`, body);
  },
  markExpired(id: string) {
    return api.post<ApiResponse<null>>(`/integrations/${id}/mark-expired`);
  },
};

export default api;

