using MMP.Generic_Functions;
using MMP.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMP.Controllers
{
    [Authorize(Roles = "admin")]
    public class InActiveUsersController : Controller
    {
        //USER DETAILS  // TODO: Restrict Access to Admin
        public ActionResult InActiveUserDetails(int id = 0)
        {
            ViewBag.userID = id;
            return View();
        }


        public ActionResult GetData(int id = 0)
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
                                 supervisor = u.employee_id + "   " + u.user_name,
                                 region = region.region_name,
                                 user_primary_department = upd.ctd_name,
                                 user_primary_project = upp.ctd_name
                             }).Where(x => x.user.user_status == "inactive");
                if (id != 0)
                {
                    query = query.Where(x => x.user.user_id == id);
                }
                return Json(new { data = query.AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult InActiveUserTimesheets(int id = 0)
        {
            ViewBag.UserID = id;
            return View();
        }

        [HttpGet]
        public ActionResult GetTimeSheets(int id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;

                var timesheets = (from ts in mP.timesheets
                                  join user in mP.users on ts.timesheet_user equals user.user_id
                                  join supervisor in mP.users on user.supervisor equals supervisor.user_id into supervisorps
                                  from supervisor in supervisorps.DefaultIfEmpty()
                                  join ctd in mP.category_type_details on user.user_primary_department equals ctd.ctd_id into ctdps
                                  from ctd in ctdps.DefaultIfEmpty()
                                  join ctp in mP.category_type_details on user.user_primary_project equals ctp.ctd_id into ctpps
                                  from ctp in ctpps.DefaultIfEmpty()
                                  join tsmr in mP.timesheet_mr on ts.timesheet_caller equals tsmr.tsmr_id
                                  select new
                                  {
                                      ts,
                                      user,
                                      supervisorName = supervisor.employee_id == null ? "" : supervisor.employee_id,
                                      tsmr,
                                      primary_department = ctd.ctd_name == null ? "" : ctd.ctd_name,
                                      primary_project = ctp.ctd_name == null ? "" : ctp.ctd_name
                                  }).Where(x => x.user.user_id == id && x.user.user_status == "inactive").OrderByDescending(x => x.tsmr.tsmr_created_at);
               


                return Json(new { data = timesheets.AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}