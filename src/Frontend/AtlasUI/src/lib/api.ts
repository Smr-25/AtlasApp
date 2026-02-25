// ...existing code...

export const API_BASE = import.meta.env.VITE_API_BASE || "http://localhost:5000";

export async function postJson<T = any>(path: string, body: any, token?: string): Promise<T> {
  const res = await fetch(`${API_BASE}${path}`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
    body: JSON.stringify(body),
  });

  const data = await res.json().catch(() => null);
  if (!res.ok) {
    const err: any = new Error(data?.message || `Request failed: ${res.status}`);
    err.status = res.status;
    err.data = data;
    throw err;
  }
  return data as T;
}

// Accounts API helpers
export const accounts = {
  register: (payload: any) => postJson('/api/accounts/register', payload),
  login: (payload: any) => postJson('/api/accounts/login', payload),
  externalLogin: (payload: { Provider: string; IdToken: string; AccessToken?: string | null; AuthorizationCode?: string | null }) =>
    postJson('/api/accounts/external-login', payload),
  forgotPassword: (payload: { Email: string }) => postJson('/api/accounts/forgot-password', payload),
  verifyResetCode: (payload: { Email: string; VerificationCode: string }) => postJson('/api/accounts/verify-reset-code', payload),
  resetPassword: (payload: { Email: string; ResetToken: string; NewPassword: string; ConfirmPassword: string }) =>
    postJson('/api/accounts/reset-password', payload),
  verifyEmail: (payload: { Email: string; VerificationCode: string }) => postJson('/api/accounts/verify-email', payload),
  verifyPhone: (payload: { PhoneNumber: string; VerificationCode: string }) => postJson('/api/accounts/verify-phone', payload),
  resendEmailVerificationCode: (payload: { Email: string }) => postJson('/api/accounts/resend-email-verification-code', payload),
  resendPhoneVerificationCode: (payload: { PhoneNumber: string; Channel: string }) => postJson('/api/accounts/resend-phone-verification-code', payload),
  refreshToken: (payload: { RefreshToken: string }) => postJson('/api/accounts/refresh-token', payload),
  revokeRefreshToken: (token?: string) => postJson('/api/accounts/revoke-refresh-token', {}, token),
};

// Onboarding API helpers
export const onboarding = {
  getProfessionQuestion: () => fetch(`${API_BASE}/api/onboarding/profession-question`).then(r => r.json()),
  getQuestionsByProfession: (profession: string) => fetch(`${API_BASE}/api/onboarding/questions?profession=${encodeURIComponent(profession)}`).then(r => r.json()),
  createQuestion: (payload: { Text: string; Profession?: string }) => postJson('/api/onboarding/questions', payload),
  addOption: (questionId: string, payload: { QuestionId: string; Text: string }) => postJson(`/api/onboarding/questions/${questionId}/options`, payload),
  complete: (payload: { UserId?: string; Answers: any[] }, token?: string) => postJson('/api/onboarding/complete', payload, token),
};
