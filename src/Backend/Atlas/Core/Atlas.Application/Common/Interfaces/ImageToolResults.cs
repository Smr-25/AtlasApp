namespace Atlas.Application.Common.Interfaces;

public record ContrastCheckResult(double Ratio, bool PassesAA, bool PassesAAA, string Level);
public record AspectRatioResult(string Ratio, int SimplifiedWidth, int SimplifiedHeight);
public record ColorVariable(string Name, string HexCode);

