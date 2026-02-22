namespace Atlas.Application.Common.Interfaces;

public interface IDesignUtilityService
{
    Task<byte[]> RemoveBackgroundAsync(Stream imageStream, CancellationToken ct);
    Task<string> IdentifyFontAsync(Stream imageStream, CancellationToken ct);
    string OptimizeSvg(string svgContent);
    ContrastCheckResult CheckContrast(string foregroundHex, string backgroundHex);
    AspectRatioResult CalculateAspectRatio(int width, int height);
    List<string> ExtractColorsFromImage(Stream imageStream, int count = 5);
    string ExtractCssVariables(List<ColorVariable> colors, string format = "css");
}

public record ContrastCheckResult(double Ratio, bool PassesAA, bool PassesAAA, string Level);
public record AspectRatioResult(string Ratio, int SimplifiedWidth, int SimplifiedHeight);
public record ColorVariable(string Name, string HexCode);

