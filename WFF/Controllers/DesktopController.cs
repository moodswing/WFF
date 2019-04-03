using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using WFF.Services;
using WFF.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WFF.Utils;
using Microsoft.Extensions.Caching.Memory;
using WFF.Models;
using Microsoft.AspNetCore.Hosting;

namespace WFF.Controllers
{
    public class DesktopController : BaseController
    {
        public IDesktopViewModel DesktopViewModel { get; set; }
        public IFormRequestService FormRequestService { get; set; }
        public IHostingEnvironment HostingEnvironment { get; set; }

        public DesktopController(IMemoryCache cache, IDesktopViewModel desktopViewModel, IHeaderViewModel headerViewModel, IFormRequestService formRequestService,
                                 IADUserViewModel aDUserViewModel, IActionViewModel actionViewModel, IUserUtil userUtil, IBitacora bitacora, IHostingEnvironment environment) :
            base(cache, headerViewModel, aDUserViewModel, actionViewModel, userUtil, bitacora)
        {
            FormRequestService = formRequestService;
            DesktopViewModel = desktopViewModel;
            HostingEnvironment = environment;

            LoadBaseViewModel(DesktopViewModel);
        }

        public IActionResult Index(string message)
        {
            Logger.Info("LoginController -> Index");
            Bitacora.registraBitacora(System.Reflection.MethodBase.GetCurrentMethod(), HttpContext);

            var user = HttpContext.Session.GetObject<UserProfile>("UserSession");
            if (user == null)
                return View("~/Views/Shared/Error.cshtml");

            DesktopViewModel.DisplayMessage = message;

            DesktopViewModel.FilterByForms = GetXmlForms();

            if (HttpContext != null && MemoryCache.Get(Constants.CacheAdUsers) as List<ADUserViewModel> == null)
                GetADUsers();

            return View(DesktopViewModel);
        }

        private Dictionary<int, string> GetXmlForms()
        {
            var forms = new Dictionary<int, string>();
            var fileEntries = Directory.GetFiles(HostingEnvironment.ContentRootPath + Constants.XmlFormsPath, "*.xml");
            foreach (var fileName in fileEntries)
            {
                var document = new XmlDocument();
                document.Load(fileName);
                var title = document.SelectSingleNode("/body/title").InnerText;
                var idForm = int.Parse(document.SelectSingleNode("/body/idForm").InnerText);

                forms.Add(idForm, title);
            }

            return forms;
        }
    }
}