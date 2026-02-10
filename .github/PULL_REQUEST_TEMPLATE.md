## Summary

<!-- Brief description of what this PR does and why -->

## Changes

<!-- Bullet list of specific changes made -->
-

## Type of Change

<!-- Check the relevant option -->
- [ ] `feat` — New feature or workflow
- [ ] `fix` — Bug fix
- [ ] `refactor` — Code restructuring (no behavior change)
- [ ] `docs` — Documentation only
- [ ] `test` — Test additions or updates
- [ ] `ci` — CI/CD workflow changes
- [ ] `chore` — Maintenance / dependency updates

## Affected Areas

<!-- Which parts of the repo are touched? -->
- [ ] `build/` — Build configuration
- [ ] `.github/workflows/` — Reusable CI workflows
- [ ] `test-infra/` — Shared test infrastructure
- [ ] `agents/` — AI agent definitions
- [ ] `docs/` — Documentation

## Checklist

<!-- Complete before requesting review -->
- [ ] PR title follows Conventional Commits: `<type>(<scope>): <subject>`
- [ ] YAML syntax validated (if workflow changes)
- [ ] Workflows remain generic / reusable across sub-repos
- [ ] Action versions are pinned (no `@main` or `@latest`)
- [ ] No secrets or tokens exposed in logs or code
- [ ] README tables updated for new workflows
- [ ] Inline comments added where logic is non-obvious

## Testing

<!-- How were these changes verified? -->
- [ ] Workflow tested via `workflow_dispatch` manual trigger
- [ ] Dry-run in a sub-repo fork
- [ ] Validated YAML with `actionlint` or equivalent
- [ ] N/A (documentation only)

## Related Issues

<!-- Link related issues: Closes #123, Relates to #456 -->
