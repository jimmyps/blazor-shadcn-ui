# Demo Projects Architecture

This document explains the refactored demo project structure that supports both WebAssembly-only and Auto (hybrid) rendering modes.

## Project Structure

### 1. NeoUI.Demo.Shared (Razor Class Library)
**Purpose**: Contains all reusable UI components, pages, and services that work in both rendering modes.

**Contents**:
- All 97+ pages from `/Pages/` directory
- Shared components (MainLayout, DarkModeToggle, SpotlightCommandPalette)
- Routes.razor component
- Services (ThemeService, CollapsibleStateService, MockDataService, KeyboardShortcutService)
- _Imports.razor with common usings

**References**:
- NeoUI.Blazor
- NeoUI.Blazor.Primitives
- NeoUI.Icons.Lucide
- NeoUI.Icons.Heroicons
- NeoUI.Icons.Feather

**Target Framework**: net8.0

### 2. NeoUI.Demo.Client (Blazor WebAssembly)
**Purpose**: Standalone WebAssembly-only application.

**Contents**:
- Program.cs - Configures WebAssembly hosting
- wwwroot/index.html - Entry point for standalone WASM
- _Imports.razor - WebAssembly-specific imports

**References**:
- NeoUI.Demo.Shared (gets all components through this)

**Key Features**:
- Runs entirely in the browser
- Uses InteractiveWebAssembly render mode
- No duplicate static assets (references from Shared and Component libraries)

**Target Framework**: net8.0

### 3. NeoUI.Demo (Blazor Web App - Auto Mode)
**Purpose**: Hybrid application using Auto rendering (Server-first, then WebAssembly).

**Contents**:
- App.razor - HTML document wrapper with InteractiveAuto rendermode
- Program.cs - Configures both Server and WebAssembly components
- wwwroot/ - Static assets (CSS, images, favicon)
- tailwind.config.js - Tailwind CSS configuration

**References**:
- NeoUI.Demo.Shared (gets all components)
- NeoUI.Demo.Client (enables WebAssembly support)

**Key Features**:
- Renders on server first for fast initial load
- Downloads WebAssembly runtime for subsequent interactions
- Manages Tailwind CSS build process
- Serves static assets for both render modes

**Target Framework**: net8.0

## How It Works

### Auto Mode (NeoUI.Demo)
1. User navigates to the app
2. Initial render happens on the server (InteractiveServer)
3. WebAssembly runtime downloads in the background
4. Subsequent interactions use WebAssembly (InteractiveWebAssembly)
5. All UI components come from NeoUI.Demo.Shared

### WebAssembly-Only Mode (NeoUI.Demo.Client)
1. User navigates to the app
2. WebAssembly runtime downloads
3. App runs entirely in the browser
4. All UI components come from NeoUI.Demo.Shared
5. Static assets served from _content/ paths

## Benefits

1. **Zero Code Duplication**: All pages and components are in one place (Shared)
2. **Flexible Deployment**: Run as Server+WASM hybrid OR pure WebAssembly
3. **Independent Testing**: Each demo can be run and tested separately
4. **Easier Maintenance**: Changes to components only need to be made once
5. **Better Performance**: Auto mode gives fast initial load + offline capability

## Running the Projects

### Run Auto Mode (Server + WebAssembly):
```bash
cd demo/NeoUI.Demo
dotnet run
```

### Run WebAssembly-Only:
```bash
cd demo/NeoUI.Demo.Client
dotnet run
```

### Build All:
```bash
dotnet build NeoUI.Blazor.sln
```

## Static Assets Strategy

### CSS and Styling
Tailwind CSS is built once in the Client project and shared with Demo:
- **NeoUI.Demo.Client**: Builds Tailwind CSS from `wwwroot/css/app-input.css` to `wwwroot/css/app.css`
- **NeoUI.Demo**: References Client's CSS via `_content/NeoUI.Demo.Client/css/app.css` (no duplicate build)
- Tailwind config scans the Shared project and component libraries for utility classes
- Component library styles (`components.css`) are served via `_content/` paths
- **Benefit**: Single source of truth for CSS, no duplication

### Asset Management
- **NeoUI.Demo**: Contains static assets (favicon, images, custom JS) but NO CSS
- **NeoUI.Demo.Client**: Contains all CSS and required assets (favicon, CSS)
- Static asset conflicts are avoided by using `StaticWebAssetBasePath` in the Client project when referenced by Demo
- When running standalone, Client serves assets from root path
- When referenced by Auto mode, Client assets are served from `_content/NeoUI.Demo.Client/`

### Tailwind CSS Build
Only the Client project builds Tailwind CSS:
```xml
<!-- In NeoUI.Demo.Client.csproj -->
<Target Name="BuildTailwindCSS" BeforeTargets="BeforeBuild" 
        Condition="Exists('$(MSBuildProjectDirectory)\..\..\tools\tailwindcss.exe')">
  <Exec Command="tailwindcss.exe -i wwwroot/css/app-input.css -o wwwroot/css/app.css" />
</Target>
```

The Demo project references the built CSS:
```html
<!-- In NeoUI.Demo/App.razor -->
<link href="_content/NeoUI.Demo.Client/css/app.css" rel="stylesheet" />
```

## Troubleshooting

### Static Asset Conflicts
If you encounter conflicts about duplicate static assets:
- The Client project uses `StaticWebAssetBasePath` to namespace its assets
- This only applies when Client is referenced as a project dependency
- When published standalone, assets are served from root

### Missing Styles
If either project appears unstyled:
- Ensure Tailwind CSS is built in Client: `dotnet build NeoUI.Demo.Client`
- Check that `wwwroot/css/app.css` exists in the Client project
- Demo references CSS via `_content/NeoUI.Demo.Client/css/app.css`
- Client references CSS via `css/app.css` (standalone mode)

## Future Enhancements

- Could add a third project for pure Server rendering if needed
- Could create platform-specific optimizations
- Could add different Tailwind configurations per mode
