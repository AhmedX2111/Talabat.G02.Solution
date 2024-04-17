using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications;
using Talabat.Core.Specifications.Products_Specs;

namespace Talabat.APIS.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericInfrastructure<Product> _productsRepo;

        public ProductsController(IGenericInfrastructure<Product> productsRepo)
        {
            _productsRepo = productsRepo;
        }

        // /api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var spec = new ProductWithBrandAndCategorySpecifications();

            var products = await _productsRepo.GetAllWithSpecAsync(spec);

            return Ok(products);
        }

        // /api/Products/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(id);

            var product = await _productsRepo.GetWithSpecAsync(spec);

            if (product is null)
                return NotFound(new { Message = "Not Found", StatusCode = 404 }); //404

            return Ok(product); //200
        }
    }
}
