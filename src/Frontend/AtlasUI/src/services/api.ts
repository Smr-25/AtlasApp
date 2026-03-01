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

// ─── System API ─────────────────────────────────────────────────────
export const systemApi = {
  getIdes() { return api.get<ApiResponse<any>>("/system/ides"); },
  analyze() { return api.get<ApiResponse<any>>("/system/analyze"); },
};

export default api;
