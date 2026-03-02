# 🚀 ATLAS Frontend API Reference (Complete)

> **Base URL:** `http://localhost:5000/api`  
> **Tarix:** 2 Mart 2026  
> **Auth:** JWT Bearer Token (`Authorization: Bearer <token>`)  
> **SignalR Hub:** `ws://localhost:5000/hubs/atlas`

---

## 📦 Response Wrapper (Bütün endpoint-lər bu formatda cavab verir)

```json
// Success
{ "data": <T>, "isSuccess": true, "errors": null }

// Error
{ "data": null, "isSuccess": false, "errors": ["Error message"] }
```

**NoContent (204):** Body yoxdur.

---

## 1. Accounts (Auth & Profile)

### `POST /api/accounts/register` — Public
```json
// Request Body
{ "fullName": "string", "userName": "string", "email": "string", "password": "string" }
// Response → AuthResponseDto
{ "data": { "accessToken": "string", "refreshToken": "string", "accessTokenExpiration": "datetime", "refreshTokenExpiration": "datetime", "userId": "guid", "userName": "string", "email": "string", "fullName": "string", "role": "string" } }
```

### `POST /api/accounts/login` — Public
```json
// Request Body
{ "email": "string | null", "userName": "string | null", "password": "string" }
// Response → AuthResponseDto (same as register)
```

### `POST /api/accounts/external-login` — Public
```json
// Request Body
{ "provider": "github | google", "idToken": "string", "accessToken": "string | null", "authorizationCode": "string | null" }
// Response → ExternalLoginResponseDto
{ "data": { "accessToken": "string", "refreshToken": "string", "refreshTokenExpiration": "datetime", "isNewUser": true, "userId": "guid", "email": "string", "fullName": "string" } }
```

### `GET /api/accounts/external/{provider}` — Public
OAuth redirect. `provider`: `github` | `google`. Browser-ı consent page-ə yönləndirir.

### `GET /api/accounts/external/callback/{provider}?code=xxx` — Public
OAuth callback. Frontend-ə redirect: `/auth/callback?accessToken=xxx&refreshToken=xxx&provider=xxx&isNewUser=true`

### `POST /api/accounts/logout` — Public
```json
// Body
{ "refreshToken": "string" }
// Response → 204 No Content
```

### `POST /api/accounts/forgot-password` — Public
```json
// Body
{ "email": "string" }
// Response
{ "data": true }
```

### `POST /api/accounts/verify-reset-code` — Public
```json
// Body
{ "email": "string", "verificationCode": "string" }
// Response
{ "data": { "resetToken": "string", "email": "string" } }
```

### `POST /api/accounts/reset-password` — Public
```json
// Body
{ "email": "string", "resetToken": "string", "newPassword": "string", "confirmPassword": "string" }
// Response
{ "data": true }
```

### `POST /api/accounts/verify-email` — Public
```json
// Body
{ "email": "string", "verificationCode": "string" }
// Response → { "data": true }
```

### `POST /api/accounts/verify-phone` — Public
```json
// Body
{ "phoneNumber": "string", "verificationCode": "string" }
// Response → { "data": true }
```

### `POST /api/accounts/resend-email-verification-code` — Public
```json
{ "email": "string" }
```

### `POST /api/accounts/resend-phone-verification-code` — Public
```json
{ "phoneNumber": "string", "channel": "Sms | Telegram" }
```

### `POST /api/accounts/refresh-token` — Public
```json
// Body
{ "refreshToken": "string" }
// Response → TokenDto
{ "data": { "accessToken": "string", "refreshToken": "string", "accessTokenExpiration": "datetime", "refreshTokenExpiration": "datetime" } }
```

### `POST /api/accounts/revoke-refresh-token` — Public
```json
// Body (empty object)
{}
// Response → 204 No Content
```

### `GET /api/accounts/profile` — 🔐 Authorized
```json
// Response → AccountDto
{ "data": { "id": "guid", "userName": "string", "email": "string", "fullName": "string", "phoneNumber": "string | null", "emailConfirmed": true, "phoneConfirmed": false, "createdAt": "datetime", "status": "Active", "lastLoginAt": "datetime", "bio": "string | null", "tags": ["React", "DevOps"] } }
```

