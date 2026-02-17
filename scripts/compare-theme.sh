#!/usr/bin/env bash
# =============================================================================
# BlazorBlueprint Theme Compatibility Checker
# =============================================================================
# Compares a tweakcn/shadcn theme CSS file against BlazorBlueprint's required
# CSS custom properties to identify gaps in both directions.
#
# Usage:
#   ./scripts/compare-theme.sh <theme-file.css> [--output report.md]
#
# Examples:
#   ./scripts/compare-theme.sh themes/my-theme.css
#   ./scripts/compare-theme.sh themes/my-theme.css --output reports/my-theme-report.md
# =============================================================================

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

# --- BlazorBlueprint variable lists ---

# Required: must be in the user's theme (no library defaults)
BB_REQUIRED_VARS=(
  # Core colors
  "--background"
  "--foreground"
  "--card"
  "--card-foreground"
  "--popover"
  "--popover-foreground"
  "--primary"
  "--primary-foreground"
  "--secondary"
  "--secondary-foreground"
  "--muted"
  "--muted-foreground"
  "--accent"
  "--accent-foreground"
  "--destructive"
  "--destructive-foreground"
  "--border"
  "--input"
  "--ring"

  # Chart colors
  "--chart-1"
  "--chart-2"
  "--chart-3"
  "--chart-4"
  "--chart-5"

  # Sidebar
  "--sidebar"
  "--sidebar-foreground"
  "--sidebar-primary"
  "--sidebar-primary-foreground"
  "--sidebar-accent"
  "--sidebar-accent-foreground"
  "--sidebar-border"
  "--sidebar-ring"

  # Typography
  "--font-sans"
  "--font-serif"
  "--font-mono"

  # Layout / Sizing
  "--radius"
  "--tracking-normal"
  "--spacing"

  # Shadows (Tailwind v4 shadow system)
  "--shadow-x"
  "--shadow-y"
  "--shadow-blur"
  "--shadow-spread"
  "--shadow-opacity"
  "--shadow-color"
  "--shadow-2xs"
  "--shadow-xs"
  "--shadow-sm"
  "--shadow"
  "--shadow-md"
  "--shadow-lg"
  "--shadow-xl"
  "--shadow-2xl"
)

# Optional: have library defaults, but can be overridden by theme
BB_OPTIONAL_VARS=(
  "--alert-success"
  "--alert-success-foreground"
  "--alert-success-bg"
  "--alert-info"
  "--alert-info-foreground"
  "--alert-info-bg"
  "--alert-warning"
  "--alert-warning-foreground"
  "--alert-warning-bg"
  "--alert-danger"
  "--alert-danger-foreground"
  "--alert-danger-bg"
)

# --- Helpers ---

usage() {
  echo "Usage: $0 <theme-file.css> [--output report.md]"
  echo ""
  echo "Compares a tweakcn/shadcn theme CSS file against BlazorBlueprint requirements."
  echo "Report is saved to themes/reports/<name>-report.md by default."
  exit 1
}

# Extract CSS custom property definitions from :root and .dark blocks only
# (excludes @theme inline which is Tailwind v4 boilerplate)
extract_base_vars() {
  local file="$1"
  awk '
    /^:root|^\.dark/ { inside=1 }
    /@theme inline/ { inside=0 }
    inside && /--[a-z]/ { match($0, /--[a-z][a-z0-9-]*/, m); if (m[0]) print m[0] }
    inside && /^\}/ { inside=0 }
  ' "$file" | sort -u
}

# --- Parse args ---

THEME_FILE=""
OUTPUT_FILE=""

while [[ $# -gt 0 ]]; do
  case "$1" in
    --output)
      OUTPUT_FILE="$2"
      shift 2
      ;;
    -h|--help)
      usage
      ;;
    *)
      THEME_FILE="$1"
      shift
      ;;
  esac
done

if [[ -z "$THEME_FILE" ]]; then
  usage
fi

if [[ ! -f "$THEME_FILE" ]]; then
  echo "Error: File not found: $THEME_FILE"
  exit 1
