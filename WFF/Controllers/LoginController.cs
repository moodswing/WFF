using System;
using System.Web;
using WFF.Utils;
using WFF.Models;
using WFF.Services;
using WFF.ViewModels;
using WFF.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace WFF.Controllers
{
    // Adding the HTTPS attribute
#if !DEBUG
            [RequireHttps]
#endif
    public class LoginController : BaseController
    {
        private ILoginService LoginService { get; set; }
        public IUserProfile UserProfile { get; set; }

        public LoginController(IMemoryCache cache, ILoginService loginService, IHeaderViewModel headerViewModel, IADUserViewModel aDUserViewModel, 
                               IActionViewModel actionViewModel, IUserUtil userUtil, IUserProfile userProfile, IBitacora bitacora) :
        base(cache, headerViewModel, aDUserViewModel, actionViewModel, userUtil, bitacora)
        {
            LoginService = loginService;
            UserProfile = userProfile;
        }

        public IActionResult Index(string message, string idRequest = null)
        {
            Logger.Info("LoginController -> Index");
            Bitacora.registraBitacora(System.Reflection.MethodBase.GetCurrentMethod(), HttpContext);

            // to-do: quitar esto
            //return View(new LoginViewModel { User = "raravena", Message = message, idRequest = idRequest });
            return View(new LoginViewModel { Message = message, idRequest = idRequest });
        }

        [HttpPost]  
        public IActionResult Validate(LoginViewModel model)
        {
            Logger.Info("UsuarioController -> Validar");
            Bitacora.registraBitacora(System.Reflection.MethodBase.GetCurrentMethod(), HttpContext);
           
            try
            {
                //if (LoginService.IsUserValid(model.User, model.Password))
                //{

                UserProfile.Email = "rob.arav@gmail.com";

                HttpContext.Session.SetObject("UserSession", UserProfile);
                HttpContext.Session.SetString("UserEmailSession", "rob.arav@gmail.com");

                HttpContext.Session.SetString("UserNameSession", "Robinson Aravena");
                //FormsAuthentication.SetAuthCookie(model.User, false);

                if (model.idRequest != null)
                    return RedirectToAction("EditRequest", "FormRequest", new { idRequest = int.Parse(model.idRequest) });

                return RedirectToAction("Index", "Desktop");
                //}

            }
            catch (LoginException e)
            {
                Logger.Info("Validacion de usuario no exitosa=[" + e.Message + "]");
                TempData[Constants.InfoKey] = e.Message;
            }
            catch (Exception e)
            {
                Logger.Error("Error=[" + e.Message + "]");
                TempData[Constants.ErrorKey] = e.Message;
            }

            return RedirectToAction("Index", "Login");
        }
    }
}
