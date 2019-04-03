using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WFF.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        [Required]
        //[Display(Name = "lblUserLogin", Prompt = "lblUserLoginPlaceholder", ResourceType = typeof(Resources.Language))]
        [Display(Name = "User", Prompt = "Enter your user")]
        public string User { get; set; }

        [Required]
        [DataType(DataType.Password)]
        //[Display(Name = "lblPasswordLogin", Prompt = "lblPasswordLoginPlaceholder", ResourceType = typeof(Resources.Language))]
        [Display(Name = "Password", Prompt = "Enter your password")]
        public string Password { get; set; }

        public string Message { get; set; }

        public string idRequest { get; set; }
    }
}