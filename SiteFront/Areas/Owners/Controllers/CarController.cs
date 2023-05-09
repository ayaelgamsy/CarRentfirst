using AutoMapper;
using Core.Dtos.CarDto;
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

namespace SiteFront.Areas.Owners.Controllers
{
    [Area("Owners")]
    //[AllowAnonymous]
    public class CarController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IToastNotification _toastNotification;
        private readonly IRepository<Car> carRepo;
        private readonly IRepository<CarOwner> carOwnerepo;
        private readonly IRepository<Company> companyRepo;
        private readonly IRepository<CarPhoto> carPhotoRepo;
        private readonly IRepository<CarVideo> carVideoRepo;
        private readonly IRepository<CustomerRent> customerRentRepo;
        private readonly IRepository<OwnerRentContract> ownerRentContractRepo;
        private readonly IWebHostEnvironment _hosting;
        private readonly UserManager<User> _userManager;

        public CarController(IMapper mapper,
            IToastNotification toastNotification,
            IRepository<Car> CarRepo,
            IRepository<CarOwner> CarOwnerepo,
            IRepository<Company> CompanyRepo,
            IRepository<CarPhoto> CarPhotoRepo,
            IRepository<CarVideo> CarVideoRepo,
            IRepository<CustomerRent> CustomerRentRepo,
            IRepository<OwnerRentContract> OwnerRentContractRepo,
            IWebHostEnvironment hosting,
            UserManager<User> userManager)
        {
            _mapper = mapper;
            _toastNotification = toastNotification;
            carRepo = CarRepo;
            carOwnerepo = CarOwnerepo;
            companyRepo = CompanyRepo;
            carPhotoRepo = CarPhotoRepo;
            carVideoRepo = CarVideoRepo;
            customerRentRepo = CustomerRentRepo;
            ownerRentContractRepo = OwnerRentContractRepo;
            _hosting = hosting;
            _userManager = userManager;
        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize("Permissions.CarIndex")]
        public async Task<IActionResult> Index()
        {
            var carData = await carRepo.GetAllAsync(c => c.Company, c => c.CarOwner, c => c.CarPhotos, c => c.CarVideos);
            var carGetDto = _mapper.Map<List<CarGetDto>>(carData);
            var carRegisterDto = new CarRegisterDto
            {
                Companys = await companyRepo.GetAllAsync(),
                CarOwners = await carOwnerepo.GetAllAsync()

            };
            var carModelDto = new CarModelDto
            {
                CarGetDtos = carGetDto,
                CarRegisterDto = carRegisterDto
            };
            return View(carModelDto);
        }

        [Authorize("Permissions.CarCreate")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var CarDbMapped = _mapper.Map<Car>(model);
                CarDbMapped.CreatedDate = DateTime.Now;
                CarDbMapped.CreatedUser = (GetCurrentUserAsync().Result).Id.ToString();
                carRepo.Add(CarDbMapped);
                await carRepo.SaveAllAsync();

                //For Photos
                if (model.CarPhotoFile != null)
                {
                    var upload = Path.Combine(_hosting.WebRootPath, "Images/Cars/Photos/" + CarDbMapped.Id.ToString());
                    if (!Directory.Exists(upload))
                        Directory.CreateDirectory(upload);

                    foreach (IFormFile item in model.CarPhotoFile)
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

                        var carPhoto = new CarPhoto
                        {
                            PhotoUrl = fileName,
                            CarId = CarDbMapped.Id
                        };
                        carPhotoRepo.Add(carPhoto);
                        await carPhotoRepo.SaveAllAsync();
                    }
                }

