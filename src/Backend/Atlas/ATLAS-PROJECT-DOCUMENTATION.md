# 🌐 ATLAS - Unified Engineering Intelligence Platform

## 📋 Complete Project Documentation for AI Collaboration

> **Last Updated:** February 2026
> **Version:** Phase 1 - Domain & Persistence Complete
> **Platform:** .NET 8.0 / PostgreSQL / Entity Framework Core

---

## 🎯 Project Vision

Atlas, Developer, Designer, AI Engineer və Security Analyst kimi texniki peşəkarlar üçün nəzərdə tutulmuş "Action & Knowledge Hub" platformasıdır. GitHub, Figma, JetBrains, HuggingFace kimi alətlərlə inteqrasiya edərək kontekstual AI əməliyyatları təqdim edir.

---

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────┐
│                        ONION ARCHITECTURE                          │
├─────────────────────────────────────────────────────────────────────┤
│  ┌───────────────────────────────────────────────────────────────┐  │
│  │                    Presentation Layer                         │  │
│  │  Atlas.WebAPI (Controllers, Middlewares)                     │  │
│  └───────────────────────────────────────────────────────────────┘  │
│                              ▼                                      │
│  ┌───────────────────────────────────────────────────────────────┐  │
│  │                   Infrastructure Layer                        │  │
│  │  Atlas.Infrastructure (Services - JWT, Email, SMS, Telegram)  │  │
│  │  Atlas.Persistence (DbContext, Configurations, EF Core)       │  │
│  └───────────────────────────────────────────────────────────────┘  │
│                              ▼                                      │
│  ┌───────────────────────────────────────────────────────────────┐  │
│  │                     Application Layer                         │  │
│  │  Atlas.Application (Features, Commands, Queries, CQRS)        │  │
│  └───────────────────────────────────────────────────────────────┘  │
│                              ▼                                      │
│  ┌───────────────────────────────────────────────────────────────┐  │
│  │                       Domain Layer                            │  │
│  │  Atlas.Domain (Entities, Value Objects, Domain Events)        │  │
│  └───────────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 📁 Project Structure

```
Atlas/
├── Atlas.sln
├── Core/
│   ├── Atlas.Domain/           # Domain Layer (Entities, Enums, Events)
│   └── Atlas.Application/      # Application Layer (CQRS, Features)
├── Infrastructure/
│   ├── Atlas.Infrastructure/   # External Services Implementation
│   └── Atlas.Persistence/      # EF Core, DbContext, Configurations
└── Presentation/
    └── Atlas.WebAPI/           # REST API, Controllers, Middlewares
```

---

## 🎨 Domain Layer (Atlas.Domain)

### Base Abstractions

#### BaseEntity (`Entities/Common/BaseEntity.cs`)
```csharp
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTimeOffset CreatedAt { get; protected set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ModifiedAt { get; protected set; }
    public bool IsDeleted { get; protected set; }  // Soft Delete
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    protected void SetModified();
    public virtual void Delete();
    public virtual void Restore();
}
```

#### IAggregateRoot (`Abstractions/IAggregateRoot.cs`)
```csharp
public interface IAggregateRoot
{
    Guid Id { get; }
}
```

#### IDomainEvent (`Events/IDomainEvent.cs`)
```csharp
public interface IDomainEvent
{
    Guid EventId { get; }
    DateTimeOffset OccurredOn { get; }
}
```

### Enums

#### PersonaType (`Enums/PersonaType.cs`)
```csharp
public enum PersonaType
{
    General = 0,
    Work = 1,
    Personal = 2,
    Learning = 3
}
```

#### IntegrationProvider (`Enums/IntegrationProvider.cs`)
```csharp
public enum IntegrationProvider
{
    System = 0,     // Local OS monitoring
    GitHub = 1,
    GitLab = 2,
    Jira = 3,
    Figma = 4,
    OpenAI = 5,
    JetBrains = 6,
    Docker = 7,
    HuggingFace = 8,
    Slack = 9,
    Discord = 10,
    AzureDevOps = 11,
    VsCode = 12
}
```