### `PUT /api/accounts/profile` — 🔐 Authorized
```json
// Body
{ "fullName": "string | null", "userName": "string | null" }
// Response → AccountDto
```

### `PUT /api/accounts/change-password` — 🔐 Authorized
```json
{ "currentPassword": "string", "newPassword": "string", "confirmPassword": "string" }
```

### `POST /api/accounts/add-phone-number` — 🔐 Authorized
```json
{ "phoneNumber": "+994501234567", "verificationChannel": "Sms | Telegram" }
```

### `DELETE /api/accounts/delete-account` — 🔐 Authorized
No body. Response: `{ "data": true }`

### `POST /api/accounts/set-telegram-chat-id` — 🔐 Authorized
```json
{ "telegramChatId": "string" }
```

### `POST /api/accounts/generate-telegram-link-code` — 🔐 Authorized
No body. Response: `{ "data": "LINK_CODE_STRING" }`

---

## 2. Onboarding

### `POST /api/onboarding/complete` — Public
```json
// Body
{ "userId": "guid", "answers": [{ "questionId": "guid", "optionId": "guid" }] }
// Response
{ "data": { "profileId": "guid" } }
```

---

## 3. Profiles

### `GET /api/profiles/me` — 🔐
Response: `UserProfileDto`

### `PUT /api/profiles/me` — 🔐
```json
{ "jobTitle": "string", "bio": "string | null", "themeColor": "string | null" }
// Response → 204 No Content
```

---

## 4. Preferences

### `GET /api/preferences` — 🔐
```json
{ "data": { "language": "en", "theme": "dark", "timezone": "Asia/Baku", "emailNotifications": true, "pushNotifications": true, "inboxAlerts": true, "inboxApprovals": true, "inboxMentions": true, "inboxSystem": true, "weeklyDigest": false, "customSettingsJson": null } }
```

### `PUT /api/preferences` — 🔐 (all fields optional)
```json
{ "language": "az", "theme": "light", "timezone": "Europe/London", "emailNotifications": false, "pushNotifications": true, "inboxAlerts": true, "inboxApprovals": true, "inboxMentions": false, "inboxSystem": true, "weeklyDigest": true, "customSettingsJson": null }
```

---

## 5. Workspaces

### `GET /api/workspaces` — 🔐
Response: `List<WorkspaceDto>`

### `GET /api/workspaces/{id}` — 🔐
Response: `WorkspaceDto`

### `POST /api/workspaces` — 🔐
```json
{ "name": "My Project", "description": "string | null", "localFolderPath": "/Users/me/projects/atlas | null" }
// Response → 201 { "data": "guid" }
```

### `PUT /api/workspaces/{id}` — 🔐
```json
{ "workspaceId": "guid", "name": "Updated", "description": "string | null", "localFolderPath": "string | null" }
// Response → 204
```

### `DELETE /api/workspaces/{id}` — 🔐 (Owner/Admin only)
### `PATCH /api/workspaces/{id}/set-default` — 🔐

### `POST /api/workspaces/{id}/integrations/toggle` — 🔐
```json
{ "integrationId": "guid", "enable": true }
```

### `POST /api/workspaces/validate-folder` — 🔐
```json
{ "folderPath": "/Users/me/projects/atlas" }
```

### `GET /api/workspaces/{id}/members` — 🔐

### `POST /api/workspaces/{id}/members` — 🔐
```json
{ "userId": "guid", "role": "Viewer | Editor | Admin | Owner" }
```

### `DELETE /api/workspaces/{id}/members/{userId}` — 🔐

### `PATCH /api/workspaces/{id}/members/{userId}/role` — 🔐
```json
{ "newRole": "Admin" }
```

---

## 6. Integrations

### `GET /api/integrations` — 🔐
### `GET /api/integrations/{id}` — 🔐
### `GET /api/integrations/pending` — 🔐

### `POST /api/integrations` — 🔐
```json
{ "provider": "GitHub | Jira | Figma | Sentry | SonarQube | Dribbble | LottieFiles | Notion | Slack | Stripe | PagerDuty | Perplexity", "name": "My GitHub", "accessToken": "ghp_xxxx", "refreshToken": "string | null", "expiresAt": "datetime | null", "metadataJson": "string | null" }
// Response → IntegrationDto
```

### `PUT /api/integrations/{id}` — 🔐
```json
{ "integrationId": "guid", "name": "Updated Name" }
```

