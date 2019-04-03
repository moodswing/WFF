using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WFF.Models;

namespace WFF.Utils
{
    public class UserUtil : IUserUtil
    {
        private IHttpContextAccessor HttpContextAccessor;

        public UserUtil(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public string DisplayUserName
        {
            get
            {
                if (HttpContextAccessor.HttpContext.Session.GetString("UserNameSession") == null)
                    return string.Empty;

                return HttpContextAccessor.HttpContext.Session.GetString("UserNameSession");
            }
            //var ctx = new PrincipalContext(ContextType.Domain);
            //var user = UserPrincipal.FindByIdentity(ctx, Environment.UserName);
        }

        public string UserName
        {
            get 
            {
                if (HttpContextAccessor.HttpContext.Session.GetObject<UserProfile>("UserSession") == null)
                    return string.Empty;

                return HttpContextAccessor.HttpContext.Session.GetString("UserSession");
            }
        }

        public string UserEmail
        {
            get
            {
                if (HttpContextAccessor.HttpContext.Session.GetString("UserEmailSession") == null)
                    return string.Empty;

                return HttpContextAccessor.HttpContext.Session.GetString("UserEmailSession");
            }
        }
    }

    public interface IUserUtil {
        string DisplayUserName { get; }
        string UserName { get; }
        string UserEmail { get; }
    }
}