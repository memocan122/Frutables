using Frutables.Data;
using Frutables.Models;
using Frutables.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Frutables.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(AppDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var sliders = new List<Slider>();
            var products = new List<Product>();
            var categories = new List<Category>();

            try
            {
                if (_context == null)
                    throw new Exception("_context is null");

                // DB qoşulmanı yoxla
                if (!_context.Database.CanConnect())
                    throw new Exception("Database-ə qoşula bilmirik. Connection string və DB server-i yoxla.");

                // Data gətir
                sliders = _context.Sliders?.ToList() ?? new List<Slider>();
                products = _context.Products?.ToList() ?? new List<Product>();
                categories = _context.Categories?.ToList() ?? new List<Category>();
            }
            catch (Exception ex)
            {
                // Xətanı logla
                _logger.LogError(ex, "Home/Index zamanı xeta baş verdi: {msg}", ex.GetBaseException().Message);

                // Development üçün səhifədə göstərə bilərsən
                TempData["HomeLoadError"] = ex.GetBaseException().Message;
            }

            var vm = new HomeVM
            {
                Sliders = sliders,
                Products = products,
                Categories = categories
            };

            return View(vm);
        }
    }
}
