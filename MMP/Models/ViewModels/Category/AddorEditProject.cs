using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMP.Models.ViewModels.Category
{
    public class AddorEditProject
    {
        public int ctd_id { get; set; }

        [Display(Name = "Project Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Project Name is required")]
        public string proj_name { get; set; }

        [Display(Name = "Project Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Project Code is required")]
        public string proj_code { get; set; }

        public int category_id { get; set; }
        
        public Nullable<System.DateTime> ctd_created_at { get; set; }

        public int proj_details_id { get; set; }

        [Display(Name = "Sector Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Sector Name is required")]
        public int sector_id { get; set; }

        [Display(Name = "Project Model")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Project Model is required ")]
        public string proj_model { get; set; }

        public IEnumerable<SelectListItem> project_model
        {
            get { return new List<SelectListItem> { new SelectListItem { Value = "retainer", Text = "Retainer" }, new SelectListItem { Value = "lump sum", Text = "Lump Sum" } }; }
        }


    }
}