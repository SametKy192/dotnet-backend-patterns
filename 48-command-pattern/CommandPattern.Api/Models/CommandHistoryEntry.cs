namespace CommandPattern.Api.Models;

/// <summary>Represents a single entry in the command execution history.</summary>
public record CommandHistoryEntry(
    int      Step,
    string   CommandName,
    string   Description,
    bool     IsUndone,
    DateTime ExecutedAt);
