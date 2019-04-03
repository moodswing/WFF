using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WFF.Models.Exceptions
{
    public class LoginException : BaseCustomException
    {
        // Codigos de error
        public static string USUARIO_SIN_LDAP = "ERROR3000";
        public static string USUARIO_SIN_BD = "ERROR3001";

        public LoginException(string key, Exception innerException) : base(key, innerException)
        {
        }

        public LoginException(string key, string message) : base(key, message)
        {
        }

        public LoginException(string key) : base(key)
        {
        }

        protected LoginException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}