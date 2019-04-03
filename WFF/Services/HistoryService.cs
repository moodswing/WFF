using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WFF.Models;
using WFF.Utils;

namespace WFF.Services
{
    public class HistoryService : IHistoryService
    {
        public UserUtil UserUtil { get; set; }
        public DataContext DataContext { get; set; }

        public HistoryService(UserUtil userUtil) {
            UserUtil = userUtil;
        }

        public void SaveHistory(int formRequestId, string log, string comment = "")
        {
            History history = new History();

            history.Log = log;
            history.Date = DateTime.Now;
            history.FormRequestID = formRequestId;
            history.Comment = comment;
            history.User = UserUtil.DisplayUserName;

            DataContext.History.Add(history);
            DataContext.SaveChanges();
        }

        public HistoryService(DataContext dataContext)
        {
            DataContext = dataContext;
        }
    }

    public interface IHistoryService
    {
        DataContext DataContext { get; set; }
        void SaveHistory(int formRequestId, string log, string comment = "");
    }
}