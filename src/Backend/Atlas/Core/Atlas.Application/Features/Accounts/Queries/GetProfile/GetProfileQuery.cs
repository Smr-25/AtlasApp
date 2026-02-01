using Atlas.Application.Features.Accounts.Dtos;
using MediatR;

namespace Atlas.Application.Features.Accounts.Queries.GetProfile;

public record GetProfileQuery : IRequest<AccountDto>;
