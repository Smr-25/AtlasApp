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

  resendPhoneVerificationCode(phoneNumber: string, channel: "Sms" | "WhatsApp" = "Sms") {
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

  addPhoneNumber(phoneNumber: string, verificationChannel: "Sms" | "WhatsApp" = "Sms") {
    return api.post<ApiResponse<null>>("/accounts/add-phone-number", {
      phoneNumber,
      verificationChannel,
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
  checkout(body: { tier: string }) { return api.post<ApiResponse<{ url: string }>>("/subscriptions/checkout", body); },
  portal() { return api.post<ApiResponse<{ url: string }>>("/subscriptions/portal"); },
  cancel() { return api.post<ApiResponse<null>>("/subscriptions/cancel"); },
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
export interface HotkeyDto { id: string; action: string; keys: string; category?: string; }

export const hotkeysApi = {
  getAll() { return api.get<ApiResponse<HotkeyDto[]>>("/hotkeys"); },
  set(body: { action: string; keys: string }) { return api.post<ApiResponse<HotkeyDto>>("/hotkeys", body); },
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
  timeSaved(from?: string, to?: string) { return api.get<ApiResponse<TimeSavedDto>>("/dev-insights/time-saved", { params: { from, to } }); },
  focusHeatmap(from?: string, to?: string) { return api.get<ApiResponse<FocusHeatmapDto>>("/dev-insights/focus-heatmap", { params: { from, to } }); },
  techDebt(projectPath?: string) { return api.get<ApiResponse<TechDebtDto>>("/dev-insights/tech-debt", { params: { projectPath } }); },
  deploySuccessRate(from?: string, to?: string) { return api.get<ApiResponse<DeploySuccessDto>>("/dev-insights/deployment-success-rate", { params: { from, to } }); },
  peakHours(from?: string, to?: string) { return api.get<ApiResponse<PeakHoursDto>>("/dev-insights/peak-hours", { params: { from, to } }); },
};

// ─── DevUtilities API ───────────────────────────────────────────────
export const devUtilitiesApi = {
  decodeJwt(token: string) { return api.post<ApiResponse<any>>("/dev-utilities/decode-jwt", { token }); },
  testRegex(body: { pattern: string; input: string; flags?: string }) { return api.post<ApiResponse<any>>("/dev-utilities/test-regex", body); },
  generateCron(body: { description: string }) { return api.post<ApiResponse<any>>("/dev-utilities/generate-cron", body); },
  base64(body: { input: string; encode: boolean }) { return api.post<ApiResponse<any>>("/dev-utilities/base64", body); },
  sshKey(body: { type?: string; bits?: number }) { return api.post<ApiResponse<any>>("/dev-utilities/ssh-key", body); },
  jsonFormat(body: { json: string }) { return api.post<ApiResponse<any>>("/dev-utilities/json/format", body); },
  sendRequest(body: { method: string; url: string; headers?: Record<string, string>; body?: string }) {
    return api.post<ApiResponse<any>>("/dev-utilities/network/send-request", body);
  },
  scanDependencies(body: { projectPath: string }) { return api.post<ApiResponse<any>>("/dev-utilities/security/scan-dependencies", body); },
  checkPort(port: number) { return api.get<ApiResponse<any>>(`/dev-utilities/system/check-port/${port}`); },
  killProcess(pid: number) { return api.delete<ApiResponse<any>>(`/dev-utilities/system/kill-process/${pid}`); },
};

// ─── ProactiveAgents API ────────────────────────────────────────────
export const proactiveAgentsApi = {
  explainError(body: { stackTrace: string; language?: string }) { return api.post<ApiResponse<any>>("/proactive-agents/explain-error", body); },
  resolvePort(body: { port: number }) { return api.post<ApiResponse<any>>("/proactive-agents/resolve-port", body); },
  killIdleContainers() { return api.post<ApiResponse<any>>("/proactive-agents/kill-idle-containers"); },
  suggestCommit(body: { diff: string }) { return api.post<ApiResponse<any>>("/proactive-agents/suggest-commit", body); },
  summarizePr(body: { prUrl: string }) { return api.post<ApiResponse<any>>("/proactive-agents/summarize-pr", body); },
  watchDependencies(body: { projectPath?: string }) { return api.post<ApiResponse<any>>("/proactive-agents/watch-dependencies", body); },
  search(body: { query: string }) { return api.post<ApiResponse<any>>("/proactive-agents/search", body); },
};

// ─── Scripts API ────────────────────────────────────────────────────
export interface ScriptDto { id: string; name: string; description?: string; language: string; content: string; lastRun?: string; }
export interface ScriptRunResult { output: string; exitCode: number; duration: number; }

export const scriptsApi = {
  create(body: { name: string; description?: string; language: string; content: string }) { return api.post<ApiResponse<ScriptDto>>("/scripts", body); },
  run(id: string) { return api.post<ApiResponse<ScriptRunResult>>(`/scripts/${id}/run`); },
  spinEnvironment(body: { projectPath: string; template?: string }) { return api.post<ApiResponse<ScriptRunResult>>("/scripts/spin-environment", body); },
  resolveConflicts(body: { projectPath: string }) { return api.post<ApiResponse<ScriptRunResult>>("/scripts/resolve-conflicts", body); },
  nukeMigrate(body: { projectPath: string }) { return api.post<ApiResponse<ScriptRunResult>>("/scripts/nuke-migrate", body); },
  flushCache() { return api.post<ApiResponse<ScriptRunResult>>("/scripts/flush-cache"); },
  formatLint(body: { projectPath: string }) { return api.post<ApiResponse<ScriptRunResult>>("/scripts/format-lint", body); },
  killNodes() { return api.post<ApiResponse<ScriptRunResult>>("/scripts/kill-nodes"); },
  generateBoilerplate(body: { template: string; name: string; outputPath: string }) { return api.post<ApiResponse<ScriptRunResult>>("/scripts/generate-boilerplate", body); },
};

// ─── Snippets API ───────────────────────────────────────────────────
export interface SnippetDto { id: string; title: string; language: string; content: string; tags?: string[]; isFavorite: boolean; createdAt: string; updatedAt: string; }

export const snippetsApi = {
  getAll() { return api.get<ApiResponse<SnippetDto[]>>("/snippets"); },
  create(body: { title: string; language: string; content: string; tags?: string[] }) { return api.post<ApiResponse<SnippetDto>>("/snippets", body); },
  update(id: string, body: { title: string; language: string; content: string; tags?: string[] }) { return api.put<ApiResponse<SnippetDto>>(`/snippets/${id}`, body); },
  remove(id: string) { return api.delete<ApiResponse<null>>(`/snippets/${id}`); },
  toggleFavorite(id: string) { return api.patch<ApiResponse<null>>(`/snippets/${id}/favorite`); },
  sendToNotion(body: { snippetId: string }) { return api.post<ApiResponse<null>>("/snippets/send-to-notion", body); },
  pasteFromNotion(body: { pageId: string }) { return api.post<ApiResponse<SnippetDto>>("/snippets/paste-from-notion", body); },
};

// ─── Focus (Pomodoro) API ───────────────────────────────────────────
export interface FocusSessionDto { id: string; task: string; duration: number; startedAt: string; endedAt?: string; status: string; breaks: number; }
export interface FocusStatsDto { totalSessions: number; totalMinutes: number; averageDuration: number; streak: number; todaySessions: number; todayMinutes: number; }

export const focusApi = {
  start(body: { task: string; duration?: number }) { return api.post<ApiResponse<FocusSessionDto>>("/focus", body); },
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
    return api.get<ApiResponse<SonarQubeDto>>(`/sonar-qube/${integrationId}/quality`, { params: { projectKey } });
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
  addMember(teamId: string, body: { email: string; role?: string }) { return api.post<ApiResponse<null>>(`/teams/${teamId}/members`, body); },
  removeMember(teamId: string, userId: string) { return api.delete<ApiResponse<null>>(`/teams/${teamId}/members/${userId}`); },
  getRadar(teamId: string) { return api.get<ApiResponse<any>>(`/teams/${teamId}/radar`); },
  getProductivity(teamId: string) { return api.get<ApiResponse<any>>(`/teams/${teamId}/productivity`); },
  shareWorkspace(teamId: string, body: { workspaceId: string }) { return api.post<ApiResponse<null>>(`/teams/${teamId}/share-workspace`, body); },
};

// ─── GlobalShortcuts API ────────────────────────────────────────────
export const globalShortcutsApi = {
  commandPalette(search: string) { return api.get<ApiResponse<any>>("/global-shortcuts/command-palette", { params: { search } }); },
  aiContext(body: { context: string }) { return api.post<ApiResponse<any>>("/global-shortcuts/ai-context", body); },
  capture(body: { content: string; type?: string }) { return api.post<ApiResponse<any>>("/global-shortcuts/capture", body); },
  share(body: { content: string; recipients?: string[] }) { return api.post<ApiResponse<any>>("/global-shortcuts/share", body); },
  calendarEvent(body: { text: string }) { return api.post<ApiResponse<any>>("/global-shortcuts/calendar-event", body); },
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
  getInfo(teamId: string) { return api.get<ApiResponse<TeamInfoDto>>(`/team-info/${teamId}`); },
  setObjective(teamId: string, body: { objective: string }) { return api.post<ApiResponse<null>>(`/team-info/${teamId}/objective`, body); },
  updateMyFocus(teamId: string, body: { focus: string }) { return api.put<ApiResponse<null>>(`/team-info/${teamId}/my-focus`, body); },
  updateArmory(teamId: string, body: { tools: string[] }) { return api.put<ApiResponse<null>>(`/team-info/${teamId}/armory`, body); },
  addVaultLink(teamId: string, body: { name: string; url: string; category?: string }) { return api.post<ApiResponse<any>>(`/team-info/${teamId}/vault-links`, body); },
  updateVaultLink(teamId: string, linkId: string, body: { name: string; url: string; category?: string }) { return api.put<ApiResponse<null>>(`/team-info/${teamId}/vault-links/${linkId}`, body); },
  deleteVaultLink(teamId: string, linkId: string) { return api.delete<ApiResponse<null>>(`/team-info/${teamId}/vault-links/${linkId}`); },
};

// ─── OmniFeed API ───────────────────────────────────────────────
export interface OmniFeedItemDto { id: string; source: string; content: string; author?: string; timestamp: string; read: boolean; emojis?: any[]; }

export const omniFeedApi = {
  getFeed(teamId: string, params?: { source?: string; page?: number; pageSize?: number }) { return api.get<ApiResponse<OmniFeedItemDto[]>>(`/omni-feed/${teamId}`, { params }); },
  publish(body: { teamId: string; content: string; source?: string }) { return api.post<ApiResponse<OmniFeedItemDto>>("/omni-feed/publish", body); },
  markRead(itemId: string) { return api.post<ApiResponse<null>>(`/omni-feed/${itemId}/read`); },
  addEmoji(itemId: string, body: { emoji: string }) { return api.post<ApiResponse<null>>(`/omni-feed/${itemId}/emoji`, body); },
};

// ─── SquadRadar API ─────────────────────────────────────────────
export interface SquadRadarDto { userId: string; fullName: string; status: string; currentTask?: string; lastActive: string; }

export const squadRadarApi = {
  getRadar(teamId: string) { return api.get<ApiResponse<SquadRadarDto[]>>(`/squad-radar/${teamId}`); },
  updatePresence(body: { status: string; currentTask?: string }) { return api.put<ApiResponse<null>>("/squad-radar/presence", body); },
};

// ─── SquadArena API ─────────────────────────────────────────────
export interface LeaderboardEntryDto { userId: string; fullName: string; xp: number; rank: number; badges: any[]; }
export interface BountyDto { id: string; title: string; description?: string; xpReward: number; status: string; claimedBy?: string; }

export const squadArenaApi = {
  getLeaderboard(teamId: string) { return api.get<ApiResponse<LeaderboardEntryDto[]>>(`/squad-arena/leaderboard/${teamId}`); },
  getBounties(teamId: string) { return api.get<ApiResponse<BountyDto[]>>(`/squad-arena/bounties/${teamId}`); },
  giveBadge(body: { userId: string; badge: string; reason?: string }) { return api.post<ApiResponse<null>>("/squad-arena/badge", body); },
  createBounty(body: { teamId: string; title: string; description?: string; xpReward: number }) { return api.post<ApiResponse<BountyDto>>("/squad-arena/bounty", body); },
  claimBounty(id: string) { return api.post<ApiResponse<null>>(`/squad-arena/bounty/${id}/claim`); },
  completeBounty(id: string) { return api.post<ApiResponse<null>>(`/squad-arena/bounty/${id}/complete`); },
};

// ─── ResourceHub API ────────────────────────────────────────────
export interface ResourceDto { id: string; title: string; url: string; category: string; description?: string; isPinned: boolean; createdAt: string; }

export const resourceHubApi = {
  getResources(teamId: string, category?: string) { return api.get<ApiResponse<ResourceDto[]>>(`/resource-hub/${teamId}`, { params: { category } }); },
  create(body: { teamId: string; title: string; url: string; category: string; description?: string }) { return api.post<ApiResponse<ResourceDto>>("/resource-hub", body); },
  update(body: { resourceId: string; title: string; url: string; category: string; description?: string }) { return api.put<ApiResponse<null>>("/resource-hub", body); },
  remove(resourceId: string) { return api.delete<ApiResponse<null>>(`/resource-hub/${resourceId}`); },
  togglePin(resourceId: string) { return api.post<ApiResponse<null>>(`/resource-hub/${resourceId}/pin`); },
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
  assetsOptimized() { return api.get<ApiResponse<AssetsOptimizedDto>>("/design-insights/assets-optimized"); },
  handoffs(from?: string, to?: string) { return api.get<ApiResponse<HandoffsDto>>("/design-insights/handoffs", { params: { from, to } }); },
  colorTrends() { return api.get<ApiResponse<Record<string, number>>>("/design-insights/color-trends"); },
  designDebt() { return api.get<ApiResponse<DesignDebtDto>>("/design-insights/design-debt"); },
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
    return api.post<ApiResponse<CompressResultDto>>("/design-utilities/compress-image", body);
  },
  convertAsset(file: File, targetFormat: string) {
    const fd = new FormData();
    fd.append("file", file);
    fd.append("targetFormat", targetFormat);
    return api.post("/design-utilities/convert-asset", fd, { responseType: "blob" });
  },
  optimizeSvg(svgContent: string) {
    return api.post<ApiResponse<OptimizeSvgDto>>("/design-utilities/optimize-svg", { svgContent });
  },
  extractCss(colors: Array<{ name: string; hexCode: string }>, format: "css" | "scss" | "less" = "css") {
    return api.post<ApiResponse<ExtractCssDto>>("/design-utilities/extract-css", { colors, format });
  },
  checkContrast(foregroundHex: string, backgroundHex: string) {
    return api.post<ApiResponse<ContrastCheckDto>>("/design-utilities/check-contrast", { foregroundHex, backgroundHex });
  },
  aspectRatio(width: number, height: number) {
    return api.get<ApiResponse<AspectRatioDto>>("/design-utilities/aspect-ratio", { params: { width, height } });
  },
  dummyData(type: string, count: number) {
    return api.get<ApiResponse<any[]>>("/design-utilities/dummy-data", { params: { type, count } });
  },
  getPalettes() { return api.get<ApiResponse<PaletteDto[]>>("/design-utilities/palettes"); },
  createPalette(name: string) { return api.post<ApiResponse<string>>("/design-utilities/palettes", { name }); },
  addColorToPalette(paletteId: string, body: { paletteId: string; name: string; hexCode: string }) {
    return api.post<ApiResponse<string>>(`/design-utilities/palettes/${paletteId}/colors`, body);
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
    return api.get<ApiResponse<LottieAnimDto[]>>(`/lottie-files/${integrationId}/search`, { params: { query } });
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
  threatsBlocked(from?: string, to?: string) { return api.get<ApiResponse<ThreatsBlockedDto>>("/sec-ops-insights/threats-blocked", { params: { from, to } }); },
  vulnerabilitiesPatched(from?: string, to?: string) { return api.get<ApiResponse<VulnsPatchedDto>>("/sec-ops-insights/vulnerabilities-patched", { params: { from, to } }); },
  avgResponseTime(from?: string, to?: string) { return api.get<ApiResponse<AvgResponseTimeDto>>("/sec-ops-insights/avg-response-time", { params: { from, to } }); },
  securityScore() { return api.get<ApiResponse<SecurityScoreDto>>("/sec-ops-insights/security-score"); },
  zeroIncidentStreak() { return api.get<ApiResponse<ZeroIncidentDto>>("/sec-ops-insights/zero-incident-streak"); },
  scannedBytes(from?: string, to?: string) { return api.get<ApiResponse<ScannedBytesDto>>("/sec-ops-insights/scanned-bytes", { params: { from, to } }); },
  openPortsGraph(from?: string, to?: string) { return api.get<ApiResponse<OpenPortsGraphDto>>("/sec-ops-insights/open-ports-graph", { params: { from, to } }); },
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
  hash(body: { input: string; algorithm: string }) { return api.post<ApiResponse<HashResultDto>>("/sec-ops-utilities/hash", body); },
  ipDns(body: { target: string }) { return api.post<ApiResponse<IpDnsResultDto>>("/sec-ops-utilities/ip-dns", body); },
  encodePayload(body: { input: string; encoding: string }) { return api.post<ApiResponse<EncodeResultDto>>("/sec-ops-utilities/encode-payload", body); },
  passwordEntropy(body: { password: string }) { return api.post<ApiResponse<PasswordEntropyDto>>("/sec-ops-utilities/password-entropy", body); },
  sslCheck(body: { hostname: string }) { return api.post<ApiResponse<SslCheckDto>>("/sec-ops-utilities/ssl-check", body); },
  portScan(body: { target: string; startPort: number; endPort: number }) { return api.post<ApiResponse<PortScanResultDto[]>>("/sec-ops-utilities/port-scan", body); },
  spoofMac(body: { interfaceName: string }) { return api.post<ApiResponse<SpoofMacDto>>("/sec-ops-utilities/spoof-mac", body); },
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
  detectRoguePorts() { return api.post<ApiResponse<RoguePortDto[]>>("/sec-ops-agents/detect-rogue-ports", {}); },
  warnExpiringSsl(body: { domains: string[] }) { return api.post<ApiResponse<ExpiringSslDto[]>>("/sec-ops-agents/warn-expiring-ssl", body); },
  detectSuspiciousTraffic(body: { targetUrl: string }) { return api.post<ApiResponse<SuspiciousTrafficDto>>("/sec-ops-agents/detect-suspicious-traffic", body); },
  scanLeakedKeys(body: { content: string }) { return api.post<ApiResponse<LeakedKeyDto[]>>("/sec-ops-agents/scan-leaked-keys", body); },
  suggestPatches(body: { projectPath: string }) { return api.post<ApiResponse<PatchSuggestionDto[]>>("/sec-ops-agents/suggest-patches", body); },
  killZombieProcesses() { return api.post<ApiResponse<ZombieProcessDto[]>>("/sec-ops-agents/kill-zombie-processes", {}); },
  vpnStatus() { return api.get<ApiResponse<VpnStatusDto>>("/sec-ops-agents/vpn-status"); },
};

// ─── SecOpsScripts API ──────────────────────────────────────────
export interface ScriptOutputDto { output: string; }

export const secOpsScriptsApi = {
  quickScan(body: { networkRange: string }) { return api.post<ApiResponse<ScriptOutputDto>>("/sec-ops-scripts/quick-scan", body); },
  panicButton(body: { interfaceName: string }) { return api.post<ApiResponse<ScriptOutputDto>>("/sec-ops-scripts/panic-button", body); },
  localWipe(body: { wipeHistory: boolean; wipeCredentials: boolean }) { return api.post<ApiResponse<ScriptOutputDto>>("/sec-ops-scripts/local-wipe", body); },
  phishingAlert(body: { emailHeaders: string; senderAddress: string }) { return api.post<ApiResponse<ScriptOutputDto>>("/sec-ops-scripts/phishing-alert", body); },
  rotateSsh(body: { keyComment: string; keySize: number }) { return api.post<ApiResponse<ScriptOutputDto>>("/sec-ops-scripts/rotate-ssh", body); },
  firewallLockdown(body: { allowedPorts: number[] }) { return api.post<ApiResponse<ScriptOutputDto>>("/sec-ops-scripts/firewall-lockdown", body); },
  clearDns() { return api.post<ApiResponse<ScriptOutputDto>>("/sec-ops-scripts/clear-dns", {}); },
};

export default api;
