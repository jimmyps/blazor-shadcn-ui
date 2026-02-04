# Upstream Merge Workflow Guide

This document outlines the standard process for merging changes from the upstream BlazorUI repository into our fork.

## Prerequisites

- Git repository with `upstream` remote configured
- Access to AI Assistant for merge conflict resolution
- Familiarity with component architecture

## Workflow Steps

### 1. Preparation Phase

```bash
# Ensure you're on your main branch and it's up to date
git checkout main
git pull origin main

# Fetch latest upstream changes
git fetch upstream

# Create a new merge branch
git checkout -b merge-upstream-$(date +%Y%m%d)
```

### 2. Analysis Phase

Before merging, analyze what's coming:

```bash
# View upstream commits
git log HEAD..upstream/main --oneline

# Identify conflicting files
git merge --no-commit --no-ff upstream/main
git status

# If conflicts exist, abort to prepare
git merge --abort
```

**Document in `UPSTREAM_MERGE_LOG.md`:**
- [ ] Merge date and commit hash
- [ ] List of affected components
- [ ] High-level overview of changes

### 3. Merge Initiation

```bash
# Perform the merge
git merge upstream/main
```

### 4. Conflict Resolution (Component by Component)

For each conflicting component:

#### A. Analyze Differences

**Use AI Assistant prompt:**
```
Analyze the merge conflicts in [ComponentName]. What are the significant changes between:
1. Current (HEAD) - our version
2. Incoming (upstream) - their version

Focus on:
- New features/parameters
- Behavior changes
- Styling differences
- Breaking changes
```

#### B. Document Decision

**In `UPSTREAM_MERGE_LOG.md`, add:**

```markdown
### [ComponentName]

**Merge Strategy:** [KEEP_CURRENT / TAKE_INCOMING / COMBINE]

**Key Decisions:**
- Decision 1: [What] → [Why]
- Decision 2: [What] → [Why]

**Changed Files:**
- path/to/file1.razor
- path/to/file2.cs

**Breaking Changes:** [Yes/No - describe if yes]

**Migration Notes:** [If applicable]
```

#### C. Resolve Conflicts

**Use AI Assistant prompt:**
```
For [ComponentName], apply the following merge strategy:
1. [Specific instruction for file 1]
2. [Specific instruction for file 2]
...

Ensure:
- No merge conflict markers remain
- Code compiles
- Backward compatibility maintained (or document breaking changes)
```

#### D. Stage Resolved Files

```bash
git add src/path/to/resolved/files
```

### 5. Validation Phase

After all conflicts resolved:

```bash
# Check for remaining conflicts
git status

# Verify no conflict markers remain
git diff --check

# Build the solution
dotnet build

# Check for compilation errors
dotnet build --no-incremental
```

**Update `UPSTREAM_MERGE_LOG.md`:**
- [ ] Mark build status
- [ ] Add manual testing checklist
- [ ] Note any pending issues

### 6. Finalization

```bash
# Update the merge log with final statistics
# Review all decisions documented

# Stage the merge log
git add UPSTREAM_MERGE_LOG.md

# Commit the merge
git commit -m "Merge upstream/main: [Brief description of main changes]

- [Component 1]: [Brief change description]
- [Component 2]: [Brief change description]
...

See UPSTREAM_MERGE_LOG.md for detailed decisions and rationale.
"
```

### 7. Testing Phase

**Manual Testing Checklist:**

Create a checklist in `UPSTREAM_MERGE_LOG.md`:

```markdown
### Manual Testing Required
- [ ] Component 1: Feature A works
- [ ] Component 1: Feature B works
- [ ] Component 2: Behavior X correct
- [ ] Component 2: Style Y renders properly
- [ ] No console errors
- [ ] Accessibility: Screen reader testing
- [ ] Responsive: Mobile/tablet/desktop
```

### 8. Pull Request

```bash
# Push merge branch
git push origin merge-upstream-$(date +%Y%m%d)
```

**PR Template:**

