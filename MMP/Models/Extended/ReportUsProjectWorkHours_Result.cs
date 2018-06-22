using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MMP.Models
{
    public partial class ReportUsProjectWorkHours_Result
    {

        public System.DateTime tdd_day { get; set; }
        public string ctd_name { get; set; }
        public double workhours { get; set; }

        [NotMapped]
        public string day { get { return this.tdd_day.ToString("MM/dd/yyyy"); } }
    }
}