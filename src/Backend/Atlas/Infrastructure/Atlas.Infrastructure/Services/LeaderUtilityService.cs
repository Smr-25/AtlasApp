using Atlas.Application.Common.Interfaces;
using Markdig;

namespace Atlas.Infrastructure.Services;

public class LeaderUtilityService : ILeaderUtilityService
{
    public TimezoneConversionResult ConvertTimezones(List<TeamMemberTimezone> members)
    {
        var utcNow = DateTime.UtcNow;
        var memberTimes = members.Select(m =>
        {
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(m.TimezoneId);
                var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, tz);
                return new MemberLocalTime(m.MemberName, m.TimezoneId, localTime.ToString("HH:mm"), tz.BaseUtcOffset.ToString());
            }
            catch
            {
                return new MemberLocalTime(m.MemberName, m.TimezoneId, "Unknown", "N/A");
            }
        }).ToList();

        return new TimezoneConversionResult(utcNow, memberTimes);
    }

    public QuickPollResult GenerateQuickPoll(string question, List<string> options)
    {
        var pollId = Guid.NewGuid().ToString("N")[..8];
        var optionsText = string.Join("\n", options.Select((o, i) => $":{i + 1}: {o}"));
        var message = $"📊 *{question}*\n\n{optionsText}\n\nReact to vote!";
        return new QuickPollResult(pollId, message);
    }

    public CapacityResult CalculateCapacity(List<MemberCapacityInput> members)
    {
        var capacities = members.Select(m =>
        {
            var weeklyHours = (m.HoursPerDay * (5 - m.DaysOff)) - m.MeetingHoursPerWeek;
            return new MemberCapacity(m.MemberName, Math.Max(0, weeklyHours));
        }).ToList();

        return new CapacityResult(capacities.Sum(c => c.AvailableHours), capacities);
    }

    public CostEstimateResult EstimateCost(double hoursEstimated, double hourlyRate, double serverMonthlyCost, int estimatedMonths)
    {
        var laborCost = hoursEstimated * hourlyRate;
        var infraCost = serverMonthlyCost * estimatedMonths;
        var total = laborCost + infraCost;
        var breakdown = $"Labor: {laborCost:C} ({hoursEstimated}h × {hourlyRate:C}/h)\nInfra: {infraCost:C} ({serverMonthlyCost:C}/mo × {estimatedMonths}mo)\nTotal: {total:C}";
        return new CostEstimateResult(laborCost, infraCost, total, breakdown);
    }

    public RiskMatrixResult GenerateRiskMatrix(List<RiskItem> items)
    {
        var categorized = items.Select(i => new CategorizedRisk(i.Title, i.Impact, i.Probability, i.Impact * i.Probability)).ToList();
        var urgent = categorized.Where(c => c.Score >= 7).OrderByDescending(c => c.Score).ToList();
        var important = categorized.Where(c => c.Score >= 4 && c.Score < 7).OrderByDescending(c => c.Score).ToList();
        var later = categorized.Where(c => c.Score < 4).OrderByDescending(c => c.Score).ToList();
        return new RiskMatrixResult(urgent, important, later);
    }

    public DecisionLogEntry CreateDecisionLogEntry(string decision, string rationale, string decidedBy)
    {
        return new DecisionLogEntry(Guid.NewGuid(), decision, rationale, decidedBy, DateTime.UtcNow);
    }

    public string RenderMarkdownPreview(string markdown)
    {
        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        return Markdown.ToHtml(markdown, pipeline);
    }
}

