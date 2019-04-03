using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFF.ViewModels
{
    public class HistoryViewModel : IHistoryViewModel
    {
        public DateTime Date { get; set; }
        public string Log { get; set; }
        public string Comment { get; set; }
        public string User { get; set; }
    }

    public interface IHistoryViewModel
    {
        DateTime Date { get; set; }
        string Log { get; set; }
        string Comment { get; set; }
        string User { get; set; }
    }
}