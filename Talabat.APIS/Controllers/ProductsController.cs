using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIS.Dtos;
using Talabat.APIS.Errors;
using Talabat.APIS.Helpers;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications;
using Talabat.Core.Specifications.Products_Specs;

namespace Talabat.APIS.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IGenericRepository<ProductBrand> _brandsRepo;
        private readonly IGenericRepository<ProductCategory> _categoriesRepo;
		private readonly IProductService _productService;
		private readonly IMapper _mapper;

        public ProductsController(
            IGenericRepository<Product> productsRepo,
            IGenericRepository<ProductBrand> brandsRepo,
            IGenericRepository<ProductCategory> categoriesRepo,
			IProductService productService,
			IMapper mapper)
        {
            _productsRepo = productsRepo;
            _brandsRepo = brandsRepo;
            _categoriesRepo = categoriesRepo;
			_productService = productService;
			_mapper = mapper;
        }

        // /api/Products
        [HttpGet]
        //[Authorize]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery] ProductSpecParams specParams)
        {
			var products = await _productService.GetProductsAsync(specParams);

			var count = await _productService.GetCountAsync(specParams);

			var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            return Ok(new Pagination<ProductToReturnDto>(specParams.PageIndex, specParams.PageSize, count, data));
        }

        [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        // /api/Products/1
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
			var product = await _productService.GetProductAsync(id);

			if (product is null)
                return NotFound(new ApiResponse(404)); //404

            return Ok(_mapper.Map<Product, ProductToReturnDto>(product)); //200
        }

        [HttpGet("brands")] // Get: /api/products/brands
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
			var brands = await _productService.GetBrandsAsync();
			return Ok(brands);
        }

        [HttpGet("categories")] // Get: /api/products/categories
        public async Task<ActionResult<IReadOnlyList<ProductCategory>>> GetCategories()
        {
			var categories = await _productService.GetCategoriesAsync();
			return Ok(categories);
        }
    }
}
