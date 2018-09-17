using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MMP.Validations;

namespace MMP.Models.ViewModels.User
{
    public class AddUser
    {
        [Display(Name = "Employee ID")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Employee ID is required")]
        [System.Web.Mvc.Remote("CheckExistingEmployeeID", "ModelValidation", HttpMethod = "POST", ErrorMessage = "Employee ID already assigned to another user", AdditionalFields = "user_id")]
        public string employee_id { get; set; }

        [Display(Name = "Employee name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "User name is required")]
        public string user_name { get; set; }

        [Display(Name = "Role")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "User Role is required")]
        public int role_id { get; set; }

        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Minimum 6 characters required")]
        public string user_password { get; set; }


        [Display(Name = "Confirm Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("user_password", ErrorMessage = "Confirm password and password do not match")]
        public string confirmPassword { get; set; }

        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [System.Web.Mvc.Remote("CheckExistingEmail", "ModelValidation", HttpMethod = "POST", ErrorMessage = "Email already exists")]
        public string user_email { get; set; }

        [Display(Name = "Joining Date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "User Joining Date is required")]
        public System.DateTime joining_date { get; set; }

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





        /*public static implicit operator Registration(user user)
        {
            return new Registration
            {
                user_name = user.user_name,
                user_email = user.user_email,
                user_password = user.user_password
            };
        }

        public static implicit operator user(Registration vm)
        {
            return new user
            {
                user_name = vm.user_name,
                user_email = vm.user_email,
                user_password = vm.user_password
            };
        }*/

    }


}