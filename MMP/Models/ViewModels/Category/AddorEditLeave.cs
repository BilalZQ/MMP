using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMP.Models.ViewModels.Category
{
    public class AddorEditLeave
    {
        public int ctd_id { get; set; }

        [Display(Name = "Leave Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Leave Name is required")]
        public string leave_name { get; set; }

        [Display(Name = "Leave Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Leave Code is required")]
        [System.Web.Mvc.Remote("CheckExistingCategoryTypeCode", "ModelValidation", HttpMethod = "POST", ErrorMessage = "Code is already assigned to another category type", AdditionalFields = "ctd_id")]
        public string code { get; set; }

        public int category_id { get; set; }

        public Nullable<System.DateTime> ctd_created_at { get; set; }

        public int leave_details_id { get; set; }  

        [Display(Name = "Total leaves allowed in this category")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "No. of leaves required")]
        public int no_of_leaves { get; set; }


        [Display(Name = "Encashable")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Encashable?")]
        public string encashable { get; set; }
        
        [Display(Name = "Carry Forward")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Carry Forward?")]
        public string carry_forward { get; set; }


        public IEnumerable<SelectListItem> selector
        {
            get { return new List<SelectListItem> { new SelectListItem { Value = "Yes", Text = "Yes" }, new SelectListItem { Value = "No", Text = "No" } };  }
        }
    }
}