#### UserStatus (`Enums/UserStatus.cs`)
```csharp
public enum UserStatus
{
    PendingVerification,
    Active,
    Suspended,
    Deactivated
}
```

#### UserVerificationChannel (`Enums/UserVerificationChannel.cs`)
```csharp
public enum UserVerificationChannel
{
    Email,
    Sms,
    Telegram
}
```

### Core Entities

#### AppUser (`Entities/AppUser.cs`)
**Identity User** - Microsoft.AspNetCore.Identity ilə genişləndirilmiş istifadəçi entity-si.

```csharp
public class AppUser : IdentityUser<Guid>
{
    // Properties
    public string FullName { get; set; }
    public UserStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ActivatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public string? EmailVerificationCode { get; set; }
    public DateTime? EmailVerificationExpiresAt { get; set; }
    public UserVerificationChannel? PreferredVerificationChannel { get; set; }
    public string? TelegramChatId { get; set; }
    public string? TelegramLinkCode { get; set; }
    public DateTime? TelegramLinkCodeExpiry { get; set; }
    public string? PhoneVerificationCode { get; set; }
    public DateTime? PhoneVerificationExpiresAt { get; set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiresAt { get; private set; }
    public string? ResetPasswordCode { get; set; }
    public DateTime? ResetPasswordExpiresAt { get; set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEndTime { get; set; }
    public bool IsLockedOut => LockoutEndTime.HasValue && LockoutEndTime > DateTime.UtcNow;
    
    // Factory Method
    public static AppUser Create(string userName, string email, string fullName, ...);
    
    // Domain Methods
    public void Activate();
    public void UpdateLastLogin();
    public void UpdateProfile(string? fullName, string? userName);
    public void SetRefreshToken(string token, DateTime expiresAt);
    public void RevokeRefreshToken();
    public void RevokeAllRefreshTokens();
    public void MarkAsDeleted();
    public void IncrementFailedLoginAttempts(int maxAttempts, TimeSpan lockoutDuration);
    public void ResetFailedLoginAttempts();
}
```

#### Persona (`Entities/Persona.cs`) - **AGGREGATE ROOT**
İstifadəçinin peşəkar rolunu təmsil edir (Developer, Designer, etc.)

```csharp
public class Persona : BaseEntity, IAggregateRoot
{
    // Properties
    public Guid UserId { get; private set; }
    public string Name { get; private set; }
    public string? Bio { get; private set; }
    public PersonaType Type { get; private set; }
    public string? Config { get; private set; }          // PostgreSQL JSONB
    public bool IsPrimary { get; private set; }
    
    // Navigation Properties
    public IReadOnlyCollection<Integration> Integrations { get; }
    public IReadOnlyCollection<Workspace> Workspaces { get; }
    
    // Factory Method
    public static Persona Create(Guid userId, string name, PersonaType type, string? bio = null, bool isPrimary = false);
    
    // Domain Methods
    public void UpdateProfile(string name, string? bio);
    public void UpdateConfig(string? config);
    public void ChangeType(PersonaType newType);
    public void SetAsPrimary();
    public void RemovePrimaryStatus();
    public void AddIntegration(Integration integration);
    public void RemoveIntegration(Guid integrationId);
    public void AddWorkspace(Workspace workspace);
    public void RemoveWorkspace(Guid workspaceId);
}
```

#### Integration (`Entities/Integration.cs`)
Xarici alət bağlantısını təmsil edir (GitHub, Figma, etc.)

