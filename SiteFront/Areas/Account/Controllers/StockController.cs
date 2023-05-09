using AutoMapper;
using Core.Common.enums;
using Core.Dtos.StockDto;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteFront.Areas.Account.Controllers
{
    [Area("Account")]
   // [AllowAnonymous]
    public class StockController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Stock> _StockRepo;
        private readonly IToastNotification _toastNotification;
        private readonly IRepository<StockMovement> _StockMovementRepo;
        private readonly UserManager<User> _userManager;

        public StockRegisterDto StockRegisterModel { get; set; }

        public StockController(
            IMapper mapper,
            IRepository<Stock> StockRepo,
            IToastNotification toastNotification,
            IRepository<StockMovement> StockMovementRepo,
            UserManager<User> userManager
            )
        {
            _mapper = mapper;
            _StockRepo = StockRepo;
            _toastNotification = toastNotification;
            _StockMovementRepo = StockMovementRepo;
           _userManager = userManager;
        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize("Permissions.StockIndex")]
        public async Task<IActionResult> Index(Guid? Id)
        {

            var StockData = await (_StockRepo.GetAll(n=>n.StockMovement).AsNoTracking()).ToListAsync();

            var StockGetDto = _mapper.Map<List<StockGetDto>>(StockData);

            if (Id != null)
            {
                var StockDbById = await _StockRepo.GetByIdAsync((Guid)Id);
                StockRegisterModel = _mapper.Map<StockRegisterDto>(StockDbById);

            }
            else
            {
                StockRegisterModel = new StockRegisterDto();
            }

            StockRegisterModel.StockGetDto = StockGetDto;

            return View(StockRegisterModel);
            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.StockCreate")]

        public async Task<IActionResult> Create(StockRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var StockDb = _mapper.Map<Stock>(model);
                StockDb.CreatedDate = DateTime.Now;
                StockDb.CreatedUser = (GetCurrentUserAsync().Result).Id.ToString();
                _StockRepo.Add(StockDb);

                StockMovement StockMovement = new StockMovement()
                {
                    StockId = StockDb.Id,
                    Date = StockDb.Date,
                    MovementId = StockDb.Id,
                    MovementType=StockMovementType.StartAccount,
                    InValue = StockDb.StartAccount,
                    OutValue = 0,
                    Notes = "رصيد بداية الخزنة",
                    Comment = "رصيد بداية الخزنة",
                    AccountValue = StockDb.StartAccount,
                };

                _StockMovementRepo.Add(StockMovement);
                await _StockRepo.SaveAllAsync();
                _toastNotification.AddSuccessToastMessage("تمت الاضافة");
                return RedirectToAction("Index");
            }
            else
            {
                var StockData = await _StockRepo.GetAllAsync();
                var StockGetDto = _mapper.Map<List<StockGetDto>>(StockData);
                model.StockGetDto = StockGetDto;
                return View(model);
            }  

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.StockEdit")]
        public async Task<IActionResult> Edit(StockRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var StockById = await _StockRepo.GetByIdAsync((Guid)model.Id);
                if (StockById == null)
                    return NotFound();
                var userAdd = StockById.CreatedUser;
                var userAddDate = StockById.CreatedDate;

                var StockEditedDb = _mapper.Map(model, StockById);
                StockEditedDb.CreatedDate = userAddDate;
                StockEditedDb.CreatedUser = userAdd;
                StockEditedDb.LastEditDate = DateTime.Now;
                StockEditedDb.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                _StockRepo.Update(StockEditedDb);

                var StockMovementById = await _StockMovementRepo.SingleOrDefaultAsync(n => n.MovementId == (Guid)model.Id && n.MovementType == StockMovementType.StartAccount);

                if (StockMovementById == null)
                {
                    StockMovement StockMovement = new StockMovement()
                    {
                        StockId = StockEditedDb.Id,
                        Date = StockEditedDb.Date,
                        MovementId= StockEditedDb.Id,
                        MovementType=StockMovementType.StartAccount,
                        InValue = StockEditedDb.StartAccount,
                        OutValue = 0,
                        Notes = "رصيد بداية الخزنة",
                        Comment = "رصيد بداية الخزنة",
                        AccountValue = StockEditedDb.StartAccount,
                    };

                    _StockMovementRepo.Add(StockMovement);
                }
                else
                {
                    StockMovementById.Date = StockEditedDb.Date;
                    StockMovementById.InValue = StockEditedDb.StartAccount;
                    StockMovementById.AccountValue = StockEditedDb.StartAccount;

                    _StockMovementRepo.Update(StockMovementById);
                }
                await _StockRepo.SaveAllAsync();
                _toastNotification.AddSuccessToastMessage("تم التعديل");

                return RedirectToAction("Index");
            }
            else
            {
                var StockData = await _StockRepo.GetAllAsync();
                var StockGetDto = _mapper.Map<List<StockGetDto>>(StockData);
                model.StockGetDto = StockGetDto;
                return View(model);
            }

        }

        [Authorize("Permissions.StockDelete")]

        public async Task<IActionResult> Delete(Guid Id)
        {
            var StockById = await _StockRepo.GetByIdAsync(Id);

            var StockMovementById = await _StockMovementRepo.GetAllAsync(n => n.StockId == Id && n.MovementType != StockMovementType.StartAccount );

            if (StockMovementById.Count() == 0)
            {
                var StockMovementOfStart = await _StockMovementRepo.SingleOrDefaultAsync(n => n.StockId == Id && n.MovementType == StockMovementType.StartAccount);

                _StockMovementRepo.Delete(StockMovementOfStart);
                _StockRepo.Delete(StockById);
                await _StockRepo.SaveAllAsync();
                _toastNotification.AddSuccessToastMessage("تم الحذف");
            }
            else
            {
                _toastNotification.AddSuccessToastMessage(" لايمكن حزف هذه الخزنة ");

            }

            return RedirectToAction("Index");



        }

    }
}
