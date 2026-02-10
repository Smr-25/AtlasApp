using System.Xml.Linq;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Projects.Dtos;
using Microsoft.Extensions.Logging;

namespace Atlas.Infrastructure.Services;

public class ProjectScannerService(ILogger<ProjectScannerService> logger) : IProjectScannerService
{
    private readonly string _defaultSearchPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); 

    public async Task<List<LocalProjectDto>> ScanForProjectsAsync(string rootPath, CancellationToken ct)
    {
        var projects = new List<LocalProjectDto>();
        var searchPath = string.IsNullOrEmpty(rootPath) ? _defaultSearchPath : rootPath;

        if (!Directory.Exists(searchPath))
        {
            logger.LogWarning("Search path does not exist: {SearchPath}", searchPath);
            return [];
        }

        logger.LogInformation("Scanning for projects in: {SearchPath}", searchPath);
        var projectFiles = Directory.GetFiles(searchPath, "*.csproj", SearchOption.AllDirectories);

        foreach (var file in projectFiles)
        {
            if (file.Contains("node_modules") || file.Contains("/bin/") || file.Contains("/obj/")) 
                continue;

            var projectInfo = await AnalyzeProjectAsync(file, ct);
            if (projectInfo != null)
            {
                projects.Add(projectInfo);
            }
        }

        logger.LogInformation("Found {Count} projects in {SearchPath}", projects.Count, searchPath);
        return projects;
    }

    private async Task<LocalProjectDto?> AnalyzeProjectAsync(string filePath, CancellationToken ct)
    {
        try
        {
            var xml = await File.ReadAllTextAsync(filePath, ct);
            var doc = XDocument.Parse(xml);

            var name = Path.GetFileNameWithoutExtension(filePath);
            var folder = Path.GetDirectoryName(filePath);

            var isWeb = xml.Contains("Sdk=\"Microsoft.NET.Sdk.Web\"");
            var hasEfCore = xml.Contains("Microsoft.EntityFrameworkCore");
            var hasEfDesign = xml.Contains("Microsoft.EntityFrameworkCore.Design");

            var type = "Library";
            if (isWeb) type = "API/Web";
            else if (hasEfDesign) type = "Database/Migration";
            else if (name.EndsWith("Tests")) type = "Test";

            return new LocalProjectDto(
                Id: Guid.NewGuid(),
                Name: name,
                Path: filePath,
                Directory: folder,
                Type: type,
                HasEfCore: hasEfCore,
                TargetFramework: GetTargetFramework(doc)
            );
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to analyze project: {FilePath}", filePath);
            return null; 
        }
    }

    private static string GetTargetFramework(XDocument doc)
    {
        var targetFramework = doc.Descendants("TargetFramework").FirstOrDefault()?.Value 
                              ?? doc.Descendants("TargetFrameworks").FirstOrDefault()?.Value;
        return targetFramework ?? "Unknown";
    }
}

