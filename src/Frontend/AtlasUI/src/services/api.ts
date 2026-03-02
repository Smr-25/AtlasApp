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

// ─── Request interceptor — attach token + workspace ─────────────────
api.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  const token = TokenService.getAccessToken();
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  const wsId = localStorage.getItem("atlas_workspace_id");
  if (wsId && config.headers) {
    config.headers["X-Workspace-Id"] = wsId;
  }
  return config;
});

// ─── Global error toast callback ────────────────────────────────────
let globalErrorToast: ((msg: string) => void) | null = null;
export const setGlobalErrorToast = (fn: (msg: string) => void) => {
  globalErrorToast = fn;
};

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

    // Show global error toast for non-401 errors
    if (error.response?.status !== 401 && globalErrorToast) {
      const body = error.response?.data as { errors?: string[]; isSuccess?: boolean } | undefined;
      const msg = body?.errors?.[0] || error.message || "An unexpected error occurred";
      // Don't toast for validation errors (400) if they'll be handled by the form
      if (error.response?.status !== 400) {
        globalErrorToast(msg);
      }
    }

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
  role?: string;
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

  resendPhoneVerificationCode(phoneNumber: string, channel: "Sms" | "Telegram" = "Sms") {
    return api.post<ApiResponse<null>>(
      "/accounts/resend-phone-verification-code",
      { phoneNumber, channel }
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

  changePassword(body: { currentPassword: string; newPassword: string; confirmPassword: string }) {
    return api.put<ApiResponse<null>>("/accounts/change-password", body);
  },

  addPhoneNumber(phoneNumber: string, channel: "Sms" | "Telegram" = "Sms") {
    return api.post<ApiResponse<null>>("/accounts/add-phone-number", {
      phoneNumber,
      verificationChannel: channel,
    });
  },

  deleteAccount() {
    return api.delete<ApiResponse<null>>("/accounts/delete-account");
  },

  setTelegramChatId(body: { telegramChatId: string }) {
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
  scope?: string;
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
  myRole?: string;
  membersCount?: number;
  activeIntegrations?: WorkspaceIntegrationDto[];
}

export interface IntegrationDto {
  id: string;
  name: string;
  provider: string;
  status: "PendingSetup" | "Active" | "Disconnected" | "Expired" | "Error";
  scope?: string;
  metadataJson?: string | null;
}

export interface FolderValidationDto {
  exists: boolean;
  path: string;
  sizeInBytes: number;
  subFolderCount: number;
  fileCount: number;
}

export interface WorkspaceMemberDto {
  userId: string;
  userName: string;
  role: string;
  joinedAt: string;
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
  getMembers(id: string) {
    return api.get<ApiResponse<WorkspaceMemberDto[]>>(`/workspaces/${id}/members`);
  },
  addMember(id: string, body: { userId: string; role?: string }) {
    return api.post<ApiResponse<null>>(`/workspaces/${id}/members`, body);
  },
  removeMember(id: string, userId: string) {
    return api.delete<ApiResponse<null>>(`/workspaces/${id}/members/${userId}`);
  },
  changeMemberRole(id: string, userId: string, newRole: string) {
    return api.patch<ApiResponse<null>>(`/workspaces/${id}/members/${userId}/role`, { newRole });
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

// ─── Profile Types & API ────────────────────────────────────────────
export interface ProfileDto {
  id: string;
  profession: string;
  jobTitle: string;
  bio?: string | null;
  tags?: string[] | null;
}

export const profileApi = {
  getMe() { return api.get<ApiResponse<ProfileDto>>("/profiles/me"); },
  updateMe(body: { jobTitle?: string; bio?: string }) {
    return api.put<ApiResponse<ProfileDto>>("/profiles/me", body);
  },
};

// ─── Subscription Types & API ───────────────────────────────────────
export interface SubscriptionDto {
  tier: string;
  status: string;
  currentPeriodEnd?: string;
}
export interface UsageDto {
  workspacesUsed: number;
  workspacesLimit: number;
  integrationsUsed: number;
  integrationsLimit: number;
}

export const subscriptionApi = {
  getCurrent() { return api.get<ApiResponse<SubscriptionDto>>("/subscriptions/current"); },
  getUsage() { return api.get<ApiResponse<UsageDto>>("/subscriptions/usage"); },
  checkout(body: { tier: string; successUrl: string; cancelUrl: string }) {
    return api.post<ApiResponse<{ url: string }>>("/subscriptions/checkout", body);
  },
  portal(body: { returnUrl: string }) {
    return api.post<ApiResponse<{ url: string }>>("/subscriptions/portal", body);
  },
  cancel() { return api.post<ApiResponse<null>>("/subscriptions/cancel"); },
  getInvoices() { return api.get<ApiResponse<InvoiceDto[]>>("/subscriptions/invoices"); },
};

// ─── Greeting API ───────────────────────────────────────────────────
export interface GreetingDto {
  greeting: string;
  emoji?: string;
  tip?: string;
}

export const greetingApi = {
  get(userName?: string, lang?: string) {
    const offset = new Date().getTimezoneOffset();
    return api.get<ApiResponse<GreetingDto>>("/greeting", {
      params: { userName, timezoneOffsetMinutes: offset, lang },
    });
  },
};

// ─── Hotkeys API ────────────────────────────────────────────────────
export interface HotkeyDto { id: string; action: string; keyCombination: string; scope?: string; }

export const hotkeysApi = {
  getAll() { return api.get<ApiResponse<HotkeyDto[]>>("/hotkeys"); },
  set(body: { action: string; keyCombination: string; scope?: string }) { return api.post<ApiResponse<HotkeyDto>>("/hotkeys", body); },
  remove(id: string) { return api.delete<ApiResponse<null>>(`/hotkeys/${id}`); },
  seedDefaults() { return api.post<ApiResponse<null>>("/hotkeys/seed-defaults"); },
};

// ─── DevInsights API ────────────────────────────────────────────────
export interface TimeSavedDto { totalMinutes: number; byCategory: Record<string, number>; trend: Array<{ date: string; minutes: number }>; }
export interface FocusHeatmapDto { data: Array<{ day: number; hour: number; value: number }>; }
export interface TechDebtDto { score: number; issues: Array<{ file: string; type: string; severity: string; message: string }>; }
export interface DeploySuccessDto { total: number; successful: number; rate: number; trend: Array<{ date: string; rate: number }>; }
export interface PeakHoursDto { hours: Array<{ hour: number; productivity: number }>; bestHour: number; }

export const devInsightsApi = {
  timeSaved(from?: string, to?: string) { return api.get<ApiResponse<TimeSavedDto>>("/devinsights/time-saved", { params: { from, to } }); },
  focusHeatmap(from?: string, to?: string) { return api.get<ApiResponse<FocusHeatmapDto>>("/devinsights/focus-heatmap", { params: { from, to } }); },
  techDebt(projectPath?: string) { return api.get<ApiResponse<TechDebtDto>>("/devinsights/tech-debt", { params: { projectPath } }); },
  deploySuccessRate(from?: string, to?: string) { return api.get<ApiResponse<DeploySuccessDto>>("/devinsights/deployment-success-rate", { params: { from, to } }); },
  peakHours(from?: string, to?: string) { return api.get<ApiResponse<PeakHoursDto>>("/devinsights/peak-hours", { params: { from, to } }); },
};

// ─── DevUtilities API ───────────────────────────────────────────────
export const devUtilitiesApi = {
  decodeJwt(token: string) { return api.post<ApiResponse<any>>("/devutilities/decode-jwt", { token }); },
  testRegex(body: { pattern: string; input: string; flags?: string }) { return api.post<ApiResponse<any>>("/devutilities/test-regex", body); },
  generateCron(body: { description: string }) { return api.post<ApiResponse<any>>("/devutilities/generate-cron", body); },
  base64(body: { input: string; encode: boolean }) { return api.post<ApiResponse<any>>("/devutilities/base64", body); },
  sshKey(body: { type?: string; bits?: number }) { return api.post<ApiResponse<any>>("/devutilities/ssh-key", body); },
  jsonFormat(body: { json: string }) { return api.post<ApiResponse<any>>("/devutilities/json/format", body); },
  sendRequest(body: { method: string; url: string; headers?: Record<string, string>; body?: string }) {
    return api.post<ApiResponse<any>>("/devutilities/network/send-request", body);
  },
  scanDependencies(body: { projectPath: string }) { return api.post<ApiResponse<any>>("/devutilities/security/scan-dependencies", body); },
  checkPort(port: number) { return api.get<ApiResponse<any>>(`/devutilities/system/check-port/${port}`); },
  killProcess(pid: number) { return api.delete<ApiResponse<any>>(`/devutilities/system/kill-process/${pid}`); },
};

// ─── ProactiveAgents API ────────────────────────────────────────────
export const proactiveAgentsApi = {
  explainError(body: { stackTrace: string; language?: string }) { return api.post<ApiResponse<any>>("/proactiveagents/explain-error", body); },
  resolvePort(body: { port: number }) { return api.post<ApiResponse<any>>("/proactiveagents/resolve-port", body); },
  killIdleContainers() { return api.post<ApiResponse<any>>("/proactiveagents/kill-idle-containers"); },
  suggestCommit(body: { diff: string }) { return api.post<ApiResponse<any>>("/proactiveagents/suggest-commit", body); },
  summarizePr(body: { prUrl: string }) { return api.post<ApiResponse<any>>("/proactiveagents/summarize-pr", body); },
  watchDependencies(body: { projectPath?: string }) { return api.post<ApiResponse<any>>("/proactiveagents/watch-dependencies", body); },
  search(body: { query: string }) { return api.post<ApiResponse<any>>("/proactiveagents/search", body); },
};

// ─── Scripts API ────────────────────────────────────────────────────
export interface ScriptDto { id: string; name: string; description?: string; language: string; content: string; lastRun?: string; }
export interface ScriptRunResult { output: string; exitCode: number; duration: number; }

export const scriptsApi = {
  create(body: { name: string; command: string; workingDirectory?: string }) { return api.post<ApiResponse<ScriptDto>>("/scripts", body); },
  run(id: string) { return api.post<ApiResponse<ScriptRunResult>>(`/scripts/${id}/run`); },
  spinEnvironment(body: { projectPath: string; env?: string }) { return api.post<ApiResponse<ScriptRunResult>>("/scripts/spin-environment", body); },
  resolveConflicts(body: { projectPath: string }) { return api.post<ApiResponse<ScriptRunResult>>("/scripts/resolve-conflicts", body); },
  nukeMigrate(body: { projectPath: string; connectionString?: string }) { return api.post<ApiResponse<ScriptRunResult>>("/scripts/nuke-migrate", body); },
  flushCache() { return api.post<ApiResponse<ScriptRunResult>>("/scripts/flush-cache"); },
  formatLint(body: { projectPath: string }) { return api.post<ApiResponse<ScriptRunResult>>("/scripts/format-lint", body); },
  killNodes() { return api.post<ApiResponse<ScriptRunResult>>("/scripts/kill-nodes"); },
  generateBoilerplate(body: { template: string; projectName: string }) { return api.post<ApiResponse<ScriptRunResult>>("/scripts/generate-boilerplate", body); },
};

// ─── Snippets API ───────────────────────────────────────────────────
export interface SnippetDto { id: string; title: string; language: string; content: string; tags?: string[]; isFavorite: boolean; createdAt: string; updatedAt: string; }

export const snippetsApi = {
  getAll() { return api.get<ApiResponse<SnippetDto[]>>("/snippets"); },
  create(body: { title: string; code: string; language: string; tags?: string[] }) { return api.post<ApiResponse<SnippetDto>>("/snippets", body); },
  update(id: string, body: { title: string; code: string; language: string; tags?: string[] }) { return api.put<ApiResponse<SnippetDto>>(`/snippets/${id}`, body); },
  remove(id: string) { return api.delete<ApiResponse<null>>(`/snippets/${id}`); },
  toggleFavorite(id: string) { return api.patch<ApiResponse<null>>(`/snippets/${id}/favorite`); },
  sendToNotion(body: { snippetId: string }) { return api.post<ApiResponse<null>>("/snippets/send-to-notion", body); },
  pasteFromNotion(body: { pageId: string }) { return api.post<ApiResponse<SnippetDto>>("/snippets/paste-from-notion", body); },
};

// ─── Focus (Pomodoro) API ───────────────────────────────────────────
export interface FocusSessionDto { id: string; task: string; duration: number; startedAt: string; endedAt?: string; status: string; breaks: number; }
export interface FocusStatsDto { totalSessions: number; totalMinutes: number; averageDuration: number; streak: number; todaySessions: number; todayMinutes: number; }

export const focusApi = {
  start(body: { durationMinutes: number; taskDescription?: string; sessionType?: string }) { return api.post<ApiResponse<FocusSessionDto>>("/focus", body); },
  getStats() { return api.get<ApiResponse<FocusStatsDto>>("/focus/stats"); },
  getActive() { return api.get<ApiResponse<FocusSessionDto | null>>("/focus/active"); },
  complete(id: string) { return api.post<ApiResponse<null>>(`/focus/${id}/complete`); },
  pause(id: string) { return api.post<ApiResponse<null>>(`/focus/${id}/pause`); },
  resume(id: string) { return api.post<ApiResponse<null>>(`/focus/${id}/resume`); },
  interrupt(id: string) { return api.post<ApiResponse<null>>(`/focus/${id}/interrupt`); },
  history(days?: number) { return api.get<ApiResponse<FocusSessionDto[]>>("/focus/history", { params: { days } }); },
};

// ─── Docker API ─────────────────────────────────────────────────────
export interface DockerContainerDto { id: string; name: string; image: string; status: string; state: string; ports?: string; created: string; }

export const dockerApi = {
  getAll() { return api.get<ApiResponse<DockerContainerDto[]>>("/docker"); },
  getLogs(id: string) { return api.get<ApiResponse<string>>(`/docker/${id}/logs`); },
  start(id: string) { return api.post<ApiResponse<null>>(`/docker/${id}/start`); },
  stop(id: string) { return api.post<ApiResponse<null>>(`/docker/${id}/stop`); },
  restart(id: string) { return api.post<ApiResponse<null>>(`/docker/${id}/restart`); },
};

// ─── Git + Jira API ─────────────────────────────────────────────────
export interface GitDashboardDto {
  repos: Array<{ name: string; stars: number; forks: number; language: string; updatedAt: string }>;
  pullRequests: Array<{ id: number; title: string; state: string; author: string; createdAt: string; url: string; repo: string }>;
  recentCommits: Array<{ sha: string; message: string; author: string; date: string; repo: string }>;
}

export const gitApi = {
  dashboard(integrationId: string) { return api.get<ApiResponse<GitDashboardDto>>(`/git/dashboard/${integrationId}`); },
  approve(body: { integrationId: string; owner: string; repo: string; prNumber: number }) { return api.post<ApiResponse<null>>("/git/approve", body); },
  reject(body: { integrationId: string; owner: string; repo: string; prNumber: number; reason?: string }) { return api.post<ApiResponse<null>>("/git/reject", body); },
  merge(body: { integrationId: string; owner: string; repo: string; prNumber: number }) { return api.post<ApiResponse<null>>("/git/merge", body); },
  jiraPomodoro(body: { issueKey: string; duration?: number }) { return api.post<ApiResponse<FocusSessionDto>>("/git/jira-pomodoro", body); },
};

// ─── Sentry API ─────────────────────────────────────────────────────
export interface SentryIssueDto { id: string; title: string; culprit: string; count: number; firstSeen: string; lastSeen: string; level: string; status: string; }

export const sentryApi = {
  getIssues(integrationId: string, projectSlug?: string) {
    return api.get<ApiResponse<SentryIssueDto[]>>(`/sentry/${integrationId}/issues`, { params: { projectSlug } });
  },
  getIssue(integrationId: string, issueId: string) {
    return api.get<ApiResponse<SentryIssueDto>>(`/sentry/${integrationId}/issues/${issueId}`);
  },
  resolve(issueId: string) { return api.post<ApiResponse<null>>(`/sentry/issues/${issueId}/resolve`); },
};

// ─── SonarQube API ──────────────────────────────────────────────────
export interface SonarQubeDto { status: string; bugs: number; vulnerabilities: number; codeSmells: number; coverage: number; duplications: number; }

export const sonarQubeApi = {
  getQuality(integrationId: string, projectKey?: string) {
    return api.get<ApiResponse<SonarQubeDto>>(`/sonarqube/${integrationId}/quality`, { params: { projectKey } });
  },
};

// ─── AWS API ────────────────────────────────────────────────────────
export interface AwsDeploymentDto { id: string; serviceName: string; status: string; createdAt: string; completedAt?: string; version: string; }

export const awsApi = {
  getDeployments(integrationId: string, serviceName?: string) {
    return api.get<ApiResponse<AwsDeploymentDto[]>>(`/aws/${integrationId}/deployments`, { params: { serviceName } });
  },
  getDeploymentStatus(integrationId: string, deploymentId: string) {
    return api.get<ApiResponse<AwsDeploymentDto>>(`/aws/${integrationId}/deployments/${deploymentId}/status`);
  },
};

// ─── Teams API ──────────────────────────────────────────────────────
export interface TeamDto { id: string; name: string; description?: string; memberCount: number; createdAt: string; }
export interface TeamMemberDto { userId: string; fullName: string; email: string; role: string; joinedAt: string; }

export const teamsApi = {
  getMyTeams() { return api.get<ApiResponse<TeamDto[]>>("/teams/my"); },
  create(body: { name: string; description?: string }) { return api.post<ApiResponse<TeamDto>>("/teams", body); },
  getTeam(teamId: string) { return api.get<ApiResponse<any>>(`/teams/${teamId}`); },
  addMember(teamId: string, body: { userId: string }) { return api.post<ApiResponse<null>>(`/teams/${teamId}/members`, body); },
  removeMember(teamId: string, userId: string) { return api.delete<ApiResponse<null>>(`/teams/${teamId}/members/${userId}`); },
  getRadar(teamId: string) { return api.get<ApiResponse<any>>(`/teams/${teamId}/radar`); },
  getProductivity(teamId: string) { return api.get<ApiResponse<any>>(`/teams/${teamId}/productivity`); },
  shareWorkspace(teamId: string, body: { workspaceId: string }) { return api.post<ApiResponse<null>>(`/teams/${teamId}/share-workspace`, body); },
};

// ─── GlobalShortcuts API ────────────────────────────────────────────
export const globalShortcutsApi = {
  commandPalette(search: string) { return api.get<ApiResponse<any>>("/globalshortcuts/command-palette", { params: { search } }); },
  aiContext(body: { selectedText: string; contextType: string }) { return api.post<ApiResponse<any>>("/globalshortcuts/ai-context", body); },
  capture(body: { title: string; content: string; tags?: string[] }) { return api.post<ApiResponse<any>>("/globalshortcuts/capture", body); },
  share(body: { content: string; recipients?: string[] }) { return api.post<ApiResponse<any>>("/globalshortcuts/share", body); },
  calendarEvent(body: { text: string }) { return api.post<ApiResponse<any>>("/globalshortcuts/calendar-event", body); },
};

// ─── Modals API ─────────────────────────────────────────────────────
export interface ModalDto { id: string; type: string; title: string; message: string; data?: any; }

export const modalsApi = {
  getPending() { return api.get<ApiResponse<ModalDto[]>>("/modals/pending"); },
  dismiss(modalId: string) { return api.post<ApiResponse<null>>(`/modals/${modalId}/dismiss`); },
};

// ─── System API ─────────────────────────────────────────────────
export const systemApi = {
  getIdes() { return api.get<ApiResponse<any>>("/system/ides"); },
  analyze() { return api.get<ApiResponse<any>>("/system/analyze"); },
};

// ─── TeamInfo API ───────────────────────────────────────────────
export interface TeamInfoDto { teamId: string; objective?: string; armory?: any; vaultLinks?: any[]; members?: any[]; }

export const teamInfoApi = {
  getInfo(teamId: string) { return api.get<ApiResponse<TeamInfoDto>>(`/teaminfo/${teamId}`); },
  setObjective(teamId: string, body: { title: string; description?: string; deadline?: string }) { return api.post<ApiResponse<null>>(`/teaminfo/${teamId}/objective`, body); },
  updateMyFocus(teamId: string, body: { focusDescription: string }) { return api.put<ApiResponse<null>>(`/teaminfo/${teamId}/my-focus`, body); },
  updateArmory(teamId: string, body: { stagingServerUrl?: string; testAccountEmail?: string; testAccountPassword?: string; productionVersion?: string; stagingVersion?: string }) { return api.put<ApiResponse<null>>(`/teaminfo/${teamId}/armory`, body); },
  addVaultLink(teamId: string, body: { label: string; url: string; icon?: string; sortOrder?: number }) { return api.post<ApiResponse<any>>(`/teaminfo/${teamId}/vault-links`, body); },
  updateVaultLink(teamId: string, linkId: string, body: { label: string; url: string; icon?: string; sortOrder?: number }) { return api.put<ApiResponse<null>>(`/teaminfo/${teamId}/vault-links/${linkId}`, body); },
  deleteVaultLink(teamId: string, linkId: string) { return api.delete<ApiResponse<null>>(`/teaminfo/${teamId}/vault-links/${linkId}`); },
};

// ─── OmniFeed API ───────────────────────────────────────────────
export interface OmniFeedItemDto { id: string; source: string; content: string; author?: string; timestamp: string; read: boolean; emojis?: any[]; }

export const omniFeedApi = {
  getFeed(teamId: string, params?: { source?: string; page?: number; pageSize?: number }) { return api.get<ApiResponse<OmniFeedItemDto[]>>(`/omnifeed/${teamId}`, { params }); },
  publish(body: { teamId: string; content: string; source?: string }) { return api.post<ApiResponse<OmniFeedItemDto>>("/omnifeed/publish", body); },
  markRead(itemId: string) { return api.post<ApiResponse<null>>(`/omnifeed/${itemId}/read`); },
  addEmoji(itemId: string, body: { emoji: string }) { return api.post<ApiResponse<null>>(`/omnifeed/${itemId}/emoji`, body); },
};

// ─── SquadRadar API ─────────────────────────────────────────────
export interface SquadRadarDto { userId: string; fullName: string; status: string; currentTask?: string; lastActive: string; }

export const squadRadarApi = {
  getRadar(teamId: string) { return api.get<ApiResponse<SquadRadarDto[]>>(`/squadradar/${teamId}`); },
  updatePresence(body: { status: string; currentTask?: string; teamId: string }) { return api.put<ApiResponse<null>>("/squadradar/presence", body); },
};

// ─── SquadArena API ─────────────────────────────────────────────
export interface LeaderboardEntryDto { userId: string; fullName: string; xp: number; rank: number; badges: any[]; }
export interface BountyDto { id: string; title: string; description?: string; xpReward: number; status: string; claimedBy?: string; }

export const squadArenaApi = {
  getLeaderboard(teamId: string) { return api.get<ApiResponse<LeaderboardEntryDto[]>>(`/squadarena/leaderboard/${teamId}`); },
  getBounties(teamId: string) { return api.get<ApiResponse<BountyDto[]>>(`/squadarena/bounties/${teamId}`); },
  giveBadge(body: { teamId: string; recipientUserId: string; badgeType: string; message?: string }) { return api.post<ApiResponse<null>>("/squadarena/badge", body); },
  createBounty(body: { teamId: string; title: string; description?: string; xpReward: number }) { return api.post<ApiResponse<BountyDto>>("/squadarena/bounty", body); },
  claimBounty(id: string) { return api.post<ApiResponse<null>>(`/squadarena/bounty/${id}/claim`); },
  completeBounty(id: string) { return api.post<ApiResponse<null>>(`/squadarena/bounty/${id}/complete`); },
};

// ─── ResourceHub API ────────────────────────────────────────────
export interface ResourceDto { id: string; title: string; url: string; category: string; description?: string; isPinned: boolean; createdAt: string; }

export const resourceHubApi = {
  getResources(teamId: string, category?: string) { return api.get<ApiResponse<ResourceDto[]>>(`/resourcehub/${teamId}`, { params: { category } }); },
  create(body: { teamId: string; title: string; url: string; category: string; description?: string }) { return api.post<ApiResponse<ResourceDto>>("/resourcehub", body); },
  update(body: { resourceId: string; title: string; url: string; category: string; description?: string }) { return api.put<ApiResponse<null>>("/resourcehub", body); },
  remove(resourceId: string) { return api.delete<ApiResponse<null>>(`/resourcehub/${resourceId}`); },
  togglePin(resourceId: string) { return api.post<ApiResponse<null>>(`/resourcehub/${resourceId}/pin`); },
};

// ─── Projects API ───────────────────────────────────────────────
export interface ProjectDto { id: string; name: string; path: string; framework?: string; createdAt: string; }

export const projectsApi = {
  create(body: { name: string; path: string; framework?: string }) { return api.post<ApiResponse<ProjectDto>>("/projects", body); },
  runMigration(id: string, body?: { migrationName?: string }) { return api.post<ApiResponse<any>>(`/projects/${id}/migration`, body); },
  databaseUpdate(id: string) { return api.post<ApiResponse<any>>(`/projects/${id}/database-update`); },
};

// ─── Knowledge API ──────────────────────────────────────────────
export const knowledgeApi = {
  getNotionDocs() { return api.get<ApiResponse<any>>("/knowledge/notion"); },
};

// ═══════════════════════════════════════════════════════════════════
// 🎨 DESIGNER DASHBOARD APIs
// ═══════════════════════════════════════════════════════════════════

// ─── DesignInsights API ─────────────────────────────────────────
export interface AssetsOptimizedDto { totalSavedMb: number; totalOptimized: number; }
export interface HandoffsDto { count: number; }
export interface DesignDebtDto { count: number; }

export const designInsightsApi = {
  assetsOptimized() { return api.get<ApiResponse<AssetsOptimizedDto>>("/designinsights/assets-optimized"); },
  handoffs(from?: string, to?: string) { return api.get<ApiResponse<HandoffsDto>>("/designinsights/handoffs", { params: { from, to } }); },
  colorTrends() { return api.get<ApiResponse<Record<string, number>>>("/designinsights/color-trends"); },
  designDebt() { return api.get<ApiResponse<DesignDebtDto>>("/designinsights/design-debt"); },
};

// ─── DesignUtilities API ────────────────────────────────────────
export interface CompressResultDto { outputPath: string; originalSize: number; compressedSize: number; savedPercent: number; }
export interface OptimizeSvgDto { optimizedSvg: string; originalLength: number; optimizedLength: number; }
export interface ExtractCssDto { css: string; }
export interface ContrastCheckDto { ratio: number; passesAA: boolean; passesAAA: boolean; level: string; }
export interface AspectRatioDto { ratio: string; simplifiedWidth: number; simplifiedHeight: number; }
export interface PaletteDto { id: string; name: string; colors: PaletteColorDto[]; }
export interface PaletteColorDto { id?: string; name: string; hexCode: string; }

export const designUtilitiesApi = {
  compressImage(body: { filePath: string; quality: number }) {
    return api.post<ApiResponse<CompressResultDto>>("/designutilities/compress-image", body);
  },
  convertAsset(file: File, targetFormat: string) {
    const fd = new FormData();
    fd.append("file", file);
    fd.append("targetFormat", targetFormat);
    return api.post("/designutilities/convert-asset", fd, { responseType: "blob" });
  },
  optimizeSvg(svgContent: string) {
    return api.post<ApiResponse<OptimizeSvgDto>>("/designutilities/optimize-svg", { svgContent });
  },
  extractCss(colors: Array<{ name: string; hexCode: string }>, format: "css" | "scss" | "less" = "css") {
    return api.post<ApiResponse<ExtractCssDto>>("/designutilities/extract-css", { colors, format });
  },
  checkContrast(foregroundHex: string, backgroundHex: string) {
    return api.post<ApiResponse<ContrastCheckDto>>("/designutilities/check-contrast", { foregroundHex, backgroundHex });
  },
  aspectRatio(width: number, height: number) {
    return api.get<ApiResponse<AspectRatioDto>>("/designutilities/aspect-ratio", { params: { width, height } });
  },
  dummyData(type: string, count: number) {
    return api.get<ApiResponse<any[]>>("/designutilities/dummy-data", { params: { type, count } });
  },
  getPalettes() { return api.get<ApiResponse<PaletteDto[]>>("/designutilities/palettes"); },
  createPalette(name: string) { return api.post<ApiResponse<string>>("/designutilities/palettes", { name }); },
  addColorToPalette(paletteId: string, body: { paletteId: string; name: string; hexCode: string }) {
    return api.post<ApiResponse<string>>(`/designutilities/palettes/${paletteId}/colors`, body);
  },
};

// ─── Figma API ──────────────────────────────────────────────────
export interface FigmaCommentDto { id: string; message: string; authorName: string; authorAvatarUrl?: string; createdAt: string; isResolved: boolean; parentId?: string | null; }

export const figmaApi = {
  getComments(integrationId: string, fileKey: string) {
    return api.get<ApiResponse<FigmaCommentDto[]>>(`/figma/${integrationId}/comments`, { params: { fileKey } });
  },
  resolveComment(body: { integrationId: string; fileKey: string; commentId: string }) {
    return api.post<ApiResponse<null>>("/figma/comments/resolve", body);
  },
};

// ─── Miro API ───────────────────────────────────────────────────
export interface MiroBoardDto { id: string; name: string; description?: string; viewLink: string; modifiedAt: string; stickyNoteCount: number; }

export const miroApi = {
  getBoards(integrationId: string) {
    return api.get<ApiResponse<MiroBoardDto[]>>(`/miro/${integrationId}/boards`);
  },
  createSticky(body: { integrationId: string; boardId: string; content: string }) {
    return api.post<ApiResponse<null>>("/miro/sticky", body);
  },
};

// ─── LottieFiles API ────────────────────────────────────────────
export interface LottieAnimDto { id: string; name: string; previewUrl: string; downloadUrl: string; authorName: string; likesCount: number; }

export const lottieFilesApi = {
  search(integrationId: string, query: string) {
    return api.get<ApiResponse<LottieAnimDto[]>>(`/lottiefiles/${integrationId}/search`, { params: { query } });
  },
};

// ─── Dribbble API ───────────────────────────────────────────────
export interface DribbbleShotDto { id: string; title: string; htmlUrl: string; imageUrl: string; authorName: string; authorAvatarUrl?: string; likesCount: number; viewsCount: number; publishedAt: string; }

export const dribbbleApi = {
  inspiration(integrationId: string, query: string) {
    return api.get<ApiResponse<DribbbleShotDto[]>>(`/dribbble/${integrationId}/inspiration`, { params: { query } });
  },
};

// ─── Zeplin API ─────────────────────────────────────────────────
export interface ZeplinScreenDto { id: string; name: string; imageUrl: string; width: number; height: number; updatedAt: string; }
export interface ZeplinStyleGuideDto {
  projectId: string;
  colors: Array<{ name: string; hexCode: string; opacity: number }>;
  fonts: Array<{ family: string; size: number; weight: string }>;
  spacings: Array<{ name: string; value: number }>;
}

export const zeplinApi = {
  getScreens(integrationId: string, projectId: string) {
    return api.get<ApiResponse<ZeplinScreenDto[]>>(`/zeplin/${integrationId}/screens`, { params: { projectId } });
  },
  getStyleGuide(integrationId: string, projectId: string) {
    return api.get<ApiResponse<ZeplinStyleGuideDto>>(`/zeplin/${integrationId}/style-guide`, { params: { projectId } });
  },
};

// ═══════════════════════════════════════════════════════════════════
// 🛡️ SECOPS DASHBOARD APIs
// ═══════════════════════════════════════════════════════════════════

// ─── SecOpsInsights API ─────────────────────────────────────────
export interface ThreatsBlockedDto { totalBlocked: number; ddosBlocked: number; malwareBlocked: number; bruteForceBlocked: number; }
export interface VulnsPatchedDto { totalPatched: number; critical: number; high: number; medium: number; low: number; }
export interface AvgResponseTimeDto { averageMinutes: number; fastestMinutes: number; slowestMinutes: number; }
export interface SecurityScoreDto { score: number; grade: string; recommendations: string[]; }
export interface ZeroIncidentDto { days: number; lastIncidentDate: string; }
export interface ScannedBytesDto { totalBytes: number; formattedSize: string; }
export interface OpenPortsGraphDto { dataPoints: Record<string, number>; }

export const secOpsInsightsApi = {
  threatsBlocked(from?: string, to?: string) { return api.get<ApiResponse<ThreatsBlockedDto>>("/secopsinsights/threats-blocked", { params: { from, to } }); },
  vulnerabilitiesPatched(from?: string, to?: string) { return api.get<ApiResponse<VulnsPatchedDto>>("/secopsinsights/vulnerabilities-patched", { params: { from, to } }); },
  avgResponseTime(from?: string, to?: string) { return api.get<ApiResponse<AvgResponseTimeDto>>("/secopsinsights/avg-response-time", { params: { from, to } }); },
  securityScore() { return api.get<ApiResponse<SecurityScoreDto>>("/secopsinsights/security-score"); },
  zeroIncidentStreak() { return api.get<ApiResponse<ZeroIncidentDto>>("/secopsinsights/zero-incident-streak"); },
  scannedBytes(from?: string, to?: string) { return api.get<ApiResponse<ScannedBytesDto>>("/secopsinsights/scanned-bytes", { params: { from, to } }); },
  openPortsGraph(from?: string, to?: string) { return api.get<ApiResponse<OpenPortsGraphDto>>("/secopsinsights/open-ports-graph", { params: { from, to } }); },
};

// ─── SecOpsUtilities API ────────────────────────────────────────
export interface HashResultDto { hash: string; }
export interface IpDnsResultDto { ip: string; hostname: string; country: string; isp: string; organization: string; }
export interface EncodeResultDto { encoded: string; }
export interface PasswordEntropyDto { entropy: number; strength: string; estimatedCrackTime: string; }
export interface SslCheckDto { subject: string; issuer: string; notBefore: string; notAfter: string; daysRemaining: number; isValid: boolean; }
export interface PortScanResultDto { port: number; protocol: string; serviceName: string; }
export interface SpoofMacDto { result: string; }

export const secOpsUtilitiesApi = {
  hash(body: { input: string; algorithm: string }) { return api.post<ApiResponse<HashResultDto>>("/secopsutilities/hash", body); },
  ipDns(body: { target: string }) { return api.post<ApiResponse<IpDnsResultDto>>("/secopsutilities/ip-dns", body); },
  encodePayload(body: { input: string; encoding: string }) { return api.post<ApiResponse<EncodeResultDto>>("/secopsutilities/encode-payload", body); },
  passwordEntropy(body: { password: string }) { return api.post<ApiResponse<PasswordEntropyDto>>("/secopsutilities/password-entropy", body); },
  sslCheck(body: { domain: string }) { return api.post<ApiResponse<SslCheckDto>>("/secopsutilities/ssl-check", body); },
  portScan(body: { target: string; ports: number[] }) { return api.post<ApiResponse<PortScanResultDto[]>>("/secopsutilities/port-scan", body); },
  spoofMac(body: { interfaceName: string }) { return api.post<ApiResponse<SpoofMacDto>>("/secopsutilities/spoof-mac", body); },
};

// ─── SecOpsAgents API ───────────────────────────────────────────
export interface RoguePortDto { port: number; processName: string; processId: number; status: string; }
export interface ExpiringSslDto { domain: string; expiresAt: string; daysRemaining: number; }
export interface SuspiciousTrafficDto { isSuspicious: boolean; requestCount: number; originCountry: string; summary: string; }
export interface LeakedKeyDto { keyType: string; snippet: string; lineNumber: number; }
export interface PatchSuggestionDto { packageName: string; currentVersion: string; suggestedVersion: string; severity: string; }
export interface ZombieProcessDto { processId: number; processName: string; memoryMb: number; status: string; }
export interface VpnStatusDto { isConnected: boolean; publicIp: string; vpnIp: string | null; isLeaking: boolean; }

export const secOpsAgentsApi = {
  detectRoguePorts() { return api.post<ApiResponse<RoguePortDto[]>>("/secopsagents/detect-rogue-ports", {}); },
  warnExpiringSsl(body: { domains: string[] }) { return api.post<ApiResponse<ExpiringSslDto[]>>("/secopsagents/warn-expiring-ssl", body); },
  detectSuspiciousTraffic(body: { targetUrl: string }) { return api.post<ApiResponse<SuspiciousTrafficDto>>("/secopsagents/detect-suspicious-traffic", body); },
  scanLeakedKeys(body: { content: string }) { return api.post<ApiResponse<LeakedKeyDto[]>>("/secopsagents/scan-leaked-keys", body); },
  suggestPatches(body: { projectPath: string }) { return api.post<ApiResponse<PatchSuggestionDto[]>>("/secopsagents/suggest-patches", body); },
  killZombieProcesses() { return api.post<ApiResponse<ZombieProcessDto[]>>("/secopsagents/kill-zombie-processes", {}); },
  vpnStatus() { return api.get<ApiResponse<VpnStatusDto>>("/secopsagents/vpn-status"); },
};

// ─── SecOpsScripts API ──────────────────────────────────────────
export interface ScriptOutputDto { output: string; }

export const secOpsScriptsApi = {
  quickScan(body: { networkRange: string }) { return api.post<ApiResponse<ScriptOutputDto>>("/secopsscripts/quick-scan", body); },
  panicButton(body: { interfaceName: string }) { return api.post<ApiResponse<ScriptOutputDto>>("/secopsscripts/panic-button", body); },
  localWipe(body: { wipeHistory: boolean; wipeCredentials: boolean }) { return api.post<ApiResponse<ScriptOutputDto>>("/secopsscripts/local-wipe", body); },
  phishingAlert(body: { emailHeaders: string; senderAddress: string }) { return api.post<ApiResponse<ScriptOutputDto>>("/secopsscripts/phishing-alert", body); },
  rotateSsh(body: { keyComment: string; keySize: number }) { return api.post<ApiResponse<ScriptOutputDto>>("/secopsscripts/rotate-ssh", body); },
  firewallLockdown(body: { allowedPorts: number[] }) { return api.post<ApiResponse<ScriptOutputDto>>("/secopsscripts/firewall-lockdown", body); },
  clearDns() { return api.post<ApiResponse<ScriptOutputDto>>("/secopsscripts/clear-dns", {}); },
};


// ═══════════════════════════════════════════════════════════════════
// 👑 TEAM LEADER DASHBOARD APIs
// ═══════════════════════════════════════════════════════════════════

// ─── LeaderInsights ─────────────────────────────────────────────
export interface SprintDataPoint { sprintName: string; points: number; endDate: string; }
export interface SprintVelocityDto { totalPoints: number; averagePerSprint: number; dataPoints: SprintDataPoint[]; }
export interface MeetingsAvoidedDto { meetingsCancelled: number; hoursSaved: number; estimatedMoneySaved: number; }
export interface BlockedMember { memberName: string; blockedHours: number; topBlocker: string; }
export interface BlockedTimeDto { totalBlockedHours: number; members: BlockedMember[]; }
export interface FeatureCost { featureName: string; estimatedHours: number; cost: number; }
export interface CostPerFeatureDto { features: FeatureCost[]; averageCost: number; }
export interface ReviewTurnaroundDto { averageHours: number; medianHours: number; totalReviews: number; }
export interface TopContributorDto { memberName: string; tasksClosed: number; prsMerged: number; bugsFixed: number; totalScore: number; }
export interface MoodFactor { factor: string; impact: number; direction: string; }
export interface TeamMoodDto { stressLevel: number; happinessLevel: number; overallMood: string; factors: MoodFactor[]; }

export const leaderInsightsApi = {
  sprintVelocity(teamId: string, from?: string, to?: string) { return api.get<ApiResponse<SprintVelocityDto>>("/leaderinsights/sprint-velocity", { params: { teamId, from, to } }); },
  meetingsAvoided(teamId: string, from?: string, to?: string) { return api.get<ApiResponse<MeetingsAvoidedDto>>("/leaderinsights/meetings-avoided", { params: { teamId, from, to } }); },
  blockedTime(teamId: string, from?: string, to?: string) { return api.get<ApiResponse<BlockedTimeDto>>("/leaderinsights/blocked-time", { params: { teamId, from, to } }); },
  costPerFeature(teamId: string, from?: string, to?: string) { return api.get<ApiResponse<CostPerFeatureDto>>("/leaderinsights/cost-per-feature", { params: { teamId, from, to } }); },
  reviewTurnaround(teamId: string, from?: string, to?: string) { return api.get<ApiResponse<ReviewTurnaroundDto>>("/leaderinsights/review-turnaround", { params: { teamId, from, to } }); },
  topContributor(teamId: string, from?: string, to?: string) { return api.get<ApiResponse<TopContributorDto>>("/leaderinsights/top-contributor", { params: { teamId, from, to } }); },
  teamMood(teamId: string, from?: string, to?: string) { return api.get<ApiResponse<TeamMoodDto>>("/leaderinsights/team-mood", { params: { teamId, from, to } }); },
};

// ─── LeaderUtilities ────────────────────────────────────────────
export interface MemberTimeEntry { memberName: string; timezoneId: string; localTime: string; offset: string; }
export interface TimezonesDto { utcNow: string; memberTimes: MemberTimeEntry[]; }
export interface QuickPollDto { pollId: string; formattedMessage: string; }
export interface CapacityMember { memberName: string; availableHours: number; }
export interface CapacityDto { totalAvailableHours: number; members: CapacityMember[]; }
export interface CostEstimateDto { laborCost: number; infrastructureCost: number; totalCost: number; breakdown: string; }
export interface RiskItem { title: string; impact: number; probability: number; score: number; }
export interface RiskMatrixDto { urgent: RiskItem[]; important: RiskItem[]; later: RiskItem[]; }
export interface DecisionLogDto { id: string; decision: string; rationale: string; decidedBy: string; recordedAt: string; }
export interface MdHtmlDto { html: string; }

export const leaderUtilitiesApi = {
  timezones(body: { members: { memberName: string; timezoneId: string }[] }) { return api.post<ApiResponse<TimezonesDto>>("/leaderutilities/timezones", body); },
  quickPoll(body: { question: string; options: string[] }) { return api.post<ApiResponse<QuickPollDto>>("/leaderutilities/quick-poll", body); },
  capacity(body: { members: { memberName: string; hoursPerDay: number; daysOff: number; meetingHoursPerWeek: number }[] }) { return api.post<ApiResponse<CapacityDto>>("/leaderutilities/capacity", body); },
  costEstimate(body: { hoursEstimated: number; hourlyRate: number; serverMonthlyCost: number; estimatedMonths: number }) { return api.post<ApiResponse<CostEstimateDto>>("/leaderutilities/cost-estimate", body); },
  riskMatrix(body: { items: { title: string; impact: number; probability: number }[] }) { return api.post<ApiResponse<RiskMatrixDto>>("/leaderutilities/risk-matrix", body); },
  decisionLog(body: { decision: string; rationale: string; decidedBy: string }) { return api.post<ApiResponse<DecisionLogDto>>("/leaderutilities/decision-log", body); },
  markdown(body: { markdown: string }) { return api.post<ApiResponse<MdHtmlDto>>("/leaderutilities/markdown", body); },
};

// ─── LeaderAgents ───────────────────────────────────────────────
export interface BottleneckMember { memberName: string; taskKey: string; daysStuck: number; recommendation: string; }
export interface BottleneckDto { members: BottleneckMember[]; }
export interface BurnoutMember { memberName: string; overtimeHours: number; lateNightCommits: number; riskLevel: string; }
export interface BurnoutRiskDto { members: BurnoutMember[]; }
export interface ScopeCreepDto { originalTaskCount: number; currentTaskCount: number; addedMidSprint: number; creepPercentage: number; warning: string; }
export interface StalePr { prTitle: string; author: string; hoursPending: number; url: string; }
export interface PrReviewNagDto { stalePrs: StalePr[]; totalStale: number; }
export interface UnassignedBug { issueKey: string; title: string; severity: string; reportedAt: string; }
export interface UnassignedBugsDto { bugs: UnassignedBug[]; totalUnassigned: number; }
export interface GhostMember { memberName: string; lastActiveAt: string; hoursInactive: number; }
export interface GhostMembersDto { ghostMembers: GhostMember[]; }
export interface MilestoneDto { hasMilestone: boolean; milestoneName: string; completionPercentage: number; celebrationMessage: string; }

export const leaderAgentsApi = {
  bottleneck(teamId: string) { return api.get<ApiResponse<BottleneckDto>>(`/leaderagents/bottleneck/${teamId}`); },
  burnoutRisk(teamId: string) { return api.get<ApiResponse<BurnoutRiskDto>>(`/leaderagents/burnout-risk/${teamId}`); },
  scopeCreep(teamId: string, sprintId?: string) { return api.get<ApiResponse<ScopeCreepDto>>(`/leaderagents/scope-creep/${teamId}`, { params: { sprintId } }); },
  prReviewNag(body: { teamId: string; thresholdHours: number }) { return api.post<ApiResponse<PrReviewNagDto>>("/leaderagents/pr-review-nag", body); },
  unassignedBugs(teamId: string) { return api.get<ApiResponse<UnassignedBugsDto>>(`/leaderagents/unassigned-bugs/${teamId}`); },
  ghostMembers(body: { teamId: string }) { return api.post<ApiResponse<GhostMembersDto>>("/leaderagents/ghost-members", body); },
  milestone(teamId: string) { return api.get<ApiResponse<MilestoneDto>>(`/leaderagents/milestone/${teamId}`); },
};

// ─── LeaderScripts ──────────────────────────────────────────────
export interface SprintStarterDto { sprintId: string; sprintName: string; tasksCreated: number; slackNotification: string; }
export interface BlockedTask { taskKey: string; assignee: string; summary: string; daysBlocked: number; }
export interface BlockedTaskBlasterDto { blockedTasksFound: number; tasks: BlockedTask[]; messagesSent: number; }
export interface ReleaseNotesDto { notes: string; }
export interface LeaderScriptOutputDto { output: string; }
export interface WeekSummaryDto { tasksCompleted: number; bugsFixed: number; prsMerged: number; velocityPoints: number; summaryMarkdown: string; }
export interface ReassignedTask { taskKey: string; fromUser: string; toUser: string; }
export interface BulkReassignDto { tasksReassigned: number; tasks: ReassignedTask[]; }

export const leaderScriptsApi = {
  sprintStarter(body: { sprintName: string; initialTasks: string[]; teamId: string }) { return api.post<ApiResponse<SprintStarterDto>>("/leaderscripts/sprint-starter", body); },
  blockedTaskBlaster(body: { teamId: string }) { return api.post<ApiResponse<BlockedTaskBlasterDto>>("/leaderscripts/blocked-task-blaster", body); },
  releaseNotes(body: { repoName: string; fromTag: string; toTag: string }) { return api.post<ApiResponse<ReleaseNotesDto>>("/leaderscripts/release-notes", body); },
  meetingMode(body: { durationMinutes: number }) { return api.post<ApiResponse<LeaderScriptOutputDto>>("/leaderscripts/meeting-mode", body); },
  weekSummary(body: { teamId: string }) { return api.post<ApiResponse<WeekSummaryDto>>("/leaderscripts/week-summary", body); },
  bulkReassign(body: { absentMemberId: string; teamId: string }) { return api.post<ApiResponse<BulkReassignDto>>("/leaderscripts/bulk-reassign", body); },
  standupPing(body: { teamId: string }) { return api.post<ApiResponse<LeaderScriptOutputDto>>("/leaderscripts/standup-ping", body); },
};

// ─── LeaderModals ───────────────────────────────────────────────
export interface LeaderModalDto { id: string; modalType: string; hasBeenSeen: boolean; dismissedAt: string | null; payloadJson: string | null; teamId: string; }
export interface LeaderModalPayloadDto { id: string; modalType: string; payloadJson: string; }

export const leaderModalsApi = {
  getAll() { return api.get<ApiResponse<LeaderModalDto[]>>("/leadermodals"); },
  getPayload(modalId: string) { return api.get<ApiResponse<LeaderModalPayloadDto>>(`/leadermodals/${modalId}/payload`); },
  create(body: { modalType: number; teamId: string; payloadJson?: string }) { return api.post<ApiResponse<{ id: string }>>("/leadermodals", body); },
  dismiss(modalId: string) { return api.post<ApiResponse<void>>(`/leadermodals/${modalId}/dismiss`); },
};

// ─── Gmail API ──────────────────────────────────────────────────
export interface GmailMessageDto { id: string; from: string; subject: string; snippet: string; date: string; isRead: boolean; }
export const gmailApi = {
  getUnread() { return api.get<ApiResponse<GmailMessageDto[]>>("/gmail/unread"); },
};

// ─── Invoice DTO ────────────────────────────────────────────────
export interface InvoiceDto {
  id: string;
  date: string;
  status: string;
  amountPaid: number;
  currency: string;
  pdfUrl: string;
  hostedUrl: string;
}

// ─── Preferences API ────────────────────────────────────────────
export interface PreferencesDto {
  language: string;
  theme: string;
  timezone: string;
  emailNotifications: boolean;
  pushNotifications: boolean;
  inboxAlerts: boolean;
  inboxApprovals: boolean;
  inboxMentions: boolean;
  inboxSystem: boolean;
  weeklyDigest: boolean;
  customSettingsJson?: string | null;
}

export const preferencesApi = {
  get() { return api.get<ApiResponse<PreferencesDto>>("/preferences"); },
  update(body: Partial<PreferencesDto>) { return api.put<ApiResponse<null>>("/preferences", body); },
};

// ─── Notifications / Smart Inbox API ────────────────────────────
export interface NotificationDto {
  id: string;
  category: "AlertsSecOps" | "ApprovalsGit" | "MentionsSocial" | "SystemInsights";
  priority: "Low" | "Normal" | "High" | "Critical";
  title: string;
  body: string;
  actionType?: string | null;
  actionPayloadJson?: string | null;
  sourceEntity?: string | null;
  sourceEntityId?: string | null;
  isRead: boolean;
  readAt?: string | null;
  workspaceId?: string | null;
  createdAt: string;
}

export interface UnreadCountDto {
  total: number;
  alertsSecOps: number;
  approvalsGit: number;
  mentionsSocial: number;
  systemInsights: number;
}

export const notificationsApi = {
  getAll(params?: { category?: string; unreadOnly?: boolean; page?: number; pageSize?: number }) {
    return api.get<ApiResponse<NotificationDto[]>>("/notifications", { params });
  },
  getUnreadCount() {
    return api.get<ApiResponse<UnreadCountDto>>("/notifications/unread-count");
  },
  markAsRead(id: string) {
    return api.post<ApiResponse<null>>(`/notifications/${id}/read`);
  },
  markAllAsRead(category?: string) {
    return api.post<ApiResponse<null>>("/notifications/read-all", null, { params: { category } });
  },
  execute(id: string) {
    return api.post<ApiResponse<unknown>>(`/notifications/${id}/execute`);
  },
  remove(id: string) {
    return api.delete<ApiResponse<null>>(`/notifications/${id}`);
  },
};

// ─── Search API (Cmd+K) ────────────────────────────────────────
export interface SearchResultItem {
  type: string;
  id: string;
  title: string;
  subtitle?: string;
  icon?: string;
  route: string;
}

export interface SearchResultDto {
  workspaces: SearchResultItem[];
  integrations: SearchResultItem[];
  scripts: SearchResultItem[];
  snippets: SearchResultItem[];
  projects: SearchResultItem[];
  teams: SearchResultItem[];
  commands: SearchResultItem[];
}

export const searchApi = {
  search(q: string, limit = 5) {
    return api.get<ApiResponse<SearchResultDto>>("/search", { params: { q, limit } });
  },
};

// ─── Audit Logs API ─────────────────────────────────────────────
export interface AuditLogDto {
  id: string;
  action: string;
  detail?: string | null;
  ipAddress?: string | null;
  createdAt: string;
}

export interface SessionDto {
  ipAddress: string;
  userAgent: string;
  lastLoginAt: string;
  isCurrent: boolean;
}

export const auditLogsApi = {
  getAll(params?: { action?: string; from?: string; to?: string; page?: number; pageSize?: number }) {
    return api.get<ApiResponse<AuditLogDto[]>>("/auditlogs", { params });
  },
  getSessions() {
    return api.get<ApiResponse<SessionDto[]>>("/auditlogs/sessions");
  },
};

// ─── Personal Tokens API ────────────────────────────────────────
export interface PersonalTokenDto {
  id: string;
  name: string;
  token?: string;         // Only on create response
  tokenPrefix?: string;   // On list
  scopes: string[];
  expiresAt?: string | null;
  lastUsedAt?: string | null;
  isRevoked: boolean;
}

export const personalTokensApi = {
  getAll() {
    return api.get<ApiResponse<PersonalTokenDto[]>>("/personaltokens");
  },
  create(body: { name: string; scopes: string[]; expiresAt?: string | null }) {
    return api.post<ApiResponse<PersonalTokenDto>>("/personaltokens", body);
  },
  revoke(id: string) {
    return api.post<ApiResponse<null>>(`/personaltokens/${id}/revoke`);
  },
};

// ─── Webhooks API ───────────────────────────────────────────────
export interface WebhookDto {
  id: string;
  name: string;
  url: string;
  secret?: string | null;
  events: string[];
  workspaceId?: string | null;
  active: boolean;
  failCount: number;
  createdAt: string;
}

export const webhooksApi = {
  getAll() {
    return api.get<ApiResponse<WebhookDto[]>>("/webhooks");
  },
  create(body: { name: string; url: string; secret?: string; events: string[]; workspaceId?: string | null }) {
    return api.post<ApiResponse<WebhookDto>>("/webhooks", body);
  },
  update(id: string, body: { name?: string; url?: string; secret?: string; events?: string[] }) {
    return api.put<ApiResponse<null>>(`/webhooks/${id}`, body);
  },
  toggle(id: string, active: boolean) {
    return api.post<ApiResponse<null>>(`/webhooks/${id}/toggle`, { active });
  },
  remove(id: string) {
    return api.delete<ApiResponse<null>>(`/webhooks/${id}`);
  },
};

// ─── Support API ────────────────────────────────────────────────
export interface SupportTicketDto {
  id: string;
  type: string;
  subject: string;
  body: string;
  status: "Open" | "InProgress" | "Resolved" | "Closed";
  pageUrl?: string | null;
  browserInfo?: string | null;
  createdAt: string;
}

export const supportApi = {
  getTickets() {
    return api.get<ApiResponse<SupportTicketDto[]>>("/support");
  },
  create(body: { type: string; subject: string; body: string; pageUrl?: string; browserInfo?: string }) {
    return api.post<ApiResponse<SupportTicketDto>>("/support", body);
  },
  close(id: string) {
    return api.post<ApiResponse<null>>(`/support/${id}/close`);
  },
};

export default api;
