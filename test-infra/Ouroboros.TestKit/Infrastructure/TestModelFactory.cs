// <copyright file="TestModelFactory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Ouroboros.Tests.Infrastructure;

using Ouroboros.Domain;
using Ouroboros.Providers;
using Ouroboros.Tests.Mocks;
using Ouroboros.Tools;

/// <summary>
/// Factory for creating model instances in tests.
/// By default uses fast mock models. Can be configured via environment variables
/// to use cloud or local models for integration testing.
/// </summary>
public static class TestModelFactory
{
    private static readonly bool UseRealModels =
        Environment.GetEnvironmentVariable("TEST_USE_CLOUD_MODEL") == "true" ||
        Environment.GetEnvironmentVariable("TEST_USE_LOCAL_MODEL") == "true";

    /// <summary>
    /// Gets whether tests are configured to use real models (cloud or local).
    /// </summary>
    public static bool UsesRealModels => UseRealModels;

    /// <summary>
    /// Creates a chat completion model for testing.
    /// Returns a mock by default for fast tests.
    /// </summary>
    public static IChatCompletionModel CreateChatModel(
        string? response = null,
        Func<string, string>? responseFactory = null)
    {
        if (responseFactory != null)
        {
            return new MockChatModel(responseFactory);
        }

        return new MockChatModel(response ?? GetDefaultMockResponse());
    }

    /// <summary>
    /// Creates an embedding model for testing.
    /// Returns a mock by default for fast tests.
    /// </summary>
    public static IEmbeddingModel CreateEmbeddingModel(int embeddingSize = 384)
    {
        return new MockEmbeddingModel(embeddingSize);
    }

    /// <summary>
    /// Creates a tool-aware chat model for testing.
    /// </summary>
    public static ToolAwareChatModel CreateToolAwareChatModel(
        ToolRegistry? tools = null,
        string? response = null)
    {
        var chatModel = CreateChatModel(response);
        return new ToolAwareChatModel(chatModel, tools ?? new ToolRegistry());
    }

    /// <summary>
    /// Creates both chat and embedding models as a tuple for convenience.
    /// </summary>
    public static (IChatCompletionModel Chat, IEmbeddingModel Embed) CreateModels()
    {
        return (CreateChatModel(), CreateEmbeddingModel());
    }

    /// <summary>
    /// Skips the test if real models are not available.
    /// </summary>
    public static void SkipIfMockModels()
    {
        if (!UsesRealModels)
        {
            throw new InvalidOperationException(
                "Test requires real models. Set TEST_USE_CLOUD_MODEL=true or TEST_USE_LOCAL_MODEL=true");
        }
    }

    private static string GetDefaultMockResponse()
    {
        return "[mock-response] This is a mock LLM response for testing. " +
               "To use real models, set TEST_USE_CLOUD_MODEL=true or TEST_USE_LOCAL_MODEL=true.";
    }
}
