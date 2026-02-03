using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;

namespace Atlas.Application.Features.Snippets.Commands.CreateSnippet;

public record CreateSnippetCommand(
    string Title, 
    string Code, 
    string Language, 
    List<string> Tags
) : IRequest<Guid>;

public class CreateSnippetCommandHandler : IRequestHandler<CreateSnippetCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService; // User ID-ni tapmaq üçün

    public CreateSnippetCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateSnippetCommand request, CancellationToken cancellationToken)
    {
        var snippet = Snippet.Create(
            title: request.Title,
            code: request.Code,
            language: request.Language,
            tags: request.Tags,
            userId: _currentUserService.UserId! 
        );

        _context.Snippets.Add(snippet);
        await _context.SaveChangesAsync(cancellationToken);

        return snippet.Id;
    }
}