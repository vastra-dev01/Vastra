namespace Vastra.API.Models.CustomException
{
    public class CantDeleteResourceContainingChildResourcesException : Exception
    {
        public CantDeleteResourceContainingChildResourcesException(string message) : base (message)
        {
            
        }
    }
}