### `DELETE /api/integrations/{id}` — 🔐

### `POST /api/integrations/{id}/reconnect` — 🔐
```json
{ "integrationId": "guid", "accessToken": "new_token", "refreshToken": "string | null", "expiresAt": "datetime | null", "metadataJson": "string | null" }
```

### `POST /api/integrations/{id}/mark-expired` — 🔐

---

## 7. Teams

### `GET /api/teams/my` — 🔐
### `GET /api/teams/{teamId}` — 🔐 → TeamDashboardDto

### `POST /api/teams` — 🔐
```json
{ "name": "Backend Team" }
// Response → 201 { "data": "guid" }
```

### `POST /api/teams/{teamId}/members` — 🔐
```json
{ "userId": "guid" }
```

### `DELETE /api/teams/{teamId}/members/{userId}` — 🔐
### `GET /api/teams/{teamId}/radar` — 🔐 TeamLeader Only
### `GET /api/teams/{teamId}/productivity` — 🔐 TeamLeader Only

### `POST /api/teams/{teamId}/share-workspace` — 🔐
```json
{ "workspaceId": "guid" }
```

---

## 8. Team Info

### `GET /api/teaminfo/{teamId}` — 🔐

### `POST /api/teaminfo/{teamId}/objective` — 🔐
```json
{ "title": "Q1 Sprint Goal", "description": "string | null", "deadline": "datetime | null" }
```

### `PUT /api/teaminfo/{teamId}/my-focus` — 🔐
```json
{ "focusDescription": "Working on auth module" }
```

### `PUT /api/teaminfo/{teamId}/armory` — 🔐
```json
{ "stagingServerUrl": "https://staging.app.com", "testAccountEmail": "string | null", "testAccountPassword": "string | null", "productionVersion": "string | null", "stagingVersion": "string | null" }
```

### `POST /api/teaminfo/{teamId}/vault-links` — 🔐
```json
{ "label": "Figma Design", "url": "https://figma.com/file/xxx", "icon": "figma | null", "sortOrder": 0 }
```

### `PUT /api/teaminfo/{teamId}/vault-links/{linkId}` — 🔐
### `DELETE /api/teaminfo/{teamId}/vault-links/{linkId}` — 🔐

---

## 9. Projects

### `POST /api/projects` — 🔐
```json
{ "name": "Atlas Backend", "type": "DotNet | Node | React | Angular | Flutter | Python | Other", "rootPath": "/path/to/project", "startupPath": "src/WebAPI | null", "migrationPath": "src/Persistence | null" }
```

### `POST /api/projects/{id}/migration` — 🔐
Body: `"InitialCreate"` (plain string, nullable)

### `POST /api/projects/{id}/database-update` — 🔐
Body: `"MigrationName"` (plain string, nullable)

---

## 10. Focus (Pomodoro)

### `POST /api/focus` — 🔐
```json
{ "durationMinutes": 25, "tag": "Deep Work", "sessionType": "Pomodoro | DeepWork | ShortBreak | LongBreak", "breakDurationMinutes": 5, "workspaceId": "guid | null" }
// Response → 201 { "data": "guid" }
```

### `GET /api/focus/stats` — 🔐
### `GET /api/focus/active` — 🔐
### `POST /api/focus/{sessionId}/complete` — 🔐
### `POST /api/focus/{sessionId}/pause` — 🔐
### `POST /api/focus/{sessionId}/resume` — 🔐
### `POST /api/focus/{sessionId}/interrupt` — 🔐
### `GET /api/focus/history?days=7` — 🔐

---

## 11. Notifications (Smart Inbox)

### `GET /api/notifications` — 🔐
**Query:** `?category=AlertsSecOps|ApprovalsGit|MentionsSocial|SystemInsights&unreadOnly=true&page=1&pageSize=30`

### `GET /api/notifications/unread-count` — 🔐
```json
{ "data": { "count": 5 } }
```

### `POST /api/notifications/{id}/read` — 🔐 → 204
### `POST /api/notifications/read-all?category=AlertsSecOps` — 🔐
```json
{ "data": { "markedAsRead": 12 } }
```

### `POST /api/notifications/{id}/execute` — 🔐
```json
{ "data": { "actionPayload": "{\"route\": \"/docker\"}" } }
```

