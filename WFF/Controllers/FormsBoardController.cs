using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Xml;
using WFF.Models;
using WFF.ViewModels;
using Microsoft.AspNetCore.Mvc;
using WFF.Services;
using Microsoft.Extensions.Caching.Memory;
using WFF.Utils;

namespace WFF.Controllers
{
    public class FormsBoardController : BaseController
    {
        public IFormsBoardViewModel FormsBoardViewModel { get; set; }
        public IFormRequestService FormRequestService { get; set; }
        private IFormXmlService FormXmlService { get; set; }
        private XmlDocument XmlForm { get; set; }

        public FormsBoardController(IMemoryCache cache, IFormsBoardViewModel formsBoardViewModel, IHeaderViewModel headerViewModel, IFormRequestService formRequestService,
                                    IFormXmlService formXmlService, IADUserViewModel aDUserViewModel, IActionViewModel actionViewModel, IUserUtil userUtil, IBitacora bitacora) :
        base(cache, headerViewModel, aDUserViewModel, actionViewModel, userUtil, bitacora)
        {
            FormRequestService = formRequestService;
            FormsBoardViewModel = formsBoardViewModel;
            FormXmlService = formXmlService;

            LoadBaseViewModel(FormsBoardViewModel);
        }

        public IActionResult Index(int idForm, string message)
        {
            Logger.Info("FormsBoardController -> Index");
            Bitacora.registraBitacora(System.Reflection.MethodBase.GetCurrentMethod(), HttpContext);

            FormsBoardViewModel.DisplayMessage = message;

            if (HttpContext != null && MemoryCache.Get(Constants.CacheAdUsers) as List<ADUserViewModel> == null)
                GetADUsers();

            FormsBoardViewModel.IdForm = HeaderViewModel.IdForm = idForm;

            if (HeaderViewModel.AddNewForm == null)
                LoadActions();

            GetViews(idForm);
            var groups = FormsBoardViewModel.GroupViews;
            if (groups != null && groups.Any())
            {
                var views = groups.First().Views;
                if (views != null && views.Any())
                    GetRequestForms(idForm, views.First().Id);
            }
                        

            return View(FormsBoardViewModel);
        }

        public IActionResult View(int idForm, int idView)
        {
            Logger.Info("FormsBoardController -> View");
            Bitacora.registraBitacora(System.Reflection.MethodBase.GetCurrentMethod(), HttpContext);

            if (HeaderViewModel.AddNewForm == null)
                LoadActions();

            FormsBoardViewModel.IdForm = idForm;

            GetViews(idForm);
            GetRequestForms(idForm, idView);

            return View("Index", FormsBoardViewModel);
        }

        private void GetRequestForms(int idForm, int idView)
        {
            var viewNode = XmlForm.SelectSingleNode("//body/views/group/view[viewId='" + idView.ToString() + "']");

            var groupView = FormsBoardViewModel.GroupViews.SelectMany(g => g.Views.Where(v => v.Id.Equals(idView))).ToList();
            FormsBoardViewModel.CurrentView = groupView.First();

            var getAllRequests = viewNode.SelectSingleNode("listAllRequests") != null && viewNode.SelectSingleNode("listAllRequests").InnerText == "true";
            var formRequests = FormRequestService.GetFormRequests(idForm, getAllRequests ? string.Empty : UserUtil.DisplayUserName);

            var jsonResult = new JObject();
            var row = 1;
            foreach (var request in formRequests)
            {
                var formData = JObject.Parse(request.JSonFormData);
                formData.Add("IdFormRequest", request.Id.ToString());

                if (viewNode.SelectSingleNode("filter") != null)
                {
                    var filter = viewNode.SelectSingleNode("filter").InnerText;
                    if (!FormXmlService.ValidateConditions(request.JSonFormData, filter))
                        continue;
                }

                jsonResult.Add(row.ToString(), formData);
                row++;
            }

            FormsBoardViewModel.FormRequests = formRequests;

            var groupedJson = GroupBy(jsonResult, FormsBoardViewModel.CurrentView.GroupBy);

            var orderByNode = viewNode.SelectSingleNode("orderBy");
            if (orderByNode != null && !FormsBoardViewModel.CurrentView.GroupBy.Contains("Date") && !FormsBoardViewModel.CurrentView.GroupBy.Contains("Calendar"))
                OrderBy(groupedJson, orderByNode.InnerText);

            FormsBoardViewModel.JSonResult = groupedJson;
        }

