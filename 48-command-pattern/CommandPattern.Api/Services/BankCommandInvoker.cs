using CommandPattern.Api.Commands;
using CommandPattern.Api.Models;

namespace CommandPattern.Api.Services;

/// <summary>
/// The Invoker — executes commands, maintains an undo stack and a redo stack.
/// The invoker knows nothing about the actual operations; it delegates entirely
/// to the <see cref="IBankCommand"/> interface.
/// </summary>
public class BankCommandInvoker
{
    private readonly Stack<IBankCommand> _undoStack = new();
    private readonly Stack<IBankCommand> _redoStack = new();
    private readonly List<CommandHistoryEntry> _history = [];
    private int _step;

    // ── Core operations ───────────────────────────────────────────────────────

    /// <summary>Executes a command and pushes it onto the undo stack.</summary>
    public void Execute(IBankCommand command)
    {
        command.Execute();

        _undoStack.Push(command);
        _redoStack.Clear(); // a new execution invalidates the redo branch

        _history.Add(new CommandHistoryEntry(
            ++_step,
            command.Name,
            command.Description,
            IsUndone: false,
            DateTime.UtcNow));
    }

    /// <summary>
    /// Undoes the most recent command and moves it to the redo stack.
    /// Throws <see cref="InvalidOperationException"/> if there is nothing to undo.
    /// </summary>
    public void Undo()
    {
        if (_undoStack.Count == 0)
            throw new InvalidOperationException("Nothing to undo.");

        var command = _undoStack.Pop();
        command.Undo();
        _redoStack.Push(command);

        // Mark the matching history entry as undone
        var entry = _history.Last(e => e.CommandName == command.Name && !e.IsUndone);
        var idx   = _history.IndexOf(entry);
        _history[idx] = entry with { IsUndone = true };
    }

    /// <summary>
    /// Re-executes the most recently undone command.
    /// Throws <see cref="InvalidOperationException"/> if there is nothing to redo.
    /// </summary>
    public void Redo()
    {
        if (_redoStack.Count == 0)
            throw new InvalidOperationException("Nothing to redo.");

        var command = _redoStack.Pop();
        command.Execute();
        _undoStack.Push(command);

        _history.Add(new CommandHistoryEntry(
            ++_step,
            command.Name,
            $"[Redo] {command.Description}",
            IsUndone: false,
            DateTime.UtcNow));
    }

    // ── Query helpers ─────────────────────────────────────────────────────────

    /// <summary>Returns the full command execution history in order.</summary>
    public IReadOnlyList<CommandHistoryEntry> History => _history;

    /// <summary>True when there is at least one command that can be undone.</summary>
    public bool CanUndo => _undoStack.Count > 0;

    /// <summary>True when there is at least one command that can be redone.</summary>
    public bool CanRedo => _redoStack.Count > 0;
}
