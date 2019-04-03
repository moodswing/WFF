using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFF.Models
{
    public class History
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string Log { get; set; }
        public string Comment { get; set; }
        public string User { get; set; }
        public int FormRequestID { get; set; }
        public virtual FormRequest FormRequest { get; set; }
    }

    public interface IHistory
    {
        int ID { get; set; }
        DateTime Date { get; set; }
        string Log { get; set; }
        string Comment { get; set; }
        string User { get; set; }
        int FormRequestID { get; set; }
        FormRequest FormRequest { get; set; }
    }
}