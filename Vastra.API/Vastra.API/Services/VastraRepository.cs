using Microsoft.EntityFrameworkCore;
using Vastra.API.DBContexts;
using Vastra.API.Entities;

namespace Vastra.API.Services
{
    public class VastraRepository : IVastraRepository
    {
        private readonly VastraContext _context;

        public VastraRepository(VastraContext context)
        {
            _context = context;
        }
        public async Task AddAddressForUserAsync(int userId, Address address)
        {
            var user = await GetUserAsync(userId);
            if (user != null)
            {
                user.Addresses.Add(address);
            }
        }

        public async Task AddCartItemForOrderAsync(int orderId, CartItem cartItem)
        {
            var order = await GetOrderAsync(orderId);
            if (order != null) { 
                order.CartItems.Add(cartItem);
            }
        }

        public async Task AddCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
        }

        public async Task AddChildCategoryForCategoryAsync(int categoryId, Category category)
        {
            var parentCategory = await GetCategoryAsync(categoryId);
            if (parentCategory != null)
            {
                parentCategory.ChildCategories.Add(category);
            }

        }

        public async Task AddOrderForUserAsync(int userId, Order order)
        {
            var user = await GetUserAsync(userId);
            if (user != null)
            {
                user.Orders.Add(order);
            }
        }

        public async Task AddProductForCategoryAsync(int categoryId, Product product)
        {
            var category = await GetCategoryAsync(categoryId);
            if(category != null)
            {
                category.Products.Add(product);
            }
        }

        public async Task<bool> AddressExistsAsync(int addressId)
        {
            return await _context.Addresses.AnyAsync(a => a.AddressId == addressId);
        }

