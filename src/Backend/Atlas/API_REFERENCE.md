# Atlas API Reference — Full Controller Endpoint Index

Aşağıda layihənin `Presentation/Atlas.WebAPI/Controllers` qovluğundakı bütün controller-lərin endpointlərinin qısa və aydın xülasəsi verilib. Hər bir entry: HTTP verb, tam route (base prefix `/api` əsas götürülmüşdür), və əsas parametrlər (route/query/body) qeyd olunur.

Qeyd: Base route konvensiyası: Controller-lərin əksəriyyətində `ApiControllerBase` üzərindən `[Route("api/[controller]")]` istifadə olunur -> `/api/{controllerNameWithoutController}`. Xüsusi `[Route(...)]` atributu olan controller-lərdə həmin route dəqiq verilib.

--

Index (alfabetik, controller adı -> bölməyə keçid)

- AccountsController
- ApiControllerBase (helpers)
- AwsController
- DevUtilitiesController
- DevInsightsController
- DesignController
- DesignInsightsController
- DesignUtilitiesController
- DockerController
- DribbbleController
- FigmaController
- FocusController
- Framework / System controllers (GreetingController, SystemController, SystemToolsController)
- GitController
- GmailController
- GlobalShortcutsController
- Goal/Team controllers (TeamInfoController, TeamsController, SquadRadarController, SquadArenaController)
- IntegrationsController
- JsonToolsController
- KnowledgeController
- LeaderAgentsController
- LeaderInsightsController
- LeaderModalsController
- LeaderScriptsController
- LeaderUtilitiesController
- LottieFilesController
- MarketerAgentsController
- MarketerInsightsController
- MarketerScriptsController
- MarketerUtilitiesController
- MiroController
- ModalsController
- NetworkToolsController
- OmniFeedController
- OnboardingController
- PalettesController
- ProjectsController
- ProactiveAgentsController
- ProfilesController
- ResourceHubController
- SentryController
- SecurityToolsController
- SecOpsAgentsController
- SecOpsInsightsController
- SecOpsScriptsController
- SecOpsUtilitiesController
- Sent (Stripe) Webhook (StripeWebhookController)
- SnippetsController
- Social / SubscriptionsController
- SonarQubeController
- ScriptsController
- SystemController
- TeamInfoController
- TeamsController
- SubscriptionsController
- WorkspacesController

--

Notes on notation:
- {id}, {teamId:guid}, etc — route parameters
- ?param= x — query parameters
- [body] — JSON body (object/command)

-------------------------------------------------------------------------------

AccountsController (Base: /api/accounts)
- POST /api/accounts/register [body: RegisterCommand]
- POST /api/accounts/login [body: LoginCommand]
- POST /api/accounts/external-login [body: ExternalLoginCommand]
- POST /api/accounts/logout [body: LogoutCommand]
- POST /api/accounts/forgot-password [body: ForgotPasswordCommand]
- POST /api/accounts/verify-reset-code [body: VerifyResetCodeCommand]
- POST /api/accounts/reset-password [body: ResetPasswordCommand]
- POST /api/accounts/verify-email [body: VerifyEmailCommand]
- POST /api/accounts/verify-phone [body: VerifyPhoneCommand]
- POST /api/accounts/resend-email-verification-code [body: ResendEmailVerificationCommand]
- POST /api/accounts/resend-phone-verification-code [body: ResendPhoneVerificationCommand]
- POST /api/accounts/refresh-token [body: RefreshTokenCommand]
- POST /api/accounts/revoke-refresh-token [body: RevokeAllTokenCommand]
- GET  /api/accounts/profile [auth] -> GetProfileQuery (no params)
- PUT  /api/accounts/profile [auth, body: UpdateProfileCommand]
- PUT  /api/accounts/change-password [auth, body: ChangePasswordCommand]
- POST /api/accounts/add-phone-number [auth, body: AddPhoneNumberCommand]
- DELETE /api/accounts/delete-account [auth]
- POST /api/accounts/set-telegram-chat-id [auth, body: SetTelegramChatIdCommand]
- POST /api/accounts/generate-telegram-link-code [auth]
- GET  /api/accounts/external/{provider} [AllowAnonymous] (redirect to provider auth)
- GET  /api/accounts/external/callback/{provider}?code=... [AllowAnonymous]

