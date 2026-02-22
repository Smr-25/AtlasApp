using Atlas.Application.Features.LottieFiles.Dtos;
using MediatR;

namespace Atlas.Application.Features.LottieFiles.Queries.SearchAnimations;

public record SearchLottieAnimationsQuery(Guid IntegrationId, string Query) : IRequest<List<LottieAnimationDto>>;

