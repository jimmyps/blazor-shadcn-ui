# Chart Components

BlazorUI Chart components provide beautiful, themeable data visualizations built with Chart.js and shadcn-inspired styling.

## Available Components

### ChartPanel
Container component with header, content, and footer sections for consistent chart layouts.

**Usage:**
```razor
<ChartPanel>
    <ChartPanelHeader>
        <ChartPanelTitle>Sales Overview</ChartPanelTitle>
        <ChartPanelDescription>Monthly sales data</ChartPanelDescription>
    </ChartPanelHeader>
    <ChartPanelContent>
        <!-- Chart goes here -->
    </ChartPanelContent>
</ChartPanel>
```

### LineChart
Display trends over time or continuous data with customizable line styles.

**Features:**
- Curved or straight lines
- Area fill support
- Data point markers
- Multi-color support

**Usage:**
```razor
<LineChart Data="@salesData" 
          XAxisDataKey="Month"
          YAxisDataKey="Sales"
          Label="Sales ($)"
          Height="300"
          Curved="true"
          FillArea="false" />
```

### BarChart
Compare values across categories with vertical or horizontal bars.

**Features:**
- Vertical and horizontal orientations
- Grouped or stacked modes
- Customizable bar appearance
- Border radius support

**Usage:**
```razor
<BarChart Data="@revenueData" 
         XAxisDataKey="Product"
         YAxisDataKey="Revenue"
         Label="Revenue"
         Height="300"
         Orientation="BarChartOrientation.Vertical" />
```

### PieChart / DonutChart
Show composition and proportions with pie or donut charts.

**Features:**
- Pie or donut variants
- Automatic color distribution
- Legend positioning
- Center content for donut charts

**Usage:**
```razor
<!-- Pie Chart -->
<PieChart Data="@marketShareData" 
         LabelDataKey="Browser"
         ValueDataKey="Share"
         Height="350" />

<!-- Donut Chart with Center Content -->
<PieChart Data="@categoryData" 
         LabelDataKey="Category"
         ValueDataKey="Amount"
         IsDonut="true"
         DonutInnerRadius="0.65">
    <CenterContent>
        <div class="text-center">
            <div class="text-2xl font-bold">$45K</div>
            <div class="text-xs text-muted-foreground">Total</div>
        </div>
    </CenterContent>
</PieChart>
```

## Theming

Charts use CSS variables from the BlazorUI theme system for consistent styling:

- `--chart-1` through `--chart-5`: Data series colors
- `--background`: Chart background
- `--foreground`: Text and labels
- `--border`: Grid lines and borders
- `--muted-foreground`: Secondary text

These variables automatically adapt to light/dark mode.

## Animation

All charts support smooth animations with the following configuration:

```razor
<LineChart Animation="@(new ChartAnimation 
{ 
    Enabled = true, 
    Duration = 750, 
    Easing = AnimationEasing.EaseInOutQuart 
})" />
```

Animations automatically respect the user's `prefers-reduced-motion` setting for accessibility.

## Common Parameters

All chart components share these parameters:

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Data` | `IEnumerable<TData>` | Required | Data to display |
| `Height` | `int` | 350 | Chart height in pixels |
| `Responsive` | `bool` | true | Enable responsive sizing |
| `ShowLegend` | `bool` | true | Display legend |
| `ShowTooltip` | `bool` | true | Enable tooltips |
| `ShowXAxis` | `bool` | true | Display X axis |
| `ShowYAxis` | `bool` | true | Display Y axis |
| `ShowGrid` | `bool` | true | Show grid lines |
| `DisableAnimations` | `bool` | false | Disable animations |
| `Class` | `string?` | null | Additional CSS classes |

## Technical Details

### Dependencies

Charts use Chart.js v4.4.1, loaded dynamically from CDN. No additional packages need to be installed.

### Resource Management

All chart components implement `IAsyncDisposable` for proper cleanup:

```csharp
@implements IAsyncDisposable

// Component automatically disposes Chart.js instances
```

### Data Binding

Charts use reflection to access data properties specified by `DataKey` parameters. Ensure your data classes have public properties:

```csharp
public class SalesData
{
    public string Month { get; set; }
    public int Sales { get; set; }
}
```

## Examples

See `/Components/Chart` in the demo application for comprehensive examples of all chart types.
