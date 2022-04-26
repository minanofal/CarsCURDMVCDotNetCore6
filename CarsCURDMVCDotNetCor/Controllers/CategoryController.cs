using CarsCURDMVCDotNetCor.Data;
using CarsCURDMVCDotNetCor.Models;
using CarsCURDMVCDotNetCor.ViewForme;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace CarsCURDMVCDotNetCor.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _TostrNotification;
        private readonly List<string> _ExtentionList = new List<string>() { ".jpg", ".png" };
        private readonly long _MaxlengthImage = 5048576;

        public CategoryController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _TostrNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        public async Task<IActionResult> Create()
        {
            var category = new CategoryViewForm();
            return View("CategoryForm", category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewForm category)
        {
            if (!ModelState.IsValid) {
                _TostrNotification.AddErrorToastMessage("something went wrong");
                return View(category);
            }
            var files = Request.Form.Files;
            if (!files.Any())
            {
                ModelState.AddModelError("Image", "Please Select Image");
                _TostrNotification.AddErrorToastMessage("something went wrong");
                return View(category);

            }
            var image = files.FirstOrDefault();
            if (!_ExtentionList.Contains(Path.GetExtension(image.FileName).ToLower()))
            {
                ModelState.AddModelError("Image", ".png .jpg images inley");
                _TostrNotification.AddErrorToastMessage("something went wrong");
                return View(category);
            }
            if (_MaxlengthImage < image.Length)
            {
                ModelState.AddModelError("Image", "the image selected has large space");
                _TostrNotification.AddErrorToastMessage("something went wrong");
                return View(category);
            }

            using var datastream = new MemoryStream();
            await image.CopyToAsync(datastream);
            var Category = new Category() {
                Name = category.Name,
                Image = datastream.ToArray()
            };
            _context.Categories.Add(Category);
            _context.SaveChanges();
            _TostrNotification.AddSuccessToastMessage("Category Created Successfully");
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id == null)
                return BadRequest();
            var Category = await _context.Categories.SingleOrDefaultAsync(c => c.Id == Id);
            if (Category == null)
                return NotFound();
            var category = new CategoryViewForm()
            {
                Id = Category.Id,
                Name = Category.Name,
                Image = Category.Image
            };
            return View("CategoryForm", category);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryViewForm category)
        {
            if (!ModelState.IsValid)
            {
                _TostrNotification.AddErrorToastMessage("SomeThing Went Wrong");
                return View("CategoryForm", category);
            }
            var Category = await _context.Categories.SingleOrDefaultAsync(c => c.Id == category.Id);

            if (Category == null)
            {
                return NotFound();
            }
            var files = Request.Form.Files;
            if (files.Any())
            {
                var image = files.FirstOrDefault();
                if (!_ExtentionList.Contains(Path.GetExtension(image.FileName).ToLower()))
                {
                    _TostrNotification.AddErrorToastMessage("SomeThing Went Wrong");
                    ModelState.AddModelError("Image", ".jpg , .png images only");
                    return View("CategoryForm", category);
                }
                if (_MaxlengthImage < image.Length)
                {
                    _TostrNotification.AddErrorToastMessage("SomeThing Went Wrong");
                    ModelState.AddModelError("Image", "image space is too large");
                    return View("CategoryForm", category);
                }
                var datastream = new MemoryStream();
                await image.CopyToAsync(datastream);
                Category.Image = datastream.ToArray();

            }
            Category.Name = category.Name;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();
            var category = await _context.Categories.SingleOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return NotFound();
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return Ok();
        }
        public async Task<IActionResult> CategoryModel(int? id)
        {
            if (id == null)
                return BadRequest();

            var Category = await _context.Categories.SingleOrDefaultAsync(c => c.Id == id);

            if (Category == null)
                return NotFound();

            var Cars = await _context.Cars.Include(c => c.MakesCompaney)
                .Include(c => c.Category).Where(c => c.CategoryId == id)
                .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
                .ToListAsync();
            //var categories = await _context.MakesCompaneys.Include(m => m.Categories).Where(m => m.Id == id).Select(c => new Category { Id = c.Categories.Select(c=>c.Id).SingleOrDefault(), Image = c.Image, Name = c.Categories.Select(c=>c.Name).SingleOrDefault() }).ToListAsync();
            var Companys = await _context.Categories.Where(c => c.Id == id).Select(c => c.MakesCompaneys).SingleOrDefaultAsync();
            CompaneysModels companeysModels = new CompaneysModels()
            {
                Models = Cars,
                Companeys = Companys,
                searchForm = new SearchForm()
                {
                    CategoryId = id,
                    ModelYears = await _context.Cars.Where(c => c.CategoryId == id).Select(C => C.Year).Distinct().ToListAsync(),
                    ModelNames = await _context.Cars.Where(c => c.CategoryId== id).Select(c => c.ModelName).Distinct().ToListAsync()
                },

            };
            return View(companeysModels);

        }
        public async Task<IActionResult> Search(CompaneysModels model)
        {
            if (model.searchForm.ModelYearSelected == null && model.searchForm.ModelNameSelected == null && model.searchForm.MinPrice == null && model.searchForm.MaxPrice == null)
            {
                return RedirectToAction(nameof(CategoryModel), new { id = model.searchForm.CategoryId });
            }
            else if (model.searchForm.ModelYearSelected == null && model.searchForm.ModelNameSelected == null && model.searchForm.MinPrice == null && model.searchForm.MaxPrice != null)
            {
                model.Models = await _context.Cars.Include(c => c.MakesCompaney)
            .Include(c => c.Category).Where(c => c.CategoryId == model.searchForm.CategoryId && c.Price <= model.searchForm.MaxPrice)
            .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
            .ToListAsync();
                model.Companeys = await _context.Cars
                    .Include(C => C.MakesCompaney)
                    .Include(c => c.Category)
                    .Where(c => c.CategoryId == model.searchForm.CategoryId && c.Price <= model.searchForm.MaxPrice)
                    .Select(c => c.MakesCompaney)
                    .ToListAsync();
            }
            else if (model.searchForm.ModelYearSelected == null && model.searchForm.ModelNameSelected == null && model.searchForm.MinPrice != null && model.searchForm.MaxPrice == null)
            {
                model.Models = await _context.Cars.Include(c => c.MakesCompaney)
            .Include(c => c.Category).Where(c => c.CategoryId == model.searchForm.CategoryId && c.Price >= model.searchForm.MinPrice)
            .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
            .ToListAsync();
                model.Companeys = await _context.Cars
                    .Include(C => C.MakesCompaney)
                    .Include(c => c.Category)
                    .Where(c => c.CategoryId == model.searchForm.CompaneyId && c.Price >= model.searchForm.MinPrice)
                    .Select(c => c.MakesCompaney)
                    .ToListAsync();
            }
            else if (model.searchForm.ModelYearSelected == null && model.searchForm.ModelNameSelected == null && model.searchForm.MinPrice != null && model.searchForm.MaxPrice != null)
            {
                model.Models = await _context.Cars.Include(c => c.MakesCompaney)
            .Include(c => c.Category).Where(c => c.CategoryId == model.searchForm.CategoryId && c.Price <= model.searchForm.MaxPrice && c.Price >= model.searchForm.MinPrice)
            .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
            .ToListAsync();
                model.Companeys = await _context.Cars
                    .Include(C => C.MakesCompaney)
                    .Include(c => c.Category)
                    .Where(c => c.CategoryId == model.searchForm.CategoryId && c.Price <= model.searchForm.MaxPrice && c.Price >= model.searchForm.MinPrice)
                    .Select(c => c.MakesCompaney)
                    .ToListAsync();
            }
            else if (model.searchForm.ModelYearSelected == null && model.searchForm.ModelNameSelected != null && model.searchForm.MinPrice == null && model.searchForm.MaxPrice == null)
            {
                model.Models = await _context.Cars.Include(c => c.MakesCompaney)
              .Include(c => c.Category).Where(c => c.CategoryId == model.searchForm.CategoryId && c.ModelName == model.searchForm.ModelNameSelected)
              .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
              .ToListAsync();
                model.Categories = await _context.Cars
                    .Include(C => C.MakesCompaney)
                    .Include(c => c.Category)
                    .Where(c => c.MakesCompaney.Id == model.searchForm.CompaneyId && c.ModelName == model.searchForm.ModelNameSelected)
                    .Select(c => c.Category)
                    .ToListAsync();
            }
            else if (model.searchForm.ModelYearSelected == null && model.searchForm.ModelNameSelected != null && model.searchForm.MinPrice == null && model.searchForm.MaxPrice != null)
            {
                model.Models = await _context.Cars.Include(c => c.MakesCompaney)
              .Include(c => c.Category).Where(c => c.CategoryId == model.searchForm.CategoryId && c.ModelName == model.searchForm.ModelNameSelected && c.Price <= model.searchForm.MaxPrice)
              .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
              .ToListAsync();
                model.Companeys = await _context.Cars
                    .Include(C => C.MakesCompaney)
                    .Include(c => c.Category)
                    .Where(c => c.CategoryId == model.searchForm.CategoryId && c.ModelName == model.searchForm.ModelNameSelected && c.Price <= model.searchForm.MaxPrice)
                    .Select(c => c.MakesCompaney)
                    .ToListAsync();
            }
            else if (model.searchForm.ModelYearSelected == null && model.searchForm.ModelNameSelected != null && model.searchForm.MinPrice != null && model.searchForm.MaxPrice == null)
            {
                model.Models = await _context.Cars.Include(c => c.MakesCompaney)
              .Include(c => c.Category).Where(c => c.CategoryId == model.searchForm.CategoryId && c.ModelName == model.searchForm.ModelNameSelected && c.Price >= model.searchForm.MinPrice)
              .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
              .ToListAsync();
                model.Companeys = await _context.Cars
                    .Include(C => C.MakesCompaney)
                    .Include(c => c.Category)
                    .Where(c => c.CategoryId == model.searchForm.CategoryId && c.ModelName == model.searchForm.ModelNameSelected && c.Price >= model.searchForm.MinPrice)
                    .Select(c => c.MakesCompaney)
                    .ToListAsync();
            }
            else if (model.searchForm.ModelYearSelected == null && model.searchForm.ModelNameSelected != null && model.searchForm.MinPrice != null && model.searchForm.MaxPrice != null)
            {
                model.Models = await _context.Cars.Include(c => c.MakesCompaney)
            .Include(c => c.Category).Where(c => c.CategoryId == model.searchForm.CategoryId && c.Price <= model.searchForm.MaxPrice && c.Price >= model.searchForm.MinPrice && c.ModelName == model.searchForm.ModelNameSelected)
            .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
            .ToListAsync();
                model.Companeys = await _context.Cars
                    .Include(C => C.MakesCompaney)
                    .Include(c => c.Category)
                    .Where(c => c.CategoryId == model.searchForm.CategoryId && c.Price <= model.searchForm.MaxPrice && c.Price >= model.searchForm.MinPrice && c.ModelName == model.searchForm.ModelNameSelected)
                    .Select(c => c.MakesCompaney)
                    .ToListAsync();
            }
            else if (model.searchForm.ModelYearSelected != null && model.searchForm.ModelNameSelected == null && model.searchForm.MinPrice == null && model.searchForm.MaxPrice == null)
            {
                model.Models = await _context.Cars.Include(c => c.MakesCompaney)
               .Include(c => c.Category).Where(c => c.CategoryId == model.searchForm.CategoryId && c.Year == model.searchForm.ModelYearSelected)
               .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
               .ToListAsync();
                model.Companeys = await _context.Cars
                    .Include(C => C.MakesCompaney)
                    .Include(c => c.Category)
                    .Where(c => c.CategoryId == model.searchForm.CategoryId && c.Year == model.searchForm.ModelYearSelected)
                    .Select(c => c.MakesCompaney)
                    .ToListAsync();
            }
            else if (model.searchForm.ModelYearSelected != null && model.searchForm.ModelNameSelected == null && model.searchForm.MinPrice == null && model.searchForm.MaxPrice != null)
            {
                model.Models = await _context.Cars.Include(c => c.MakesCompaney)
               .Include(c => c.Category).Where(c => c.CategoryId == model.searchForm.CategoryId && c.Year == model.searchForm.ModelYearSelected && c.Price <= model.searchForm.MaxPrice)
               .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
               .ToListAsync();
                model.Companeys = await _context.Cars
                    .Include(C => C.MakesCompaney)
                    .Include(c => c.Category)
                    .Where(c => c.CategoryId == model.searchForm.CategoryId && c.Year == model.searchForm.ModelYearSelected && c.Price <= model.searchForm.MaxPrice)
                    .Select(c => c.MakesCompaney)
                    .ToListAsync();
            }
            else if (model.searchForm.ModelYearSelected != null && model.searchForm.ModelNameSelected == null && model.searchForm.MinPrice != null && model.searchForm.MaxPrice == null)
            {
                model.Models = await _context.Cars.Include(c => c.MakesCompaney)
               .Include(c => c.Category).Where(c => c.CategoryId == model.searchForm.CategoryId && c.Year == model.searchForm.ModelYearSelected && c.Price >= model.searchForm.MinPrice)
               .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
               .ToListAsync();
                model.Companeys = await _context.Cars
                    .Include(C => C.MakesCompaney)
                    .Include(c => c.Category)
                    .Where(c => c.CategoryId == model.searchForm.CategoryId && c.Year == model.searchForm.ModelYearSelected && c.Price >= model.searchForm.MinPrice)
                    .Select(c => c.MakesCompaney)
                    .ToListAsync();
            }
            else if (model.searchForm.ModelYearSelected != null && model.searchForm.ModelNameSelected == null && model.searchForm.MinPrice != null && model.searchForm.MaxPrice != null)
            {
                model.Models = await _context.Cars.Include(c => c.MakesCompaney)
               .Include(c => c.Category).Where(c => c.CategoryId == model.searchForm.CategoryId && c.Year == model.searchForm.ModelYearSelected && c.Price <= model.searchForm.MaxPrice && c.Price >= model.searchForm.MinPrice)
               .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
               .ToListAsync();
                model.Companeys = await _context.Cars
                    .Include(C => C.MakesCompaney)
                    .Include(c => c.Category)
                    .Where(c => c.CategoryId == model.searchForm.CategoryId && c.Year == model.searchForm.ModelYearSelected && c.Price <= model.searchForm.MaxPrice && c.Price >= model.searchForm.MinPrice)
                    .Select(c => c.MakesCompaney)
                    .ToListAsync();
            }
            else if (model.searchForm.ModelYearSelected != null && model.searchForm.ModelNameSelected != null && model.searchForm.MinPrice == null && model.searchForm.MaxPrice == null)
            {
                model.Models = await _context.Cars.Include(c => c.MakesCompaney)
             .Include(c => c.Category).Where(c => c.CategoryId == model.searchForm.CategoryId && c.ModelName == model.searchForm.ModelNameSelected && c.Year == model.searchForm.ModelYearSelected)
             .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
             .ToListAsync();
                model.Companeys = await _context.Cars
                    .Include(C => C.MakesCompaney)
                    .Include(c => c.Category)
                    .Where(c => c.CategoryId == model.searchForm.CategoryId && c.ModelName == model.searchForm.ModelNameSelected && c.Year == model.searchForm.ModelYearSelected)
                    .Select(c => c.MakesCompaney)
                    .ToListAsync();
            }
            else if (model.searchForm.ModelYearSelected != null && model.searchForm.ModelNameSelected != null && model.searchForm.MinPrice == null && model.searchForm.MaxPrice != null)
            {
                model.Models = await _context.Cars.Include(c => c.MakesCompaney)
            .Include(c => c.Category).Where(c => c.CategoryId == model.searchForm.CategoryId && c.Price <= model.searchForm.MaxPrice && c.ModelName == model.searchForm.ModelNameSelected && c.Year == model.searchForm.ModelYearSelected)
            .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
            .ToListAsync();
                model.Companeys = await _context.Cars
                    .Include(C => C.MakesCompaney)
                    .Include(c => c.Category)
                    .Where(c => c.CategoryId == model.searchForm.CategoryId && c.Price <= model.searchForm.MaxPrice && c.ModelName == model.searchForm.ModelNameSelected && c.Year == model.searchForm.ModelYearSelected)
                    .Select(c => c.MakesCompaney)
                    .ToListAsync();
            }
            else if (model.searchForm.ModelYearSelected != null && model.searchForm.ModelNameSelected != null && model.searchForm.MinPrice != null && model.searchForm.MaxPrice == null)
            {
                model.Models = await _context.Cars.Include(c => c.MakesCompaney)
            .Include(c => c.Category).Where(c => c.CategoryId == model.searchForm.CategoryId && c.Price >= model.searchForm.MinPrice && c.ModelName == model.searchForm.ModelNameSelected && c.Year == model.searchForm.ModelYearSelected)
            .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
            .ToListAsync();
                model.Companeys = await _context.Cars
                    .Include(C => C.MakesCompaney)
                    .Include(c => c.Category)
                    .Where(c => c.CategoryId == model.searchForm.CategoryId && c.Price >= model.searchForm.MinPrice && c.ModelName == model.searchForm.ModelNameSelected && c.Year == model.searchForm.ModelYearSelected)
                    .Select(c => c.MakesCompaney)
                    .ToListAsync();
            }
            else if (model.searchForm.ModelYearSelected != null && model.searchForm.ModelNameSelected != null && model.searchForm.MinPrice != null && model.searchForm.MaxPrice != null)
            {
                model.Models = await _context.Cars.Include(c => c.MakesCompaney)
            .Include(c => c.Category).Where(c => c.CategoryId == model.searchForm.CategoryId && c.Price <= model.searchForm.MaxPrice && c.Price >= model.searchForm.MinPrice && c.ModelName == model.searchForm.ModelNameSelected && c.Year == model.searchForm.ModelYearSelected)
            .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
            .ToListAsync();
                model.Companeys = await _context.Cars
                    .Include(C => C.MakesCompaney)
                    .Include(c => c.Category)
                    .Where(c => c.CategoryId == model.searchForm.CategoryId && c.Price <= model.searchForm.MaxPrice && c.Price >= model.searchForm.MinPrice && c.ModelName == model.searchForm.ModelNameSelected && c.Year == model.searchForm.ModelYearSelected)
                    .Select(c => c.MakesCompaney)
                    .ToListAsync();
            }





            model.searchForm.ModelYears = await _context.Cars.Where(c => c.CategoryId == model.searchForm.CategoryId).Select(C => C.Year).Distinct().ToListAsync();
            model.searchForm.ModelNames = await _context.Cars.Where(c => c.CategoryId == model.searchForm.CategoryId).Select(c => c.ModelName).Distinct().ToListAsync();

            return View("CategoryModel", model);
        }
    }

    }

