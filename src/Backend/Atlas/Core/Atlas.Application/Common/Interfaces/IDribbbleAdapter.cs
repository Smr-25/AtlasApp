using Atlas.Application.Features.Dribbble.Dtos;

namespace Atlas.Application.Common.Interfaces;

public interface IDribbbleAdapter
{
    Task<List<DribbbleShotDto>> GetShotsAsync(string accessToken, CancellationToken ct);
    Task<List<DribbbleShotDto>> SearchInspirationAsync(string accessToken, string query, CancellationToken ct);
}

