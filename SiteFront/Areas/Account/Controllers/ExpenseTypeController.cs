using AutoMapper;
using Core.Dtos.ExpenseTypeDto;
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

namespace SiteFront.Areas.Account.Controllers
{

    [Area("Account")]
   // [AllowAnonymous]
    public class ExpenseTypeController : Controller
    {
        private readonly IMapper _Mapper;
        private readonly IRepository<ExpenseType> _ExpenseTypeRepo;
        private readonly IRepository<Expense> _ExpenseRepo;
        private readonly IToastNotification _ToastNotification;
        private readonly UserManager<User> _userManager;

        public ExpenseTypeController(IMapper mapper,
            IRepository<ExpenseType> ExpenseTypeRepo,
            IRepository<Expense> ExpenseRepo,
            IToastNotification toastNotification,
            UserManager<User> userManager
          )
        {
            _Mapper = mapper;
            _ExpenseTypeRepo = ExpenseTypeRepo;
            _ExpenseRepo = ExpenseRepo;
            _ToastNotification = toastNotification;
            _userManager = userManager;
        }

        public ExpenseTypeRegisterDto ExpenseTypeModel { get; set; }

        public List<ExpenseTypeGetDto> ExpenseTypeData { get; set; }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize("Permissions.ExpenseTypeIndex")]
        public async Task<IActionResult> Index(Guid ? Id)
        {

            var AllExpensesData = await _ExpenseTypeRepo.GetAllAsync();

            ExpenseTypeData = _Mapper.Map<List<ExpenseTypeGetDto>>(AllExpensesData);

            if (Id == null)
            {
                ExpenseTypeModel = new ExpenseTypeRegisterDto()
                {
                    ExpenseTypeGetDtos= ExpenseTypeData,
                };
            }
            else
            {
                var ExpensesDbById = await _ExpenseTypeRepo.GetByIdAsync((Guid)Id);

                ExpenseTypeModel = _Mapper.Map<ExpenseTypeRegisterDto>(ExpensesDbById);
                ExpenseTypeModel.ExpenseTypeGetDtos = ExpenseTypeData;

            }

            return View(ExpenseTypeModel);
        }



        [Authorize("Permissions.ExpenseTypeCreate")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseTypeRegisterDto Model)
        {
            if (ModelState.IsValid)
            {
                var ExpensesDb = _Mapper.Map<ExpenseType>(Model);
                ExpensesDb.CreatedDate = DateTime.Now;
                ExpensesDb.CreatedUser = (GetCurrentUserAsync().Result).Id.ToString();
                _ExpenseTypeRepo.Add(ExpensesDb);
                await _ExpenseTypeRepo.SaveAllAsync();
                _ToastNotification.AddSuccessToastMessage("تمت الاضافة");
                return RedirectToAction("Index");
            }
            else
            {
                var AllExpensesData = await _ExpenseTypeRepo.GetAllAsync();
                ExpenseTypeData = _Mapper.Map<List<ExpenseTypeGetDto>>(AllExpensesData);
                Model.ExpenseTypeGetDtos = ExpenseTypeData;
                return View(Model);
            }
          
        }

        [Authorize("Permissions.ExpenseTypeEdit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit (ExpenseTypeRegisterDto Model)
        {
            if (ModelState.IsValid)
            {
                var ExpensisbyId = await _ExpenseTypeRepo.GetByIdAsync((Guid)Model.Id);

                if(ExpensisbyId == null)
                    return NotFound();
                var userAdd = ExpensisbyId.CreatedUser;
                var userAddDate = ExpensisbyId.CreatedDate;
                var  ExpenseTypeMapped = _Mapper.Map(Model, ExpensisbyId);
                ExpenseTypeMapped.CreatedDate = userAddDate;
                ExpenseTypeMapped.CreatedUser = userAdd;
                ExpenseTypeMapped.LastEditDate = DateTime.Now;
                ExpenseTypeMapped.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                _ExpenseTypeRepo.Update(ExpenseTypeMapped);
                    await _ExpenseTypeRepo.SaveAllAsync();
                   _ToastNotification.AddSuccessToastMessage("تم تعديل المصروف");
              
                return RedirectToAction("Index");


            }
            else
            {
                var AllExpensesData = await _ExpenseTypeRepo.GetAllAsync();
                ExpenseTypeData = _Mapper.Map<List<ExpenseTypeGetDto>>(AllExpensesData);
                Model.ExpenseTypeGetDtos = ExpenseTypeData;
                return View(Model);
            }

        }
        [Authorize("Permissions.ExpenseTypeDelete")]

        public async Task<IActionResult> Delete(Guid Id)
        {
            var ExpenseTypeById = await _ExpenseTypeRepo.GetByIdAsync(Id);

            var ExpenseRepoByExpenseTypeById = await _ExpenseRepo.GetAllAsync(n => n.ExpenseTypeId == Id );

            if (ExpenseRepoByExpenseTypeById.Count() == 0)
            {
                _ExpenseTypeRepo.Delete(ExpenseTypeById);
                await _ExpenseTypeRepo.SaveAllAsync();
                _ToastNotification.AddSuccessToastMessage("تم الحذف");
            }
            else
            {
                _ToastNotification.AddSuccessToastMessage(" لايمكن حزف هذا المصروف ");

            }

            return RedirectToAction("Index");



        }



    }

   



}
