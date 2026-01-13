using ApiEntegrasyon.Entity;

namespace ApiEntegrasyon.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAll();
        Task<IEnumerable<Product>> GetFakeAndAll();
        Task<Product?> GetById(int id);
        Task Create(Product product);
        Task Update(Product product);
        Task Delete(int id);
    }
}
