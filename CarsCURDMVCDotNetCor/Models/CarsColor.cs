namespace CarsCURDMVCDotNetCor.Models
{
    public class CarsColor
    {
        public string Color { get; set; }

        public int CarModelId { get; set; }

        public virtual CarModel? CarModel { get; set; }
    }
}
