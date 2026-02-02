# 🤖 AI Collaboration Guide - Atlas Project

## Qısa Məlumat

Bu fayl Gemini, Claude, ChatGPT və digər AI asistentlərlə ortaq iş üçün hazırlanıb.

**Ətraflı dokumentasiya üçün:** [`ATLAS-PROJECT-DOCUMENTATION.md`](./ATLAS-PROJECT-DOCUMENTATION.md)

---

## 📊 Layihə Status Matrix

| Layer | Component | Status | Completeness |
|-------|-----------|--------|--------------|
| Domain | BaseEntity, IAggregateRoot | ✅ Complete | 100% |
| Domain | AppUser | ✅ Complete | 100% |
| Domain | Persona (Aggregate Root) | ✅ Complete | 100% |
| Domain | Integration | ✅ Complete | 100% |
| Domain | Workspace | ✅ Complete | 100% |
| Domain | WorkspaceIntegration | ✅ Complete | 100% |
| Domain | Enums | ✅ Complete | 100% |
| Domain | Exceptions | ✅ Complete | 100% |
| Persistence | ApplicationDbContext | ✅ Complete | 100% |
| Persistence | Entity Configurations | ✅ Complete | 100% |
| Application | Account Commands | ✅ Complete | 100% |
| Application | Account Queries | ✅ Complete | 100% |
| Application | Persona Commands | 🚧 In Progress | 40% |
| Application | Persona Queries | 🚧 In Progress | 30% |
| Application | Workspace Feature | ❌ Not Started | 0% |
| Infrastructure | Services | ✅ Complete | 100% |
| WebAPI | AccountController | ✅ Complete | 100% |
| WebAPI | PersonaController | 🚧 In Progress | 20% |
| WebAPI | WorkspaceController | ❌ Not Started | 0% |

---

## 🔧 Texniki Qeydlər AI Üçün

### 1. CQRS Pattern

```csharp
// Command nümunəsi
public record CreateSomethingCommand(string Name, ...) : IRequest<Guid>;

public class CreateSomethingCommandHandler(IApplicationDbContext context) 
    : IRequestHandler<CreateSomethingCommand, Guid>
{
    public async Task<Guid> Handle(CreateSomethingCommand request, CancellationToken cancellationToken)
    {
        var entity = Entity.Create(...);
        await context.Entities.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}

// Query nümunəsi
public record GetSomethingQuery(Guid Id) : IRequest<SomethingDto>;
```

### 2. Entity Factory Pattern

```csharp
// ❌ YANLIŞ
var entity = new Entity { Name = "test" };

// ✅ DOĞRU
var entity = Entity.Create(userId, "name", Type.Something);
```

### 3. Validation (FluentValidation)

```csharp
// Command ilə eyni faylda
public class CreateSomethingCommandValidator : AbstractValidator<CreateSomethingCommand>
{
    public CreateSomethingCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
    }
}
```

### 4. Response Format

```csharp
// Application layer-də: birbaşa DTO/data return
return personaDto;

// API layer-də: ResponseModel ilə wrap
return OkResponse(result);
```

### 5. JSONB Properties

PostgreSQL JSONB istifadə edən property-lər:
- `Persona.Config`
- `Integration.Metadata`
- `Workspace.Config`
- `WorkspaceIntegration.Config`

Bu property-lər dinamik konfiqurasiya saxlamaq üçündür.

---

## 📁 Fayl Strukturu

```
Atlas/
├── Core/
│   ├── Atlas.Domain/
│   │   ├── Abstractions/         # IAggregateRoot
│   │   ├── Entities/
│   │   │   ├── Common/           # BaseEntity
│   │   │   ├── AppUser.cs
│   │   │   ├── Persona.cs        # AGGREGATE ROOT
│   │   │   ├── Integration.cs
│   │   │   ├── Workspace.cs
│   │   │   └── WorkspaceIntegration.cs
│   │   ├── Enums/
│   │   ├── Events/
│   │   └── Exceptions/
│   │
│   └── Atlas.Application/
│       ├── Common/
│       │   ├── Behaviors/        # MediatR pipeline
│       │   ├── Exceptions/       # Application exceptions
│       │   ├── Interfaces/       # Service contracts
│       │   └── Models/           # ResponseModel
│       ├── Features/
│       │   ├── Accounts/         # ✅ Complete
│       │   └── Personas/         # 🚧 In Progress
│       ├── MapProfiles/
│       └── Settings/
│
├── Infrastructure/
│   ├── Atlas.Infrastructure/
│   │   └── Services/             # Service implementations
│   │
│   └── Atlas.Persistence/
│       ├── Configurations/       # EF Fluent API
│       └── Data/                 # DbContext
│
└── Presentation/
    └── Atlas.WebAPI/
        ├── Controllers/
        └── Middlewares/
```