                //For Videos
                if (model.CarVideoFile != null)
                {
                    var upload = Path.Combine(_hosting.WebRootPath, "Videos/Cars/" + CarDbMapped.Id.ToString());
                    if (!Directory.Exists(upload))
                        Directory.CreateDirectory(upload);
                    foreach (IFormFile item in model.CarVideoFile)
                    {
                        var fileName = Guid.NewGuid().ToString() + "." + item.FileName.Split(".")[1].ToString();
                        var FullPath = Path.Combine(upload, fileName);
                        item.CopyTo(new FileStream(FullPath, FileMode.Create));

                        var carvideo = new CarVideo
                        {
                            VideoUrl = fileName,
                            CarId = CarDbMapped.Id
                        };
                        carVideoRepo.Add(carvideo);
                        await carVideoRepo.SaveAllAsync();
                    }
                }
                _toastNotification.AddSuccessToastMessage("تم الاضافة");
                return RedirectToAction("Index");
            }
            else
            {
                var carData = await carRepo.GetAllAsync(c => c.Company, c => c.CarOwner, c => c.CarPhotos, c => c.CarVideos);
                var carGetDto = _mapper.Map<List<CarGetDto>>(carData);
                var carRegisterDto = _mapper.Map<CarRegisterDto>(model);

                carRegisterDto.Companys = await companyRepo.GetAllAsync();
                carRegisterDto.CarOwners = await carOwnerepo.GetAllAsync();

                var carModelDto = new CarModelDto
                {
                    CarGetDtos = carGetDto,
                    CarRegisterDto = carRegisterDto
                };
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة !! ");
                return View("Index", carModelDto);
            }
           
        }

        public async Task<IActionResult> GetData(Guid id)
        {
            var CarById = await carRepo.GetByIdAsync(id);
            if (CarById == null)
                return NotFound();
            var CarRegisterDto = _mapper.Map<CarRegisterDto>(CarById);
            CarRegisterDto.Companys = await companyRepo.GetAllAsync();
            CarRegisterDto.CarOwners = await carOwnerepo.GetAllAsync();

            return PartialView("_PartialCar", CarRegisterDto);
        }

        [Authorize("Permissions.CarEdit")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CarRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var CarById = await carRepo.GetByIdAsync((Guid)model.Id);
                var userAdd = CarById.CreatedUser;
                var userAddDate = CarById.CreatedDate;

                if (model.CarPhotoFile != null || model.CarVideoFile != null)
                {
                    var CarDbMapped = _mapper.Map(model, CarById);
                    CarDbMapped.CreatedDate = userAddDate;
                    CarDbMapped.CreatedUser = userAdd;
                    CarDbMapped.LastEditDate = DateTime.Now;
                    CarDbMapped.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                    carRepo.Update(CarDbMapped);
                    await carRepo.SaveAllAsync();
                    //For Photos
                    if (model.CarPhotoFile != null)
                    {
                        var upload = Path.Combine(_hosting.WebRootPath, "Images/Cars/Photos/" + CarDbMapped.Id.ToString());
                        if (!Directory.Exists(upload))
                            Directory.CreateDirectory(upload);
                        foreach (IFormFile item in model.CarPhotoFile)
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
                            var carPhoto = new CarPhoto
                            {
                                PhotoUrl = fileName,
                                CarId = CarById.Id
                            };
                            carPhotoRepo.Add(carPhoto);
                            await carPhotoRepo.SaveAllAsync();
                        }
                    }
                    //For Videos
                    if (model.CarVideoFile != null)
                    {
                        var upload = Path.Combine(_hosting.WebRootPath, "Videos/Cars/" + CarDbMapped.Id.ToString());
                        if (!Directory.Exists(upload))
                            Directory.CreateDirectory(upload);
                        //AddNewPath
                        foreach (IFormFile item in model.CarVideoFile)
                        {
                            var fileName = Guid.NewGuid().ToString() + "." + item.FileName.Split(".")[1].ToString();
                            var FullPath = Path.Combine(upload, fileName);
                            item.CopyTo(new FileStream(FullPath, FileMode.Create));
                            var carVideo = new CarVideo
                            {
                                VideoUrl = fileName,
                                CarId = CarById.Id
                            };
                            carVideoRepo.Add(carVideo);
                            await carVideoRepo.SaveAllAsync();
                        }
                    }
                }
                else
                {
                    model.CarPhotos = CarById.CarPhotos;
                    model.CarVideos = CarById.CarVideos;
                    var CarDbMapped = _mapper.Map(model, CarById);
                    CarDbMapped.CreatedDate = userAddDate;
                    CarDbMapped.CreatedUser = userAdd;
                    CarDbMapped.LastEditDate = DateTime.Now;
                    CarDbMapped.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                    carRepo.Update(CarDbMapped);
                    await carRepo.SaveAllAsync();
                }
                _toastNotification.AddSuccessToastMessage("تم التعديل");
                return RedirectToAction("Index");
            }
            else
            {
                var carData = await carRepo.GetAllAsync(c => c.Company, c => c.CarOwner, c => c.CarPhotos, c => c.CarVideos);
                var carGetDto = _mapper.Map<List<CarGetDto>>(carData);
                var carRegisterDto = _mapper.Map<CarRegisterDto>(model);

                carRegisterDto.Companys = await companyRepo.GetAllAsync();
                carRegisterDto.CarOwners = await carOwnerepo.GetAllAsync();

                var carModelDto = new CarModelDto
                {
                    CarGetDtos = carGetDto,
                    CarRegisterDto = carRegisterDto
                };
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة !! ");
                return View("Index", carModelDto);
            }
        }
        [Authorize("Permissions.CarDelete")]

        public async Task<IActionResult> Delete(Guid id)
        {
            var CarById = await carRepo.GetByIdAsync(id);
            var CustomerRentById = await customerRentRepo.GetAllAsync(c => c.CarId == id);
            var OwnerRentContractById = await ownerRentContractRepo.GetAllAsync(c => c.CarId == id);
            var oldUrlPhoto = await carPhotoRepo.GetAllAsync(e => e.CarId == id);
            var oldUrlVideo = await carVideoRepo.GetAllAsync(e => e.CarId == id);
            if (CustomerRentById.Count() != 0 || OwnerRentContractById.Count() != 0)
                _toastNotification.AddErrorToastMessage("لا يمكن حذف هذه السيارة");
            else
            {
                carRepo.Delete(CarById);
                await carRepo.SaveAllAsync();
                //DeleteOldPath For Photo   
                var uploadPhoto = Path.Combine(_hosting.WebRootPath + "/Images/Cars/Photos/" + CarById.Id.ToString());
                foreach (var urlPhoto in oldUrlPhoto)
                {
                    var oldFileName = urlPhoto.PhotoUrl;
                    var oldFullPath = Path.Combine(uploadPhoto, oldFileName);
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    System.IO.File.Delete(oldFullPath);
                }
                //DeleteOldPath For Video   
                var uploadVideo = Path.Combine(_hosting.WebRootPath + "/Videos/Cars/" + CarById.Id.ToString());
                foreach (var urlVideo in oldUrlVideo)
                {
                    var oldFileName = urlVideo.VideoUrl;
                    var oldFullPath = Path.Combine(uploadVideo, oldFileName);
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    System.IO.File.Delete(oldFullPath);
                }
                _toastNotification.AddSuccessToastMessage("تم الحذف");
            }
            return RedirectToAction("Index");
        }

        //Get Car Photos
        public IActionResult GetCarPhotos(Guid id)
        {
            var CarPhotos = carPhotoRepo.GetAllAsync(p => p.CarId == id, p => p.Car).Result;
            string FilePath = "/Images/Cars/Photos/" + id.ToString();

            return Json(CarPhotos.Select(p => new { image = FilePath + "/" + p.PhotoUrl, id = p.Id, name = p.Car.name }));
        }

        //Delete Photo
        public async Task<IActionResult> DeletePhoto(Guid id)
        {
            var Photo = await carPhotoRepo.GetByIdAsync(id);
            ////DeleteOldPath   
            string FilePath = Path.GetFullPath("wwwroot/Images/Cars/Photos/" + Photo.CarId.ToString());
            var FileName = Photo.PhotoUrl;
            var FullPath = Path.Combine(FilePath, FileName);
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.IO.File.Delete(FullPath);

            carPhotoRepo.Delete(Photo);
            await carPhotoRepo.SaveAllAsync();

            return Ok();
        }

        //Get Car Videos
        public IActionResult GetCarVideos(Guid id)
        {
            var CarVideos = carVideoRepo.GetAllAsync(p => p.CarId == id, p => p.Car).Result;
            string FilePath = "/Videos/Cars/" + id.ToString();

            return Json(CarVideos.Select(p => new { video = FilePath + "/" + p.VideoUrl, id = p.Id, name = p.Car.name }));
        }

        //Delete Video
        public async Task<IActionResult> DeleteVideo(Guid id)
        {
            var video = await carVideoRepo.GetByIdAsync(id);
            ////DeleteOldPath   
            string FilePath = Path.GetFullPath("wwwroot/Videos/Cars/" + video.CarId.ToString());
            var FileName = video.VideoUrl;
            var FullPath = Path.Combine(FilePath, FileName);
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.IO.File.Delete(FullPath);

            carVideoRepo.Delete(video);
            await carVideoRepo.SaveAllAsync();

            return Ok();
        }
    }
}
