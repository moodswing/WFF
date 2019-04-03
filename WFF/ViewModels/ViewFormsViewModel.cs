using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFF.ViewModels
{
    public class ViewFormsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string GroupBy { get; set; }
        public List<ViewColumn> ViewColumns { get; set; }
    }

    public struct ViewColumn
    {
        public string Name { get; set; }
        public string ControlId { get; set; }
        public string FunctionControlId { get; set; }
    }
}