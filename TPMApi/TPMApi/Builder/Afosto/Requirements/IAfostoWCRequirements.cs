using WooCommerceNET.WooCommerce.v3;

namespace TPMApi.Builder.Afosto.Requirements
{
    public interface IAfostoWCRequirements
    {
        Product Product { get; }
        WCObject WCObject { get; }
    }
}