### `DELETE /api/notifications/{id}` — 🔐 → 204

---

## 12. OmniFeed

### `GET /api/omnifeed/{teamId}?source=Git|Jira|Manual|System&page=1&pageSize=20` — 🔐

### `POST /api/omnifeed/publish` — 🔐
```json
{ "teamId": "guid", "title": "Deployed v2.1", "body": "string | null" }
```

### `POST /api/omnifeed/{itemId}/read` — 🔐 → 204
### `POST /api/omnifeed/{itemId}/emoji` — 🔐
```json
{ "emoji": "🎉" }
```

---

## 13. Snippets

### `GET /api/snippets` — 🔐
### `POST /api/snippets` — 🔐
```json
{ "title": "Debounce Hook", "code": "const useDebounce = ...", "language": "typescript", "tags": ["react", "hook"] }
```

### `PUT /api/snippets/{snippetId}` — 🔐
```json
{ "snippetId": "guid", "title": "Updated", "code": "...", "language": "typescript", "tags": ["react"] }
```

### `DELETE /api/snippets/{snippetId}` — 🔐
### `PATCH /api/snippets/{snippetId}/favorite` — 🔐

### `POST /api/snippets/send-to-notion` — 🔐
```json
{ "snippetId": "guid", "notionDatabaseId": "string", "notionAuthToken": "string" }
```

### `POST /api/snippets/paste-from-notion` — 🔐
```json
{ "notionDatabaseId": "string", "notionAuthToken": "string" }
```

---

## 14. Scripts

### `POST /api/scripts` — 🔐
```json
{ "name": "Start Dev", "command": "npm run dev", "arguments": "", "workingDirectory": "/path", "icon": "terminal | null", "color": "#4ade80 | null" }
```

### `POST /api/scripts/{id}/run` — 🔐
### `POST /api/scripts/spin-environment` — `{ "dockerComposePath": "...", "projectPath": "..." }`
### `POST /api/scripts/resolve-conflicts` — `{ "repositoryPath": "...", "strategy": "theirs" }`
### `POST /api/scripts/nuke-migrate` — `{ "connectionString": "...", "migrationsProjectPath": "..." }`
### `POST /api/scripts/flush-cache` — `{ "redisConnectionString": "... | null", "flushMemory": true }`
### `POST /api/scripts/format-lint` — `{ "projectPath": "..." }`
### `POST /api/scripts/kill-nodes` — No body
### `POST /api/scripts/generate-boilerplate` — `{ "projectName": "...", "templateName": "react-ts", "outputPath": "..." }`

---

## 15. Docker

### `GET /api/docker` — 🔐
```json
{ "data": [{ "id": "abc123", "name": "postgres", "image": "postgres:15", "state": "running", "status": "Up 2 hours", "ports": "5432:5432", "isRunning": true }] }
```

### `GET /api/docker/{id}/logs` — 🔐
### `POST /api/docker/{id}/start` — 🔐 → 204
### `POST /api/docker/{id}/stop` — 🔐 → 204
### `POST /api/docker/{id}/restart` — 🔐 → 204

---

## 16. System Monitor

### `GET /api/system/ides` — 🔐
### `GET /api/system/analyze` — 🔐 → Snapshot + AI advice

---

## 17. Git / GitHub

### `GET /api/git/dashboard/{integrationId}` — 🔐 → GitDashboardVm

### `POST /api/git/approve` — 🔐
```json
{ "integrationId": "guid", "owner": "octocat", "repo": "atlas", "prNumber": "42" }
```

### `POST /api/git/reject` — 🔐
```json
{ "integrationId": "guid", "owner": "...", "repo": "...", "prNumber": "42", "reason": "string | null" }
```

### `POST /api/git/merge` — 🔐 (same as approve)

### `POST /api/git/jira-pomodoro` — 🔐
```json
{ "integrationId": "guid", "issueKey": "ATLAS-123", "domainUrl": "https://mycompany.atlassian.net", "durationMinutes": 25, "breakDurationMinutes": 5 }
```

---

## 18. Figma

### `GET /api/figma/{integrationId}/comments?fileKey=xxx` — 🔐
### `POST /api/figma/comments/resolve` — 🔐
```json
{ "integrationId": "guid", "fileKey": "...", "commentId": "..." }
```

