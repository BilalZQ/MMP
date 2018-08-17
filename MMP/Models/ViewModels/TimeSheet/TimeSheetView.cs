using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MMP.Models.ViewModels.TimeSheet
{
    public class ParentVM
    {
        public int? tsd_id { get; set; }
        
        public int tsd_timesheet_id { get; set; }
        
        public int tsd_category_id { get; set; }
        
        public int tsd_category_type_id { get; set; }
        //public string timesheet_entry_type { get; set; }
        
        public List<ChildVM> timesheet_day_details { get; set; }
    }
    public class ChildVM
    {
        
        public int? tdd_id { get; set; }
        
        public Nullable<DateTime> tdd_day { get; set; }
        
        public Nullable<double> workhours { get; set; }

        public int?  tsd_id { get; set; }

        public Nullable<int> holiday { get; set; }
    }

    public class AddCategoryandCategoryType
    {
        public int tsd_timesheet_id { get; set; }
        
        [Display(Name = "Category")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Category is required")]
        public int tsd_category_id { get; set; }

        [Display(Name = "Category Type ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Category Type is required")]
        [System.Web.Mvc.Remote("CheckExistingTimeSheetCategory", "ModelValidation", HttpMethod = "POST", ErrorMessage = "Category type already added to timeSheet", AdditionalFields = "tsd_timesheet_id")]
        public int tsd_category_type_id { get; set; }
    }
    
}