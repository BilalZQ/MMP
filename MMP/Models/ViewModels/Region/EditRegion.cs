using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MMP.Models.ViewModels.Region
{
    public class EditRegion
    {
        public int region_id { get; set; }

        [Display(Name = "Region Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Region Name is required")]
        [System.Web.Mvc.Remote("CheckExistingRegions", "ModelValidation", HttpMethod = "POST", ErrorMessage = "Region name already exists", AdditionalFields = "region_id")]
        public string region_name { get; set; }
    }
}