---

## 🎯 Növbəti Tapşırıqlar

### Priority 1: Persona Feature Completion
1. `GetPersonaByIdQuery` - Tək persona almaq
2. `UpdatePersonaCommand` - Persona yeniləmək
3. `DeletePersonaCommand` - Persona silmək (soft delete)
4. `SetPrimaryPersonaCommand` - Primary persona təyin etmək

### Priority 2: Workspace Feature
1. `CreateWorkspaceCommand`
2. `GetWorkspacesQuery`
3. `UpdateWorkspaceCommand`
4. `DeleteWorkspaceCommand`
5. `LinkIntegrationCommand`
6. `UnlinkIntegrationCommand`

### Priority 3: Integration Feature
1. `UpdateIntegrationCommand`
2. `DeleteIntegrationCommand`
3. `RotateTokenCommand`
4. `DeactivateIntegrationCommand`

---

## ⚠️ Vacib Qaydalar

1. **Private setters** - Bütün entity property-lərində
2. **Factory methods** - `Create()` static method istifadə et
3. **Domain methods** - Business logic entity daxilində
4. **Soft delete** - `IsDeleted` flag, heç vaxt fiziki silmə
5. **Audit fields** - `CreatedAt`, `ModifiedAt` avtomatik
6. **Query filters** - Soft delete üçün konfiqurasiya olunub

---

## 🔗 Faydalı Linklər

- [Full Documentation](./ATLAS-PROJECT-DOCUMENTATION.md)
- [Domain README](./Core/Atlas.Domain/Phase1-README.md)
- [Persistence README](./Infrastructure/Atlas.Persistence/Persistence-README.md)

---

## 📝 Kod Nümunələri (Copy-Paste Ready)

### Yeni Command Yaratmaq

```csharp
// Features/[FeatureName]/Commands/[CommandName]/[CommandName]Command.cs
using FluentValidation;
using MediatR;

namespace Atlas.Application.Features.[FeatureName].Commands.[CommandName];

public record [CommandName]Command(
    Guid Id,
    string Name
    // ... digər parametrlər
) : IRequest<Guid>;  // və ya IRequest<Unit>, IRequest<SomeDto>

public class [CommandName]CommandValidator : AbstractValidator<[CommandName]Command>
{
    public [CommandName]CommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
    }
}
```

### Yeni Command Handler Yaratmaq

```csharp
// Features/[FeatureName]/Commands/[CommandName]/[CommandName]CommandHandler.cs
using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.[FeatureName].Commands.[CommandName];

public class [CommandName]CommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService
) : IRequestHandler<[CommandName]Command, Guid>
{
    public async Task<Guid> Handle([CommandName]Command request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId 
            ?? throw new UnauthorizedException("User not authenticated.");

        // Entity yaratmaq (Factory Method istifadə et!)
        var entity = Domain.Entities.SomeEntity.Create(
            userId,
            request.Name
        );

        await context.SomeEntities.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return entity.Id;
    }
}
```

### Yeni Query Yaratmaq

```csharp
// Features/[FeatureName]/Queries/[QueryName]/[QueryName]Query.cs
using MediatR;

namespace Atlas.Application.Features.[FeatureName].Queries.[QueryName];

public record [QueryName]Query(Guid? Id = null) : IRequest<List<SomeDto>>;

// və ya tək item üçün:
public record Get[Entity]ByIdQuery(Guid Id) : IRequest<SomeDto>;
```

### Yeni Query Handler Yaratmaq

```csharp
// Features/[FeatureName]/Queries/[QueryName]/[QueryName]QueryHandler.cs
using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.[FeatureName].Queries.[QueryName];

public class [QueryName]QueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    IMapper mapper
) : IRequestHandler<[QueryName]Query, List<SomeDto>>
{
    public async Task<List<SomeDto>> Handle([QueryName]Query request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId 
            ?? throw new UnauthorizedException("User not authenticated.");

        var query = context.SomeEntities
            .Where(e => e.UserId == userId);

        if (request.Id.HasValue)
            query = query.Where(e => e.Id == request.Id.Value);

        return await query
            .ProjectTo<SomeDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
```

