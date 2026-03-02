using NeoUI.Blazor.Charts;

namespace NeoUI.Demo.Shared.Pages.Components.Charts
{
    /// <summary>
    /// Shared default values for chart component demos.
    /// </summary>
    public static class ChartDefaults
    {
        /// <summary>Gets the default chart padding used across demo examples.</summary>
        public static readonly Padding DefaultPadding = new Padding
        {
            Top = 32, // space for legend
            Right = 0,
            Bottom = 0,
            Left = 0
        };
    }
}
