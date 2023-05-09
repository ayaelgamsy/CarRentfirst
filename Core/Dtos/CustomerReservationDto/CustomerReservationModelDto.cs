using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerReservationDto
{
    public class CustomerReservationModelDto
    {
        public List<CustomerReservationGetDto> CustomerReservationGetDtos { get; set; }
        public CustomerReservationRegisterDto CustomerReservationRegisterDto { get; set; }
    }
}
