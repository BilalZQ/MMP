using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MMP.Models.ViewModels.User
{
    public class Registration
    {
        [Display(Name = "User name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "User name is required")]
        public string user_name { get; set; }

        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string user_email { get; set; }

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