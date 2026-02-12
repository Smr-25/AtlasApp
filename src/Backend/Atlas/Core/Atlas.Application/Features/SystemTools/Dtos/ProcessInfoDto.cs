namespace Atlas.Application.Features.SystemTools.Dtos;

public record ProcessInfoDto(
    int Pid,
    string Name,
    int Port,
    bool IsFound
);