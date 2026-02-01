using Atlas.Application.Features.Accounts.Dtos;
using MediatR;

namespace Atlas.Application.Features.Accounts.Queries;

public record GetProfessionsQuery : IRequest<List<ProfessionDto>>;