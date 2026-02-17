using Ouroboros.Abstractions.Monads;
using Ouroboros.Tools;

namespace Ouroboros.Tests.Mocks;

/// <summary>
/// Mock tool that throws an exception.
/// </summary>
public class ThrowingMockTool : ITool
{
    private readonly Exception exception;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThrowingMockTool"/> class.
    /// </summary>
    /// <param name="name">The tool name.</param>
    /// <param name="exception">The exception to throw.</param>
    public ThrowingMockTool(string name, Exception? exception = null)
    {
        Name = name;
        this.exception = exception ?? new InvalidOperationException("Tool error");
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string Description => "A tool that throws";

    /// <inheritdoc />
    public string? JsonSchema => null;

    /// <inheritdoc />
    public Task<Result<string, string>> InvokeAsync(string input, CancellationToken ct = default)
    {
        throw exception;
    }
}