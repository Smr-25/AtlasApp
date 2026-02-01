using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Accounts.Commands.CompleteOnboarding;

public class CompleteOnboardingCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService) : IRequestHandler<CompleteOnboardingCommand, bool>
{
    public async Task<bool> Handle(CompleteOnboardingCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (userId == Guid.Empty.ToString())
            throw new UnauthorizedAccessException();
        var selectedOptions = await applicationDbContext.OnboardingOptions
            .Where(o => request.SelectedOptionIds.Contains(o.Id))
            .ToListAsync(cancellationToken);
        var profession = await applicationDbContext.Professions
            .FirstOrDefaultAsync(p => p.Id == request.ProfessionId, cancellationToken);
        if (profession == null)
            throw new NotFoundException(nameof(Profession), request.ProfessionId);
        var personaName = true ? profession.Name : "Work Mode";

        var bioParts = selectedOptions
            .Where(o => !string.IsNullOrEmpty(o.BioPart))
            .Select(o => o.BioPart)
            .ToList();
        var dynamicBio = string.Join(", ", bioParts) + ".";

        var workPersona = Persona.Create(
            Guid.Parse(profession.Id.ToString()),
            personaName,
            PersonaType.Work,
            isPrimary: true,
            bio: dynamicBio
        );

        var chillPersona = Persona.Create(
            Guid.Parse(profession.Id.ToString()),
            "Personal Mode",
            PersonaType.Personal,
            isPrimary: false,
            bio: "Just here to relax and have fun."
        );
        await applicationDbContext.Personas.AddRangeAsync(workPersona, chillPersona);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}