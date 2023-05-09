using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
 public   class MarketerPhoto : EntityBase
    {
        public string PhotoUrl { get; set; }

        [ForeignKey("Marketer")]
        public Guid EmployeeId { get; set; }
        public Marketer Marketer { get; set; }
    }
}
