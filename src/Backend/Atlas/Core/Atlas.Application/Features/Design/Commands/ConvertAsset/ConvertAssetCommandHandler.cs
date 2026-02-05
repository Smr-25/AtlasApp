using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;

namespace Atlas.Application.Features.Design.Commands.ConvertAsset;

public class ConvertAssetCommandHandler(
    IApplicationDbContext applicationDbContext,
    IImageProcessingService imageService,
    ICurrentUserService currentUserService
) : IRequestHandler<ConvertAssetCommand, AssetConversionResult>
{
    public async Task<AssetConversionResult> Handle(ConvertAssetCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        await using var stream = request.File.OpenReadStream();
        var originalSize = request.File.Length;
        
        var convertedStream = await imageService.ConvertImageAsync(stream, request.TargetFormat, cancellationToken);
        var newSize = convertedStream.Length;
        
        var asset = DesignAsset.Create(
            userId,
            request.File.FileName,
            request.TargetFormat,
            originalSize,
            "MemoryStream" 
        );
        asset.SetConvertedSize(newSize);

        await applicationDbContext.DesignAssets.AddAsync(asset, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        var newFileName = $"{Path.GetFileNameWithoutExtension(request.File.FileName)}.{request.TargetFormat}";
        var contentType = $"image/{request.TargetFormat}";

        return new AssetConversionResult(convertedStream, contentType, newFileName);
    }
}