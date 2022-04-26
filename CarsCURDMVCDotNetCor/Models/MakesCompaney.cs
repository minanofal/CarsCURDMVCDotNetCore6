namespace CarsCURDMVCDotNetCor.Models
{
    public class MakesCompaney
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public byte[] Image { get; set; }

        public virtual ICollection<Category>? Categories { get; set; }

        public virtual ICollection<CarModel>? Models { get; set; }

        public virtual ICollection<CategoryMakesCompaney>? CategoryMakesCompaneys { get; set; }

    }
}
