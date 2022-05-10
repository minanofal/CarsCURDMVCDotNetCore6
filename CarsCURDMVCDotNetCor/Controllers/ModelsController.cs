using CarsCURDMVCDotNetCor.Data;
using CarsCURDMVCDotNetCor.Models;
using CarsCURDMVCDotNetCor.ViewForme;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace CarsCURDMVCDotNetCor.Controllers
{
    public class ModelsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _TostrNotification;
        private readonly List<string> _ExtentionList = new List<string>() { ".jpg", ".png" };
        private readonly long _MaxlengthImage = 5048576;

        public ModelsController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _TostrNotification = toastNotification;
        }
        public async Task<IActionResult> Index()
        {
            var models = await _context.Cars.Include(c => c.MakesCompaney).Include(c => c.Category)
                .Select(c => new DisplayModel { ModelName = c.ModelName, CategoryName = c.Category.Name, MakesCompaneyName = c.MakesCompaney.Name, Price = c.Price, Year = c.Year, Image = c.Image, MotorCapacity = c.MotorCapacity, Discription = c.Discription, Id = c.Id }).ToListAsync();

            return View(models);
        }

        public async Task<IActionResult> Create()
        {
            var model = new ModelViewForm()
            {
                Categories = await _context.Categories.OrderBy(c=>c.Name).ToListAsync(),
                MakesCompaneys = await _context.MakesCompaneys.OrderBy(c=>c.Name).ToListAsync(),
                Fauls = new List<string>() { "Oil", "Gas" },
                CarsColorsList = new List<string>() { "Red", "Blue", "Black", "White" }
            };
            return View("ModelForm",model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ModelViewForm model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _context.Categories.ToListAsync();
                model.MakesCompaneys = await _context.MakesCompaneys.ToListAsync();
                model.Fauls = new List<string>() { "Oil", "Gas" };
                model.CarsColorsList = new List<string>() { "Red", "Blue", "Black", "White" };

                _TostrNotification.AddErrorToastMessage("SomeThing Went Wrong");
                return View("ModelForm", model);

            }
            var files = Request.Form.Files;
            if (!files.Any())
            {
                model.Categories = await _context.Categories.ToListAsync();
                model.MakesCompaneys = await _context.MakesCompaneys.ToListAsync();
                model.Fauls = new List<string>() { "Oil", "Gas" };
                model.CarsColorsList = new List<string>() { "Red", "Blue", "Black", "White" };
                ModelState.AddModelError("Image", "Please Select Image");

                _TostrNotification.AddErrorToastMessage("SomeThing Went Wrong");
                return View("ModelForm", model);
            }
            var image = files.FirstOrDefault();
            if (!_ExtentionList.Contains(Path.GetExtension(image.FileName).ToLower()))
            {
                model.Categories = await _context.Categories.ToListAsync();
                model.MakesCompaneys = await _context.MakesCompaneys.ToListAsync();
                model.Fauls = new List<string>() { "Oil", "Gas" };
                model.CarsColorsList = new List<string>() { "Red", "Blue", "Black", "White" };
                ModelState.AddModelError("Image", ".jpg , .png imahes only");

                _TostrNotification.AddErrorToastMessage("SomeThing Went Wrong");
                return View("ModelForm", model);
            }

            if (_MaxlengthImage < image.Length)
            {
                model.Categories = await _context.Categories.ToListAsync();
                model.MakesCompaneys = await _context.MakesCompaneys.ToListAsync();
                model.Fauls = new List<string>() { "Oil", "Gas" };
                model.CarsColorsList = new List<string>() { "Red", "Blue", "Black", "White" };
                ModelState.AddModelError("Image", "Image is too large");

                _TostrNotification.AddErrorToastMessage("SomeThing Went Wrong");
                return View("ModelForm", model);
            }
            using var datastream = new MemoryStream();
            await image.CopyToAsync(datastream);

            var Model = new CarModel()
            {
                ModelName = model.ModelName.Trim(),
                Year = model.Year,
                Hight = model.Hight,
                Width = model.Width,
                Tall = model.Tall,
                MotorCapacity = model.MotorCapacity,
                Faul = model.Faul,
                CategoryId = model.CategoryId,
                MakesCompaneyId = model.MakesCompaneyId,
                Image = datastream.ToArray(),
                Discription = model.Discription,
                Price = model.Price,
            };

            _context.Cars.Add(Model);
            _context.SaveChanges();

            var car = await _context.Cars.OrderBy(c => c.Id).Select(c => c.Id).LastOrDefaultAsync();

            var colors = new List<CarsColor>();
            foreach (var item in model.CarsColors)
            {
                if(item !=null)
                    colors.Add(new CarsColor() { CarModelId = car, Color = item });
            }
            _context.carsColors.AddRange(colors);

            var companey_category_test = await _context.CategoryMakesCompaney.SingleOrDefaultAsync(c=> c.MakesCompaneyId==model.MakesCompaneyId && c.CategoryId == model.CategoryId);
            if (companey_category_test == null)
            {
                var CompanyCategory = new CategoryMakesCompaney()
                {
                    CategoryId = model.CategoryId,
                    MakesCompaneyId = model.MakesCompaneyId,
                };
                _context.CategoryMakesCompaney.Add(CompanyCategory);
            }
            _context.SaveChanges();
            _TostrNotification.AddSuccessToastMessage("Model Created Successfully !");

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

           ModelDetails car = await _context.Cars.Where(c => c.Id == id).Include(c=>c.Category)
                .Include(c=>c.MakesCompaney)
                .Include(c=>c.CarsColors).Select(c=> new ModelDetails
                {
                    CarsColors = (List<string>)c.CarsColors.Select(c => c.Color ),
                    CategoryName = c.Category.Name,
                    Discription = c.Discription,
                    Faul = c.Faul,
                    Hight=c.Hight,
                    Image =c.Image,
                    MakesCompaneyName= c.MakesCompaney.Name,
                    ModelName= c.ModelName,
                    MotorCapacity=c.MotorCapacity,
                    Price=c.Price,
                    Tall=c.Tall,
                    Width=c.Width,
                    Year= c.Year


                }).FirstOrDefaultAsync();
            if( car == null)
            {
                return NotFound();
            }
           
            return View(car);

        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            ModelViewForm model = await _context.Cars.Select(c => new ModelViewForm 
            {
                Id = c.Id,
                CategoryId = c.CategoryId,
                Discription = c.Discription,
                Faul =c.Faul,
                Hight = c.Hight,
                Tall = c.Tall,
                Width= c.Width,
                MakesCompaneyId=c.MakesCompaneyId,
                Image =c.Image,
                ModelName=c.ModelName,
                Price =c.Price,
                MotorCapacity =c.MotorCapacity,
                Year =c.Year,
                CarsColors  = c.CarsColors.Select(c=>c.Color).ToList(),
                
            })
                .Where(c=> c.Id == id)
                .SingleOrDefaultAsync();

            model.Categories = await _context.Categories.ToListAsync();
            model.MakesCompaneys = await _context.MakesCompaneys.ToListAsync();
            model.Fauls = new List<string>() { "Oil", "Gas" };
            model.CarsColorsList = new List<string>() { "Red", "Blue", "Black", "White" };
            return View("ModelForm",model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ModelViewForm model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _context.Categories.ToListAsync();
                model.MakesCompaneys = await _context.MakesCompaneys.ToListAsync();
                model.Fauls = new List<string>() { "Oil", "Gas" };
                model.CarsColorsList = new List<string>() { "Red", "Blue", "Black", "White" };

                _TostrNotification.AddErrorToastMessage("SomeThing Went Wrong");
                return View("ModelForm", model);
            }
            var car = await _context.Cars.SingleOrDefaultAsync(c => c.Id== model.Id);
            if (car == null)
                return NotFound();
            var files = Request.Form.Files;
            if (files.Any())
            {
                var image = files.FirstOrDefault();
                if (!_ExtentionList.Contains(Path.GetExtension(image.FileName).ToLower()))
                {
                    model.Categories = await _context.Categories.ToListAsync();
                    model.MakesCompaneys = await _context.MakesCompaneys.ToListAsync();
                    model.Fauls = new List<string>() { "Oil", "Gas" };
                    model.CarsColorsList = new List<string>() { "Red", "Blue", "Black", "White" };
                    ModelState.AddModelError("Image", ".jpg , .png imahes only");

                    _TostrNotification.AddErrorToastMessage("SomeThing Went Wrong");
                    return View("ModelForm", model);
                }

                if (_MaxlengthImage < image.Length)
                {
                    model.Categories = await _context.Categories.ToListAsync();
                    model.MakesCompaneys = await _context.MakesCompaneys.ToListAsync();
                    model.Fauls = new List<string>() { "Oil", "Gas" };
                    model.CarsColorsList = new List<string>() { "Red", "Blue", "Black", "White" };
                    ModelState.AddModelError("Image", "Image is too large");

                    _TostrNotification.AddErrorToastMessage("SomeThing Went Wrong");
                    return View("ModelForm", model);
                }
                using var datastream = new MemoryStream();
                await image.CopyToAsync(datastream);
                car.Image = datastream.ToArray();
            }
            car.Width = model.Width;
            car.CategoryId= model.CategoryId;
            car.MotorCapacity = model.MotorCapacity;
            car.Year = model.Year;
            car.ModelName = model.ModelName;
            car.Tall = model.Tall;
            car.Faul = model.Faul;
            car.Discription = model.Discription;
            car.Price = model.Price;
            car.MakesCompaneyId = model.MakesCompaneyId;
            car.Hight = model.Hight;

            var old_Colors = await _context.carsColors.Where(c => c.CarModelId == model.Id).ToListAsync();
            _context.RemoveRange(old_Colors);
            var colors = new List<CarsColor>();
            foreach (var item in model.CarsColors)
            {
                colors.Add(new CarsColor() { CarModelId = car.Id, Color = item });
            }
            _context.AddRange(colors);
            _context.SaveChanges();
            _TostrNotification.AddSuccessToastMessage("Model Edited Successfully");
            

            return RedirectToAction(nameof(Index));

        }

       
        public async Task<IActionResult> Delete(int? id)
        { 
            if (id == null)
                return BadRequest();
            var car = await _context.Cars.SingleOrDefaultAsync(c=>c.Id==id);
            if (car == null)
                return NotFound();
            _context.Cars.Remove(car);
            var Colors = await _context.carsColors.Where(c => c.CarModelId == id).ToListAsync();
            _context.carsColors.RemoveRange(Colors);
            _context.SaveChanges();
            var CategoryCompaney = await _context.Cars.Where(c => c.MakesCompaneyId == car.MakesCompaneyId && c.CategoryId == car.CategoryId).ToListAsync();
            if (!CategoryCompaney.Any())
            {
                var CompaneyCategory = await _context.CategoryMakesCompaney.Where(c => c.MakesCompaneyId == car.MakesCompaneyId && c.CategoryId == car.CategoryId).SingleOrDefaultAsync();
                _context.CategoryMakesCompaney.Remove(CompaneyCategory);
                _context.SaveChanges();
            }
            return Ok();
        }

       
    }
}
