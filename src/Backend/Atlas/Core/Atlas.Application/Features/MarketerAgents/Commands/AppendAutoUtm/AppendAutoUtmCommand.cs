using MediatR;

namespace Atlas.Application.Features.MarketerAgents.Commands.AppendAutoUtm;

public record AppendAutoUtmCommand(string Url, string Source, string Medium, string Campaign) : IRequest<string>;

