using CarsCURDMVCDotNetCor.Data;
using CarsCURDMVCDotNetCor.Models;
using CarsCURDMVCDotNetCor.ViewForme;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace CarsCURDMVCDotNetCor.Controllers
{
    public class MakesCompaneyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly List<string> _AllowedExtentions = new List<string>() {".jpg",".png",".jpeg"};
        private readonly long _MaxImageLength = 5048576;
        private readonly IToastNotification _toastrNotification;


        public MakesCompaneyController(ApplicationDbContext coontext , IToastNotification toastrNotification)
        {
            _context = coontext; 

            _toastrNotification = toastrNotification;
        }
        public async  Task<IActionResult> Index()
        {
           var Companeyes = await _context.MakesCompaneys.ToListAsync();
            return View(Companeyes);
        }

        public async Task<IActionResult> Create()
        {
            var Companey = new MakeCompaneyViewForm();
            return View("MakeCompaneyForm",Companey);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MakeCompaneyViewForm Companey)
        {
            if (!ModelState.IsValid)
            {
                _toastrNotification.AddErrorToastMessage("Something Went Wrong");
                return View("MakeCompaneyForm", Companey);
            }
            
            var files = Request.Form.Files;
            if (!files.Any())
            {
                ModelState.AddModelError("Image", "Please Select An Image");
                _toastrNotification.AddErrorToastMessage("Something Went Wrong");
                return View("MakeCompaneyForm", Companey);
            }
            var image = files.FirstOrDefault();
            if(!_AllowedExtentions.Contains(Path.GetExtension(image.FileName).ToLower()))
            {
                ModelState.AddModelError("Image", ".jpg, .png, .jpeg images onley");
                _toastrNotification.AddErrorToastMessage("Something Went Wrong");
                return View("MakeCompaneyForm", Companey);

            }
            if(_MaxImageLength < image.Length)
            {
                ModelState.AddModelError("Image", "max size 5mg");
                _toastrNotification.AddErrorToastMessage("Something Went Wrong");
                return View("MakeCompaneyForm", Companey);

            }
            using var datastream = new MemoryStream();
            await image.CopyToAsync(datastream);

            var companey = new MakesCompaney()
            {
                Name = Companey.Name,
                Image = datastream.ToArray()
            };

            _context.MakesCompaneys.Add(companey);
            _context.SaveChanges();
            _toastrNotification.AddSuccessToastMessage("The Make Created Sucessfully");
            return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
                return BadRequest();
            
            var companey = await _context.MakesCompaneys.SingleOrDefaultAsync(c=>c.Id==id);

            if (companey == null)
                return NotFound();

            var Companey = new MakeCompaneyViewForm()
            {
                Id = companey.Id,
                Name = companey.Name,
                Image = companey.Image,
            };
            return View("MakeCompaneyForm", Companey);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MakeCompaneyViewForm Companey)
        {
            if (!ModelState.IsValid)
            {
                _toastrNotification.AddErrorToastMessage("Something Went Wrong");
                return View("MakeCompaneyForm", Companey);
            }
            var companey = await _context.MakesCompaneys.SingleOrDefaultAsync(c=>c.Id== Companey.Id);
            if(companey == null)
            {
                return NotFound();
            }
            var file = Request.Form.Files;

            if (file.Any())
            {
                var image = file.FirstOrDefault();
                if (!_AllowedExtentions.Contains(Path.GetExtension(image.FileName).ToLower()))
                {
                    ModelState.AddModelError("Image", ".jpg, .png, .jpeg images onley");
                    _toastrNotification.AddErrorToastMessage("Something Went Wrong");
                    return View("MakeCompaneyForm", Companey);

                }
                if (_MaxImageLength < image.Length)
                {
                    ModelState.AddModelError("Image", "max size 5mg");
                    _toastrNotification.AddErrorToastMessage("Something Went Wrong");
                    return View("MakeCompaneyForm", Companey);

                }
                using var datastream = new MemoryStream();
                await image.CopyToAsync(datastream);
                companey.Image=datastream.ToArray();


            }
            if (companey.Name != Companey.Name){
                companey.Name = Companey.Name;
                _context.SaveChanges();
                _toastrNotification.AddSuccessToastMessage("The Make Updated Sucessfully"); }
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Delete(int? id) {
            if (id == null)
                return BadRequest();

            var companey = await _context.MakesCompaneys.SingleOrDefaultAsync(c=> c.Id == id);

            if (companey == null)
                return NotFound();
            _context.MakesCompaneys.Remove(companey);
            _context.SaveChanges();

            return Ok();
        }

        public async Task<IActionResult> CompaneysModels(int? id)
        {
            if (id == null)
                return BadRequest();

            var companey = await _context.MakesCompaneys.SingleOrDefaultAsync(c => c.Id == id);

            if (companey == null)
                return NotFound();

            var Cars = await _context.Cars.Include(c=>c.MakesCompaney)
                .Include(c=>c.Category).Where(c=>c.MakesCompaneyId==id)
                .Select(c=> new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
                .ToListAsync();
            //var categories = await _context.MakesCompaneys.Include(m => m.Categories).Where(m => m.Id == id).Select(c => new Category { Id = c.Categories.Select(c=>c.Id).SingleOrDefault(), Image = c.Image, Name = c.Categories.Select(c=>c.Name).SingleOrDefault() }).ToListAsync();
            var categories = await _context.MakesCompaneys.Where(c => c.Id == id).Select(c => c.Categories).SingleOrDefaultAsync();
            CompaneysModels companeysModels = new CompaneysModels()
            {
                Models = Cars,
                Categories = categories,
                searchForm = new SearchForm() 
                { 
                    CompaneyId = id ,
                    ModelYears = await _context.Cars.Where(c=>c.MakesCompaneyId==id).Select(C => C.Year).Distinct().ToListAsync(),
                    ModelNames= await _context.Cars.Where(c => c.MakesCompaneyId == id).Select(c => c.ModelName).Distinct().ToListAsync()
        },
                
            };
            return View(companeysModels);

    }
        public async  Task<IActionResult> Search(CompaneysModels model)
        {
            
                if (model.searchForm.ModelYearSelected == null && model.searchForm.ModelNameSelected == null && model.searchForm.MinPrice == null && model.searchForm.MaxPrice == null)
                {
                    return RedirectToAction(nameof(CompaneysModels), new { id = model.searchForm.CompaneyId });
                }
                else if (model.searchForm.ModelYearSelected == null && model.searchForm.ModelNameSelected == null && model.searchForm.MinPrice == null && model.searchForm.MaxPrice != null)
                {
                    model.Models = await _context.Cars.Include(c => c.MakesCompaney)
                .Include(c => c.Category).Where(c => c.MakesCompaneyId == model.searchForm.CompaneyId && c.Price <= model.searchForm.MaxPrice)
                .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
                .ToListAsync();
                    model.Categories = await _context.Cars
                        .Include(C => C.MakesCompaney)
                        .Include(c => c.Category)
                        .Where(c => c.MakesCompaney.Id == model.searchForm.CompaneyId && c.Price <= model.searchForm.MaxPrice)
                        .Select(c => c.Category)
                        .ToListAsync();
                }
                else if (model.searchForm.ModelYearSelected == null && model.searchForm.ModelNameSelected == null && model.searchForm.MinPrice != null && model.searchForm.MaxPrice == null)
                {
                    model.Models = await _context.Cars.Include(c => c.MakesCompaney)
                .Include(c => c.Category).Where(c => c.MakesCompaneyId == model.searchForm.CompaneyId && c.Price >= model.searchForm.MinPrice)
                .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
                .ToListAsync();
                    model.Categories = await _context.Cars
                        .Include(C => C.MakesCompaney)
                        .Include(c => c.Category)
                        .Where(c => c.MakesCompaney.Id == model.searchForm.CompaneyId && c.Price >= model.searchForm.MinPrice)
                        .Select(c => c.Category)
                        .ToListAsync();
                }
                else if (model.searchForm.ModelYearSelected == null && model.searchForm.ModelNameSelected == null && model.searchForm.MinPrice != null && model.searchForm.MaxPrice != null)
                {
                    model.Models = await _context.Cars.Include(c => c.MakesCompaney)
                .Include(c => c.Category).Where(c => c.MakesCompaneyId == model.searchForm.CompaneyId && c.Price <= model.searchForm.MaxPrice && c.Price >= model.searchForm.MinPrice)
                .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
                .ToListAsync();
                    model.Categories = await _context.Cars
                        .Include(C => C.MakesCompaney)
                        .Include(c => c.Category)
                        .Where(c => c.MakesCompaney.Id == model.searchForm.CompaneyId && c.Price <= model.searchForm.MaxPrice && c.Price >= model.searchForm.MinPrice)
                        .Select(c => c.Category)
                        .ToListAsync();
                }
                else if (model.searchForm.ModelYearSelected == null && model.searchForm.ModelNameSelected != null && model.searchForm.MinPrice == null && model.searchForm.MaxPrice == null)
                {
                    model.Models = await _context.Cars.Include(c => c.MakesCompaney)
                  .Include(c => c.Category).Where(c => c.MakesCompaneyId == model.searchForm.CompaneyId && c.ModelName == model.searchForm.ModelNameSelected)
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
                  .Include(c => c.Category).Where(c => c.MakesCompaneyId == model.searchForm.CompaneyId && c.ModelName == model.searchForm.ModelNameSelected && c.Price <= model.searchForm.MaxPrice)
                  .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
                  .ToListAsync();
                    model.Categories = await _context.Cars
                        .Include(C => C.MakesCompaney)
                        .Include(c => c.Category)
                        .Where(c => c.MakesCompaney.Id == model.searchForm.CompaneyId && c.ModelName == model.searchForm.ModelNameSelected && c.Price <= model.searchForm.MaxPrice)
                        .Select(c => c.Category)
                        .ToListAsync();
                }
                else if (model.searchForm.ModelYearSelected == null && model.searchForm.ModelNameSelected != null && model.searchForm.MinPrice != null && model.searchForm.MaxPrice == null)
                {
                    model.Models = await _context.Cars.Include(c => c.MakesCompaney)
                  .Include(c => c.Category).Where(c => c.MakesCompaneyId == model.searchForm.CompaneyId && c.ModelName == model.searchForm.ModelNameSelected && c.Price >= model.searchForm.MinPrice)
                  .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
                  .ToListAsync();
                    model.Categories = await _context.Cars
                        .Include(C => C.MakesCompaney)
                        .Include(c => c.Category)
                        .Where(c => c.MakesCompaney.Id == model.searchForm.CompaneyId && c.ModelName == model.searchForm.ModelNameSelected && c.Price >= model.searchForm.MinPrice)
                        .Select(c => c.Category)
                        .ToListAsync();
                }
                else if (model.searchForm.ModelYearSelected == null && model.searchForm.ModelNameSelected != null && model.searchForm.MinPrice != null && model.searchForm.MaxPrice != null)
                {
                    model.Models = await _context.Cars.Include(c => c.MakesCompaney)
                .Include(c => c.Category).Where(c => c.MakesCompaneyId == model.searchForm.CompaneyId && c.Price <= model.searchForm.MaxPrice && c.Price >= model.searchForm.MinPrice && c.ModelName == model.searchForm.ModelNameSelected)
                .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
                .ToListAsync();
                    model.Categories = await _context.Cars
                        .Include(C => C.MakesCompaney)
                        .Include(c => c.Category)
                        .Where(c => c.MakesCompaney.Id == model.searchForm.CompaneyId && c.Price <= model.searchForm.MaxPrice && c.Price >= model.searchForm.MinPrice && c.ModelName == model.searchForm.ModelNameSelected)
                        .Select(c => c.Category)
                        .ToListAsync();
                }
                else if (model.searchForm.ModelYearSelected != null && model.searchForm.ModelNameSelected == null && model.searchForm.MinPrice == null && model.searchForm.MaxPrice == null)
                {
                    model.Models = await _context.Cars.Include(c => c.MakesCompaney)
                   .Include(c => c.Category).Where(c => c.MakesCompaneyId == model.searchForm.CompaneyId && c.Year == model.searchForm.ModelYearSelected)
                   .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
                   .ToListAsync();
                    model.Categories = await _context.Cars
                        .Include(C => C.MakesCompaney)
                        .Include(c => c.Category)
                        .Where(c => c.MakesCompaney.Id == model.searchForm.CompaneyId && c.Year == model.searchForm.ModelYearSelected)
                        .Select(c => c.Category)
                        .ToListAsync();
                }
                else if (model.searchForm.ModelYearSelected != null && model.searchForm.ModelNameSelected == null && model.searchForm.MinPrice == null && model.searchForm.MaxPrice != null)
                {
                    model.Models = await _context.Cars.Include(c => c.MakesCompaney)
                   .Include(c => c.Category).Where(c => c.MakesCompaneyId == model.searchForm.CompaneyId && c.Year == model.searchForm.ModelYearSelected && c.Price <= model.searchForm.MaxPrice)
                   .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
                   .ToListAsync();
                    model.Categories = await _context.Cars
                        .Include(C => C.MakesCompaney)
                        .Include(c => c.Category)
                        .Where(c => c.MakesCompaney.Id == model.searchForm.CompaneyId && c.Year == model.searchForm.ModelYearSelected && c.Price <= model.searchForm.MaxPrice)
                        .Select(c => c.Category)
                        .ToListAsync();
                }
                else if (model.searchForm.ModelYearSelected != null && model.searchForm.ModelNameSelected == null && model.searchForm.MinPrice != null && model.searchForm.MaxPrice == null)
                {
                    model.Models = await _context.Cars.Include(c => c.MakesCompaney)
                   .Include(c => c.Category).Where(c => c.MakesCompaneyId == model.searchForm.CompaneyId && c.Year == model.searchForm.ModelYearSelected && c.Price >= model.searchForm.MinPrice)
                   .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
                   .ToListAsync();
                    model.Categories = await _context.Cars
                        .Include(C => C.MakesCompaney)
                        .Include(c => c.Category)
                        .Where(c => c.MakesCompaney.Id == model.searchForm.CompaneyId && c.Year == model.searchForm.ModelYearSelected && c.Price >= model.searchForm.MinPrice)
                        .Select(c => c.Category)
                        .ToListAsync();
                }
                else if (model.searchForm.ModelYearSelected != null && model.searchForm.ModelNameSelected == null && model.searchForm.MinPrice != null && model.searchForm.MaxPrice != null)
                {
                    model.Models = await _context.Cars.Include(c => c.MakesCompaney)
                   .Include(c => c.Category).Where(c => c.MakesCompaneyId == model.searchForm.CompaneyId && c.Year == model.searchForm.ModelYearSelected && c.Price <= model.searchForm.MaxPrice && c.Price >= model.searchForm.MinPrice)
                   .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
                   .ToListAsync();
                    model.Categories = await _context.Cars
                        .Include(C => C.MakesCompaney)
                        .Include(c => c.Category)
                        .Where(c => c.MakesCompaney.Id == model.searchForm.CompaneyId && c.Year == model.searchForm.ModelYearSelected && c.Price <= model.searchForm.MaxPrice && c.Price >= model.searchForm.MinPrice)
                        .Select(c => c.Category)
                        .ToListAsync();
                }
                else if (model.searchForm.ModelYearSelected != null && model.searchForm.ModelNameSelected != null && model.searchForm.MinPrice == null && model.searchForm.MaxPrice == null)
                {
                    model.Models = await _context.Cars.Include(c => c.MakesCompaney)
                 .Include(c => c.Category).Where(c => c.MakesCompaneyId == model.searchForm.CompaneyId && c.ModelName == model.searchForm.ModelNameSelected && c.Year == model.searchForm.ModelYearSelected)
                 .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
                 .ToListAsync();
                    model.Categories = await _context.Cars
                        .Include(C => C.MakesCompaney)
                        .Include(c => c.Category)
                        .Where(c => c.MakesCompaney.Id == model.searchForm.CompaneyId && c.ModelName == model.searchForm.ModelNameSelected && c.Year == model.searchForm.ModelYearSelected)
                        .Select(c => c.Category)
                        .ToListAsync();
                }
                else if (model.searchForm.ModelYearSelected != null && model.searchForm.ModelNameSelected != null && model.searchForm.MinPrice == null && model.searchForm.MaxPrice != null)
                {
                    model.Models = await _context.Cars.Include(c => c.MakesCompaney)
                .Include(c => c.Category).Where(c => c.MakesCompaneyId == model.searchForm.CompaneyId && c.Price <= model.searchForm.MaxPrice && c.ModelName == model.searchForm.ModelNameSelected && c.Year == model.searchForm.ModelYearSelected)
                .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
                .ToListAsync();
                    model.Categories = await _context.Cars
                        .Include(C => C.MakesCompaney)
                        .Include(c => c.Category)
                        .Where(c => c.MakesCompaney.Id == model.searchForm.CompaneyId && c.Price <= model.searchForm.MaxPrice && c.ModelName == model.searchForm.ModelNameSelected && c.Year == model.searchForm.ModelYearSelected)
                        .Select(c => c.Category)
                        .ToListAsync();
                }
                else if (model.searchForm.ModelYearSelected != null && model.searchForm.ModelNameSelected != null && model.searchForm.MinPrice != null && model.searchForm.MaxPrice == null)
                {
                    model.Models = await _context.Cars.Include(c => c.MakesCompaney)
                .Include(c => c.Category).Where(c => c.MakesCompaneyId == model.searchForm.CompaneyId && c.Price >= model.searchForm.MinPrice && c.ModelName == model.searchForm.ModelNameSelected && c.Year == model.searchForm.ModelYearSelected)
                .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
                .ToListAsync();
                    model.Categories = await _context.Cars
                        .Include(C => C.MakesCompaney)
                        .Include(c => c.Category)
                        .Where(c => c.MakesCompaney.Id == model.searchForm.CompaneyId && c.Price >= model.searchForm.MinPrice && c.ModelName == model.searchForm.ModelNameSelected && c.Year == model.searchForm.ModelYearSelected)
                        .Select(c => c.Category)
                        .ToListAsync();
                }
                else if (model.searchForm.ModelYearSelected != null && model.searchForm.ModelNameSelected != null && model.searchForm.MinPrice != null && model.searchForm.MaxPrice != null)
                {
                    model.Models = await _context.Cars.Include(c => c.MakesCompaney)
                .Include(c => c.Category).Where(c => c.MakesCompaneyId == model.searchForm.CompaneyId && c.Price <= model.searchForm.MaxPrice && c.Price >= model.searchForm.MinPrice && c.ModelName == model.searchForm.ModelNameSelected && c.Year == model.searchForm.ModelYearSelected)
                .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id })
                .ToListAsync();
                    model.Categories = await _context.Cars
                        .Include(C => C.MakesCompaney)
                        .Include(c => c.Category)
                        .Where(c => c.MakesCompaney.Id == model.searchForm.CompaneyId && c.Price <= model.searchForm.MaxPrice && c.Price >= model.searchForm.MinPrice && c.ModelName == model.searchForm.ModelNameSelected && c.Year == model.searchForm.ModelYearSelected)
                        .Select(c => c.Category)
                        .ToListAsync();
                }
            



      
            model.searchForm.ModelYears= await _context.Cars.Where(c => c.MakesCompaneyId == model.searchForm.CompaneyId).Select(C=>C.Year).Distinct().ToListAsync();
            model.searchForm.ModelNames = await _context.Cars.Where(c => c.MakesCompaneyId == model.searchForm.CompaneyId).Select(c => c.ModelName).Distinct().ToListAsync();
         
            return View("CompaneysModels", model);
        }
    }
}
