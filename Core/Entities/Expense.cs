using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
 public   class Expense : EntityBase
    {

        public DateTime Date { get; set; }
        public double Value { get; set; }
        public string Notes { get; set; }

        [ForeignKey("ExpenseType")]
        public Guid ExpenseTypeId { get; set; }
        public ExpenseType ExpenseType { get; set; }

        [ForeignKey("Stock")]
        public Guid StockId { get; set; }
        public Stock Stock { get; set; }

    }
}
