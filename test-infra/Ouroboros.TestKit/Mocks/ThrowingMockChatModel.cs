using Ouroboros.Abstractions.Core;

namespace Ouroboros.Tests.Mocks;

/// <summary>
/// Mock implementation of IChatCompletionModel that throws exceptions.
/// </summary>
public class ThrowingMockChatModel : IChatCompletionModel
{
    private readonly Exception exception;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThrowingMockChatModel"/> class.
    /// </summary>
    /// <param name="exception">The exception to throw.</param>
    public ThrowingMockChatModel(Exception exception)
    {
        this.exception = exception;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThrowingMockChatModel"/> class with default exception.
    /// </summary>
    public ThrowingMockChatModel()
        : this(new InvalidOperationException("Mock model error"))
    {
    }

    /// <summary>
    /// Always throws the configured exception.
    /// </summary>
    public Task<string> GenerateTextAsync(string prompt, CancellationToken ct = default)
    {
        throw this.exception;
    }
}