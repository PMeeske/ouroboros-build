// <copyright file="MockMemoryStore.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Ouroboros.Tests.Mocks;

using Ouroboros.Abstractions;
using Ouroboros.Abstractions.Monads;
using Ouroboros.Agent.MetaAI;

/// <summary>
/// Mock implementation of IMemoryStore for testing purposes.
/// Provides in-memory storage with configurable behavior.
/// </summary>
public sealed class MockMemoryStore : IMemoryStore
{
    private readonly Dictionary<string, Experience> experiences = new();
    private readonly Func<MemoryQuery, List<Experience>>? retrievalFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockMemoryStore"/> class.
    /// </summary>
    public MockMemoryStore()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MockMemoryStore"/> class with custom retrieval logic.
    /// </summary>
    /// <param name="retrievalFactory">Factory function for retrieving experiences.</param>
    public MockMemoryStore(Func<MemoryQuery, List<Experience>> retrievalFactory)
    {
        this.retrievalFactory = retrievalFactory;
    }

    /// <summary>
    /// Gets the number of stored experiences.
    /// </summary>
    public int Count => this.experiences.Count;

    /// <summary>
    /// Gets the last experience that was stored.
    /// </summary>
    public Experience? LastStoredExperience { get; private set; }

    /// <summary>
    /// Gets the number of times StoreExperienceAsync was called.
    /// </summary>
    public int StoreCallCount { get; private set; }

    /// <summary>
    /// Gets the number of times RetrieveRelevantExperiencesAsync was called.
    /// </summary>
    public int RetrieveCallCount { get; private set; }

    /// <summary>
    /// Stores an experience in memory.
    /// </summary>
    public Task<Result<Unit, string>> StoreExperienceAsync(Experience experience, CancellationToken ct = default)
    {
        this.StoreCallCount++;
        this.LastStoredExperience = experience;
        this.experiences[experience.Id] = experience;
        return Task.FromResult(Result<Unit, string>.Success(Unit.Value));
    }

    /// <summary>
    /// Queries experiences based on query criteria.
    /// </summary>
    public Task<Result<IReadOnlyList<Experience>, string>> QueryExperiencesAsync(
        MemoryQuery query,
        CancellationToken ct = default)
    {
        var results = FilterExperiences(query);
        return Task.FromResult(Result<IReadOnlyList<Experience>, string>.Success(results));
    }

    /// <summary>
    /// Retrieves relevant experiences based on query.
    /// </summary>
    public Task<Result<IReadOnlyList<Experience>, string>> RetrieveRelevantExperiencesAsync(
        MemoryQuery query,
        CancellationToken ct = default)
    {
        this.RetrieveCallCount++;

        if (this.retrievalFactory != null)
        {
            IReadOnlyList<Experience> factoryResult = this.retrievalFactory(query);
            return Task.FromResult(Result<IReadOnlyList<Experience>, string>.Success(factoryResult));
        }

        var results = FilterExperiences(query);
        return Task.FromResult(Result<IReadOnlyList<Experience>, string>.Success(results));
    }

    /// <summary>
    /// Gets an experience by ID.
    /// </summary>
    public Task<Result<Experience, string>> GetExperienceAsync(string id, CancellationToken ct = default)
    {
        if (this.experiences.TryGetValue(id, out var experience))
        {
            return Task.FromResult(Result<Experience, string>.Success(experience));
        }

        return Task.FromResult(Result<Experience, string>.Failure($"Experience '{id}' not found"));
    }

    /// <summary>
    /// Deletes an experience from memory.
    /// </summary>
    public Task<Result<Unit, string>> DeleteExperienceAsync(string id, CancellationToken ct = default)
    {
        this.experiences.Remove(id);
        return Task.FromResult(Result<Unit, string>.Success(Unit.Value));
    }

    /// <summary>
    /// Gets statistics about stored experiences (Result wrapper).
    /// </summary>
    public Task<Result<MemoryStatistics, string>> GetStatisticsAsync(CancellationToken ct = default)
    {
        var stats = BuildStatistics();
        return Task.FromResult(Result<MemoryStatistics, string>.Success(stats));
    }

    /// <summary>
    /// Gets statistics about stored experiences.
    /// </summary>
    public Task<MemoryStatistics> GetStatsAsync(CancellationToken ct = default)
    {
        return Task.FromResult(BuildStatistics());
    }

    /// <summary>
    /// Clears all stored experiences.
    /// </summary>
    public Task<Result<Unit, string>> ClearAsync(CancellationToken ct = default)
    {
        this.experiences.Clear();
        this.LastStoredExperience = null;
        return Task.FromResult(Result<Unit, string>.Success(Unit.Value));
    }

    /// <summary>
    /// Adds a pre-configured experience for testing.
    /// </summary>
    /// <param name="experience">Experience to add.</param>
    public void AddExperience(Experience experience)
    {
        this.experiences[experience.Id] = experience;
    }

    private IReadOnlyList<Experience> FilterExperiences(MemoryQuery query)
    {
        var filtered = this.experiences.Values.AsEnumerable();

        if (query.Goal != null)
        {
            filtered = filtered.Where(e => e.Goal.Contains(query.Goal, StringComparison.OrdinalIgnoreCase));
        }

        if (query.SuccessOnly == true)
        {
            filtered = filtered.Where(e => e.Success);
        }

        if (query.FromDate.HasValue)
        {
            filtered = filtered.Where(e => e.Timestamp >= query.FromDate.Value);
        }

        if (query.ToDate.HasValue)
        {
            filtered = filtered.Where(e => e.Timestamp <= query.ToDate.Value);
        }

        return filtered
            .OrderByDescending(e => e.Timestamp)
            .Take(query.MaxResults)
            .ToList();
    }

    private MemoryStatistics BuildStatistics()
    {
        var successful = this.experiences.Values.Count(e => e.Success);
        var failed = this.experiences.Values.Count(e => !e.Success);
        var uniqueContexts = this.experiences.Values.Select(e => e.Context).Distinct().Count();
        var uniqueTags = this.experiences.Values.SelectMany(e => e.Tags).Distinct().Count();
        var oldest = this.experiences.Values.Any()
            ? this.experiences.Values.Min(e => e.Timestamp)
            : (DateTime?)null;
        var newest = this.experiences.Values.Any()
            ? this.experiences.Values.Max(e => e.Timestamp)
            : (DateTime?)null;
        var avgQuality = this.experiences.Values.Any()
            ? this.experiences.Values.Average(e => e.Verification.QualityScore)
            : 0.0;

        return new MemoryStatistics(
            TotalExperiences: this.experiences.Count,
            SuccessfulExperiences: successful,
            FailedExperiences: failed,
            UniqueContexts: uniqueContexts,
            UniqueTags: uniqueTags,
            OldestExperience: oldest,
            NewestExperience: newest,
            AverageQualityScore: avgQuality);
    }
}
