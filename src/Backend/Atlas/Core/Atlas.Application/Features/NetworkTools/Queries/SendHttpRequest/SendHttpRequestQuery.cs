using Atlas.Application.Features.NetworkTools.Dtos;
using MediatR;

namespace Atlas.Application.Features.NetworkTools.Queries.SendHttpRequest;

public record SendHttpRequestQuery(
    string Url, 
    string Method = "GET", 
    string? Body = null,
    Dictionary<string, string>? Headers = null
) : IRequest<HttpResponseDto>;