```csharp
public class Integration : BaseEntity
{
    // Properties
    public Guid PersonaId { get; private set; }
    public IntegrationProvider Provider { get; private set; }
    public string Name { get; private set; }
    public string? EncryptedAccessToken { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTimeOffset? TokenExpiresAt { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset? LastUsedAt { get; private set; }
    public string? Metadata { get; private set; }          // PostgreSQL JSONB
    
    // Navigation
    public Persona Persona { get; private set; }
    
    // Factory Method
    public static Integration Create(Guid personaId, IntegrationProvider provider, string name, ...);
    
    // Domain Methods
    public void UpdateName(string name);
    public void UpdateTokens(string encryptedAccessToken, ...);
    public void RotateToken(string newEncryptedAccessToken, ...);
    public void UpdateMetadata(string? metadata);
    public void RecordUsage();
    public void Activate();
    public void Deactivate();
    public void Revoke();
    public bool IsTokenExpired(int bufferMinutes = 5);
    public bool IsUsable();
}
```

#### Workspace (`Entities/Workspace.cs`)
İş kontekstini təmsil edir (Project Atlas, Freelance Work, etc.)

```csharp
public class Workspace : BaseEntity
{
    // Properties
    public Guid PersonaId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? Icon { get; private set; }
    public string? Color { get; private set; }           // Hex color (#FF5733)
    public bool IsDefault { get; private set; }
    public string? Config { get; private set; }          // PostgreSQL JSONB
    public DateTimeOffset? LastAccessedAt { get; private set; }
    
    // Navigation
    public Persona Persona { get; private set; }
    public IReadOnlyCollection<WorkspaceIntegration> WorkspaceIntegrations { get; }
    
    // Factory Method
    public static Workspace Create(Guid personaId, string name, ...);
    
    // Domain Methods
    public void Update(string name, string? description, string? icon, string? color);
    public void UpdateConfig(string? config);
    public void SetAsDefault();
    public void RemoveDefaultStatus();
    public void RecordAccess();
    public void LinkIntegration(Guid integrationId, string? config = null);
    public void UnlinkIntegration(Guid integrationId);
}
```

#### WorkspaceIntegration (`Entities/WorkspaceIntegration.cs`)
Workspace-Integration many-to-many əlaqəsi

```csharp
public class WorkspaceIntegration : BaseEntity
{
    public Guid WorkspaceId { get; private set; }
    public Guid IntegrationId { get; private set; }
    public string? Config { get; private set; }          // PostgreSQL JSONB
    
    internal static WorkspaceIntegration Create(Guid workspaceId, Guid integrationId, string? config = null);
    public void UpdateConfig(string? config);
}
```

### Supporting Entities

#### Profession, Interest, OnboardingQuestion, OnboardingOption
Onboarding prosesi üçün istifadə olunan entity-lər.

### Domain Exceptions (`Exceptions/DomainExceptions.cs`)

```csharp
public abstract class DomainException : Exception
{
    public string ErrorCode { get; }
}

public class InvalidEntityStateException : DomainException
{
    public string EntityName { get; }
    public string PropertyName { get; }
}

public class EntityNotFoundException : DomainException
{
    public string EntityName { get; }
    public object EntityId { get; }
}

public class BusinessRuleViolationException : DomainException
{
    public string RuleName { get; }
}
```

---

## 💼 Application Layer (Atlas.Application)

### CQRS Pattern Implementation

Layihə **MediatR** ilə CQRS pattern istifadə edir:
- **Commands**: Dəyişiklik əməliyyatları (Create, Update, Delete)
- **Queries**: Oxuma əməliyyatları (Get, List)

### Service Registration (`ServiceRegistration.cs`)

```csharp
public static class ServiceRegistration
{
    extension(IServiceCollection services)
    {
        public void AddApplicationServices(IConfiguration configuration)
        {
            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(opt => {
                opt.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                opt.AddOpenBehavior(typeof(UnhandledExceptionBehavior<,>));
                opt.AddOpenBehavior(typeof(LoggingBehavior<,>));
                opt.AddOpenBehavior(typeof(PerformanceBehavior<,>));
                opt.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
            services.Configure<LockoutSettings>(configuration.GetSection("LockoutSettings"));
        }
    }
}
```

