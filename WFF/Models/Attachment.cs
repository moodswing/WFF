using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFF.Models
{
    public class Attachment
    {
        public int ID { get; set; }
        public string Field { get; set; }
        public string FileName { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string User { get; set; }
        public int FormRequestID { get; set; }
        public virtual FormRequest FormRequest { get; set; }
    }

    public interface IAttachment
    {
        int ID { get; set; }
        string Field { get; set; }
        string FileName { get; set; }
        DateTime Date { get; set; }
        string Location { get; set; }
        string User { get; set; }
        int FormRequestID { get; set; }
        FormRequest FormRequest { get; set; }
    }
}