# Upstream Merge Checklist

Quick reference checklist for merging upstream changes.

## Pre-Merge

- [ ] `git checkout main && git pull origin main`
- [ ] `git fetch upstream`
- [ ] `git checkout -b merge-upstream-YYYYMMDD`
- [ ] Review upstream commits: `git log HEAD..upstream/main`
- [ ] Create new entry in `UPSTREAM_MERGE_LOG.md` with date and commit

## Merge & Analyze

- [ ] `git merge upstream/main`
- [ ] List conflicting files: `git status`
- [ ] For each component group, analyze significant changes
- [ ] Document overview in `UPSTREAM_MERGE_LOG.md`

## For Each Conflicting Component

### Analysis
- [ ] Compare current vs incoming changes
- [ ] Identify: New features, behavior changes, styling differences
- [ ] Note breaking changes
- [ ] Determine merge strategy (Keep/Take/Combine)

### Documentation (in UPSTREAM_MERGE_LOG.md)
- [ ] Component name and merge strategy
- [ ] Key decisions with rationale
- [ ] List of changed files
- [ ] Breaking changes (if any)
- [ ] Migration notes (if needed)

### Resolution
- [ ] Resolve conflicts according to strategy
- [ ] Remove all merge conflict markers
- [ ] Verify code compiles
- [ ] `git add` resolved files

### AI Assistant Prompts

**Initial Analysis:**
```
Analyze merge conflicts in [ComponentName]. What are the significant changes?
Focus on: new features, behavior changes, styling, breaking changes.
```

**Resolution:**
```
For [ComponentName], apply this strategy:
1. [Specific instruction]
2. [Specific instruction]
Ensure: no conflict markers, compiles, backward compatible
```

## Validation

- [ ] Check no conflicts remain: `git status`
- [ ] No conflict markers: `git diff --check`
- [ ] Build solution: `dotnet build`
- [ ] No compilation errors
- [ ] Update merge log with build status

## Finalization

### Documentation
- [ ] Complete all sections in `UPSTREAM_MERGE_LOG.md`
- [ ] Add file count statistics
- [ ] Add testing checklist
- [ ] Review all documented decisions

### Commit
- [ ] `git add UPSTREAM_MERGE_LOG.md`
- [ ] `git add .github/UPSTREAM_MERGE_WORKFLOW.md` (if updated)
- [ ] Commit with descriptive message:
  ```
  Merge upstream/main: [Brief description]
  
  - Component 1: [change]
  - Component 2: [change]
  
  See UPSTREAM_MERGE_LOG.md for details.
  ```

## Testing

Create checklist in `UPSTREAM_MERGE_LOG.md`:

```markdown
### Manual Testing Required
- [ ] Component 1 renders correctly
- [ ] Component 2 new feature works
- [ ] No console errors
- [ ] Accessibility testing
- [ ] Responsive design
```

- [ ] Execute all manual tests
- [ ] Update merge log with test results

## Pull Request

- [ ] `git push origin merge-upstream-YYYYMMDD`
- [ ] Create PR using template (see `.github/UPSTREAM_MERGE_WORKFLOW.md`)
- [ ] Link to `UPSTREAM_MERGE_LOG.md` in PR description
- [ ] Request review
- [ ] Address review feedback
- [ ] Merge when approved

## Post-Merge

- [ ] Update `CHANGELOG.md` if needed
- [ ] Update component documentation if needed
- [ ] Close any related issues
- [ ] Share summary with team

---

## Quick Commands

```bash
# Fetch and merge
git fetch upstream && git merge upstream/main

# View conflicts
git status | grep "both modified"

# Check for conflict markers
git diff --check

# See what changed in a file
git diff HEAD..upstream/main -- path/to/file

# Abort merge if needed
git merge --abort

# Continue after resolving
git commit -m "Merge upstream/main: [description]"
```

## Template Snippets

### Component Section (for UPSTREAM_MERGE_LOG.md)

```markdown
### [ComponentName]

**Merge Strategy:** [KEEP_CURRENT / TAKE_INCOMING / COMBINE]

**Key Decisions:**
- **[Aspect]**: [Decision] → Rationale: [Why]

**Changed Files:**
- `path/to/file.razor`

**Breaking Changes:** ❌ None / ⚠️ [Description]

**Migration Notes:** [If applicable]
```

### Testing Section

```markdown
### Manual Testing Required
- [ ] [Component]: [Feature/behavior to test]
- [ ] [Component]: [Another test case]
- [ ] No console errors
- [ ] Accessibility with screen reader
- [ ] Responsive: mobile/tablet/desktop
```

---

**See Also:**
- `UPSTREAM_MERGE_LOG.md` - Historical merge record
- `.github/UPSTREAM_MERGE_WORKFLOW.md` - Detailed workflow guide
