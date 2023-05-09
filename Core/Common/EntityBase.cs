using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common
{
   public class EntityBase
    {
        [Key]
        public Guid Id { get; set; }

        public string CreatedUser { get; set; }
        public string LastEditUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastEditDate { get; set; }

    }
}
