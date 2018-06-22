using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MMP.Models
{
    public partial class timesheet_mr
    {
        [NotMapped]
        public string created_at { get { return this.tsmr_created_at.ToString(); } }

        [NotMapped]
        public string startDate { get { return this.tsmr_start_date.ToString(); } }

        [NotMapped]
        public string endDate { get { return this.tsmr_valid_till.ToString(); } }

    }
}