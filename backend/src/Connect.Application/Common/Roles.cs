namespace Connect.Application.Common;

/// <summary>Application roles used for authorization.</summary>
public static class Roles
{
    /// <summary>A business customer's buyer/admin — uses the webshop.</summary>
    public const string Buyer = "Buyer";

    /// <summary>Arla staff — manages the catalogue, orders and claims (back-office).</summary>
    public const string PlatformAdmin = "PlatformAdmin";
}
