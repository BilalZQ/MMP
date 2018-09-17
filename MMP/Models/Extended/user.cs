using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MMP.Models
{
    public partial class user
    {
        [NotMapped]
        public string creationDate { get { return this.created_at.ToString(); } }

        [NotMapped]
        public string updateDate { get { return this.updated_at.ToString(); } }

        [NotMapped]
        public string joining_date { get { return this.join_date.ToString(); } }
    }
}