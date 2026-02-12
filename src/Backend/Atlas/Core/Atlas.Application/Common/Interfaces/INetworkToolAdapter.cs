using Atlas.Application.Features.NetworkTools.Dtos;

namespace Atlas.Application.Common.Interfaces;

public interface INetworkToolAdapter
{
    Task<HttpResponseDto> SendRequestAsync(HttpRequestDto request, CancellationToken ct);
}