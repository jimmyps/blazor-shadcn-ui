namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class PageTransitionDemo
    {
        private const string _basicCode =
            """
            @* MainLayout.razor *@
            <RenderStateProvider>
                <PageTransition>
                    @Body
                </PageTransition>
            </RenderStateProvider>
            """;

        private const string _slideCode =
            """
            <PageTransition EnableSlide="true" SlideDistance="16" Duration="0.3">
                @Body
            </PageTransition>
            """;
    }
}
