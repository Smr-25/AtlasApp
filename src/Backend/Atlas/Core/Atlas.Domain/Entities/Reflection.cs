using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class Reflection : BaseEntity
{
    public ReflectionType Type { get; private set; } = ReflectionType.General;
    public string Content { get; private set; } = null!;
    public int? MoodScore { get; private set; }
    public List<string> Tags { get; private set; } = [];
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public bool IsPrivate { get; private set; } = true;
    public Guid PersonaId { get; private set; }
    public Persona Persona { get; private set; } = null!;
    public Guid? DecisionId { get; private set; }
    public Decision? Decision { get; private set; }

    public static Reflection Create(Guid personaId, string content, ReflectionType type = ReflectionType.General,
        Guid? decisionId = null, int? moodScore = null, List<string>? tags = null)
    {
        var reflection = new Reflection
        {
            PersonaId = personaId,
            Content = content,
            Type = type,
            DecisionId = decisionId,
            MoodScore = moodScore,
            Tags = tags ?? []
        };
        return reflection;
    }

    public void UpdateContent(string newContent)
    {
        Content = newContent;
    }

    public void AddTag(string tag)
    {
        Tags.Add(tag);
    }

    public void RemoveTag(string tag)
    {
        Tags.Remove(tag);
    }

    public void SetMoodScore(int score)
    {
        MoodScore = score;
    }
}