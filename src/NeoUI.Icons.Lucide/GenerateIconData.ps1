# PowerShell script to convert lucide.json to C# dictionary code

$jsonPath = Join-Path $PSScriptRoot "lucide.json"
$outputPath = Join-Path $PSScriptRoot "Data\LucideIconData.cs"

Write-Host "Reading Lucide icon data from $jsonPath..."
$json = Get-Content -Path $jsonPath -Raw | ConvertFrom-Json

$iconProperties = @($json.icons.PSObject.Properties)
$iconCount = $iconProperties.Count
Write-Host "Found $iconCount icons"

# Iconify keeps renamed icons in an "aliases" block that points at the new name.
# Emit them as additional dictionary entries so a name that used to work keeps
# working after an upstream rename.
$entries = [System.Collections.Generic.Dictionary[string, string]]::new([System.StringComparer]::OrdinalIgnoreCase)
foreach ($icon in $iconProperties) {
    $entries[$icon.Name] = $icon.Value.body
}

$aliasCount = 0
$skippedAliases = @()
foreach ($alias in @($json.aliases.PSObject.Properties)) {
    $parent = $alias.Value.parent
    if ($entries.ContainsKey($alias.Name)) { continue }
    if ($parent -and $entries.ContainsKey($parent)) {
        $entries[$alias.Name] = $entries[$parent]
        $aliasCount++
    }
    else {
        $skippedAliases += "$($alias.Name) -> $parent"
    }
}
Write-Host "Found $aliasCount aliases"
if ($skippedAliases.Count -gt 0) {
    Write-Warning "Skipped $($skippedAliases.Count) alias(es) with an unresolved parent: $($skippedAliases -join ', ')"
}

$totalCount = $entries.Count

# Create Data directory if it doesn't exist
$dataDir = Join-Path $PSScriptRoot "Data"
if (!(Test-Path $dataDir)) {
    New-Item -ItemType Directory -Path $dataDir | Out-Null
    Write-Host "Created Data directory"
}

# Start building the C# file
$sb = New-Object System.Text.StringBuilder

[void]$sb.AppendLine("// This file is auto-generated. Do not edit manually.")
[void]$sb.AppendLine("// Generated from lucide.json on $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("namespace NeoUI.Icons.Lucide;")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("/// <summary>")
[void]$sb.AppendLine("/// Provides access to Lucide icon SVG data.")
[void]$sb.AppendLine("/// Contains $totalCount icons from the Lucide icon set ($iconCount icons + $aliasCount aliases).")
[void]$sb.AppendLine("/// </summary>")
[void]$sb.AppendLine("public static class LucideIconData")
[void]$sb.AppendLine("{")
[void]$sb.AppendLine("    private static readonly IReadOnlyDictionary<string, string> Icons = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)")
[void]$sb.AppendLine("    {")

# Add each icon to the dictionary.
# Sort with the ordinal comparer, not Sort-Object: the default sort is
# culture-aware, so the generated ordering would vary by machine locale.
$sortedNames = [string[]]$entries.Keys
[Array]::Sort($sortedNames, [System.StringComparer]::Ordinal)
$lastIndex = $sortedNames.Count - 1
$currentIndex = 0

foreach ($iconName in $sortedNames) {
    $iconBody = $entries[$iconName]

    # Escape double quotes and backslashes in the SVG
    $escapedBody = $iconBody -replace '\\', '\\' -replace '"', '\"'

    # Add comma except for last item
    $comma = if ($currentIndex -eq $lastIndex) { "" } else { "," }

    [void]$sb.AppendLine("        [`"$iconName`"] = `"$escapedBody`"$comma")

    $currentIndex++

    if ($currentIndex % 100 -eq 0) {
        Write-Host "Processed $currentIndex / $totalCount icons..."
    }
}

[void]$sb.AppendLine("    };")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("    /// <summary>")
[void]$sb.AppendLine("    /// Retrieves the SVG content for the specified icon name.")
[void]$sb.AppendLine("    /// </summary>")
[void]$sb.AppendLine("    /// <param name=`"name`">The name of the icon (case-insensitive).</param>")
[void]$sb.AppendLine("    /// <returns>The SVG path data for the icon, or null if not found.</returns>")
[void]$sb.AppendLine("    public static string? GetIcon(string name)")
[void]$sb.AppendLine("    {")
[void]$sb.AppendLine("        return Icons.TryGetValue(name, out var svg) ? svg : null;")
[void]$sb.AppendLine("    }")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("    /// <summary>")
[void]$sb.AppendLine("    /// Gets all available icon names.")
[void]$sb.AppendLine("    /// </summary>")
[void]$sb.AppendLine("    /// <returns>An enumerable collection of icon names.</returns>")
[void]$sb.AppendLine("    public static IEnumerable<string> GetAvailableIcons() => Icons.Keys;")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("    /// <summary>")
[void]$sb.AppendLine("    /// Checks if an icon with the specified name exists.")
[void]$sb.AppendLine("    /// </summary>")
[void]$sb.AppendLine("    /// <param name=`"name`">The name of the icon (case-insensitive).</param>")
[void]$sb.AppendLine("    /// <returns>True if the icon exists, false otherwise.</returns>")
[void]$sb.AppendLine("    public static bool IconExists(string name) => Icons.ContainsKey(name);")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("    /// <summary>")
[void]$sb.AppendLine("    /// Gets the total number of available icons.")
[void]$sb.AppendLine("    /// </summary>")
[void]$sb.AppendLine("    public static int IconCount => Icons.Count;")
[void]$sb.AppendLine("}")

# Write to file
# Write UTF-8 without a BOM: Out-File -Encoding UTF8 emits one on Windows
# PowerShell 5.1, which then has to be stripped by hand.
[System.IO.File]::WriteAllText($outputPath, $sb.ToString(), (New-Object System.Text.UTF8Encoding($false)))
Write-Host "✓ Generated C# file: $outputPath"
Write-Host "✓ Total entries: $totalCount ($iconCount icons + $aliasCount aliases)"
