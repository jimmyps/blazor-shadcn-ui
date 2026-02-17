namespace BlazorBlueprint.Tests.ApiSurface;

public class PrimitivesApiSurfaceTests
{
    [Fact]
    public Task PrimitivesApiSurfaceMatchesBaseline()
    {
        var assembly = typeof(BlazorBlueprint.Primitives.Checkbox.BbCheckbox).Assembly;
        var apiSurface = ApiSurfaceGenerator.Generate(assembly);
        return Verify(apiSurface);
    }
}
