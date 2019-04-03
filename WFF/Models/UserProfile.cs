using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFF.Models
{
    public class UserProfile : IUserProfile
    {
        public int UserProfileID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Active { get; set; }
    }

    public interface IUserProfile
    {
        int UserProfileID { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        string Name { get; set; }
        string Lastname { get; set; }
        string Active { get; set; }
    }
}