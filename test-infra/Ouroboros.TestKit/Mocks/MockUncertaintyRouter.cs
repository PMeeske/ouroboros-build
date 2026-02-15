// <copyright file="MockUncertaintyRouter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Ouroboros.Tests.Mocks;

using Ouroboros.Agent.MetaAI;

/// <summary>
/// Mock implementation of IUncertaintyRouter for testing purposes.
/// Provides configurable routing behavior with confidence scores.
/// </summary>
public sealed class MockUncertaintyRouter : IUncertaintyRouter
{
    private readonly Func<string, string, double, RoutingDecision>? routingFactory;
    private readonly double minimumConfidence;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockUncertaintyRouter"/> class.
    /// </summary>
    /// <param name="minimumConfidence">Minimum confidence threshold (default: 0.7).</param>
    public MockUncertaintyRouter(double minimumConfidence = 0.7)
    {
        this.minimumConfidence = minimumConfidence;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MockUncertaintyRouter"/> class with custom routing logic.
    /// </summary>
    /// <param name="routingFactory">Factory function for routing decisions.</param>
    /// <param name="minimumConfidence">Minimum confidence threshold.</param>
    public MockUncertaintyRouter(
        Func<string, string, double, RoutingDecision> routingFactory,
        double minimumConfidence = 0.7)
    {
        this.routingFactory = routingFactory;
        this.minimumConfidence = minimumConfidence;
    }

    /// <summary>
    /// Gets the minimum confidence threshold.
    /// </summary>
    public double MinimumConfidenceThreshold => this.minimumConfidence;

    /// <summary>
    /// Gets the number of times RouteDecisionAsync was called.
    /// </summary>
    public int RouteCallCount { get; private set; }

    /// <summary>
    /// Gets the last routing decision made.
    /// </summary>
    public RoutingDecision? LastDecision { get; private set; }

    /// <summary>
    /// Routes a decision based on uncertainty level.
    /// </summary>
    public Task<RoutingDecision> RouteDecisionAsync(
        string context,
        string proposedAction,
        double confidenceLevel,
        CancellationToken ct = default)
    {
        this.RouteCallCount++;

        RoutingDecision decision;
        if (this.routingFactory != null)
        {
            decision = this.routingFactory(context, proposedAction, confidenceLevel);
        }
        else
        {
            var shouldProceed = confidenceLevel >= this.minimumConfidence;
            var strategy = shouldProceed ? FallbackStrategy.Retry : FallbackStrategy.RequestClarification;

            decision = new RoutingDecision(
                ShouldProceed: shouldProceed,
                ConfidenceLevel: confidenceLevel,
                RecommendedStrategy: strategy,
                Reason: shouldProceed ? "Confidence is sufficient" : "Confidence is below threshold",
                RequiresHumanOversight: confidenceLevel < 0.3,
                AlternativeActions: Array.Empty<string>());
        }

        this.LastDecision = decision;
        return Task.FromResult(decision);
    }

    /// <summary>
    /// Determines if human oversight is required.
    /// </summary>
    public Task<bool> RequiresHumanOversightAsync(
        string context,
        double riskLevel,
        double confidenceLevel,
        CancellationToken ct = default)
    {
        // Require oversight if risk is high or confidence is very low
        var required = riskLevel > 0.7 || confidenceLevel < 0.3;
        return Task.FromResult(required);
    }

    /// <summary>
    /// Gets the fallback strategy for a given confidence level.
    /// </summary>
    public Task<FallbackStrategy> GetFallbackStrategyAsync(
        double confidenceLevel,
        int attemptCount,
        CancellationToken ct = default)
    {
        FallbackStrategy strategy;

        if (attemptCount >= 3)
        {
            strategy = FallbackStrategy.EscalateToHuman;
        }
        else if (confidenceLevel >= this.minimumConfidence)
        {
            strategy = FallbackStrategy.Retry;
        }
        else if (confidenceLevel >= 0.5)
        {
            strategy = FallbackStrategy.UseConservativeApproach;
        }
        else if (confidenceLevel >= 0.3)
        {
            strategy = FallbackStrategy.Defer;
        }
        else
        {
            strategy = FallbackStrategy.RequestClarification;
        }

        return Task.FromResult(strategy);
    }
}
