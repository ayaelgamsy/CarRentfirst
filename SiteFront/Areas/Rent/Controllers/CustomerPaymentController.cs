using AutoMapper;
using Core.Common.enums;
using Core.Dtos.CustomerPaymentDto;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteFront.Areas.Rent.Controllers
{
    [Area("Rent")]
    //[AllowAnonymous]
    public class CustomerPaymentController : Controller
    {

        public CustomerPayRegisterDto customerPayRegistermodel { get; set; }


        private readonly IMapper _mapper;
        private readonly IRepository<CustomerPayment> _custPayRepo;
        private readonly IRepository<Stock> _stockRepo;
        private readonly IRepository<Customer> _custRepo;
        private readonly IToastNotification _toastNotification;
        private readonly IRepository<StockMovement> _stockMovementRepo;
        private readonly IRepository<CustomerAccount> _customerAccountRepo;
        private readonly UserManager<User> _userManager;

        public CustomerPaymentController(IMapper mapper,
            IRepository<CustomerPayment> custPayRepo,
            IRepository<Stock> stockRepo,
            IRepository<Customer> custRepo,
            IToastNotification toastNotification,
            IRepository<StockMovement> stockMovementRepo,
            IRepository<CustomerAccount> customerAccountRepo,
            UserManager<User> userManager)
        {
            _mapper = mapper;
            _custPayRepo = custPayRepo;
            _stockRepo = stockRepo;
            _custRepo = custRepo;
            _toastNotification = toastNotification;
            _stockMovementRepo = stockMovementRepo;
            _customerAccountRepo = customerAccountRepo;
            _userManager = userManager;
        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize("Permissions.CustomerPaymentIndex")]
        public async Task<IActionResult> Index()
        {
            var CustomerPayData = await _custPayRepo.GetAllAsync(c => c.Customer, c => c.Stock);
            var customerPayGetDto = _mapper.Map<List<CustomerPayGetDto>>(CustomerPayData);
            var customerpayRegisterDto = new CustomerPayRegisterDto
            {
                Customers = await _custRepo.GetAllAsync(),
                Stocks = await _stockRepo.GetAllAsync()
            };
            var customerPaymentModelDto = new CustomerPaymentModelDto
            {
                CustomerPayGetDtos = customerPayGetDto,
                CustomerPayRegisterDto = customerpayRegisterDto
            };
            return View(customerPaymentModelDto);
            
        }

        [Authorize("Permissions.CustomerPaymentCreate")]
        [HttpPost]
        public async Task<IActionResult> Create(CustomerPayRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var CustomerpayDb = _mapper.Map<CustomerPayment>(model);
                CustomerpayDb.CreatedDate = DateTime.Now;
                CustomerpayDb.CreatedUser = (GetCurrentUserAsync().Result).Id.ToString();
                _custPayRepo.Add(CustomerpayDb);
                var customer = _custRepo.SingleOrDefaultAsync(c => c.Id == CustomerpayDb.CustomerId).Result.name;

                StockMovement stockMovement = new StockMovement
                {
                    MovementId = CustomerpayDb.Id,
                    MovementType=StockMovementType.CustomerPayment,
                    StockId= CustomerpayDb.StockId,
                    Date = CustomerpayDb.Date,
                    InValue= CustomerpayDb.Value,
                    OutValue=0,
                    Comment="دفعة من العميل "+" "+customer,
                    Notes= CustomerpayDb.Notes
                };

                _stockMovementRepo.Add(stockMovement);

                CustomerAccount customerAccount = new CustomerAccount
                {
                    CustomerId= CustomerpayDb.CustomerId,
                    Date= CustomerpayDb.Date,
                    Borrower= CustomerpayDb.Value,
                    Explain= " دفعة من العميل" +" "+ customer,
                    Notes= CustomerpayDb.Notes,
                    MovementId= CustomerpayDb.Id,
                    AccountType=RentAccountType.payment
                };
                _customerAccountRepo.Add(customerAccount);
                await _custPayRepo.SaveAllAsync();
                _toastNotification.AddSuccessToastMessage("تم الاضافة");
                return RedirectToAction("Index");
            }
            else
            {
                var CustomerPayData = await _custPayRepo.GetAllAsync(c => c.Customer, c => c.Stock);
                var customerPayGetDto = _mapper.Map<List<CustomerPayGetDto>>(CustomerPayData);
                var customerpayRegisterDto = _mapper.Map<CustomerPayRegisterDto>(model);
                customerpayRegisterDto.Customers = await _custRepo.GetAllAsync();
                customerpayRegisterDto.Stocks = await _stockRepo.GetAllAsync();
                var customerPaymentModelDto = new CustomerPaymentModelDto
                {
                    CustomerPayGetDtos = customerPayGetDto,
                    CustomerPayRegisterDto = customerpayRegisterDto
                };
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                return View("Index",customerPaymentModelDto);
            }

        }

        [Authorize("Permissions.CustomerPaymentEdit")]
        public async Task<IActionResult> GetData(Guid id)
        {
            var CustomerPaymentById = await _custPayRepo.GetByIdAsync(id);
            if (CustomerPaymentById == null)
                return NotFound();
            var customerpayRegisterDto = _mapper.Map<CustomerPayRegisterDto>(CustomerPaymentById);
            customerpayRegisterDto.Customers = await _custRepo.GetAllAsync();
            customerpayRegisterDto.Stocks = await _stockRepo.GetAllAsync();
            return PartialView("_PartialCustomerPayment", customerpayRegisterDto);
        }

        //For Movement And Account 
        [Authorize("Permissions.CustomerPaymentEdit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var CustomerPaymentById = await _custPayRepo.GetByIdAsync(id);
            if (CustomerPaymentById == null)
                return NotFound();
            var CustomerPayData = await _custPayRepo.GetAllAsync(c => c.Customer, c => c.Stock);
            var customerPayGetDto = _mapper.Map<List<CustomerPayGetDto>>(CustomerPayData);
            var customerpayRegisterDto = _mapper.Map<CustomerPayRegisterDto>(CustomerPaymentById);
            customerpayRegisterDto.Customers = await _custRepo.GetAllAsync();
            customerpayRegisterDto.Stocks = await _stockRepo.GetAllAsync();
            var customerPaymentModelDto = new CustomerPaymentModelDto
            {
                CustomerPayGetDtos = customerPayGetDto,
                CustomerPayRegisterDto = customerpayRegisterDto
            };
            return View("Index", customerPaymentModelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.CustomerPaymentEdit")]
        public async Task<IActionResult> Edit(CustomerPayRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var CustomerPayById = await _custPayRepo.GetByIdAsync((Guid)model.Id);
                var userAdd = CustomerPayById.CreatedUser;
                var userAddDate = CustomerPayById.CreatedDate;
                var CustomerPayByIdMapped = _mapper.Map(model, CustomerPayById);
                CustomerPayByIdMapped.CreatedDate = userAddDate;
                CustomerPayByIdMapped.CreatedUser = userAdd;
                CustomerPayByIdMapped.LastEditDate = DateTime.Now;
                CustomerPayByIdMapped.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                _custPayRepo.Update(CustomerPayByIdMapped);

                var customer = _custRepo.SingleOrDefaultAsync(c => c.Id == CustomerPayById.CustomerId).Result.name;

                var StockMovementById = await _stockMovementRepo.SingleOrDefaultAsync(s => s.MovementId == (Guid)model.Id&&s.MovementType==StockMovementType.CustomerPayment);
                StockMovementById.StockId = CustomerPayByIdMapped.StockId;
                StockMovementById.Date = CustomerPayByIdMapped.Date;
                StockMovementById.InValue = CustomerPayByIdMapped.Value;
                StockMovementById.Notes = CustomerPayByIdMapped.Notes;
                StockMovementById.Comment = "دفعة من العميل "+" "+ customer;
                _stockMovementRepo.Update(StockMovementById);

                var CustomerAccountById = await _customerAccountRepo.SingleOrDefaultAsync(c => c.MovementId == (Guid)model.Id&&c.AccountType==RentAccountType.payment);
                CustomerAccountById.CustomerId = CustomerPayByIdMapped.CustomerId;
                CustomerAccountById.Date = CustomerPayByIdMapped.Date;
                CustomerAccountById.Borrower = CustomerPayByIdMapped.Value;
                CustomerAccountById.Notes = CustomerPayByIdMapped.Notes;
                CustomerAccountById.Explain = "دفعة من العميل " +" "+ customer;
                _customerAccountRepo.Update(CustomerAccountById);

                await _custPayRepo.SaveAllAsync();
                _toastNotification.AddSuccessToastMessage("تم التعديل");
                return RedirectToAction("Index");
            }
            else
            {
                var CustomerPayData = await _custPayRepo.GetAllAsync(c => c.Customer, c => c.Stock);
                var customerPayGetDto = _mapper.Map<List<CustomerPayGetDto>>(CustomerPayData);
                var customerpayRegisterDto = _mapper.Map<CustomerPayRegisterDto>(model);
                customerpayRegisterDto.Customers = await _custRepo.GetAllAsync();
                customerpayRegisterDto.Stocks = await _stockRepo.GetAllAsync();
                var customerPaymentModelDto = new CustomerPaymentModelDto
                {
                    CustomerPayGetDtos = customerPayGetDto,
                    CustomerPayRegisterDto = customerpayRegisterDto
                };
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                return View("Index", customerPaymentModelDto);
            }
        }

        [Authorize("Permissions.CustomerPaymentDelete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var StockMovementById = await _stockMovementRepo.SingleOrDefaultAsync(s => s.MovementId == id&& s.MovementType==StockMovementType.CustomerPayment);
            _stockMovementRepo.Delete(StockMovementById);

            var CustomerAccountById = await _customerAccountRepo.SingleOrDefaultAsync(c => c.MovementId == id&& c.AccountType==RentAccountType.payment);
            _customerAccountRepo.Delete(CustomerAccountById);

            var CustomerPayById = await _custPayRepo.GetByIdAsync(id);
            _custPayRepo.Delete(CustomerPayById);

            await _custPayRepo.SaveAllAsync();
            _toastNotification.AddSuccessToastMessage("تم الحذف");
            return RedirectToAction("Index");
        }



        // apis
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCustomerAccount(Guid custometId)
        {
            var CustomerDb = await _custRepo.SingleOrDefaultAsync(c=>c.Id== custometId ,c=>c.customerAccounts);
            
            if (CustomerDb == null)
                return NotFound();

            return Json(new { Debt = CustomerDb.customerAccounts.Sum(n => n.Dept) - CustomerDb.customerAccounts.Sum(n => n.Borrower) });
        }

    }
}
