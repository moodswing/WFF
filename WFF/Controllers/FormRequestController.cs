using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Xsl;
using WFF.Models;
using WFF.ViewModels;
using Microsoft.AspNetCore.Mvc;
using WFF.Services;
using Microsoft.Extensions.Caching.Memory;
using WFF.Utils;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Hosting;

namespace WFF.Controllers
{
    public class FormRequestController : BaseController
    {
        private IFormRequestViewModel FormRequestViewModel { get; set; }
        private IFormRequest FormRequest { get; set; }
        private IFormRequestService FormRequestService { get; set; }
        private IFormXmlService FormXmlService { get; set; }
        private XmlDocument XmlForm { get; set; }
        public IHostingEnvironment HostingEnvironment { get; set; }

        public FormRequestController(IMemoryCache cache, IFormRequestViewModel formRequestViewModel, IFormRequest formRequest, IHeaderViewModel headerViewModel, IFormRequestService formRequestService,
                                     IFormXmlService formXmlService, IADUserViewModel aDUserViewModel, IActionViewModel actionViewModel, IHostingEnvironment environment, IUserUtil userUtil, IBitacora bitacora) :
        base(cache, headerViewModel, aDUserViewModel, actionViewModel, userUtil, bitacora)
        {
            FormRequestService = formRequestService;
            FormRequest = formRequest;
            FormRequestViewModel = formRequestViewModel;
            FormXmlService = formXmlService;
            HostingEnvironment = environment;
        }

        public IActionResult NewRequest(int idForm, string language)
        {
            FormRequestViewModel.HtmlForm = GetFormHtml(idForm, true);
            PopulateViewModel();

            return View("FormRequest", FormRequestViewModel);
        }

        public IActionResult EditRequest(int idRequest)
        {
            FormRequestViewModel = FormRequestService.GetFormRequest(idRequest);
            FormRequestViewModel.HtmlForm = GetFormHtml(FormRequestViewModel.FormId, false);
            PopulateViewModel();

            return View("FormRequest", FormRequestViewModel);
        }

        private string GetFormHtml(int idForm, bool newForm)
        {
            try {
                var xmlHtml = new StringWriter();
                var xslt = new XslCompiledTransform();
                var settings = new XsltSettings();
                settings.EnableScript = true;
                settings.EnableDocumentFunction = true;
                XsltArgumentList args = new XsltArgumentList();

                xslt.Load(HostingEnvironment.ContentRootPath + "/XmlForms/FormTransform.xslt", settings, new XmlUrlResolver());

                XmlForm = FormXmlService.LoadXmlForm(idForm);
                FillBaseFormData(XmlForm, newForm);
                xslt.Transform(XmlForm, null, xmlHtml);

                return xmlHtml.ToString();    
            }
            catch (Exception e) {
                throw new Exception(e.Message);    
            }
        }

        [HttpPost]
        public IActionResult Save(string formData, FormRequestViewModel viewModel)
        {
            var jsonFormData = JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(formData) ? "{}" : formData);

            if (ModelState.IsValid)
            {
                try
                {
                    FormRequestService.SaveFormRequest(viewModel, formData);
                }
                catch (Exception ex)
                {
                    Logger.Info("Ha ocurrido un error: " + ex.Message);
                    TempData[Constants.InfoKey] = "An error has happened in the saving process.";
                }
            }

            var redirectTo = Url.Action("Index", "FormsBoard", new { idForm = viewModel.FormId, message = "Request Form created successfully!" });
            return Json(new { RedirectTo = redirectTo });
        }

