<p align="center">
  <img src="https://img.shields.io/badge/ATLAS-SaaS%20Platform-f97316?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCIgZmlsbD0id2hpdGUiPjxwYXRoIGQ9Ik0xMiAyTDIgMTloMjBMMTIgMnptMCAxNy41Yy0uNTUgMC0xLS40NS0xLTFzLjQ1LTEgMS0xIDEgLjQ1IDEgMXMtLjQ1IDEtMSAxem0xLTNoLTJ2LTJoMnYyem0wLTRoLTJWN2gydjUuNXoiLz48L3N2Zz4=&logoColor=white" alt="ATLAS" />
</p>

<h1 align="center">🚀 ATLAS — SaaS Team Productivity Platform</h1>

<p align="center">
  <strong>4 fərqli ixtisas qrupu üçün nəzərdə tutulmuş, hər bir rolun özünəməxsus dashboard, AI agent, insight, utility və inteqrasiyalara sahib olduğu tam funksional SaaS platforması.</strong>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET%209-512BD4?style=flat-square&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/React%2019-61DAFB?style=flat-square&logo=react&logoColor=black" />
  <img src="https://img.shields.io/badge/TypeScript-3178C6?style=flat-square&logo=typescript&logoColor=white" />
  <img src="https://img.shields.io/badge/PostgreSQL-4169E1?style=flat-square&logo=postgresql&logoColor=white" />
  <img src="https://img.shields.io/badge/TailwindCSS-06B6D4?style=flat-square&logo=tailwindcss&logoColor=white" />
  <img src="https://img.shields.io/badge/SignalR-512BD4?style=flat-square&logo=dotnet&logoColor=white" />
</p>

---

## 📋 Haqqında

**ATLAS** — Developer, Designer, CyberSecurity (SecOps) və Team Leader/Product Manager rolları üçün hazırlanmış enterprise-grade SaaS platformasıdır. Hər ixtisas öz dashboard-una, AI agentlərinə, alətlərinə və inteqrasiyalarına sahibdir.

## 🏗 Arxitektura

```
ATLAS/
├── Core/
│   ├── Atlas.Domain          → Entities, Enums, Domain Events
│   └── Atlas.Application     → CQRS (MediatR), FluentValidation, DTOs
├── Infrastructure/
│   ├── Atlas.Infrastructure  → AI, Email, SMS, Telegram, Stripe adapters
│   └── Atlas.Persistence     → EF Core, DbContext, Configurations
├── Presentation/
│   └── Atlas.WebAPI          → Controllers, SignalR Hubs, Middlewares
└── Frontend/
    └── AtlasUI               → React + TypeScript + Vite + TailwindCSS
```

**Pattern:** Clean Architecture + CQRS with MediatR

## 🔧 Texnologiya Steki

### Backend
| Texnologiya | Məqsəd |
|---|---|
| .NET 8 / ASP.NET Core | Backend framework |
| Entity Framework Core | ORM, Database (PostgreSQL) |
| MediatR | CQRS pattern |
| FluentValidation | Request validation |
| ASP.NET Identity | User management |
| JWT Bearer | Authentication |
| SignalR | Real-time communication |
| Stripe | Subscription / payment |
| OpenAI API | AI agent features |
| Twilio | SMS verification |
| Hangfire | Background jobs |

### Frontend
| Texnologiya | Məqsəd |
|---|---|
| React 18 | UI framework |
| TypeScript | Type safety |
| Vite | Build tool |
| TailwindCSS | Styling |
| shadcn/ui | Component library |
| Framer Motion | Animations |
| Axios | HTTP client |
| @microsoft/signalr | Real-time events |
| Recharts | Data visualization |
| Lucide React | Icons |

## 🎨 Role-Native UI (Rola Uyğun İnterfeys)

Hər ixtisas öz atmosferinə uyğun tema alır:

| Rol | Rəng | Atmosfer |
|---|---|---|
| 💻 **Developer** | Mavi tonlar | VS Code / Terminal — dark mode, monospace |
| 🎨 **Designer** | Qırmızı tonlar | Figma / Canvas — light mode, minimalist |
| 🛡️ **SecOps** | Matrix yaşılı | Radar / Terminal — dark matrix, real-time |
| 👑 **Team Leader** | Sarı tonlar | Executive dashboard — structured, charts |

**Əsas brend rəngi:** 🟠 Narıncı (`#f97316`)

## 👤 5 İxtisas Dashboard-u

### 💻 Developer Dashboard
- **DevInsights** — Time saved, focus heatmap, tech debt, deploy success rate, peak hours
- **DevUtilities** — JWT decoder, regex tester, cron generator, base64, SSH key generator, JSON formatter, HTTP client, vulnerability scanner, port checker, process killer
- **AI Agents** — Error explainer, port resolver, commit suggester, PR summarizer, dependency watcher, Perplexity search
- **Scripts** — Spin environment, resolve conflicts, nuke migrate, flush cache, format lint, kill nodes, generate boilerplate
- **Snippets** — Code vault with Notion sync
- **Focus/Pomodoro** — Deep work sessions with stats
- **Docker** — Container management
- **GitHub** — PR review, approve, reject, merge
- **Sentry** — Error tracking
- **SonarQube** — Code quality

