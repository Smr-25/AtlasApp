using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.Design.Commands.CompressImage;

public class CompressImageCommandHandler(
    IImageProcessingService imageProcessing
) : IRequestHandler<CompressImageCommand, CompressImageResult>
{
    public async Task<CompressImageResult> Handle(CompressImageCommand request, CancellationToken cancellationToken)
    {
        var fileInfo = new FileInfo(request.FilePath);
        var originalSize = fileInfo.Length;

        await using var inputStream = File.OpenRead(request.FilePath);
        var outputStream = await imageProcessing.ConvertImageAsync(inputStream, "webp", cancellationToken);

        var outputPath = Path.ChangeExtension(request.FilePath, ".webp");
        await using var fileStream = File.Create(outputPath);
        await outputStream.CopyToAsync(fileStream, cancellationToken);

        var compressedSize = new FileInfo(outputPath).Length;
        var savedPercent = originalSize > 0 ? (1.0 - (double)compressedSize / originalSize) * 100 : 0;

        return new CompressImageResult(outputPath, originalSize, compressedSize, Math.Round(savedPercent, 2));
    }
}

