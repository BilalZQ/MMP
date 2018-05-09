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
        public string hd_name { get; set; }

        [Display(Name = "Starting from")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Starting date is required")]
        public System.DateTime hd_from { get; set; }

        [Display(Name = "till")]
        public System.DateTime hd_to { get; set; }

        [Display(Name = "Holiday Year")]
        public int hy_id { get; set; }

    }
}