using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CustomerReservationPhoto:EntityBase
    {
        public string PhotoUrl { get; set; }

        [ForeignKey("CustomerReservation")]
        public Guid CustomerReservationId { get; set; }
        public CustomerReservation CustomerReservation { get; set; }
    }
}