### Pipeline Behaviors (`Common/Behaviors/`)

| Behavior | Purpose |
|----------|---------|
| `ValidationBehavior` | FluentValidation ilə request validasyonu |
| `LoggingBehavior` | Request/Response logging |
| `PerformanceBehavior` | Performance monitoring |
| `UnhandledExceptionBehavior` | Exception handling |

### Common Models

#### ResponseModel<T> (`Common/Models/ResponseModel.cs`)
API cavabları üçün standart wrapper.

```csharp
public class ResponseModel<T>
{
    public T? Data { get; set; }
    public bool IsSuccess { get; set; }
    public IEnumerable<string>? Errors { get; set; }
    
    public static ResponseModel<T> Success(T data);
    public static ResponseModel<T> Failure(string error);
    public static ResponseModel<T> Failure(IEnumerable<string> errors);
}
```

### Interfaces (`Common/Interfaces/`)

| Interface | Purpose |
|-----------|---------|
| `IApplicationDbContext` | DbContext abstraction |
| `ICurrentUserService` | Current user information |
| `IJwtService` | JWT token generation |
| `IEmailService` | Email sending |
| `ISmsService` | SMS sending |
| `ITelegramService` | Telegram messaging |
| `IPhoneVerificationService` | Phone verification |
| `IExternalAuthService` | OAuth providers |

### Features Structure

```
Features/
├── Accounts/                    # ✅ COMPLETE
│   ├── Commands/
│   │   ├── Register/
│   │   ├── Login/
│   │   ├── Logout/
│   │   ├── RefreshToken/
│   │   ├── ForgotPassword/
│   │   ├── ResetPassword/
│   │   ├── ChangePassword/
│   │   ├── VerifyEmail/
│   │   ├── VerifyPhone/
│   │   ├── ResendEmailVerification/
│   │   ├── ResendPhoneVerification/
│   │   ├── UpdateProfile/
│   │   ├── DeleteAccount/
│   │   ├── ExternalLogin/
│   │   ├── AddPhoneNumber/
│   │   ├── SetTelegramChatId/
│   │   ├── GenerateTelegramLinkCode/
│   │   ├── LinkTelegramByChatId/
│   │   ├── RevokeToken/
│   │   └── CompleteOnboarding/
│   ├── Queries/
│   │   ├── GetProfile/
│   │   ├── GetOnboardingQuestions/
│   │   └── GetProfessionsQuery.cs
│   └── Dtos/
│       ├── AccountDto.cs
│       ├── AuthResponseDto.cs
│       ├── AccessTokenResponseDto.cs
│       ├── RefreshTokenResponseDto.cs
│       └── ...
│
└── Personas/                    # 🚧 IN PROGRESS
    ├── Commands/
    │   ├── CreatePersona/
    │   └── AddIntegration/
    ├── Queries/
    │   └── GetPersonas/
    └── Dtos/
        ├── PersonaDto.cs
        └── IntegrationDto.cs
```

### Account Feature DTOs

```csharp
// Account profile information
public record AccountDto(
    string Id, string UserName, string Email, string FullName,
    string? PhoneNumber, bool EmailConfirmed, bool PhoneNumberConfirmed,
    DateTime CreatedAt, UserStatus Status, DateTime? LastLoginAt
);

// Authentication response after login
public record AuthResponseDto(
    string AccessToken, string RefreshToken,
    DateTime AccessTokenExpiration, DateTime RefreshTokenExpiration,
    string UserId, string UserName, string Email, string FullName
);

// Access token with expiration
public record AccessTokenResponseDto(string Token, DateTime Expiration);

// Refresh token with expiration
public record RefreshTokenResponseDto(string RefreshToken, DateTime RefreshTokenExpiresAt);
```

### Persona Feature DTOs

```csharp
public record PersonaDto(
    Guid Id, string Name, string Bio,
    PersonaType Type, bool IsPrimary,
    List<IntegrationDto> Integrations
);

public record IntegrationDto(Guid Id, string Name, string Provider);
```