        [HttpPost]
        public IActionResult ChangeStatus(string formData, ChangeStatusViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var urlScheme = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
                    FormRequestService.ChangeStatus(formData, viewModel, (UrlHelper)Url, urlScheme);
                }
                catch (Exception ex)
                {
                    Logger.Info("Ha ocurrido un error: " + ex.Message);
                    TempData[Constants.InfoKey] = "An error has happened in the saving process.";
                }
            }

            return Json(new { RedirectTo = Url.Action("Index", "FormsBoard", new { idForm = viewModel.FormId, message = "Action executed successfully!" }) });
        }

        private void PopulateViewModel()
        {
            LoadBaseViewModel(FormRequestViewModel);
            FormRequestViewModel.ADUsers = GetADUsers();
            LoadActions(FormRequestViewModel.StatusId);
        }

        private void LoadActions(string status)
        {
            //UpdateCulture();

            var closeForm = (IActionViewModel)ActionViewModel.Clone();
            HeaderViewModel.CloseForm = closeForm;
            //HeaderViewModel.CloseForm.Title = Resources.Language.lblCloseForm;
            HeaderViewModel.CloseForm.Title = Constants.Close;

            if (FormRequestViewModel.UserAssigned.ToLower() != UserUtil.DisplayUserName.ToLower())
                return;

            var saveForm = (IActionViewModel)ActionViewModel.Clone();
            HeaderViewModel.SaveForm = saveForm;
            //HeaderViewModel.SaveForm.Title = Resources.Language.lblSaveForm;
            HeaderViewModel.SaveForm.Title = Constants.Save;

            if (!string.IsNullOrEmpty(status) && XmlForm != null)
            {
                FormRequestViewModel.HeaderViewModel.FormActions = new List<IActionViewModel>();
                var actionNodes = XmlForm.SelectNodes("//body/statuses/status[name='" + status + "']/actions/action");

                foreach(XmlNode action in actionNodes)
                {
                    var visibleCondition = "";
                    if (action["visible"] != null)
                    {
                        visibleCondition = action["visible"].InnerText;
                        string[] functions = { "parameter(", "role(" };
                        foreach(var func in functions)
                        {
                            while (visibleCondition.Contains(func))
                            {
                                var index = visibleCondition.IndexOf(func);
                                var endIndex = visibleCondition.IndexOf(")", index);
                                var parameter = visibleCondition.Substring(index, endIndex - index + 1);
                                var value = FormXmlService.GetValue(FormRequestViewModel.JSonFormData, parameter);

                                visibleCondition = visibleCondition.Replace(parameter, "'" + value + "'");
                            }
                        }
                    }

                    var newStatusNode = XmlForm.SelectSingleNode("//body/statuses/status[name='" + action["targetStatus"].InnerText + "']");

                    var newAction = new ActionViewModel
                    {
                        Id = action["name"].InnerText,
                        RequestFormId = FormRequestViewModel.Id,
                        Title = action["displayName"].InnerText,
                        RequiredFields = action["requiredFields"] == null ? null : action["requiredFields"].InnerText,
                        SendEmailNotification = action["sendEmailNotification"] == null ? null : action["sendEmailNotification"].InnerText,
                        ConfirmationText = action["confirmationText"] == null ? "Are you sure you want to realize this action?" : action["confirmationText"].InnerText,
                        CommentRequired = action["commentRequired"] == null || action["commentRequired"].InnerText != "true" ? false : true,
                        HistoryText = action["historyText"] == null ? string.Empty : action["historyText"].InnerText,
                        VisibleValidation = action["visible"] == null ? "" : visibleCondition,
                        JSFunction = action["jsFunction"] == null ? "" : action["jsFunction"].InnerText,
                        AddCommentToList = action["addCommentToList"] == null ? "" : action["addCommentToList"].InnerText,
                        CancelStatusChange = action["cancelStatusChange"] == null || string.IsNullOrEmpty(action["cancelStatusChange"].InnerText) || action["cancelStatusChange"].InnerText.ToLower() != "true" ? false : true,
                        NewStatus = action["targetStatus"].InnerText,
                        NewStatusDisplayName = newStatusNode["displayName"].InnerText
                    };

                    FormRequestViewModel.HeaderViewModel.FormActions.Add(newAction);
                }
            }
        }

        private void FillBaseFormData(XmlDocument xml, bool newForm)
        {
            var statusDisplayName = string.Empty;
            var status = string.Empty;
            var reference = string.Empty;

            if (newForm)
            {
                statusDisplayName = xml.SelectSingleNode("//body/statuses/status[order=0]/displayName").InnerXml;
                status = xml.SelectSingleNode("//body/statuses/status[order=0]/name").InnerXml;
                FormRequestViewModel.CreatedBy = UserUtil.DisplayUserName;
                FormRequestViewModel.UserAssigned = UserUtil.DisplayUserName;

                if (xml.SelectSingleNode("//body/initialInput/createdBy") != null)
                    xml.SelectSingleNode("//body/initialInput/createdBy").InnerXml = UserUtil.DisplayUserName;
                if (xml.SelectSingleNode("//body/initialInput/createdByEmail") != null)
                    xml.SelectSingleNode("//body/initialInput/createdByEmail").InnerXml = UserUtil.UserEmail;
                if (xml.SelectSingleNode("//body/initialInput/createdDate") != null)
                    xml.SelectSingleNode("//body/initialInput/createdDate").InnerXml = DateTime.Now.ToString("MM/dd/yyyy");
                if (xml.SelectSingleNode("//body/initialInput/userAssigned") != null)
                    xml.SelectSingleNode("//body/initialInput/userAssigned").InnerXml = UserUtil.DisplayUserName;

                if (xml.SelectSingleNode("//body/fields/field[controlId='Reference']") != null)
                {
                    reference = FormRequestService.GetNextReferenceCode(xml.SelectSingleNode("//body/referenceCode").InnerXml);
                    xml.SelectSingleNode("//body/fields/field[controlId='Reference']/value").InnerXml = reference;
                }
            }
            else
            {
                statusDisplayName = xml.SelectSingleNode("//body/statuses/status[name='" + FormRequestViewModel.StatusId + "']/displayName").InnerXml;
                status = xml.SelectSingleNode("//body/statuses/status[name='" + FormRequestViewModel.StatusId + "']/name").InnerXml;

                if (xml.SelectSingleNode("//body/initialInput/formRequestId") != null)
                    xml.SelectSingleNode("//body/initialInput/formRequestId").InnerXml = FormRequestViewModel.Id.ToString();
                if (xml.SelectSingleNode("//body/initialInput/createdBy") != null)
                    xml.SelectSingleNode("//body/initialInput/createdBy").InnerXml = FormXmlService.GetValue(FormRequestViewModel.JSonFormData, "field(createdBy)");
                if (xml.SelectSingleNode("//body/initialInput/createdByEmail") != null)
                    xml.SelectSingleNode("//body/initialInput/createdByEmail").InnerXml = FormXmlService.GetValue(FormRequestViewModel.JSonFormData, "field(createdByEmail)");
                if (xml.SelectSingleNode("//body/initialInput/createdDate") != null)
                    xml.SelectSingleNode("//body/initialInput/createdDate").InnerXml = FormXmlService.GetValue(FormRequestViewModel.JSonFormData, "field(createdDate)");
                if (xml.SelectSingleNode("//body/initialInput/userAssigned") != null)
                    xml.SelectSingleNode("//body/initialInput/userAssigned").InnerXml = FormRequestViewModel.UserAssigned;

                if (xml.SelectSingleNode("//body/fields/field[controlId='Reference']") != null)
                    xml.SelectSingleNode("//body/fields/field[controlId='Reference']/value").InnerXml = FormXmlService.GetValue(FormRequestViewModel.JSonFormData, "field(reference)");

                LoadAttachedFilesIntoXML(xml);
            }

            if (xml.SelectSingleNode("//body/initialInput/currentUser") != null)
                xml.SelectSingleNode("//body/initialInput/currentUser").InnerXml = UserUtil.DisplayUserName;
            if (xml.SelectSingleNode("//body/initialInput/currentUserEmail") != null)
                xml.SelectSingleNode("//body/initialInput/currentUserEmail").InnerXml = UserUtil.UserEmail;

            if (xml.SelectSingleNode("//body/initialInput/status") != null)
                xml.SelectSingleNode("//body/initialInput/status").InnerXml = statusDisplayName;
            if (xml.SelectSingleNode("//body/initialInput/statusId") != null)
                xml.SelectSingleNode("//body/initialInput/statusId").InnerXml = status;

            FormRequestViewModel.FormId = int.Parse(xml.SelectSingleNode("//body/idForm").InnerXml);
            FormRequestViewModel.StatusDisplayName = statusDisplayName;
            FormRequestViewModel.StatusId = status;

            if (!string.IsNullOrEmpty(reference))
                FormRequestViewModel.Reference = reference;
        }

        private void LoadAttachedFilesIntoXML(XmlDocument xml)
        {
            if (FormRequestViewModel.Attachments != null && FormRequestViewModel.Attachments.Any())
            {
                var initialInputsNode = xml.SelectSingleNode("//body/initialInput");
                var attachmentsNode = xml.CreateElement("attachments");

                foreach (var fieldAttachments in FormRequestViewModel.Attachments)
                {
                    var attachmentNode = xml.CreateElement("attachment");

                    AddNode(xml, attachmentNode, "field", fieldAttachments.ControlId);

                    foreach (var file in fieldAttachments.Files)
                    {
                        var fileNode = xml.CreateElement("file");

                        var fileLocation = file.Location;
                        fileLocation = fileLocation.Replace(HostingEnvironment.ContentRootPath, "/WFF/");
                        
                        AddNode(xml, fileNode, "id", file.Id.ToString());
                        AddNode(xml, fileNode, "fileName", file.FileName);
                        AddNode(xml, fileNode, "location", fileLocation);

                        attachmentNode.AppendChild(fileNode);
                    }

                    attachmentsNode.AppendChild(attachmentNode);
                }

                initialInputsNode.AppendChild(attachmentsNode);
            }
        }

        private void AddNode(XmlDocument document, XmlElement node, string name, string value)
        {
            var newNode = document.CreateElement(name);
            newNode.InnerText = value;
            node.AppendChild(newNode);
        }
    }
}