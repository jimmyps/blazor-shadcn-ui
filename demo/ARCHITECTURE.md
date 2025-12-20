# Demo Projects Architecture

This document explains the refactored demo project structure that supports both WebAssembly-only and Auto (hybrid) rendering modes.

## Project Structure

### 1. BlazorUI.Demo.Shared (Razor Class Library)
**Purpose**: Contains all reusable UI components, pages, and services that work in both rendering modes.

**Contents**:
- All 97+ pages from `/Pages/` directory
- Shared components (MainLayout, DarkModeToggle, SpotlightCommandPalette)
- Routes.razor component
- Services (ThemeService, CollapsibleStateService, MockDataService, KeyboardShortcutService)
- _Imports.razor with common usings

**References**:
- BlazorUI.Components
- BlazorUI.Primitives
- BlazorUI.Icons.Lucide
- BlazorUI.Icons.Heroicons
- BlazorUI.Icons.Feather

**Target Framework**: net8.0

### 2. BlazorUI.Demo.Client (Blazor WebAssembly)
**Purpose**: Standalone WebAssembly-only application.

**Contents**:
- Program.cs - Configures WebAssembly hosting
- wwwroot/index.html - Entry point for standalone WASM
- _Imports.razor - WebAssembly-specific imports

**References**:
- BlazorUI.Demo.Shared (gets all components through this)

**Key Features**:
- Runs entirely in the browser
- Uses InteractiveWebAssembly render mode
- No duplicate static assets (references from Shared and Component libraries)

**Target Framework**: net8.0

### 3. BlazorUI.Demo (Blazor Web App - Auto Mode)
**Purpose**: Hybrid application using Auto rendering (Server-first, then WebAssembly).

**Contents**:
- App.razor - HTML document wrapper with InteractiveAuto rendermode
- Program.cs - Configures both Server and WebAssembly components
- wwwroot/ - Static assets (CSS, images, favicon)
- tailwind.config.js - Tailwind CSS configuration

**References**:
- BlazorUI.Demo.Shared (gets all components)
- BlazorUI.Demo.Client (enables WebAssembly support)

**Key Features**:
- Renders on server first for fast initial load
- Downloads WebAssembly runtime for subsequent interactions
- Manages Tailwind CSS build process
- Serves static assets for both render modes

**Target Framework**: net8.0

## How It Works

### Auto Mode (BlazorUI.Demo)
1. User navigates to the app
2. Initial render happens on the server (InteractiveServer)
3. WebAssembly runtime downloads in the background
4. Subsequent interactions use WebAssembly (InteractiveWebAssembly)
5. All UI components come from BlazorUI.Demo.Shared

### WebAssembly-Only Mode (BlazorUI.Demo.Client)
1. User navigates to the app
2. WebAssembly runtime downloads
3. App runs entirely in the browser
4. All UI components come from BlazorUI.Demo.Shared
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
cd demo/BlazorUI.Demo
dotnet run
```

### Run WebAssembly-Only:
```bash
cd demo/BlazorUI.Demo.Client
dotnet run
```

### Build All:
```bash
dotnet build BlazorUI.sln
```

## Static Assets Strategy

### CSS and Styling
Both projects include Tailwind CSS build support:
- **BlazorUI.Demo**: Builds Tailwind CSS from `wwwroot/css/app-input.css` to `wwwroot/css/app.css`
- **BlazorUI.Demo.Client**: Has its own Tailwind build pipeline with the same configuration
- Tailwind config scans both projects and the Shared project for utility classes
- Component library styles (`blazorui.css`) are served via `_content/` paths

### Asset Management
- **BlazorUI.Demo**: Contains all static assets (favicon, images, custom JS)
- **BlazorUI.Demo.Client**: Contains its own copy of required assets (favicon, CSS)
- Static asset conflicts are avoided by using `StaticWebAssetBasePath` in the Client project when referenced by Demo
- When running standalone, Client serves assets from root path
- When referenced by Auto mode, Client assets are served from `_content/BlazorUI.Demo.Client/`

### Tailwind CSS Build
Both projects have a build target that runs before compilation:
```xml
<Target Name="BuildTailwindCSS" BeforeTargets="BeforeBuild" 
        Condition="Exists('$(MSBuildProjectDirectory)\..\..\tools\tailwindcss.exe')">
  <Exec Command="tailwindcss.exe -i wwwroot/css/app-input.css -o wwwroot/css/app.css" />
</Target>
```

## Troubleshooting

### Static Asset Conflicts
If you encounter conflicts about duplicate static assets:
- The Client project uses `StaticWebAssetBasePath` to namespace its assets
- This only applies when Client is referenced as a project dependency
- When published standalone, assets are served from root

### Missing Styles
If the WASM client appears unstyled:
- Ensure Tailwind CSS is built: `dotnet build BlazorUI.Demo.Client`
- Check that `wwwroot/css/app.css` exists in the Client project
- Verify `index.html` references both `css/app.css` and `_content/BlazorUI.Components/blazorui.css`

## Future Enhancements

- Could add a third project for pure Server rendering if needed
- Could create platform-specific optimizations
- Could add different Tailwind configurations per mode
