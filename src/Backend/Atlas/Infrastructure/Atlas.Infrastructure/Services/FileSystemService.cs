using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Services;

public class FileSystemService : IFileSystemService
{
    public bool FolderExists(string path) => Directory.Exists(path);

    public long GetFolderSize(string path)
    {
        if (!Directory.Exists(path)) return 0;
        
        return new DirectoryInfo(path)
            .EnumerateFiles("*", SearchOption.AllDirectories)
            .Sum(file => file.Length);
    }

    public IEnumerable<string> GetSubFolders(string path)
    {
        if (!Directory.Exists(path)) return [];
        return Directory.GetDirectories(path);
    }

    public IEnumerable<string> GetFiles(string path, string? searchPattern = null)
    {
        if (!Directory.Exists(path)) return [];
        return Directory.GetFiles(path, searchPattern ?? "*", SearchOption.TopDirectoryOnly);
    }
}

