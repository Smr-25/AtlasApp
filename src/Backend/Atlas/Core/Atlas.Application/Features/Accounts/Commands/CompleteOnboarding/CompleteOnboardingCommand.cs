using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Accounts.Dtos;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Accounts.Commands.CompleteOnboarding;

public record CompleteOnboardingCommand(
    Guid ProfessionId,
    List<OnboardingAnswerDto> Answers 
) : IRequest<bool>;

public class CompleteOnboardingCommandHandler(IApplicationDbContext applicationDbContext,ICurrentUserService currentUserService) : IRequestHandler<CompleteOnboardingCommand, bool>
{
    public async Task<bool> Handle(CompleteOnboardingCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId; 
        if (userId == Guid.Empty.ToString()) 
            throw new UnauthorizedAccessException();

        var selectedOptionIds = request.Answers.Select(a => a.OptionId).ToList();
        var selectedOptions = await applicationDbContext.Set<OnboardingOption>()
            .Where(o => selectedOptionIds.Contains(o.Id))
            .ToListAsync(cancellationToken);
        
        var profession = await applicationDbContext.Professions.FirstOrDefaultAsync(p => p.Id == request.ProfessionId, cancellationToken);
        if (profession == null)
            throw new NotFoundException("Profession not found");
        
        var personaName = "Deep Work 🧠";
        var bio = "Focus mode enabled.";
        
        if (selectedOptions.Any())
        {
            var tools = string.Join(", ", selectedOptions.Select(o => o.Text));
            bio = $"Stack: {tools}. Ready to build awesome things.";
        
            if (selectedOptions.Any(o => o.Value == "csharp")) 
                personaName = ".NET Master 🚀";
        
            if (selectedOptions.Any(o => o.Value == "python")) 
                personaName = "Pythonista 🐍";
            if (selectedOptions.Any(o => o.Value == "javascript")) 
                personaName = "JS Ninja ⚔️";
            if (selectedOptions.Any(o => o.Value == "golang")) 
                personaName = "Go Guru 🦫";
        }
}