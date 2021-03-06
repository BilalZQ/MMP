﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class mmpEntities : DbContext
    {
        public mmpEntities()
            : base("name=mmpEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<category> categories { get; set; }
        public virtual DbSet<category_type_details> category_type_details { get; set; }
        public virtual DbSet<holiday_details> holiday_details { get; set; }
        public virtual DbSet<holiday_year> holiday_year { get; set; }
        public virtual DbSet<leave_details> leave_details { get; set; }
        public virtual DbSet<presence> presences { get; set; }
        public virtual DbSet<project_details> project_details { get; set; }
        public virtual DbSet<region> regions { get; set; }
        public virtual DbSet<role> roles { get; set; }
        public virtual DbSet<sector> sectors { get; set; }
        public virtual DbSet<timesheet_day_details> timesheet_day_details { get; set; }
        public virtual DbSet<timesheet_details> timesheet_details { get; set; }
        public virtual DbSet<timesheet_mr> timesheet_mr { get; set; }
        public virtual DbSet<timesheet> timesheets { get; set; }
        public virtual DbSet<user> users { get; set; }
    
        public virtual ObjectResult<ReportCategoryTotalHours_Result> ReportCategoryTotalHours(Nullable<int> category_id, string startDate, string endDate)
        {
            var category_idParameter = category_id.HasValue ?
                new ObjectParameter("category_id", category_id) :
                new ObjectParameter("category_id", typeof(int));
    
            var startDateParameter = startDate != null ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(string));
    
            var endDateParameter = endDate != null ?
                new ObjectParameter("endDate", endDate) :
                new ObjectParameter("endDate", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportCategoryTotalHours_Result>("ReportCategoryTotalHours", category_idParameter, startDateParameter, endDateParameter);
        }
    
        public virtual ObjectResult<ReportCategoryUserWorkHours_Result> ReportCategoryUserWorkHours(Nullable<int> category_id, string startDate, string endDate)
        {
            var category_idParameter = category_id.HasValue ?
                new ObjectParameter("category_id", category_id) :
                new ObjectParameter("category_id", typeof(int));
    
            var startDateParameter = startDate != null ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(string));
    
            var endDateParameter = endDate != null ?
                new ObjectParameter("endDate", endDate) :
                new ObjectParameter("endDate", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportCategoryUserWorkHours_Result>("ReportCategoryUserWorkHours", category_idParameter, startDateParameter, endDateParameter);
        }
    
        public virtual ObjectResult<ReportLeaves_Result> ReportLeaves(Nullable<int> userID, string startDate, string endDate)
        {
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("userID", userID) :
                new ObjectParameter("userID", typeof(int));
    
            var startDateParameter = startDate != null ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(string));
    
            var endDateParameter = endDate != null ?
                new ObjectParameter("endDate", endDate) :
                new ObjectParameter("endDate", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportLeaves_Result>("ReportLeaves", userIDParameter, startDateParameter, endDateParameter);
        }
    
        public virtual ObjectResult<ReportLeavesTotal_Result> ReportLeavesTotal(Nullable<int> userID, string startDate, string endDate)
        {
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("userID", userID) :
                new ObjectParameter("userID", typeof(int));
    
            var startDateParameter = startDate != null ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(string));
    
            var endDateParameter = endDate != null ?
                new ObjectParameter("endDate", endDate) :
                new ObjectParameter("endDate", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportLeavesTotal_Result>("ReportLeavesTotal", userIDParameter, startDateParameter, endDateParameter);
        }
    
        public virtual ObjectResult<ReportSectorProjectTotalHours_Result> ReportSectorProjectTotalHours(Nullable<int> sectorID, string startDate, string endDate)
        {
            var sectorIDParameter = sectorID.HasValue ?
                new ObjectParameter("sectorID", sectorID) :
                new ObjectParameter("sectorID", typeof(int));
    
            var startDateParameter = startDate != null ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(string));
    
            var endDateParameter = endDate != null ?
                new ObjectParameter("endDate", endDate) :
                new ObjectParameter("endDate", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportSectorProjectTotalHours_Result>("ReportSectorProjectTotalHours", sectorIDParameter, startDateParameter, endDateParameter);
        }
    
        public virtual ObjectResult<ReportUsProjectTotalWorkHours_Result> ReportUsProjectTotalWorkHours(Nullable<int> userID, string startDate, string endDate)
        {
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("userID", userID) :
                new ObjectParameter("userID", typeof(int));
    
            var startDateParameter = startDate != null ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(string));
    
            var endDateParameter = endDate != null ?
                new ObjectParameter("endDate", endDate) :
                new ObjectParameter("endDate", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportUsProjectTotalWorkHours_Result>("ReportUsProjectTotalWorkHours", userIDParameter, startDateParameter, endDateParameter);
        }
    
        public virtual ObjectResult<ReportUsProjectWorkHours_Result> ReportUsProjectWorkHours(Nullable<int> userID, string startDate, string endDate)
        {
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("userID", userID) :
                new ObjectParameter("userID", typeof(int));
    
            var startDateParameter = startDate != null ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(string));
    
            var endDateParameter = endDate != null ?
                new ObjectParameter("endDate", endDate) :
                new ObjectParameter("endDate", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportUsProjectWorkHours_Result>("ReportUsProjectWorkHours", userIDParameter, startDateParameter, endDateParameter);
        }
    }
}
