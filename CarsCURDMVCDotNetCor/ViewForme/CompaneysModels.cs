using CarsCURDMVCDotNetCor.Models;

namespace CarsCURDMVCDotNetCor.ViewForme
{
    public class CompaneysModels
    {
        public IEnumerable<DisplayModel>? Models { get; set; }

        public IEnumerable<Category>? Categories { get; set; }

        public IEnumerable<MakesCompaney>? Companeys  { get; set; }

        public SearchForm ? searchForm { get; set; }
    }
}
