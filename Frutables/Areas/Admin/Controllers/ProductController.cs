using Frutables.Areas.Admin.ViewModels.ProductVMs;
using Frutables.Data;
using Frutables.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Frutables.Helpers;

namespace Frutables.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<GetAllProductVM> getAllProductVMs = await _context.Products
                .Where(p => !p.IsDeleted)
                .Include(p => p.ProductImages)
                .Include(p => p.Category)
                .Select(p => new GetAllProductVM
                {
                    Id = p.Id,
                    MainImage = p.ProductImages.FirstOrDefault(m => m.IsMain).Image,
                    Name = p.Name,
                    Price = (decimal)p.Price,
                    CategoryName = p.Category.Name
                }).OrderByDescending(m => m.Id).ToListAsync();

            return View(getAllProductVMs);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await GetAllCategories();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await GetAllCategories();
                return View();
            }

            if (!request.MainImage.CheckFileType("image/"))
            {
                ViewBag.Categories = await GetAllCategories();

                ModelState.AddModelError("MainImage", "Shekil tipinde olmalidir!");
                return View();
            }

            if (request.MainImage.CheckFileSize(2000))
            {
                ViewBag.Categories = await GetAllCategories();

                ModelState.AddModelError("MainImage", "Shekil max 2mb olmalidir!");
                return View();
            }



            bool isExist = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId && !c.IsDeleted);

            if (!isExist)
            {
                ViewBag.Categories = await GetAllCategories();
                ModelState.AddModelError("CategoryId", "Category tapilmadi!");
                return View();
            }

            List<ProductImage> productImages = new();

            string mainFileName = request.MainImage.GenerateFileName();
            string mainImagePath = _env.WebRootPath.GetFilePath("img", mainFileName);
            request.MainImage.SaveFile(mainImagePath);

            ProductImage newMainImage = new()
            {
                Image = mainFileName,
                IsMain = true
            };

            productImages.Add(newMainImage);



            Product newProduct = new()
            {
                Name = request.Name,
                Price = (double)request.Price,
                CategoryId = request.CategoryId,
                ProductImages = productImages
            };

            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Product? product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == id);

            if (product == null) return NotFound();

            ViewBag.Categories = await GetAllCategories();

            UpdateProductVM updateProductVM = new()
            {
                Name = product.Name,
                Price = (decimal)product.Price,
                CategoryId = product.CategoryId.Value,
                MainPhoto = product.ProductImages.FirstOrDefault(pi => pi.IsMain)?.Image,
                AdditionalPhotos = product.ProductImages.Where(pi => !pi.IsMain).Select(pi => pi.Image).ToList()
            };

            return View(updateProductVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateProductVM request)
        {
            if (id == null) return BadRequest();

            Product? product = await _context.Products
    .Include(p => p.ProductImages)
    .FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == id);

            if (product == null) return NotFound();

            bool isExist = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId && !c.IsDeleted);

            if (!isExist)
            {
                ViewBag.Categories = await GetAllCategories();

                request.MainPhoto = product.ProductImages.FirstOrDefault(pi => pi.IsMain)?.Image;
                request.AdditionalPhotos = product.ProductImages.Where(pi => !pi.IsMain).Select(pi => pi.Image).ToList();

                ModelState.AddModelError("CategoryId", "Category tapilmadi!");
                return View(request);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await GetAllCategories();

                request.MainPhoto = product.ProductImages.FirstOrDefault(pi => pi.IsMain)?.Image;
                request.AdditionalPhotos = product.ProductImages.Where(pi => !pi.IsMain).Select(pi => pi.Image).ToList();

                return View(request);
            }

            if (request.MainImage != null)
            {
                if (!request.MainImage.CheckFileType("image/"))
                {
                    ViewBag.Categories = await GetAllCategories();

                    request.MainPhoto = product.ProductImages.FirstOrDefault(pi => pi.IsMain)?.Image;
                    request.AdditionalPhotos = product.ProductImages.Where(pi => !pi.IsMain).Select(pi => pi.Image).ToList();

                    ModelState.AddModelError("MainImage", "Shekil tipinde olmalidir!");
                    return View(request);
                }

                if (request.MainImage.CheckFileSize(2000))
                {
                    ViewBag.Categories = await GetAllCategories();

                    request.MainPhoto = product.ProductImages.FirstOrDefault(pi => pi.IsMain)?.Image;
                    request.AdditionalPhotos = product.ProductImages.Where(pi => !pi.IsMain).Select(pi => pi.Image).ToList();

                    ModelState.AddModelError("MainImage", "Shekil max 2mb olmalidir!");
                    return View(request);
                }
            }




            if (request.MainImage != null)
            {
                string oldMainPath = _env.WebRootPath.GetFilePath("img", product.ProductImages
                    .FirstOrDefault(pi => pi.IsMain)?.Image);
                oldMainPath.DeleteFile();

                string mainFileName = request.MainImage.GenerateFileName();
                string mainPath = _env.WebRootPath.GetFilePath("img", mainFileName);
                request.MainImage.SaveFile(mainPath);

                product.ProductImages.FirstOrDefault(p => p.IsMain)?.Image = mainFileName;
            }

            if (request.AdditionalImages != null)
            {
                var oldAdditionalImages = product.ProductImages.Where(pi => !pi.IsMain).ToList();

                foreach (var item in oldAdditionalImages)
                {
                    string oldAdditionalPath = _env.WebRootPath.GetFilePath("img", item.Image);
                    oldAdditionalPath.DeleteFile();
                }

                foreach (var item in request.AdditionalImages)
                {
                    string additioanlFileName = item.GenerateFileName();
                    string additionalPath = _env.WebRootPath.GetFilePath("img", additioanlFileName);
                    item.SaveFile(additionalPath);

                    product.ProductImages.FirstOrDefault(p => !p.IsMain)?.Image = additioanlFileName;
                }
            }

            product.Name = request.Name;
            product.Price = (double)request.Price;
            product.CategoryId = request.CategoryId;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<SelectList> GetAllCategories()
        {
            var categories = await _context.Categories
                .Where(c => !c.IsDeleted)
                .ToListAsync();

            return new SelectList(categories, "Id", "Name");
        }
    }

    public static class FileExtensions
    {
        public static bool CheckFileType(this IFormFile file, string fileType)
        {
            return file.ContentType.StartsWith(fileType, StringComparison.OrdinalIgnoreCase);
        }

        public static bool CheckFileSize(this IFormFile file, int maxKb)
        {
            return file.Length > maxKb * 1024;
        }

        public static string GenerateFileName(this IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName);
            string uniqueName = Guid.NewGuid().ToString();
            return $"{uniqueName}{extension}";
        }

        // Add this extension method to fix CS1061
        public static void SaveFile(this IFormFile file, string path)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
        }
    }

    public static class PathExtensions
    {
        public static string GetFilePath(this string rootPath, params string[] paths)
        {
            var allPaths = new List<string> { rootPath };
            allPaths.AddRange(paths);
            return Path.Combine(allPaths.ToArray());
        }
    }

    public static class FileHelperExtensions
    {
        public static void DeleteFile(this string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