fi

# Auto-generate output path if not specified: themes/reports/<name>-report.md
if [[ -z "$OUTPUT_FILE" ]]; then
  THEME_DIR="$(dirname "$THEME_FILE")"
  THEME_NAME="$(basename "$THEME_FILE" .css)"
  OUTPUT_FILE="${THEME_DIR}/reports/${THEME_NAME}-report.md"
fi

# --- Extract variables from theme file ---

THEME_BASE_VARS=$(extract_base_vars "$THEME_FILE")

# --- Categorize required vars ---

MISSING_REQUIRED=()
MATCHED_REQUIRED=()

for var in "${BB_REQUIRED_VARS[@]}"; do
  if echo "$THEME_BASE_VARS" | grep -Fqx -- "$var"; then
    MATCHED_REQUIRED+=("$var")
  else
    MISSING_REQUIRED+=("$var")
  fi
done

# --- Categorize optional vars ---

MATCHED_OPTIONAL=()
DEFAULTED_OPTIONAL=()

for var in "${BB_OPTIONAL_VARS[@]}"; do
  if echo "$THEME_BASE_VARS" | grep -Fqx -- "$var"; then
    MATCHED_OPTIONAL+=("$var")
  else
    DEFAULTED_OPTIONAL+=("$var")
  fi
done

# --- Check for extras in theme ---

EXTRA_IN_THEME=()
ALL_BB_VARS=("${BB_REQUIRED_VARS[@]}" "${BB_OPTIONAL_VARS[@]}")

for var in $THEME_BASE_VARS; do
  found=false
  for req in "${ALL_BB_VARS[@]}"; do
    if [[ "$var" == "$req" ]]; then
      found=true
      break
    fi
  done
  if [[ "$found" == "false" ]]; then
    EXTRA_IN_THEME+=("$var")
  fi
done

# --- Categorize missing by group ---

categorize_var() {
  local var="$1"
  case "$var" in
    --alert-*)       echo "Alert Semantic Colors" ;;
    --sidebar-*)     echo "Sidebar" ;;
    --chart-*)       echo "Chart Colors" ;;
    --shadow*)       echo "Shadows" ;;
    --font-*)        echo "Typography" ;;
    --tracking-*|--spacing|--radius) echo "Layout / Sizing" ;;
    *)               echo "Core Colors" ;;
  esac
}

# --- Build report ---

