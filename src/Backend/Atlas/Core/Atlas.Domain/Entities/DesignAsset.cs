using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class DesignAsset : BaseEntity
{
    public string OriginalFileName { get; private set; }
    public string TargetFormat { get; private set; } 
    public long OriginalSizeBytes { get; private set; }
    public long ConvertedSizeBytes { get; private set; }
    public string FilePath { get; private set; } 
    public bool IsOptimized { get; private set; }
    public long OptimizedSizeBytes { get; private set; }
    public Guid UserId { get; private set; }

    private DesignAsset() { }

    public static DesignAsset Create(
        Guid userId, 
        string originalFileName, 
        string targetFormat, 
        long originalSize, 
        string filePath)
    {
        return new DesignAsset
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OriginalFileName = originalFileName,
            TargetFormat = targetFormat,
            OriginalSizeBytes = originalSize,
            FilePath = filePath,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void SetConvertedSize(long sizeBytes)
    {
        ConvertedSizeBytes = sizeBytes;
        SetModified();
    }

    public void MarkOptimized(long optimizedSize)
    {
        IsOptimized = true;
        OptimizedSizeBytes = optimizedSize;
        SetModified();
    }
}