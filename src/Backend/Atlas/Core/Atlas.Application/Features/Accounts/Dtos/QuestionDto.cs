namespace Atlas.Application.Features.Accounts.Dtos;

public record QuestionDto(Guid Id, string Text, bool IsMultiSelect, List<OptionDto> Options);