---

## 19. Dribbble
### `GET /api/dribbble/{integrationId}/inspiration?query=dashboard` — 🔐

---

## 20. Sentry
### `GET /api/sentry/{integrationId}/issues?projectSlug=atlas-backend` — 🔐
### `GET /api/sentry/{integrationId}/issues/{issueId}` — 🔐
### `POST /api/sentry/issues/{issueId}/resolve` — 🔐
```json
{ "integrationId": "guid", "issueId": "string" }
```

---

## 21. SonarQube
### `GET /api/sonarqube/{integrationId}/quality?projectKey=atlas` — 🔐

---

## 22. LottieFiles
### `GET /api/lottiefiles/{integrationId}/search?query=loading` — 🔐

---

## 23. Knowledge (Notion)
### `GET /api/knowledge/notion` — 🔐

---

## 24. Gmail
### `GET /api/gmail/unread` — 🔐

---

## 25. Proactive AI Agents

### `POST /api/proactiveagents/explain-error` — 🔐
```json
{ "errorMessage": "NullReferenceException...", "stackTrace": "string | null" }
```

### `POST /api/proactiveagents/resolve-port` — `{ "port": 3000 }`
### `POST /api/proactiveagents/kill-idle-containers` — `{ "idleMinutes": 60 }`
### `POST /api/proactiveagents/suggest-commit` — `{ "diff": "git diff output..." }`
### `POST /api/proactiveagents/summarize-pr` — `{ "prUrl": "https://github.com/..." }`
### `POST /api/proactiveagents/watch-dependencies` — `{ "projectFilePath": "..." }`
### `POST /api/proactiveagents/search` — `{ "query": "how to fix CORS error" }`

---

## 26. SecOps Agents

### `POST /api/secopsagents/detect-rogue-ports` — 🔐 (no body)
### `POST /api/secopsagents/warn-expiring-ssl` — `{ "domains": ["example.com"] }`
### `POST /api/secopsagents/detect-suspicious-traffic` — `{ "targetUrl": "..." }`
### `POST /api/secopsagents/scan-leaked-keys` — `{ "content": "source code text" }`
### `POST /api/secopsagents/suggest-patches` — `{ "projectPath": "..." }`
### `GET /api/secopsagents/vpn-status` — 🔐

---

## 27. SecOps Insights
All: `?from=datetime&to=datetime`
- `GET /api/secopsinsights/threats-blocked`
- `GET /api/secopsinsights/vulnerabilities-patched`
- `GET /api/secopsinsights/avg-response-time`
- `GET /api/secopsinsights/security-score` (no date params)
- `GET /api/secopsinsights/zero-incident-streak` (no date params)
- `GET /api/secopsinsights/scanned-bytes`
- `GET /api/secopsinsights/open-ports-graph`

---

## 28. SecOps Scripts
- `POST /api/secopsscripts/quick-scan` — `{ "networkRange": "192.168.1.0/24" }`
- `POST /api/secopsscripts/phishing-alert` — `{ "emailHeaders": "...", "senderAddress": "..." }`
- `POST /api/secopsscripts/rotate-ssh` — `{ "keyComment": "deploy@atlas", "keySize": 4096 }`
- `POST /api/secopsscripts/clear-dns` — No body

---

## 29. SecOps Utilities
- `POST /api/secopsutilities/hash` — `{ "input": "text", "algorithm": "SHA256 | MD5 | SHA512" }`
- `POST /api/secopsutilities/ip-dns` — `{ "target": "example.com" }`
- `POST /api/secopsutilities/encode-payload` — `{ "input": "text", "encoding": "Base64 | Hex | URL" }`
- `POST /api/secopsutilities/password-entropy` — `{ "password": "MyP@ss!" }`
- `POST /api/secopsutilities/ssl-check` — `{ "domain": "google.com" }`
- `POST /api/secopsutilities/port-scan` — `{ "target": "localhost", "ports": [80, 443, 3000] }`

---

## 30. Dev Insights
All: `?from=datetime&to=datetime`
- `GET /api/devinsights/time-saved`
- `GET /api/devinsights/focus-heatmap`
- `GET /api/devinsights/tech-debt?projectPath=/path`
- `GET /api/devinsights/deployment-success-rate`
- `GET /api/devinsights/peak-hours`

---

