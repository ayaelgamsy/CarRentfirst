using AutoMapper;
using Core.Common.enums;
using Core.Dtos.ExpenseDto;
using Core.Dtos.StockTransferDto;
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
    public class StockTransferController : Controller
    {
        private readonly IMapper _Mapper;
        private readonly IRepository<StockTransfer> _StockTransferRepo;
        private readonly IRepository<Stock> _StockRepo;
        private readonly IRepository<StockMovement> _StockMovementRepo;
        private readonly IToastNotification _ToastNotification;
        private readonly UserManager<User> _userManager;

        public StockTransferController(IMapper mapper,

            IRepository<StockTransfer> StockTransferRepo,
            IRepository<Stock> StockRepo,
            IRepository<StockMovement> StockMovementRepo,
            IToastNotification toastNotification,
            UserManager<User> userManager
          )
        {
            _Mapper = mapper;
            _StockTransferRepo = StockTransferRepo;
            _StockRepo = StockRepo;
            _StockMovementRepo = StockMovementRepo;
            _ToastNotification = toastNotification;
            _userManager = userManager;
        }

        public List<StockTransferGetDto> AllStockTransferModel { get; set; }
        public StockTransferRegisterDto StockTransferRegistermodel { get; set; }


        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize("Permissions.StockTransferIndex")]
        public async Task<IActionResult> Index()
        {
            var AllStockTransfer = await _StockTransferRepo.GetAllAsync(n => n.StockFrom,n=>n.ToStock);
            AllStockTransferModel = _Mapper.Map<List<StockTransferGetDto>>(AllStockTransfer);
            var Stocks = await _StockRepo.GetAllAsync();
            var StockTransferRegisterDto = new StockTransferRegisterDto
            {
                DrpstockDto = _Mapper.Map<List<DrpDto>>(Stocks)
            };
            var stockTransferModelDto = new StockTransferModelDto
            {
                StockTransferGetDtos= AllStockTransferModel,
                StockTransferRegisterDto= StockTransferRegisterDto
            };
            return View(stockTransferModelDto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.StockTransferCreate")]

        public async Task<IActionResult> Create(StockTransferRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var StockTransferDb = _Mapper.Map<StockTransfer>(model);
                StockTransferDb.CreatedDate = DateTime.Now;
                StockTransferDb.CreatedUser = (GetCurrentUserAsync().Result).Id.ToString();
                _StockTransferRepo.Add(StockTransferDb);
                var FromStock = _StockRepo.SingleOrDefaultAsync(n=>n.Id== model.FromStockId).Result.name;
                var ToStock = _StockRepo.SingleOrDefaultAsync(n=>n.Id== model.ToStockId).Result.name;

                //from
                StockMovement StockMovement = new StockMovement()
                {
                    MovementId = StockTransferDb.Id,
                    MovementType= StockMovementType.StockTransferfrom, 
                    StockId = StockTransferDb.FromStockId,
                    Date = StockTransferDb.Date,
                    OutValue = StockTransferDb.Value,
                    InValue = 0,
                    Notes = StockTransferDb.Notes,
                    Comment = "تحويل مبلغ الى خزنة" +" "+ ToStock + "وقدرة" +" "+ StockTransferDb.Value,

                };

                _StockMovementRepo.Add(StockMovement);

                //to
                StockMovement StockMovementto = new StockMovement()
                {
                    MovementId = StockTransferDb.Id,
                    MovementType = StockMovementType.StockTransferto,
                    StockId = StockTransferDb.ToStockId,
                    Date = StockTransferDb.Date,
                    InValue = StockTransferDb.Value,
                    OutValue = 0,
                    Notes = StockTransferDb.Notes,
                    Comment = "تحويل مبلغ من خزنة" +" "+ FromStock + "وقدرة" +" "+ StockTransferDb.Value,

                };

                _StockMovementRepo.Add(StockMovementto);
                await _StockTransferRepo.SaveAllAsync();
                _ToastNotification.AddSuccessToastMessage("تمت الاضافة");
                return RedirectToAction("Index");
            }
            else
            {
                var AllStockTransfer = await _StockTransferRepo.GetAllAsync(n => n.StockFrom, n => n.ToStock);
                AllStockTransferModel = _Mapper.Map<List<StockTransferGetDto>>(AllStockTransfer);
                var Stocks = await _StockRepo.GetAllAsync();
                var StockTransferRegisterDto = _Mapper.Map<StockTransferRegisterDto>(model);
                StockTransferRegisterDto.DrpstockDto = _Mapper.Map<List<DrpDto>>(Stocks);
                var stockTransferModelDto = new StockTransferModelDto
                {
                    StockTransferGetDtos = AllStockTransferModel,
                    StockTransferRegisterDto = StockTransferRegisterDto
                };
                _ToastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                return View("Index",stockTransferModelDto);
            }
        }

        [Authorize("Permissions.StockTransferEdit")]
        public async Task<IActionResult> GetData(Guid id)
        {
            var StockTransferById = await _StockTransferRepo.GetByIdAsync(id);
            if (StockTransferById == null)
                return NotFound();
            var StockTransferRegisterDto = _Mapper.Map<StockTransferRegisterDto>(StockTransferById);
            var Stocks = await _StockRepo.GetAllAsync();
            StockTransferRegisterDto.DrpstockDto = _Mapper.Map<List<DrpDto>>(Stocks);
            return PartialView("_PartialStockTransfer", StockTransferRegisterDto);
        }


        //For Movement And Account 
        [Authorize("Permissions.StockTransferEdit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var StockTransferById = await _StockTransferRepo.GetByIdAsync(id);
            if (StockTransferById == null)
                return NotFound();
            var AllStockTransfer = await _StockTransferRepo.GetAllAsync(n => n.StockFrom, n => n.ToStock);
            AllStockTransferModel = _Mapper.Map<List<StockTransferGetDto>>(AllStockTransfer);
            var Stocks = await _StockRepo.GetAllAsync();
            var StockTransferRegisterDto = _Mapper.Map<StockTransferRegisterDto>(StockTransferById);
            StockTransferRegisterDto.DrpstockDto = _Mapper.Map<List<DrpDto>>(Stocks);
            var stockTransferModelDto = new StockTransferModelDto
            {
                StockTransferGetDtos = AllStockTransferModel,
                StockTransferRegisterDto = StockTransferRegisterDto
            };
            return View("Index", stockTransferModelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.StockTransferEdit")]
        public async Task<IActionResult> Edit(StockTransferRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var StockTransferById = await _StockTransferRepo.GetByIdAsync((Guid)model.Id);
                if (StockTransferById == null)
                    return NotFound();
                var userAdd = StockTransferById.CreatedUser;
                var userAddDate = StockTransferById.CreatedDate;

                var StockTransferEditedDb = _Mapper.Map(model, StockTransferById);
                StockTransferEditedDb.CreatedDate = userAddDate;
                StockTransferEditedDb.CreatedUser = userAdd;
                StockTransferEditedDb.LastEditDate = DateTime.Now;
                StockTransferEditedDb.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                _StockTransferRepo.Update(StockTransferEditedDb);
                var StockMovementDbFrom = await _StockMovementRepo.SingleOrDefaultAsync(n => n.MovementId == (Guid)model.Id && n.MovementType== StockMovementType.StockTransferfrom);
                var StockMovementDbTo = await _StockMovementRepo.SingleOrDefaultAsync(n => n.MovementId == (Guid)model.Id && n.MovementType== StockMovementType.StockTransferto);
                var FromStock = _StockRepo.SingleOrDefaultAsync(n => n.Id == model.FromStockId).Result.name;
                var ToStock = _StockRepo.SingleOrDefaultAsync(n => n.Id == model.ToStockId).Result.name;

                StockMovementDbFrom.MovementId = StockTransferEditedDb.Id;
                StockMovementDbFrom.MovementType = StockMovementType.StockTransferfrom;
                StockMovementDbFrom.StockId = StockTransferEditedDb.FromStockId;
                StockMovementDbFrom.Date = StockTransferEditedDb.Date;
                StockMovementDbFrom.OutValue = StockTransferEditedDb.Value;
                StockMovementDbFrom.InValue = 0;
                StockMovementDbFrom.Notes = StockTransferEditedDb.Notes;
                StockMovementDbFrom.Comment = "تحويل مبلغ الى خزنة" +" "+ ToStock + "وقدرة" +" "+ StockTransferEditedDb.Value;

                _StockMovementRepo.Update(StockMovementDbFrom);

                StockMovementDbTo.MovementId = StockTransferEditedDb.Id;
                StockMovementDbTo.MovementType = StockMovementType.StockTransferto;
                StockMovementDbTo.Date = StockTransferEditedDb.Date;
                StockMovementDbTo.InValue = StockTransferEditedDb.Value;
                StockMovementDbTo.OutValue = 0;
                   StockMovementDbTo.Notes = StockTransferEditedDb.Notes;
                StockMovementDbTo.Comment = "تحويل مبلغ من خزنة" +" "+ FromStock + "وقدرة"+" " + StockTransferEditedDb.Value;

                _StockMovementRepo.Update(StockMovementDbFrom);
                await _StockTransferRepo.SaveAllAsync();
                _ToastNotification.AddSuccessToastMessage("تم التعديل");
                return RedirectToAction("Index");
            }
            else
            {
                var AllStockTransfer = await _StockTransferRepo.GetAllAsync(n => n.StockFrom, n => n.ToStock);
                AllStockTransferModel = _Mapper.Map<List<StockTransferGetDto>>(AllStockTransfer);
                var Stocks = await _StockRepo.GetAllAsync();
                var StockTransferRegisterDto = _Mapper.Map<StockTransferRegisterDto>(model);
                StockTransferRegisterDto.DrpstockDto = _Mapper.Map<List<DrpDto>>(Stocks);
                var stockTransferModelDto = new StockTransferModelDto
                {
                    StockTransferGetDtos = AllStockTransferModel,
                    StockTransferRegisterDto = StockTransferRegisterDto
                };
                _ToastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                return View("Index", stockTransferModelDto);
            }

        }

        [Authorize("Permissions.StockTransferDelete")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var StockMovementDb = await _StockMovementRepo.GetAllAsync(n => n.MovementId == Id && (n.MovementType== StockMovementType.StockTransferfrom || n.MovementType == StockMovementType.StockTransferto));
           
            _StockMovementRepo.DeletelistRange(StockMovementDb.ToList());


            var StockTransferById = await _StockTransferRepo.GetByIdAsync(Id);
            _StockTransferRepo.Delete(StockTransferById);

            await _StockTransferRepo.SaveAllAsync();
            _ToastNotification.AddSuccessToastMessage("تم الحذف");

            return RedirectToAction("Index");
        }


    }
}