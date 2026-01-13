using ApiEntegrasyon.Context;
using ApiEntegrasyon.Dtos;
using ApiEntegrasyon.Entity;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data;
using System.Net.Http;
using System.Globalization;
namespace ApiEntegrasyon.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly DapperContext _context;
        private readonly HttpClient _httpClient;

        public ProductRepository(DapperContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            using var db = _context.CreateConnection();
            return await db.QueryAsync<Product>(
                "sp_product_get_all",
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Product>> GetFakeAndAll()
        {
            var fakeProducts = await _httpClient.GetFromJsonAsync<List<FakeStoreProductDto>>(
                "https://fakestoreapi.com/products"
            );

            using var db = _context.CreateConnection();
            var dbProducts = await db.QueryAsync<Product>(
                "sp_product_get_all",
                commandType: CommandType.StoredProcedure
            );
            
            List<Product> products = fakeProducts.Select(x => new Product
            {
                Id = x.Id,
                Name = x.Title,          
                Category = x.Category,
                Price = x.Price,
                IsActive = true,
                IsDeleted = false,
                CreatedBy = "fake system",
                CreatedAt = DateTime.Now
            }).ToList();


            dbProducts ??= Enumerable.Empty<Product>();

            var idSet = dbProducts.Select(x => x.Id).ToHashSet();

            var fakeProductList = products
                .Where(p => !idSet.Contains(p.Id))
                .ToList();

            return dbProducts
                .Concat(fakeProductList)
                .ToList();

        }


        public async Task<Product?> GetById(int id)
        {
            using var db = _context.CreateConnection();
            return await db.QueryFirstOrDefaultAsync<Product>(
                "sp_product_get_by_id",
                new { Id = id },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task Create(Product product)
        {
            using var db = _context.CreateConnection();
            await db.ExecuteAsync(
                "sp_product_insert",
                new
                {
                    product.Name,
                    product.Category,
                    product.Price,
                    product.IsActive,
                    product.IsDeleted,
                    product.CreatedBy,
                    product.CreatedAt
                },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task Update(Product product)
        {
            using var db = _context.CreateConnection();
            await db.ExecuteAsync(
                "sp_product_update",
                new
                {
                    product.Id,
                    product.Name,
                    product.Category,
                    product.Price,
                    product.IsActive,
                    product.IsDeleted,
                    product.CreatedBy,
                    product.CreatedAt
                },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task Delete(int id)
        {
            using var db = _context.CreateConnection();
            await db.ExecuteAsync(
                "sp_product_delete",
                new { Id = id },
                commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
