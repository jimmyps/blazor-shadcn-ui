namespace NeoUI.Demo.Shared.Pages.Components;

partial class TreeViewDemo
{
    // ── File tree (nested) ────────────────────────────────────────────────

    private record FileNode(string Name, string Path, List<FileNode>? Children = null);

    private static readonly List<FileNode> _fileTree =
    [
        new("src", "src", [
            new("NeoUI.Blazor", "src/NeoUI.Blazor", [
                new("Components", "src/NeoUI.Blazor/Components", [
                    new("Button.razor", "src/NeoUI.Blazor/Components/Button.razor"),
                    new("Card.razor",   "src/NeoUI.Blazor/Components/Card.razor"),
                ]),
                new("wwwroot", "src/NeoUI.Blazor/wwwroot", [
                    new("app.js", "src/NeoUI.Blazor/wwwroot/app.js"),
                ]),
            ]),
            new("NeoUI.Demo", "src/NeoUI.Demo", [
                new("Pages", "src/NeoUI.Demo/Pages", [
                    new("Index.razor", "src/NeoUI.Demo/Pages/Index.razor"),
                ]),
            ]),
        ]),
        new("README.md",      "README.md"),
        new("blazor-ui.sln",  "blazor-ui.sln"),
    ];

    // ── Org tree (nested, for checkboxes & multi-select) ──────────────────

    private record OrgNode(string Id, string Name, List<OrgNode>? Reports = null);

    private static readonly List<OrgNode> _orgTree =
    [
        new("ceo",  "Alice Chen (CEO)", [
            new("cto", "Bob Smith (CTO)", [
                new("swe1", "Carol Lee (SWE)"),
                new("sre1", "Dave Park (SRE)"),
            ]),
            new("cpo", "Eva Brown (CPO)", [
                new("pm1",   "Frank Wu (PM)"),
                new("ux1",   "Grace Kim (Design)"),
            ]),
        ]),
    ];

    private static readonly List<OrgNode> _orgFlat = FlattenOrg(_orgTree);

    private static List<OrgNode> FlattenOrg(IEnumerable<OrgNode> nodes)
    {
        var result = new List<OrgNode>();
        foreach (var n in nodes) { result.Add(n); if (n.Reports is { } r) result.AddRange(FlattenOrg(r)); }
        return result;
    }

    // ── Flat category list ────────────────────────────────────────────────

    private record CategoryNode(string Id, string? ParentId, string Label);

    private static readonly List<CategoryNode> _categories =
    [
        new("1",  null, "Electronics"),
        new("2",  null, "Clothing"),
        new("3",  null, "Books"),
        new("4",  "1",  "Laptops"),
        new("5",  "1",  "Phones"),
        new("6",  "1",  "Accessories"),
        new("7",  "2",  "Men's"),
        new("8",  "2",  "Women's"),
        new("9",  "3",  "Fiction"),
        new("10", "3",  "Non-fiction"),
    ];

    // ── Lazy nodes ────────────────────────────────────────────────────────

    private record LazyNode(string Id, string Label, bool HasChildren, List<LazyNode>? Children = null);

    private static readonly List<LazyNode> _lazyRoots =
    [
        new("docs",   "Documents", true),
        new("pics",   "Pictures",  true),
        new("readme", "README.md", false),
    ];

    // ── Feature installer ─────────────────────────────────────────────────

    private record FeatureNode(string Id, string Name, string Icon, long SizeMb, List<FeatureNode>? Children = null);

