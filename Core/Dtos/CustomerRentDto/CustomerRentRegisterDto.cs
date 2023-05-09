using Core.Common.enums;
using Core.Dtos.EmployeeDto;
using Core.Dtos.ExpenseDto;
using Core.Dtos.MarketerDto;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Core.Dtos.CustomerRentDto
{
    public class CustomerRentRegisterDto
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "الضامن")]
        public string GuarantorName { get; set; }

        [Required(ErrorMessage ="*")]
        [Display(Name ="اختر العميل")]
        public Guid CustomerId { get; set; }

        [Display(Name = "هاتف العميل ")]
        public string CustomerNumber { get; set; }


        [Required(ErrorMessage = "*")]
        [Display(Name = "اختر السياره")]
        public Guid CarId { get; set; }


        [Required(ErrorMessage = "*")]
        [Display(Name = "بدايه الفتره")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "*")]
        public DayType TypeOfDay { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "نهايه الفتره")]
        public DateTime ?  EndDate { get; set; } 

        [Required(ErrorMessage = "*")]
        [Display(Name = "عدد الايام")]
        [RegularExpression(@"(\d+)?(\.)?(\d+)?", ErrorMessage = "No Char")]
        public double? NumberOfDays { get; set; }

        [Display(Name = "سعر اليوم")]
        [RegularExpression(@"(\d+)?(\.)?(\d+)?", ErrorMessage = "No Char")]
        public double? PricePerDay { get; set; }

        [Display(Name = "الاجمالي")]
        public double Total { get; set; }

        [Required(ErrorMessage ="*")]
        [Display(Name = "المدفوع")]
        [RegularExpression(@"(\d+)?(\.)?(\d+)?", ErrorMessage = "No Char")]
        public double ? payment { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "الخصم")]
        [RegularExpression(@"(\d+)?(\.)?(\d+)?", ErrorMessage = "No Char")]
        public double RentDiscount { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "الباقي")]
        public double RestValue { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "رقم العداد قبل التأجير")]
        public double ? CounterStart { get; set; }

   

        [Required(ErrorMessage = "*")]
        [Display(Name = "الموظف")]
        public Guid EmployeeId { get; set; }
      

        [Required(ErrorMessage = "*")]
        [Display(Name = "المسافه المسموحه")]
        public double? AllowedDistance { get; set; }
        public string SerialNumber { get; set; }

        [Display(Name = "ملاحظات")]
        public string Notes { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "الخزنة")]
        public Guid StockId { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "المسوق")]
        public Guid MarketerId { get; set; }



        public IEnumerable<DrpDto> Stocks { get; set; }
        public IEnumerable<DrpDto> Employees { get; set; }
        public IEnumerable<MarketerDropDto> Marketers { get; set; }
        public IEnumerable<DrpDto> Cars { get; set; }
        public IEnumerable<DrpDto> Customers { get; set; }

        public List<GuarantorPhoto> GuarantorPhotos { get; set; }
        public List<IFormFile> GuarantorPhotoFile { get; set; }

    }
}
