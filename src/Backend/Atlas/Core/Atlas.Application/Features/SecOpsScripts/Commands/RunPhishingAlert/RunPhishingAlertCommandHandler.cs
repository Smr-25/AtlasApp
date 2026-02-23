using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsScripts.Commands.RunPhishingAlert;

public class RunPhishingAlertCommandHandler(
    IScriptRunnerService scriptRunner
) : IRequestHandler<RunPhishingAlertCommand, string>
{
    public async Task<string> Handle(RunPhishingAlertCommand request, CancellationToken cancellationToken)
    {
        var spfResult = await scriptRunner.ExecuteAsync(
            "nslookup", $"-type=txt {request.SenderAddress.Split('@').LastOrDefault()}", ".", cancellationToken);

        var hasSPF = spfResult.Contains("v=spf1", StringComparison.OrdinalIgnoreCase);
        var hasDKIM = request.EmailHeaders.Contains("dkim=pass", StringComparison.OrdinalIgnoreCase);
        var hasDMARC = request.EmailHeaders.Contains("dmarc=pass", StringComparison.OrdinalIgnoreCase);

        var verdict = hasSPF && hasDKIM && hasDMARC
            ? "LEGITIMATE - All email authentication checks passed."
            : $"SUSPICIOUS - SPF: {hasSPF}, DKIM: {hasDKIM}, DMARC: {hasDMARC}. Exercise caution.";

        return verdict;
    }
}

