using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using WFF.Models;
using WFF.Utils;
using WFF.ViewModels;

namespace WFF.Services
{
    public class AttachmentService : IAttachmentService
    {
        public UserUtil UserUtil { get; set; }
        public DataContext DataContext { get; set; }
        private readonly IOptions<Configuration> Config;

        public AttachmentService(UserUtil userUtil, IOptions<Configuration> config) {
            UserUtil = userUtil;
            Config = config;
        }

        public readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void SaveAttachments(IFormRequestViewModel formRequestViewModel)
        {
            foreach(var controlAttachment in formRequestViewModel.Attachments)
            {
                foreach(var file in controlAttachment.Attachments)
                {
                    var attachment = new Attachment();

                    attachment.Field = controlAttachment.ControlId;

                    var reference = string.Empty;
                    if (string.IsNullOrEmpty(formRequestViewModel.Reference) || formRequestViewModel.Reference.Equals("undefined"))
                        reference = formRequestViewModel.Id.ToString();

                    attachment.Location = UploadFile(file, reference);
                    attachment.FileName = file.FileName;
                    attachment.User = UserUtil.DisplayUserName;
                    attachment.FormRequestID = formRequestViewModel.Id;
                    attachment.Date = DateTime.Now;

                    DataContext.Attachments.Add(attachment);
                }
            }

            DataContext.SaveChanges();
        }

        private string UploadFile(IFormFile filebase, string reference)
        {
            var result = "";

            try
            {
                if (filebase.Length > 0)
                {
                    var fileName = Path.GetFileName(filebase.FileName);
                    var pathFolder = AppDomain.CurrentDomain.BaseDirectory + Config.Value.AttachmentsFolder + "/" + reference.Replace("/", "");

                    if (!Directory.Exists(pathFolder))
                        Directory.CreateDirectory(pathFolder);

                    var path = pathFolder + "/" + fileName;

                    using (var fileStream = new FileStream(path, FileMode.Create)) {
                        filebase.CopyToAsync(fileStream);
                    }

                    Logger.Info("File attached upload to: " + path);
                    result = path;
                }
            }
            catch (Exception ex)
            {
                Logger.Info("Ha ocurrido un error: " + ex.Message);
                result = "An error has happened in the saving process.";
            }

            return result;
        }

        public AttachmentService(DataContext dataContext)
        {
            DataContext = dataContext;
        }
    }

    public interface IAttachmentService
    {
        DataContext DataContext { get; set; }
        void SaveAttachments(IFormRequestViewModel formRequestViewModel);
    }
}