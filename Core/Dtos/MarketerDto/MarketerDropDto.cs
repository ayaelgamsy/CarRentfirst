using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.MarketerDto
{
    public class MarketerDropDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string MarketerNameAndPhone
        {
            get
            {
                return string.Format("{0} - {1}", Name, Phone);
            }
        }
    }
}
