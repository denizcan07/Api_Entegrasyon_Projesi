using ApiEntegrasyon.Entity;
using ApiEntegrasyon.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiEntegrasyon.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductManagerController : ControllerBase
    {
    
        private readonly IProductService _service;

        public ProductManagerController(IProductService service)
        {
            _service = service;
        }


        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Product>> GetAll()
            => await _service.GetAll();

        [HttpGet("{id}")]
        [Authorize]
        public async Task<Product?> GetById(int id)
            => await _service.GetById(id);

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            await _service.Create(product);
            return Ok(product);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] Product product)
        {
            await _service.Update(product);
            return Ok(product);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id);
            return Ok();
        }


        [HttpGet("fake")]
        [Authorize]
        public async Task<IEnumerable<Product>> GetFakeAndAll()
            => await _service.GetFakeAndAll();
    }
}
