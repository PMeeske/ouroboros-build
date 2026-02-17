// <copyright file="MockChatModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Ouroboros.Tests.Mocks;

using Ouroboros.Abstractions.Core;

/// <summary>
/// Mock implementation of IChatCompletionModel for testing purposes.
/// </summary>
public class MockChatModel : IChatCompletionModel
{
    private readonly string response;
    private readonly bool shouldCheckCancellation;
    private readonly Func<string, string>? responseFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockChatModel"/> class with a fixed response.
    /// </summary>
    /// <param name="response">The response to return.</param>
    /// <param name="shouldCheckCancellation">Whether to check cancellation token.</param>
    public MockChatModel(string response, bool shouldCheckCancellation = false)
    {
        this.response = response;
        this.shouldCheckCancellation = shouldCheckCancellation;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MockChatModel"/> class with a response factory.
    /// </summary>
    /// <param name="responseFactory">Factory function to generate responses based on prompts.</param>
    public MockChatModel(Func<string, string> responseFactory)
    {
        this.responseFactory = responseFactory;
        this.response = string.Empty;
    }

    /// <summary>
    /// Gets the number of times GenerateTextAsync was called.
    /// </summary>
    public int CallCount { get; private set; }

    /// <summary>
    /// Gets the last prompt passed to GenerateTextAsync.
    /// </summary>
    public string? LastPrompt { get; private set; }

    /// <summary>
    /// Generates text using the mock configuration.
    /// </summary>
    /// <param name="prompt">The prompt to respond to.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The configured response.</returns>
    public Task<string> GenerateTextAsync(string prompt, CancellationToken ct = default)
    {
        CallCount++;
        LastPrompt = prompt;

        if (this.shouldCheckCancellation)
        {
            ct.ThrowIfCancellationRequested();
        }

        var result = this.responseFactory != null ? this.responseFactory(prompt) : this.response;
        return Task.FromResult(result);
    }
}