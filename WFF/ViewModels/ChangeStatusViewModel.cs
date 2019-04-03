using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFF.ViewModels
{
    public class ChangeStatusViewModel : IChangeStatusViewModel
    {
        public int RequestFormId { get; set; }
        public string NewStatus { get; set; }
        public string NewStatusDisplayName { get; set; }
        public string Reference { get; set; }
        public int FormId { get; set; }
        public string SendEmailNotification { get; set; }
        public string CreatedBy { get; set; }
        public string Comment { get; set; }
        public string Action { get; set; }
    }

    public interface IChangeStatusViewModel
    {
        int RequestFormId { get; set; }
        string NewStatus { get; set; }
        string NewStatusDisplayName { get; set; }
        string Reference { get; set; }
        int FormId { get; set; }
        string SendEmailNotification { get; set; }
        string CreatedBy { get; set; }
        string Comment { get; set; }
        string Action { get; set; }
    }
}