using AutoMapper;
using Core.Dtos.CustomerEvaluationDto;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteFront.Areas.Rent.Controllers
{
    [Area("Rent")]
   // [AllowAnonymous]
    public class CustomerEvaluationController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IToastNotification _toastNotification;
        private readonly IRepository<CustomerEvaluation> _customerEvaluationRepo;
        private readonly IRepository<CustomerRent> _customerRentRepo;
        private readonly IRepository<Customer> _customerRepo;

        public CustomerEvaluationController(IMapper mapper,
            IToastNotification toastNotification,
            IRepository<CustomerEvaluation> customerEvaluationRepo,
            IRepository<CustomerRent> customerRentRepo,
            IRepository<Customer> customerRepo)
        {
            _mapper = mapper;
            _toastNotification = toastNotification;
            _customerEvaluationRepo = customerEvaluationRepo;
           _customerRentRepo = customerRentRepo;
            _customerRepo = customerRepo;
        }

        [Authorize("Permissions.CustomerEvaluationIndex")]
        public async Task<IActionResult> Index()
        {
            var customerEvaluationData =await _customerEvaluationRepo.GetAllAsync();
            var customerEvaluationGetDto = _mapper.Map<List<CustomerEvaluationGetDto>>(customerEvaluationData);
            var customerEvaluationModelDto = new CustomerEvaluationModelDto
            {
                CustomerEvaluationRegisterDto = new CustomerEvaluationRegisterDto(),
                CustomerEvaluationGetDtos = customerEvaluationGetDto
            };
            return View(customerEvaluationModelDto);
        }

        [Authorize("Permissions.CustomerEvaluationCreate")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerEvaluationRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var customerEvaluationDb = _mapper.Map<CustomerEvaluation>(model);
                _customerEvaluationRepo.Add(customerEvaluationDb);
                await _customerEvaluationRepo.SaveAllAsync();
                _toastNotification.AddSuccessToastMessage("تمت الاضافة");
                return RedirectToAction("Index");
            }

            else
            {
                var customerEvaluationData = await _customerEvaluationRepo.GetAllAsync();
                var customerEvaluationGetDto = _mapper.Map<List<CustomerEvaluationGetDto>>(customerEvaluationData);
                var customerEvaluationModelDto = new CustomerEvaluationModelDto
                {
                    CustomerEvaluationRegisterDto = new CustomerEvaluationRegisterDto(),
                    CustomerEvaluationGetDtos = customerEvaluationGetDto
                };
                return View("Index",customerEvaluationModelDto);
            }
        }

        [Authorize("Permissions.CustomerEvaluationEdit")]
        public async Task<IActionResult> GetData(Guid id)
        {
            var customerEvaluation = await _customerEvaluationRepo.GetByIdAsync(id);
            var customerEvaluationRegisterDto = _mapper.Map<CustomerEvaluationRegisterDto>(customerEvaluation);
            return PartialView("_PartialCustomerEvaluation", customerEvaluationRegisterDto);
        }

        [Authorize("Permissions.CustomerEvaluationEdit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerEvaluationRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var customerEvaluation = await _customerEvaluationRepo.GetByIdAsync((Guid)model.Id);
                var customerEvaluationDb = _mapper.Map(model, customerEvaluation);
                _customerEvaluationRepo.Update(customerEvaluationDb);
                await _customerEvaluationRepo.SaveAllAsync();
                _toastNotification.AddSuccessToastMessage(" تم التعديل");
                return RedirectToAction("Index");
            }

            else
            {
                var customerEvaluationData = await _customerEvaluationRepo.GetAllAsync();
                var customerEvaluationGetDto = _mapper.Map<List<CustomerEvaluationGetDto>>(customerEvaluationData);
                var customerEvaluationModelDto = new CustomerEvaluationModelDto
                {
                    CustomerEvaluationRegisterDto = model,
                    CustomerEvaluationGetDtos = customerEvaluationGetDto
                };
                return View("Index", customerEvaluationModelDto);
            }
        }

        [Authorize("Permissions.CustomerEvaluationDelete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var customerEvaluation = await _customerEvaluationRepo.GetByIdAsync(id);
            var customerRent = await _customerRentRepo.GetAllAsync(c => c.CustomerEvaluationId == id);
            var customer = await _customerRepo.GetAllAsync(c => c.CustomerEvaluationId == id);
            if(customerRent.Count()!= 0 || customer.Count() != 0)
            {
                _toastNotification.AddErrorToastMessage("لا يمكن حذف هذا التقييم");
            }
            else
            {
                _customerEvaluationRepo.Delete(customerEvaluation);
                await _customerEvaluationRepo.SaveAllAsync();
                _toastNotification.AddSuccessToastMessage(" تم الحذف");
            }
            return RedirectToAction("Index");
        }
    }
}
