namespace Atlas.Application.Common.Interfaces;

public interface IFileSystemService
{
    bool FolderExists(string path);
    long GetFolderSize(string path);
    IEnumerable<string> GetSubFolders(string path);
    IEnumerable<string> GetFiles(string path, string? searchPattern = null);
}

