namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class ScreenTransitionDemo
    {
        private const string _demoCode =
            """
            @* Tabs drives the tab bar state; OnValueChange wires it to ScreenTransition.
               No TabsContent needed — ScreenTransition owns the content area. *@

            <Tabs Value="@_activeTab" OnValueChange="@OnTabChanged">
                <TabsList Class="w-full h-14 ...">
                    <TabsTrigger Value="home">    <LucideIcon Name="home"    /> Home    </TabsTrigger>
                    <TabsTrigger Value="explore"> <LucideIcon Name="compass" /> Explore </TabsTrigger>
                    <TabsTrigger Value="profile"> <LucideIcon Name="user"    /> Profile </TabsTrigger>
                </TabsList>
            </Tabs>

            @* Content area — animated by ScreenTransition *@
            <ScreenTransition Direction="@_direction" Key="@_screen" Class="...">
                @if (_screen == Screen.Home)         { <HomeScreen /> }
                else if (_screen == Screen.Explore)  { <ExploreScreen /> }
                else if (_screen == Screen.Profile)  { <ProfileScreen OnPushSettings="PushSettings" /> }
                else                                 { <SettingsScreen /> }
            </ScreenTransition>

            @code {
                private string _activeTab = "home";
                private Screen _screen = Screen.Home;
                private ScreenTransitionDirection _direction = ScreenTransitionDirection.None;
                private Screen _backTarget;

                // Tabs fires this — set Tab direction, sync screen
                private void OnTabChanged(string? value)
                {
                    _direction  = ScreenTransitionDirection.Tab;
                    _activeTab  = value ?? "home";
                    _screen     = value switch { "explore" => Screen.Explore, "profile" => Screen.Profile, _ => Screen.Home };
                }

                // Push — slide new screen in from right, remember where to go back
                private void PushSettings()
                {
                    _backTarget = _screen;
                    _direction  = ScreenTransitionDirection.Push;
                    _screen     = Screen.Settings;
                }

                // Pop — slide back to previous tab screen
                private void GoBack()
                {
                    _direction = ScreenTransitionDirection.Pop;
                    _screen    = _backTarget;
                    _activeTab = _backTarget switch { Screen.Explore => "explore", Screen.Profile => "profile", _ => "home" };
                }
            }
            """;

        private const string _shellCode =
            """
            // In the parent shell component:
            private ScreenTransitionDirection _dir;
            private Screen _current;

            private void NavigateTo(Screen screen)
            {
                _dir = screen is Screen.Home or Screen.Menu
                    ? ScreenTransitionDirection.Tab
                    : ScreenTransitionDirection.Push;
                _current = screen;    // Key changes → animation fires
            }

            private void GoBack()
            {
                _dir = ScreenTransitionDirection.Pop;
                _current = _previousScreen;
            }
            """;

        private const string _autoDetectCode =
            """
            // When sub-components call Nav.NavigateTo() directly,
            // detect direction in OnLocationChanged:

            private string _fromPath = string.Empty;
            private string _backTarget = string.Empty;
            private bool _isGoingBack = false;

            protected override void OnInitialized()
            {
                _fromPath = Nav.ToBaseRelativePath(Nav.Uri).ToLower().TrimEnd('/');
                Nav.LocationChanged += OnLocationChanged;
            }

            private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
            {
                var newPath = Nav.ToBaseRelativePath(e.Location).ToLower().TrimEnd('/');

                if (_isGoingBack)
                {
                    _transitionDirection = ScreenTransitionDirection.Pop;
                    _isGoingBack = false;
                }
                else if (IsRootTab(newPath))
                {
                    _transitionDirection = ScreenTransitionDirection.Tab;
                    _backTarget = string.Empty;
                }
                else
                {
                    var fromDepth = _fromPath.Split('/').Length;
                    var newDepth  = newPath.Split('/').Length;
                    if (newDepth > fromDepth) { _transitionDirection = ScreenTransitionDirection.Push; _backTarget = _fromPath; }
                    else if (newDepth < fromDepth) { _transitionDirection = ScreenTransitionDirection.Pop; }
                    else { _transitionDirection = ScreenTransitionDirection.Tab; }
                }

                _fromPath = newPath;
                InvokeAsync(StateHasChanged);
            }

            private void GoBack()
            {
                if (!string.IsNullOrEmpty(_backTarget))
                {
                    _isGoingBack = true;
                    Nav.NavigateTo(_backTarget);
                }
            }
            """;
    }
}
