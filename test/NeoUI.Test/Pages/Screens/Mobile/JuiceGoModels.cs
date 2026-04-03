namespace NeoUI.Test.Pages.Screens.Mobile;

public enum Screen { Login, Home, Menu, Product, Cart, Profile, Orders, Favourites, Help }

public record Product(
    string   Id,
    string   Name,
    string   Tagline,
    string   Emoji,
    string   Color,
    decimal  Price,
    float    Rating,
    int      Reviews,
    bool     IsNew,
    bool     IsPopular,
    string   Category,
    string[] Sizes);

public record CartItem(Product Product, string Size, int Quantity);

public record ProfileRow(string Icon, string Label, Screen? NavScreen = null, bool IsSeparator = false);

public record SampleOrder(string Id, string[] Items, decimal Total, string Date, string Status);

public record Faq(string Q, string A);

public static class JuiceGoRoutes
{
    public const string Base = "/screens/mobile/juicego";

    public static string Get(Screen screen, string? id = null) => screen switch
    {
        Screen.Home       => Base + "/home",
        Screen.Menu       => Base + "/menu",
        Screen.Product    => Base + "/menu/" + id,
        Screen.Cart       => Base + "/cart",
        Screen.Profile    => Base + "/profile",
        Screen.Orders     => Base + "/profile/orders",
        Screen.Favourites => Base + "/profile/favourites",
        Screen.Help       => Base + "/profile/help",
        _                 => Base,
    };
}
