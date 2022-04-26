using System.ComponentModel.DataAnnotations;

namespace CarsCURDMVCDotNetCor.ViewForme
{
    public class MakeCompaneyViewForm
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        public virtual byte[]? Image { get; set; }

    }
}
