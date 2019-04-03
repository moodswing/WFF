using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFF.ViewModels
{
    public class ActionViewModel : IActionViewModel
    {
        public string Id { get; set; }
        public int RequestFormId { get; set; }
        public string Title { get; set; }
        public string NewStatus { get; set; }
        public string NewStatusDisplayName { get; set; }
        public string NewUserAssigned { get; set; }
        public string RequiredFields { get; set; }
        public string SendEmailNotification { get; set; }
        public string ConfirmationText { get; set; }
        public string HistoryText { get; set; }
        public bool CommentRequired { get; set; }
        public string AddCommentToList { get; set; }
        public bool CancelStatusChange { get; set; }
        public string JSFunction { get; set; }

        public string VisibleValidation { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public interface IActionViewModel : ICloneable
    {
        string Id { get; set; }
        int RequestFormId { get; set; }
        string Title { get; set; }
        string NewStatus { get; set; }
        string NewStatusDisplayName { get; set; }
        string NewUserAssigned { get; set; }
        string RequiredFields { get; set; }
        string SendEmailNotification { get; set; }
        string ConfirmationText { get; set; }
        string HistoryText { get; set; }
        bool CommentRequired { get; set; }
        string AddCommentToList { get; set; }
        bool CancelStatusChange { get; set; }
        string JSFunction { get; set; }
        string VisibleValidation { get; set; }
    }
}