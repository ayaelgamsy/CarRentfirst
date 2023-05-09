using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CarVideo:EntityBase
    {
        public string VideoUrl { get; set; }

        [ForeignKey("Car")]
        public Guid CarId { get; set; }
        public Car Car { get; set; }
    }
}
