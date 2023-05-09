using AutoMapper;
using Core.Common.enums;
using Core.Dtos.CustomerDeptDto;
using Core.Dtos.ExpenseDto;
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
    public class CustomerDeptController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IToastNotification _toastNotification;
        private readonly IRepository<CustomerLastDept> _customerDeptRepo;
        private readonly IRepository<Customer> _customerRepo;
        private readonly IRepository<CustomerAccount> _CustomerAccountRepo;
        private readonly UserManager<User> _userManager;

        public CustomerDeptController(IMapper mapper,
            IToastNotification toastNotification,
            IRepository<CustomerLastDept> customerDeptRepo,
            IRepository<Customer> customerRepo,
            IRepository<CustomerAccount> CustomerAccountRepo,
            UserManager<User> userManager)
        {
            _mapper = mapper;
            _toastNotification = toastNotification;
            _customerDeptRepo = customerDeptRepo;
            _customerRepo = customerRepo;
            _CustomerAccountRepo = CustomerAccountRepo;
            _userManager = userManager;
        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize("Permissions.CustomerDeptIndex")]
        public async Task<IActionResult> Index()
        {
            var CustomerDeptData = await _customerDeptRepo.GetAllAsync(c=>c.Customer);
            var customerDeptGetDto = _mapper.Map<List<CustomerDeptGetDto>>(CustomerDeptData);
            var customers = await _customerRepo.GetAllAsync();
            var customerDeptRegisterDto = new CustomerDeptRegisterDto
            {
                Customers = _mapper.Map<List<DrpDto>>(customers)
            };
            var customerDeptModelDto = new CustomerDeptModelDto
            {
                CustomerDeptGetDtos = customerDeptGetDto,
                CustomerDeptRegisterDto = customerDeptRegisterDto
            };
            return View(customerDeptModelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.CustomerDeptCreate")]

        public async Task<IActionResult> Create(CustomerDeptRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var CustomerDeptDb = _mapper.Map<CustomerLastDept>(model);
                CustomerDeptDb.CreatedDate = DateTime.Now;
                CustomerDeptDb.CreatedUser = (GetCurrentUserAsync().Result).Id.ToString();
                _customerDeptRepo.Add(CustomerDeptDb);
                var customer = _customerRepo.SingleOrDefaultAsync(c => c.Id == CustomerDeptDb.CustomerId).Result.name;
                var customerAccount = new CustomerAccount
                {
                    CustomerId = CustomerDeptDb.CustomerId,
                    Dept = CustomerDeptDb.DebtValue,
                    MovementId = CustomerDeptDb.Id,
                    AccountType = RentAccountType.LastDept,
                    Explain = "مديونية العميل" + " " + customer,
                    Date = CustomerDeptDb.Date,
                    Notes = "مديونية العميل" + " " + customer,
                };
                _CustomerAccountRepo.Add(customerAccount);
                await _customerDeptRepo.SaveAllAsync();
                _toastNotification.AddSuccessToastMessage("تمت الاضافة");
                return RedirectToAction("Index");
            }
            else
            {
                var CustomerDeptData = await _customerDeptRepo.GetAllAsync(c => c.Customer);
                var customerDeptGetDto = _mapper.Map<List<CustomerDeptGetDto>>(CustomerDeptData);
                var customers = await _customerRepo.GetAllAsync();
                var customerDeptRegisterDto = _mapper.Map<CustomerDeptRegisterDto>(model);
                customerDeptRegisterDto.Customers = _mapper.Map<List<DrpDto>>(customers);
                var customerDeptModelDto = new CustomerDeptModelDto
                {
                    CustomerDeptGetDtos = customerDeptGetDto,
                    CustomerDeptRegisterDto = customerDeptRegisterDto
                };
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                return View("Index",customerDeptModelDto);
            }
        }

        [Authorize("Permissions.CustomerDeptEdit")]
        public async Task<IActionResult> GetData(Guid id)
        {
            var CustomerDeptById = await _customerDeptRepo.GetByIdAsync(id);
            var customerDeptRegisterDto = _mapper.Map<CustomerDeptRegisterDto>(CustomerDeptById);
            var customers = await _customerRepo.GetAllAsync();
            customerDeptRegisterDto.Customers = _mapper.Map<List<DrpDto>>(customers);
            return PartialView("_PartialCustomerDept", customerDeptRegisterDto);
        }

        //For Movement And Account 
        [Authorize("Permissions.CustomerDeptEdit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var CustomerDeptById = await _customerDeptRepo.GetByIdAsync(id);
            var CustomerDeptData = await _customerDeptRepo.GetAllAsync(c => c.Customer);
            var customerDeptGetDto = _mapper.Map<List<CustomerDeptGetDto>>(CustomerDeptData);
            var customerDeptRegisterDto = _mapper.Map<CustomerDeptRegisterDto>(CustomerDeptById);
            var customers = await _customerRepo.GetAllAsync();
            customerDeptRegisterDto.Customers = _mapper.Map<List<DrpDto>>(customers);
            var customerDeptModelDto = new CustomerDeptModelDto
            {
                CustomerDeptGetDtos = customerDeptGetDto,
                CustomerDeptRegisterDto = customerDeptRegisterDto
            };
            return View("Index", customerDeptModelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.CustomerDeptEdit")]
        public async Task<IActionResult> Edit(CustomerDeptRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var CustomerDeptById = await _customerDeptRepo.GetByIdAsync((Guid)model.Id);
                var userAdd = CustomerDeptById.CreatedUser;
                var userAddDate = CustomerDeptById.CreatedDate;
                var CustomerDeptDb = _mapper.Map(model, CustomerDeptById);
                CustomerDeptDb.CreatedDate = userAddDate;
                CustomerDeptDb.CreatedUser = userAdd;
                CustomerDeptDb.LastEditDate = DateTime.Now;
                CustomerDeptDb.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                var customer = _customerRepo.SingleOrDefaultAsync(c => c.Id == CustomerDeptDb.CustomerId).Result.name;
                var customerAccountById = await _CustomerAccountRepo.SingleOrDefaultAsync(c => c.MovementId == (Guid)model.Id && c.AccountType == RentAccountType.LastDept);

                customerAccountById.CustomerId = CustomerDeptDb.CustomerId;
                customerAccountById.Dept = CustomerDeptDb.DebtValue;
                customerAccountById.MovementId = CustomerDeptDb.Id;
                customerAccountById.AccountType = RentAccountType.LastDept;
                customerAccountById.Explain = "مديونية العميل" + " " + customer;
                customerAccountById.Date = CustomerDeptDb.Date;
                customerAccountById.Notes = "مديونية العميل" + " " + customer;
                _CustomerAccountRepo.Update(customerAccountById);
                _customerDeptRepo.Update(CustomerDeptDb);
                await _customerDeptRepo.SaveAllAsync();
                _toastNotification.AddSuccessToastMessage(" تم التعديل");
                return RedirectToAction("Index");
            }
            else
            {
                var CustomerDeptData = await _customerDeptRepo.GetAllAsync(c => c.Customer);
                var customerDeptGetDto = _mapper.Map<List<CustomerDeptGetDto>>(CustomerDeptData);
                var customers = await _customerRepo.GetAllAsync();
                var customerDeptRegisterDto = _mapper.Map<CustomerDeptRegisterDto>(model);
                customerDeptRegisterDto.Customers = _mapper.Map<List<DrpDto>>(customers);
                var customerDeptModelDto = new CustomerDeptModelDto
                {
                    CustomerDeptGetDtos = customerDeptGetDto,
                    CustomerDeptRegisterDto = customerDeptRegisterDto
                };
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                return View("Index", customerDeptModelDto);
            }
        }
        [Authorize("Permissions.CustomerDeptDelete")]

        public async Task<IActionResult> Delete(Guid id)
        {
            var CustomerDeptById = await _customerDeptRepo.GetByIdAsync(id);
            if (CustomerDeptById == null)
                return NotFound();
            var customerAccountById = await _CustomerAccountRepo.SingleOrDefaultAsync(c => c.MovementId == CustomerDeptById.Id && c.AccountType == RentAccountType.LastDept);
            _CustomerAccountRepo.Delete(customerAccountById);
            _customerDeptRepo.Delete(CustomerDeptById);
            await _customerDeptRepo.SaveAllAsync();
            return Ok();
        }
    }
}
