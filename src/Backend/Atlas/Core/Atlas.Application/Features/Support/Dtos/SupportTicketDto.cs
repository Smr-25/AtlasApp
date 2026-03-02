using Atlas.Domain.Enums;

namespace Atlas.Application.Features.Support.Dtos;

public record SupportTicketDto(
    Guid Id,
    FeedbackType Type,
    FeedbackStatus Status,
    string Subject,
    string Body,
    string? PageUrl,
    string? AdminReply,
    DateTime? RepliedAt,
    DateTimeOffset CreatedAt
);