### Controller Endpoint

```csharp
// Controllers/[FeatureName]Controller.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class [FeatureName]Controller : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSomethingCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedResponse(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await Mediator.Send(new GetAllSomethingQuery());
        return OkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetSomethingByIdQuery(id));
        return OkResponse(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSomethingCommand command)
    {
        // Id-ni command-a əlavə et
        var result = await Mediator.Send(command with { Id = id });
        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteSomethingCommand(id));
        return NoContentResponse();
    }
}
```

### AutoMapper Profile-a Map Əlavə Etmək

```csharp
// MapProfiles/MapProfile.cs
public class MapProfile : Profile
{
    public MapProfile()
    {
        // ... existing mappings ...
        
        CreateMap<NewEntity, NewEntityDto>();
        // və ya custom mapping:
        CreateMap<NewEntity, NewEntityDto>()
            .ForMember(dest => dest.SomeProperty, 
                       opt => opt.MapFrom(src => src.RelatedEntity.Name));
    }
}
```

---

## 🚨 Exception İstifadəsi

```csharp
// Common Exceptions (Atlas.Application.Common.Exceptions.Common/)
throw new NotFoundException("Entity", id);           // 404
throw new BadRequestException("Invalid request");    // 400
throw new UnauthorizedException("Not authenticated"); // 401
throw new ForbiddenException("Access denied");       // 403
throw new AlreadyExistException("Entity", "name");   // 409

// User Exceptions (Atlas.Application.Common.Exceptions.Users/)
throw new InvalidCredentialsException("Wrong password");
throw new AccountLockedException("Too many attempts");
throw new EmailNotVerifiedException("Verify email first");

// Domain Exceptions (Atlas.Domain.Exceptions/)
throw new InvalidEntityStateException("Entity", "Property", "Invalid value");
throw new EntityNotFoundException("Entity", id);
throw new BusinessRuleViolationException("RuleName", "Description");
```

---

## 📊 Database Schema

```
┌─────────────────────────────────────────────────────────────────────┐
│                         SCHEMA: identity                            │
├─────────────────────────────────────────────────────────────────────┤
│  Users, Roles, UserRoles, UserClaims, UserLogins, RoleClaims,      │
│  UserTokens (ASP.NET Identity)                                      │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                          SCHEMA: atlas                              │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌─────────────┐         ┌─────────────┐                           │
│  │   Personas  │────────<│ Integrations│                           │
│  │  (jsonb:    │         │  (jsonb:    │                           │
│  │   Config)   │         │   Metadata) │                           │
│  └──────┬──────┘         └──────┬──────┘                           │
│         │                       │                                   │
│         │    ┌─────────────┐    │                                   │
│         └───>│  Workspaces │<───┘                                   │
│              │  (jsonb:    │     via WorkspaceIntegrations         │
│              │   Config)   │                                        │
│              └─────────────┘                                        │
│                                                                     │
│  ┌─────────────┐    ┌───────────────────┐    ┌─────────────────┐   │
│  │ Professions │───>│OnboardingQuestions│───>│OnboardingOptions│   │
│  └─────────────┘    └───────────────────┘    └─────────────────┘   │
│                                                                     │
│  ┌─────────────┐                                                    │
│  │  Interests  │                                                    │
│  └─────────────┘                                                    │
└─────────────────────────────────────────────────────────────────────┘
```

---

## ⚡ Quick Reference

| İş | Nə Etmək Lazım |
|----|----------------|
| Yeni Entity | Domain/Entities/ + Configuration + DbSet |
| Yeni Command | Features/[X]/Commands/[Y]/ (Command + Handler + Validator) |
| Yeni Query | Features/[X]/Queries/[Y]/ (Query + Handler) |
| Yeni DTO | Features/[X]/Dtos/ + MapProfile update |
| Yeni Endpoint | Controllers/[X]Controller.cs |
| Yeni Exception | Common/Exceptions/ + Middleware update |
| Yeni Service | Common/Interfaces/ + Infrastructure/Services/ + ServiceRegistration |
