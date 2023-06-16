using Vastra.API.Entities;

namespace Vastra.API.Services
{
    public interface IVastraRepository
    {
        #region User

        Task<(IEnumerable<User>, PaginationMetadata)> GetUsersAsync(string? name, string? searchQuery, int pageNumber, int pageSize);
        Task<User?> GetUserByPhoneNumberAsync(string phoneNumber, bool includeAddresses = false, bool includeOrders = false);
        Task<User?> GetUserAsync(int userId, bool includeAddresses = false, bool includeOrders = false);
        Task<IEnumerable<Address>?> GetAddressesForUserAsync(int userId);
        Task<Address?> GetAddressForUserAsync(int userId, int addressId);
        Task<IEnumerable<Order>?> GetOrdersForUserAsync(int userId);
        Task<Order?> GetOrderForUserAsync(int userId, int orderId);
        Task AddOrderForUserAsync(int userId, Order order);
        Task AddAddressForUserAsync(int userId, Address address);
        Task AddUser(User user);
        Task UpdateUser(User user);
        void DeleteUser(User user);

        #endregion User

        #region Role

        Task<Role?> GetRoleAsync(int roleId);
        Task AddRoleAsync(Role role);
        void DeleteRole(Role role);

        #endregion Role

        #region Product

        Task<(IEnumerable<Product>, PaginationMetadata)> GetProductsAsync(string? name, string? searchQuery, int pageNumber, int pageSize);
        Task<Product?> GetProductAsync(int productId);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        void DeleteProduct(Product product);

        #endregion Product

        #region Order

        Task<(IEnumerable<Order>, PaginationMetadata)> GetOrdersAsync(int pageNumber, int pageSize);
        Task<Order?> GetOrderAsync(int orderId, bool includeCartItems = false);
        Task<IEnumerable<CartItem>?> GetCartItemsForOrderAsync(int orderId);
        Task<CartItem?> GetCartItemForOrderAsync(int orderId, int cartItemId);
        Task AddCartItemForOrderAsync(int orderId, CartItem cartItem);
        Task AddOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        void DeleteOrder(Order order);



        #endregion Order

        #region Category

        Task<(IEnumerable<Category>, PaginationMetadata)> GetCategoriesAsync(int pageNumber, int pageSize);
        Task<Category?> GetCategoryAsync(int categoryId, bool includeChildCategories = false);
        Task<IEnumerable<Category>?> GetChildCategoriesForCategoryAsync(int categoryId);
        Task<Category?> GetChildCategoryForCategoryAsync(int categoryId, int childCategoryId);
        Task AddChildCategoryForCategoryAsync(int categoryId, Category category);
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        void DeleteCategory(Category category);


        #endregion Category

        #region CartItem

        Task<IEnumerable<CartItem>> UpdateCartItemForOrderAsync(int orderId, int cartItemId, CartItem cartItem);
        void DeleteCartItem(CartItem cartItem);


        #endregion CartItem

        #region Address

        Task UpdateAddressForUserAsync(int userId, int addressId, Address address);
        void DeleteAddress(Address address);

        #endregion Address

        #region common

        Task<bool> SaveChangesAsync();

        #endregion common


    }
}
