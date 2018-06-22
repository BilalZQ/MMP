using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MMP.Models
{
    public partial class ReportLeaves_Result
    {
        [NotMapped]
        public string day { get { return this.tdd_day.ToString("MM/dd/yyyy"); } }
    }
}