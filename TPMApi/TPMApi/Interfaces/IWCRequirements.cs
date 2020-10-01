using WooCommerceNET.WooCommerce.v3;

namespace TPMApi.Interfaces
{
    public interface IWCRequirements
    {
        Product Product { get; }
        WCObject WCObject { get; }
    }
}