namespace CommandPattern.Api.Commands;

/// <summary>
/// Command interface — every banking operation implements this contract.
/// Supports both Execute and Undo to enable full command history replay.
/// </summary>
public interface IBankCommand
{
    /// <summary>Human-readable name used in the history log.</summary>
    string Name { get; }

    /// <summary>Short description of what this command does (e.g. "Deposit $500 to account X").</summary>
    string Description { get; }

    /// <summary>Executes the command, applying the operation to the receiver.</summary>
    void Execute();

    /// <summary>Reverses the effect of <see cref="Execute"/> — the inverse operation.</summary>
    void Undo();
}