ApiControllerBase
- Base route provider: [Route("api/[controller]")] — istifadəçi üçün standart response envelope helpers: OkResponse / CreatedResponse / NoContentResponse / BadRequestResponse / NotFoundResponse / UnauthorizedResponse / ForbiddenResponse

AwsController (Base: /api/aws)
- GET /api/aws/{integrationId}/deployments?serviceName=string
- GET /api/aws/{integrationId}/deployments/{deploymentId}/status

DevUtilitiesController (Base: /api/devutilities)
- POST /api/devutilities/decode-jwt [body: DecodeJwtQuery]
- POST /api/devutilities/test-regex [body: TestRegexQuery]
- POST /api/devutilities/generate-cron [body: GenerateCronQuery]
- POST /api/devutilities/base64 [body: ConvertBase64Command]
- POST /api/devutilities/ssh-key [body: GenerateSshKeyCommand]

DevInsightsController (Base: /api/devinsights)
- GET /api/devinsights/time-saved?from={ISO}&to={ISO}
- GET /api/devinsights/focus-heatmap?from={ISO}&to={ISO}
- GET /api/devinsights/tech-debt?projectPath=string
- GET /api/devinsights/deployment-success-rate?from={ISO}&to={ISO}
- GET /api/devinsights/peak-hours?from={ISO}&to={ISO}

DesignController (Base: /api/design)
- POST /api/design/convert [form-data: ConvertAssetCommand] -> returns File (stream)

DesignInsightsController (Base: /api/designinsights)
- GET /api/designinsights/assets-optimized
- GET /api/designinsights/handoffs?from={ISO}&to={ISO}
- GET /api/designinsights/color-trends
- GET /api/designinsights/design-debt

DesignUtilitiesController (Base: /api/designutilities)
- POST /api/designutilities/compress-image [body: CompressImageCommand]
- POST /api/designutilities/extract-css [body: ExtractCssVarsCommand]
- POST /api/designutilities/optimize-svg [body: OptimizeSvgCommand]
- POST /api/designutilities/check-contrast [body: CheckContrastQuery]
- GET  /api/designutilities/aspect-ratio?width=int&height=int
- GET  /api/designutilities/dummy-data?type=string&count=int

DockerController (Base: /api/docker)
- GET  /api/docker
- GET  /api/docker/{id}/logs
- POST /api/docker/{id}/start
- POST /api/docker/{id}/stop
- POST /api/docker/{id}/restart

DribbbleController (Base: /api/dribbble)
- GET /api/dribbble/{integrationId}/inspiration?query=string

FigmaController (Base: /api/figma)
- GET /api/figma/{integrationId}/comments?fileKey=string
- POST /api/figma/comments/resolve [body: ResolveFigmaCommentCommand]

FocusController (Base: /api/focus)
- POST /api/focus [body: LogSessionCommand] -> create focus session
- GET  /api/focus/stats
- GET  /api/focus/active
- POST /api/focus/{sessionId}/complete
- POST /api/focus/{sessionId}/pause
- POST /api/focus/{sessionId}/resume
- POST /api/focus/{sessionId}/interrupt
- GET  /api/focus/history?days=7

GreetingController (Base: /api/greeting)
- GET /api/greeting?userName=string&timezoneOffsetMinutes=int&lang=string [AllowAnonymous]
- POST /api/greeting [body: GreetingRequest] [AllowAnonymous]

SystemController (Base: /api/system)
- GET /api/system/ides
- GET /api/system/analyze

SystemToolsController (Base: /api/systemtools)
- GET /api/systemtools/check-port/{port}
- DELETE /api/systemtools/kill-process/{pid}

GitController (Base: /api/git)
- GET /api/git/dashboard/{integrationId}
- POST /api/git/approve [body: ApprovePrCommand]
- POST /api/git/reject [body: RejectPrCommand]
- POST /api/git/merge [body: MergePrCommand]
- POST /api/git/jira-pomodoro [body: StartJiraPomodoroCommand]

GmailController (Base: /api/gmail)
- GET /api/gmail/unread -> returns List<EmailDto>

