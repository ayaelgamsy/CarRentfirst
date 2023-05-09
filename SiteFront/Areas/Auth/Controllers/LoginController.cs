using AutoMapper;
using Core.Dtos.AuthDto;
using Core.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteFront.Areas.Auth.Controllers
{

    [Area("Auth")]
    [AllowAnonymous]

    public class LoginController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IToastNotification _toastNotification;
      
        private readonly IMapper _mapper;

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public LoginController(
           IMapper mapper, SignInManager<User> signInManager, ILogger<LoginModel> logger
            , UserManager<User> userManager, IToastNotification toastNotification)
        {
            _mapper = mapper;
          
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _toastNotification = toastNotification;
        }



       
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Index(LoginDto model, string RedirectUrl)
        {
            if (ModelState.IsValid)
            {
                var check = model.Email.Contains('@');
                User user = null;
                if (check)
                    user = await _userManager.FindByEmailAsync(model.Email);
                else
                    user = await _userManager.FindByNameAsync(model.Email);

                if (user == null)
                {
                    _toastNotification.AddErrorToastMessage("مستخدم خاطىء");

                    return View(model);

                }

                RedirectUrl = RedirectUrl ?? "~/Home/index";

                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, true, false);
                if (result.Succeeded)
                {
                    if (Url.IsLocalUrl(RedirectUrl))
                    {

                        //  var UserRoles = await _unitOfWork.UserRoles.GetAllAsync(n=>n.UserId== user.Id);
                        // var UserRoles =  _unitOfWork.UserRoles.GetAllAsync(n=>n.UserId== user.Id).Result.Select(c=>c.RoleId);


                        _toastNotification.AddSuccessToastMessage("مستخدم صحيح");

                        return Redirect(RedirectUrl);
                    }
                    else
                    {

                        _toastNotification.AddSuccessToastMessage("مستخدم صحيح");

                        return RedirectToAction("Index", "Home");

                    }
                }

                _toastNotification.AddErrorToastMessage("خطا بالاسم او كلمه المرور");

                return View(model);

            }
            else
            {
                _toastNotification.AddErrorToastMessage("اكمل البيانات");

                return View();

            }


        }

    }
}
