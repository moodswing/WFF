using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFF.ViewModels
{
    public class BaseViewModel : IBaseViewModel
    {
        public IHeaderViewModel HeaderViewModel { get; set; }
        public List<ADUserViewModel> ADUsers { get; set; }
        public string DisplayMessage { get; set; }
    }

    public interface IBaseViewModel
    {
        IHeaderViewModel HeaderViewModel { get; set; }
        List<ADUserViewModel> ADUsers { get; set; }
        string DisplayMessage { get; set; }
    }
}