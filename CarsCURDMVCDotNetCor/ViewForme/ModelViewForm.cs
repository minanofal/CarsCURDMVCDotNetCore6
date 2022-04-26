using System.ComponentModel.DataAnnotations;
using CarsCURDMVCDotNetCor.Models;

namespace CarsCURDMVCDotNetCor.ViewForme
{
    public class ModelViewForm
    {
        public int Id { get; set; }

        public string ModelName { get; set; }

        public int Year { get; set; }

        public decimal Price { get; set; }

        [Range(1,3)]
        [Display(Name ="Hight by mater")]
        public double Hight { get; set; }
        [Range(3, 6)]
        [Display(Name = "Tall by mater")]
        public double Tall { get; set; }
        [Range(1, 3)]
        [Display(Name = "Width by mater")]
        public double Width { get; set; }

        public byte[]? Image { get; set; }

        public string Discription { get; set; }

        [Display(Name = " Capacity Of Motor")]
        public int MotorCapacity { get; set; }

        public List<string>? Fauls { get; set; }

        public string Faul { get; set; }
        [Display(Name ="Category")]
        public int CategoryId { get; set; }
        [Display(Name ="Companey")]
        public int MakesCompaneyId { get; set; }
        public IEnumerable<Category>?  Categories { get; set; }
        public IEnumerable< MakesCompaney>? MakesCompaneys { get; set; }
        public List<string>? CarsColors { get; set; }
        public List<string>? CarsColorsList { get; set; }
    }
}
