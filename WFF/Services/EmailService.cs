using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using WFF.ViewModels;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using WFF.Utils;

namespace WFF.Services
{
    public class EmailService : IEmailService
    {
        public IFormXmlService FormXmlService { get; set; }
        private readonly IOptions<Configuration> Config;

        public readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public EmailService(IFormXmlService formXmlService, IOptions<Configuration> config)
        {
            FormXmlService = formXmlService;
            Config = config;
        }

        public void SendEmail(string formData, ChangeStatusViewModel viewModel, UrlHelper url, string scheme)
        {
            try
            {
                var emailTemplateNode = "//body/emailNotificationTemplates/emailNotificationTemplate";

                var xmlForm = FormXmlService.LoadXmlForm(viewModel.FormId);
                FormXmlService.XmlForm = xmlForm;
                FormXmlService.Url = url;
                FormXmlService.Scheme = scheme;

                var senderMail = "WFF_info@asda.org";
                var receivers = FormXmlService.ParseFields(formData, xmlForm.SelectSingleNode(emailTemplateNode + "[templateId='" + viewModel.SendEmailNotification + "']/recipient").InnerXml).Split(',');
                var smtpserver = String.Format("{0}", Config.Value.SmtpServer);
                string objectMail = FormXmlService.ParseFields(formData, xmlForm.SelectSingleNode(emailTemplateNode + "[templateId='" + viewModel.SendEmailNotification + "']/subject").InnerXml);
                string bodyMail = FormXmlService.ParseFields(formData, xmlForm.SelectSingleNode(emailTemplateNode + "[templateId='" + viewModel.SendEmailNotification + "']/body").InnerXml, viewModel.RequestFormId, viewModel.Comment);
                int numPort = Int32.Parse(String.Format("{0}", Config.Value.SmtpPort));
                //int i = 0;

                //var ws = new verifyPassword();
                //var receiverEmail = ws.GetUserDataByKeyLDAP("vvergara1", "mail");

                //var receiverMail = LdapIdentify.
                var mail = new MailMessage(senderMail, receivers[0], objectMail, bodyMail);
                if (receivers.Count() > 1)
                    foreach (var receiver in receivers)
                        mail.To.Add(receiver);

                SmtpClient client = new SmtpClient(smtpserver);
                client.Port = numPort;

                // Send the mail
                Task.Run(() => {
                    try
                    {
                        client.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        Logger.Info("An error has occurred sending the notification email: " + ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Info("An error has occurred: " + ex.Message);
            }

            Logger.Info("Email sent successfully.");
        }
    }

    public interface IEmailService
    {
        void SendEmail(string formData, ChangeStatusViewModel viewModel, UrlHelper url, string scheme);
    }
}