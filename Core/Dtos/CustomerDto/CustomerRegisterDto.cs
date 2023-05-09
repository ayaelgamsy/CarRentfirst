using Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerDto
{
    public class CustomerRegisterDto
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage ="*")]
        public string Name { get; set; }

        [Required(ErrorMessage = "*")]
        public string Address { get; set; }
        public string Email { get; set; }

        [Display(Name = "Phone")]
        [Required(ErrorMessage = "*")]
        [StringLength(11, ErrorMessage = "Lenght", MinimumLength = 11)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "No Char")]
        public string phone1 { get; set; }

       
        [StringLength(11, ErrorMessage = "Lenght", MinimumLength = 11)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "No Char")]
        public string phone2 { get; set; }

        
        [StringLength(11, ErrorMessage = "Lenght", MinimumLength = 11)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "No Char")]
        public string phone3 { get; set; }

        [Display(Name = "Identity")]
        [StringLength(14, ErrorMessage = "Lenght", MinimumLength = 14)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "No Char")]
        public string IdentNumber { get; set; }

        public string PassportNumber { get; set; }
        public string GobTitle { get; set; }

        public string Location1 { get; set; }
        public string Location2 { get; set; }

        [Required(ErrorMessage = "*")]
        public double StartAccount { get; set; }

        [Required(ErrorMessage = "*")]
        public DateTime? Date { get; set; } = DateTime.Now;


        public Guid? GovernmentId { get; set; }
        public IEnumerable<Government> Governments { get; set; }

        public string Country { get; set; }

        public List<CustomerPhoto> CustomerPhotos { get; set; }
        public List<IFormFile> CustomerPhotoFile { get; set; }

    }
}
