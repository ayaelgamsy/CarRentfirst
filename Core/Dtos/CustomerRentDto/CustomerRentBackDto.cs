using Core.Dtos.ExpenseDto;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerRentDto
{
   public class CustomerRentBackDto
    {

        public Guid Id { get; set; }

      
        [Display(Name = "الضامن")]
        public string GuarantorName { get; set; }

        [Display(Name = "اسم العميل")]
        public string CustomerName { get; set; }
        public Guid CustomerId { get; set; }

        [Display(Name = "اختر السياره")]
        public string CarName { get; set; }
        public Guid CarId { get; set; }

        [Display(Name = "الخزنة")]
        public string StockName { get; set; }
        public Guid StockId { get; set; }

        [Display(Name = "الموظف")]
        public string EmployeeName { get; set; }
        public Guid EmployeeId { get; set; }


        
        [Display(Name = "المسوق")]
        public string MarketerName { get; set; }
        public Guid MarketerId { get; set; }


        [Required(ErrorMessage = "*")]
        [Display(Name = "بدايه الفتره")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "*")]
        [Display(Name = "نهايه الفتره")]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "عدد الايام")]
        [RegularExpression(@"(\d+)?(\.)?(\d+)?", ErrorMessage = "No Char")]
        public double? NumberOfDays { get; set; }

        [Display(Name = "سعر اليوم")]
        [RegularExpression(@"(\d+)?(\.)?(\d+)?", ErrorMessage = "No Char")]
        public double? PricePerDay { get; set; }

        [Display(Name = "الاجمالي")]
        public double Total { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "المدفوع")]
        [RegularExpression(@"(\d+)?(\.)?(\d+)?", ErrorMessage = "No Char")]
        public double? payment { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "الخصم")]
        [RegularExpression(@"(\d+)?(\.)?(\d+)?", ErrorMessage = "No Char")]
        public double RentDiscount { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "الباقي")]
        public double RestValue { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "رقم العداد قبل التأجير")]
        public double? CounterStart { get; set; }


        [Required(ErrorMessage = "*")]
        [Display(Name = "المسافه المسموحه")]
        public double? AllowedDistance { get; set; }

        [Display(Name = "ملاحظات")]
        public string Notes { get; set; }


        [Required(ErrorMessage = "*")]
        [Display(Name = "المستلم")]
        public Guid EmployeeReceiveId { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = " الخزنة")]
        public Guid StockBackId { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "تاريخ العوده الفعلى ")]
        public DateTime? ActualBackDate { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "عدد ساعات التاخير")]
        public double LateHours { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "سعر ساعه التاخير")]
        public double PriceTimeLatePerHoure { get; set; }


        [Required(ErrorMessage = "*")]
        [Display(Name = "سعر كيلو التاخير")]
        public double PriceLatePerKm { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "الاجمالى الكلي بعد التأخير")]
        public double FinalTotalAfterLate { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "المدفوع")]
        [RegularExpression(@"(\d+)?(\.)?(\d+)?", ErrorMessage = "No Char")]
        public double PaymentFinal { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "الخصم")]
        [RegularExpression(@"(\d+)?(\.)?(\d+)?", ErrorMessage = "No Char")]
        public double RentBackDiscount { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "الباقي")]
        [RegularExpression(@"(\d+)?(\.)?(\d+)?", ErrorMessage = "No Char")]
        public double RentBackRest { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "قراءة العداد")]
        [RegularExpression(@"(\d+)?(\.)?(\d+)?", ErrorMessage = "No Char")]
        public double CounterEnd { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "سعر غسيل السيارة")]
        [RegularExpression(@"(\d+)?(\.)?(\d+)?", ErrorMessage = "No Char")]
        public double PriceWash { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "سعر  البنزين")]
        [RegularExpression(@"(\d+)?(\.)?(\d+)?", ErrorMessage = "No Char")]
        public double PetrolPrice { get; set; }


        [Display(Name = "تاريخ استحقاق باقى المبلغ")]
        public DateTime? DueDate { get; set; }


        [Required(ErrorMessage = "*")]
        [Display(Name = "المسافة الزائده")]
        public double IncreaseDistanc { get; set; }

        [Display(Name = " التقييم")]
        public Guid? CustomerEvaluationId { get; set; }
        public IEnumerable<CustomerEvaluation> CustomerEvaluations { get; set; }

        public IEnumerable<DrpDto> Stocks { get; set; }
        public IEnumerable<DrpDto> Employees { get; set; }
      //  public IEnumerable<DrpDto> Cars { get; set; }
       // public IEnumerable<DrpDto> Customers { get; set; }
    }
}
