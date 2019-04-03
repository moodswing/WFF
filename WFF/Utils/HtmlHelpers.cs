using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using System;

namespace WFF.Utils
{
    public static class HtmlHelpers
    {
        public static IHtmlContent ParseFunc(this IHtmlHelper htmlHelper, string func, JToken json) 
        {
            var validate = func.Split('?')[0];
            var optionsResult = func.Split('?')[1].Split(':');

            var isEqual = validate.Contains("==");
            string[] equalities = { "==", "!=" };
            var constants = validate.Split(equalities, StringSplitOptions.None);

            var value1 = constants[0].Trim().Trim('\"');
            var value2 = constants[1].Trim().Trim('\"');

            if (json[constants[0].Trim()] != null)
            {
                value1 = json[value1].ToString().Trim('\"');
            }

            if (json[constants[1].Trim()] != null)
            {
                value2 = json[value2].ToString().Trim('\"');
            }

            var comparisonResult = isEqual ? value1.Equals(value2) : !value1.Equals(value2);


            var result = comparisonResult ? optionsResult[0].Trim() : optionsResult[1].Trim();

            string[] whiteSpaces = { "+ \" \" +" };
            var resultSplit = result.Split(whiteSpaces, StringSplitOptions.None);

            var funcResult = "";
            foreach (var item5 in resultSplit)
            {
                funcResult += json[item5.Trim()].ToString() + " ";
            }

            return new HtmlString(funcResult);
        }
    }
}