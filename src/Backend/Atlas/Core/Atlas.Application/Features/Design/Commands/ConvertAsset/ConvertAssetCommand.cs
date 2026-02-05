using MediatR;
using Microsoft.AspNetCore.Http;

namespace Atlas.Application.Features.Design.Commands.ConvertAsset;

public record AssetConversionResult(Stream FileStream, string ContentType, string FileName);

public record ConvertAssetCommand(IFormFile File, string TargetFormat) : IRequest<AssetConversionResult>;