GlobalShortcutsController (Base: /api/globalshortcuts)
- GET /api/globalshortcuts/command-palette?search=string
- POST /api/globalshortcuts/ai-context [body: ProcessAiContextCommand]
- POST /api/globalshortcuts/capture [body: CaptureToNotionCommand]
- POST /api/globalshortcuts/share [body: QuickShareCommand]
- POST /api/globalshortcuts/calendar-event [body: ParseCalendarEventCommand]

TeamInfoController (Base: /api/teaminfo)
- GET /api/teaminfo/{teamId}
- POST /api/teaminfo/{teamId}/objective [body: SetObjectiveRequest]
- PUT /api/teaminfo/{teamId}/my-focus [body: UpdateFocusRequest]
- PUT /api/teaminfo/{teamId}/armory [body: UpsertArmoryRequest]
- POST /api/teaminfo/{teamId}/vault-links [body: AddVaultLinkRequest]
- PUT /api/teaminfo/{teamId}/vault-links/{linkId} [body: UpdateVaultLinkRequest]
- DELETE /api/teaminfo/{teamId}/vault-links/{linkId}

TeamsController (Base: /api/teams)
- GET /api/teams/my
- POST /api/teams [body: CreateTeamCommand]
- GET /api/teams/{teamId}
- POST /api/teams/{teamId}/members [body: InviteMemberRequest]
- DELETE /api/teams/{teamId}/members/{userId}
- GET /api/teams/{teamId}/radar [Authorize: TeamLeaderOnly]
- GET /api/teams/{teamId}/productivity [Authorize: TeamLeaderOnly]
- POST /api/teams/{teamId}/share-workspace [body: ShareWorkspaceRequest]

SquadRadarController (Base: /api/squadradar)
- GET /api/squadradar/{teamId}
- PUT /api/squadradar/presence [body: UpdatePresenceCommand]

SquadArenaController (Base: /api/squadarena)
- GET /api/squadarena/leaderboard/{teamId}
- GET /api/squadarena/bounties/{teamId}
- POST /api/squadarena/badge [body: AwardBadgeCommand]
- POST /api/squadarena/bounty [body: CreateBountyCommand]
- POST /api/squadarena/bounty/{bountyId}/claim
- POST /api/squadarena/bounty/{bountyId}/complete

PalettesController (Base: /api/palettes)
- GET /api/palettes
- POST /api/palettes [body: CreatePaletteCommand]
- POST /api/palettes/{id}/colors [body: AddColorCommand]

ProjectsController (Base: /api/projects)
- POST /api/projects [body: CreateProjectCommand]
- POST /api/projects/{id}/migration [body: string? name]
- POST /api/projects/{id}/database-update [body: string? targetMigration]

IntegrationsController (Base: /api/integrations)
- GET /api/integrations
- GET /api/integrations/{id}
- POST /api/integrations [body: ConnectIntegrationCommand]
- PUT /api/integrations/{id} [body: UpdateIntegrationCommand]
- DELETE /api/integrations/{id}
- POST /api/integrations/{id}/reconnect [body: ReconnectIntegrationCommand]
- POST /api/integrations/{id}/mark-expired
- GET /api/integrations/pending

JsonToolsController (explicit Route: /api/jsontools)
- POST /api/jsontools/format [body: FormatJsonQuery]

KnowledgeController (Base: /api/knowledge)
- GET /api/knowledge/notion

LeaderInsightsController (Base: /api/leaderinsights)
- GET /api/leaderinsights/sprint-velocity?teamId={guid}&from={ISO}&to={ISO}
- GET /api/leaderinsights/meetings-avoided?teamId={guid}&from={ISO}&to={ISO}
- GET /api/leaderinsights/blocked-time?teamId={guid}&from={ISO}&to={ISO}
- GET /api/leaderinsights/cost-per-feature?teamId={guid}&from={ISO}&to={ISO}
- GET /api/leaderinsights/review-turnaround?teamId={guid}&from={ISO}&to={ISO}
- GET /api/leaderinsights/top-contributor?teamId={guid}&from={ISO}&to={ISO}
- GET /api/leaderinsights/team-mood?teamId={guid}&from={ISO}&to={ISO}

