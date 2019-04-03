using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using WFF.Models;
using WFF.Models.Exceptions;

namespace WFF.Services
{
    public class LoginService : ILoginService
    {
        readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DataContext DataContext { get; set; }
        public IUserProfile User { get; set; }

        public LoginService(DataContext dataContext, IUserProfile user)
        {
            DataContext = dataContext;
            User = user;
        }

        //public bool IsUserValid(string sUser, string sPass)
        //{
        //    // Validar contra LDAP el usuario y Password
        //    var result = true;
        //    var hash = "";

        //    var ws = new verifyPassword();

        //    using (var md5Hash = MD5.Create())
        //    {
        //        hash = GetMd5Hash(md5Hash, sPass);
        //    }

        //    result = ws.validate(sUser, sPass);
        //    //User = DataContext.PerfilesUsuarios.First(u => u.Email.Equals(sUser) && u.Password.Equals(hash));

        //    if (!result)
        //        throw new LoginException(LoginException.USUARIO_SIN_BD);
        //    else
        //    {
        //        HttpContext.Current.Session["UserSession"] = sUser;
        //        HttpContext.Current.Session["UserEmailSession"] = GetUserDataByKeyLDAP(sUser, "mail");
        //        if (HttpContext.Current.Session["UserEmailSession"].ToString().ToLower().Contains("robinson"))
        //            HttpContext.Current.Session["UserEmailSession"] = "robinson.aravena@asdasd.org";

        //        HttpContext.Current.Session["UserNameSession"] = GetUserDataByKeyLDAP(sUser, "displayname");

        //        Logger.Info("Login Successful for " + sUser);
        //    }

        //    return result;
        //}

        //private string GetUserDataByKeyLDAP(string userName, string sKey)
        //{
        //    StringBuilder sRes = new StringBuilder();
        //    //--------------------------------------------------------------------
        //    //-- validamos si se ha ingresado el usuario        
        //    if (userName == null || userName.Length == 0 || sKey == null || sKey.Length == 0)
        //    {
        //        return "";                                    //-- no existe
        //    }
        //    //--------------------------------------------------------------------
        //    //-- procesamos
        //    DirectoryEntry entry = null;
        //    entry = new DirectoryEntry("asad://asda-asdasd-ad-p1.asdas.un.org:389", "asdasd", "asd@asdasd");
        //    //---------------------------------------------------------------------
        //    DirectorySearcher directorySearch = new DirectorySearcher(entry);
        //    directorySearch.Filter = "(samaccountname=" + userName + ")";
        //    //---------------------------------------------------------------------
        //    DirectorySearcher search = new DirectorySearcher(entry);
        //    SearchResult results = directorySearch.FindOne();
        //    StringBuilder sb = new StringBuilder();
        //    //---------------------------------------------------------------------
        //    if (results != null)
        //    {
        //        try
        //        {
        //            return results.Properties[sKey][0].ToString();  //sRes.ToString();// 
        //        }
        //        catch (Exception ex)
        //        {
        //            //-- si se esta consultando por la organization y esta no es provista por el LDAP
        //            //-- se usa la que esta configurada por defecto en el web.config
        //            if (sKey == "o")
        //            {
        //                return "asdasd";
        //            }
        //            else
        //            {
        //                return "ERROR: " + ex.Message;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        return "";                                //-- no existe
        //    }
        //    //--------------------------------------------------------------------

        //}

        private static string GetMd5Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }

    public interface ILoginService
    {
        //bool IsUserValid(string sUser, string sPass);
    }
}