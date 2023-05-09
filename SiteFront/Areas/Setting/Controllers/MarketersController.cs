using AutoMapper;
using Core.Dtos.MarketerDto;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SiteFront.Areas.Setting.Controllers
{
    [Area("Setting")]
    //[AllowAnonymous]
    public class MarketersController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IToastNotification _toastNotification;
        private readonly IRepository<Marketer> _marketerRepo;
        private readonly IRepository<Government> _governmentRepo;
        private readonly IRepository<MarketerPhoto> _marketerPhotoRepo;
        private readonly IWebHostEnvironment _hosting;
        private readonly UserManager<User> _userManager;

        public MarketersController(IMapper mapper,
            IToastNotification toastNotification,
            IRepository<Marketer> marketerRepo,
            IRepository<Government> governmentRepo,
            IRepository<MarketerPhoto> marketerPhotoRepo,
            IWebHostEnvironment hosting,
            UserManager<User> userManager)
        {
            _mapper = mapper;
            _toastNotification = toastNotification;
            _marketerRepo = marketerRepo;
            _governmentRepo = governmentRepo;
            _marketerPhotoRepo = marketerPhotoRepo;
            _hosting = hosting;
           _userManager = userManager;
        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize("Permissions.MarketersIndex")]
        public async Task<IActionResult> Index()
        {
            var MarketerData =await _marketerRepo.GetAllAsync(m=>m.Government,m=>m.MarketerPhotos);
            var marketerGetDto = _mapper.Map<List<MarketerGetDto>>(MarketerData);
            var marketerRegisterDto = new MarketerRegisterDto
            {
                Governments = await _governmentRepo.GetAllAsync()
            };
            var marketerModelDto = new MarketerModelDto
            {
                MarketerGetDtos = marketerGetDto,
                MarketerRegisterDto = marketerRegisterDto
            };
            return View(marketerModelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.MarketersCreate")]
        public async Task<IActionResult> Create(MarketerRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                if (model.IdentNumber == null && model.PassportNumber == null)
                {
                    _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                    return RedirectToAction("Index");
                }
                var MarketerDb = _mapper.Map<Marketer>(model);
                MarketerDb.CreatedDate = DateTime.Now;
                MarketerDb.CreatedUser = (GetCurrentUserAsync().Result).Id.ToString();
                _marketerRepo.Add(MarketerDb);
                await _marketerRepo.SaveAllAsync();
                if (model.MarketerPhotoFile != null)
                {
                    var upload = Path.Combine(_hosting.WebRootPath + "/Images/Marketers/" + MarketerDb.Id.ToString());
                    if (!Directory.Exists(upload))
                        Directory.CreateDirectory(upload);
                    foreach (IFormFile Photo in model.MarketerPhotoFile)
                    {
                        var fileName = Guid.NewGuid().ToString()+"." + Photo.FileName.Split('.')[1].ToString();
                        var fullPath = Path.Combine(upload, fileName);

                        string fileExtention = Photo.ContentType;
                        var fileLenght = Photo.Length;
                        if (fileExtention == "image/png" || fileExtention == "image/jpeg" || fileExtention == "image/x-png" || fileExtention == "image/jpg")
                        {
                            if (fileLenght >= 3145728)
                            {
                                Bitmap bmpPostedImage = new Bitmap(Photo.OpenReadStream());
                                Image objImage = ResizeImage.ScaleImage(bmpPostedImage, 1000);
                                // Saving image in jpeg format
                                objImage.Save(fullPath, ImageFormat.Jpeg);
                            }
                            else
                            {
                                Photo.CopyTo(new FileStream(fullPath, FileMode.Create));
                            }
                        }
                        else
                        {
                            Photo.CopyTo(new FileStream(fullPath, FileMode.Create));
                        }

                        var marketerPhoto = new MarketerPhoto
                        {
                            EmployeeId = MarketerDb.Id,
                            PhotoUrl = fileName
                        };
                        _marketerPhotoRepo.Add(marketerPhoto);
                        await _marketerPhotoRepo.SaveAllAsync();
                    }
                }
                _toastNotification.AddSuccessToastMessage("تم الاضافة");
                return RedirectToAction("Index");
            }
            _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
            return RedirectToAction("Index");
        }

        [Authorize("Permissions.MarketersEdit")]
        public async Task<IActionResult> GetData(Guid id)
        {
            var MarketerById = await _marketerRepo.GetByIdAsync(id);
            var marketerRegisterDto = _mapper.Map<MarketerRegisterDto>(MarketerById);
            marketerRegisterDto.Governments = await _governmentRepo.GetAllAsync();
            return PartialView("_PartialMarketer", marketerRegisterDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.MarketersEdit")]
        public async Task<IActionResult> Edit(MarketerRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                if (model.IdentNumber == null && model.PassportNumber == null)
                {
                    _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                    return RedirectToAction("Index");
                }
                var MarketerById = await _marketerRepo.GetByIdAsync((Guid)model.Id);
                var userAdd = MarketerById.CreatedUser;
                var userAddDate = MarketerById.CreatedDate;
                if (model.MarketerPhotoFile != null)
                {
                    var MarketerDb = _mapper.Map(model, MarketerById);
                    MarketerDb.CreatedDate = userAddDate;
                    MarketerDb.CreatedUser = userAdd;
                    MarketerDb.LastEditDate = DateTime.Now;
                    MarketerDb.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                    _marketerRepo.Update(MarketerDb);
                    await _marketerRepo.SaveAllAsync();

                    var upload = Path.Combine(_hosting.WebRootPath + "/Images/Marketers/" + MarketerDb.Id.ToString());
                    if (!Directory.Exists(upload))
                        Directory.CreateDirectory(upload);
                    foreach (IFormFile Photo in model.MarketerPhotoFile)
                    {
                        var fileName = Guid.NewGuid().ToString() + "." + Photo.FileName.Split('.')[1].ToString();
                        var fullPath = Path.Combine(upload, fileName);

                        string fileExtention = Photo.ContentType;
                        var fileLenght = Photo.Length;
                        if (fileExtention == "image/png" || fileExtention == "image/jpeg" || fileExtention == "image/x-png" || fileExtention == "image/jpg")
                        {
                            if (fileLenght >= 3145728)
                            {
                                Bitmap bmpPostedImage = new Bitmap(Photo.OpenReadStream());
                                Image objImage = ResizeImage.ScaleImage(bmpPostedImage, 1000);
                                // Saving image in jpeg format
                                objImage.Save(fullPath, ImageFormat.Jpeg);
                            }
                            else
                            {
                                Photo.CopyTo(new FileStream(fullPath, FileMode.Create));
                            }
                        }
                        else
                        {
                            Photo.CopyTo(new FileStream(fullPath, FileMode.Create));
                        }

                        var marketerPhoto = new MarketerPhoto
                        {
                            EmployeeId = MarketerDb.Id,
                            PhotoUrl = fileName
                        };
                        _marketerPhotoRepo.Add(marketerPhoto);
                        await _marketerPhotoRepo.SaveAllAsync();
                    }
                }
                else
                {
                    model.MarketerPhotos = MarketerById.MarketerPhotos;
                    var MarketerDb = _mapper.Map(model, MarketerById);
                    MarketerDb.CreatedDate = userAddDate;
                    MarketerDb.CreatedUser = userAdd;
                    MarketerDb.LastEditDate = DateTime.Now;
                    MarketerDb.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                    _marketerRepo.Update(MarketerDb);
                    await _marketerRepo.SaveAllAsync();
                }
                _toastNotification.AddSuccessToastMessage("تم التعديل");
                return RedirectToAction("Index");
            }

            var MarketerData = await _marketerRepo.GetAllAsync(m => m.Government, m => m.MarketerPhotos);
            var marketerGetDto = _mapper.Map<List<MarketerGetDto>>(MarketerData);
            var marketerRegisterDto = _mapper.Map<MarketerRegisterDto>(model);
            marketerRegisterDto.Governments = await _governmentRepo.GetAllAsync();
            var marketerModelDto = new MarketerModelDto
            {
                MarketerGetDtos = marketerGetDto,
                MarketerRegisterDto = marketerRegisterDto
            };
            _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
            return View("Index",marketerModelDto);

        }


        [Authorize("Permissions.MarketersDelete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var MarketerById = await _marketerRepo.GetByIdAsync(id);
            if (MarketerById == null)
                return NotFound();
            var marketerPhotos = await _marketerPhotoRepo.GetAllAsync(p => p.EmployeeId == id);
            _marketerRepo.Delete(MarketerById);

            //Delete Photo
            var upload = Path.Combine(_hosting.WebRootPath + "/Images/Marketers/" + MarketerById.Id.ToString());
            foreach (var photo in marketerPhotos)
            {
                var oldFileName = photo.PhotoUrl;
                var oldFullPath = Path.Combine(upload, oldFileName);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                System.IO.File.Delete(oldFullPath);
            }
            await _marketerRepo.SaveAllAsync();
            return Ok();
        }

        //Get Employee Photos
        public IActionResult GetMarketerPhotos(Guid id)
        {
            var MarketerPhotos = _marketerPhotoRepo.GetAllAsync(p => p.EmployeeId == id, p => p.Marketer).Result;
            string FilePath = "/Images/Marketers/" + id.ToString();

            return Json(MarketerPhotos.Select(p => new { image = FilePath + "/" + p.PhotoUrl, id = p.Id, name = p.Marketer.name }));
        }


        //Delete Photo
        public async Task<IActionResult> DeletePhoto(Guid id)
        {
            var Photo = await _marketerPhotoRepo.GetByIdAsync(id);
            ////DeleteOldPath   
            string FilePath = Path.GetFullPath("wwwroot/Images/Marketers/" + Photo.EmployeeId.ToString());
            var FileName = Photo.PhotoUrl;
            var FullPath = Path.Combine(FilePath, FileName);
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.IO.File.Delete(FullPath);

            _marketerPhotoRepo.Delete(Photo);
            await _marketerPhotoRepo.SaveAllAsync();

            return Ok();
        }

    }
}
