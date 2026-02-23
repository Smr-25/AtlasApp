namespace Atlas.Application.Common.Interfaces;

public interface ILeaderUtilityService
{
    TimezoneConversionResult ConvertTimezones(List<TeamMemberTimezone> members);
    QuickPollResult GenerateQuickPoll(string question, List<string> options);
    CapacityResult CalculateCapacity(List<MemberCapacityInput> members);
    CostEstimateResult EstimateCost(double hoursEstimated, double hourlyRate, double serverMonthlyCost, int estimatedMonths);
    RiskMatrixResult GenerateRiskMatrix(List<RiskItem> items);
    DecisionLogEntry CreateDecisionLogEntry(string decision, string rationale, string decidedBy);
    string RenderMarkdownPreview(string markdown);
}

public record TeamMemberTimezone(string MemberName, string TimezoneId);
public record TimezoneConversionResult(DateTime UtcNow, List<MemberLocalTime> MemberTimes);
public record MemberLocalTime(string MemberName, string TimezoneId, string LocalTime, string Offset);
public record QuickPollResult(string PollId, string FormattedMessage);
public record MemberCapacityInput(string MemberName, double HoursPerDay, int DaysOff, int MeetingHoursPerWeek);
public record CapacityResult(double TotalAvailableHours, List<MemberCapacity> Members);
public record MemberCapacity(string MemberName, double AvailableHours);
public record CostEstimateResult(double LaborCost, double InfrastructureCost, double TotalCost, string Breakdown);
public record RiskItem(string Title, int Impact, int Probability);
public record RiskMatrixResult(List<CategorizedRisk> Urgent, List<CategorizedRisk> Important, List<CategorizedRisk> Later);
public record CategorizedRisk(string Title, int Impact, int Probability, int Score);
public record DecisionLogEntry(Guid Id, string Decision, string Rationale, string DecidedBy, DateTime RecordedAt);

