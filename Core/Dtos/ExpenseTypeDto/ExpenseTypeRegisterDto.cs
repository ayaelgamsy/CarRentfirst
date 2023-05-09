using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Dtos.ExpenseTypeDto
{
    public class ExpenseTypeRegisterDto
    {
        public Guid ? Id { get; set; }

        [Required(ErrorMessage ="*")]
        public string Name { get; set; }

        public List<ExpenseTypeGetDto> ExpenseTypeGetDtos { get; set; }
    }
}
