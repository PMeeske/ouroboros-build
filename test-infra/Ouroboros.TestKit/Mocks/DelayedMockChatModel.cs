using Ouroboros.Abstractions.Core;

namespace Ouroboros.Tests.Mocks;

/// <summary>
/// Mock implementation that returns responses with simulated delay.
/// </summary>
public class DelayedMockChatModel : IChatCompletionModel
{
    private readonly string response;
    private readonly TimeSpan delay;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelayedMockChatModel"/> class.
    /// </summary>
    /// <param name="response">The response to return.</param>
    /// <param name="delay">The delay before returning.</param>
    public DelayedMockChatModel(string response, TimeSpan delay)
    {
        this.response = response;
        this.delay = delay;
    }

    /// <summary>
    /// Generates text after a simulated delay.
    /// </summary>
    public async Task<string> GenerateTextAsync(string prompt, CancellationToken ct = default)
    {
        await Task.Delay(this.delay, ct);
        return this.response;
    }
}