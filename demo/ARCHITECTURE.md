# Demo Projects Architecture

This document explains the demo project structure that supports Server-only, WebAssembly-only, and Auto (hybrid) rendering modes.

## Project Structure

### 1. NeoUI.Demo.Shared (Razor Class Library)
**Purpose**: Contains all reusable UI components, pages, services, and static assets shared by every hosting project.

**Contents**:
- All 97+ pages from `/Pages/` directory
- Shared components (MainLayout, DarkModeToggle, SpotlightCommandPalette)
- Routes.razor component
- Services (ThemeService, CollapsibleStateService, MockDataService, KeyboardShortcutService)
- _Imports.razor with common usings
- `wwwroot/css/app-input.css` - Tailwind CSS source
- `wwwroot/css/app.css` - compiled Tailwind CSS output (generated at build)
- `wwwroot/favicon.png` - app favicon
- `wwwroot/images/` - shared images (logo, avatar, etc.)
- `wwwroot/styles/` - base and theme CSS files
- `tailwind.config.js` + `package.json` - CSS build tooling

**References**:
- NeoUI.Blazor
- NeoUI.Blazor.Primitives
- NeoUI.Icons.Lucide
- NeoUI.Icons.Heroicons
- NeoUI.Icons.Feather

**Target Framework**: net10.0

### 2. NeoUI.Demo.Wasm (Blazor WebAssembly)
**Purpose**: Standalone WebAssembly-only hosting application.

**Contents**:
- Program.cs - Configures WebAssembly hosting
- wwwroot/index.html - WASM bootstrap entry point
- _Imports.razor - WebAssembly-specific imports

**References**:
- NeoUI.Demo.Shared (all pages, components, and static assets)

**Key Features**:
- Runs entirely in the browser
- Uses InteractiveWebAssembly render mode
- All static assets (CSS, images, favicon) served from `_content/NeoUI.Demo.Shared/`

**Target Framework**: net10.0

### 3. NeoUI.Demo.Server (Blazor Web App - Server Mode)
**Purpose**: Server-side rendering hosting application.

**Contents**:
- App.razor - HTML document wrapper with InteractiveServer rendermode
- Program.cs - Configures server-side hosting

**References**:
- NeoUI.Demo.Shared (all pages, components, and static assets)

**Key Features**:
- Renders entirely on the server
- Uses InteractiveServer render mode
- All static assets (CSS, images, favicon) served from `_content/NeoUI.Demo.Shared/`

**Target Framework**: net10.0

### 4. NeoUI.Demo.Auto (Blazor Web App - Auto Mode)
**Purpose**: Hybrid hosting application using Auto rendering (Server-first, then WebAssembly).

**Contents**:
- App.razor - HTML document wrapper with InteractiveAuto rendermode
- Program.cs - Configures both Server and WebAssembly components

**References**:
- NeoUI.Demo.Shared (all pages, components, and static assets)
- NeoUI.Demo.Auto.Client (WASM satellite, enables WebAssembly handoff)

**Key Features**:
- Renders on the server first for fast initial load
- Downloads WebAssembly runtime for subsequent interactions
- All static assets (CSS, images, favicon) served from `_content/NeoUI.Demo.Shared/`

**Target Framework**: net10.0

### 5. NeoUI.Demo.Auto.Client (Blazor WebAssembly satellite)
**Purpose**: WebAssembly satellite project for the Auto-mode host.

**Contents**:
- Program.cs - Configures WebAssembly hosting
- App.razor - Router component

**References**:
- NeoUI.Demo.Shared (all pages, components, and static assets)

**Target Framework**: net10.0

## How It Works

### Server Mode (NeoUI.Demo.Server)
1. User navigates to the app
2. All rendering happens on the server (InteractiveServer)
3. All UI components and static assets come from NeoUI.Demo.Shared

### WebAssembly-Only Mode (NeoUI.Demo.Wasm)
1. User navigates to the app
2. WebAssembly runtime downloads
3. App runs entirely in the browser
4. All UI components and static assets come from NeoUI.Demo.Shared via `_content/` paths

### Auto Mode (NeoUI.Demo.Auto)
1. User navigates to the app
2. Initial render happens on the server (InteractiveServer)
3. WebAssembly runtime downloads in the background
4. Subsequent interactions use WebAssembly (InteractiveWebAssembly)
5. All UI components and static assets come from NeoUI.Demo.Shared

## Benefits

1. **Zero Code Duplication**: All pages, components, and static assets are in one place (Shared)
2. **Flexible Deployment**: Run as Server-only, pure WebAssembly, or Server+WASM hybrid
3. **Independent Testing**: Each hosting project can be run and tested separately
4. **Easier Maintenance**: Changes to components or styles only need to be made once in Shared
5. **Better Performance**: Auto mode gives fast initial load + offline capability

## Running the Projects

### Run Server Mode:
```bash
cd demo/NeoUI.Demo.Server
dotnet run
```

### Run Auto Mode (Server + WebAssembly):
```bash
cd demo/NeoUI.Demo.Auto
dotnet run
```

### Run WebAssembly-Only:
```bash
cd demo/NeoUI.Demo.Wasm
dotnet run
```

### Build All:
```bash
dotnet build NeoUI.Blazor.sln
```

## Static Assets Strategy

### CSS and Styling
All CSS is built and owned by NeoUI.Demo.Shared and automatically available to every hosting project:
- **NeoUI.Demo.Shared**: Builds Tailwind CSS from `wwwroot/css/app-input.css` to `wwwroot/css/app.css` via `npm run build:css`
- All hosting projects reference the compiled CSS via `_content/NeoUI.Demo.Shared/css/app.css`
- Tailwind config (`tailwind.config.js`) in Shared scans all project files for utility classes
- Component library styles (`components.css`) are served via `_content/NeoUI.Blazor/` paths
- **Benefit**: Single source of truth for CSS; no duplication across hosting projects

### Asset Management
- **NeoUI.Demo.Shared**: Owns all static assets — CSS, images, favicon, and additional stylesheets
- Hosting projects (Server, Auto, Wasm) contain no static assets of their own
- Assets are served automatically via the `_content/NeoUI.Demo.Shared/` static web assets path
- NeoUI.Demo.Wasm's `wwwroot/index.html` references assets the same way: `_content/NeoUI.Demo.Shared/...`

### Tailwind CSS Build
Tailwind CSS is built in the Shared project:
```xml
<!-- In NeoUI.Demo.Shared.csproj -->
<Target Name="BuildTailwindCSS" BeforeTargets="BeforeBuild" DependsOnTargets="NpmInstall">
  <Exec Command="npm run build:css" WorkingDirectory="$(MSBuildProjectDirectory)" />
</Target>
```

All hosting projects reference the built CSS from Shared:
```html
<!-- In any hosting project's App.razor or index.html -->
<link rel="stylesheet" href="_content/NeoUI.Demo.Shared/css/app.css" />
<link rel="icon" type="image/png" href="_content/NeoUI.Demo.Shared/favicon.png" />
```

## Troubleshooting

### Missing Styles
If any hosting project appears unstyled:
- Ensure Tailwind CSS is built in Shared: `dotnet build demo/NeoUI.Demo.Shared`
- Check that `wwwroot/css/app.css` exists in the Shared project after the build
- All hosting projects reference CSS via `_content/NeoUI.Demo.Shared/css/app.css`
- Alternatively, run `npm run build:css` manually inside `demo/NeoUI.Demo.Shared/`

## Future Enhancements

- Could create platform-specific optimizations per hosting mode
- Could add different Tailwind configurations per mode
