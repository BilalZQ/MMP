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
    
    public partial class holiday_year
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public holiday_year()
        {
            this.holiday_details = new HashSet<holiday_details>();
        }
    
        public int hy_id { get; set; }
        public string hy_name { get; set; }
        public Nullable<System.DateTime> creation_date { get; set; }
        public short year { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<holiday_details> holiday_details { get; set; }
    }
}
