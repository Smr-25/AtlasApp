using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.Workspaces.Queries.ValidateFolder;

public class ValidateFolderQueryHandler(IFileSystemService fileSystemService)
    : IRequestHandler<ValidateFolderQuery, FolderValidationDto>
{
    public Task<FolderValidationDto> Handle(ValidateFolderQuery request, CancellationToken cancellationToken)
    {
        var exists = fileSystemService.FolderExists(request.FolderPath);

        if (!exists)
        {
            return Task.FromResult(new FolderValidationDto(false, request.FolderPath, 0, 0, 0));
        }

        var size = fileSystemService.GetFolderSize(request.FolderPath);
        var subFolders = fileSystemService.GetSubFolders(request.FolderPath).Count();
        var files = fileSystemService.GetFiles(request.FolderPath).Count();

        return Task.FromResult(new FolderValidationDto(true, request.FolderPath, size, subFolders, files));
    }
}