## 31. Dev Utilities
- `POST /api/devutilities/decode-jwt` — `{ "token": "eyJhbGci..." }`
- `POST /api/devutilities/test-regex` — `{ "pattern": "^[a-z]+$", "testString": "hello", "flags": "i | null" }`
- `POST /api/devutilities/generate-cron` — `{ "description": "Every 5 minutes" }`
- `POST /api/devutilities/base64` — `{ "input": "Hello", "encode": true }`
- `POST /api/devutilities/ssh-key` — `{ "comment": "deploy@atlas", "keySize": 4096 }`
- `POST /api/devutilities/json/format` — `{ "json": "{\"key\":\"value\"}" }`
- `POST /api/devutilities/network/send-request` — `{ "url": "...", "method": "GET", "headers": {}, "body": "" }`
- `POST /api/devutilities/security/scan-dependencies` — `{ "projectFilePath": "..." }`
- `GET /api/devutilities/system/check-port/{port}`
- `DELETE /api/devutilities/system/kill-process/{pid}`

---

## 32. Design Insights
- `GET /api/designinsights/assets-optimized`
- `GET /api/designinsights/handoffs?from=...&to=...`
- `GET /api/designinsights/color-trends`
- `GET /api/designinsights/design-debt`

---

## 33. Design Utilities
- `POST /api/designutilities/compress-image` — `{ "filePath": "...", "quality": 75 }`
- `POST /api/designutilities/convert-asset` — **FormData** `{ file: File, targetFormat: "png | webp | svg" }` → binary file
- `POST /api/designutilities/optimize-svg` — `{ "svgContent": "<svg>...</svg>" }`
- `POST /api/designutilities/extract-css` — `{ "colors": [{ "name": "primary", "hex": "#4ade80" }], "format": "css | scss | tailwind" }`
- `POST /api/designutilities/check-contrast` — `{ "foreground": "#fff", "background": "#000" }`
- `GET /api/designutilities/aspect-ratio?width=1920&height=1080`
- `GET /api/designutilities/dummy-data?type=user&count=10`
- `GET /api/designutilities/palettes`
- `POST /api/designutilities/palettes` — `{ "name": "Brand Colors" }`
- `POST /api/designutilities/palettes/{id}/colors` — `{ "paletteId": "guid", "name": "Primary", "hexCode": "#4ade80" }`

---

## 34. Leader Agents
- `GET /api/leaderagents/bottleneck/{teamId}`
- `GET /api/leaderagents/burnout-risk/{teamId}`
- `GET /api/leaderagents/scope-creep/{teamId}?sprintId=xxx`
- `POST /api/leaderagents/pr-review-nag` — `{ "teamId": "guid", "thresholdHours": 24 }`
- `GET /api/leaderagents/unassigned-bugs/{teamId}`
- `POST /api/leaderagents/ghost-members` — `{ "teamId": "guid" }`
- `GET /api/leaderagents/milestone/{teamId}`

---

## 35. Leader Insights
All: `?teamId=guid&from=datetime&to=datetime`
- `GET /api/leaderinsights/sprint-velocity`
- `GET /api/leaderinsights/meetings-avoided`
- `GET /api/leaderinsights/blocked-time`
- `GET /api/leaderinsights/cost-per-feature`
- `GET /api/leaderinsights/review-turnaround`
- `GET /api/leaderinsights/top-contributor`
- `GET /api/leaderinsights/team-mood`

---

## 36. Leader Scripts
- `POST /api/leaderscripts/sprint-starter` — `{ "sprintName": "Sprint 5", "initialTasks": ["Task 1"], "teamId": "guid" }`
- `POST /api/leaderscripts/blocked-task-blaster` — `{ "teamId": "guid" }`
- `POST /api/leaderscripts/release-notes` — `{ "repoName": "atlas", "fromTag": "v1.0", "toTag": "v2.0" }`
- `POST /api/leaderscripts/meeting-mode` — `{ "durationMinutes": 30 }`
- `POST /api/leaderscripts/week-summary` — `{ "teamId": "guid" }`
- `POST /api/leaderscripts/bulk-reassign` — `{ "absentMemberId": "guid", "teamId": "guid" }`
- `POST /api/leaderscripts/standup-ping` — `{ "teamId": "guid" }`

---

