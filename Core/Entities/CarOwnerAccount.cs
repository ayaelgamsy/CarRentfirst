﻿using Core.Common;
using Core.Common.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
  public  class CarOwnerAccount : EntityBase
    {
        [ForeignKey("CarOwner")]
        public Guid CarOwnerId { get; set; }
        public CarOwner CarOwner { get; set; }

        public double Dept { get; set; }
        public double Borrower { get; set; }
        public double Account { get; set; }
        
        public DateTime Date { get; set; }
      
        public string Notes { get; set; }
       
        public string Explain { get; set; }

        public Guid MovementId { get; set; }

        public RentAccountType AccountType { get; set; }

       

      

        

    }
}
