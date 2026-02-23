using MediatR;

namespace Atlas.Application.Features.MarketerUtilities.Commands.GenerateCopywriting;

public record GenerateCopywritingCommand(string ProductName, string Tone = "professional") : IRequest<string>;