```markdown
## Upstream Merge: [Date]

Merges changes from `upstream/main` commit [`hash`](link)

### Components Affected
- Component 1
- Component 2
...

### Key Changes
- **Component 1**: [Brief description]
- **Component 2**: [Brief description]

### Breaking Changes
[None / List them]

### Migration Guide
[If applicable, describe migration steps]

### Testing
- [x] All conflicts resolved
- [x] Solution builds successfully
- [ ] Manual testing completed (checklist in UPSTREAM_MERGE_LOG.md)

### Documentation
- [x] Merge decisions documented in UPSTREAM_MERGE_LOG.md
- [ ] Component documentation updated (if needed)
- [ ] CHANGELOG.md updated

---

See [UPSTREAM_MERGE_LOG.md](../UPSTREAM_MERGE_LOG.md) for detailed merge decisions and rationale.
```

## AI Assistant Prompts Library

### Initial Analysis

```
Scan the merge conflicts in [files/directory]. Identify:
1. What are the significant changes?
2. Are there new features or parameters?
3. Are there behavior changes?
4. Are there styling/CSS differences?
5. Any breaking changes?
```

### Component-Specific Resolution

```
For [ComponentName], I need to:
1. [Keep/Take/Combine] [specific aspect]
2. [Keep/Take/Combine] [specific aspect]
...

Reason: [explain the strategy]

Please resolve the conflicts according to this strategy.
```

### Validation

```
Check all [ComponentName] files for:
1. Remaining merge conflict markers
2. Compilation errors
3. Consistency in style/patterns
```

## Best Practices

### Decision Making

1. **Default to Upstream**: If no strong reason to keep our version, take upstream
2. **Preserve Customizations**: Document why we deviate from upstream
3. **Backward Compatibility**: Prioritize non-breaking changes
4. **Accessibility First**: Always favor accessibility improvements
5. **Consistency**: Match shadcn/ui patterns when applicable

### Documentation

1. **Be Specific**: Document exact CSS classes, parameter names, values
2. **Explain Rationale**: Every decision needs a "why"
3. **Include Examples**: Show before/after for complex changes
4. **Link Resources**: Reference upstream commits, shadcn docs, etc.

### Testing

1. **Build First**: Always verify compilation before manual testing
2. **Test Visually**: Check demos for each affected component
3. **Test Accessibility**: Use screen readers, keyboard navigation
4. **Test Responsive**: Verify mobile/tablet/desktop layouts
5. **Test Integration**: Ensure components work together

## Troubleshooting

### Common Issues

**Issue: Too many conflicts**
```bash
# Solution: Merge incrementally
git merge upstream/main~10  # Merge up to 10 commits ago
# Resolve, commit
git merge upstream/main~5   # Merge next batch
# Continue until caught up
```

**Issue: Lost track of decisions**
```bash
# Solution: Review merge log structure
grep "### " UPSTREAM_MERGE_LOG.md  # See all component sections
```

**Issue: Unclear what changed**
```bash
# Solution: Compare with upstream directly
git diff HEAD..upstream/main -- path/to/component/
```

## Automation Opportunities

### Future Enhancements

1. **Conflict Categorization Script**: Auto-detect type of conflict (style/behavior/feature)
2. **Merge Log Template Generator**: Create component sections automatically
3. **Testing Checklist Generator**: Auto-generate test cases from changed files
4. **Change Summary Script**: Parse commits and generate initial log entry

### Template Scripts

**Generate Component Section Template:**

```bash
#!/bin/bash
# generate-merge-section.sh
component_name=$1
echo "### ${component_name}"
echo ""
echo "**Merge Strategy:** [KEEP_CURRENT / TAKE_INCOMING / COMBINE]"
echo ""
echo "**Key Decisions:**"
echo "- Decision 1: [What] → [Why]"
echo ""
echo "**Changed Files:**"
git diff --name-only --diff-filter=U | grep -i "$component_name"
echo ""
echo "**Breaking Changes:** [Yes/No]"
echo ""
```

## Maintenance

### Regular Review

- **After Each Merge**: Review merge log for completeness
- **Monthly**: Review decision patterns, update best practices
- **Quarterly**: Evaluate if our customizations still make sense
- **Yearly**: Consider upstreaming our improvements

### Documentation Updates

Keep these documents in sync:
- `UPSTREAM_MERGE_LOG.md` - Historical record
- `.github/UPSTREAM_MERGE_WORKFLOW.md` - This guide
- `CONTRIBUTING.md` - Include link to this workflow
- `README.md` - Note fork maintenance strategy

---

**Workflow Version:** 1.0  
**Last Updated:** 2025-01-XX  
**Maintained By:** Development Team
