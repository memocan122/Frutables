using Frutables.Data;
using Frutables.Models;
using Frutables.ViewModel.basketVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Frutables.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;

        public BasketController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var basketProduct = HttpContext.Request.Cookies["basket"];

            List<BasketDetailVM> basketDetails = new();

            if (basketProduct != null)
            {
                List<BasketVM> basket = JsonConvert.DeserializeObject<List<BasketVM>>(basketProduct);

                foreach (var item in basket)
                {
                    Product product = await _context.Products.Include(m => m.ProductImages).Include(m => m.Category).FirstOrDefaultAsync(m => m.Id == item.Id
                    && !m.IsDeleted);

                    basketDetails.Add(new BasketDetailVM
                    {
                        Id = item.Id,
                        Count = item.Count,
                        Image = product.ProductImages.FirstOrDefault(m => m.IsMain)?.Image,
                        Name = product.Name,
                        Category = product.Category.Name,
                        Price = (double)product.Price,
                       
                    });
                }
            }

            return View(basketDetails);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int? id)
        {
            if (id is null) return BadRequest();

            var isExist = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (isExist == null) return NotFound();

            List<BasketVM> basket;

            if (HttpContext.Request.Cookies["basket"] != null)
            {
                basket = JsonConvert.DeserializeObject<List<BasketVM>>(HttpContext.Request.Cookies["basket"]);
            }
            else
            {
                basket = new();
            }

            var isProductExist = basket.FirstOrDefault(m => m.Id == id);

            if (isProductExist == null)
            {
                basket.Add(new BasketVM()
                {
                    Id = (int)id,
                    Count = 1,
                    Price = (double)isExist.Price
                });
            }
            else
            {
                isProductExist.Count++;
            }

            HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));

            return RedirectToAction("Index", "Home");
        }
    }
}
