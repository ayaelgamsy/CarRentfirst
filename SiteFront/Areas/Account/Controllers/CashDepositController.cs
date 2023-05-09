using AutoMapper;
using Core.Common.enums;
using Core.Dtos.CashDepositDto;
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

namespace SiteFront.Areas.Account.Controllers
{
    //ايداع

    [Area("Account")]
   // [AllowAnonymous]
    public class CashDepositController : Controller
    {
        private readonly IMapper _Mapper;
        private readonly IRepository<CashDeposit> _CashDepositRepo;
        private readonly IRepository<Stock> _StockRepo;
        private readonly IRepository<StockMovement> _StockMovementRepo;
        private readonly IToastNotification _ToastNotification;
        private readonly UserManager<User> _userManager;

        public CashDepositController(IMapper mapper,
          
            IRepository<CashDeposit> CashDepositRepo,
            IRepository<Stock> StockRepo,
            IRepository<StockMovement> StockMovementRepo,
            IToastNotification toastNotification,
            UserManager<User> userManager
          )
        {
            _Mapper = mapper;
            _CashDepositRepo = CashDepositRepo;
           _StockRepo = StockRepo;
            _StockMovementRepo = StockMovementRepo;
            _ToastNotification = toastNotification;
            _userManager = userManager;
        }

        public List<CashDepositGetDto> AllCashDepositModel { get; set; }
        public CashDepositRegisterDto CashDepositRegistermodel { get; set; }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);


