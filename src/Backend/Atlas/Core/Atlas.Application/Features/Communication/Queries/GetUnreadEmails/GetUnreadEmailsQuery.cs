using Atlas.Application.Features.Communication.Dtos;
using MediatR;

namespace Atlas.Application.Features.Communication.Queries.GetUnreadEmails;

public record GetUnreadEmailsQuery : IRequest<List<EmailDto>>;