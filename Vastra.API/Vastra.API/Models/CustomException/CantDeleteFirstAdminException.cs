namespace Vastra.API.Models.CustomException
{
    public class CantDeleteFirstAdminException : Exception
    {
        public CantDeleteFirstAdminException(string message) : base(message)
        {
            
        }
    }
}
