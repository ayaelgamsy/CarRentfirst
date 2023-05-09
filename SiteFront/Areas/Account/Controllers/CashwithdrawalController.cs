using AutoMapper;
using Core.Common.enums;
using Core.Dtos.CashwithdrawalDto;
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
    //سحب

    [Area("Account")]
  //  [AllowAnonymous]
    public class CashwithdrawalController : Controller
    {
        private readonly IMapper _Mapper;
        private readonly IRepository<Cashwithdrawal> _CashwithdrawalRepo;
        private readonly IRepository<Stock> _StockRepo;
        private readonly IRepository<StockMovement> _StockMovementRepo;
        private readonly IToastNotification _ToastNotification;
        private readonly UserManager<User> _userManager;

        public CashwithdrawalController(IMapper mapper,

            IRepository<Cashwithdrawal> CashwithdrawalRepo,
            IRepository<Stock> StockRepo,
            IRepository<StockMovement> StockMovementRepo,
            IToastNotification toastNotification,
            UserManager<User> userManager
          )
        {
            _Mapper = mapper;
            _CashwithdrawalRepo = CashwithdrawalRepo;
            _StockRepo = StockRepo;
            _StockMovementRepo = StockMovementRepo;
            _ToastNotification = toastNotification;
            _userManager = userManager;
        }


        public List<CashwithdrawalGetDto> AllCashwithdrawalModel { get; set; }
        public CashwithdrawalRegisterDto CashwithdrawalRegistermodel { get; set; }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize("Permissions.CashwithdrawalIndex")]
        public async Task<IActionResult> Index()
        {
            var AllCashwithdrawal = await _CashwithdrawalRepo.GetAllAsync(n => n.Stock);
            AllCashwithdrawalModel = _Mapper.Map<List<CashwithdrawalGetDto>>(AllCashwithdrawal);
            var Stocks = await _StockRepo.GetAllAsync();
            var cashwithdrawalRegisterDto = new CashwithdrawalRegisterDto
            {
                DrpstockDto = _Mapper.Map<List<DrpDto>>(Stocks)
            };
            var cashwithdrawalModelDto = new CashWithdrawalModelDto
            {
               CashwithdrawalGetDtos= AllCashwithdrawalModel,
               CashwithdrawalRegisterDto= cashwithdrawalRegisterDto
            };
            return View(cashwithdrawalModelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.CashwithdrawalCreate")]
        public async Task<IActionResult> Create(CashwithdrawalRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var CashwithdrawalDb = _Mapper.Map<Cashwithdrawal>(model);
                CashwithdrawalDb.CreatedDate = DateTime.Now;
                CashwithdrawalDb.CreatedUser = (GetCurrentUserAsync().Result).Id.ToString();
                _CashwithdrawalRepo.Add(CashwithdrawalDb);
                StockMovement StockMovement = new StockMovement()
                {
                    MovementId = CashwithdrawalDb.Id,
                    MovementType=StockMovementType.Cashwithdrawal,
                    StockId = CashwithdrawalDb.StockId,
                    Date = CashwithdrawalDb.Date,
                    OutValue = CashwithdrawalDb.Value,
                    InValue = 0,
                    Notes = CashwithdrawalDb.Notes,
                    Comment = "سحب مبلغ بواسطه " +" "+ CashwithdrawalDb.name + "وقدرة"+" " + CashwithdrawalDb.Value,
                };
                _StockMovementRepo.Add(StockMovement);
                await _CashwithdrawalRepo.SaveAllAsync();
                _ToastNotification.AddSuccessToastMessage("تمت الاضافة");
                return RedirectToAction("Index");
            }
            else
            {
                var AllCashwithdrawal = await _CashwithdrawalRepo.GetAllAsync(n => n.Stock);
                AllCashwithdrawalModel = _Mapper.Map<List<CashwithdrawalGetDto>>(AllCashwithdrawal);
                var Stocks = await _StockRepo.GetAllAsync();
                var cashwithdrawalRegisterDto = _Mapper.Map<CashwithdrawalRegisterDto>(model);
                cashwithdrawalRegisterDto.DrpstockDto = _Mapper.Map<List<DrpDto>>(Stocks);
                var cashwithdrawalModelDto = new CashWithdrawalModelDto
                {
                    CashwithdrawalGetDtos = AllCashwithdrawalModel,
                    CashwithdrawalRegisterDto = cashwithdrawalRegisterDto
                };
                _ToastNotification.AddErrorToastMessage("بيانات غير صحيحة ");
                return View("Index",cashwithdrawalModelDto);
            }
        }

        [Authorize("Permissions.CashwithdrawalEdit")]
        public async Task<IActionResult> GetData(Guid id)
        {
            var CashwithdrawalById = await _CashwithdrawalRepo.GetByIdAsync(id);
            if (CashwithdrawalById == null)
                return NotFound();
            var cashwithdrawalRegisterDto = _Mapper.Map<CashwithdrawalRegisterDto>(CashwithdrawalById);
            var Stocks = await _StockRepo.GetAllAsync();
            cashwithdrawalRegisterDto.DrpstockDto = _Mapper.Map<List<DrpDto>>(Stocks);
            return PartialView("_PartialCashwithdrawal", cashwithdrawalRegisterDto);
        }

        //For Movement And Account 
        [Authorize("Permissions.CashwithdrawalEdit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var CashwithdrawalById = await _CashwithdrawalRepo.GetByIdAsync(id);
            if (CashwithdrawalById == null)
                return NotFound();
            var AllCashwithdrawal = await _CashwithdrawalRepo.GetAllAsync(n => n.Stock);
            AllCashwithdrawalModel = _Mapper.Map<List<CashwithdrawalGetDto>>(AllCashwithdrawal);
            var Stocks = await _StockRepo.GetAllAsync();
            var cashwithdrawalRegisterDto = _Mapper.Map<CashwithdrawalRegisterDto>(CashwithdrawalById);
            cashwithdrawalRegisterDto.DrpstockDto = _Mapper.Map<List<DrpDto>>(Stocks);
            var cashwithdrawalModelDto = new CashWithdrawalModelDto
            {
                CashwithdrawalGetDtos = AllCashwithdrawalModel,
                CashwithdrawalRegisterDto = cashwithdrawalRegisterDto
            };
            return View("Index",cashwithdrawalModelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.CashwithdrawalEdit")]
        public async Task<IActionResult> Edit(CashwithdrawalRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var CashwithdrawalById = await _CashwithdrawalRepo.GetByIdAsync((Guid)model.Id);
                if (CashwithdrawalById == null)
                    return NotFound();
                var userAdd = CashwithdrawalById.CreatedUser;
                var userAddDate = CashwithdrawalById.CreatedDate;
                var CashwithdrawalEditedDb = _Mapper.Map(model, CashwithdrawalById);
                CashwithdrawalEditedDb.CreatedDate = userAddDate;
                CashwithdrawalEditedDb.CreatedUser = userAdd;
                CashwithdrawalEditedDb.LastEditDate = DateTime.Now;
                CashwithdrawalEditedDb.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                _CashwithdrawalRepo.Update(CashwithdrawalEditedDb);
                var StockMovementById = await _StockMovementRepo.SingleOrDefaultAsync(n => n.MovementId == (Guid)model.Id && n.MovementType==StockMovementType.Cashwithdrawal);

                StockMovementById.Date = CashwithdrawalEditedDb.Date;
                StockMovementById.StockId = CashwithdrawalEditedDb.StockId;
                StockMovementById.Date = CashwithdrawalEditedDb.Date;
                StockMovementById.OutValue = CashwithdrawalEditedDb.Value;
                StockMovementById.Notes = CashwithdrawalEditedDb.Notes;
                StockMovementById.Comment = "سحب مبلغ بواسطه "+" " + CashwithdrawalEditedDb.name + "وقدرة"+" " + CashwithdrawalEditedDb.Value;

                _StockMovementRepo.Update(StockMovementById);
                await _CashwithdrawalRepo.SaveAllAsync();
                _ToastNotification.AddSuccessToastMessage("تم التعديل");
                return RedirectToAction("Index");
            }
            else
            {
                var AllCashwithdrawal = await _CashwithdrawalRepo.GetAllAsync(n => n.Stock);
                AllCashwithdrawalModel = _Mapper.Map<List<CashwithdrawalGetDto>>(AllCashwithdrawal);
                var Stocks = await _StockRepo.GetAllAsync();
                var cashwithdrawalRegisterDto = _Mapper.Map<CashwithdrawalRegisterDto>(model);
                cashwithdrawalRegisterDto.DrpstockDto = _Mapper.Map<List<DrpDto>>(Stocks);
                var cashwithdrawalModelDto = new CashWithdrawalModelDto
                {
                    CashwithdrawalGetDtos = AllCashwithdrawalModel,
                    CashwithdrawalRegisterDto = cashwithdrawalRegisterDto
                };
                _ToastNotification.AddErrorToastMessage("بيانات غير صحيحة ");
                return View("Index",cashwithdrawalModelDto);
            }
        }

        [Authorize("Permissions.CashwithdrawalDelete")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var StockMovement = await _StockMovementRepo.SingleOrDefaultAsync(n => n.MovementId == Id && n.MovementType==StockMovementType.Cashwithdrawal);
            _StockMovementRepo.Delete(StockMovement);

            var CashwithdrawalById = await _CashwithdrawalRepo.GetByIdAsync(Id);
            _CashwithdrawalRepo.Delete(CashwithdrawalById);

            await _CashwithdrawalRepo.SaveAllAsync();
            _ToastNotification.AddSuccessToastMessage("تم الحذف");

            return RedirectToAction("Index");
        }


    }
}