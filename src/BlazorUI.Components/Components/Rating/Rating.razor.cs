using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.Linq.Expressions;

namespace BlazorUI.Components.Rating;

/// <summary>
/// A rating component for selecting a value with star/heart/circle icons.
/// </summary>
public partial class Rating : ComponentBase, IDisposable
{
    private EditContext? _previousEditContext;
    private FieldIdentifier _fieldIdentifier;
    private double? _hoverValue;
    private double? _lastClickValue;

    [CascadingParameter]
    private EditContext? EditContext { get; set; }

    /// <summary>
    /// Gets or sets the current rating value.
    /// </summary>
    [Parameter]
    public double Value { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the rating value changes.
    /// </summary>
    [Parameter]
    public EventCallback<double> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the maximum rating value.
    /// </summary>
    [Parameter]
    public int MaxRating { get; set; } = 5;

    /// <summary>
    /// Gets or sets whether half ratings are allowed.
    /// </summary>
    [Parameter]
    public bool AllowHalf { get; set; }

    /// <summary>
    /// Gets or sets whether the rating can be cleared by clicking the same value.
    /// </summary>
    [Parameter]
    public bool AllowClear { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the rating is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets whether the rating is read-only.
    /// </summary>
    [Parameter]
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets the size of the rating icons.
    /// </summary>
    [Parameter]
    public RatingSize Size { get; set; } = RatingSize.Medium;

    /// <summary>
    /// Gets or sets the icon type.
    /// </summary>
    [Parameter]
    public RatingIconType IconType { get; set; } = RatingIconType.Star;

    /// <summary>
    /// Gets or sets whether to show validation errors.
    /// </summary>
    [Parameter]
    public bool ShowValidationError { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the HTML id attribute.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the name for form submission.
    /// </summary>
    [Parameter]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the bound value.
    /// </summary>
    [Parameter]
    public Expression<Func<double>>? ValueExpression { get; set; }

    private string ContainerCssClass => ClassNames.cn(
        "inline-flex items-center gap-1",
        Disabled ? "cursor-not-allowed opacity-50" : ReadOnly ? "cursor-default" : "cursor-pointer",
        Class
    );

    private string IconSizeClass => Size switch
    {
        RatingSize.Small => "w-4 h-4",
        RatingSize.Large => "w-8 h-8",
        _ => "w-6 h-6"
    };

    private double GetDisplayValue()
    {
        return _hoverValue ?? Value;
    }

    private bool IsIconFilled(int iconIndex)
    {
        var displayValue = GetDisplayValue();
        return displayValue >= iconIndex;
    }

    private bool IsIconHalfFilled(int iconIndex)
    {
        if (!AllowHalf) return false;
        
        var displayValue = GetDisplayValue();
        return displayValue >= iconIndex - 0.5 && displayValue < iconIndex;
    }

    private async Task OnIconClick(int iconIndex, bool isHalf)
    {
        if (Disabled || ReadOnly) return;

        var newValue = isHalf && AllowHalf ? iconIndex - 0.5 : iconIndex;

        // Allow clearing if clicking the same value
        if (AllowClear && _lastClickValue == newValue)
        {
            newValue = 0;
            _lastClickValue = null;
        }
        else
        {
            _lastClickValue = newValue;
        }

        Value = newValue;
        await ValueChanged.InvokeAsync(Value);

        if (EditContext != null && ValueExpression != null)
        {
            EditContext.NotifyFieldChanged(_fieldIdentifier);
        }

        StateHasChanged();
    }

    private void OnIconMouseEnter(int iconIndex, bool isHalf)
    {
        if (Disabled || ReadOnly) return;
        _hoverValue = isHalf && AllowHalf ? iconIndex - 0.5 : iconIndex;
        StateHasChanged();
    }

    private void OnMouseLeave()
    {
        if (Disabled || ReadOnly) return;
        _hoverValue = null;
        StateHasChanged();
    }

    private async Task OnKeyDown(KeyboardEventArgs e)
    {
        if (Disabled || ReadOnly) return;

        var newValue = Value;
        var handled = false;

        switch (e.Key)
        {
            case "ArrowRight":
            case "ArrowUp":
                newValue = Math.Min(MaxRating, Value + (AllowHalf ? 0.5 : 1));
                handled = true;
                break;
            case "ArrowLeft":
            case "ArrowDown":
                newValue = Math.Max(0, Value - (AllowHalf ? 0.5 : 1));
                handled = true;
                break;
            case "0":
                newValue = 0;
                handled = true;
                break;
            case "1":
            case "2":
            case "3":
            case "4":
            case "5":
            case "6":
            case "7":
            case "8":
            case "9":
                var numValue = int.Parse(e.Key);
                if (numValue <= MaxRating)
                {
                    newValue = numValue;
                    handled = true;
                }
                break;
        }

        if (handled)
        {
            Value = newValue;
            await ValueChanged.InvokeAsync(Value);

            if (EditContext != null && ValueExpression != null)
            {
                EditContext.NotifyFieldChanged(_fieldIdentifier);
            }

            StateHasChanged();
        }
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // Set up field identifier for validation
        if (EditContext != null && ValueExpression != null)
        {
            _fieldIdentifier = FieldIdentifier.Create(ValueExpression);

            // Subscribe to EditContext if it changed
            if (EditContext != _previousEditContext)
            {
                DetachValidationStateChangedListener();
                EditContext.OnValidationStateChanged += OnValidationStateChanged;
                _previousEditContext = EditContext;
            }
        }
    }

    private void OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    private void DetachValidationStateChangedListener()
    {
        if (_previousEditContext != null)
        {
            _previousEditContext.OnValidationStateChanged -= OnValidationStateChanged;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        DetachValidationStateChangedListener();
    }
}
