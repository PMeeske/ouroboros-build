# ouroboros-build

Shared build infrastructure for the Ouroboros project family. This repo is the "parent class" of all Ouroboros sub-repos — it contains zero application code.

## What's Here

| Directory | Purpose |
|-----------|---------|
| `build/` | Shared `Directory.Build.props`, `.editorconfig`, `coverlet.runsettings`, and base Stryker config |
| `.github/workflows/` | Reusable CI workflow templates called by all sub-repos |
| `test-infra/Ouroboros.TestKit/` | Shared mock implementations and test builders |
| `test-infra/Ouroboros.TestKit.BDD/` | Shared BDD (Reqnroll) step definitions |
| `agents/` | Copilot/AI agent definitions |

## Reusable Workflows

All reusable workflows support both `workflow_call` (called from other workflows) and `workflow_dispatch` (manual trigger via the GitHub Actions UI).

| Workflow | Description |
|----------|-------------|
| `_reusable-dotnet-setup.yml` | .NET SDK installation, NuGet caching, optional MAUI workloads |
| `_reusable-dotnet-build.yml` | Build .NET projects with retry logic |
| `_reusable-dotnet-test.yml` | Run tests with coverage collection and reporting |
| `_reusable-mutation-testing.yml` | Stryker.NET mutation testing with configurable thresholds |
| `_reusable-docker-publish.yml` | Docker build and push to any container registry |
| `_reusable-update-submodule.yml` | Automatically update git submodule pointers to latest upstream commits |
| `_reusable-claude-review.yml` | Claude-powered PR code review with configurable depth and custom instructions |

### Claude AI Workflows

These workflows integrate Claude into the PR and issue lifecycle. They require an `ANTHROPIC_API_KEY` repository secret.

| Workflow | Trigger | Description |
|----------|---------|-------------|
| `claude-code-review.yml` | PR opened / updated | Automated code review on every pull request |
| `claude-pr-assistant.yml` | `@claude` mention in comments | Interactive assistant for PR and issue discussions |
| `claude-issue-triage.yml` | Issue opened | Automatic issue categorization and triage |

#### Setup

1. Add your Anthropic API key as a repository secret named `ANTHROPIC_API_KEY`
2. The workflows activate automatically — no additional configuration needed

#### Using the PR Assistant

Mention `@claude` in any PR or issue comment to interact:

```
@claude explain the retry logic in this workflow
@claude suggest tests for this change
@claude is this change safe to deploy?
```

#### Calling the Reusable Review from Sub-Repos

```yaml
jobs:
  claude-review:
    uses: PMeeske/ouroboros-build/.github/workflows/_reusable-claude-review.yml@main
    with:
      review-level: 'standard'
    secrets:
      anthropic-api-key: ${{ secrets.ANTHROPIC_API_KEY }}
```

### Manual Trigger

Each workflow can be triggered manually from the **Actions** tab in GitHub:

1. Navigate to the **Actions** tab of this repository
2. Select the desired workflow from the left sidebar
3. Click **Run workflow**
4. Fill in the required inputs and click **Run workflow**

> **Note:** For `_reusable-docker-publish.yml`, the registry credentials (`registry-username` and `registry-password`) must be configured as repository secrets.

## Usage

Sub-repos include this as a git submodule at `.build/`:

```bash
git submodule add git@github.com:PMeeske/ouroboros-build.git .build
```

### Inherit Build Config

In your sub-repo's `Directory.Build.props`:

```xml
<Project>
  <Import Project="$(MSBuildThisFileDirectory)..\.build\build\Directory.Build.props"
          Condition="Exists('$(MSBuildThisFileDirectory)..\.build\build\Directory.Build.props')" />

  <PropertyGroup>
    <OuroborosLayer>Foundation</OuroborosLayer>
  </PropertyGroup>
</Project>
```

### Call Reusable Workflows

In your sub-repo's `.github/workflows/ci.yml`:

```yaml
jobs:
  build:
    uses: PMeeske/ouroboros-build/.github/workflows/_reusable-dotnet-build.yml@main
    with:
      projects: 'src/MyProject/MyProject.csproj'

  test:
    needs: build
    uses: PMeeske/ouroboros-build/.github/workflows/_reusable-dotnet-test.yml@main
    with:
      test-projects: 'tests/**/*.csproj'
      coverage-threshold: 60
```

### Auto-Update Submodules

To automatically update submodule pointers when upstream dependencies change, downstream repos can call the workflow from a `repository_dispatch` event or `workflow_dispatch`:

```yaml
name: Update Submodule

on:
  repository_dispatch:
    types: [upstream-updated]
  workflow_dispatch:

jobs:
  update-build-submodule:
    uses: PMeeske/ouroboros-build/.github/workflows/_reusable-update-submodule.yml@main
    with:
      submodule-path: '.build'
      submodule-branch: 'main'
      create-pr: true
    secrets:
      token: ${{ secrets.SUBMODULE_UPDATE_TOKEN }}
```

This creates a PR when the submodule has new commits available.

## Dependency Hierarchy

```
ouroboros-build  (this repo)
    |
    +-- ouroboros-foundation  (leaf types, no upstream deps)
    |
    +-- ouroboros-engine  (AI runtime, depends on foundation)
    |
    +-- ouroboros-app  (hosts + integration, depends on engine + foundation)
```
