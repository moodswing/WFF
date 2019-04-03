using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFF.ViewModels
{
    public class ADUserViewModel : IADUserViewModel
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public interface IADUserViewModel
    {
        string UserName { get; set; }
        string Name { get; set; }
        string Email { get; set; }
    }
}