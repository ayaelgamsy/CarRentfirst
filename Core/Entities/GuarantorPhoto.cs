using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class GuarantorPhoto
    {
        [Key]
        public Guid Id { get; set; }
        public string PhotoUrl { get; set; }

        [ForeignKey("CustomerRent")]
        public Guid CustomerRentId { get; set; }
        public CustomerRent CustomerRent { get; set; }
    }
}
