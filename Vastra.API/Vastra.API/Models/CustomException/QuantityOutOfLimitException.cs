namespace Vastra.API.Models.CustomException
{
    public class QuantityOutOfLimitException : Exception
    {
        public QuantityOutOfLimitException(string message) : base(message) { }
        
    }
}
