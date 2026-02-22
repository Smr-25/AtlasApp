using MediatR;

namespace Atlas.Application.Features.Figma.Commands.ResolveComment;

public record ResolveFigmaCommentCommand(Guid IntegrationId, string FileKey, string CommentId) : IRequest;

