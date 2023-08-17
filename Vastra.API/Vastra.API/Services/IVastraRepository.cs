using System.Security.Claims;
using Vastra.API.Entities;

namespace Vastra.API.Services
{
    public interface IVastraRepository
    {
        #region User

        Task<(IEnumerable<User>, PaginationMetadata)> GetUsersByRoleAsync(int roleId, string? name, string? searchQuery, int pageNumber, int pageSize);
        Task<User?> GetUserByRoleAsync(int roleId, int userId, bool includeAddresses = false, bool includeOrders = false); 
        Task<User?> GetUserByPhoneNumberAsync(string phoneNumber, bool includeAddresses = false, bool includeOrders = false);
        Task<User?> GetUserAsync(int userId, bool includeAddresses = false, bool includeOrders = false);
        Task<(IEnumerable<Address>, PaginationMetadata)> GetAddressesForUserAsync(int userId, int pageNumber, int pageSize);
        Task<Address?> GetAddressForUserAsync(int userId, int addressId);
        Task<(IEnumerable<Order>, PaginationMetadata)> GetOrdersForUserAsync(int userId, int pageNumber, int pageSize);
        Task<Order?> GetOrderForUserAsync(int userId, int orderId, bool includeCartItems = false);
        Task AddOrderForUserAsync(int userId, Order order);
        Task AddAddressForUserAsync(int userId, Address address);
        Task AddUserForRole(int roleId, User user);
        Task UpdateUser(User user);
        void DeleteUser(User user);
        Task<bool> UserExistsAsync(int userId);
        Task<bool> UserExistsWithRoleAsync(int roleId, int userId);
        #endregion User

        #region Role
        Task<IEnumerable<Role>> GetRolesAsync();
        Task<Role?> GetRoleAsync(int roleId);
        Task AddRoleAsync(Role role);
        void DeleteRole(Role role);
        Task<bool> RoleExistsAsync(int roleId);

        #endregion Role

        #region Product

        Task<(IEnumerable<Product>, PaginationMetadata)> GetProductsAsync(string? name, string? searchQuery, int pageNumber, int pageSize);
        Task<Product?> GetProductAsync(int productId);
        Task AddProductForCategoryAsync(int categoryId, Product product);
        Task UpdateProductAsync(Product product);
        void DeleteProduct(Product product);
        Task<bool> ProductExistsAsync(int productId);
        Task<CartItem?> ProductExistsAsACartItemForOrder(int orderId, int productId);


        #endregion Product

        #region Order

        Task<(IEnumerable<Order>, PaginationMetadata)> GetOrdersAsync(int pageNumber, int pageSize);
        Task<Order?> GetOrderAsync(int orderId, bool includeCartItems = false);
        Task<(IEnumerable<CartItem>, PaginationMetadata)> GetCartItemsForOrderAsync(int orderId, int pageNumber, int pageSize);
        Task<CartItem?> GetCartItemForOrderAsync(int orderId, int cartItemId, bool includeProduct = false);
        Task AddCartItemForOrderAsync(int orderId, CartItem cartItem);
        Task UpdateOrderAsync(Order order);
        void DeleteOrder(Order order);
        Task<bool> OrderExistsAsync(int orderId);
        Task<bool> OrderExistsForUser(int userId, int orderId);
        void UpdateAmountForOrder(int orderId);


        #endregion Order

        #region Category

        Task<(IEnumerable<Category>, PaginationMetadata)> GetCategoriesAsync(int pageNumber, int pageSize);
        Task<Category?> GetCategoryAsync(int categoryId, bool includeChildCategories = false, bool includeProducts = false);
        Task<IEnumerable<Category>?> GetChildCategoriesForCategoryAsync(int categoryId);
        Task<Category?> GetChildCategoryForCategoryAsync(int categoryId, int childCategoryId);
        Task<Product?> GetProductForCategoryAsync(int categoryId, int productId);
        Task<(IEnumerable<Product>, PaginationMetadata)> GetProductsForCategoryAsync(int categoryId, string? name, string? searchQuery, int pageNumber, int pageSize);
        Task AddChildCategoryForCategoryAsync(int categoryId, Category category);
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        void DeleteCategory(Category category);
        Task<bool> CategoryExistsAsync(int categoryId);

        #endregion Category

        #region CartItem

        Task<IEnumerable<CartItem>> UpdateCartItemForOrderAsync(int orderId, int cartItemId, CartItem cartItem);
        void DeleteCartItem(CartItem cartItem);
        Task<bool> CartItemExistsAsync(int CartItemId);


        #endregion CartItem

        #region Address

        Task UpdateAddressForUserAsync(int userId, int addressId, Address address);
        void DeleteAddress(Address address);
        Task<bool> AddressExistsAsync(int addressId);

        #endregion Address

        #region authentication
        Task<User?> ValidateUserCredentials(string phone, string password);
        Task<bool> ValidateUserClaim(ClaimsPrincipal User, int userId);
        #endregion authentication

        #region common

        Task<bool> SaveChangesAsync();

        #endregion common


    }
}
