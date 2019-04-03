using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFF.ViewModels
{
    public class HeaderViewModel : IHeaderViewModel
    {
        public IActionViewModel AddNewForm { get; set; }
        public IActionViewModel ReturnAction { get; set; }
        public IActionViewModel SaveForm { get; set; }
        public IActionViewModel CloseForm { get; set; }
        public List<IActionViewModel> FormActions { get; set; }

        public int IdForm { get; set; }
    }

    public interface IHeaderViewModel
    {
        IActionViewModel AddNewForm { get; set; }
        IActionViewModel ReturnAction { get; set; }
        IActionViewModel SaveForm { get; set; }
        IActionViewModel CloseForm { get; set; }
        List<IActionViewModel> FormActions { get; set; }

        int IdForm { get; set; }
    }
}