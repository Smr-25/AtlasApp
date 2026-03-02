# ⚡ ATLAS — Developer & Team Leader Workspace Platform

**ATLAS** is a unified SaaS platform built for developers and team leaders that consolidates workspace management, integrations, AI-powered agents, DevOps monitoring, security operations, and team collaboration into a single application.

Built with **.NET 10** and **C# 14** following **Clean Architecture** (CQRS + MediatR).

---

## 🏗️ Architecture

```
Atlas.sln
├── Core/
│   ├── Atlas.Domain          → Entities, Enums, Abstractions
│   └── Atlas.Application     → CQRS Commands/Queries, DTOs, Interfaces, Validators
├── Infrastructure/
│   ├── Atlas.Infrastructure   → Service implementations, External API adapters
│   └── Atlas.Persistence      → EF Core DbContext, Configurations, Migrations
└── Presentation/
    └── Atlas.WebAPI           → Controllers, Hubs, Middleware, Background Jobs
```

**Key Technologies:**
- .NET 10 / C# 14
- Entity Framework Core + PostgreSQL
- MediatR (CQRS pattern)
- FluentValidation
- ASP.NET Core Identity + JWT Bearer Authentication
- SignalR (real-time events)
- Hangfire (background jobs)
- AutoMapper
- Stripe (subscriptions & billing)
- MailKit (email), Twilio (SMS), Telegram.Bot
- Docker.DotNet (container management)
- Roslyn (code analysis)

---

## 🔑 Core Concepts

### Workspace-Centric Design
Everything in ATLAS revolves around **Workspaces**. A workspace is an isolated context that can be linked to a local folder on disk, have its own set of enabled integrations, team members with roles (Owner, Admin, Editor, Viewer), and projects.

### Integration Toggling
Users connect integrations once (GitHub, Figma, Sentry, Jira, etc.), then toggle them per workspace. A freelance workspace might only need GitHub + Figma, while a DevOps workspace enables Sentry + SonarQube + Docker.

### Smart Inbox (Notifications)
Notifications are categorized into 4 lanes:
- 🔴 **Alerts & SecOps** — Critical security/system alerts with actionable buttons
- 🟢 **Approvals & Git** — PR reviews, merge requests
- 🔵 **Mentions & Social** — Figma comments, team mentions
- 🟣 **System & Insights** — Reports, AI advice, weekly digests

### AI-Powered Background Jobs
Hangfire runs periodic jobs that analyze system health, Docker containers, and daily productivity — pushing results via SignalR in real-time.

---

## 📦 Feature Modules

| Module | Description |
|--------|-------------|
| **Accounts** | Register, Login, OAuth (GitHub/Google), Email/Phone verification, Telegram linking |
| **Onboarding** | Post-registration questionnaire to configure user profile |
| **Profiles** | User profile with job title, bio, theme color |
| **Preferences** | Language, theme, timezone, notification toggles, weekly digest |
| **Workspaces** | CRUD, member management, role-based access, folder validation, integration toggling |
| **Integrations** | Connect/disconnect 12+ providers, reconnect expired tokens, workspace scoping |
| **Teams** | Create teams, invite members, share workspaces, team dashboard |
| **Team Info** | Sprint objectives, member focus status, team armory (staging URLs, credentials), vault links |
| **Projects** | Link local projects, run EF Core migrations from UI |
| **Focus (Pomodoro)** | Timed sessions with pause/resume/interrupt, stats, history, workspace-scoped |
| **Notifications** | Smart Inbox with 4 categories, actionable items, bulk operations |
| **OmniFeed** | Team activity feed with emoji reactions |
| **Snippets** | Code snippet manager with Notion sync, favorites |
| **Scripts** | Custom script runner + 8 pre-built scripts (spin env, nuke-migrate, flush cache, etc.) |
| **Docker** | Container list, logs, start/stop/restart |
| **System Monitor** | AI-analyzed system health (CPU, RAM, battery, top processes) |
| **Git / GitHub** | PR dashboard, approve/reject/merge from within ATLAS |
| **Figma** | View and resolve design comments |
| **Sentry** | View and resolve error tracking issues |
| **SonarQube** | Project code quality metrics |
| **Dribbble** | Design inspiration search |
| **LottieFiles** | Animation asset search |
| **Knowledge** | Notion page browser |
| **Gmail** | Unread email list |
| **Proactive AI Agents** | Error explanation, commit message suggestion, PR summary, dependency watch |
| **SecOps Agents** | Rogue port detection, SSL expiry, leaked key scan, VPN status |
| **SecOps Insights** | Security score, threats blocked, zero-incident streak, vulnerability patches |
| **SecOps Scripts** | Quick network scan, phishing alert, SSH rotation, DNS flush |
| **SecOps Utilities** | Hash generation, IP/DNS lookup, SSL check, port scan, password entropy |
| **Dev Insights** | Time saved, focus heatmap, tech debt, deployment success rate, peak hours |
| **Dev Utilities** | JWT decode, regex tester, cron generator, Base64, SSH key gen, JSON formatter, HTTP client |
| **Design Insights** | Asset optimization stats, handoff tracking, color trends, design debt |
| **Design Utilities** | Image compression, SVG optimization, contrast checker, aspect ratio, dummy data, color palettes |
| **Leader Agents** | Bottleneck prediction, burnout risk, scope creep, PR nag, ghost member ping |
| **Leader Insights** | Sprint velocity, blocked time, cost per feature, review turnaround, team mood |
| **Leader Scripts** | Sprint starter, release notes, meeting mode, week summary, bulk reassign, standup ping |
| **Leader Utilities** | Timezone converter, quick poll, capacity calc, cost estimate, risk matrix, decision log, markdown render |
| **Leader Modals** | Context-aware modals for sprint kickoff, retrospective |
| **Global Shortcuts** | Cmd+K command palette, AI context actions, quick capture, calendar event parsing |
| **Search** | Global search across all entities |
| **Hotkeys** | Customizable keyboard shortcuts |
| **Greeting** | Localized time-of-day greeting |
| **Subscriptions** | Stripe integration — checkout, portal, invoices, usage tracking |
| **Personal Tokens** | API key management for CLI/CI-CD integration |
| **Audit Logs** | Activity history, active sessions |
| **Webhooks** | Outgoing webhook management for external notifications |
| **Support** | In-app bug reports and feedback tickets |
| **Squad Radar** | Real-time team presence (online, focusing, in meeting, away) |
| **Squad Arena** | Gamification — leaderboard, badges, bounty board |
| **Resource Hub** | Shared team resource links (docs, APIs, designs) |

