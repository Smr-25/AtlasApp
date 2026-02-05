using Atlas.Application.Common.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;

namespace Atlas.Infrastructure.Services;

public class ImageProcessingService : IImageProcessingService
{
    public async Task<Stream> ConvertImageAsync(Stream inputStream, string targetFormat, CancellationToken cancellationToken)
    {
        inputStream.Position = 0;

        using var image = await Image.LoadAsync(inputStream, cancellationToken);
        
        var outputStream = new MemoryStream();
        IImageEncoder encoder = targetFormat.ToLower() switch
        {
            "png" => new PngEncoder(),
            "jpg" or "jpeg" => new JpegEncoder { Quality = 80 }, 
            "webp" => new WebpEncoder { Quality = 75 }, 
            _ => throw new ArgumentException("Unsupported format")
        };

        await image.SaveAsync(outputStream, encoder, cancellationToken);
        
        outputStream.Position = 0;
        return outputStream;
    }
}