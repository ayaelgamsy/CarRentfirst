using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.ExpenseDto
{
  public   class ExpenseGetDto
    {

        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public string Notes { get; set; }
        public string ExpenseTypeName { get; set; }
        public string StockName { get; set; }

    }
}
