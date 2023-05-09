using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CarAccidentVideo:EntityBase
    {
        public string VideoUrl { get; set; }

        [ForeignKey("CarAccident")]
        public Guid CarAccidentId { get; set; }
        public CarAccident CarAccident { get; set; }
    }
}