### AutoMapper Profile (`MapProfiles/MapProfile.cs`)

```csharp
public class MapProfile : Profile
{
    public MapProfile()
    {
        CreateMap<AppUser, AccountDto>();
        CreateMap<Persona, PersonaDto>()
            .ForMember(dest => dest.Integrations, opt => opt.MapFrom(src => src.Integrations));
        CreateMap<Integration, IntegrationDto>()
            .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src.Provider.ToString()));
    }
}
```

### Settings (`Settings/`)

| Setting Class | Purpose |
|---------------|---------|
| `JwtSettings` | JWT configuration (SecretKey, Issuer, Audience, Expiration) |
| `LockoutSettings` | Account lockout policy |
| `PasswordPolicySettings` | Password requirements |
| `RateLimitSettings` | API rate limiting |
| `EmailSettings` | Email service configuration |
| `SmsSettings` | SMS service configuration |
| `TelegramSettings` | Telegram bot configuration |
| `ExternalAuthSettings` | OAuth provider settings |

---

## 🔧 Infrastructure Layer

### Atlas.Infrastructure

External service implementations:

| Service | Interface | Purpose |
|---------|-----------|---------|
| `JwtService` | `IJwtService` | JWT token generation (HS256) |
| `CurrentUserService` | `ICurrentUserService` | Get current user from HttpContext |
| `EmailService` | `IEmailService` | Email sending |
| `SmsService` | `ISmsService` | SMS sending |
| `TelegramService` | `ITelegramService` | Telegram messaging |
| `ExternalAuthService` | `IExternalAuthService` | OAuth integration |
| `PhoneVerificationService` | `IPhoneVerificationService` | Phone verification |

### Atlas.Persistence

#### ApplicationDbContext (`Data/ApplicationDbContext.cs`)

```csharp
public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    // DbSets
    public DbSet<AppUser> Users { get; set; }
    public DbSet<Persona> Personas { get; set; }
    public DbSet<Integration> Integrations { get; set; }
    public DbSet<Workspace> Workspaces { get; set; }
    public DbSet<WorkspaceIntegration> WorkspaceIntegrations { get; set; }
    public DbSet<Profession> Professions { get; set; }
    public DbSet<Interest> Interests { get; set; }
    public DbSet<OnboardingQuestion> OnboardingQuestions { get; set; }
    public DbSet<OnboardingOption> OnboardingOptions { get; set; }
    
    // Auto audit fields (CreatedAt, ModifiedAt)
    private void UpdateAuditFields();
}
```

#### Entity Configurations (Fluent API)

**Schema:** `atlas` (Domain entities), `identity` (ASP.NET Identity)

**JSONB Properties:**
- `Persona.Config` → PostgreSQL `jsonb`
- `Integration.Metadata` → PostgreSQL `jsonb`
- `Workspace.Config` → PostgreSQL `jsonb`
- `WorkspaceIntegration.Config` → PostgreSQL `jsonb`

**Soft Delete:** Bütün entity-lərdə `IsDeleted` query filter tətbiq olunub.

**Indexes:**
- `IX_Personas_UserId`
- `IX_Personas_UserId_Type`
- `IX_Integrations_PersonaId_Provider_Name` (Unique with filter)
- `IX_Workspaces_PersonaId_Name` (Unique with filter)

---

## 🌐 Presentation Layer (Atlas.WebAPI)

### Controllers

#### ApiControllerBase
Bütün controller-lar üçün base class.

```csharp
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected ISender Mediator { get; }
    
    // Response helpers
    protected IActionResult OkResponse<T>(T data);
    protected IActionResult CreatedResponse<T>(T data, string? location = null);
    protected IActionResult NoContentResponse();
    protected IActionResult BadRequestResponse(string message);
    protected IActionResult NotFoundResponse(string message);
    protected IActionResult UnauthorizedResponse(string message = "Unauthorized");
    protected IActionResult ForbiddenResponse(string message = "Forbidden");
}
```

