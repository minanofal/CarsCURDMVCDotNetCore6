using CarsCURDMVCDotNetCor.Models;
using System.ComponentModel.DataAnnotations;

namespace CarsCURDMVCDotNetCor.ViewForme
{
    public class CategoryViewForm
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        public byte[]? Image { get; set; }

       
    
    }
}
