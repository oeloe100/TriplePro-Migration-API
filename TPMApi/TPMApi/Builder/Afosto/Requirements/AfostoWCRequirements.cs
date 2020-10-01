using WooCommerceNET.WooCommerce.v3;

namespace TPMApi.Builder.Afosto.Requirements
{
    public class AfostoWCRequirements : IAfostoWCRequirements
    {
        public Product Product { get; }
        public WCObject WCObject { get; }

        public AfostoWCRequirements(Product product, WCObject wcObject)
        {
            Product = product;
            WCObject = wcObject;
        }
    }
}
