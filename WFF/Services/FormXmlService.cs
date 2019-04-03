using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using WFF.Utils;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace WFF.Services
{
    public class FormXmlService : IFormXmlService
    {
        public XmlDocument XmlForm { get; set; }
        public IUrlHelper Url { get; set; }
        public string Scheme { get; set; }
        public IUserUtil UserUtil { get; set; }
        public IHostingEnvironment HostingEnvironment;

        public FormXmlService(IUserUtil userUtil, IHostingEnvironment environment) {
            UserUtil = userUtil;
            HostingEnvironment = environment;
        }

        public string ParseFields(string formData, string nodeText, int requestId = 0, string comments = "")
        {
            string[] delimiters = { "field", "emailField", "role", "emailRole" };
            nodeText = nodeText.Replace("<![CDATA[", "").Replace("]]>", "");

            if (!string.IsNullOrEmpty(formData))
                foreach (var delim in delimiters) {
                    var regex = new Regex(delim + @"\(([^)]+)\)");
                    var matches = regex.Matches(nodeText);

                    foreach (var item in matches)
                    {
                        var match = item.ToString();
                        nodeText = nodeText.Replace(match, GetValue(formData, match));
                    }
                }

            var dateTimeFunc = "DateTime()";
            if (nodeText.Contains(dateTimeFunc))
                nodeText = nodeText.Replace(dateTimeFunc, DateTime.Today.ToString("dd/M/yyyy"));

            var linkFunc = "Link()";
            if (nodeText.Contains(linkFunc) && requestId != 0)
            {
                var actionContext = new UrlActionContext
                {
                    Action = "EditRequest",
                    Controller = "FormRequest",
                    Values = new { idRequest = requestId.ToString() },
                    Host = Scheme
                };
            }

            var userFunc = "User()";
            if (nodeText.Contains(userFunc))
                nodeText = nodeText.Replace(userFunc, UserUtil.DisplayUserName);

            var commentsFunc = "Comments()";
            if (nodeText.Contains(commentsFunc) && !string.IsNullOrEmpty(comments))
                nodeText = nodeText.Replace(commentsFunc, comments);

            var roleFunc = "role(";
            var emailRoleFunc = "roleEmail()";
            var parameterFunc = "parameter()";
            if (nodeText.Contains(roleFunc) || nodeText.Contains(emailRoleFunc) || nodeText.Contains(parameterFunc))
                nodeText = GetValue(formData, nodeText);

            return nodeText;
        }

        public XmlDocument LoadXmlForm(int idForm)
        {
            var fileEntries = Directory.GetFiles(HostingEnvironment.ContentRootPath + Constants.XmlFormsPath, "*.xml");
            foreach (var fileName in fileEntries)
            {
                var document = new XmlDocument();
                document.Load(fileName);
                var xmlIdForm = int.Parse(document.SelectSingleNode("/body/idForm").InnerText);

                if (idForm.Equals(xmlIdForm))
                {
                    XmlForm = new XmlDocument();
                    XmlForm.Load(fileName);

                    break;
                }
            }

            return XmlForm;
        }

        public string GetValue(string formData, string parameter)
        {
            var result = parameter;
            JObject jObj = new JObject();

            if (!string.IsNullOrEmpty(formData))
                jObj = JObject.Parse(formData);

            var node = parameter.Replace("field(", "").Replace("emailField(", "").Replace("role(", "").Replace("emailRole(", "").Replace("parameter(", "");

            if (!string.IsNullOrEmpty(node) && node.Last() == ')')
                node = node.Substring(0, node.Length - 1);

            if (parameter.Contains("role(") && XmlForm == null)
                LoadXmlForm(int.Parse(jObj["FormId"].ToString()));

            if (parameter.Contains("field(") && jObj[node] != null)
            {
                if (node.Contains(","))
                {
                    var nodeSplit = node.Split(',');
                    foreach(var field in nodeSplit)
                    {
                        if (jObj[field] != null && !string.IsNullOrEmpty(jObj[field].Value<string>()))
                        {
                            result = jObj[field].Value<string>();
                            break;
                        }
                    }
                }
                else 
                    result = jObj[node].Value<string>();
            }
            else if (parameter.Contains("emailField(") && jObj["email" + node] != null)
            {
                if (node.Contains(","))
                {
                    var nodeSplit = node.Split(',');
                    foreach (var field in nodeSplit)
                    {
                        if (jObj[field] != null && !string.IsNullOrEmpty(jObj["email" + field].Value<string>()))
                        {
                            result = jObj["email" + field].Value<string>();
                            break;
                        }
                    }
                }
                else
                    result = jObj["email" + node].Value<string>();
            }
            else if (parameter.Contains("role("))
            {
                var users = XmlForm.SelectNodes("//body/roles/role[id='" + node + "']/users/user");
                if (users.Count > 0) result = "";
                for (var i = 0; i < users.Count; i++)
                {
                    result += users[i].SelectSingleNode("name").InnerXml;
                    if (i != users.Count -1) result += ",";
                }
            }
            else if (parameter.Contains("emailRole("))
            {
                var users = XmlForm.SelectNodes("//body/roles/role[id='" + node + "']/users/user");
                if (users.Count > 0) result = "";
                for (var i = 0; i < users.Count; i++)
                {
                    result += users[i].SelectSingleNode("email").InnerXml;
                    if (i != users.Count - 1) result += ",";
                }
            }
            else if (parameter.Contains("parameter("))
                result = XmlForm.SelectSingleNode("//body/parameters/parameter[id='" + node + "']/value").InnerXml;

            return result;
        }

        public bool ValidateConditions(string formData, string validation)
        {
            var result = false;
            string[] delimiters = { " #and# ", " #or# " };

            var pieces = validation.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            var operatr = string.Empty;
            foreach (var condition in pieces)
            {
                if (!pieces.First().Equals(condition))
                    operatr = validation.Substring(validation.IndexOf(condition) - 7, 7) == " #and# " ? "and" : "or";

                if ((condition.Contains("NotIn(") || condition.Contains("In(")))
                {
                    var funct = condition.Contains("NotIn(") ? "NotIn(" : "In(";

                    var clearParameter = condition.Substring(funct.Length, condition.Length - (funct.Length + 1));
                    var parameters = clearParameter.Split(',');

                    var validateIn = ParseFields(formData, parameters[0].Trim()).ToLower();
                    var list = new string[] { };

                    var validateList = ParseFields(formData, parameters[1].Trim());
                    if (validateList.Contains("|"))
                        list = validateList.Split('|');
                    else
                        list = validateList.Split(',');

                    for (var i = 0; i < list.Count(); i++)
                        list[i] = list[i].ToLower();

                    var operation = funct == "In(" ? list.Contains(validateIn) : !list.Contains(validateIn);

                    if (operatr == "and") result = result && operation;
                    else if (operatr == "or") result = result || operation;
                    else result = operation;
                }
                else if (condition.Contains("!=") || condition.Contains("=="))
                {
                    var isEqual = condition.Contains("==");
                    string[] equalities = { "==", "!=" };
                    var constants = condition.Split(equalities, StringSplitOptions.None);

                    var value1 = ParseFields(formData, constants[0].Trim());
                    var value2 = ParseFields(formData, constants[1].Trim().Trim('\"'));

                    var comparisonResult = isEqual ? value1.Equals(value2) : !value1.Equals(value2);
                    if (operatr == "and") result = result && comparisonResult;
                    else if (operatr == "or") result = result || comparisonResult;
                    else result = comparisonResult;
                }
            }

            return result;
        }

        public void SetValue(ref string formData, string parameter, string value)
        {
            dynamic jsonObj = JsonConvert.DeserializeObject(formData);
            jsonObj[parameter] = value;
            string output = JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.None);
            formData = output;
        }
    }

    public interface IFormXmlService
    {
        string ParseFields(string formData, string nodeText, int requestId = 0, string comments = "");
        string GetValue(string formData, string parameter);
        void SetValue(ref string formData, string parameter, string value);
        bool ValidateConditions(string formData, string validation);
        XmlDocument LoadXmlForm(int idForm);

        XmlDocument XmlForm { get; set; }
        IUrlHelper Url { get; set; }
        string Scheme { get; set; }
        IUserUtil UserUtil { get; set; }
    }
}