TOTAL_REQUIRED=${#BB_REQUIRED_VARS[@]}
TOTAL_OPTIONAL=${#BB_OPTIONAL_VARS[@]}
TOTAL_ALL=$(( TOTAL_REQUIRED + TOTAL_OPTIONAL ))
TOTAL_MATCHED_REQ=${#MATCHED_REQUIRED[@]}
TOTAL_MATCHED_OPT=${#MATCHED_OPTIONAL[@]}
TOTAL_DEFAULTED=${#DEFAULTED_OPTIONAL[@]}
TOTAL_MISSING_REQ=${#MISSING_REQUIRED[@]}
TOTAL_EXTRA=${#EXTRA_IN_THEME[@]}
TOTAL_MATCHED_ALL=$(( TOTAL_MATCHED_REQ + TOTAL_MATCHED_OPT ))

if [[ $TOTAL_REQUIRED -gt 0 ]]; then
  PERCENT_REQ=$(( TOTAL_MATCHED_REQ * 100 / TOTAL_REQUIRED ))
else
  PERCENT_REQ=100
fi

REPORT=""
REPORT+="# Theme Compatibility Report\n"
REPORT+="\n"
REPORT+="**Theme file:** \`$(basename "$THEME_FILE")\`\n"
REPORT+="**Date:** $(date +%Y-%m-%d)\n"
REPORT+="\n"
REPORT+="## Summary\n"
REPORT+="\n"
REPORT+="| Metric | Count |\n"
REPORT+="| --- | --- |\n"
REPORT+="| Required variables (no library default) | $TOTAL_REQUIRED |\n"
REPORT+="| Required — matched by theme | $TOTAL_MATCHED_REQ |\n"
REPORT+="| Required — **missing** | $TOTAL_MISSING_REQ |\n"
REPORT+="| Optional variables (have library defaults) | $TOTAL_OPTIONAL |\n"
REPORT+="| Optional — customized by theme | $TOTAL_MATCHED_OPT |\n"
REPORT+="| Optional — using library defaults | $TOTAL_DEFAULTED |\n"
REPORT+="| Extra in theme (not used by BB) | $TOTAL_EXTRA |\n"
REPORT+="| **Required compatibility** | **${PERCENT_REQ}%** |\n"
REPORT+="\n"

if [[ $TOTAL_MISSING_REQ -eq 0 && $TOTAL_DEFAULTED -gt 0 ]]; then
  REPORT+="> **Drop-in compatible.** All required variables are present. ${TOTAL_DEFAULTED} optional alert variables will use library defaults (override in your theme to customize).\n"
  REPORT+="\n"
elif [[ $TOTAL_MISSING_REQ -eq 0 && $TOTAL_DEFAULTED -eq 0 ]]; then
  REPORT+="> **Fully compatible.** All required and optional variables are present.\n"
  REPORT+="\n"
fi

# --- Missing required ---

if [[ ${#MISSING_REQUIRED[@]} -gt 0 ]]; then
  REPORT+="## Missing Required Variables\n"
  REPORT+="\n"
  REPORT+="These variables have **no library default** and must be defined in your theme.\n"
  REPORT+="Components using these will have broken styling without them.\n"
  REPORT+="\n"

  declare -A MISSING_GROUPS
  for var in "${MISSING_REQUIRED[@]}"; do
    group=$(categorize_var "$var")
    MISSING_GROUPS["$group"]+="$var\n"
  done

  for group in "${!MISSING_GROUPS[@]}"; do
    REPORT+="### $group\n"
    REPORT+="\n"
    REPORT+="| Variable | Impact |\n"
    REPORT+="| --- | --- |\n"
    while IFS= read -r var; do
      [[ -z "$var" ]] && continue
      REPORT+="| \`$var\` | Used across components |\n"
    done <<< "$(echo -e "${MISSING_GROUPS[$group]}")"
    REPORT+="\n"
  done
fi

# --- Optional vars using defaults ---

if [[ ${#DEFAULTED_OPTIONAL[@]} -gt 0 ]]; then
  REPORT+="## Optional Variables Using Library Defaults\n"
  REPORT+="\n"
  REPORT+="These variables have sensible defaults shipped in the library CSS.\n"
  REPORT+="The Alert and Toast components will work out-of-the-box. Override in your theme to customize.\n"
  REPORT+="\n"
  REPORT+="| Variable | Default source | Impact |\n"
  REPORT+="| --- | --- | --- |\n"
  for var in "${DEFAULTED_OPTIONAL[@]}"; do
    case "$var" in
      --alert-danger)            REPORT+="| \`$var\` | Derived from \`--destructive\` | Danger Alert border/icon |\n" ;;
      --alert-danger-foreground) REPORT+="| \`$var\` | Derived from \`--destructive-foreground\` | Danger Alert text |\n" ;;
      --alert-danger-bg)         REPORT+="| \`$var\` | Fixed oklch(0.993 0.003 27) | Danger Alert background |\n" ;;
      --alert-success)           REPORT+="| \`$var\` | Fixed oklch (hue 142) | Success Alert border/icon |\n" ;;
      --alert-success-foreground) REPORT+="| \`$var\` | Fixed oklch (hue 142) | Success Alert text |\n" ;;
      --alert-success-bg)        REPORT+="| \`$var\` | Fixed oklch (hue 142) | Success Alert background |\n" ;;
      --alert-info)              REPORT+="| \`$var\` | Fixed oklch (hue 255) | Info Alert border/icon |\n" ;;
      --alert-info-foreground)   REPORT+="| \`$var\` | Fixed oklch (hue 255) | Info Alert text |\n" ;;
      --alert-info-bg)           REPORT+="| \`$var\` | Fixed oklch (hue 255) | Info Alert background |\n" ;;
      --alert-warning)           REPORT+="| \`$var\` | Fixed oklch (hue 55) | Warning Alert border/icon |\n" ;;
      --alert-warning-foreground) REPORT+="| \`$var\` | Fixed oklch (hue 55) | Warning Alert text |\n" ;;
      --alert-warning-bg)        REPORT+="| \`$var\` | Fixed oklch (hue 55) | Warning Alert background |\n" ;;
      *)                         REPORT+="| \`$var\` | Library default | Override to customize |\n" ;;
    esac
  done
  REPORT+="\n"
