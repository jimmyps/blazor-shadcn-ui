using System;

namespace NeoUI.Blazor.Attributes;

/// <summary>
/// Marks a method as a grid cell action handler.
/// The method will be automatically discovered and registered when used within a DataGrid component.
/// </summary>
/// <remarks>
/// <para>
/// Methods marked with this attribute will be automatically invoked when a cell template
/// contains an element with a matching data-action attribute.
/// </para>
/// <para>
/// The method must accept a single parameter of type TItem (the grid item type) and
/// can return void, Task, or ValueTask.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Grid Items="@orders" ActionHost="this"&gt;
///     &lt;Columns&gt;
///         &lt;DataGridColumn Id="actions"&gt;
///             &lt;CellTemplate Context="order"&gt;
///                 &lt;button data-action="Edit"&gt;Edit&lt;/button&gt;
///                 &lt;button data-action="Delete"&gt;Delete&lt;/button&gt;
///             &lt;/CellTemplate&gt;
///         &lt;/DataGridColumn&gt;
///     &lt;/Columns&gt;
/// &lt;/Grid&gt;
/// 
/// @code {
///     [DataGridAction] // Matches data-action="Edit"
///     private void Edit(Order order)
///     {
///         // Handle edit
///     }
///     
///     [DataGridAction(Name = "Delete")] // Explicit name
///     private void HandleDelete(Order order)
///     {
///         // Handle delete
///     }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class DataGridActionAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the action name that will be matched against data-action attributes.
    /// If not specified, the method name will be used.
    /// </summary>
    public string? Name { get; set; }
}
