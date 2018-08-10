using MMP.Models;
using MMP.Models.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMP.Controllers.Reports
{
    [Authorize(Roles = "admin")]
    public class ProjectReportController : Controller
    {
        // GET: ProjectReport
        [HttpGet]
        public ActionResult USProjectReport()
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;
                ViewBag.Users = mP.users.ToList<user>();

                return View(new USProjectReport());
            }
        }

        public JsonResult GetUser(string Areas, string term = "")
        {
            using (mmpEntities mP = new mmpEntities())
            {
                // Use Stored Procedure
                var objUserlist = mP.users.Where(c => c.user_status == "active" && c.employee_id.ToUpper()
                            .Contains(term.ToUpper()))
                            .Select(c => new { Name = c.employee_id, ID = c.user_id })
                            .Distinct().ToList();
                return Json(objUserlist, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetUserData(string employee_id = "")
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;

                var query = (from user in mP.users
                             join role in mP.roles on user.role_id equals role.role_id
                             join region in mP.regions on user.region_id equals region.region_id
                             join u in mP.users on user.supervisor equals u.user_id into us
                             from u in us.DefaultIfEmpty()
                             join upd in mP.category_type_details on user.user_primary_department equals upd.ctd_id into upds
                             from upd in upds.DefaultIfEmpty()
                             join upp in mP.category_type_details on user.user_primary_project equals upp.ctd_id into upps
                             from upp in upps.DefaultIfEmpty()
                             select new
                             {
                                 user,
                                 user_role = role.role_name,
                                 supervisor = u.user_name,
                                 region = region.region_name,
                                 user_primary_department = upd.ctd_name,
                                 user_primary_project = upp.ctd_name
                             }).Where(x => x.user.user_status == "active");
                if (employee_id != "")
                {
                    query = query.Where(x => x.user.employee_id == employee_id);//x.user.user_id == id);
                    return Json(new { data = query.AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult GetUserProjects(string employee_id = "", string startDate = "", string endDate = "")
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;

                //Debug.WriteLine(startDate);
                //Debug.WriteLine(endDate);
                var user = mP.users.Where(x => x.employee_id == employee_id).FirstOrDefault<user>();

                if (user != null)
                {
                    var ret = mP.ReportUsProjectWorkHours(user.user_id, startDate, endDate).ToList<ReportUsProjectWorkHours_Result>();
                    return Json(new { data = ret }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
                }


                
            }
        }

        public ActionResult GetUserProjectsTotalHours(string employee_id = "", string startDate = "", string endDate = "")
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;

                var user = mP.users.Where(x => x.employee_id == employee_id).FirstOrDefault<user>();

                if (user != null)
                {
                    var ret = mP.ReportUsProjectTotalWorkHours(user.user_id, startDate, endDate).ToList<ReportUsProjectTotalWorkHours_Result>();
                    return Json(new { data = ret }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
                }
                
            }
        }
    }  

}