namespace Vastra.API.Models.CustomException
{
    public class ProductWithProductIdInCartItemNotFoundException : Exception
    {
        public ProductWithProductIdInCartItemNotFoundException(string message) : base(message)
        {
            
        }
    }
}
