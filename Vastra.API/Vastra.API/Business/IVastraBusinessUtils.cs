namespace Vastra.API.Business
{
    public interface IVastraBusinessUtils
    {
        public Task<bool> IsFirstAdmin(int userId);
        public Task<bool> CategoryContainsCategoriesOrProducts(int categoryId);
    }
}
