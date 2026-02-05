namespace Atlas.Application.Common.Interfaces;

public interface IImageProcessingService
{
    Task<Stream> ConvertImageAsync(Stream inputStream, string targetFormat, CancellationToken cancellationToken);
}