    private static readonly List<FeatureNode> _featureTree =
    [
        new("core", "Core Platform", "package", 0,
        [
            new("runtime",   "Runtime Engine",    "cpu",      28),
            new("cli",       "CLI Tools",          "terminal",  8),
            new("sdk",       "SDK & Build Tools",  "code",     14),
        ]),
        new("devtools", "Developer Tools", "wrench", 0,
        [
            new("debugger",   "Debugger",        "bug",        24),
            new("profiler",   "Profiler",         "activity",   19),
            new("testrunner", "Test Runner",      "flask-conical", 13),
            new("formatter",  "Code Formatter",   "align-left",  4),
        ]),
        new("docs-pkg", "Documentation", "book-open", 0,
        [
            new("api-docs",   "API Reference",        "file-text",       9),
            new("tutorials",  "Tutorials & Guides",   "graduation-cap",  7),
        ]),
        new("cloud", "Cloud Integrations", "cloud", 0,
        [
            new("aws", "Amazon Web Services", "cloud", 0,
            [
                new("aws-lambda", "Lambda Functions", "zap",        5),
                new("aws-s3",     "S3 Storage",       "database",   4),
                new("aws-rds",    "RDS Database",     "hard-drive", 5),
            ]),
            new("azure", "Microsoft Azure",        "cloud", 14),
            new("gcp",   "Google Cloud Platform",  "cloud", 14),
        ]),
        new("ui", "UI Framework", "monitor", 0,
        [
            new("ui-core",   "Core Components",  "layers",    20),
            new("ui-data",   "Data Display",      "table",     17),
            new("ui-charts", "Charts & Graphs",   "chart-column", 21),
        ]),
    ];

    private static IEnumerable<FeatureNode> FlattenFeatures(IEnumerable<FeatureNode> nodes)
    {
        foreach (var n in nodes)
        {
            yield return n;
            if (n.Children is { } c) foreach (var d in FlattenFeatures(c)) yield return d;
        }
    }

    private HashSet<string> _installFeatures = new();

    private IEnumerable<FeatureNode> SelectedLeaves =>
        FlattenFeatures(_featureTree)
            .Where(n => (n.Children == null || n.Children.Count == 0)
                        && _installFeatures.Contains(n.Id));

    private long   SelectedSizeMb       => SelectedLeaves.Sum(n => n.SizeMb);
    private int    SelectedFeatureCount => SelectedLeaves.Count();
    private bool   _installed;

    private void HandleInstall() => _installed = true;
    private void HandleReset()   { _installFeatures = new(); _installed = false; }

    // ── Props table ───────────────────────────────────────────────────────

    private static readonly IReadOnlyList<DemoPropRow> _props =
    [
        new("Items",              "IEnumerable<TItem>?",                    "null",  "Data source. Use ChildrenProperty for nested or ParentField for flat mode."),
        new("ValueField",         "Func<TItem, string>?",                   "null",  "Extracts the unique string identifier for each node."),
        new("TextField",          "Func<TItem, string>?",                   "null",  "Extracts the display label for each node."),
        new("ChildrenProperty",   "Func<TItem, IEnumerable<TItem>?>?",      "null",  "Returns child items for nested data."),
        new("ParentField",        "Func<TItem, string?>?",                  "null",  "Returns the parent's ValueField for flat data."),
        new("IconField",          "Func<TItem, string?>?",                  "null",  "Returns a LucideIcon name for each node."),
        new("SelectionMode",      "TreeSelectionMode",                      "None",  "None, Single, or Multiple."),
        new("SelectedValue",      "string?",                                "null",  "Selected node's value (single mode, two-way binding)."),
        new("SelectedValues",     "HashSet<string>?",                       "null",  "Selected nodes' values (multi mode, two-way binding)."),
        new("Checkable",          "bool",                                   "false", "Shows checkboxes; use CheckedValues/@bind-CheckedValues."),
        new("CheckedValues",      "HashSet<string>?",                       "null",  "Checked node values (two-way binding)."),
        new("PropagateChecks",    "bool",                                   "false", "When true, checking a parent checks all descendants; partial selection shows indeterminate on ancestors."),
        new("DefaultExpandAll",   "bool",                                   "false", "Expands all nodes on first render."),
        new("DefaultExpandDepth", "int?",                                   "null",  "Auto-expands to this depth on first render."),
        new("ShowLines",          "bool",                                   "false", "Draws connecting lines between sibling nodes."),
        new("Draggable",          "bool",                                   "false", "Enables drag-and-drop reordering."),
        new("AllowDrag",          "Func<TItem, bool>?",                     "null",  "Per-item predicate to restrict draggable nodes."),
        new("SearchText",         "string?",                                "null",  "Filters and highlights nodes matching this text. Parents of matching nodes stay visible."),
        new("LoadingNodes",       "HashSet<string>",                        "[]",    "Node values currently in async loading state — shows spinner."),
        new("ErrorNodes",         "HashSet<string>",                        "[]",    "Node values that failed to load — shows Retry button."),
        new("OnRetryLoad",        "EventCallback<string>",                  "",      "Fires when user clicks Retry on a failed node."),
        new("LoadChildrenAsync",  "Func<TItem, Task<IEnumerable<TItem>>>?", "null",  "When set, called on first expand; tree manages loading/error/cache automatically. Throw to trigger error state."),
        new("OnNodeClick",        "EventCallback<string>",                  "",      "Fires when a node is clicked."),
        new("OnNodeExpand",       "EventCallback<string>",                  "",      "Fires when a node is expanded."),
        new("OnNodeCollapse",     "EventCallback<string>",                  "",      "Fires when a node is collapsed."),
        new("OnNodeDrop",         "EventCallback<(string, string, string)>","",      "Fires when a drag-drop completes."),
    ];

