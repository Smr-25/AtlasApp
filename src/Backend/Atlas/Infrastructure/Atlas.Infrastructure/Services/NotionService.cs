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

    public async Task<string> SendSnippetToNotionAsync(string title, string code, string language,
        string databaseId, string authToken, CancellationToken cancellationToken = default)
    {
        var client = NotionClientFactory.Create(new ClientOptions { AuthToken = authToken });

        var properties = new Dictionary<string, PropertyValue>
        {
            ["Title"] = new TitlePropertyValue
            {
                Title = [new RichTextText { Text = new Text { Content = title } }]
            },
            ["Language"] = new RichTextPropertyValue
            {
                RichText = [new RichTextText { Text = new Text { Content = language } }]
            }
        };

        var children = new List<IBlock>
        {
            new CodeBlock
            {
                Code = new CodeBlock.Info
                {
                    RichText = [new RichTextText { Text = new Text { Content = code } }],
                    Language = language
                }
            }
        };

        var page = await client.Pages.CreateAsync(new PagesCreateParameters
        {
            Parent = new DatabaseParentInput { DatabaseId = databaseId },
            Properties = properties,
            Children = children
        }, cancellationToken);

        return page.Id;
    }

    public async Task<List<NotionSnippetDto>> FetchSnippetsFromNotionAsync(string databaseId, string authToken,
        int limit = 10, CancellationToken cancellationToken = default)
    {
        var client = NotionClientFactory.Create(new ClientOptions { AuthToken = authToken });

        var queryParams = new DatabasesQueryParameters
        {
            PageSize = limit,
            Sorts = [new Sort { Direction = Direction.Descending, Timestamp = Timestamp.LastEditedTime }]
        };

        var result = await client.Databases.QueryAsync(databaseId, queryParams, cancellationToken);
        var snippets = new List<NotionSnippetDto>();

        foreach (var item in result.Results)
        {
            if (item is not Page page) continue;

            var title = "Untitled";
            var language = "text";

            foreach (var prop in page.Properties)
            {
                if (prop.Value is TitlePropertyValue titleProp && titleProp.Title.Count > 0)
                    title = titleProp.Title[0].PlainText;
                if (prop.Key == "Language" && prop.Value is RichTextPropertyValue langProp && langProp.RichText.Count > 0)
                    language = langProp.RichText[0].PlainText;
            }

            var code = "";
            try
            {
                var blockRequest = new BlockRetrieveChildrenRequest { BlockId = page.Id };
                var blocks = await client.Blocks.RetrieveChildrenAsync(blockRequest, cancellationToken);
                foreach (var block in blocks.Results)
                {
                    if (block is CodeBlock codeBlock && codeBlock.Code.RichText.Any())
                    {
                        code = codeBlock.Code.RichText.First().PlainText;
                        break;
                    }
                }
            }
            catch
            {
            }

            snippets.Add(new NotionSnippetDto(
                page.Id,
                title,
                code,
                language,
                page.Url,
                page.LastEditedTime
            ));
        }

        return snippets;
    }
}