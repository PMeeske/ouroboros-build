using Ouroboros.Abstractions.Monads;
using Ouroboros.Tools;

namespace Ouroboros.Tests.Mocks;

/// <summary>
/// Mock tool with a delay for testing async behavior.
/// </summary>
public class DelayedMockTool : ITool
{
    private readonly string result;
    private readonly TimeSpan delay;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelayedMockTool"/> class.
    /// </summary>
    /// <param name="name">The tool name.</param>
    /// <param name="result">The result to return after delay.</param>
    /// <param name="delay">The delay duration.</param>
    public DelayedMockTool(string name, string result, TimeSpan delay)
    {
        Name = name;
        this.result = result;
        this.delay = delay;
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string Description => "A delayed tool";

    /// <inheritdoc />
    public string? JsonSchema => null;

    /// <inheritdoc />
    public async Task<Result<string, string>> InvokeAsync(string input, CancellationToken ct = default)
    {
        await Task.Delay(delay, ct);
        return Result<string, string>.Success(result);
    }
}