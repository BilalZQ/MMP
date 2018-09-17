using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MMP.Models.ViewModels.TimeSheet
{
    public class BulkExtendTimeSheet
    {
        public int tsmr_id { get; set; }
        public System.DateTime tsmr_valid_till { get; set; }

        [Display(Name = "Extended Date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Category is required")]
        [System.Web.Mvc.Remote("CheckTimeSheetExtention", "ModelValidation", HttpMethod = "POST", ErrorMessage = "TimeSheet Extension date should be greater than current DateTime as well as the Valid Date of the respective timesheet and it cannot be more than 6 months starting from the valid date of the respective timesheet.", AdditionalFields = "tsmr_valid_till")]
        public System.DateTime? tsmr_extension { get; set; }
    }
}