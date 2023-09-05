namespace Vastra.API.Models.CustomException
{
    public class ItemAlreadyExistsException : Exception
    {
        public ItemAlreadyExistsException(string message) : base(message)
        {
            
        }
    }
}