fi

# --- Optional vars customized by theme ---

if [[ ${#MATCHED_OPTIONAL[@]} -gt 0 ]]; then
  REPORT+="## Optional Variables Customized by Theme\n"
  REPORT+="\n"
  REPORT+="These optional variables are explicitly defined in the theme (overriding library defaults).\n"
  REPORT+="\n"
  REPORT+="| Variable | Status |\n"
  REPORT+="| --- | --- |\n"
  for var in "${MATCHED_OPTIONAL[@]}"; do
    REPORT+="| \`$var\` | Theme override |\n"
  done
  REPORT+="\n"
fi

# --- Extra in theme ---

if [[ ${#EXTRA_IN_THEME[@]} -gt 0 ]]; then
  REPORT+="## Extra in Theme (not used by BlazorBlueprint)\n"
  REPORT+="\n"
  REPORT+="These variables are defined in the theme but **not referenced** by any BlazorBlueprint component.\n"
  REPORT+="They are harmless (no negative impact) but represent unused theme surface area.\n"
  REPORT+="\n"
  REPORT+="| Variable | Notes |\n"
  REPORT+="| --- | --- |\n"
  for var in "${EXTRA_IN_THEME[@]}"; do
    REPORT+="| \`$var\` | Not referenced by any BB component |\n"
  done
  REPORT+="\n"
fi

# --- @theme inline check ---

REPORT+="## @theme inline Compatibility\n"
REPORT+="\n"

if grep -q '@theme inline' "$THEME_FILE"; then
  REPORT+="The theme includes a \`@theme inline\` block for Tailwind v4 integration.\n"
  REPORT+="\n"

  if grep -q 'tracking-tighter' "$THEME_FILE"; then
    REPORT+="- Tracking variants: Present in theme\n"
  else
    REPORT+="- Tracking variants: Provided by library (no action needed)\n"
  fi

  if grep -q 'color-alert' "$THEME_FILE"; then
    REPORT+="- Alert color mappings: Present in theme\n"
  else
    REPORT+="- Alert color mappings: Provided by library via \`tailwind.config.js\` (no action needed)\n"
  fi
else
  REPORT+="The theme does **not** include a \`@theme inline\` block.\n"
  REPORT+="BlazorBlueprint provides tracking variants and alert color mappings in the library CSS, so this is fine for basic usage.\n"
  REPORT+="For full Tailwind v4 integration (custom utilities), consider adding a \`@theme inline\` block.\n"
fi

REPORT+="\n"

# --- Matched required (collapsible) ---

REPORT+="## Matched Required Variables (${TOTAL_MATCHED_REQ}/${TOTAL_REQUIRED})\n"
REPORT+="\n"
REPORT+="<details>\n"
REPORT+="<summary>Click to expand full list of matched variables</summary>\n"
REPORT+="\n"
REPORT+="| Variable | Status |\n"
REPORT+="| --- | --- |\n"
for var in "${MATCHED_REQUIRED[@]}"; do
  REPORT+="| \`$var\` | Matched |\n"
done
REPORT+="\n"
REPORT+="</details>\n"
REPORT+="\n"

# --- Output ---

mkdir -p "$(dirname "$OUTPUT_FILE")"
echo -e "$REPORT" > "$OUTPUT_FILE"
echo "Report written to: $OUTPUT_FILE"
