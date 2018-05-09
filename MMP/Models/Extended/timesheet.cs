using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MMP.Models
{
    public partial class timesheet
    {
        [NotMapped]
        public string ts_status_update { get { return this.timesheet_status_update.ToString(); } }
    }
}