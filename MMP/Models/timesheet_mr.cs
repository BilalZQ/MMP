//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MMP.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class timesheet_mr
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public timesheet_mr()
        {
            this.timesheets = new HashSet<timesheet>();
        }
    
        public int tsmr_id { get; set; }
        public Nullable<int> tsmr_generated_by { get; set; }
        public int days { get; set; }
        public Nullable<System.DateTime> tsmr_created_at { get; set; }
        public Nullable<System.DateTime> tsmr_start_date { get; set; }
        public Nullable<System.DateTime> tsmr_valid_till { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<timesheet> timesheets { get; set; }
        public virtual user user { get; set; }
    }
}
