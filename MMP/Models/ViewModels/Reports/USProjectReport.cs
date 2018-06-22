using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MMP.Models.ViewModels.Reports
{
    public class USProjectReport
    {
        [Display(Name = "From")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Start Date is required")]
        public string date_from { get; set; }

        [Display(Name = "TO")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "End Date is required")]
        public string date_to { get; set; }

        [Display(Name = "User Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "User Name is required")]
        public string userName { get; set; }

        public string userID { get; set; }
    }
}