## 37. Leader Utilities
- `POST /api/leaderutilities/timezones`
- `POST /api/leaderutilities/quick-poll` — `{ "question": "Which?", "options": ["React", "Vue"] }`
- `POST /api/leaderutilities/capacity`
- `POST /api/leaderutilities/cost-estimate`
- `POST /api/leaderutilities/risk-matrix` — `{ "items": [{ "name": "DB Failure", "likelihood": 3, "impact": 5 }] }`
- `POST /api/leaderutilities/decision-log` — `{ "decision": "Use PostgreSQL", "rationale": "...", "decidedBy": "John" }`
- `POST /api/leaderutilities/markdown` — `{ "markdown": "# Hello" }` → `{ "data": { "html": "<h1>Hello</h1>" } }`

---

## 38. Leader Modals
- `GET /api/leadermodals`
- `GET /api/leadermodals/{modalId}/payload`
- `POST /api/leadermodals` — `{ "modalType": "SprintKickoff | Retrospective", "teamId": "guid | null", "payloadJson": "string | null" }`
- `POST /api/leadermodals/{modalId}/dismiss` → 204

---

## 39. Modals
- `GET /api/modals/pending`
- `POST /api/modals/{modalId}/dismiss`

---

## 40. Global Shortcuts (Cmd+K)
- `GET /api/globalshortcuts/command-palette?search=docker`
- `POST /api/globalshortcuts/ai-context` — `{ "selectedContent": "const x = ...", "action": "Explain | Refactor | Translate | Summarize" }`
- `POST /api/globalshortcuts/capture` — `{ "content": "Quick note", "title": "string | null", "url": "string | null" }`
- `POST /api/globalshortcuts/share` — `{ "content": "...", "channel": "Email | Slack | Clipboard", "recipientEmail": "string | null", "slackChannel": "string | null" }`
- `POST /api/globalshortcuts/calendar-event` — `{ "text": "Meeting with John tomorrow at 3pm" }`

---

## 41. Search
### `GET /api/search?q=docker&limit=5` — 🔐

---

## 42. Hotkeys
- `GET /api/hotkeys`
- `POST /api/hotkeys` — `{ "action": "openCommandPalette", "keyCombination": "Cmd+K", "isGlobal": true }`
- `DELETE /api/hotkeys/{hotkeyId}`
- `POST /api/hotkeys/seed-defaults` → `{ "data": { "createdCount": 12 } }`

---

## 43. Greeting
- `GET /api/greeting?userName=Samir&timezoneOffsetMinutes=-240&lang=en` — **Public**
- `POST /api/greeting` — `{ "userName": "Samir", "timezoneOffsetMinutes": -240, "lang": "en" }`

---

## 44. Subscriptions & Billing
- `GET /api/subscriptions/current`
- `GET /api/subscriptions/usage`
- `POST /api/subscriptions/checkout` — `{ "tier": "Pro | Team", "successUrl": "...", "cancelUrl": "..." }` → `{ "data": { "url": "stripe_checkout_url" } }`
- `POST /api/subscriptions/portal` — `{ "returnUrl": "..." }` → `{ "data": { "url": "stripe_portal_url" } }`
- `POST /api/subscriptions/cancel` → `{ "data": true }`
- `GET /api/subscriptions/invoices`

---

## 45. Personal Access Tokens
- `GET /api/personaltokens`
- `POST /api/personaltokens`
```json
{ "name": "CI/CD Token", "scopes": ["read:projects", "write:scripts"], "expiresAt": "datetime | null" }
// Response (⚠️ token ONLY returned once!)
{ "data": { "id": "guid", "name": "CI/CD Token", "token": "atlas_xYz123...", "scopes": [...], "expiresAt": "datetime" } }
```
- `POST /api/personaltokens/{id}/revoke` → 204

---

## 46. Audit Logs
- `GET /api/auditlogs?action=Login&from=datetime&to=datetime&page=1&pageSize=50`
- `GET /api/auditlogs/sessions` → Active sessions list

---

## 47. Webhooks (Outgoing)
- `GET /api/webhooks`
- `POST /api/webhooks` — `{ "name": "Slack", "url": "https://hooks.slack.com/...", "secret": "string | null", "events": ["AlertCreated", "PrApproved", "FocusCompleted"], "workspaceId": "guid | null" }`
- `PUT /api/webhooks/{id}` — `{ "webhookId": "guid", "name": "...", "url": "...", "secret": "...", "events": [...] }`
- `POST /api/webhooks/{id}/toggle` — `{ "active": true }`
- `DELETE /api/webhooks/{id}`

