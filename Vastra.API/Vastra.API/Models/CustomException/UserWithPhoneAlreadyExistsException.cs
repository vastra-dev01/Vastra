namespace Vastra.API.Models.CustomException
{
    public class UserWithPhoneAlreadyExistsException : ItemAlreadyExistsException
    {
        public UserWithPhoneAlreadyExistsException(string message) : base(message)
        {
            
        }
    }
}
