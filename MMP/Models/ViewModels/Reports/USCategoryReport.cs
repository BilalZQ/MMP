using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MMP.Models.ViewModels.Reports
{
    public class USCategoryReport
    {
        [Display(Name = "From")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Start Date is required")]
        public string date_from { get; set; }

        [Display(Name = "TO")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "End Date is required")]
        public string date_to { get; set; }

        [Display(Name = "Category Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Category Name is required")]
        public string categoryID { get; set; }
    }
}