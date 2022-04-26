namespace CarsCURDMVCDotNetCor.ViewForme
{
    public class ModelDetails
    {
        public string ModelName { get; set; }
        public int Year { get; set; }

        public decimal Price { get; set; }

        public byte[]? Image { get; set; }
        public string MakesCompaneyName { get; set; }

        public string CategoryName { get; set; }

        public string Discription { get; set; }

        public double Hight { get; set; }

        public double Tall { get; set; }

        public double Width { get; set; }

        public string Faul { get; set; }

        public int MotorCapacity { get; set; }

        public List<string>? CarsColors { get; set; }
    }
}
