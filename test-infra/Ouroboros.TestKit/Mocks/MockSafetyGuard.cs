// <copyright file="MockSafetyGuard.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Ouroboros.Tests.Mocks;

using Ouroboros.Agent.MetaAI;

/// <summary>
/// Mock implementation of ISafetyGuard for testing purposes.
/// Provides configurable safety checking behavior.
/// </summary>
public sealed class MockSafetyGuard : ISafetyGuard
{
    private readonly Dictionary<string, Permission> permissions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MockSafetyGuard"/> class.
    /// By default, all operations are considered safe.
    /// </summary>
    public MockSafetyGuard()
    {
    }

    /// <summary>
    /// Gets the number of times CheckSafety was called.
    /// </summary>
    public int CheckCallCount { get; private set; }

    /// <summary>
    /// Gets the last operation that was checked.
    /// </summary>
    public string? LastCheckedOperation { get; private set; }

    /// <summary>
    /// Checks if an action is safe to execute (async with parameters and context).
    /// </summary>
    public Task<SafetyCheckResult> CheckActionSafetyAsync(
        string actionName,
        IReadOnlyDictionary<string, object> parameters,
        object? context = null,
        CancellationToken ct = default)
    {
        this.CheckCallCount++;
        this.LastCheckedOperation = actionName;
        return Task.FromResult(SafetyCheckResult.Allowed($"Mock: action '{actionName}' is allowed"));
    }

    /// <summary>
    /// Checks safety of an action (async with permission level).
    /// </summary>
    public Task<SafetyCheckResult> CheckSafetyAsync(
        string action,
        PermissionLevel permissionLevel,
        CancellationToken ct = default)
    {
        this.CheckCallCount++;
        this.LastCheckedOperation = action;
        return Task.FromResult(SafetyCheckResult.Allowed($"Mock: action '{action}' is allowed"));
    }

    /// <summary>
    /// Checks if an operation is safe to execute (sync).
    /// </summary>
    public SafetyCheckResult CheckSafety(
        string action,
        Dictionary<string, object> parameters,
        PermissionLevel permissionLevel)
    {
        this.CheckCallCount++;
        this.LastCheckedOperation = action;
        return SafetyCheckResult.Allowed($"Mock: action '{action}' is allowed");
    }

    /// <summary>
    /// Sandboxes a plan step for safe execution (async).
    /// </summary>
    public Task<SandboxResult> SandboxStepAsync(PlanStep step, CancellationToken ct = default)
    {
        return Task.FromResult(new SandboxResult(
            Success: true,
            SandboxedStep: step,
            Restrictions: Array.Empty<string>(),
            Error: null));
    }

    /// <summary>
    /// Sandboxes a plan step for safe execution.
    /// </summary>
    public PlanStep SandboxStep(PlanStep step)
    {
        // Default: return step unchanged
        return step;
    }

    /// <summary>
    /// Checks if an agent has the required permissions.
    /// </summary>
    public Task<bool> CheckPermissionsAsync(
        string agentId,
        IReadOnlyList<Permission> permissions,
        CancellationToken ct = default)
    {
        // Default: all permissions are granted
        return Task.FromResult(true);
    }

    /// <summary>
    /// Assesses the risk level of an action.
    /// </summary>
    public Task<double> AssessRiskAsync(
        string actionName,
        IReadOnlyDictionary<string, object> parameters,
        CancellationToken ct = default)
    {
        // Default: low risk
        return Task.FromResult(0.1);
    }

    /// <summary>
    /// Registers a permission policy.
    /// </summary>
    public void RegisterPermission(Permission permission)
    {
        this.permissions[permission.Resource] = permission;
    }

    /// <summary>
    /// Gets all registered permissions.
    /// </summary>
    public IReadOnlyList<Permission> GetPermissions()
    {
        return this.permissions.Values.ToList();
    }
}
