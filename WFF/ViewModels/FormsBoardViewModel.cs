using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFF.ViewModels
{
    public class FormsBoardViewModel : BaseViewModel, IFormsBoardViewModel
    {
        public int IdForm { get; set; }
        public List<FormRequestViewModel> FormRequests { get; set; }
        public List<ViewsGroupViewModel> GroupViews { get; set; }
        public string TypeOfView { get; set; }

        public ViewFormsViewModel CurrentView { get; set; }
        public JObject JSonResult { get; set; }
    }

    public interface IFormsBoardViewModel : IBaseViewModel
    {
        int IdForm { get; set; }
        List<FormRequestViewModel> FormRequests { get; set; }
        List<ViewsGroupViewModel> GroupViews { get; set; }
        string TypeOfView { get; set; }

        ViewFormsViewModel CurrentView { get; set; }
        JObject JSonResult { get; set; }
    }
}