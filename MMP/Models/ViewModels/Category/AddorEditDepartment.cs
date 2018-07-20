using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MMP.Models.ViewModels.Category
{
    public class AddorEditDepartment
    {
        public int ctd_id { get; set; }

        [Display(Name = "Department Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Department Name is required")]
        public string dept_name { get; set; }

        [Display(Name = "Department Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Department Code is required")]
        [System.Web.Mvc.Remote("CheckExistingCategoryTypeCode", "ModelValidation", HttpMethod = "POST", ErrorMessage = "Code is already assigned to another category type", AdditionalFields = "ctd_id")]
        public string code { get; set; }

        public int category_id { get; set; }   
        
        public Nullable<System.DateTime> ctd_created_at { get; set; }

    }
}