using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.JsonTools.Queries.FormatJson;

public class FormatJsonQueryHandler(IJsonToolService jsonService) 
    : IRequestHandler<FormatJsonQuery, string>
{
    public Task<string> Handle(FormatJsonQuery request, CancellationToken cancellationToken)
    {
        return Task.Run(() => request.Minify 
            ? jsonService.MinifyJson(request.JsonContent) 
            : jsonService.FormatJson(request.JsonContent), cancellationToken);
    }
}