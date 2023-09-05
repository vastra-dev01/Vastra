namespace Vastra.API.Models.CustomException
{
    public class ProductWithSKUNumberAlreadyExistsException : ItemAlreadyExistsException
    {
        public ProductWithSKUNumberAlreadyExistsException(string message) : base(message)
        {
            
        }
    }
}
