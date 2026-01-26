using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger,
    ICurrentUserService currentUserService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = currentUserService.UserId ?? "Anonymous";

        logger.LogInformation("Atlas Request: {Name} {@UserId} {@Request}", requestName, userId, request);

        var response = await next();

        logger.LogInformation("Atlas Response: {Name} {@UserId}", requestName, userId);

        return response;
    }
}