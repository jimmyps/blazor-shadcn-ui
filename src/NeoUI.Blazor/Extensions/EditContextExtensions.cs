using Microsoft.AspNetCore.Components.Forms;

namespace BlazorUI.Components.Extensions;

/// <summary>
/// Extension methods for EditContext to enhance validation behavior.
/// </summary>
public static class EditContextExtensions
{
    private const string FocusFirstInvalidInputKey = "__FocusFirstInvalidInput__";

    /// <summary>
    /// Requests that the first invalid input should be focused after validation.
    /// This is useful when re-validating a form that already has errors.
    /// </summary>
    /// <param name="editContext">The EditContext to operate on.</param>
    /// <remarks>
    /// <para>
    /// When called, sets a flag that Input components will check during validation.
    /// The first invalid input will be focused and show its validation error tooltip.
    /// </para>
    /// <para>
    /// This is particularly useful when:
    /// - User clicks Save on a form that's already invalid
    /// - You want to ensure the first error always gets focus, even if validation state didn't change
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// if (!editContext.Validate())
    /// {
    ///     editContext.RequestFocusFirstInvalidInput();
    ///     return;
    /// }
    /// </code>
    /// </example>
    public static void RequestFocusFirstInvalidInput(this EditContext editContext)
    {
        if (editContext == null)
            throw new ArgumentNullException(nameof(editContext));

        editContext.Properties[FocusFirstInvalidInputKey] = true;
    }

    /// <summary>
    /// Checks if focus on first invalid input has been requested.
    /// Used internally by Input components.
    /// </summary>
    internal static bool ShouldFocusFirstInvalidInput(this EditContext editContext)
    {
        if (editContext == null)
            return false;

        return editContext.Properties.TryGetValue(FocusFirstInvalidInputKey, out var value) && value is bool flag && flag;
    }

    /// <summary>
    /// Clears the focus request flag.
    /// Used internally by Input components after focusing.
    /// </summary>
    internal static void ClearFocusRequest(this EditContext editContext)
    {
        if (editContext == null)
            return;

        editContext.Properties.Remove(FocusFirstInvalidInputKey);
    }
}
