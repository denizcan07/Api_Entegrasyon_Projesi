using ApiEntegrasyon.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiEntegrasyon.Service
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAll();
        Task<IEnumerable<Product>> GetFakeAndAll();
        Task<Product?> GetById(int id);
        Task Create(Product product);
        Task Update(Product product);
        Task Delete(int id);
    }
}
