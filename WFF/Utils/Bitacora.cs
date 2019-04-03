using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using WFF.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Features;

namespace WFF.Utils
{
    public class Bitacora : IBitacora
    {
        private IHttpContextAccessor HttpContextAccessor;
        readonly log4net.ILog logger = log4net.LogManager.GetLogger("WFF", "BitacoraLog");

        public Bitacora(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public void registraBitacora(MethodBase method, HttpContext Request)
        {
            string bitacora = "";

            bitacora += getClassInfo(method) + ", ";
            bitacora += getUserInfo() + ", ";
            bitacora += getBrowserInfo(Request.Request) + ", ";
            bitacora += getIPAddress(Request) + ", ";

            logger.Info("BITACORA: " + bitacora);
        }

        private string getBrowserInfo(HttpRequest Request)
        {
            string browserInfo = "";
            var browser = Request.Headers["User-Agent"];
            browserInfo += "Browser=[" + browser + "]" + ", ";
            browserInfo += "VersionBrowser=[" + browser + "]" + ", ";

            return browserInfo;
        }

        private string getClassInfo(MethodBase method)
        {
            string className = method.DeclaringType.Name;
            string methodName = method.Name;

            return "Clase=[" + className + "], Metodo=[" + methodName + "]";
        }

        private string getUserInfo()
        {
            string usuario = "Guest";

            UserProfile user = HttpContextAccessor.HttpContext.Session.GetObject<UserProfile>("UserSession") as UserProfile;
            if (user != null)
                usuario = user.Email;

            return "Usuario=[" + usuario + "]";
        }

        public string getIPAddress(HttpContext Request)
        {
            string ip = Request.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();

            return "IP=[" + ip + "]";
        }
    }

    public interface IBitacora {
        void registraBitacora(MethodBase method, HttpContext Request);
        string getIPAddress(HttpContext Request);
    }
}