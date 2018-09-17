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
        
        [Display(Name = "Employee ID")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Employee ID is required")]
        [System.Web.Mvc.Remote("CheckExistingEmployeeID", "ModelValidation", HttpMethod = "POST", ErrorMessage = "Employee ID already assigned to another user", AdditionalFields = "user_id")]
        public string employee_id { get; set; }

        [Display(Name = "Employee name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "User name is required")]
        public string user_name { get; set; }

        [Display(Name = "Role")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Confirm Password is required")]
        public int role_id { get; set; }

        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [System.Web.Mvc.Remote("CheckExistingEmail", "ModelValidation", HttpMethod = "POST", ErrorMessage = "Email already exists", AdditionalFields = "user_id")]
        public string user_email { get; set; }

        [Display(Name = "User Designation")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "User Designation is required")]
        public string designation { get; set; }

        [Display(Name = "Supervisor")]
        public Nullable<int> supervisor { get; set; }

        [Display(Name = "Region")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "User Region is required")]
        public int region_id { get; set; }

        [Display(Name = "User Primary Department")]
        public Nullable<int> user_primary_department { get; set; }

        [Display(Name = "User Primary Project")]
        public Nullable<int> user_primary_project { get; set; }
    }
}