namespace Atlas.Application.Common.Interfaces;

public interface IImageTools
{
    Task<Stream> ConvertImageAsync(Stream inputStream, string targetFormat, CancellationToken cancellationToken);
}