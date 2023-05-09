using AutoMapper;
using Core.Dtos.GovernmentDto;
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

namespace SiteFront.Areas.Setting.Controllers
{
    [Area("Setting")]
   // [AllowAnonymous]
    public class GovernmentController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Government> _repository;
        private readonly IToastNotification _toastNotification;
        private readonly IRepository<Customer> custRepo;
        private readonly IRepository<Employee> employeeRepo;
        private readonly IRepository<CarOwner> carOwnerRepo;
        private readonly UserManager<User> _userManager;

        public GovernmentController(IMapper mapper,
            IRepository<Government> repository,
            IToastNotification toastNotification,
            IRepository<Customer> CustRepo,
            IRepository<Employee> EmployeeRepo,
            IRepository<CarOwner> CarOwnerRepo,
            UserManager<User> userManager)
        {
            _mapper = mapper;
            _repository = repository;
            _toastNotification = toastNotification;
            custRepo = CustRepo;
            employeeRepo = EmployeeRepo;
            carOwnerRepo = CarOwnerRepo;
            _userManager = userManager;
        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize("Permissions.GovernmentIndex")]
        public async Task<IActionResult> Index()
        {
            var GoverData = await _repository.GetAllAsync();
            var GovernmentGetDto = _mapper.Map<List<GovernmentGetDto>>(GoverData);
            return View(GovernmentGetDto);
        }

        [Authorize("Permissions.GovernmentCreate")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GovernmentRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var GovernmentDb = _mapper.Map<Government>(model);
                GovernmentDb.CreatedDate = DateTime.Now;
                GovernmentDb.CreatedUser = (GetCurrentUserAsync().Result).Id.ToString();
                _repository.Add(GovernmentDb);
                await _repository.SaveAllAsync();
                _toastNotification.AddSuccessToastMessage("تمت الاضافة");

                return RedirectToAction("Index");

            }

            return View("_PartialGovernment", model);
        }
       
        [Authorize("Permissions.GovernmentEdit")]
        public async Task<IActionResult> GetData(Guid id)
        {
            var GovernmentById = await _repository.GetByIdAsync(id);
            if (GovernmentById == null)
                return NotFound();
            var GovernmentRegisterDto = _mapper.Map<GovernmentRegisterDto>(GovernmentById);
            return PartialView("_PartialGovernment", GovernmentRegisterDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.GovernmentEdit")]
        public async Task<IActionResult> Edit(GovernmentRegisterDto model)
        {
            if (!ModelState.IsValid)
                return PartialView("_PartialGovernment", model);
            var GovernmentById = await _repository.GetByIdAsync((Guid)model.Id);
            if (GovernmentById == null)
                return NotFound();
            var userAdd = GovernmentById.CreatedUser;
            var userAddDate = GovernmentById.CreatedDate;
            var GovernmentEditedDb = _mapper.Map(model, GovernmentById);
            GovernmentEditedDb.CreatedDate = userAddDate;
            GovernmentEditedDb.CreatedUser = userAdd;
            GovernmentEditedDb.LastEditDate = DateTime.Now;
            GovernmentEditedDb.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();

            _repository.Update(GovernmentEditedDb);
            await _repository.SaveAllAsync();
            _toastNotification.AddSuccessToastMessage("تم التعديل");

            return RedirectToAction("Index");
        }

        [Authorize("Permissions.GovernmentDelete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var GovernmentById = await _repository.GetByIdAsync(id);
            var CustomerById = await custRepo.GetAllAsync(c => c.GovernmentId == id);
            var EmployeeById = await employeeRepo.GetAllAsync(e => e.GovernmentId == id);
            var CarOwnerById = await carOwnerRepo.GetAllAsync(c => c.GovernmentId == id);
            if (CustomerById.Count() != 0 || EmployeeById.Count() != 0 || CarOwnerById.Count() != 0)
                _toastNotification.AddErrorToastMessage("لا يمكن حذف هذه المحافظة");

            else
            {
                _repository.Delete(GovernmentById);
                await _repository.SaveAllAsync();
                _toastNotification.AddSuccessToastMessage("تم الحذف");
               
            }
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetId(string code)
        {
            var GovernmentByCode = await _repository.SingleOrDefaultAsync(g => g.Code == code);
            if (GovernmentByCode != null)
                return Json(new { id = GovernmentByCode.Id });
            else 
                return NotFound();
        }
    }
}
