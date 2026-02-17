using Ouroboros.Abstractions.Monads;
using Ouroboros.Tools;

namespace Ouroboros.Tests.Mocks;

/// <summary>
/// Mock tool that always succeeds with a fixed result.
/// </summary>
public class SuccessMockTool : ITool
{
    private readonly string result;

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessMockTool"/> class.
    /// </summary>
    /// <param name="name">The tool name.</param>
    /// <param name="result">The result to return.</param>
    public SuccessMockTool(string name, string result)
    {
        Name = name;
        this.result = result;
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string Description => $"Always returns: {result}";

    /// <inheritdoc />
    public string? JsonSchema => null;

    /// <inheritdoc />
    public Task<Result<string, string>> InvokeAsync(string input, CancellationToken ct = default)
    {
        return Task.FromResult(Result<string, string>.Success(result));
    }
}