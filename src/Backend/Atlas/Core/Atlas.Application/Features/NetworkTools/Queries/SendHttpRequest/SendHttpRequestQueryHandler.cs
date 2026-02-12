using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.NetworkTools.Dtos;
using MediatR;

namespace Atlas.Application.Features.NetworkTools.Queries.SendHttpRequest;

public class SendHttpRequestQueryHandler(INetworkToolAdapter networkTool) 
    : IRequestHandler<SendHttpRequestQuery, HttpResponseDto>
{
    public async Task<HttpResponseDto> Handle(SendHttpRequestQuery request, CancellationToken cancellationToken)
    {
        var dto = new HttpRequestDto(request.Url, request.Method, request.Body, request.Headers);
        return await networkTool.SendRequestAsync(dto, cancellationToken);
    }
}