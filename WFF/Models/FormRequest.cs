using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFF.Models
{
    public class FormRequest : IFormRequest
    {
        public int ID { get; set; }
        public int FormId { get; set; }
        public string StatusId { get; set; }
        public string Reference { get; set; }
        public string CreatedBy { get; set; }
        public string UserAssigned { get; set; }
        public string JSonFormData { get; set; }
        public virtual List<History> History { get; set; }
        public virtual List<Attachment> Attachments { get; set; }
    }

    public interface IFormRequest
    {
        int ID { get; set; }
        int FormId { get; set; }
        string StatusId { get; set; }
        string Reference { get; set; }
        string CreatedBy { get; set; }
        string UserAssigned { get; set; }
        string JSonFormData { get; set; }
        List<History> History { get; set; }
        List<Attachment> Attachments { get; set; }
    }
}