using Atlas.Application.Features.Figma.Dtos;
using MediatR;

namespace Atlas.Application.Features.Figma.Queries.GetComments;

public record GetFigmaCommentsQuery(Guid IntegrationId, string FileKey) : IRequest<List<FigmaCommentDto>>;

