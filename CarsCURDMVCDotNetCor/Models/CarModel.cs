using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarsCURDMVCDotNetCor.Models
{
    public class CarModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ModelName { get; set; }

        public int Year { get; set; }

        public decimal Price { get; set; }

        public double Hight { get; set; }

        public double Tall { get; set; }

        public double Width { get; set; }

        public string Faul { get; set; }

        public byte[] Image { get; set; }
        

        public string Discription { get; set; }

        [Display(Name =" Capacity Of Motor")]
        public int MotorCapacity { get; set; }



        public int CategoryId { get; set; }

        public int MakesCompaneyId { get; set; }
        public virtual Category? Category { get; set; }
        public virtual MakesCompaney? MakesCompaney { get; set; }

        public virtual ICollection<CarsColor>? CarsColors { get; set; }
    }
}
