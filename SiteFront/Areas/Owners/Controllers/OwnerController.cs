using AutoMapper;
using Core.Dtos.DropDowns;
using Core.Dtos.OwnersDto;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SiteFront.Areas.Owners.Controllers
{
    [Area("Owners")]
   // [AllowAnonymous]
    public class OwnerController : Controller
    {
        public OwnerController(
            IRepository<CarOwner> CarOwnerRepo,
            IRepository<CarOwnerPhoto> OwnerImageRepo,
            IRepository<Government> GovernmentRepo,
            IMapper mapper,
            IToastNotification toastNotification,
            UserManager<User> userManager)
        {
            _mapper = mapper;
            _toastNotification = toastNotification;
            _OwnerImageRepo = OwnerImageRepo;
            _CarOwnerRepo = CarOwnerRepo;
            _GovernmentRepo = GovernmentRepo;
            _userManager = userManager;
            _OwnerModelDto = new OwnerModelDto();
        }

        public IMapper _mapper { get; private set; }
        public IToastNotification _toastNotification { get; private set; }
        public IRepository<CarOwnerPhoto> _OwnerImageRepo { get; private set; }
        public IRepository<CarOwner> _CarOwnerRepo { get; private set; }
        public IRepository<Government> _GovernmentRepo { get; private set; }
        private readonly UserManager<User> _userManager;
        private OwnerModelDto _OwnerModelDto;

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize("Permissions.OwnerIndex")]
        public async Task<IActionResult> IndexAsync()
        {
            var Government = await _GovernmentRepo.GetAllAsync();
            _OwnerModelDto.OwnerRegister.Government = _mapper.Map<List<GovernmentDrop>>(Government);

            var CarOwners = await (_CarOwnerRepo.GetAll().Include(o => o.Government).Include(o => o.CarOwnerPhotos)).ToListAsync();
            _OwnerModelDto.OwnerGetDtos = _mapper.Map<List<OwnerGetDto>>(CarOwners);

            return View(_OwnerModelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.OwnerCreate")]
        public async Task<IActionResult> CreateAsync(OwnerRegisterDto owner)
        {
            if (ModelState.IsValid)
            {
                if (owner.IdentNumber == null && owner.PassportNumber == null)
                {
                    _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                    return RedirectToAction("Index");
                }
                #region Insert New Owner
                CarOwner DbOwner = _mapper.Map<CarOwner>(owner);
                DbOwner.CreatedDate = DateTime.Now;
                DbOwner.CreatedUser = ( GetCurrentUserAsync().Result).Id.ToString();

                _CarOwnerRepo.Add(DbOwner);
                await _CarOwnerRepo.SaveAllAsync();

                _toastNotification.AddSuccessToastMessage("تمت الاضافة");
                #endregion 
                #region Images For New Owner

                var files = Request.Form.Files;
                string FilePath = Path.GetFullPath("wwwroot/Images/Owners/" + DbOwner.Id.ToString());

                if (!Directory.Exists(FilePath))
                    Directory.CreateDirectory(FilePath);

                if (Request.Form.Files.Any())
                {
                    foreach (var file in Request.Form.Files)
                    {
                        var fileName = string.Format("{0:dd_MM_yyyy_HH_mm_ss.}", DateTime.Now) + file.FileName;
                        var filePath = Path.Combine(FilePath, fileName.ToString());
                        string fileExtention = file.ContentType;
                        var fileLenght = file.Length;
                        if (fileExtention == "image/png" || fileExtention == "image/jpeg" || fileExtention == "image/x-png" || fileExtention == "image/jpg")
                        {
                            if (fileLenght >= 3145728)
                            {
                                Bitmap bmpPostedImage = new Bitmap(file.OpenReadStream());
                                Image objImage = ResizeImage.ScaleImage(bmpPostedImage, 1000);
                                // Saving image in jpeg format
                                objImage.Save(filePath, ImageFormat.Jpeg);
                            }
                            else
                            {
                                file.CopyTo(new FileStream(filePath, FileMode.Create));
                            }
                        }
                        else
                        {
                            file.CopyTo(new FileStream(filePath, FileMode.Create));
                        }
                        
                            DbOwner.CarOwnerPhotos.Add(new CarOwnerPhoto
                            {
                                PhotoUrl = fileName
                            });
                           
                        
                    }

                    await _CarOwnerRepo.SaveAllAsync();
                }
                #endregion

                return RedirectToAction("Index");
            }
            else
            {
                _OwnerModelDto.OwnerRegister = owner;
                var Government = await _GovernmentRepo.GetAllAsync();
                _OwnerModelDto.OwnerRegister.Government = _mapper.Map<List<GovernmentDrop>>(Government);
                var CarOwners = await (_CarOwnerRepo.GetAll().Include(o => o.Government).Include(o => o.CarOwnerPhotos)).ToListAsync();
                _OwnerModelDto.OwnerGetDtos = _mapper.Map<List<OwnerGetDto>>(CarOwners);
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة !!");
                return View("Index", _OwnerModelDto);
            }

        }
      
        [AllowAnonymous]
        public async Task<IActionResult> GetData(Guid id)
        {
            var owner = await _CarOwnerRepo.GetByIdAsync(id);
            if (owner == null)
                return NotFound();

            var ownerEdit = _mapper.Map<OwnerRegisterDto>(owner);

            var Government = await _GovernmentRepo.GetAllAsync();
            ownerEdit.Government = _mapper.Map<List<GovernmentDrop>>(Government);

            return PartialView("_PartialAddOwner", ownerEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.OwnerEdit")]
        public async Task<IActionResult> Edit(OwnerRegisterDto owner)
        {
            if (ModelState.IsValid)
            {
                if (owner.IdentNumber == null && owner.PassportNumber == null)
                {
                    _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                    return RedirectToAction("Index");
                }
                var ownerToEdit = await _CarOwnerRepo.SingleOrDefaultAsync(o => o.Id == (Guid)owner.Id, NoTacking: true, o => o.CarOwnerPhotos);
                if (ownerToEdit == null)
                    return NotFound();

                var userAdd = ownerToEdit.CreatedUser;
                var userAddDate = ownerToEdit.CreatedDate;

                ownerToEdit = _mapper.Map<CarOwner>(owner);
                ownerToEdit.CreatedDate = userAddDate;
                ownerToEdit.CreatedUser = userAdd;
                ownerToEdit.LastEditDate = DateTime.Now;
                ownerToEdit.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();

                if (Request.Form.Files.Any())
                {
                    var files = Request.Form.Files;
                    string FilePath = Path.GetFullPath("wwwroot/Images/Owners/" + ownerToEdit.Id.ToString());

                    if (!Directory.Exists(FilePath))
                        Directory.CreateDirectory(FilePath);

                    foreach (var file in Request.Form.Files)
                    {
                        var fileName = string.Format("{0:dd_MM_yyyy_HH_mm_ss.}", DateTime.Now) + file.FileName;
                        var filePath = Path.Combine(FilePath, fileName.ToString());

                        string fileExtention = file.ContentType;
                        var fileLenght = file.Length;
                        if (fileExtention == "image/png" || fileExtention == "image/jpeg" || fileExtention == "image/x-png" || fileExtention == "image/jpg")
                        {
                            if (fileLenght >= 3145728)
                            {
                                Bitmap bmpPostedImage = new Bitmap(file.OpenReadStream());
                                Image objImage = ResizeImage.ScaleImage(bmpPostedImage, 1000);
                                // Saving image in jpeg format
                                objImage.Save(filePath, ImageFormat.Jpeg);
                            }
                            else
                            {
                                file.CopyTo(new FileStream(filePath, FileMode.Create));
                            }
                        }
                        else
                        {
                            file.CopyTo(new FileStream(filePath, FileMode.Create));
                        }

                        ownerToEdit.CarOwnerPhotos.Add(new CarOwnerPhoto
                        {
                            PhotoUrl = fileName
                        });
                    }                   
                }

                _CarOwnerRepo.Update(ownerToEdit);
                await _CarOwnerRepo.SaveAllAsync();

                _toastNotification.AddSuccessToastMessage("تم التعديل");

                return RedirectToAction("Index");
            }
            else
            {
                _OwnerModelDto.OwnerRegister = owner;
                var Government = await _GovernmentRepo.GetAllAsync();
                _OwnerModelDto.OwnerRegister.Government = _mapper.Map<List<GovernmentDrop>>(Government);
                var CarOwners = await (_CarOwnerRepo.GetAll().Include(o => o.Government).Include(o => o.CarOwnerPhotos)).ToListAsync();
                _OwnerModelDto.OwnerGetDtos = _mapper.Map<List<OwnerGetDto>>(CarOwners);
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة !!");
                return View("Index", _OwnerModelDto);
            }

        }

        [HttpDelete]
        [Authorize("Permissions.OwnerDelete")]
        public async Task<IActionResult> DeleteOwner(Guid ownerId)
        {
            var owner = await _CarOwnerRepo.GetByIdAsync(ownerId);
            if (owner == null)
                return NotFound();

            _CarOwnerRepo.Delete(owner);
            var result = _CarOwnerRepo.SaveAllAsync().Result;
            if (result)
                return Ok();
            else
                return BadRequest();
        }


        public IActionResult GetOwnerPhotos(Guid id)
        {
            var OwnerPhotos = _OwnerImageRepo.GetAllAsync(p => p.CarOwnerId == id, p => p.CarOwner).Result;
            string FilePath = "/Images/Owners/" + id.ToString();

            return Json(OwnerPhotos.Select(p => new { image = FilePath +"/"+ p.PhotoUrl , id=p.Id , name=p.CarOwner.name}));
        }


        //Delete Photo
        public async Task<IActionResult> DeletePhoto(Guid id)
        {
            var Photo = await _OwnerImageRepo.GetByIdAsync(id);
            ////DeleteOldPath   
            string FilePath = Path.GetFullPath("wwwroot/Images/Owners/" + Photo.CarOwnerId.ToString());
            var FileName = Photo.PhotoUrl;
            var FullPath = Path.Combine(FilePath, FileName);
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.IO.File.Delete(FullPath);

            _OwnerImageRepo.Delete(Photo);
            await _OwnerImageRepo.SaveAllAsync();

            return Ok();
        }

    }
}
