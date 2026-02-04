using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Knowledge.Dtos;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Atlas.Application.Features.Knowledge.Queries.GetNotionPages;

public class GetNotionPagesQueryHandler(INotionService notionService, IConfiguration configuration)
    : IRequestHandler<GetNotionPagesQuery, List<NoteDto>>
{
    public async Task<List<NoteDto>> Handle(GetNotionPagesQuery request, CancellationToken cancellationToken)
    {
        var token = configuration.GetSection("NotionSettings:IntegrationToken").Value;
        var dbId = configuration.GetSection("NotionSettings:DatabaseId").Value;

        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(dbId))
            throw new Exception("Notion Token or Database ID is missing in configuration!");
        
        return await notionService.GetImportantPagesAsync(dbId, token, cancellationToken);
    }
}