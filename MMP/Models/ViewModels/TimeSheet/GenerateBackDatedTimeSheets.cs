using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MMP.Models.ViewModels.TimeSheet
{
    public class GenerateBackDatedTimeSheets
    {
        [Display(Name = "Extended Date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Back Date is required")]
        public System.DateTime tsmr_backDate { get; set; }
    }
}