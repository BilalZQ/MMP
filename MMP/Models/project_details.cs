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
    
    public partial class project_details
    {
        public int id { get; set; }
        public int sector_id { get; set; }
        public string project_model { get; set; }
        public int category_type_id { get; set; }
    
        public virtual category_type_details category_type_details { get; set; }
        public virtual sector sector { get; set; }
    }
}
