namespace Vastra.API.Models.CustomException
{
    public class ItemWithNameAlreadyExistsException : ItemAlreadyExistsException
    {
        public ItemWithNameAlreadyExistsException(string message): base(message) { }
    }
}