### 🎨 Designer Dashboard
- **DesignInsights** — Assets optimized, handoffs, color trends, design debt
- **DesignUtilities** — Image compress, asset convert, SVG optimize, CSS extract, contrast check, aspect ratio, dummy data, palette manager
- **Figma** — Comments & resolve
- **Miro** — Boards & sticky notes
- **LottieFiles** — Animation search
- **Dribbble** — Design inspiration
- **Zeplin** — Screens & style guide

### 🛡️ SecOps Dashboard
- **SecOpsInsights** — Threats blocked, vulnerabilities patched, avg response time, security score, zero-incident streak, scanned bytes, open ports graph
- **SecOpsUtilities** — Hash generator, IP/DNS lookup, payload encoder, password entropy, SSL check, port scan, MAC spoof
- **SecOpsAgents** — Rogue port detection, SSL expiry warning, suspicious traffic, leaked key scanner, patch suggestions, zombie process killer, VPN status
- **SecOpsScripts** — Quick scan, panic button, local wipe, phishing alert, SSH rotation, firewall lockdown, DNS clear

### 👑 Team Leader Dashboard
- **LeaderInsights** — Sprint velocity, meetings avoided, blocked time, cost per feature, review turnaround, top contributor, team mood
- **LeaderUtilities** — Timezone converter, quick poll, capacity planner, cost estimator, risk matrix, decision log, markdown renderer
- **LeaderAgents** — Bottleneck predictor, burnout risk, scope creep detector, PR review nag, unassigned bugs, ghost members, milestone celebration
- **LeaderScripts** — Sprint starter, blocked task blaster, release notes, meeting mode, week summary, bulk reassign, standup ping
- **LeaderModals** — Jira board, GitHub pulse, Slack channels, Notion docs, Calendar sync, Sentry feed, PagerDuty on-call

## 🔌 İnteqrasiyalar

| Provider | Kateqoriya |
|---|---|
| GitHub | Developer |
| Jira | Developer/Leader |
| Notion | Developer/Leader |
| Figma | Designer |
| Miro | Designer |
| LottieFiles | Designer |
| Dribbble | Designer |
| Zeplin | Designer |
| Sentry | Monitoring |
| SonarQube | Code Quality |

## 🌐 Ortaq Funksionallıqlar (Bütün Rollara Aid)

- **Workspace Management** — Multi-workspace, integration toggle, folder validation
- **Teams** — Create team, invite members, share workspace, squad radar, squad arena (gamification)
- **OmniFeed** — Real-time activity stream
- **Focus/Pomodoro** — Deep work sessions
- **Snippets** — Code vault with Notion sync
- **Notifications** — Smart inbox (4 category: Alerts, Approvals, Mentions, System)
- **Settings** — Profile, preferences, security, tokens, webhooks, billing, support
- **Command Palette** — Cmd+K global search
- **Zen Mode** — Distraction-free workspace

## 🔐 Auth Sistemi

- JWT Bearer Token + Refresh Token rotation
- Google & GitHub OAuth (server-side flow)
- Email & Phone verification
- Password reset flow (3-step)
- Rate limiting (register, login, password-reset, verification)
- Telegram bot integration

## 📊 API Statistikası

| Bölmə | Endpoint Sayı |
|---|---|
| Accounts & Onboarding | 24 |
| Workspaces & Integrations | 20 |
| Ortaq Endpointlər | 89 |
| Developer Dashboard | 62 |
| Designer Dashboard | 22 |
| SecOps Dashboard | 28 |
| Team Leader Dashboard | 32 |
| **TOPLAM** | **~315** |

## 🚀 Başlama

### Backend
```bash
cd src/Presentation/Atlas.WebAPI
dotnet ef database update --project ../Infrastructure/Atlas.Persistence
dotnet run
# → http://localhost:5075
# → Scalar API Docs: http://localhost:5075/scalar/v1
```

### Frontend
```bash
cd src/Frontend/AtlasUI
bun install   # və ya npm install
bun dev       # və ya npm run dev
# → http://localhost:8080
```

### Mühit Dəyişənləri
Backend `appsettings.json` faylında konfiqurasiya olunur:
- PostgreSQL connection string
- JWT secret & issuer
- Google/GitHub OAuth credentials
- OpenAI API key
- Twilio credentials
- Stripe API keys
- SMTP settings

## 📝 Lisenziya

Bu layihə Code Academy Final Project olaraq hazırlanmışdır.

---

<p align="center">
  <strong>ATLAS</strong> — Clean Architecture + CQRS, ~315 endpoint, 5 role-adaptive dashboard, real-time SignalR, AI-powered.
</p>

