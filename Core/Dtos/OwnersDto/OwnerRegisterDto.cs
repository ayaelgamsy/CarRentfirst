using Core.Dtos.DropDowns;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Dtos.OwnersDto
{
    
    public class OwnerRegisterDto
    {
        public Guid? Id { get; set; }
        

        [Display(Name = "Identity")]
        [StringLength(14, ErrorMessage = "Lenght", MinimumLength = 14)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage ="No Char")]
        
        public string IdentNumber { get; set; }

        public string PassportNumber { get; set; }

        [Required(ErrorMessage = "*")]
        public string Address { get; set; }

        [Display(Name = "Phone")]
        [Required(ErrorMessage = "*")]
        [StringLength(11, ErrorMessage = "Lenght", MinimumLength = 11)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "No Char")]
        public string Phone { get; set; }
        
        [Required(ErrorMessage = "*")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Name")]
        public string name { get; set; }
        
        public Guid? GovernmentId { get; set; }
        public List<GovernmentDrop> Government { get; set; }

        public string Country { get; set; }

        public List<CarOwnerPhoto> CarOwnerPhotos { get; set; } = new List<CarOwnerPhoto>();


        public byte[] OwnerImage { get; set; }
    }
}
