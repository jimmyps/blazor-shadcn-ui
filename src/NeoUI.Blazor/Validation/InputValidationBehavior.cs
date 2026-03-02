using BlazorUI.Components.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;

namespace BlazorUI.Components.Validation;

/// <summary>
/// Reusable validation behavior for input components integrated with EditContext.
/// Handles validation state changes, error display, and focus management.
/// </summary>
public class InputValidationBehavior : IAsyncDisposable
{
    private static readonly string FirstInvalidInputIdKey = "firstInvalidInputId";
    
    private readonly ComponentBase _owner;
    private readonly Func<string> _getEffectiveId;
    private readonly Func<EditContext?> _getEditContext;
    private readonly Func<bool> _shouldShowValidation;
    private readonly Func<IJSObjectReference?> _getJsModule;
    
    private EditContext? _previousEditContext;
    private FieldIdentifier _fieldIdentifier;
    private string? _currentErrorMessage;
    private bool _hasShownTooltip;

    /// <summary>
    /// Gets the effective AriaInvalid value based on validation state.
    /// </summary>
    public bool? EffectiveAriaInvalid => 
        _getEditContext() != null && _shouldShowValidation() 
            ? !string.IsNullOrEmpty(_currentErrorMessage) 
            : null;

    /// <summary>
    /// Initializes a new instance of the <see cref="InputValidationBehavior"/> class.
    /// </summary>
    /// <param name="owner">The component that owns this behavior (for InvokeAsync and StateHasChanged).</param>
    /// <param name="getEffectiveId">Function to get the component's effective ID.</param>
    /// <param name="getEditContext">Function to get the current EditContext.</param>
    /// <param name="shouldShowValidation">Function to check if validation should be shown.</param>
    /// <param name="getJsModule">Function to get the JS module reference.</param>
    public InputValidationBehavior(
        ComponentBase owner,
        Func<string> getEffectiveId,
        Func<EditContext?> getEditContext,
        Func<bool> shouldShowValidation,
        Func<IJSObjectReference?> getJsModule)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        _getEffectiveId = getEffectiveId ?? throw new ArgumentNullException(nameof(getEffectiveId));
        _getEditContext = getEditContext ?? throw new ArgumentNullException(nameof(getEditContext));
        _shouldShowValidation = shouldShowValidation ?? throw new ArgumentNullException(nameof(shouldShowValidation));
        _getJsModule = getJsModule ?? throw new ArgumentNullException(nameof(getJsModule));
    }

    /// <summary>
    /// Sets up field identifier. Call this from OnParametersSet.
    /// The component should subscribe to EditContext.OnValidationStateChanged and call HandleValidationStateChangedAsync.
    /// </summary>
    /// <typeparam name="TValue">The type of the value being validated.</typeparam>
    /// <param name="valueExpression">Expression for the value being validated.</param>
    public void OnParametersSet<TValue>(Expression<Func<TValue>>? valueExpression)
    {
        if (!_shouldShowValidation() || valueExpression == null)
            return;
        
        _fieldIdentifier = FieldIdentifier.Create(valueExpression);
        
        var editContext = _getEditContext();
        if (editContext != _previousEditContext)
        {
            _previousEditContext = editContext;
        }
    }

    /// <summary>
    /// Updates the validation display (error messages, focus, aria-invalid).
    /// Call this from OnAfterRenderAsync and when validation state changes.
    /// </summary>
    /// <returns>True if the validation state changed and component should re-render</returns>
    public async Task<bool> UpdateValidationDisplayAsync()
    {
        var editContext = _getEditContext();
        var jsModule = _getJsModule();
        
        if (editContext == null || jsModule == null)
            return false;

        bool becomesInvalid = false;

        try
        {
            var messages = editContext.GetValidationMessages(_fieldIdentifier).ToList();
            var errorMessage = messages.FirstOrDefault();

            var focusRequested = editContext.ShouldFocusFirstInvalidInput();

            becomesInvalid = errorMessage != _currentErrorMessage;
            _currentErrorMessage = errorMessage;

            if (!string.IsNullOrEmpty(errorMessage))
            {
                string? firstInvalidId = null;
                if (editContext.Properties.TryGetValue(FirstInvalidInputIdKey, out var value))
                {
                    firstInvalidId = value as string;
                }

                var effectiveId = _getEffectiveId();
                var isFirstInvalid = firstInvalidId == null;
                var isCurrentlyFirstInvalid = firstInvalidId == effectiveId;

                if (isFirstInvalid)
                {
                    editContext.Properties[FirstInvalidInputIdKey] = effectiveId;
                }

                if ((isFirstInvalid && !_hasShownTooltip) || (focusRequested && isCurrentlyFirstInvalid))
                {
                    // Only steal focus if explicitly requested (e.g., form submit validation)
                    // Don't steal focus during natural field navigation/tabbing
                    bool shouldFocus = focusRequested;
                    
                    await jsModule.InvokeVoidAsync(
                        "setValidationError",
                        effectiveId,
                        errorMessage,
                        shouldFocus
                    );

                    becomesInvalid = true;
                    _hasShownTooltip = true;
                    
                    if (focusRequested)
                    {
                        editContext.ClearFocusRequest();
                    }
                }
                else
                {
                    await jsModule.InvokeVoidAsync(
                        "setValidationErrorSilent",
                        effectiveId,
                        errorMessage
                    );
                }
            }
            else
            {
                await jsModule.InvokeVoidAsync(
                    "clearValidationError",
                    _getEffectiveId()
                );
                
                _hasShownTooltip = false;
            }
        }
        catch (JSException)
        {
            return false;
        }

        return becomesInvalid;
    }

    /// <summary>
    /// Handles validation state changes from EditContext.
    /// Call this from the component's EditContext.OnValidationStateChanged event handler.
    /// Returns true if the component should call StateHasChanged().
    /// </summary>
    public async Task<bool> HandleValidationStateChangedAsync()
    {
        var editContext = _getEditContext();
        if (editContext == null)
            return false;

        bool hasChanges = false;

        if (editContext.Properties.TryGetValue(FirstInvalidInputIdKey, out var storedValue))
        {
            var storedId = storedValue as string;
            var thisInputErrors = editContext.GetValidationMessages(_fieldIdentifier).Any();
            
            if (storedId == _getEffectiveId() && !thisInputErrors)
            {
                editContext.Properties.Remove(FirstInvalidInputIdKey);
                hasChanges = true;
            }
        }

        var hasValidationChanges = await UpdateValidationDisplayAsync();
        return hasValidationChanges || hasChanges;
    }

    /// <summary>
    /// Notifies EditContext that the field value changed and updates validation state.
    /// Call this when the input value changes.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task NotifyFieldChangedAsync()
    {
        var editContext = _getEditContext();
        if (editContext == null)
            return;

        editContext.NotifyFieldChanged(_fieldIdentifier);
        
        if (_shouldShowValidation())
        {
            await UpdateValidationDisplayAsync();
        }
    }

    /// <summary>
    /// Disposes the validation behavior and releases resources.
    /// </summary>
    /// <returns>A task representing the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await ValueTask.CompletedTask;
    }
}

