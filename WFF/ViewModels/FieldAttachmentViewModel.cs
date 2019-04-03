using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace WFF.ViewModels
{
    public class FieldAttachmentViewModel
    {
        public string ControlId { get; set; }

        public List<AttachmentFile> Files { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }

    public struct AttachmentFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Location { get; set; }
    }

    public interface IFieldAttachmentViewModel
    {
        string ControlId { get; set; }
        List<AttachmentFile> Files { get; set; }
        List<IFormFile> Attachments { get; set; }
    }
}