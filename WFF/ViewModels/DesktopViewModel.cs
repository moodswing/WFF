using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFF.ViewModels
{
    public class DesktopViewModel : BaseViewModel, IDesktopViewModel
    {
        public Dictionary<int, string> FilterByForms { get; set; }
    }


    public interface IDesktopViewModel : IBaseViewModel
    {
        Dictionary<int, string> FilterByForms { get; set; }
    }
}