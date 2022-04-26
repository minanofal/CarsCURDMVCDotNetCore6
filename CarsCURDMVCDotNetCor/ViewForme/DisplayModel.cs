namespace CarsCURDMVCDotNetCor.ViewForme
{
    public class DisplayModel
    {
        public int Id { get; set; }
        public string ModelName { get; set; }
        public int Year { get; set; }

        public decimal Price { get; set; }

        public  byte[]? Image { get; set; }
        public string MakesCompaneyName { get; set; }

        public string CategoryName { get; set; }
       
        public string Discription { get; set; }
        public int MotorCapacity { get; set; }
    }
}