---

## 48. Support & Feedback
- `GET /api/support`
- `POST /api/support` — `{ "type": "Bug | Feature | Question | Feedback", "subject": "...", "body": "...", "pageUrl": "string | null", "browserInfo": "string | null" }`
- `POST /api/support/{id}/close` → 204

---

## 49. Squad Radar
- `GET /api/squadradar/{teamId}`
- `PUT /api/squadradar/presence` — `{ "teamId": "guid", "status": "Online | InMeeting | Focusing | Away | Offline", "toolIcon": "vscode | null", "focus": "string | null", "meetingMinutesLeft": 15 }`

---

## 50. Squad Arena (Gamification)
- `GET /api/squadarena/leaderboard/{teamId}`
- `GET /api/squadarena/bounties/{teamId}`
- `POST /api/squadarena/badge` — `{ "teamId": "guid", "userId": "guid", "badgeType": "BugSlayer | SpeedRunner | CodeReviewer", "points": 100, "sprintId": "string | null" }`
- `POST /api/squadarena/bounty` — `{ "teamId": "guid", "title": "Fix flaky test", "description": "string | null", "rewardPoints": 50, "jiraIssueKey": "string | null" }`
- `POST /api/squadarena/bounty/{bountyId}/claim` → 204
- `POST /api/squadarena/bounty/{bountyId}/complete` → 204

---

## 51. Resource Hub
- `GET /api/resourcehub/{teamId}?category=Documentation|Design|DevOps|API|Other`
- `POST /api/resourcehub` — `{ "teamId": "guid", "title": "...", "url": "...", "category": "Documentation", "description": "string | null" }`
- `PUT /api/resourcehub` — `{ "resourceId": "guid", "title": "...", "url": "...", "category": "...", "description": "..." }`
- `DELETE /api/resourcehub/{resourceId}`
- `POST /api/resourcehub/{resourceId}/pin`

---

## 52. SignalR Real-Time Events

**Hub URL:** `ws://localhost:5000/hubs/atlas?access_token=JWT_TOKEN`

### Client → Server
- `JoinTeam(teamId)` — Team qrupuna qoşul
- `LeaveTeam(teamId)` — Team qrupundan çıx

### Server → Client
| Event | Payload | Tetikleyici |
|-------|---------|-------------|
| `NotificationReceived` | `{ id, title, body, category, priority, timestamp }` | Hangfire background jobs |
| `ReceiveAlert` | `{ alertType, payload, timestamp }` | Team alert |
| `PresenceUpdated` | `{ payload, timestamp }` | Squad Radar dəyişiklik |
| `FocusStateChanged` | `{ payload, timestamp }` | Focus session dəyişiklik |
| `JobCompleted` | `{ jobType, payload, timestamp }` | Background job bitdi |
| `FeedUpdated` | `{ eventType, payload, timestamp }` | OmniFeed yeni item |

---

## 53. Hangfire Background Jobs

| Job | Interval | Funksiya |
|-----|----------|----------|
| `system-health-check` | 5 dəq | CPU/RAM/Battery → AI analiz → Notification + SignalR |
| `docker-health-check` | 15 dəq | Docker container sağlamlığı → Notification |
| `daily-insights` | Hər gün 09:00 | Focus report → Notification |

**Dashboard:** `http://localhost:5000/hangfire` (dev only)

---

## 📝 Qeydlər

- **Tarixlər:** UTC format `2026-03-02T12:00:00Z`
- **Guid:** `"a1b2c3d4-e5f6-7890-abcd-ef1234567890"`
- **Enum-lar:** String olaraq göndərilir (StringEnumConverter aktiv)
- **Rate Limiting:** Register, Login, Password reset ayrı limitlər
- **Pagination:** `page` (1-based), `pageSize`
- **Validation:** FluentValidation → `{ "errors": ["Name is required"] }`
- **401:** Token expired/invalid
- **403:** İcazəsiz (məs: TeamLeader policy)
- **404:** `{ "isSuccess": false, "errors": ["Resource not found"] }`

