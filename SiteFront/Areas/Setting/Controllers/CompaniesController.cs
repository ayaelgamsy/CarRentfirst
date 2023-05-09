using AutoMapper;
using Core.Dtos;
using Core.Dtos.CompaniesDto;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteFront.Areas.Setting.Controllers
{
    [Area("Setting")]
    //[AllowAnonymous]
    public class CompaniesController : Controller
    {
        private readonly IRepository<Car> carRepo;
        private readonly UserManager<User> _userManager;

        public CompaniesController(
            IRepository<Company> CompanyRepo,
            IMapper mapper,
            IToastNotification toastNotification,
            IRepository<Car> CarRepo,
            UserManager<User> userManager)
        {
            _mapper = mapper;
            _toastNotification = toastNotification;
            carRepo = CarRepo;
            _userManager = userManager;
            _Company = CompanyRepo;
            _CompanyRepo = CompanyRepo;
        }

        public IMapper _mapper { get; private set; }
        public IToastNotification _toastNotification { get; private set; }
        public IRepository<Company> _Company { get; private set; }
        public IRepository<Company> _CompanyRepo { get; private set; }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize("Permissions.CompaniesIndex")]
        public async Task<IActionResult> IndexAsync()
        {
            var company = await _CompanyRepo.GetAllAsync();

            return View(_mapper.Map<List<CompanyGetDto>>(company));
        }

        [Authorize("Permissions.CompaniesCreate")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(CompanyRegisterDto companyRegister)
        {
            if (ModelState.IsValid)
            {
                var Company = _mapper.Map<Company>(companyRegister);
                Company.CreatedDate = DateTime.Now;
                Company.CreatedUser = (GetCurrentUserAsync().Result).Id.ToString();
                _CompanyRepo.Add(Company);
                await _CompanyRepo.SaveAllAsync();

                _toastNotification.AddSuccessToastMessage("تمت الاضافة");
                var company = await _CompanyRepo.GetAllAsync();
                return RedirectToAction("Index", _mapper.Map<List<CompanyGetDto>>(company));
            }
            else
            {
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                var company = await _CompanyRepo.GetAllAsync();
                return View("Index", _mapper.Map<List<CompanyGetDto>>(company));
            }

        }

        [Authorize("Permissions.CompaniesEdit")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var company = await _CompanyRepo.GetByIdAsync(id);

            return PartialView("_PartialAddCopmany", _mapper.Map<CompanyRegisterDto>(company));
        }

        [Authorize("Permissions.CompaniesEdit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CompanyRegisterDto companyRegister)
        {
            if (ModelState.IsValid)
            {
                var Company = await _CompanyRepo.GetByIdAsync((Guid)companyRegister.Id);
                var userAdd = Company.CreatedUser;
                var userAddDate = Company.CreatedDate;
                var newCompany = _mapper.Map(companyRegister, Company);
                newCompany.CreatedDate = userAddDate;
                newCompany.CreatedUser = userAdd;
                newCompany.LastEditDate = DateTime.Now;
                newCompany.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                _CompanyRepo.Update(newCompany);
                await _CompanyRepo.SaveAllAsync();
                _toastNotification.AddSuccessToastMessage("تمت التعديل");

            }
            else
            {
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
            }
            var company = await _CompanyRepo.GetAllAsync();
            return RedirectToAction("Index");
        }

        [Authorize("Permissions.CompaniesDelete")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {

            var company = await _CompanyRepo.GetByIdAsync(id);
            var CarById = await carRepo.GetAllAsync(c => c.CompanyId == id);
            if (CarById.Count() != 0)
                _toastNotification.AddErrorToastMessage(" لا يمكن حذف هذه الشركة ");   
            else
            {
                _CompanyRepo.Delete(company);
                await _CompanyRepo.SaveAllAsync();
                _toastNotification.AddSuccessToastMessage("تم الحذف");
                
            }
            return RedirectToAction("Index");
        }
    }
}
