using Atlas.Application.Common.Models;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Accounts.Commands.GenerateTelegramLinkCode;

public class GenerateTelegramLinkCodeCommandHandler(UserManager<AppUser> userManager)
    : IRequestHandler<GenerateTelegramLinkCodeCommand, ResponseModel<string>>
{
    public async Task<ResponseModel<string>> Handle(GenerateTelegramLinkCodeCommand request, CancellationToken cancellationToken)
    {
        
    }
}