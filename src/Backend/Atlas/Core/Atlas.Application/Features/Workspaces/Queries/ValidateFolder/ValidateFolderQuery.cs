using MediatR;

namespace Atlas.Application.Features.Workspaces.Queries.ValidateFolder;

public record ValidateFolderQuery(string FolderPath) : IRequest<FolderValidationDto>;

public record FolderValidationDto(
    bool Exists,
    string Path,
    long SizeInBytes,
    int SubFolderCount,
    int FileCount
);

