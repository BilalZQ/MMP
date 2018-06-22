using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MMP.Models
{
    public partial class presence
    {
        [NotMapped]
        public string day { get { return this.p_date.ToString("MM/dd/yyyy"); } }
        
    }
}