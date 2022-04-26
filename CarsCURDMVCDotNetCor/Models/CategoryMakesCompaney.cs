namespace CarsCURDMVCDotNetCor.Models
{
    public class CategoryMakesCompaney
    {
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public int MakesCompaneyId { get; set; }

        public virtual MakesCompaney? MakesCompaney { get; set; }
    }
}
