using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.DevUtilities.Queries.DecodeJwt;

public class DecodeJwtQueryHandler(
    IDevUtilityService devUtility
) : IRequestHandler<DecodeJwtQuery, DecodeJwtResult>
{
    public Task<DecodeJwtResult> Handle(DecodeJwtQuery request, CancellationToken cancellationToken)
    {
        var json = devUtility.DecodeJwt(request.Token);
        var parts = request.Token.Split('.');
        var header = parts.Length > 0 ? System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(PadBase64(parts[0]))) : "";
        var payload = json;

        DateTime? expiresAt = null;
        var isExpired = false;

        if (json.Contains("\"exp\""))
        {
            var expIndex = json.IndexOf("\"exp\"", StringComparison.Ordinal);
            var colonIndex = json.IndexOf(':', expIndex);
            var commaIndex = json.IndexOf(',', colonIndex);
            if (commaIndex == -1) commaIndex = json.IndexOf('}', colonIndex);
            if (colonIndex > 0 && commaIndex > 0)
            {
                var expStr = json[(colonIndex + 1)..commaIndex].Trim();
                if (long.TryParse(expStr, out var expUnix))
                {
                    expiresAt = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
                    isExpired = expiresAt < DateTime.UtcNow;
                }
            }
        }

        return Task.FromResult(new DecodeJwtResult(header, payload, expiresAt, isExpired));
    }

    private static string PadBase64(string input)
    {
        var padded = input.Replace('-', '+').Replace('_', '/');
        switch (padded.Length % 4)
        {
            case 2: padded += "=="; break;
            case 3: padded += "="; break;
        }
        return padded;
    }
}

