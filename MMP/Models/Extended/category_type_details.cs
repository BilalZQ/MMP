using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MMP.Models
{
    public partial class category_type_details
    {
        [NotMapped]
        public string created_at { get { return this.ctd_created_at?.ToString("MM/dd/yyyy"); } }

        [NotMapped]
        public string updated_at { get { return this.ctd_updated_at?.ToString("MM/dd/yyyy"); } }
    }
}