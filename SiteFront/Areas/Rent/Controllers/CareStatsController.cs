using AutoMapper;
using Core.Common.enums;
using Core.Dtos.CarDto;
using Core.Dtos.CustomerRentDto;
using Core.Dtos.ExpenseDto;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteFront.Areas.Rent.Controllers
{
    [Area("Rent")]
   // [AllowAnonymous]
    public class CareStatsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IToastNotification _toastNotification;
        private readonly IRepository<Car> carRepo;
        private readonly IRepository<Company> companyRepo;
        private readonly IRepository<CarPhoto> carPhotoRepo;
        private readonly IRepository<CustomerRent> _CustomerRentRepo;
        private readonly IRepository<CarVideo> carVideoRepo;

        private readonly IRepository<Customer> _custRepo;
        private readonly IRepository<Employee> _emplRepo;
        private readonly IRepository<Car> _carRepo;
        private readonly IRepository<Stock> _StockRepo;
        private readonly IRepository<StockMovement> _StockMovementRepo;
        private readonly IRepository<CustomerAccount> _CustomerAccountRepo;
        private readonly IRepository<CustomerEvaluation> _customerEvaluationRepo;

        public CareStatsController(IMapper mapper,
            IToastNotification toastNotification,
            IRepository<Car> CarRepo,
            IRepository<CustomerRent> CustomerRentRepo,
            IRepository<Company> CompanyRepo,
            IRepository<CarPhoto> CarPhotoRepo,
            IRepository<CarVideo> CarVideoRepo,
            IRepository<Customer> custRepo,
            IRepository<Employee> emplRepo,
            IRepository<Stock> StockRepo,
            IRepository<StockMovement> StockMovementRepo,
            IRepository<CustomerAccount> CustomerAccountRepo,
            IRepository<CustomerEvaluation> customerEvaluationRepo
)
        {
            _mapper = mapper;
            _toastNotification = toastNotification;
            carRepo = CarRepo;
            companyRepo = CompanyRepo;
            carPhotoRepo = CarPhotoRepo;
            carVideoRepo = CarVideoRepo;
            _CustomerRentRepo = CustomerRentRepo;
            _emplRepo = emplRepo;
            _carRepo = carRepo;
            _StockRepo = StockRepo;
            _StockMovementRepo = StockMovementRepo;
            _CustomerAccountRepo = CustomerAccountRepo;
            _customerEvaluationRepo = customerEvaluationRepo;
            _custRepo = custRepo;
        }

        [Authorize("Permissions.CareStatsAvailable")]
        [AllowAnonymous]
        public IActionResult Available()
        {
            var tommorow = DateTime.Now.AddDays(1);
            //var now_date = DateTime.Now.Date;
            //DateTime End = DateTime.Parse(tommorow.ToString("MM/dd/yyyy"));
            //var dateend = End.Date;// date end.

            var carData = carRepo.GetAllAsync(c => c.Active == true && c.Available==true, c => c.Company, c => c.CarPhotos, c => c.CustomerRents).Result;
            var carDataComeToday = carRepo.GetAllAsync(c => c.Active == true && c.Available==false, c => c.Company, c => c.CarPhotos, c => c.CustomerRents).Result.Where(c => c.EndTripDate.Date < tommorow.Date);
            var AllCars= carData.Concat(carDataComeToday);  
            var carGetDto = _mapper.Map<List<CarAvailableGetDto>>(AllCars);

            return View(carGetDto);
        }

        [Authorize("Permissions.CareStatsAvailableToday")]
        public IActionResult AvailableToday()
        {

            var carDataComeToday = carRepo.GetAllAsync(c => c.Active == true && c.Available == false, c => c.CarPhotos, c => c.CustomerRents).Result
                .Where(c => c.EndTripDate.Date == DateTime.Now.Date).Select(c => new CarAvailableGetDto
                {
                    CustomerRents = _CustomerRentRepo.GetAllAsync(c => c.Customer).Result.Where(c=>c.EndDate.Date== DateTime.Now.Date).ToList(),
                    CarModel=c.CarModel,
                    Name=c.name,
                    EndTripDate=c.EndTripDate,
                    Number=c.Number,
                    CarPhotos=c.CarPhotos,
                    Id=c.Id
                }).ToList();
 
            return View(carDataComeToday);
        }

        [Authorize("Permissions.CareStatsNotAvailable")]
        [AllowAnonymous]
        public IActionResult NotAvailable()
        {
            //var carData = await carRepo.GetAllAsync(c => c.Active == true && c.Available == false, c => c.Company, c => c.CarPhotos);
           // var carData =  carRepo.GetAllAsync(c => c.Active == true && c.Available == false, c => c.Company, c => c.CarPhotos, c=>c.CustomerRents).Result.Where(n=>n.CustomerRents.FirstOrDefault(n=>n.Finished==true)));
            var carData = _CustomerRentRepo.GetAllAsync(n=>n.Finished==false,n=>n.Car,c=>c.Car.Company, c => c.Car.CarPhotos).Result.Select(n => new CarUnavailableGetDto 
            {
                CarId=n.CarId,
                Name = n.Car.name,
                CompanyName = n.Car.Company.name,
                CarModel= n.Car.CarModel,
                Number = n.Car.Number,
                PriceOfDaye = n.Car.PriceOfDaye,
                CustomerRentId = n.Id,
                EndDate=n.EndDate,
                CarPhotos = n.Car.CarPhotos
            });
              return View(carData);
        }

        [Authorize("Permissions.CareStatsEditRentBack")]
        [HttpGet]
        public async Task<IActionResult> EditRentBack(Guid id)
        {
            var customerRent = await _CustomerRentRepo.SingleOrDefaultAsync(n=>n.Id== id, true , n=>n.Car,n=>n.Customer,n=>n.Employee, n => n.Marketer, n=>n.Stock);
            if (customerRent == null)
                return NotFound();

            var Employees = await _emplRepo.GetAllAsync(true);
            var Stocks = await _StockRepo.GetAllAsync(true);
          
            var customerRentModel = _mapper.Map<CustomerRentBackDto>(customerRent);
            customerRentModel.PriceWash = customerRent.Car.PriceWash;
            customerRentModel.PriceTimeLatePerHoure = customerRent.Car.PriceTimeLatePerHoure;
            customerRentModel.PriceLatePerKm = customerRent.Car.PricePerKm;

            customerRentModel.Employees = _mapper.Map<List<DrpDto>>(Employees);
            customerRentModel.Stocks = _mapper.Map<List<DrpDto>>(Stocks);
            customerRentModel.CustomerEvaluations = await _customerEvaluationRepo.GetAllAsync();

            return View(customerRentModel);
        }

        [Authorize("Permissions.CareStatsEditRentBack")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRentBack(CustomerRentBackDto model)
        {
             if (!ModelState.IsValid)
            {
                var customerRent = await _CustomerRentRepo.SingleOrDefaultAsync(n => n.Id == model.Id, n => n.Car, n => n.Customer, n => n.Employee, n => n.Stock, n => n.Marketer);
                var Employees = await _emplRepo.GetAllAsync(true);
                var Stocks = await _StockRepo.GetAllAsync(true);
                var customerRentModel = _mapper.Map<CustomerRentBackDto>(customerRent);
                customerRentModel.PriceWash = customerRent.Car.PriceWash;
                customerRentModel.PriceTimeLatePerHoure = customerRent.Car.PriceTimeLatePerHoure;
                customerRentModel.PriceLatePerKm = customerRent.Car.PricePerKm;
                customerRentModel.Employees = _mapper.Map<List<DrpDto>>(Employees);
                customerRentModel.Stocks = _mapper.Map<List<DrpDto>>(Stocks);
                customerRentModel.CustomerEvaluations = await _customerEvaluationRepo.GetAllAsync();
                return View(customerRentModel);
            }

          //  var EditCustomerBackRent = await _CustomerRentRepo.GetByIdAsync((Guid)model.Id);
            var EditCustomerBackRent = await _CustomerRentRepo.SingleOrDefaultAsync(n=>n.Id== (Guid)model.Id,n=>n.Car, n => n.Customer, n => n.Employee, n => n.Stock, n => n.Marketer);

            var CustomerBackRentMaped = _mapper.Map(model, EditCustomerBackRent);
            CustomerBackRentMaped.Car.Available = true;
            CustomerBackRentMaped.Finished = true;
         


            _CustomerRentRepo.Update(CustomerBackRentMaped);

            StockMovement stockMovement = await _StockMovementRepo.SingleOrDefaultAsync(c => c.MovementId == CustomerBackRentMaped.Id && c.MovementType==StockMovementType.CustomerRentBack);

            if(stockMovement != null)
            {
                stockMovement.StockId = (Guid) CustomerBackRentMaped.StockBackId;
                stockMovement.Date = CustomerBackRentMaped.ActualBackDate;
                stockMovement.InValue = CustomerBackRentMaped.PaymentFinal;
                stockMovement.OutValue = 0;
                stockMovement.Comment = "دفعة من فاتوره تاجير للعميل"+" " + model.CustomerName;
                stockMovement.Notes = CustomerBackRentMaped.Notes;

                _StockMovementRepo.Update(stockMovement);
            }
            else
            {
                 stockMovement = new StockMovement
                {
                    MovementId = CustomerBackRentMaped.Id,
                    MovementType= StockMovementType.CustomerRentBack,
                    StockId = (Guid)CustomerBackRentMaped.StockBackId,
                    Date = CustomerBackRentMaped.ActualBackDate,
                    InValue = CustomerBackRentMaped.PaymentFinal,
                    OutValue = 0,
                    Comment = "دفعة من استلام فاتورة  تاجير للعميل" +" "+ model.CustomerName,
                    Notes = CustomerBackRentMaped.Notes
                };
                _StockMovementRepo.Add(stockMovement);

            }

            CustomerAccount customerAccount = await _CustomerAccountRepo.SingleOrDefaultAsync(c => c.MovementId == CustomerBackRentMaped.Id  &&c.AccountType==RentAccountType.RentBack);
            if (customerAccount != null)
            {
                customerAccount.CustomerId = CustomerBackRentMaped.CustomerId;
                customerAccount.Date = CustomerBackRentMaped.ActualBackDate;
                customerAccount.Borrower = CustomerBackRentMaped.PaymentFinal;
                customerAccount.Dept = CustomerBackRentMaped.FinalTotalAfterLate;
                customerAccount.Explain = "استلام فاتوره تاجير للعميل"+" " + model.CustomerName;
                customerAccount.Notes = CustomerBackRentMaped.Notes;
                _CustomerAccountRepo.Update(customerAccount);
            }
            else
            {
                customerAccount = new CustomerAccount
                {
                    CustomerId = CustomerBackRentMaped.CustomerId,
                    Date = CustomerBackRentMaped.ActualBackDate,
                    Borrower = CustomerBackRentMaped.PaymentFinal,
                    Dept = CustomerBackRentMaped.FinalTotalAfterLate,
                    Explain = "استلام فاتوره تاجير للعميل"+" " + model.CustomerName,
                    Notes = CustomerBackRentMaped.Notes,
                    MovementId = CustomerBackRentMaped.Id,
                    AccountType= RentAccountType.RentBack
                };

                _CustomerAccountRepo.Add(customerAccount);

            }

            var customer = await _custRepo.SingleOrDefaultAsync(c => c.Id == model.CustomerId);
            customer.CustomerEvaluationId = model.CustomerEvaluationId;

            await _CustomerRentRepo.SaveAllAsync();

            _toastNotification.AddSuccessToastMessage("تم الحفظ");

            return RedirectToAction(nameof(NotAvailable));
        }



        }
}
