// <copyright file="MockSkillRegistry.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Ouroboros.Tests.Mocks;

using System.Collections.Concurrent;
using Ouroboros.Abstractions;
using Ouroboros.Abstractions.Monads;
using Ouroboros.Agent.MetaAI;

/// <summary>
/// Mock implementation of ISkillRegistry for testing purposes.
/// Provides in-memory skill storage with configurable behavior.
/// </summary>
public sealed class MockSkillRegistry : ISkillRegistry
{
    private readonly ConcurrentDictionary<string, AgentSkill> skills = new();
    private readonly Func<string, Dictionary<string, object>?, List<Skill>>? matchingFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockSkillRegistry"/> class.
    /// </summary>
    public MockSkillRegistry()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MockSkillRegistry"/> class with custom matching logic.
    /// </summary>
    /// <param name="matchingFactory">Factory function for finding matching skills.</param>
    public MockSkillRegistry(Func<string, Dictionary<string, object>?, List<Skill>> matchingFactory)
    {
        this.matchingFactory = matchingFactory;
    }

    /// <summary>
    /// Gets the number of registered skills.
    /// </summary>
    public int Count => this.skills.Count;

    /// <summary>
    /// Gets the last skill that was registered.
    /// </summary>
    public AgentSkill? LastRegisteredSkill { get; private set; }

    /// <summary>
    /// Gets the number of times RegisterSkill was called.
    /// </summary>
    public int RegisterCallCount { get; private set; }

    /// <summary>
    /// Registers a new skill asynchronously.
    /// </summary>
    public Task<Result<Unit, string>> RegisterSkillAsync(AgentSkill skill, CancellationToken ct = default)
    {
        var result = RegisterSkill(skill);
        return Task.FromResult(result);
    }

    /// <summary>
    /// Registers a new skill.
    /// </summary>
    public Result<Unit, string> RegisterSkill(AgentSkill skill)
    {
        this.RegisterCallCount++;
        this.LastRegisteredSkill = skill;
        this.skills[skill.Id] = skill;
        return Result<Unit, string>.Success(Unit.Value);
    }

    /// <summary>
    /// Gets a skill by ID asynchronously.
    /// </summary>
    public Task<Result<AgentSkill, string>> GetSkillAsync(string skillId, CancellationToken ct = default)
    {
        if (this.skills.TryGetValue(skillId, out var skill))
        {
            return Task.FromResult(Result<AgentSkill, string>.Success(skill));
        }

        return Task.FromResult(Result<AgentSkill, string>.Failure($"Skill '{skillId}' not found"));
    }

    /// <summary>
    /// Gets a skill by ID.
    /// </summary>
    public AgentSkill? GetSkill(string skillId)
    {
        this.skills.TryGetValue(skillId, out var skill);
        return skill;
    }

    /// <summary>
    /// Finds skills matching the given criteria.
    /// </summary>
    public Task<Result<IReadOnlyList<AgentSkill>, string>> FindSkillsAsync(
        string? category = null,
        IReadOnlyList<string>? tags = null,
        CancellationToken ct = default)
    {
        var filtered = this.skills.Values.AsEnumerable();

        if (category != null)
        {
            filtered = filtered.Where(s => s.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        if (tags != null && tags.Count > 0)
        {
            filtered = filtered.Where(s => tags.Any(t => s.Tags.Contains(t)));
        }

        IReadOnlyList<AgentSkill> result = filtered.ToList();
        return Task.FromResult(Result<IReadOnlyList<AgentSkill>, string>.Success(result));
    }

    /// <summary>
    /// Finds skills matching the given goal and context.
    /// </summary>
    public Task<List<Skill>> FindMatchingSkillsAsync(
        string goal,
        Dictionary<string, object>? context = null,
        CancellationToken ct = default)
    {
        if (this.matchingFactory != null)
        {
            return Task.FromResult(this.matchingFactory(goal, context));
        }

        // Default: return empty list (Skill type is different from AgentSkill)
        return Task.FromResult(new List<Skill>());
    }

    /// <summary>
    /// Updates an existing skill.
    /// </summary>
    public Task<Result<Unit, string>> UpdateSkillAsync(AgentSkill skill, CancellationToken ct = default)
    {
        this.skills[skill.Id] = skill;
        return Task.FromResult(Result<Unit, string>.Success(Unit.Value));
    }

    /// <summary>
    /// Records a skill execution result asynchronously.
    /// </summary>
    public Task<Result<Unit, string>> RecordExecutionAsync(
        string skillId,
        bool success,
        long executionTimeMs,
        CancellationToken ct = default)
    {
        RecordSkillExecution(skillId, success, executionTimeMs);
        return Task.FromResult(Result<Unit, string>.Success(Unit.Value));
    }

    /// <summary>
    /// Records skill execution and updates metrics.
    /// </summary>
    public void RecordSkillExecution(string skillId, bool success, long executionTimeMs)
    {
        if (this.skills.TryGetValue(skillId, out var skill))
        {
            var newUsageCount = skill.UsageCount + 1;
            var newSuccessRate = success
                ? ((skill.SuccessRate * skill.UsageCount) + 1.0) / newUsageCount
                : (skill.SuccessRate * skill.UsageCount) / newUsageCount;
            var newAvgTime = ((skill.AverageExecutionTime * skill.UsageCount) + executionTimeMs) / newUsageCount;

            var updatedSkill = skill with
            {
                SuccessRate = newSuccessRate,
                UsageCount = newUsageCount,
                AverageExecutionTime = newAvgTime,
            };

            this.skills[skillId] = updatedSkill;
        }
    }

    /// <summary>
    /// Removes a skill from the registry.
    /// </summary>
    public Task<Result<Unit, string>> UnregisterSkillAsync(string skillId, CancellationToken ct = default)
    {
        this.skills.TryRemove(skillId, out _);
        return Task.FromResult(Result<Unit, string>.Success(Unit.Value));
    }

    /// <summary>
    /// Gets all registered skills asynchronously.
    /// </summary>
    public Task<Result<IReadOnlyList<AgentSkill>, string>> GetAllSkillsAsync(CancellationToken ct = default)
    {
        IReadOnlyList<AgentSkill> result = this.skills.Values.ToList();
        return Task.FromResult(Result<IReadOnlyList<AgentSkill>, string>.Success(result));
    }

    /// <summary>
    /// Gets all registered skills.
    /// </summary>
    public IReadOnlyList<AgentSkill> GetAllSkills()
    {
        return this.skills.Values.ToList();
    }

    /// <summary>
    /// Extracts and registers a skill from an execution result.
    /// </summary>
    public Task<Result<Skill, string>> ExtractSkillAsync(
        PlanExecutionResult execution,
        string skillName,
        string description,
        CancellationToken ct = default)
    {
        if (!execution.Success)
        {
            return Task.FromResult(Result<Skill, string>.Failure("Cannot extract skill from failed execution"));
        }

        var skill = new Skill(
            Name: skillName,
            Description: description,
            Prerequisites: new List<string>(),
            Steps: execution.Plan.Steps,
            SuccessRate: 1.0,
            UsageCount: 0,
            CreatedAt: DateTime.UtcNow,
            LastUsed: DateTime.UtcNow);

        return Task.FromResult(Result<Skill, string>.Success(skill));
    }

    /// <summary>
    /// Adds a pre-configured skill for testing.
    /// </summary>
    /// <param name="skill">Skill to add.</param>
    public void AddSkill(AgentSkill skill)
    {
        this.skills[skill.Id] = skill;
    }
}
