using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Knowledge.Dtos;
using Notion.Client;

namespace Atlas.Infrastructure.Services;

public class NotionService : INotionService
{
    public async Task<List<NoteDto>> GetImportantPagesAsync(string databaseId, string authToken,
        CancellationToken cancellationToken = default)
    {
        var client = NotionClientFactory.Create(new ClientOptions
        {
            AuthToken = authToken
        });

        var queryParams = new DatabasesQueryParameters();
        var result = await client.Databases.QueryAsync(databaseId, queryParams, cancellationToken);
        var notes = new List<NoteDto>();

        foreach (var item in result.Results)
        {
            if (item is not Page page) continue;
            
            var title = "Untitled";

            foreach (var prop in page.Properties)
            {
                if (prop.Value is not TitlePropertyValue titleProp || titleProp.Title.Count == 0) continue;
                title = titleProp.Title[0].PlainText;
                break;
            }

            var icon = "📄";
            if (page.Icon is EmojiObject emojiIcon)
            {
                icon = emojiIcon.Emoji;
            }

            notes.Add(new NoteDto(
                Id: page.Id,
                Title: title,
                Url: page.Url,
                Icon: icon,
                LastEdited: page.LastEditedTime
            ));
        }

        return notes;
    }
}