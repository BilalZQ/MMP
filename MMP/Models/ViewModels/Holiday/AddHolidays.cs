using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MMP.Models.ViewModels.Holiday
{
    public class AddHolidays
    {
        [Display(Name = "Holiday Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Holiday Name is required")]
        [System.Web.Mvc.Remote("CheckExistingHolidays", "ModelValidation", HttpMethod = "POST", ErrorMessage = "Holiday already exists", AdditionalFields = "hy_id")]
        public string hd_name { get; set; }

        [Display(Name = "Starting from")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Starting date is required")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime hd_from { get; set; }

        [Display(Name = "till")]
        [System.Web.Mvc.Remote("CheckHolidayDateRange", "ModelValidation", HttpMethod = "POST", ErrorMessage = "To Date should be equal to or greater than start date", AdditionalFields = "hy_id, hd_from")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime hd_to { get; set; }

        [Display(Name = "Holiday Year")]
        public int hy_id { get; set; }

    }
}