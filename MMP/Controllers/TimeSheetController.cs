using MMP.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMP.Generic_Functions;

namespace MMP.Controllers
{
    [Authorize]
    public class TimeSheetController : Controller
    {

        // ===========================================================================================================================================================================
        // View Timesheets: ADMIN
        // AUTHORIZE ALL ACTION RESULTS FOR ADMIN

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult AdminTimesheets()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult GetDataAdmin()
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;

                var timesheets = (from ts in mP.timesheets
                                  join user in mP.users on ts.timesheet_user equals user.user_id
                                  join supervisor in mP.users on user.supervisor equals supervisor.user_id into supervisorps from supervisor in supervisorps.DefaultIfEmpty()
                                  join tsmr in mP.timesheet_mr on ts.timesheet_caller equals tsmr.tsmr_id
                                  select new
                                  {
                                      ts,
                                      user,
                                      supervisorName = supervisor.user_name == null ? "" : supervisor.user_name,
                                      tsmr
                                  }).OrderByDescending(x => x.tsmr.tsmr_created_at);

                return Json(new { data = timesheets.AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet );
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult GenerateTimeSheets()
        {
            using (mmpEntities mP = new mmpEntities())
            {
                var timesheet = mP.timesheet_mr.Where(x => x.tsmr_valid_till < DateTime.Now).FirstOrDefault<timesheet_mr>();

                if (timesheet == null)
                {
                    DateTime startDate = StartOfWeek(DateTime.Today, DayOfWeek.Monday);
                    DateTime endDate = startDate.AddDays(6);
                    timesheet_mr tsmr = new timesheet_mr()
                    {
                        tsmr_generated_by = UserID_RoleID.getUserID(),
                        days = 7,
                        tsmr_created_at = DateTime.Now,
                        tsmr_start_date = startDate,
                        tsmr_valid_till = endDate
                    };
                    mP.timesheet_mr.Add(tsmr);
                    mP.SaveChanges();

                    int timesheetID = tsmr.tsmr_id;

                    foreach (user user in mP.users)
                    {
                        timesheet ts = new timesheet()
                        {
                            timesheet_user = user.user_id,
                            total_hours = 0,
                            time_my = DateTime.Now,
                            timesheet_status = "saved",
                            timesheet_caller = tsmr.tsmr_id
                        };
                        mP.timesheets.Add(ts);
                    }
                    mP.SaveChanges();

                    return Json(new { success = true, message = "TimeSheets Generated Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = true, message = "TimeSheet already exists for current week" }, JsonRequestBehavior.AllowGet);
                }
            }
        }


        // ===========================================================================================================================================================================

        // View Timesheets: Supervisor
        // AUTHORIZE ALL ACTION RESULTS FOR SUPERVISOR

        [Authorize(Roles = "supervisor")]
        [HttpGet]
        public ActionResult SupervisorTimesheets()
        {
            return View();
        }

        [Authorize(Roles = "supervisor")]
        [HttpGet]
        public ActionResult GetDataSupervisor()
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;

                int userID = UserID_RoleID.getUserID();

                var timesheets = (from ts in mP.timesheets
                                  join user in mP.users on ts.timesheet_user equals user.user_id
                                  join supervisor in mP.users on user.supervisor equals supervisor.user_id into supervisorps
                                  from supervisor in supervisorps.DefaultIfEmpty()
                                  join tsmr in mP.timesheet_mr on ts.timesheet_caller equals tsmr.tsmr_id
                                  select new
                                  {
                                      ts,
                                      user,
                                      supervisorName = supervisor.user_name == null ? "" : supervisor.user_name,
                                      tsmr
                                  }).Where(x => x.user.user_id == userID || x.user.supervisor == userID).OrderByDescending(x => x.user.user_id == userID).ThenBy(x => x.tsmr.tsmr_created_at);

                return Json(new { data = timesheets.AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
            }
        }

        // ===========================================================================================================================================================================

        // View Timesheets: User
        // AUTHORIZE ALL ACTION RESULTS FOR USER

        [Authorize(Roles = "user")]
        [HttpGet]
        public ActionResult UserTimesheets()
        {
            return View();
        }

        [Authorize(Roles = "user")]
        [HttpGet]
        public ActionResult GetDataUser()
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;

                int userID = UserID_RoleID.getUserID();

                var timesheets = (from ts in mP.timesheets
                                  join user in mP.users on ts.timesheet_user equals user.user_id
                                  join supervisor in mP.users on user.supervisor equals supervisor.user_id into supervisorps
                                  from supervisor in supervisorps.DefaultIfEmpty()
                                  join tsmr in mP.timesheet_mr on ts.timesheet_caller equals tsmr.tsmr_id
                                  select new
                                  {
                                      ts,
                                      user,
                                      supervisorName = supervisor.user_name == null ? "" : supervisor.user_name,
                                      tsmr
                                  }).Where(x => x.user.user_id == userID).OrderByDescending(x => x.tsmr.tsmr_created_at);

                return Json(new { data = timesheets.AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
            }
        }

        // ===========================================================================================================================================================================


        [NonAction]
        public static DateTime StartOfWeek( DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}