using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.StockDto
{
    public class StockGetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double StartAccount { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public double Account { get; set; }
    }
}
