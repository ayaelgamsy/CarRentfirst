using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CustomerViolationPhoto:EntityBase
    {
        public string PhotoUrl { get; set; }

        [ForeignKey("CustomerViolation")]
        public Guid CustomerViolationId { get; set; }
        public CustomerViolation CustomerViolation { get; set; }
    }
}