    private static class TreeViewDemoCode
    {
        public const string Basic =
            """
            <TreeView TItem="FileNode"
                      Items="@_fileTree"
                      ChildrenProperty="@(n => n.Children)"
                      ValueField="@(n => n.Path)"
                      TextField="@(n => n.Name)"
                      IconField="@(n => n.Children?.Count > 0 ? "folder" : "file")"
                      SelectionMode="TreeSelectionMode.Single"
                      @bind-SelectedValue="_selectedPath" />

            @code {
                private record FileNode(string Name, string Path, List<FileNode>? Children = null);
            }
            """;

        public const string Checkboxes =
            """
            <TreeView TItem="OrgNode"
                      Items="@_orgTree"
                      ChildrenProperty="@(n => n.Reports)"
                      ValueField="@(n => n.Id)"
                      TextField="@(n => n.Name)"
                      IconField="@(n => n.Reports?.Count > 0 ? "users" : "user")"
                      Checkable="true"
                      DefaultExpandAll="true"
                      @bind-CheckedValues="_checkedPeople" />
            """;

        public const string MultiSelect =
            """
            <TreeView TItem="OrgNode"
                      Items="@_orgTree"
                      ChildrenProperty="@(n => n.Reports)"
                      ValueField="@(n => n.Id)"
                      TextField="@(n => n.Name)"
                      SelectionMode="TreeSelectionMode.Multiple"
                      DefaultExpandAll="true"
                      @bind-SelectedValues="_selectedPeople" />
            """;

        public const string FlatData =
            """
            <TreeView TItem="CategoryNode"
                      Items="@_categories"
                      ValueField="@(c => c.Id)"
                      ParentField="@(c => c.ParentId)"
                      TextField="@(c => c.Label)"
                      IconField="@(c => c.ParentId == null ? "layers" : "tag")"
                      SelectionMode="TreeSelectionMode.Single"
                      @bind-SelectedValue="_selectedCategory" />

            @code {
                private record CategoryNode(string Id, string? ParentId, string Label);
            }
            """;

        public const string Lines =
            """
            <TreeView TItem="FileNode"
                      Items="@_fileTree"
                      ChildrenProperty="@(n => n.Children)"
                      ValueField="@(n => n.Path)"
                      TextField="@(n => n.Name)"
                      DefaultExpandAll="true"
                      ShowLines="true" />
            """;

        public const string Search =
            """
            <Input @bind-Value="_searchText" Placeholder="Search nodes…" Class="h-8 text-sm" />
            <TreeView TItem="FileNode"
                      Items="@_fileTree"
                      ChildrenProperty="@(n => n.Children)"
                      ValueField="@(n => n.Path)"
                      TextField="@(n => n.Name)"
                      IconField="@(n => n.Children?.Count > 0 ? "folder" : "file")"
                      SearchText="@_searchText"
                      DefaultExpandAll="true" />

            @code {
                private string _searchText = "";
            }
            """;

