using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WFF.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WFF.Utils;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Routing;

namespace WFF.Controllers
{
    public class BaseController : Controller
    {
        public IMemoryCache MemoryCache;
        public readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public IHeaderViewModel HeaderViewModel { get; set; }
        public IActionViewModel ActionViewModel { get; set; }
        public IADUserViewModel ADUserViewModel { get; set; }
        public IUserUtil UserUtil { get; set; }
        public IBitacora Bitacora { get; set; }

        public BaseController()
        {

        }

        public BaseController(IMemoryCache cache, IHeaderViewModel headerViewModel, IADUserViewModel aDUserViewModel, IActionViewModel actionViewModel, IUserUtil userUtil, IBitacora bitacora)
        {
            MemoryCache = cache;
            HeaderViewModel = headerViewModel;
            ADUserViewModel = aDUserViewModel;
            ActionViewModel = actionViewModel;
            Bitacora = bitacora;
            UserUtil = userUtil;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (HttpContext != null && MemoryCache.Get(Constants.CacheAdUsers) as List<ADUserViewModel> == null)
                GetADUsers();

            if (string.IsNullOrEmpty(UserUtil.UserName) && ControllerContext.ToString() != "Login")
            {
                object controllerParameters;
                if (!string.IsNullOrEmpty(HttpContext.Request.Query["idRequest"].ToString()))
                    controllerParameters = new { controller = "Login", action = "Index", message = "Session timeout, please login again.", idRequest = HttpContext.Request.Query["idRequest"].ToString() };
                else
                    controllerParameters = new { controller = "Login", action = "Index", message = "Session timeout, please login again." };

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(controllerParameters));
            }
        }

        //public List<ADUserViewModel> LoadADUsers()
        //{
        //    var entry = new DirectoryEntry("asdasd://sgo-win12-ad-p1.asdasd.sd.org:389", "asdasd", "ld@asdasd");

        //    DirectorySearcher directorySearch = new DirectorySearcher(entry);
        //    SearchResultCollection results = directorySearch.FindAll();

        //    var adUsers = new List<ADUserViewModel>();

        //    foreach (SearchResult item in results)
        //    {
        //        var viewModel = new ADUserViewModel();

        //        if (item.Properties["name"].Count == 0 || item.Properties["adspath"].Count == 0 || item.Properties["mail"].Count == 0 || item.Properties["displayname"].Count == 0) continue;

        //        var name = item.Properties["displayname"][0].ToString();
        //        var objectCategory = item.Properties["objectcategory"][0].ToString().Split(',');

        //        var type = objectCategory[0].Substring(3, objectCategory[0].Length - 3);
        //        if (name.ToLower().Equals("guest") || !type.Equals("Person")) continue;

        //        viewModel.UserName = item.Properties["samaccountname"][0].ToString();

        //        // Todo: remove this
        //        viewModel.Email = item.Properties["mail"][0].ToString();
        //        if (item.Properties["mail"][0].ToString().ToLower().Equals("robinson aravena"))
        //            viewModel.Email = "robinson.aravena@asdas.org";

        //        viewModel.Name = name;

        //        adUsers.Add(viewModel);
        //    }

        //    return adUsers.OrderBy(e => e.Name).ToList();
        //}

        public List<ADUserViewModel> GetADUsers()
        {
            var adUsers = MemoryCache.Get(Constants.CacheAdUsers) as List<ADUserViewModel>;
            if (adUsers == null) //not in cache
            {
                adUsers = new List<ADUserViewModel>
                {
                    new ADUserViewModel {
                        Name = "Usuario Prueba 1",
                        Email = "prueba1@correo.com",
                        UserName = "usuarioPrueba1"
                    },
                    new ADUserViewModel {
                        Name = "Usuario Prueba 2",
                        Email = "prueba2@correo.com",
                        UserName = "usuarioPrueba2"
                    },
                    new ADUserViewModel {
                        Name = "Usuario Prueba 3",
                        Email = "prueba3@correo.com",
                        UserName = "usuarioPrueba3"
                    },
                    new ADUserViewModel {
                        Name = "Usuario Prueba 4",
                        Email = "prueba4@correo.com",
                        UserName = "usuarioPrueba4"
                    },
                    new ADUserViewModel {
                        Name = "Usuario Prueba 5",
                        Email = "prueba5@correo.com",
                        UserName = "usuarioPrueba5"
                    },
                    new ADUserViewModel {
                        Name = "Usuario Prueba 6",
                        Email = "prueba6@correo.com",
                        UserName = "usuarioPrueba6"
                    }
                };
                MemoryCache.Set(Constants.CacheAdUsers, adUsers);
            }

            return adUsers;
        }

        public void LoadBaseViewModel(IBaseViewModel model) {
            model.HeaderViewModel = HeaderViewModel;
        }
    }
}