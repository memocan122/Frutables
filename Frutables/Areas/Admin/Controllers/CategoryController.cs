using Frutables.Areas.Admin.ViewModels.CategoryVMs;
using Frutables.Data;
using Frutables.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Frutables.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> categories = await _context.Categories.OrderByDescending(m => m.Id).Where(c => !c.IsDeleted).ToListAsync();
            IEnumerable<GetAllCategoryVM> getAllcategoryvms = categories.Select(c => new GetAllCategoryVM()
            {
                Id = c.Id,
                Name = c.Name,
            });

            return View(getAllcategoryvms);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryVm request)
        {
            if (!ModelState.IsValid) return View();
            bool isExist = await _context.Categories.AnyAsync(c => c.Name.ToLower().Trim() == request.Name.ToLower().Trim());
            if (isExist)
            {
                ModelState.AddModelError("Name", "Bu Adda Movcuddr!");
                return View();
            }
            Category newCategory = new Category()
            {
                Name = request.Name,
            };

            await _context.Categories.AddAsync(newCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (category == null) return NotFound()
                    ;
            DetailCategoryVM detailCategoryVM = new DetailCategoryVM()
            {
                ID = category.Id,
                Name = category.Name,
            };

            return View(detailCategoryVM);

        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            Category category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (category == null) return NotFound();
            UpdateCategoryVm updateCategoryVm = new UpdateCategoryVm()
            {
                Name = category.Name,
            };
            return View(updateCategoryVm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateCategoryVm request)
        {
            if (!ModelState.IsValid) return View();
            Category? category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (category == null) return NotFound();
            bool isExist = await _context.Categories.AnyAsync(c => c.Name.ToLower().Trim() == category.Name.ToLower().Trim() && c.Id != id);



            if (isExist)
            {
                ModelState.AddModelError("Name", "Bu Adda Movcuddr!");
                return View();
            }
            category.Name = request.Name;

            //Category updatedCategory = new()(melimin dediyi variantdi sehla melime gpt falan deyil 100 bal verin nolayyyy)
            //{
            //    Id = category.Id,
            //    Name = request.Name,

            //};
            //_context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (category == null) return NotFound();

            //category.IsDeleted = true;  bu soft delete ucun beledi melim vidyoda gosterib 
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
