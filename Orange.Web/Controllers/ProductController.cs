using Microsoft.AspNetCore.Mvc;
using Orange.Models.DTO;
using Orange.Web.Services.Interfaces;

namespace Orange.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDTO> products = await _productService.GetAllAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDTO product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            var productResponse = await _productService.CreateProductAsync(product);
            if (productResponse == null)
            {
                TempData["error"] = "Error on creating product";
                return View(product);
            }

            TempData["success"] = "Product created successfully";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                TempData["error"] = "Product not found";
                return RedirectToAction("Index");
            }
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductDTO product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            var productResponse = await _productService.UpdateProductAsync(product);
            if (productResponse == null)
            {
                TempData["error"] = "Error on editing product";
                return View(product);
            }

            TempData["success"] = "Product edited successfully";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteProductAsync(id);

            if (deleted)
            {
                TempData["success"] = "Product deleted successfully";
            }
            else
            {
                TempData["error"] = "Error on deleting product";
            }

            return RedirectToAction("Index");
        }
    }
}
