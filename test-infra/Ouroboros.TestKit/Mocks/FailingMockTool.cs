using Ouroboros.Abstractions.Monads;
using Ouroboros.Tools;

namespace Ouroboros.Tests.Mocks;

/// <summary>
/// Mock tool that always fails with a fixed error.
/// </summary>
public class FailingMockTool : ITool
{
    private readonly string error;

    /// <summary>
    /// Initializes a new instance of the <see cref="FailingMockTool"/> class.
    /// </summary>
    /// <param name="name">The tool name.</param>
    /// <param name="error">The error message to return.</param>
    public FailingMockTool(string name, string error)
    {
        Name = name;
        this.error = error;
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string Description => $"Always fails with: {error}";

    /// <inheritdoc />
    public string? JsonSchema => null;

    /// <inheritdoc />
    public Task<Result<string, string>> InvokeAsync(string input, CancellationToken ct = default)
    {
        return Task.FromResult(Result<string, string>.Failure(error));
    }
}