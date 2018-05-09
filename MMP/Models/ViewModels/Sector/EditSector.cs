using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MMP.Models.ViewModels.Sector
{
    public class EditSector
    {
        public int sector_id { get; set; }

        [Display(Name = "Sector Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Sector Name is required")]
        public string sector_name { get; set; }
    }
}