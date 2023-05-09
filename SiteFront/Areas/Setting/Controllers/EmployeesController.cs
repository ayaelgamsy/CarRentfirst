using AutoMapper;
using Core.Dtos.EmployeeDto;
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
   // [AllowAnonymous]
    public class EmployeesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Employee> employeeRepo;
        private readonly IToastNotification _toastNotification;
        private readonly IRepository<Government> goverRepo;
        private readonly IRepository<EmployeePhoto> empPhotoRepo;
        private readonly IWebHostEnvironment _hosting;
        private readonly UserManager<User> _userManager;

        public EmployeesController(IMapper mapper,
            IRepository<Employee> EmployeeRepo, 
            IToastNotification toastNotification,
            IRepository<Government> GoverRepo, 
            IRepository<EmployeePhoto> EmpPhotoRepo,
            IWebHostEnvironment hosting,
            UserManager<User> userManager)
        {
            _mapper = mapper;
            employeeRepo = EmployeeRepo;
            _toastNotification = toastNotification;
            goverRepo = GoverRepo;
            empPhotoRepo = EmpPhotoRepo;
            _hosting = hosting;
            _userManager = userManager;
        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize("Permissions.EmployeesIndex")]
        public async Task<IActionResult> Index()
        {
            var EmployeeData = await employeeRepo.GetAllAsync(e => e.Government, e => e.EmployeePhotos);
            var employeeGetDto = _mapper.Map<List<EmployeeGetDto>>(EmployeeData);
            var employeeRegisterDto = new EmployeeRegisterDto
            {
                Governments = await goverRepo.GetAllAsync()
            };
            var employeeModelDto = new EmployeeModelDto
            {
                EmployeeGetDtos = employeeGetDto,
                EmployeeRegisterDto = employeeRegisterDto
            };
            return View(employeeModelDto);
        }

        [Authorize("Permissions.EmployeesCreate")]

        public async Task<IActionResult> Create(EmployeeRegisterDto model)
        {

            if (ModelState.IsValid)
            {
                if (model.IdentNumber == null && model.PassportNumber == null)
                {
                    _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                    return RedirectToAction("Index");
                }
                var EmployeeDbMapped = _mapper.Map<Employee>(model);
                EmployeeDbMapped.CreatedDate = DateTime.Now;
                EmployeeDbMapped.CreatedUser = (GetCurrentUserAsync().Result).Id.ToString();
                employeeRepo.Add(EmployeeDbMapped);
                await employeeRepo.SaveAllAsync();

                //For Photos
                if (model.EmployeePhotoFile != null)
                {
                    var upload = Path.Combine(_hosting.WebRootPath, "Images/Employees/" + EmployeeDbMapped.Id.ToString());
                    if (!Directory.Exists(upload))
                        Directory.CreateDirectory(upload);
                    foreach (IFormFile item in model.EmployeePhotoFile)
                    {
                        var fileName = Guid.NewGuid().ToString() + "." + item.FileName.Split(".")[1].ToString();
                        var FullPath = Path.Combine(upload, fileName);

                        string fileExtention = item.ContentType;
                        var fileLenght = item.Length;
                        if (fileExtention == "image/png" || fileExtention == "image/jpeg" || fileExtention == "image/x-png" || fileExtention == "image/jpg")
                        {
                            if (fileLenght >= 3145728)
                            {
                                Bitmap bmpPostedImage = new Bitmap(item.OpenReadStream());
                                Image objImage = ResizeImage.ScaleImage(bmpPostedImage, 1000);
                                // Saving image in jpeg format
                                objImage.Save(FullPath, ImageFormat.Jpeg);
                            }
                            else
                            {
                                item.CopyTo(new FileStream(FullPath, FileMode.Create));
                            }
                        }
                        else
                        {
                            item.CopyTo(new FileStream(FullPath, FileMode.Create));
                        }

                        var employeePhoto = new EmployeePhoto 
                        {
                            PhotoUrl = fileName,
                            EmployeeId = EmployeeDbMapped.Id
                        };
                        empPhotoRepo.Add(employeePhoto);
                        await empPhotoRepo.SaveAllAsync();
                    }
                }
                _toastNotification.AddSuccessToastMessage("تم الاضافة");
                return RedirectToAction("Index");
            }

            else
            {
                var EmployeeData = await employeeRepo.GetAllAsync(e => e.Government, e => e.EmployeePhotos);
                var employeeGetDto = _mapper.Map<List<EmployeeGetDto>>(EmployeeData);
                var employeeRegisterDto = _mapper.Map<EmployeeRegisterDto>(model);
                employeeRegisterDto.Governments = await goverRepo.GetAllAsync();
                var employeeModelDto = new EmployeeModelDto
                {
                    EmployeeGetDtos = employeeGetDto,
                    EmployeeRegisterDto = employeeRegisterDto
                };
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                return View("Index", employeeModelDto);
            }
        }

        [Authorize("Permissions.EmployeesEdit")]
        public async Task<IActionResult> GetData(Guid id)
        {
            var EmployeeById = await employeeRepo.GetByIdAsync(id);
            if (EmployeeById == null)
                return NotFound();
            var EmployeeByIdMapped = _mapper.Map<EmployeeRegisterDto>(EmployeeById);
            EmployeeByIdMapped.Governments = await goverRepo.GetAllAsync();
            return PartialView("_PartialEmployee", EmployeeByIdMapped);
        }

        [Authorize("Permissions.EmployeesEdit")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                if (model.IdentNumber == null && model.PassportNumber == null)
                {
                    _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                    return RedirectToAction("Index");
                }
                var EmployeeById = await employeeRepo.GetByIdAsync((Guid)model.Id);
                var userAdd = EmployeeById.CreatedUser;
                var userAddDate = EmployeeById.CreatedDate;
                if (model.EmployeePhotoFile != null)
                {
                    var EmployeeByIdMapped = _mapper.Map(model, EmployeeById);
                    EmployeeByIdMapped.CreatedDate = userAddDate;
                    EmployeeByIdMapped.CreatedUser = userAdd;
                    EmployeeByIdMapped.LastEditDate = DateTime.Now;
                    EmployeeByIdMapped.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                    employeeRepo.Update(EmployeeByIdMapped);
                    await employeeRepo.SaveAllAsync();

                    var upload = Path.Combine(_hosting.WebRootPath + "/Images/Employees/" + EmployeeById.Id.ToString());
                    if (!Directory.Exists(upload))
                        Directory.CreateDirectory(upload);

                    foreach (var item in model.EmployeePhotoFile)
                    {
                        var fileName = Guid.NewGuid().ToString() + "." + item.FileName.Split('.')[1].ToString();
                        var fullPath = Path.Combine(upload, fileName);

                        string fileExtention = item.ContentType;
                        var fileLenght = item.Length;
                        if (fileExtention == "image/png" || fileExtention == "image/jpeg" || fileExtention == "image/x-png" || fileExtention == "image/jpg")
                        {
                            if (fileLenght >= 3145728)
                            {
                                Bitmap bmpPostedImage = new Bitmap(item.OpenReadStream());
                                Image objImage = ResizeImage.ScaleImage(bmpPostedImage, 1000);
                                // Saving image in jpeg format
                                objImage.Save(fullPath, ImageFormat.Jpeg);
                            }
                            else
                            {
                                item.CopyTo(new FileStream(fullPath, FileMode.Create));
                            }
                        }
                        else
                        {
                            item.CopyTo(new FileStream(fullPath, FileMode.Create));
                        }

                        var employeePhoto = new EmployeePhoto
                        {
                            PhotoUrl = fileName,
                            EmployeeId = EmployeeById.Id
                        };
                        empPhotoRepo.Add(employeePhoto);
                        await empPhotoRepo.SaveAllAsync();
                    }
                }

                else
                {
                    model.EmployeePhotos = EmployeeById.EmployeePhotos;
                    var EmployeeByIdMapped = _mapper.Map(model, EmployeeById);
                    EmployeeByIdMapped.CreatedDate = userAddDate;
                    EmployeeByIdMapped.CreatedUser = userAdd;
                    EmployeeByIdMapped.LastEditDate = DateTime.Now;
                    EmployeeByIdMapped.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                    employeeRepo.Update(EmployeeByIdMapped);
                    await employeeRepo.SaveAllAsync();
                }

                _toastNotification.AddSuccessToastMessage("تم التعديل");
                return RedirectToAction("Index");
            }

            else
            {
                var EmployeeData = await employeeRepo.GetAllAsync(e => e.Government, e => e.EmployeePhotos);
                var employeeGetDto = _mapper.Map<List<EmployeeGetDto>>(EmployeeData);
                var employeeRegisterDto = _mapper.Map<EmployeeRegisterDto>(model);
                employeeRegisterDto.Governments = await goverRepo.GetAllAsync();
                var employeeModelDto = new EmployeeModelDto
                {
                    EmployeeGetDtos = employeeGetDto,
                    EmployeeRegisterDto = employeeRegisterDto
                };
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                return View("Index", employeeModelDto);
            }

        }

        [Authorize("Permissions.EmployeesDelete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var EmployeeById = await employeeRepo.GetByIdAsync((Guid)id);
            var oldUrlPhoto = await empPhotoRepo.GetAllAsync(e => e.EmployeeId == id);
            if (EmployeeById == null)
                return NotFound();
            employeeRepo.Delete(EmployeeById);

            ////DeleteOldPath   
            var upload = Path.Combine(_hosting.WebRootPath + "/Images/Employees/" + EmployeeById.Id.ToString());
            foreach (var urlPhoto in oldUrlPhoto)
            {
                var oldFileName = urlPhoto.PhotoUrl;
                var oldFullPath = Path.Combine(upload, oldFileName);
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.IO.File.Delete(oldFullPath);
                //empPhotoRepo.Delete(urlPhoto);
                //await empPhotoRepo.SaveAllAsync();
            }

            await employeeRepo.SaveAllAsync();
            return Ok();

        }

        //Get Employee Photos
        public IActionResult GetEmployeePhotos(Guid id)
        {
            var EmployeePhotos = empPhotoRepo.GetAllAsync(p => p.EmployeeId == id, p => p.Employee).Result;
            string FilePath = "/Images/Employees/" + id.ToString();

            return Json(EmployeePhotos.Select(p => new { image = FilePath + "/" + p.PhotoUrl, id = p.Id, name = p.Employee.name }));
        }


        //Delete Photo
        public async Task<IActionResult> DeletePhoto(Guid id)
        {
            var Photo = await empPhotoRepo.GetByIdAsync(id);
            ////DeleteOldPath   
            string FilePath = Path.GetFullPath("wwwroot/Images/Employees/" + Photo.EmployeeId.ToString());
            var FileName = Photo.PhotoUrl;
            var FullPath = Path.Combine(FilePath, FileName);
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.IO.File.Delete(FullPath);

            empPhotoRepo.Delete(Photo);
            await empPhotoRepo.SaveAllAsync();

            return Ok();
        }

    }
}