        public const string PropagateChecks =
            """
            <TreeView TItem="FeatureNode"
                      Items="@_featureTree"
                      ChildrenProperty="@(n => n.Children)"
                      ValueField="@(n => n.Id)"
                      TextField="@(n => n.Name)"
                      IconField="@(n => n.Icon)"
                      Checkable="true"
                      PropagateChecks="true"
                      DefaultExpandAll="true"
                      @bind-CheckedValues="_installFeatures" />

            @* Summary bar *@
            <div class="flex items-center justify-between">
                <span class="text-sm text-muted-foreground">
                    @SelectedFeatureCount features · @SelectedSizeMb MB
                </span>
                <Button Size="ButtonSize.Sm"
                        Disabled="@(SelectedFeatureCount == 0)"
                        OnClick="HandleInstall">
                    Install
                </Button>
            </div>

            @code {
                private record FeatureNode(string Id, string Name, string Icon,
                                           long SizeMb, List<FeatureNode>? Children = null);
                private HashSet<string> _installFeatures = new();

                private IEnumerable<FeatureNode> SelectedLeaves =>
                    FlattenFeatures(_featureTree)
                        .Where(n => n.Children is null or { Count: 0 }
                                    && _installFeatures.Contains(n.Id));

                private long SelectedSizeMb       => SelectedLeaves.Sum(n => n.SizeMb);
                private int  SelectedFeatureCount => SelectedLeaves.Count();
            }
            """;

        public const string LoadingError =
            """
            <TreeView TItem="LazyNode"
                      Items="@_lazyRoots"
                      ChildrenProperty="@(n => _childrenDict.TryGetValue(n.Id, out var c) ? c : n.Children)"
                      HasChildrenField="@(n => n.HasChildren)"
                      ValueField="@(n => n.Id)"
                      TextField="@(n => n.Label)"
                      IconField="@(n => n.HasChildren ? "folder" : "file")"
                      LoadingNodes="@_loadingNodes"
                      ErrorNodes="@_errorNodes"
                      OnNodeExpand="@HandleNodeExpand"
                      OnRetryLoad="@HandleRetryLoad" />

            @code {
                private record LazyNode(string Id, string Label, bool HasChildren, List<LazyNode>? Children = null);
                private readonly Dictionary<string, List<LazyNode>> _childrenDict = new();
                private readonly HashSet<string> _loadingNodes = new();
                private readonly HashSet<string> _errorNodes   = new();

                private async Task HandleNodeExpand(string nodeId)
                {
                    if (_childrenDict.ContainsKey(nodeId)
                        || _loadingNodes.Contains(nodeId)
                        || _errorNodes.Contains(nodeId)) return;

                    _loadingNodes.Add(nodeId);
                    StateHasChanged();
                    await Task.Delay(1500);
                    _loadingNodes.Remove(nodeId);
                    _errorNodes.Add(nodeId); // simulate error
                    StateHasChanged();
                }

                private async Task HandleRetryLoad(string nodeId)
                {
                    _errorNodes.Remove(nodeId);
                    _loadingNodes.Add(nodeId);
                    StateHasChanged();
                    await Task.Delay(1200);
                    _loadingNodes.Remove(nodeId);
                    _childrenDict[nodeId] = GetChildrenFor(nodeId);
                    StateHasChanged();
                }
            }
            """;

        public const string LoadChildrenAsync =
            """
            <TreeView TItem="FolderNode"
                      Items="@_roots"
                      HasChildrenField="@(n => n.IsFolder)"
                      ValueField="@(n => n.Id)"
                      TextField="@(n => n.Name)"
                      IconField="@(n => n.IsFolder ? "folder" : "file")"
                      LoadChildrenAsync="@FetchChildren" />

            @code {
                private record FolderNode(string Id, string Name, bool IsFolder);

                private async Task<IEnumerable<FolderNode>> FetchChildren(FolderNode node)
                {
                    var result = await MyApi.GetChildren(node.Id);
                    return result.Select(r => new FolderNode(r.Id, r.Name, r.IsFolder));
                }
            }
            """;
    }
}
