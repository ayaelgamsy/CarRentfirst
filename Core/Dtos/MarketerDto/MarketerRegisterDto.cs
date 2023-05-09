using Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.MarketerDto
{
    public class MarketerRegisterDto
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "*")]
        public string Name { get; set; }

        [Required(ErrorMessage = "*")]
        [StringLength(11, ErrorMessage = "Lenght", MinimumLength = 11)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "No Char")]
        public string Phone { get; set; }

        [StringLength(14, ErrorMessage = "Lenght", MinimumLength = 14)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "No Char")]
        public string IdentNumber { get; set; }

        public string PassportNumber { get; set; }

        [Required(ErrorMessage = "*")]
        public string Address { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public Guid? GovernmentId { get; set; }
        public IEnumerable<Government> Governments { get; set; }

        public string Country { get; set; }

        public List<MarketerPhoto> MarketerPhotos { get; set; }
        public List<IFormFile> MarketerPhotoFile { get; set; }
    }
}
