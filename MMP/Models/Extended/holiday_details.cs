﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MMP.Models
{
    [MetadataType(typeof(Holiday_details_metaData))]
    public partial class holiday_details
    {
        [NotMapped]
        public string holiday_from { get { return this.hd_from.ToString("MM/dd/yyyy");  } }

        [NotMapped]
        public string holiday_till { get { return this.hd_to.ToString("MM/dd/yyyy"); } }
    }

    public class Holiday_details_metaData
    {
        [Display(Name = "Holiday Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Holiday Name is required")]
        public string hd_name { get; set; }

        [Display(Name = "Starting from")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Starting date is required")]
        public System.DateTime hd_from { get; set; }

        [Display(Name = "till")]
        public System.DateTime hd_to { get; set; }
        
    }
}