---

## 🔐 Authentication & Authorization

- **JWT Bearer Tokens** with refresh token rotation
- **OAuth 2.0** — GitHub and Google providers
- **ASP.NET Core Identity** with email/phone verification
- **Telegram Bot** linking for 2FA and notifications
- **Role-based policies** — `TeamLeaderOnly` for sensitive team endpoints
- **Workspace roles** — Owner, Admin, Editor, Viewer with permission checks
- **Rate Limiting** — Separate limits for auth, verification, and API endpoints
- **Personal Access Tokens** — SHA256-hashed, prefix-based, scoped API keys

---

## ⚡ Real-Time (SignalR)

**Hub:** `/hubs/atlas`

| Event | Description |
|-------|-------------|
| `NotificationReceived` | New notification pushed from background jobs |
| `ReceiveAlert` | Team-level alert |
| `PresenceUpdated` | Squad Radar member status change |
| `FocusStateChanged` | Focus session state update |
| `JobCompleted` | Background job completion |
| `FeedUpdated` | New OmniFeed item |

---

## 🕐 Background Jobs (Hangfire)

| Job | Interval | Purpose |
|-----|----------|---------|
| System Health Check | Every 5 min | CPU/RAM/Battery analysis → AI advice → Notification |
| Docker Health Check | Every 15 min | Container health → Notification on issues |
| Daily Insights | Daily 09:00 | Focus session summary → Notification |

---

## 🚀 Getting Started

```bash
# Prerequisites
# - .NET 10 SDK
# - PostgreSQL
# - Docker (optional, for container management features)

# Clone and run
cd src/Backend/Atlas
dotnet restore
dotnet ef database update --project Infrastructure/Atlas.Persistence --startup-project Presentation/Atlas.WebAPI
dotnet run --project Presentation/Atlas.WebAPI
```

**API:** `http://localhost:5000`  
**Scalar Docs:** `http://localhost:5000/scalar/v1`  
**Hangfire Dashboard:** `http://localhost:5000/hangfire`

---

## 📄 Configuration

Key settings in `appsettings.json`:
- `ConnectionStrings:DefaultConnection` — PostgreSQL
- `JwtSettings` — Token secrets and expiration
- `ExternalAuthSettings` — GitHub/Google OAuth credentials
- `StripeSettings` — Stripe API keys and price IDs
- `EmailSettings` — SMTP configuration
- `SmsSettings` — Twilio credentials
- `TelegramSettings` — Bot token
- `AiSettings` — AI service configuration
- `NotionSettings` — Notion API key

---

*Built with Clean Architecture, CQRS, and love for developer productivity.*

