using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CarOwnerPhoto:EntityBase
    {
        public string PhotoUrl { get; set; }

        [ForeignKey("CarOwner")]
        public Guid CarOwnerId { get; set; }
        public CarOwner CarOwner { get; set; }
    }
}
