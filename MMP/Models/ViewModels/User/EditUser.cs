using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MMP.Models.ViewModels.User
{
    public class EditUser
    {
        public int user_id { get; set; }

        [Display(Name = "User name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "User name is required")]
        public string user_name { get; set; }

        [Display(Name = "Role")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Confirm Password is required")]
        public int role_id { get; set; }

        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string user_email { get; set; }

        [Display(Name = "Supervisor")]
        public Nullable<int> supervisor { get; set; }

        [Display(Name = "Region")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "User Region is required")]
        public int region_id { get; set; }
    }
}