        [Authorize("Permissions.CashDepositIndex")]
        public async Task<IActionResult> Index()
        {
            var AllCashDeposit = await _CashDepositRepo.GetAllAsync(n => n.Stock);
            AllCashDepositModel = _Mapper.Map<List<CashDepositGetDto>>(AllCashDeposit);
            var Stocks = await _StockRepo.GetAllAsync();
            var cashDepositRegisterDto = new CashDepositRegisterDto
            {
                DrpstockDto = _Mapper.Map<List<DrpDto>>(Stocks)
            };
            var cashDepositModelDto = new CashDepositModelDto
            {
                CashDepositGetDtos = AllCashDepositModel,
                CashDepositRegisterDto = cashDepositRegisterDto
            };
            return View(cashDepositModelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.CashDepositCreate")]
        public async Task<IActionResult> Create(CashDepositRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var CashDepositDb = _Mapper.Map<CashDeposit>(model);
                CashDepositDb.CreatedDate = DateTime.Now;
                CashDepositDb.CreatedUser = (GetCurrentUserAsync().Result).Id.ToString();
                _CashDepositRepo.Add(CashDepositDb);

                StockMovement StockMovement = new StockMovement()
                {
                    MovementId = CashDepositDb.Id,
                    MovementType=StockMovementType.CashDeposit,
                    StockId = CashDepositDb.StockId,
                    Date = CashDepositDb.Date,
                    OutValue =0,
                    InValue = CashDepositDb.Value,
                    Notes = CashDepositDb.Notes,
                    Comment = "ايداع مبلغ بواسطه "+" "+ CashDepositDb.name+"وقدرة" +" "+ CashDepositDb.Value,
                };
                _StockMovementRepo.Add(StockMovement);
                await _CashDepositRepo.SaveAllAsync();
                _ToastNotification.AddSuccessToastMessage("تمت الاضافة");
                return RedirectToAction("Index");
            }
            else
            {
                var AllCashDeposit = await _CashDepositRepo.GetAllAsync(n => n.Stock);
                AllCashDepositModel = _Mapper.Map<List<CashDepositGetDto>>(AllCashDeposit);
                var Stocks = await _StockRepo.GetAllAsync();
                var cashDepositRegisterDto = _Mapper.Map<CashDepositRegisterDto>(model);
                cashDepositRegisterDto.DrpstockDto = _Mapper.Map<List<DrpDto>>(Stocks);
                var cashDepositModelDto = new CashDepositModelDto
                {
                    CashDepositGetDtos = AllCashDepositModel,
                    CashDepositRegisterDto = cashDepositRegisterDto
                };
                _ToastNotification.AddErrorToastMessage("بيانات غير صحيحة ");
                return View("Index", cashDepositModelDto);
            }
        }


        [Authorize("Permissions.CashDepositEdit")]
        public async Task<IActionResult> GetData(Guid id)
        {
            var CashDepositById = await _CashDepositRepo.GetByIdAsync(id);
            if (CashDepositById == null)
                return NotFound();
            var cashDepositRegisterDto = _Mapper.Map<CashDepositRegisterDto>(CashDepositById);
            var Stocks = await _StockRepo.GetAllAsync();
            cashDepositRegisterDto.DrpstockDto = _Mapper.Map<List<DrpDto>>(Stocks);
            return PartialView("_PartialCashDeposit", cashDepositRegisterDto);
        }

        //For Movement And Account 
        [Authorize("Permissions.CashDepositEdit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var CashDepositById = await _CashDepositRepo.GetByIdAsync(id);
            if (CashDepositById == null)
                return NotFound();
            var AllCashDeposit = await _CashDepositRepo.GetAllAsync(n => n.Stock);
            AllCashDepositModel = _Mapper.Map<List<CashDepositGetDto>>(AllCashDeposit);
            var Stocks = await _StockRepo.GetAllAsync();
            var cashDepositRegisterDto = _Mapper.Map<CashDepositRegisterDto>(CashDepositById);
            cashDepositRegisterDto.DrpstockDto = _Mapper.Map<List<DrpDto>>(Stocks);
            var cashDepositModelDto = new CashDepositModelDto
            {
                CashDepositGetDtos = AllCashDepositModel,
                CashDepositRegisterDto = cashDepositRegisterDto
            };
            return View("Index", cashDepositModelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.CashDepositEdit")]
        public async Task<IActionResult> Edit(CashDepositRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var CashDepositById = await _CashDepositRepo.GetByIdAsync((Guid)model.Id);
                if (CashDepositById == null)
                    return NotFound();
                var userAdd = CashDepositById.CreatedUser;
                var userAddDate = CashDepositById.CreatedDate;
                var CashDepositEditedDb = _Mapper.Map(model, CashDepositById);
                CashDepositEditedDb.CreatedDate = userAddDate;
                CashDepositEditedDb.CreatedUser = userAdd;
                CashDepositEditedDb.LastEditDate = DateTime.Now;
                CashDepositEditedDb.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                _CashDepositRepo.Update(CashDepositEditedDb);
                var StockMovementById = await _StockMovementRepo.SingleOrDefaultAsync(n => n.MovementId == (Guid)model.Id && n.MovementType==StockMovementType.CashDeposit);

                StockMovementById.Date = CashDepositEditedDb.Date;
                StockMovementById.StockId = CashDepositEditedDb.StockId;
                StockMovementById.Date = CashDepositEditedDb.Date;
                StockMovementById.InValue = CashDepositEditedDb.Value;
                StockMovementById.Notes = CashDepositEditedDb.Notes;
                StockMovementById.Comment = "ايداع مبلغ بواسطه " +" "+ CashDepositEditedDb.name + "وقدرة" +" "+ CashDepositEditedDb.Value;

                _StockMovementRepo.Update(StockMovementById);
                await _CashDepositRepo.SaveAllAsync();
                _ToastNotification.AddSuccessToastMessage("تم التعديل");
                return RedirectToAction("Index");
            }
            else
            {
                var AllCashDeposit = await _CashDepositRepo.GetAllAsync(n => n.Stock);
                AllCashDepositModel = _Mapper.Map<List<CashDepositGetDto>>(AllCashDeposit);
                var Stocks = await _StockRepo.GetAllAsync();
                var cashDepositRegisterDto = _Mapper.Map<CashDepositRegisterDto>(model);
                cashDepositRegisterDto.DrpstockDto = _Mapper.Map<List<DrpDto>>(Stocks);
                var cashDepositModelDto = new CashDepositModelDto
                {
                    CashDepositGetDtos = AllCashDepositModel,
                    CashDepositRegisterDto = cashDepositRegisterDto
                };
                _ToastNotification.AddErrorToastMessage("بيانات غير صحيحة ");
                return View("Index",cashDepositModelDto);
            }
        }

        [Authorize("Permissions.CashDepositDelete")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var StockMovement = await _StockMovementRepo.SingleOrDefaultAsync(n => n.MovementId == Id&& n.MovementType==StockMovementType.CashDeposit);
            _StockMovementRepo.Delete(StockMovement);

            var CashDepositById = await _CashDepositRepo.GetByIdAsync(Id);
            _CashDepositRepo.Delete(CashDepositById);

            await _CashDepositRepo.SaveAllAsync();
            _ToastNotification.AddSuccessToastMessage("تم الحذف");

            return RedirectToAction("Index");
        }


    }
}
