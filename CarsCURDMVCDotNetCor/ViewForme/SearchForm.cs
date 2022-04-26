using System.ComponentModel.DataAnnotations;

namespace CarsCURDMVCDotNetCor.ViewForme
{
    public class SearchForm
    {
        public int? CompaneyId { get; set; }
        public int ?CategoryId { get; set; }
        public List<string>? ModelNames { get; set; }
        [Display(Name = "Model")]
        public string? ModelNameSelected { get; set; }

        public List<int>? ModelYears     { get; set; }
        [Display(Name ="Year")]
        public int ? ModelYearSelected { get; set; }
        [Display(Name = "min price")]
        public decimal ? MinPrice { get; set; }
        [Display(Name = "Max price")]
        public decimal ? MaxPrice { get; set; }



    }
}
