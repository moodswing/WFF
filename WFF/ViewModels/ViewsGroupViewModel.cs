using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFF.ViewModels
{
    public class ViewsGroupViewModel : BaseViewModel, IViewsGroupViewModel
    {
        public string Title { get; set; }
        public List<ViewFormsViewModel> Views { get; set; }
    }

    public interface IViewsGroupViewModel
    {
        string Title { get; set; }
        List<ViewFormsViewModel> Views { get; set; }
    }
}