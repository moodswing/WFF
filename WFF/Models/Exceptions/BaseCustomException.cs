using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization;
using System.Web;

namespace WFF.Models.Exceptions
{
    public class BaseCustomException : System.Exception
    {
        private static string RESOURCE_FILE = "ASDS.Resource1";

        // Codigos genericos
        public static string GENERICO = "ERROR1000";

        // Constructor necesario para serializacion
        protected BaseCustomException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        // codigo
        public BaseCustomException(string codigo) : base(findMessage(codigo))
        {
        }

        // Codigo y excepcion original
        public BaseCustomException(string codigo, Exception innerException) : base(findMessage(codigo), innerException)
        {
        }

        // Codigo y mensaje adicional
        public BaseCustomException(string codigo, string message) : base(findMessage(codigo) + " " + message)
        {
        }

        // Busca mensaje en archivo de recuersos
        private static string findMessage(string codigo)
        {
            var rm = new ResourceManager(RESOURCE_FILE, Assembly.GetExecutingAssembly());
            return rm.GetString(codigo);
        }
    }
}