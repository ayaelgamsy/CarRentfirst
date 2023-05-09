using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.OwnerAccountDto
{
    public class OwnerAccountRegisterDto
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid OwnerId { get; set; }
        public IEnumerable<CarOwner> CarOwners { get; set; }

        [Required(ErrorMessage = "*")]
        public DateTime? FromDate { get; set; }

        [Required(ErrorMessage = "*")]
        public DateTime? ToDate { get; set; }
    }
}
