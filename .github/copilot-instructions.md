# Copilot Instructions

## Project Guidelines
- FilterBuilder validation feature plan (to be implemented later):

1. `Models/FilterFieldDefinition.cs` — add `Func<object?, string?>? Validate` property
2. `FilterField.razor` — add `Validate` parameter, pass through to `FilterFieldDefinition`
3. `FilterValue.razor.cs` — add `_isInvalid` / `_isSecondaryInvalid` fields; run validate-before-notify in property setters; clear invalid state when operator requires no value (`IsValueNotNeeded()`); re-run on `OnParametersSet`
4. `FilterValue.razor` — bind `AriaInvalid="@(_isInvalid ? true : null)"` on the `<Input>` (and secondary input for Between); no new CSS needed — `Input` already has `aria-[invalid=true]:border-destructive`
5. `FilterChip.razor` — optional: add `OnValidationChanged EventCallback<bool>` output from `FilterValue`; `FilterChip` applies `ring-1 ring-destructive` to chip wrapper when invalid
6. `AllFieldTypesExample.razor` — add `Validate` on Email FilterField using a simple `IsValidEmail` helper (Regex or MailAddress try-parse)
7. `AllFieldTypesExample.cs` — update code snippet constant to match

Design decisions:
- No EditContext/DataAnnotations coupling — FilterBuilder-specific only
- No validation for Select/MultiSelect/Date/Boolean — already type-safe via pickers
- Validation applies primarily to free-text (Input/Masked) editor types
- Invalid state suppresses `NotifyConditionChanged()` so the FilterGroup is never updated with a bad value
- Error message string is captured in the delegate return but no inline tooltip in v1 (deferred)
