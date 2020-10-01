using WooCommerceNET.WooCommerce.v3;

namespace TPMApi.Interfaces
{
    public class WCRequirements : IWCRequirements
    {
        public Product Product { get; }
        public WCObject WCObject { get; }

        public WCRequirements(Product product, WCObject wcObject)
        {
            Product = product;
            WCObject = wcObject;
        }
    }
}
