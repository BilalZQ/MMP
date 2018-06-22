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
                var objCustomerlist = mP.users.Where(c => c.user_name.ToUpper()
                            .Contains(term.ToUpper()))
                            .Select(c => new { Name = c.user_name, ID = c.user_id })
                            .Distinct().ToList();
                return Json(objCustomerlist, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetUserData(int id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;

                var query = from user in mP.users
                            join role in mP.roles on user.role_id equals role.role_id
                            join region in mP.regions on user.region_id equals region.region_id
                            join u in mP.users on user.supervisor equals u.user_id into us
                            from u in us.DefaultIfEmpty()
                            select new
                            {
                                user,
                                user_role = role.role_name,
                                supervisor = u.user_name,
                                region = region.region_name
                            };
                if (id != 0)
                {
                    query = query.Where(x => x.user.user_id == id);
                }
                return Json(new { data = query.AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult GetUserProjects(int id = 0, string startDate = "", string endDate = "")
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;

                Debug.WriteLine(startDate);
                Debug.WriteLine(endDate);

                var ret = mP.ReportUsProjectWorkHours(id, startDate, endDate).ToList<ReportUsProjectWorkHours_Result>();

                return Json(new { data = ret}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetUserProjectsTotalHours(int id = 0, string startDate = "", string endDate = "")
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;

                var ret = mP.ReportUsProjectTotalWorkHours(id, startDate, endDate).ToList<ReportUsProjectTotalWorkHours_Result>();

                return Json(new { data = ret }, JsonRequestBehavior.AllowGet);
            }
        }
    }  

}