# Ouroboros Build — Claude Project Instructions

This is the **ouroboros-build** meta-repository: shared build infrastructure for the Ouroboros project family. It contains zero application code.

## Repository Structure

| Directory | Purpose |
|-----------|---------|
| `build/` | Shared `Directory.Build.props`, `.editorconfig`, `coverlet.runsettings`, base Stryker config |
| `.github/workflows/` | Reusable CI workflow templates called by all sub-repos |
| `test-infra/Ouroboros.TestKit/` | Shared mock implementations and test builders |
| `test-infra/Ouroboros.TestKit.BDD/` | Shared BDD (Reqnroll) step definitions |
| `agents/` | AI agent definitions for code assistance |
| `docs/` | Contributing guides, test coverage documentation |

## Technology Stack

- **Language:** C# on .NET 10.0 (C# 14)
- **Build:** MSBuild via `dotnet` CLI
- **Test framework:** xUnit with NSubstitute for mocking
- **Code coverage:** Coverlet (Cobertura/LCOV) with ReportGenerator
- **Mutation testing:** Stryker.NET (thresholds: high=80%, low=60%, break=50%)
- **Package manager:** NuGet
- **CI/CD:** GitHub Actions with reusable workflow templates

## Dependency Hierarchy

```
ouroboros-build  (this repo — infrastructure only)
    ├── ouroboros-foundation  (leaf types, no upstream deps)
    ├── ouroboros-engine      (AI runtime, depends on foundation)
    └── ouroboros-app         (hosts + integration, depends on engine + foundation)
```

Sub-repos include this as a git submodule at `.build/`.

## Coding Standards

- Follow `.editorconfig` rules in `build/.editorconfig`
- Nullable reference types enabled and enforced
- No `var` for built-in types
- Expression-bodied members for properties/indexers/accessors only
- Modifier ordering: `public,private,protected,internal,static,extern,new,virtual,abstract,sealed,override,readonly,unsafe,volatile,async`
- Use monadic composition (`Result<T>`, `Option<T>`) for error handling
- Prefer immutability and pure functions
- Use Kleisli arrows (`Step<TInput, TOutput>`) for composable operations

## Commit Convention

Follow [Conventional Commits](https://www.conventionalcommits.org/):
```
<type>(<scope>): <subject>
```
Types: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`, `ci`

## Testing Policy

- All functional changes require tests
- Target 85%+ unit test coverage (90%+ for core logic)
- Integration tests required for cross-boundary interactions
- Test naming: `Should_<Expected>_When_<Condition>`
- Use shared mocks from `test-infra/Ouroboros.TestKit/`

## PR Review Focus Areas

When reviewing pull requests in this repo, prioritize:
1. **Workflow correctness** — YAML syntax, input/output contracts, job dependencies
2. **Reusability** — workflows must remain generic across sub-repos
3. **Security** — no secrets in logs, proper permissions, pinned action versions
4. **Idempotency** — workflows should be safe to re-run
5. **Documentation** — README tables and inline comments updated for new workflows
