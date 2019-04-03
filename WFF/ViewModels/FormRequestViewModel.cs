using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFF.ViewModels
{
    public class FormRequestViewModel : BaseViewModel, IFormRequestViewModel
    {
        public string Reference { get; set; }
        public int Id { get; set; }
        public int FormId { get; set; }
        public string StatusId { get; set; }
        public string StatusDisplayName { get; set; }
        public string HtmlForm { get; set; }
        public string CreatedBy { get; set; }
        public string UserAssigned { get; set; }
        public string JSonFormData { get; set; }
        public List<HistoryViewModel> History { get; set; }
        public List<FieldAttachmentViewModel> Attachments { get; set; }
    }

    public interface IFormRequestViewModel : IBaseViewModel
    {
        string Reference { get; set; }
        int Id { get; set; }
        int FormId { get; set; }
        string StatusId { get; set; }
        string StatusDisplayName { get; set; }
        string HtmlForm { get; set; }
        string CreatedBy { get; set; }
        string UserAssigned { get; set; }
        string JSonFormData { get; set; }
        List<HistoryViewModel> History { get; set; }
        List<FieldAttachmentViewModel> Attachments { get; set; }
    }
}