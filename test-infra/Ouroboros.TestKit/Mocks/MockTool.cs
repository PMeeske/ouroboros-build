// <copyright file="MockTool.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Ouroboros.Tests.Mocks;

using Ouroboros.Abstractions.Monads;
using Ouroboros.Core.Monads;
using Ouroboros.Tools;

/// <summary>
/// Mock implementation of ITool for testing purposes.
/// </summary>
public class MockTool : ITool
{
    private readonly Func<string, CancellationToken, Task<Result<string, string>>> invokeFunc;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockTool"/> class.
    /// </summary>
    /// <param name="name">The tool name.</param>
    /// <param name="description">The tool description.</param>
    /// <param name="jsonSchema">Optional JSON schema.</param>
    public MockTool(string name, string description = "Mock tool", string? jsonSchema = null)
    {
        Name = name;
        Description = description;
        JsonSchema = jsonSchema;
        this.invokeFunc = (input, ct) => Task.FromResult(Result<string, string>.Success($"Result: {input}"));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MockTool"/> class with custom invoke behavior.
    /// </summary>
    /// <param name="name">The tool name.</param>
    /// <param name="description">The tool description.</param>
    /// <param name="invokeFunc">Custom invoke function.</param>
    public MockTool(string name, string description, Func<string, Result<string, string>> invokeFunc)
    {
        Name = name;
        Description = description;
        this.invokeFunc = (input, ct) => Task.FromResult(invokeFunc(input));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MockTool"/> class with async invoke behavior.
    /// </summary>
    /// <param name="name">The tool name.</param>
    /// <param name="description">The tool description.</param>
    /// <param name="invokeFunc">Async custom invoke function.</param>
    public MockTool(string name, string description, Func<string, CancellationToken, Task<Result<string, string>>> invokeFunc)
    {
        Name = name;
        Description = description;
        this.invokeFunc = invokeFunc;
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string Description { get; }

    /// <inheritdoc />
    public string? JsonSchema { get; }

    /// <summary>
    /// Gets the number of times InvokeAsync was called.
    /// </summary>
    public int InvokeCount { get; private set; }

    /// <summary>
    /// Gets the last input passed to InvokeAsync.
    /// </summary>
    public string? LastInput { get; private set; }

    /// <inheritdoc />
    public Task<Result<string, string>> InvokeAsync(string input, CancellationToken ct = default)
    {
        InvokeCount++;
        LastInput = input;
        return this.invokeFunc(input, ct);
    }
}