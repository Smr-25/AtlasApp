using MediatR;

namespace Atlas.Application.Features.Design.Commands.CompressImage;

public record CompressImageCommand(string FilePath, int Quality = 75) : IRequest<CompressImageResult>;

public record CompressImageResult(string OutputPath, long OriginalSize, long CompressedSize, double SavedPercent);

