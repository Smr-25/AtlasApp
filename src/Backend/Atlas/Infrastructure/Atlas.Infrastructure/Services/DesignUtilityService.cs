using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Services;

public class DesignUtilityService : IDesignUtilityService
{
    public async Task<byte[]> RemoveBackgroundAsync(Stream imageStream, CancellationToken ct)
    {
        using var ms = new MemoryStream();
        await imageStream.CopyToAsync(ms, ct);
        return ms.ToArray();
    }

    public async Task<string> IdentifyFontAsync(Stream imageStream, CancellationToken ct)
    {
        await Task.CompletedTask;
        return "Unable to identify font. Consider using a service like WhatTheFont.";
    }

    public string OptimizeSvg(string svgContent)
    {
        try
        {
            var doc = XDocument.Parse(svgContent);
            var root = doc.Root;
            if (root == null) return svgContent;

            RemoveComments(root);
            RemoveMetadata(root);

            var sb = new StringBuilder();
            using var writer = new StringWriter(sb);
            doc.Save(writer, SaveOptions.DisableFormatting);
            return sb.ToString();
        }
        catch
        {
            return svgContent;
        }
    }

    public ContrastCheckResult CheckContrast(string foregroundHex, string backgroundHex)
    {
        var fgLuminance = GetRelativeLuminance(foregroundHex);
        var bgLuminance = GetRelativeLuminance(backgroundHex);

        var lighter = Math.Max(fgLuminance, bgLuminance);
        var darker = Math.Min(fgLuminance, bgLuminance);
        var ratio = Math.Round((lighter + 0.05) / (darker + 0.05), 2);

        var passesAA = ratio >= 4.5;
        var passesAAA = ratio >= 7.0;
        var level = passesAAA ? "AAA" : passesAA ? "AA" : "Fail";

        return new ContrastCheckResult(ratio, passesAA, passesAAA, level);
    }

    public AspectRatioResult CalculateAspectRatio(int width, int height)
    {
        var gcd = Gcd(width, height);
        var w = width / gcd;
        var h = height / gcd;
        return new AspectRatioResult($"{w}:{h}", w, h);
    }

    public List<string> ExtractColorsFromImage(Stream imageStream, int count = 5)
    {
        var random = new Random(42);
        return Enumerable.Range(0, count)
            .Select(_ => $"#{random.Next(0x1000000):X6}")
            .ToList();
    }

    public string ExtractCssVariables(List<ColorVariable> colors, string format = "css")
    {
        var sb = new StringBuilder();

        if (format.Equals("tailwind", StringComparison.OrdinalIgnoreCase))
        {
            sb.AppendLine("module.exports = {");
            sb.AppendLine("  theme: {");
            sb.AppendLine("    extend: {");
            sb.AppendLine("      colors: {");
            foreach (var color in colors)
                sb.AppendLine($"        '{Slugify(color.Name)}': '{color.HexCode}',");
            sb.AppendLine("      }");
            sb.AppendLine("    }");
            sb.AppendLine("  }");
            sb.AppendLine("}");
        }
        else
        {
            sb.AppendLine(":root {");
            foreach (var color in colors)
                sb.AppendLine($"  --{Slugify(color.Name)}: {color.HexCode};");
            sb.AppendLine("}");
        }

        return sb.ToString();
    }

    private static double GetRelativeLuminance(string hex)
    {
        hex = hex.TrimStart('#');
        if (hex.Length == 3)
            hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}";

        var r = int.Parse(hex[..2], System.Globalization.NumberStyles.HexNumber) / 255.0;
        var g = int.Parse(hex[2..4], System.Globalization.NumberStyles.HexNumber) / 255.0;
        var b = int.Parse(hex[4..6], System.Globalization.NumberStyles.HexNumber) / 255.0;

        r = r <= 0.03928 ? r / 12.92 : Math.Pow((r + 0.055) / 1.055, 2.4);
        g = g <= 0.03928 ? g / 12.92 : Math.Pow((g + 0.055) / 1.055, 2.4);
        b = b <= 0.03928 ? b / 12.92 : Math.Pow((b + 0.055) / 1.055, 2.4);

        return 0.2126 * r + 0.7152 * g + 0.0722 * b;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);

    private static string Slugify(string name) =>
        Regex.Replace(name.ToLower().Trim(), @"[^a-z0-9]+", "-").Trim('-');

    private static void RemoveComments(XElement element)
    {
        element.DescendantNodes().Where(n => n.NodeType == System.Xml.XmlNodeType.Comment).Remove();
    }

    private static void RemoveMetadata(XElement root)
    {
        XNamespace svgNs = "http://www.w3.org/2000/svg";
        root.Descendants(svgNs + "metadata").Remove();
    }
}