LeaderAgentsController (Base: /api/leaderagents)
- GET /api/leaderagents/bottleneck/{teamId}
- GET /api/leaderagents/burnout-risk/{teamId}
- GET /api/leaderagents/scope-creep/{teamId}?sprintId=string
- POST /api/leaderagents/pr-review-nag [body: NagPrReviewsCommand]
- GET /api/leaderagents/unassigned-bugs/{teamId}
- POST /api/leaderagents/ghost-members [body: PingGhostMembersCommand]
- GET /api/leaderagents/milestone/{teamId}

LeaderModalsController (Base: /api/leadermodals)
- GET /api/leadermodals
- GET /api/leadermodals/{modalId}/payload
- POST /api/leadermodals [body: OpenLeaderModalCommand]
- POST /api/leadermodals/{modalId}/dismiss

LeaderScriptsController (Base: /api/leaderscripts)
- POST /api/leaderscripts/sprint-starter [body: RunSprintStarterCommand]
- POST /api/leaderscripts/blocked-task-blaster [body: RunBlockedTaskBlasterCommand]
- POST /api/leaderscripts/release-notes [body: RunReleaseNoteGenCommand]
- POST /api/leaderscripts/meeting-mode [body: RunMeetingModeCommand]
- POST /api/leaderscripts/week-summary [body: RunEndOfWeekSummaryCommand]
- POST /api/leaderscripts/bulk-reassign [body: RunBulkReassignCommand]
- POST /api/leaderscripts/standup-ping [body: RunStandupPingCommand]

LeaderUtilitiesController (Base: /api/leaderutilities)
- POST /api/leaderutilities/timezones [body: ConvertTimezonesQuery]
- POST /api/leaderutilities/quick-poll [body: GenerateQuickPollCommand]
- POST /api/leaderutilities/capacity [body: CalculateCapacityQuery]
- POST /api/leaderutilities/cost-estimate [body: EstimateCostQuery]
- POST /api/leaderutilities/risk-matrix [body: GenerateRiskMatrixCommand]
- POST /api/leaderutilities/decision-log [body: CreateDecisionLogCommand]
- POST /api/leaderutilities/markdown [body: RenderMarkdownCommand]

LottieFilesController (Base: /api/lottiefiles)
- GET /api/lottiefiles/{integrationId}/search?query=string

MarketerAgentsController (Base: /api/marketeragents)
- POST /api/marketeragents/budget-bleed
- POST /api/marketeragents/broken-links [body: DetectBrokenLinksQuery]
- POST /api/marketeragents/viral-trends [body: GetViralTrendsQuery]
- POST /api/marketeragents/competitor-price-drop [body: DetectCompetitorPriceDropQuery]
- POST /api/marketeragents/resend-low-open [body: ResendLowOpenRateCommand]
- POST /api/marketeragents/auto-utm [body: AppendAutoUtmCommand]
- GET  /api/marketeragents/cart-abandonment

MarketerInsightsController (Base: /api/marketerinsights)
- GET /api/marketerinsights/total-roas?from={ISO}&to={ISO}
- GET /api/marketerinsights/leads-generated?from={ISO}&to={ISO}
- GET /api/marketerinsights/zombie-ads-killed?from={ISO}&to={ISO}
- GET /api/marketerinsights/ab-test-win-rate?from={ISO}&to={ISO}
- GET /api/marketerinsights/peak-engagement?from={ISO}&to={ISO}
- GET /api/marketerinsights/audience-sentiment?from={ISO}&to={ISO}
- GET /api/marketerinsights/time-saved-reporting?from={ISO}&to={ISO}

MarketerScriptsController (Base: /api/marketerscripts)
- POST /api/marketerscripts/pause-campaigns [body: RunCampaignPauserCommand]
- POST /api/marketerscripts/social-blast [body: RunSocialBlastCommand]
- POST /api/marketerscripts/weekly-report [body: RunWeeklyReportCommand]
- POST /api/marketerscripts/utm-link [body: RunUtmLinkSaverCommand]
- POST /api/marketerscripts/competitor-scrape [body: RunCompetitorScraperCommand]
- POST /api/marketerscripts/clear-cookies [body: RunClearBrowserCookieCommand]
- POST /api/marketerscripts/verify-emails [body: RunBulkEmailVerifierCommand]