        public async Task AddRoleAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
        }

        public async Task AddUserForRole(int roleId, User user)
        {
            var role  = await GetRoleAsync(roleId);
            if(role != null)
            {
                role.Users.Add(user);
            }
        }

        public async Task<bool> CartItemExistsAsync(int CartItemId)
        {
            return await _context.CartItems.AnyAsync(c => c.CartItemId == CartItemId);
        }

        public async Task<bool> CategoryExistsAsync(int categoryId)
        {
            return await _context.Categories.AnyAsync(c => c.CategoryId == categoryId);
        }

        public void DeleteAddress(Address address)
        {
            _context.Addresses.Remove(address);
        }

        public void DeleteCartItem(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
        }

        public void DeleteCategory(Category category)
        {
            _context.Categories.Remove(category);
        }

        public void DeleteOrder(Order order)
        {
            _context.Orders.Remove(order);
        }

        public void DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
        }

        public void DeleteRole(Role role)
        {
            _context.Roles.Remove(role);
        }

        public void DeleteUser(User user)
        {
            _context.Users.Remove(user);
        }

        public async Task<IEnumerable<Address>?> GetAddressesForUserAsync(int userId)
        {
            return await _context.Addresses.Where(a => a.UserId == userId).ToListAsync();
        }

        public async Task<Address?> GetAddressForUserAsync(int userId, int addressId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId && a.AddressId == addressId)
                .FirstOrDefaultAsync();
        }

        public async Task<CartItem?> GetCartItemForOrderAsync(int orderId, int cartItemId)
        {
            return await _context.CartItems
                .Where(c => c.OrderId == orderId && c.CartItemId == cartItemId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CartItem>?> GetCartItemsForOrderAsync(int orderId)
        {
            return await _context.CartItems.Where(c => c.OrderId == orderId).ToListAsync();
        }

        public async Task<(IEnumerable<Category>, PaginationMetadata)> GetCategoriesAsync(int pageNumber, int pageSize)
        {
            var collection = _context.Categories as IQueryable<Category>;
            var totalItemCount = await collection.CountAsync();
            var paginationMetadata = new PaginationMetadata(totalItemCount, pageNumber, pageSize);

            var collectionToReturn = await collection.OrderBy(c => c.CategoryName)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetadata);
        }

        public async Task<Category?> GetCategoryAsync(int categoryId, bool includeChildCategories = false, bool includeProducts = false)
        {

            if (includeChildCategories && includeProducts)
            {
                return await _context.Categories.Include(c => c.ChildCategories).Include(c => c.Products)
                    .Where(c => c.CategoryId == categoryId).FirstOrDefaultAsync();
            }
            else if(includeProducts) {
                return await _context.Categories.Include(c => c.Products)
                    .Where(c => c.CategoryId == categoryId).FirstOrDefaultAsync();
            }
            else if (includeChildCategories)
            {
                var t = await _context.Categories.Include(c => c.ChildCategories)
                    .Where(c => c.CategoryId == categoryId).FirstOrDefaultAsync();
                return t;
            }
            return await _context.Categories.Where(c => c.CategoryId == categoryId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Category>?> GetChildCategoriesForCategoryAsync(int categoryId)
        {
            return await _context.Categories.Where(c => c.CategoryId == categoryId).Select(c => c.ChildCategories).FirstOrDefaultAsync();
        }

        public async Task<Category?> GetChildCategoryForCategoryAsync(int categoryId, int childCategoryId)
        {
            var childCategories = _context.Categories
                .Where(c => c.CategoryId == categoryId)
                .Select(c => c.ChildCategories) as IQueryable<Category>;
            if(childCategories != null)
            return await childCategories.Where(c => c.CategoryId == childCategoryId).FirstOrDefaultAsync();

            return null;
        }

        public async Task<Order?> GetOrderAsync(int orderId, bool includeCartItems = false)
        {
            if (includeCartItems)
            {
                return await _context.Orders.Include(o => o.CartItems).Where(o => o.OrderId == orderId).FirstOrDefaultAsync(); 
            }
            return await _context.Orders.Where(o => o.OrderId == orderId).FirstOrDefaultAsync();
        }

        public async Task<Order?> GetOrderForUserAsync(int userId, int orderId)
        {
            return await _context.Orders
                .Where(o => o.OrderId == orderId && o.UserId == userId)
                .FirstOrDefaultAsync();
                 
        }

        public async Task<(IEnumerable<Order>, PaginationMetadata)> GetOrdersAsync(int pageNumber, int pageSize)
        {
            var collection = _context.Orders as IQueryable<Order>;
            var totalPageSize = await collection.CountAsync();

            var paginationMetadata = new PaginationMetadata(totalPageSize, pageSize, pageNumber);

            var collectionToReturn = await collection.OrderBy(c => c.DateModified) 
                .Skip(pageSize * ( pageNumber - 1) )
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetadata);
        }

        public async Task<IEnumerable<Order>?> GetOrdersForUserAsync(int userId)
        {
            return await _context.Orders.Where(o => o.UserId == userId).ToListAsync();
        }

        public async Task<Product?> GetProductAsync(int productId)
        {
            return await _context.Products.Where(p => p.ProductId == productId).FirstOrDefaultAsync();
        }

        public async Task<Product?> GetProductForCategoryAsync(int categoryId, int productId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId && p.ProductId == productId)
                .FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<Product>, PaginationMetadata)> GetProductsAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            var collection = _context.Products as IQueryable<Product>;
            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim();
                collection = collection.Where(p => p.Name == name);
            }
            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(c => c.Name.Contains(searchQuery)
                || (c.Description != null && c.Description.Contains(searchQuery)));
            }

            var totalPageCount = await collection.CountAsync();
            var paginationMetadata = new PaginationMetadata(totalPageCount, pageSize, pageNumber);
            var collectionToReturn = await collection.OrderBy(p => p.DateModified)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();
            return (collectionToReturn, paginationMetadata);

        }

        public async Task<(IEnumerable<Product>, PaginationMetadata)> GetProductsForCategoryAsync(int categoryId, string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            var collection = _context.Products.Where(p => p.CategoryId == categoryId) as IQueryable<Product>;
            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim();
                collection = collection.Where(p => p.Name == name);
            }
            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(c => c.Name.Contains(searchQuery)
                || (c.Description != null && c.Description.Contains(searchQuery)));
            }

            var totalPageCount = await collection.CountAsync();
            var paginationMetadata = new PaginationMetadata(totalPageCount, pageSize, pageNumber);
            var collectionToReturn = await collection.OrderByDescending(p => p.DateModified)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();
            return (collectionToReturn, paginationMetadata);
        }

        public async Task<Role?> GetRoleAsync(int roleId)
        {
            return await _context.Roles.Where(r => r.RoleId == roleId).FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserAsync(int userId, bool includeAddresses = false, bool includeOrders = false)
        {
            if(includeAddresses && includeOrders)
            {
                return await _context.Users
                        .Include(u => u.Orders)
                        .Include(u => u.Addresses)
                        .Where(u => u.UserId == userId).FirstOrDefaultAsync();
            }
            else if(includeAddresses) {
                return await _context.Users
                        .Include(u => u.Addresses)
                        .Where(u => u.UserId == userId).FirstOrDefaultAsync();
            }
            else if (includeOrders)
            {
                return await _context.Users
                        .Include(u => u.Orders)
                        .Where(u => u.UserId == userId).FirstOrDefaultAsync();
            }
            else
            {
                return await _context.Users.Where(u => u.UserId == userId).FirstOrDefaultAsync();
            }
        }

        public async Task<User?> GetUserByPhoneNumberAsync(string phoneNumber, bool includeAddresses = false, bool includeOrders = false)
        {
            if (includeAddresses && includeOrders)
            {
                return await _context.Users
                        .Include(u => u.Orders)
                        .Include(u => u.Addresses)
                        .Where(u => u.PhoneNumber.Equals(phoneNumber)).FirstOrDefaultAsync();
            }
            else if (includeAddresses)
            {
                return await _context.Users
                        .Include(u => u.Addresses)
                        .Where(u => u.PhoneNumber.Equals(phoneNumber)).FirstOrDefaultAsync();
            }
            else if (includeOrders)
            {
                return await _context.Users
                        .Include(u => u.Orders)
                        .Where(u => u.PhoneNumber.Equals(phoneNumber)).FirstOrDefaultAsync();
            }
            else
            {
                return await _context.Users.Where(u => u.PhoneNumber.Equals(phoneNumber)).FirstOrDefaultAsync();
            }
        }

        public async Task<(IEnumerable<User>, PaginationMetadata)> GetUsersByRoleAsync(int roleId, string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            var collection = _context.Users.Where(u => u.RoleId == roleId) as IQueryable<User>;
            if (!string.IsNullOrEmpty(name))
            {
                collection = collection.Where(u => u.FirstName.Equals(name) || (u.LastName != null && u.LastName.Equals(name)));
            }
            if (!string.IsNullOrEmpty(searchQuery))
            {
                collection = collection.Where(u => u.FirstName.Contains(searchQuery) 
                || (u.LastName != null && u.LastName.Contains(searchQuery))
                || (u.EmailId != null && u.EmailId.Contains(searchQuery)));
            }
            var totalPageSize = await collection.CountAsync();
            var paginationMetadata = new PaginationMetadata(totalPageSize, pageSize, pageNumber);

            var collectionToReturn = await collection.OrderBy(u => u.FirstName)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetadata);
        }

        public async Task<User?> GetUserByRoleAsync(int roleId, int userId, bool includeAddresses = false, bool includeOrders = false)
        {
            if (includeAddresses && includeOrders)
            {
                return await _context.Users
                        .Include(u => u.Orders)
                        .Include(u => u.Addresses)
                        .Where(u => u.RoleId == roleId && u.UserId == userId)
                        .FirstOrDefaultAsync();
            }
            else if (includeAddresses)
            {
                return await _context.Users
                        .Include(u => u.Addresses)
                        .Where(u => u.RoleId == roleId && u.UserId == userId)
                        .FirstOrDefaultAsync();
            }
            else if (includeOrders)
            {
                return await _context.Users
                        .Include(u => u.Orders)
                        .Where(u => u.RoleId == roleId && u.UserId == userId)
                        .FirstOrDefaultAsync();
            }
            else
            {
                return await _context.Users.Where(u => u.RoleId == roleId && u.UserId == userId)
                .FirstOrDefaultAsync();
            }
        }
        public async Task<bool> OrderExistsAsync(int orderId)
        {
            return await _context.Orders.AnyAsync(o => o.OrderId == orderId);
        }

        public async Task<bool> ProductExistsAsync(int productId)
        {
            return await _context.Products.AnyAsync(p => p.ProductId == productId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public Task UpdateAddressForUserAsync(int userId, int addressId, Address address)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CartItem>> UpdateCartItemForOrderAsync(int orderId, int cartItemId, CartItem cartItem)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCategoryAsync(Category category)
        {
            throw new NotImplementedException();
        }

        public Task UpdateOrderAsync(Order order)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUser(User user)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _context.Users.AnyAsync(u => u.UserId == userId);
        }
        public async Task<bool> RoleExistsAsync(int roleId)
        {
            return await _context.Roles.AnyAsync(r => r.RoleId == roleId);
        }

        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            return await _context.Roles.OrderBy(r => r.DateModified).ToListAsync();
        }
    }
}
