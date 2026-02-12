using System.Diagnostics;
using System.Text;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.NetworkTools.Dtos;
using Microsoft.Extensions.Logging;

namespace Atlas.Infrastructure.Adapters;

public class NetworkToolAdapter(IHttpClientFactory httpClientFactory, ILogger<NetworkToolAdapter> logger) : INetworkToolAdapter
{
    public async Task<HttpResponseDto> SendRequestAsync(HttpRequestDto request, CancellationToken ct)
    {
        logger.LogInformation("Sending {Method} request to {Url}", request.Method, request.Url);

        var client = httpClientFactory.CreateClient("AtlasClient");
        
        var httpRequest = new HttpRequestMessage(new HttpMethod(request.Method), request.Url);

        if (request.Headers != null)
        {
            foreach (var header in request.Headers)
                httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            
        }
        
        if (!string.IsNullOrEmpty(request.Body) && 
            (request.Method == "POST" || request.Method == "PUT" || request.Method == "PATCH"))
            httpRequest.Content = new StringContent(request.Body, Encoding.UTF8, "application/json");
        

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var response = await client.SendAsync(httpRequest, ct);
            stopwatch.Stop();

            var content = await response.Content.ReadAsStringAsync(ct);

            logger.LogInformation("Request completed with status {StatusCode} in {Time}ms", response.StatusCode, stopwatch.ElapsedMilliseconds);

            return new HttpResponseDto(
                (int)response.StatusCode,
                response.ReasonPhrase ?? "Unknown",
                content,
                stopwatch.ElapsedMilliseconds,
                response.IsSuccessStatusCode
            );
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP Request failed");
            return new HttpResponseDto(0, "Connection Error", ex.Message, stopwatch.ElapsedMilliseconds, false);
        }catch (TaskCanceledException)
        {
            return new HttpResponseDto(408, "Timeout", "Request timed out.", stopwatch.ElapsedMilliseconds, false);
        }
    }
}