MarketerUtilitiesController (Base: /api/marketerutilities)
- POST /api/marketerutilities/seo-check [body: CheckSeoMetaQuery]
- POST /api/marketerutilities/copywriting [body: GenerateCopywritingCommand]
- POST /api/marketerutilities/markdown-to-html [body: ConvertMarkdownToHtmlQuery]
- POST /api/marketerutilities/keyword-density [body: AnalyzeKeywordDensityQuery]
- POST /api/marketerutilities/readability [body: CalculateReadabilityQuery]
- POST /api/marketerutilities/emojis [body: SearchEmojisQuery]

MiroController (Base: /api/miro)
- GET /api/miro/{integrationId}/boards
- POST /api/miro/sticky [body: CreateMiroStickyCommand]

ModalsController (Base: /api/modals)
- GET /api/modals/pending
- POST /api/modals/{modalId}/dismiss

NetworkToolsController (Base: /api/networktools)
- POST /api/networktools/send-request [body: SendHttpRequestQuery]

OmniFeedController (Base: /api/omnifeed)
- GET /api/omnifeed/{teamId}?source={source}&page={page}&pageSize={pageSize}
- POST /api/omnifeed/publish [body: PublishManualItemCommand]
- POST /api/omnifeed/{itemId}/read
- POST /api/omnifeed/{itemId}/emoji [body: AddEmojiReactionCommand]

OnboardingController (Base: /api/onboarding)
- POST /api/onboarding/complete [body: CompleteOnboardingCommand]

PalettesController (Base: /api/palettes) — (see above)

ProjectsController (Base: /api/projects) — (see above)

ProactiveAgentsController (Base: /api/proactiveagents)
- POST /api/proactiveagents/explain-error [body: ExplainErrorCommand]
- POST /api/proactiveagents/resolve-port [body: ResolvePortConflictCommand]
- POST /api/proactiveagents/kill-idle-containers [body: KillIdleContainersCommand]
- POST /api/proactiveagents/suggest-commit [body: SuggestCommitMessageQuery]
- POST /api/proactiveagents/summarize-pr [body: SummarizePrQuery]
- POST /api/proactiveagents/watch-dependencies [body: WatchDependenciesQuery]

ProfilesController (Base: /api/profiles)
- GET /api/profiles/me
- PUT /api/profiles/me [body: UpdateUserProfileCommand]

ResourceHubController (Base: /api/resourcehub)
- GET /api/resourcehub/{teamId}?category={ResourceCategory}
- POST /api/resourcehub [body: AddResourceCommand]
- PUT /api/resourcehub [body: UpdateResourceCommand]
- DELETE /api/resourcehub/{resourceId}
- POST /api/resourcehub/{resourceId}/pin

SentryController (Base: /api/sentry)
- GET /api/sentry/{integrationId}/issues?projectSlug=string
- GET /api/sentry/{integrationId}/issues/{issueId}
- POST /api/sentry/issues/{issueId}/resolve [body: ResolveSentryIssueCommand]

SecurityToolsController (Base: /api/securitytools)
- POST /api/securitytools/scan-dependencies [body: ScanVulnerabilitiesQuery]

SecOpsAgentsController (Base: /api/secopsagents)
- POST /api/secopsagents/detect-rogue-ports
- POST /api/secopsagents/warn-expiring-ssl [body: WarnExpiringSslQuery]
- POST /api/secopsagents/detect-suspicious-traffic [body: DetectSuspiciousTrafficQuery]
- POST /api/secopsagents/scan-leaked-keys [body: ScanLeakedKeysCommand]
- POST /api/secopsagents/suggest-patches [body: SuggestAutoPatchesQuery]
- POST /api/secopsagents/kill-zombie-processes
- GET  /api/secopsagents/vpn-status

SecOpsInsightsController (Base: /api/secopsinsights)
- GET /api/secopsinsights/threats-blocked?from={ISO}&to={ISO}
- GET /api/secopsinsights/vulnerabilities-patched?from={ISO}&to={ISO}
- GET /api/secopsinsights/avg-response-time?from={ISO}&to={ISO}
- GET /api/secopsinsights/security-score
- GET /api/secopsinsights/zero-incident-streak
- GET /api/secopsinsights/scanned-bytes?from={ISO}&to={ISO}
- GET /api/secopsinsights/open-ports-graph?from={ISO}&to={ISO}