        private JObject GroupBy(JObject json, string parameter)
        {
            var result = new JObject();
            var cleanParameter = parameter.Replace("DateList:", "").Replace("Calendar:", "");

            if (parameter.Contains("DateList:"))
            {
                FormsBoardViewModel.TypeOfView = "DateList";
                foreach (var item in json)
                {
                    var groupedBy = item.Value[cleanParameter] == null || string.IsNullOrEmpty(item.Value[cleanParameter].ToString().Trim()) ?
                        Constants.FilterFieldNotSetYet : item.Value[cleanParameter].ToString();

                    if (groupedBy != Constants.FilterFieldNotSetYet)
                    {
                        var date = DateTime.Parse(item.Value[cleanParameter].ToString());
                        groupedBy = date.Year.ToString();
                        var groupedByMonth = date.ToString("MMMM", CultureInfo.CreateSpecificCulture("es"));

                        if (result[groupedBy] == null)
                        {
                            var monthly = new JObject();
                            var monthlyList = new JArray(item.Value);
                            monthly.Add(groupedByMonth, monthlyList);

                            var list = new JArray(monthly);
                            result.Add(groupedBy, list);
                        }
                        else
                        {
                            if (result[groupedBy].Value<JToken>()[0][groupedByMonth] == null)
                            {
                                var monthlyList = new JArray(item.Value);
                                result.SelectToken(groupedBy).ToList()[0].Last().AddAfterSelf(new JProperty(groupedByMonth, monthlyList));
                            }
                            else
                            {
                                var item2 = result[groupedBy].Value<JToken>()[0][groupedByMonth];
                                ((JArray)item2).Add(item.Value);
                            }
                        }
                    }
                    else
                        AddJsonElement(result, groupedBy, item.Value);
                }
            }
            else if (parameter.Contains("Calendar:"))
            {
                FormsBoardViewModel.TypeOfView = "Calendar";
            }
            else
            {
                FormsBoardViewModel.TypeOfView = "List";
                foreach (var item in json)
                {
                    var groupedBy = "";
                    if (item.Value[cleanParameter] == null)
                        groupedBy = "Inbox";
                    else
                        groupedBy = string.IsNullOrEmpty(item.Value[parameter].ToString().Trim()) ?
                        "Undefined" : item.Value[parameter].ToString();

                    AddJsonElement(result, groupedBy, item.Value);
                }
            }

            return result;
        }

        private void OrderBy(JObject json, string parameter)
        {
            var newJson = new JObject();
            foreach (var item in json.Properties())
            {
                var list = (JArray)item.Value;
                list = new JArray(list.OrderBy(obj => (string)obj[parameter]));
                json[item.Name] = list;
            }
        }

        public void AddJsonElement(JObject json, string at, object value)
        {
            if (json[at] == null)
            {
                var list = new JArray(value);
                json.Add(at, list);
            }
            else
            {
                var item2 = json[at];
                ((JArray)item2).Add(value);
            }
        }

        private void GetViews(int idForm)
        {
            XmlForm = FormXmlService.LoadXmlForm(idForm);

            FormsBoardViewModel.GroupViews = new List<ViewsGroupViewModel>();
            var groupViews = XmlForm.SelectNodes("//body/views/group");

            foreach (XmlNode nodeGroup in groupViews)
            {
                if (nodeGroup.SelectSingleNode("visible") != null)
                {
                    var visibleNode = nodeGroup.SelectSingleNode("visible").InnerText;
                    if (!FormXmlService.ValidateConditions(null, visibleNode))
                        continue;
                }

                var titleGroup = nodeGroup["title"].InnerText;
                if (titleGroup.Equals("User()"))
                    titleGroup = UserUtil.DisplayUserName;

                var groupView = new ViewsGroupViewModel
                {
                    Title = titleGroup,
                    Views = new List<ViewFormsViewModel>()
                };

                var views = nodeGroup.SelectNodes("view");
                foreach(XmlNode nodeView in views)
                {
                    if (nodeView.SelectSingleNode("visible") != null)
                    {
                        var visibleNode = nodeView.SelectSingleNode("visible").InnerText;
                        if (!FormXmlService.ValidateConditions(null, visibleNode))
                            continue;
                    }

                    var view = new ViewFormsViewModel
                    {
                        Id = int.Parse(nodeView["viewId"].InnerText),
                        Title = nodeView["name"].InnerText,
                        GroupBy = nodeView["groupBy"] == null ? "" : nodeView["groupBy"].InnerText,
                        ViewColumns = new List<ViewColumn>()
                    };

                    var columns = nodeView.SelectNodes("columns/column");
                    foreach (XmlNode nodeColumn in columns)
                    {
                        var column = new ViewColumn
                        {
                            Name = nodeColumn["name"].InnerText,
                            ControlId = nodeColumn["controlId"] == null ? null : nodeColumn["controlId"].InnerText,
                            FunctionControlId = nodeColumn["functionControlId"] == null ? null : nodeColumn["functionControlId"].InnerText
                        };

                        view.ViewColumns.Add(column);
                    }

                    groupView.Views.Add(view);
                }

                FormsBoardViewModel.GroupViews.Add(groupView);
            }
        }

        private void LoadActions()
        {
            var addNewForm = (ActionViewModel)ActionViewModel.Clone();

            HeaderViewModel.AddNewForm = addNewForm;
            HeaderViewModel.AddNewForm.Title = Constants.NewRequest;

            var returnForm = (ActionViewModel)ActionViewModel.Clone();

            HeaderViewModel.ReturnAction = returnForm;
            HeaderViewModel.ReturnAction.Title = Constants.ReturnAction;
        }
    }
}