#### AccountController - Endpoints

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/Account/register` | ❌ | User registration |
| POST | `/api/Account/login` | ❌ | User login |
| POST | `/api/Account/external-login` | ❌ | OAuth login |
| POST | `/api/Account/logout` | ❌ | Logout |
| POST | `/api/Account/forgot-password` | ❌ | Request password reset |
| POST | `/api/Account/reset-password` | ❌ | Reset password |
| POST | `/api/Account/verify-email` | ❌ | Verify email |
| POST | `/api/Account/verify-phone` | ❌ | Verify phone |
| POST | `/api/Account/resend-email-verification-code` | ❌ | Resend email code |
| POST | `/api/Account/resend-phone-verification-code` | ❌ | Resend phone code |
| POST | `/api/Account/refresh-token` | ❌ | Refresh JWT token |
| POST | `/api/Account/revoke-refresh-token` | ❌ | Revoke token |
| GET | `/api/Account/profile` | ✅ | Get current user profile |
| PUT | `/api/Account/profile` | ✅ | Update profile |
| PUT | `/api/Account/change-password` | ✅ | Change password |
| POST | `/api/Account/add-phone-number` | ✅ | Add phone number |
| DELETE | `/api/Account/delete-account` | ✅ | Delete account |
| POST | `/api/Account/set-telegram-chat-id` | ✅ | Set Telegram |
| POST | `/api/Account/generate-telegram-link-code` | ✅ | Generate Telegram link |
| POST | `/api/Account/onboarding` | ❌ | Complete onboarding |

### Middleware

#### ExceptionHandlingMiddleware
Global exception handling with consistent response format.

```csharp
// Exception → HTTP Status Code mapping
ValidationException → 400 Bad Request
BadRequestException → 400 Bad Request
InvalidCredentialsException → 401 Unauthorized
AccountLockedException → 423 Locked
ForbiddenException → 403 Forbidden
NotFoundException → 404 Not Found
AlreadyExistException → 409 Conflict
_ → 500 Internal Server Error
```

### Rate Limiting

```csharp
// Rate limit policies
"fixed" → General API calls
"login" → Login attempts
"register" → Registration
"password-reset" → Password reset
"verification" → Verification codes
"resend" → Resend codes
"api" → Authenticated API (sliding window)
```

---

## ⚙️ Configuration

### appsettings.json Structure

```json
{
  "ConnectionStrings": {
    "PostgreSqlConnection": "Host=localhost;Database=AtlasDb;Username=...;Password=..."
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-min-32-chars",
    "Issuer": "AtlasAPI",
    "Audience": "AtlasClient",
    "AccessTokenExpirationMinutes": 30,
    "RefreshTokenExpirationDays": 7
  },
  "PasswordPolicySettings": {
    "RequiredLength": 8,
    "RequireDigit": true,
    "RequireLowercase": true,
    "RequireUppercase": true,
    "RequireNonAlphanumeric": false,
    "RequiredUniqueChars": 1
  },
  "LockoutSettings": {
    "MaxFailedAccessAttempts": 5,
    "LockoutDurationInMinutes": 15
  },
  "RateLimitSettings": {
    "Fixed": { "PermitLimit": 100, "WindowInSeconds": 60 },
    "Login": { "PermitLimit": 5, "WindowInSeconds": 60 },
    "Register": { "PermitLimit": 3, "WindowInSeconds": 300 },
    "PasswordReset": { "PermitLimit": 3, "WindowInSeconds": 300 },
    "Verification": { "PermitLimit": 5, "WindowInSeconds": 60 },
    "Resend": { "PermitLimit": 3, "WindowInSeconds": 300 },
    "Api": { "PermitLimit": 50, "WindowInSeconds": 60, "SegmentsPerWindow": 4 }
  },
  "ThirdPartyServices": {
    "EmailSettings": { ... },
    "SmsSettings": { ... },
    "TelegramSettings": { ... }
  },
  "ExternalAuthSettings": {
    "Google": { ... },
    "GitHub": { ... }
  }
}
```

---

## 📦 NuGet Packages

### Atlas.Domain
- (Pure C#, no external dependencies)

### Atlas.Application
- MediatR
- FluentValidation
- AutoMapper

### Atlas.Infrastructure
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Options

### Atlas.Persistence
- Microsoft.EntityFrameworkCore
- Npgsql.EntityFrameworkCore.PostgreSQL
- Microsoft.AspNetCore.Identity.EntityFrameworkCore

### Atlas.WebAPI
- Swashbuckle.AspNetCore / Scalar.AspNetCore
- Newtonsoft.Json
- Microsoft.AspNetCore.RateLimiting

---

## 🎯 Current Status & Next Steps

### ✅ Completed (Phase 1)

1. **Domain Layer**
   - BaseEntity with DomainEvents support
   - All core entities (Persona, Integration, Workspace, WorkspaceIntegration)
   - Enums (PersonaType, IntegrationProvider, UserStatus, etc.)
   - Domain Exceptions
   - Factory Methods & Domain Methods

2. **Persistence Layer**
   - ApplicationDbContext with audit fields
   - All Entity Configurations with JSONB support
   - Soft Delete Query Filters
   - Proper indexing

3. **Account Feature (CQRS)**
   - Complete registration flow with email/phone verification
   - Login with lockout protection
   - JWT Authentication (Access + Refresh tokens)
   - Password management (forgot, reset, change)
   - Profile management
   - Telegram integration
   - External login (OAuth)

4. **Infrastructure**
   - Rate Limiting
   - Exception Handling Middleware
   - All service interfaces and implementations

### 🚧 In Progress (Phase 2)

1. **Persona Feature**
   - CreatePersona ✅
   - AddIntegration ✅
   - GetPersonas (needs completion)
   - UpdatePersona
   - DeletePersona
   - ManageWorkspaces

2. **Workspace Feature**
   - CRUD operations
   - Integration linking

### 📋 Planned (Phase 3+)

1. **Integration Connectors**
   - GitHub OAuth flow
   - GitLab integration
   - Figma integration
   - etc.

2. **Context Monitoring**
   - Active IDE detection
   - Current task tracking
   - Desktop integration

3. **AI Actions**
   - OpenAI integration
   - Context-aware suggestions

---

## 🔐 Security Notes

1. **JWT**: HS256 symmetric encryption
2. **Password Hashing**: 100,000 iterations (PBKDF2)
3. **Soft Delete**: Users/data never truly deleted
4. **Rate Limiting**: Protection against brute force
5. **Account Lockout**: After 5 failed attempts
6. **Tokens**: Access token encrypted, stored encrypted

---

## 📝 Coding Conventions

1. **Entities**: Private setters, Factory Methods, Domain Methods
2. **CQRS**: Separate Command/Query folders with Handler
3. **Validation**: FluentValidation in same file as Command
4. **DTOs**: Record types for immutability
5. **Response**: Always wrap in ResponseModel<T>
6. **Exceptions**: Use domain exceptions, handle in middleware

---

## 🚀 How to Run

```bash
# Navigate to project
cd Atlas/Presentation/Atlas.WebAPI

# Update database
dotnet ef database update -p ../Infrastructure/Atlas.Persistence

# Run
dotnet run
```

**API Documentation**: `http://localhost:5000/scalar/v1`

---

## 📞 Contact for AI Collaboration

Bu sənəd Gemini, Claude, ChatGPT və digər AI asistentlərlə ortaq iş üçün hazırlanıb. Layihə strukturu, kodlama qaydaları və mövcud implementasiya haqqında tam məlumat verilir.

**Son yenilənmə:** Phase 1 tamamlandı, Phase 2 davam edir.