SecOpsScriptsController (Base: /api/secopsscripts)
- POST /api/secopsscripts/quick-scan [body: RunQuickScanCommand]
- POST /api/secopsscripts/panic-button [body: RunPanicButtonCommand]
- POST /api/secopsscripts/local-wipe [body: RunLocalWipeCommand]
- POST /api/secopsscripts/phishing-alert [body: RunPhishingAlertCommand]
- POST /api/secopsscripts/rotate-ssh [body: RunRotateSshCommand]
- POST /api/secopsscripts/firewall-lockdown [body: RunFirewallLockdownCommand]
- POST /api/secopsscripts/clear-dns

SecOpsUtilitiesController (Base: /api/secopsutilities)
- POST /api/secopsutilities/hash [body: GenerateHashCommand]
- POST /api/secopsutilities/ip-dns [body: IpDnsLookupQuery]
- POST /api/secopsutilities/encode-payload [body: EncodePayloadCommand]
- POST /api/secopsutilities/password-entropy [body: CalculatePasswordEntropyQuery]
- POST /api/secopsutilities/ssl-check [body: CheckSslQuery]
- POST /api/secopsutilities/port-scan [body: ScanLocalPortsQuery]
- POST /api/secopsutilities/spoof-mac [body: SpoofMacCommand]

SnippetsController (Base: /api/snippets)
- GET /api/snippets
- POST /api/snippets [body: CreateSnippetCommand]
- PUT /api/snippets/{snippetId} [body: UpdateSnippetCommand]
- DELETE /api/snippets/{snippetId}
- PATCH /api/snippets/{snippetId}/favorite
- POST /api/snippets/send-to-notion [body: SendSnippetToNotionCommand]
- POST /api/snippets/paste-from-notion [body: PasteFromNotionCommand]

SonarQubeController (Base: /api/sonarqube)
- GET /api/sonarqube/{integrationId}/quality?projectKey=string

ScriptsController (Base: /api/scripts)
- POST /api/scripts [body: CreateScriptCommand]
- POST /api/scripts/{id}/run
- POST /api/scripts/spin-environment [body: SpinEnvironmentCommand]
- POST /api/scripts/resolve-conflicts [body: ResolveGitConflictsCommand]
- POST /api/scripts/nuke-migrate [body: NukeAndMigrateCommand]
- POST /api/scripts/flush-cache [body: FlushCacheCommand]
- POST /api/scripts/format-lint [body: FormatAndLintCommand]
- POST /api/scripts/kill-nodes
- POST /api/scripts/generate-boilerplate [body: GenerateBoilerplateCommand]

StripeWebhookController (Route: /api/stripe/webhook)
- POST /api/stripe/webhook (Stripe webhook handler) — accepts raw Stripe event payload; handles CheckoutSessionCompleted, InvoicePaid, CustomerSubscriptionUpdated, CustomerSubscriptionDeleted

SubscriptionsController (Base: /api/subscriptions)
- GET /api/subscriptions/current
- GET /api/subscriptions/usage
- POST /api/subscriptions/checkout [body: CreateCheckoutSessionCommand]
- POST /api/subscriptions/portal [body: CreatePortalSessionCommand]
- POST /api/subscriptions/cancel

WorkspacesController (Base: /api/workspaces)
- GET /api/workspaces
- GET /api/workspaces/{id}
- POST /api/workspaces [body: CreateWorkspaceCommand]
- PUT /api/workspaces/{id} [body: UpdateWorkspaceCommand]
- DELETE /api/workspaces/{id}
- POST /api/workspaces/{id}/integrations/toggle [body: ToggleIntegrationDto]
- PATCH /api/workspaces/{id}/set-default
- POST /api/workspaces/validate-folder [body: ValidateFolderRequest]


-------------------------------------------------------------------------------

How I generated this reference
- Scanned: Presentation/Atlas.WebAPI/Controllers
- Base route: `api/[controller]` expanded to `/api/{controllerNameWithoutController}` unless overridden by `[Route(...)]`
- Actions: used Http* attributes and method signatures to list verbs, routes and parameter sources (route/query/body)

Caveats & next steps
- This document is a concise endpoint index (paths + parameters). For full DTO schemas and responses, we can auto-generate detailed sections from feature DTOs and MediatR request/response models.
- If you prefer controller grouping (Auth, Marketer, Leader, SecOps, Dev), I can reorganize sections accordingly.



