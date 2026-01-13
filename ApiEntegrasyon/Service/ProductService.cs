using ApiEntegrasyon.Entity;
using ApiEntegrasyon.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiEntegrasyon.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Product>> GetAll()
            => _repo.GetAll();
        public Task<IEnumerable<Product>> GetFakeAndAll()
            => _repo.GetFakeAndAll();
        public Task<Product?> GetById(int id)
            => _repo.GetById(id);

        public Task Create(Product product)
        {
            product.CreatedAt = DateTime.UtcNow;
            product.IsActive = true;
            product.IsDeleted = false;
            return _repo.Create(product);
        }

        public Task Update(Product product)
            => _repo.Update(product);

        public Task Delete(int id)
            => _repo.Delete(id);
    }
}
