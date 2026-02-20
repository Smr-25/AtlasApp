using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;
using Atlas.Domain.Events;
using Atlas.Domain.Exceptions;

namespace Atlas.Domain.Entities;

public class Team : BaseEntity
{
    public string Name { get; private set; } = null!;
    public Guid OwnerUserId { get; private set; }
    public int MaxMembers { get; private set; } = 7; // 1 Manager + 6 Members

    private readonly List<TeamMember> _members = [];
    public IReadOnlyCollection<TeamMember> Members => _members.AsReadOnly();

    private Team() { }

    public static Team Create(string name, Guid ownerUserId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidEntityStateException(nameof(Team), nameof(Name), "Team name cannot be empty.");

        var team = new Team
        {
            Name = name.Trim(),
            OwnerUserId = ownerUserId,
            MaxMembers = 7
        };

        // Owner is automatically Manager
        team._members.Add(TeamMember.Create(team.Id, ownerUserId, TeamMemberRole.Manager));

        return team;
    }

    public void AddMember(Guid userId)
    {
        if (_members.Count >= MaxMembers)
            throw new BusinessRuleViolationException("TeamLimit", $"Team cannot have more than {MaxMembers} members.");

        if (_members.Any(m => m.UserId == userId && !m.IsDeleted))
            throw new BusinessRuleViolationException("Duplicate", "User is already a member of this team.");

        _members.Add(TeamMember.Create(Id, userId, TeamMemberRole.Member));
        AddDomainEvent(new TeamMemberJoinedEvent(Id, userId));
        SetModified();
    }

    public void RemoveMember(Guid userId)
    {
        if (userId == OwnerUserId)
            throw new BusinessRuleViolationException("OwnerRemoval", "Cannot remove the team owner.");

        var member = _members.FirstOrDefault(m => m.UserId == userId && !m.IsDeleted);
        if (member == null)
            throw new BusinessRuleViolationException("NotFound", "Member not found in this team.");

        member.Remove();
        SetModified();
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidEntityStateException(nameof(Team), nameof(Name), "Team name cannot be empty.");

        Name = name.Trim();
        SetModified();
    }

    public void Delete()
    {
        SetDelete();
    }
}

