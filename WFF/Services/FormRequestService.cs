using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WFF.Models;
using WFF.Utils;
using WFF.ViewModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace WFF.Services
{
    public class FormRequestService : IFormRequestService
    {
        public DataContext DataContext { get; set; }
        public IEmailService EmailService { get; set; }
        public IHistoryService HistoryService { get; set; }
        public IAttachmentService AttachmentService { get; set; }
        public IFormXmlService FormXmlService { get; set; }
        public IFormRequestViewModel FormRequestViewModel { get; set; }
        public IUserUtil UserUtil { get; set; }

        public FormRequestService(DataContext dataContext, IEmailService emailService, IHistoryService historyService, IAttachmentService attachmentService, 
                                  IFormXmlService formXmlService, IFormRequestViewModel formRequestViewModel, IUserUtil userUtil)
        {
            DataContext = dataContext;
            EmailService = emailService;
            AttachmentService = attachmentService;
            FormXmlService = formXmlService;
            HistoryService = historyService;
            FormRequestViewModel = formRequestViewModel;
            UserUtil = userUtil;
        }

        public void SaveFormRequest(IFormRequestViewModel viewModel, string formData, bool changingStatus = false)
        {
            FormRequest formRequest;
            var xmlForm = FormXmlService.LoadXmlForm(viewModel.FormId);
            var historyLog = string.Empty;
            var isNewRequest = viewModel.Id.Equals(0);

            if (isNewRequest)
            {
                formRequest = new FormRequest();
                formRequest.UserAssigned = UserUtil.DisplayUserName;

                if (xmlForm.SelectSingleNode("//body/referenceCode") != null)
                {
                    formRequest.Reference = GetNextReferenceCode(xmlForm.SelectSingleNode("//body/referenceCode").InnerXml);
                    FormXmlService.SetValue(ref formData, "Reference", formRequest.Reference);
                }

                historyLog = "Form request created.";
            }
            else
            {
                formRequest = DataContext.FormRequests.First(x => x.ID.Equals(viewModel.Id));
                historyLog = "Form request updated.";
            }

            formRequest.StatusId = viewModel.StatusId;
            formRequest.FormId = viewModel.FormId;
            formRequest.JSonFormData = formData;
            formRequest.CreatedBy = viewModel.CreatedBy;

            if (viewModel.Id == 0)
                DataContext.FormRequests.Add(formRequest);
            else
                formRequest.UserAssigned = viewModel.UserAssigned ?? UserUtil.DisplayUserName;

            DataContext.SaveChanges();
            viewModel.Id = formRequest.ID;

            if (viewModel.Attachments != null && viewModel.Attachments.Any())
                AttachmentService.SaveAttachments(viewModel);

            if (!changingStatus || isNewRequest)
                HistoryService.SaveHistory(viewModel.Id, historyLog);
        }

        public void ChangeStatus(string formData, ChangeStatusViewModel viewModel, UrlHelper url, string scheme)
        {
            FormRequestViewModel.Id = viewModel.RequestFormId;
            FormRequestViewModel.Reference = viewModel.Reference;
            FormRequestViewModel.StatusId = viewModel.NewStatus;
            FormRequestViewModel.FormId = viewModel.FormId;
            FormRequestViewModel.JSonFormData = formData;
            FormRequestViewModel.CreatedBy = viewModel.CreatedBy;

            FormXmlService.SetValue(ref formData, "statusId", viewModel.NewStatus);
            FormXmlService.SetValue(ref formData, "status", viewModel.NewStatusDisplayName);

            if (viewModel.RequestFormId == 0)
            {
                FormRequestViewModel.UserAssigned = UserUtil.DisplayUserName;
                FormXmlService.SetValue(ref formData, "userAssigned", UserUtil.DisplayUserName);
            }
            else
            {
                var xmlForm = FormXmlService.LoadXmlForm(viewModel.FormId);
                var userAssigned = FormXmlService.ParseFields(formData, xmlForm.SelectSingleNode("//body/statuses/status[name='" + viewModel.NewStatus + "']/userAssigned").InnerXml);
                FormRequestViewModel.UserAssigned = userAssigned;
                FormXmlService.SetValue(ref formData, "userAssigned", userAssigned);
            }

            SaveFormRequest(FormRequestViewModel, formData, true);

            if (!string.IsNullOrEmpty(viewModel.Action))
            {
                var log = FormXmlService.ParseFields(formData, viewModel.Action);
                HistoryService.SaveHistory(FormRequestViewModel.Id, log, viewModel.Comment);
            }

            if (!string.IsNullOrEmpty(viewModel.SendEmailNotification))
            {
                var emailTemplates = viewModel.SendEmailNotification.Split(',');
                foreach (var template in emailTemplates)
                {
                    viewModel.SendEmailNotification = template;
                    EmailService.SendEmail(formData, viewModel, url, scheme);
                }
            }
        }

        public List<FormRequestViewModel> GetFormRequests(int idForm, string userConsulting)
        {
            var viewModelList = new List<FormRequestViewModel>();

            try {
                var user = userConsulting.ToLower();
                var formList = new List<FormRequest>();
                
                if (!string.IsNullOrEmpty(user))
                    formList = DataContext.FormRequests.Where(m => 
                                                              m.FormId.Equals(idForm) &&
                                                              (m.UserAssigned.ToLower().Equals(user) || m.CreatedBy.ToLower().Equals(user))).ToList();
                else
                    formList = DataContext.FormRequests.Where(m => m.FormId.Equals(idForm)).ToList();
                
                foreach (var form in formList)
                    viewModelList.Add(Mapper.Map<FormRequest, FormRequestViewModel>(form));
            }
            catch(Exception ex) {
                throw new Exception(ex.Message);
            }

            return viewModelList;
        }

        public FormRequestViewModel GetFormRequest(int idRequest)
        {
            var formRequest = DataContext.FormRequests.FirstOrDefault(m => m.ID.Equals(idRequest));
            var viewModel = Mapper.Map<FormRequest, FormRequestViewModel>(formRequest);

            return viewModel;
        }

        public string GetNextReferenceCode(string referenceFormCode)
        {
            var reference = referenceFormCode + "/" + DateTime.Now.Year.ToString() + "/001";

            if (DataContext.FormRequests.Where(r => r.Reference.StartsWith(referenceFormCode)).Any())
            {
                reference = DataContext.FormRequests.Where(r => r.Reference.StartsWith(referenceFormCode)).OrderByDescending(r => r.ID).First().Reference;
                var num = int.Parse(reference.Split('/').Last());
                num++;

                reference = referenceFormCode + "/" + DateTime.Now.Year.ToString() + "/" + string.Format("{0:000}", num);
            }

            return reference;
        }
    }

    public interface IFormRequestService
    {
        DataContext DataContext { get; set; }
        void SaveFormRequest(IFormRequestViewModel viewModel, string formData, bool changingStatus = false);
        List<FormRequestViewModel> GetFormRequests(int idForm, string userConsulting);
        FormRequestViewModel GetFormRequest(int idRequest);
        string GetNextReferenceCode(string referenceFormCode);
        void ChangeStatus(string formData, ChangeStatusViewModel viewModel, UrlHelper url, string scheme);
    }
}