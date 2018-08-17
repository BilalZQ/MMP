using MMP.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMP.Generic_Functions;
using MMP.Models.ViewModels.TimeSheet;
using System.Web.Script.Serialization;
using System.Threading.Tasks;

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
        public ActionResult AdminPreviousTimeSheets()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult AdminTimesheets()
        {
            return View();
        }

        /*
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
                                      supervisorName = supervisor.employee_id == null ? "" : supervisor.employee_id,
                                      tsmr
                                  }).OrderByDescending(x => x.tsmr.tsmr_created_at);

                return Json(new { data = timesheets.AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet );
            }
        }*/

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult GenerateTimeSheets()
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;
                var timesheet = mP.timesheet_mr.Where(x => x.tsmr_valid_till > DateTime.Now).FirstOrDefault<timesheet_mr>();
                string body;

                if (timesheet == null)
                {
                    DateTime startDate = StartOfWeek(DateTime.Today, DayOfWeek.Monday);
                    DateTime endDate = startDate.AddDays(6);
                    timesheet_mr tsmr = new timesheet_mr()
                    {
                        tsmr_generated_by = UserID_RoleID.getUserID(),
                        days = 7, // It was decided that every TimeSheet will be for 7 days 
                        tsmr_created_at = DateTime.Now,
                        tsmr_start_date = startDate,
                        tsmr_valid_till = endDate
                    };
                    mP.timesheet_mr.Add(tsmr);

                    int timesheetID = tsmr.tsmr_id;

                    

                    foreach (user user in mP.users.Where(x => x.user_status == "active"))
                    {
                        timesheet ts = new timesheet()
                        {
                            timesheet_user = user.user_id,
                            time_my = DateTime.Now,
                            timesheet_status = "saved",
                            timesheet_caller = tsmr.tsmr_id,
                            tsmr_extension = endDate
                        };
                        mP.timesheets.Add(ts);

                        Debug.WriteLine(ts);

                        foreach (DateTime day in EachDay(tsmr.tsmr_start_date, tsmr.tsmr_valid_till))
                        {
                            presence ps = new presence()
                            {
                                p_date = day,
                                total_hours = 0,
                                leave_status = "",
                                user_id = user.user_id
                            };
                            mP.presences.Add(ps);
                        }

                        body = "<br></br>" + user.user_name +", TimeSheet for current week has been generated. TimeSheet is valid Till "+ endDate +". Visit the following website to access timeSheet<br></br>" +
                "<a href='http://magcom-001-site3.etempurl.com'>http://magcom-001-site3.etempurl.com</a>";

                        Task.Run(() => EmailAlert.SendEmail(user.user_email, body)); 
                    }

                    mP.SaveChanges();


                    //EmailAlert.SendEmail("bilal@simsum.co");

                    return Json(new { success = true, message = "TimeSheets Generated Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ViewBag.Message = "TimeSheet already exists for current week";
                    return Json(new { success = false, message = "TimeSheet already exists for current week" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult AdminAllTimeSheets()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult AdminAllPreviousTimeSheets()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult GetUserDataForAdmin(string flag)// MERGE WITH THE ONE BELOW [GetData]
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
                                      supervisorName = supervisor.employee_id == null ? "" : supervisor.employee_id,
                                      tsmr
                                      //month = Convert.ToDateTime(tsmr.startDate).ToString("MMMM")
                                  }).Where(x => x.user.user_status == "active").OrderByDescending(x => x.tsmr.tsmr_created_at);

                if (flag.Equals("currentTimeSheets", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Json(new { data = timesheets.Where(x => x.ts.tsmr_extension > DateTime.Now).AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
                }
                else if (flag.Equals("previousTimeSheets", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Json(new { data = timesheets.Where(x => x.ts.tsmr_extension < DateTime.Now).AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { data = timesheets.AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult AdminTimeSheetEdit(int id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;

                if (id != 0)
                {
                    ViewBag.Categories = mP.categories.ToList<category>();
                    ViewBag.CategoryTypes = mP.category_type_details.ToList<category_type_details>();
                    ViewBag.TimeSheetID = id;

                    var timesheetCaller = mP.timesheets.Where(x => x.timesheet_id == id).FirstOrDefault();
                    var timesheetMR = mP.timesheet_mr.Where(x => x.tsmr_id == timesheetCaller.timesheet_caller).FirstOrDefault();
                    
                    // ADD USER TO THE TIMESHEET EDIT VIEW
                    ViewBag.UserName = mP.users.FirstOrDefault(x => x.user_id == timesheetCaller.timesheet_user).user_name;


                    ViewBag.startDate = timesheetMR.tsmr_start_date;
                    ViewBag.endDate = timesheetMR.tsmr_valid_till;

                    var timesheetDetails = from tsd in mP.timesheet_details
                                           join tsdd in mP.timesheet_day_details on tsd.tsd_id equals tsdd.tsd_id into tsddps
                                           from tsdd in tsddps.DefaultIfEmpty()
                                           select new
                                           {
                                               tsd,
                                               tsdd
                                           };

                    var model = timesheetDetails.Where(x => x.tsd.tsd_timesheet_id == id).GroupBy(x => new { tsd1 = x.tsd.tsd_id, x.tsdd.tsd_id }).Select(x => new ParentVM // USE A FUNCTION FOR THIS
                    {
                        tsd_id = x.Key.tsd1,
                        tsd_timesheet_id = x.FirstOrDefault().tsd.tsd_timesheet_id,
                        tsd_category_id = x.FirstOrDefault().tsd.tsd_category_id,
                        tsd_category_type_id = x.FirstOrDefault().tsd.tsd_category_type_id,
                        timesheet_day_details = x.Select(y => new ChildVM
                        {
                            tdd_id = y.tsdd.tdd_id,
                            tdd_day = y.tsdd.tdd_day,
                            workhours = y.tsdd.workhours,
                            tsd_id = y.tsdd.tsd_id,
                            holiday = y.tsdd.holiday_id
                        }).OrderBy(y => y.tdd_id).ToList()
                    });

                    return View(model.ToList());
                }
                else
                {
                    // No timesheet_id specified therefore a blank view will be displayed
                    return View();
                }

            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdminTimeSheetEdit(List<ParentVM> pvm, string submit)
        {
            if (ModelState.IsValid)
            {
                using (mmpEntities mP = new mmpEntities())
                {
                    var presenceList = new List<KeyValuePair<DateTime?, double?>>();
                    ViewBag.Categories = mP.categories.ToList<category>();
                    ViewBag.CategoryTypes = mP.category_type_details.ToList<category_type_details>();


                    int tsd_timesheet_id = pvm.Select(x => x.tsd_timesheet_id).FirstOrDefault();
                    ViewBag.TimeSheetID = tsd_timesheet_id;
                    timesheet ts = mP.timesheets.Where(x => x.timesheet_id == tsd_timesheet_id).First();
                    var timeSheetMR = mP.timesheet_mr.Where(x => x.tsmr_id == ts.timesheet_caller).FirstOrDefault();


                    // ADD USER TO THE TIMESHEET EDIT VIEW
                    ViewBag.UserName = mP.users.FirstOrDefault(x => x.user_id == ts.timesheet_user).user_name;


                    ViewBag.startDate = timeSheetMR.tsmr_start_date;
                    ViewBag.endDate = timeSheetMR.tsmr_valid_till;

                    if (submit == "Calculate")
                    {
                        return View(pvm);
                    }

                    // *** TimeSheet 7.5 Check ***
                    for (int i = 0; i < timeSheetMR.days; i++)
                    {
                        if (pvm.Sum(x => x.timesheet_day_details[i].workhours).Value > 7.5)
                        {
                            ViewBag.Message = string.Format("Error! Make sure sum of each column is less than 7.5.");
                            return View(pvm);
                        }
                    }
                    // *** TimeSheet 7.5 Check ***

                    if (ts.timesheet_status == "submitted" || ts.timesheet_status == "rejected" || ts.timesheet_status == "accepted") //Do this properly
                    {
                        switch (submit)
                        {
                            case "Submit":
                                ts.timesheet_status = "submitted";
                                ViewBag.Message = "TimeSheet Submitted";
                                break;
                            case "Accept":
                                ts.timesheet_status = "accepted";
                                ViewBag.Message = "TimeSheet Accepted";
                                break;
                            case "Reject":
                                ts.timesheet_status = "rejected";
                                ViewBag.Message = "TimeSheet Rejected";
                                break;
                            //case "Calculate":
                                //return View(pvm);
                        }
                        //ts.timesheet_status = ts.timesheet_status == "rejected" ? "submitted" : "saved"; //Based on button press (cannot be saved, can only be submitted)
                        ts.updated_by = UserID_RoleID.getUserID();
                        ts.timesheet_status_update = DateTime.Now;

                        mP.Entry(ts).State = EntityState.Modified;


                        foreach (var parent in pvm)
                        {
                            //ViewBag.TimeSheetID = parent.tsd_timesheet_id;
                            //Debug.WriteLine("Here");                            

                            foreach (var child in parent.timesheet_day_details)
                            {
                                //Debug.WriteLine(child.tdd_id + " " + child.tdd_day + " " + child.workhours + " " + child.tsd_id);
                                timesheet_day_details tsdd = mP.timesheet_day_details.Where(x => x.tdd_id == child.tdd_id).First();
                                tsdd.workhours = child.workhours;

                                // Attendance SETUP *****
                                if (parent.tsd_category_id != mP.categories.First(x => x.category_name == "leaves").category_id) /// USE A DELEGATE OR SOMETHING
                                {
                                    if (presenceList.FindIndex(x => x.Key == child.tdd_day) >= 0)
                                    {
                                        double? workHours = presenceList.First(x => x.Key == child.tdd_day).Value;
                                        workHours = workHours + child.workhours;
                                        presenceList.Add(new KeyValuePair<DateTime?, double?>(child.tdd_day, workHours));
                                    }
                                    else
                                    {
                                        presenceList.Add(new KeyValuePair<DateTime?, double?>(child.tdd_day, child.workhours));
                                    }
                                }

                                mP.Entry(tsdd).State = EntityState.Modified;
                            }
                        }

                        foreach (var item in presenceList)
                        {
                            var pres = mP.presences.Where(x => x.p_date == item.Key && x.user_id == ts.timesheet_user).FirstOrDefault();
                            pres.p_date = item.Key??DateTime.Now;
                            pres.total_hours = item.Value;

                            if (item.Value == 0)
                            {
                                pres.leave_status = "full day";
                            }
                            else
                            {
                                pres.leave_status = item.Value == 7.5 ? "no leave" : "partial leave";
                            }
                            pres.user_id = ts.timesheet_user;

                            mP.Entry(pres).State = EntityState.Modified;
                        }

                        mP.SaveChanges();

                        return View(pvm);

                    }
                    else if (ts.timesheet_status == ("saved")) //Do this properly
                    {
                        if (submit == "Calculate")
                        {
                            return View(pvm);
                        }
                        else
                        {
                            ViewBag.Message = string.Format("Cannot edit a Saved TimeSheet");
                            return View(pvm);
                        }
                    }
                }
            }
            return View();
        }


        // ===========================================================================================================================================================================

        // View Timesheets: Supervisor
        // AUTHORIZE ALL ACTION RESULTS FOR SUPERVISOR

        [Authorize(Roles = "supervisor")]
        [HttpGet]
        public ActionResult SupervisorPreviousTimeSheets()
        {
            return View();
        }

        [Authorize(Roles = "supervisor")]
        [HttpGet]
        public ActionResult SupervisorTimesheets()
        {
            return View();
        }

        [Authorize(Roles = "supervisor")]
        [HttpGet]
        public ActionResult SupervisorUserTimeSheets()
        {
            return View();
        }

        [Authorize(Roles = "supervisor")]
        [HttpGet]
        public ActionResult GetUserDataForSupervisor(string flag) //MERGE WITH THE ONE BELOW [GetData]
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
                                      supervisorName = supervisor.employee_id == null ? "" : supervisor.employee_id,
                                      tsmr
                                  }).Where(x => x.user.supervisor == userID && x.user.user_status == "active").OrderByDescending(x => x.tsmr.tsmr_created_at);

                if (flag.Equals("currentTimeSheets", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Json(new { data = timesheets.Where(x => x.ts.tsmr_extension > DateTime.Now).AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
                } else if (flag.Equals("previousTimeSheets", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Json(new { data = timesheets.Where(x => x.ts.tsmr_extension < DateTime.Now).AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { data = timesheets.AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "supervisor")]
        [HttpGet]
        public ActionResult SupervisorUserPreviousTimeSheets()
        {
            return View();
        }

        [Authorize(Roles = "supervisor")]
        [HttpGet]
        public ActionResult SupervisorUserTimeSheetEdit(int id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;


                if (id != 0)
                {
                    ViewBag.Categories = mP.categories.ToList<category>();
                    ViewBag.CategoryTypes = mP.category_type_details.ToList<category_type_details>();
                    ViewBag.TimeSheetID = id;

                    var timesheetCaller = mP.timesheets.Where(x => x.timesheet_id == id).FirstOrDefault();
                    var timesheetUser = mP.users.Where(x => x.user_id == timesheetCaller.timesheet_user).FirstOrDefault();


                    // ADD USER TO THE TIMESHEET EDIT VIEW
                    ViewBag.UserName = mP.users.FirstOrDefault(x => x.user_id == timesheetCaller.timesheet_user).user_name;


                    if (timesheetUser.supervisor == UserID_RoleID.getUserID() && timesheetUser.user_status == "active")
                    {
                        var timesheetMR = mP.timesheet_mr.Where(x => x.tsmr_id == timesheetCaller.timesheet_caller).FirstOrDefault();
                        ViewBag.startDate = timesheetMR.tsmr_start_date;
                        ViewBag.endDate = timesheetMR.tsmr_valid_till;

                        var timesheetDetails = from tsd in mP.timesheet_details
                                               join tsdd in mP.timesheet_day_details on tsd.tsd_id equals tsdd.tsd_id into tsddps
                                               from tsdd in tsddps.DefaultIfEmpty()
                                               select new
                                               {
                                                   tsd,
                                                   tsdd
                                               };

                        var model = timesheetDetails.Where(x => x.tsd.tsd_timesheet_id == id).GroupBy(x => new { tsd1 = x.tsd.tsd_id, x.tsdd.tsd_id }).Select(x => new ParentVM // USE A FUNCTION FOR THIS
                        {
                            tsd_id = x.Key.tsd1,
                            tsd_timesheet_id = x.FirstOrDefault().tsd.tsd_timesheet_id,
                            tsd_category_id = x.FirstOrDefault().tsd.tsd_category_id,
                            tsd_category_type_id = x.FirstOrDefault().tsd.tsd_category_type_id,
                            timesheet_day_details = x.Select(y => new ChildVM
                            {
                                tdd_id = y.tsdd.tdd_id,
                                tdd_day = y.tsdd.tdd_day,
                                workhours = y.tsdd.workhours,
                                tsd_id = y.tsdd.tsd_id,
                                holiday = y.tsdd.holiday_id
                            }).OrderBy(y => y.tdd_id).ToList()
                        });

                        return View(model.ToList());

                    }
                    else
                    {
                        //Access denied (trying to access a different timesheet)
                        return View();
                    }
                }
                else
                {
                    // No timesheet_id specified therefore a blank view will be displayed
                    return View();
                }
            }
        }

        [Authorize(Roles = "supervisor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SupervisorUserTimeSheetEdit(List<ParentVM> pvm, string submit, string remarks)
        {

            string body = "";
            if (ModelState.IsValid)
            {
                using (mmpEntities mP = new mmpEntities())
                {
                    ViewBag.Categories = mP.categories.ToList<category>();
                    ViewBag.CategoryTypes = mP.category_type_details.ToList<category_type_details>();


                    int tsd_timesheet_id = pvm.Select(x => x.tsd_timesheet_id).FirstOrDefault();
                    ViewBag.TimeSheetID = tsd_timesheet_id;
                    timesheet ts = mP.timesheets.Where(x => x.timesheet_id == tsd_timesheet_id).First();
                    var timeSheetMR = mP.timesheet_mr.Where(x => x.tsmr_id == ts.timesheet_caller).FirstOrDefault();

                    // ADD USER TO THE TIMESHEET EDIT VIEW
                    ViewBag.UserName = mP.users.FirstOrDefault(x => x.user_id == ts.timesheet_user).user_name;

                    ViewBag.startDate = timeSheetMR.tsmr_start_date;
                    ViewBag.endDate = timeSheetMR.tsmr_valid_till;

                    var user = mP.users.Where(x => x.user_id == ts.timesheet_user).FirstOrDefault();

                    if (user.supervisor == UserID_RoleID.getUserID() && ts.tsmr_extension >= DateTime.Now && ts.timesheet_status == "submitted" && user.user_status == "active") //Do this properly
                    {
                        switch (submit)
                        {
                            case "Reject":
                                ts.timesheet_status = "rejected";
                                ViewBag.Message = "TimeSheet Rejected";
                                if (remarks == "")
                                {
                                    remarks = "No remarks";
                                }
                                body = "<br></br>" + user.user_name + ", Your timeSheet created at "+ timeSheetMR.tsmr_start_date + " and Valid till "+timeSheetMR.tsmr_valid_till+" has been <b>Rejected</b> by your supervisor. You can edit and resubmit the TimeSheet Till " + ts.tsmr_extension + ".<br></br><b>Remarks from supervisor:</b> "+remarks+ "<br></br> Visit the following website to access timeSheet<br></br>" +
                "<a href='http://magcom-001-site3.etempurl.com'>http://magcom-001-site3.etempurl.com</a>";
                                break;
                            case "Accept":
                                ts.timesheet_status = "accepted";
                                ViewBag.Message = "TimeSheet Accepted";
                                if (remarks == "")
                                {
                                    remarks = "No remarks";
                                }
                                body = "<br></br>" + user.user_name + ", Your timeSheet created at " + timeSheetMR.tsmr_start_date + " and Valid till " + timeSheetMR.tsmr_valid_till + " has been <b>Accepted</b> by your supervisor. <br></br><b>Remarks from supervisor:</b> " + remarks + "<br></br> Visit the following website to access timeSheet<br></br>" +
                "<a href='http://magcom-001-site3.etempurl.com'>http://magcom-001-site3.etempurl.com</a>";
                                break;
                        }
                        //ts.timesheet_status = ts.timesheet_status == "rejected" ? "submitted" : "saved"; //Based on button press (cannot be saved, can only be submitted)
                        ts.updated_by = UserID_RoleID.getUserID();
                        ts.timesheet_status_update = DateTime.Now;

                        mP.Entry(ts).State = EntityState.Modified;

                        mP.SaveChanges();

                        

                        Task.Run(() => EmailAlert.SendEmail(user.user_email, body));

                        return View(pvm);

                    }
                    else if (ts.tsmr_extension < DateTime.Now)
                    {
                        ViewBag.Message = string.Format("Cannot edit TimeSheet's past their extension date.");
                        return View(pvm);
                    }
                    else
                    {
                        switch (ts.timesheet_status)
                        {
                            case "saved":
                                ViewBag.Message = string.Format("Cannot Accept/Reject a Saved TimeSheet");
                                break;
                            case "accepted":
                                ViewBag.Message = string.Format("Cannot Accept/Reject a Accepted TimeSheet");
                                break;
                            case "rejected":
                                ViewBag.Message = string.Format("Cannot Accept/Reject a Rejected TimeSheet");
                                break;
                        }

                        return View(pvm);
                    }

                }
            }
            return View();
        }

        /*[Authorize(Roles = "supervisor")]
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
                                      supervisorName = supervisor.employee_id == null ? "" : supervisor.employee_id,
                                      tsmr
                                  }).Where(x => x.user.user_id == userID || x.user.supervisor == userID).OrderByDescending(x => x.user.user_id == userID).ThenBy(x => x.tsmr.tsmr_created_at);

                return Json(new { data = timesheets.AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
            }
        }*/

        // ===========================================================================================================================================================================

        // View Timesheets: User
        // AUTHORIZE ALL ACTION RESULTS FOR USER

        [Authorize(Roles = "user, admin, supervisor")]
        [HttpGet]
        public ActionResult UserPreviousTimeSheets()
        {
            return View();
        }

        [Authorize(Roles = "user, admin, supervisor")]
        [HttpGet]
        public ActionResult UserTimesheets()
        {
            return View();
        }


        // ===========================================================================================================================================================================

        // *** GENERIC FUNCTIONS ***
        [Authorize(Roles = "user, admin, supervisor")]
        [HttpGet]
        public ActionResult GetData(string flag)
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
                                      supervisorName = supervisor.employee_id == null ? "" : supervisor.employee_id,
                                      tsmr
                                  }).Where(x => x.user.user_id == userID && x.user.user_status == "active").OrderByDescending(x => x.tsmr.tsmr_created_at);

                if (flag.Equals("currentTimeSheets", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Json(new { data = timesheets.Where(x => x.ts.tsmr_extension > DateTime.Now).AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
                }
                else if (flag.Equals("previousTimeSheets", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Json(new { data = timesheets.Where(x => x.ts.tsmr_extension < DateTime.Now).AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
                }


                return Json(new { data = timesheets.AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "user, admin, supervisor")]
        [HttpGet]
        public ActionResult TimeSheetEditView(int id = 0, string message = "")
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;

                if (id != 0)
                {
                    ViewBag.Categories = mP.categories.ToList<category>();
                    ViewBag.CategoryTypes = mP.category_type_details.ToList<category_type_details>();
                    ViewBag.TimeSheetID = id;

                    var timesheetCaller = mP.timesheets.Where(x => x.timesheet_id == id).FirstOrDefault();


                    // ADD USER TO THE TIMESHEET EDIT VIEW
                    ViewBag.UserName = mP.users.FirstOrDefault(x => x.user_id == timesheetCaller.timesheet_user).user_name;
                    ViewBag.Message = message;

                    if (timesheetCaller.timesheet_user == UserID_RoleID.getUserID())
                    {
                        var timeSheetMR = mP.timesheet_mr.Where(x => x.tsmr_id == timesheetCaller.timesheet_caller).FirstOrDefault();
                        ViewBag.startDate = timeSheetMR.tsmr_start_date;
                        ViewBag.endDate = timeSheetMR.tsmr_valid_till;

                        var timesheetDetails = from tsd in mP.timesheet_details
                                               join tsdd in mP.timesheet_day_details on tsd.tsd_id equals tsdd.tsd_id into tsddps
                                               from tsdd in tsddps.DefaultIfEmpty()    
                                               select new
                                               {
                                                   tsd,
                                                   tsdd
                                               };

                        var model = timesheetDetails.Where(x => x.tsd.tsd_timesheet_id == id).GroupBy(x => new { tsd1 = x.tsd.tsd_id, x.tsdd.tsd_id }).Select(x => new ParentVM
                        {
                            tsd_id = x.Key.tsd1,
                            tsd_timesheet_id = x.FirstOrDefault().tsd.tsd_timesheet_id,
                            tsd_category_id = x.FirstOrDefault().tsd.tsd_category_id,
                            tsd_category_type_id = x.FirstOrDefault().tsd.tsd_category_type_id,
                            timesheet_day_details = x.Select(y => new ChildVM
                            {
                                tdd_id = y.tsdd.tdd_id,
                                tdd_day = y.tsdd.tdd_day,
                                workhours = y.tsdd.workhours,
                                tsd_id = y.tsdd.tsd_id,
                                holiday = y.tsdd.holiday_id
                            }).OrderBy(y => y.tdd_id).ToList()
                        });

/*                        model.Where(x => x.tsd_timesheet_id == id).ToList().ForEach(s => s.timesheet_day_details.ToList().ForEach(y => y.holiday = mP.holiday_details.FirstOrDefault(z => z.hd_from <= y.tdd_day && z.hd_to >= y.tdd_day) != null ? 0 : 1));

                        var newModel = model.Select(i => {
                            i.timesheet_day_details.Select(x =>
                            {
                                x.holiday = mP.holiday_details.FirstOrDefault(z => z.hd_from <= x.tdd_day && z.hd_to >= x.tdd_day) != null ? 0 : 1
                            });
                        });*/

                        return View(model.ToList());
                    }
                    else
                    {
                        //Access denied (trying to access a different timesheet)
                        return View();
                    }
                }
                else
                {
                    // No timesheet_id specified therefore a blank view will be displayed
                    return View();
                }
            }
        }

        [Authorize(Roles = "user, admin, supervisor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TimeSheetEditView(List<ParentVM> pvm, string submit)
        {
            if (ModelState.IsValid)
            {
                using (mmpEntities mP = new mmpEntities())
                {

                    var presenceList = new List<KeyValuePair<DateTime?, double?>>();

                    ViewBag.Categories = mP.categories.ToList<category>();
                    ViewBag.CategoryTypes = mP.category_type_details.ToList<category_type_details>();


                    int tsd_timesheet_id = pvm.Select(x => x.tsd_timesheet_id).FirstOrDefault();
                    ViewBag.TimeSheetID = tsd_timesheet_id;
                    timesheet ts = mP.timesheets.Where(x => x.timesheet_id == tsd_timesheet_id).First();
                    var timeSheetMR = mP.timesheet_mr.Where(x => x.tsmr_id == ts.timesheet_caller).FirstOrDefault();

                    // ADD USER TO THE TIMESHEET EDIT VIEW
                    ViewBag.UserName = mP.users.FirstOrDefault(x => x.user_id == ts.timesheet_user).user_name;

                    ViewBag.startDate = timeSheetMR.tsmr_start_date;
                    ViewBag.endDate = timeSheetMR.tsmr_valid_till;

                    if (submit == "Calculate")
                    {
                        return View(pvm);
                    }

                    // *** TimeSheet 7.5 Check ***
                    for (int i = 0; i < timeSheetMR.days; i++)
                    {
                        if (pvm.Sum(x => x.timesheet_day_details[i].workhours).Value > 7.5)
                        {
                            ViewBag.Message = string.Format("Error! Make sure sum of each column is less than 7.5.");
                            return View(pvm);
                        }
                    }
                    // *** TimeSheet 7.5 Check ***

                    if (ts.timesheet_user == UserID_RoleID.getUserID() && ts.tsmr_extension >= DateTime.Now && (ts.timesheet_status == "saved" || ts.timesheet_status == "rejected")) //Do this properly
                    {
                        switch (submit)
                        {
                            case "Save":
                                if (ts.timesheet_status == "rejected")
                                {
                                    ViewBag.Message = "You are not allowed to save a rejected timeSheet";
                                    ModelState.Clear();
                                    return this.RedirectToAction("TimeSheetEditView", "TimeSheet", new { id = tsd_timesheet_id, message = "You are not allowed to save a rejected timeSheet" });
                                }
                                ts.timesheet_status = "saved";
                                ViewBag.Message = "TimeSheet Saved Successfully";
                                break;
                            case "Submit":
                                ts.timesheet_status = "submitted";
                                ViewBag.Message = "TimeSheet Submitted Successfully";
                                break;
                            //case "Calculate":
                                //return View(pvm);
                        }
                        //ts.timesheet_status = ts.timesheet_status == "rejected" ? "submitted" : "saved"; //Based on button press (cannot be saved, can only be submitted)
                        ts.updated_by = UserID_RoleID.getUserID();
                        ts.timesheet_status_update = DateTime.Now;

                        mP.Entry(ts).State = EntityState.Modified;

                        Debug.WriteLine(pvm);

                        foreach (var parent in pvm)
                        {
                            //ViewBag.TimeSheetID = parent.tsd_timesheet_id;
                            //Debug.WriteLine("Here");                            

                            foreach (var child in parent.timesheet_day_details)
                            {
                                //Debug.WriteLine(child.tdd_id + " " + child.tdd_day + " " + child.workhours + " " + child.tsd_id);
                                timesheet_day_details tsdd = mP.timesheet_day_details.Where(x => x.tdd_id == child.tdd_id).First();
                                tsdd.workhours = child.workhours;

                                // Attendance SETUP *****
                                if (parent.tsd_category_id != mP.categories.First(x => x.category_name == "leaves").category_id) /// USE A DELEGATE OR SOMETHING
                                {
                                    if (presenceList.FindIndex(x => x.Key == child.tdd_day) >= 0)
                                    {
                                        double? workHours = presenceList.First(x => x.Key == child.tdd_day).Value;
                                        workHours = workHours + child.workhours;
                                        presenceList.Add(new KeyValuePair<DateTime?, double?>(child.tdd_day, workHours));
                                    }
                                    else
                                    {
                                        presenceList.Add(new KeyValuePair<DateTime?, double?>(child.tdd_day, child.workhours));
                                    }
                                }

                                mP.Entry(tsdd).State = EntityState.Modified;
                            }
                        }

                        foreach (var item in presenceList)
                        {
                            var pres = mP.presences.Where(x => x.p_date == item.Key && x.user_id == ts.timesheet_user).FirstOrDefault();
                            pres.p_date = item.Key??DateTime.Now;
                            pres.total_hours = item.Value;
                            if (item.Value == 0)
                            {
                                pres.leave_status = "full day";
                            }
                            else
                            {
                                pres.leave_status = item.Value == 7.5 ? "no leave" : "partial leave";
                            }
                            pres.user_id = ts.timesheet_user;

                            mP.Entry(pres).State = EntityState.Modified;
                        }

                        mP.SaveChanges();

                        ModelState.Clear();
                        

                        if (User.Identity.IsAuthenticated && UserID_RoleID.getRole(User.Identity.Name) == "admin")
                        {
                            return this.RedirectToAction("AdminTimeSheets", "TimeSheet");
                        }
                        else
                        {
                            return this.RedirectToAction("UserTimesheets", "TimeSheet");
                        }


                    }
                    else if (ts.timesheet_status == ("accepted") || ts.timesheet_status == "submitted") //Do this properly
                    {
                        ViewBag.Message = string.Format("Cannot edit a Submitted/Accepted TimeSheet");
                        return View(pvm);
                    }
                    else if (ts.tsmr_extension < DateTime.Now)
                    {
                        ViewBag.Message = string.Format("Cannot edit TimeSheet's past their extension date.");
                        return View(pvm);
                    }

                }
            }
            return View();
        }


        [Authorize(Roles = "user, admin, supervisor")]
        [HttpGet]
        public ActionResult AddCategoryTimeSheet(int id = 0)
        {

            using (mmpEntities mP = new mmpEntities())
            {
                ViewBag.Categories = mP.categories.ToList<category>();
                ViewBag.CategoryTypes = mP.category_type_details.ToList<category_type_details>();

                AddCategoryandCategoryType cct = new AddCategoryandCategoryType()
                {
                    tsd_timesheet_id = id
                };

                return View(cct);
            }
        }

        [Authorize(Roles = "user, admin, supervisor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategoryTimeSheet(AddCategoryandCategoryType cct)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                ViewBag.Categories = mP.categories.ToList<category>();
                ViewBag.CategoryTypes = mP.category_type_details.ToList<category_type_details>();

                if (ModelState.IsValid)
                {

                    var timesheets = mP.timesheets.Where(x => x.timesheet_id == cct.tsd_timesheet_id).FirstOrDefault();
                    var timesheets_mr = mP.timesheet_mr.Where(x => x.tsmr_id == timesheets.timesheet_caller).FirstOrDefault();

                    if (timesheets.timesheet_status == "submitted" || timesheets.timesheet_status == "accepted" || timesheets.tsmr_extension < DateTime.Now)  // FIND A BETTER WAY
                    {
                        ViewBag.Message = "Cannot Add new categories to accepted/submitted timesheets";
                        return Json(new { success = true, message = "Cannot Add new categories to accepted/submitted timesheets" }, JsonRequestBehavior.AllowGet);
                    }

                    if (timesheets.timesheet_user == UserID_RoleID.getUserID()) //Do this properly
                    {
                        #region Category Type already exists in TimeSheet
                        var isCategoryTypeExists = IsCategoryTypeExist(cct.tsd_timesheet_id, cct.tsd_category_type_id);
                        if (isCategoryTypeExists)
                        {
                            ModelState.AddModelError("CategoryTypeExists", "Category Type already exists");
                            return View(cct);
                        }
                        #endregion


                        timesheet_details td = new timesheet_details()
                        {
                            tsd_timesheet_id = cct.tsd_timesheet_id,
                            tsd_category_id = cct.tsd_category_id,
                            tsd_category_type_id = cct.tsd_category_type_id
                        };
                        mP.timesheet_details.Add(td);
                        mP.SaveChanges();

                        //int DayInterval = 1;                    

                        foreach (DateTime day in EachDay(timesheets_mr.tsmr_start_date, timesheets_mr.tsmr_valid_till))
                        {
                            var hdd = mP.holiday_details.Where(z => z.hd_from <= day && z.hd_to >= day).FirstOrDefault<holiday_details>();
                            if (hdd != null)
                            {
                                timesheet_day_details tdd = new timesheet_day_details()
                                {
                                    tdd_day = day,
                                    workhours = 0,
                                    tsd_id = td.tsd_id,
                                    holiday_id = hdd.hd_id
                                };
                                mP.timesheet_day_details.Add(tdd);

                            }
                            else
                            {
                                timesheet_day_details tdd = new timesheet_day_details()
                                {
                                    tdd_day = day,
                                    workhours = 0,
                                    tsd_id = td.tsd_id,
                                    holiday_id = null
                                };
                                mP.timesheet_day_details.Add(tdd);
                            }
                            mP.SaveChanges();
                        }

                        return Json(new { success = true, message = "New Row Added to TimeSheet Successfully" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = true, message = "Go back to your own TimeSheet" }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    return View(cct);
                }
            }
        }


        #region Extend TimeSheet date
        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult ExtendTimeSheet(int id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                if (id != 0)
                {
                    var timesheet = mP.timesheets.Where(x => x.timesheet_id == id).FirstOrDefault();
                    var timesheet_caller = mP.timesheet_mr.Where(x => x.tsmr_id == timesheet.timesheet_caller).FirstOrDefault();
                    ExtendTimeSheet ets = new ExtendTimeSheet()
                    {
                        tsd_timesheet_id = timesheet.timesheet_id,
                        tsmr_valid_till = timesheet_caller.tsmr_valid_till,
                        tsmr_extension = timesheet.tsmr_extension
                    };

                    return View(ets);
                }
                else
                {
                    return View();
                }
                
            }
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExtendTimeSheet(ExtendTimeSheet ets)
        {
            string body;
            if (ModelState.IsValid)
            {
                using (mmpEntities mP = new mmpEntities())
                {
                    if (ets.tsmr_extension >= ets.tsmr_valid_till && ets.tsmr_extension > DateTime.Now && ets.tsmr_extension < ets.tsmr_valid_till.AddMonths(6))
                    {
                        timesheet ts = mP.timesheets.Where(x => x.timesheet_id == ets.tsd_timesheet_id).FirstOrDefault();
                        ts.tsmr_extension = ets.tsmr_extension;
                        mP.Entry(ts).State = EntityState.Modified;
                        mP.SaveChanges();

                        var user = mP.users.Where(x => x.user_id == ts.timesheet_user).FirstOrDefault<user>();
                        timesheet_mr tsmr = mP.timesheet_mr.Where(x => x.tsmr_id == ts.timesheet_caller).FirstOrDefault<timesheet_mr>();

                        body = "<br></br>" + user.user_name + ", Your TimeSheet that was created at " + tsmr.tsmr_start_date + " and Valid till "+ tsmr.tsmr_valid_till+" has been extended till "+ets.tsmr_extension+". Visit the following website to access timeSheet<br></br>" +
                               "<a href='http://magcom-001-site3.etempurl.com'>http://magcom-001-site3.etempurl.com</a>";

                        Task.Run(() => EmailAlert.SendEmail(user.user_email, body));

                        return Json(new { success = true, message = "TimeSheet submittion date successfully extended" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Failed to extend TimeSheet submittion date" }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            else
            {
                return Json(new { success = false, message = "Invalid ModelState" }, JsonRequestBehavior.AllowGet);
            }
            
        }
        #endregion

        #region Delete TimeSheet Row from tables [timesheet_details] & [timesheet_day_details]
        [HttpPost]
        public ActionResult Delete(int id = 0, int category_typeID = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                Debug.WriteLine(id);
                Debug.WriteLine(category_typeID);
                timesheet ts = mP.timesheets.Where(x => x.timesheet_id == id).First();
                if (ts.timesheet_status == "submitted" || ts.timesheet_status == "accepted" || ts.tsmr_extension < DateTime.Now) // FIND A BETTER WAY
                {
                    ViewBag.Message = "You are not allowed to delete from a submitted/accepted timeSheet";
                    return Json(new { success = true, message = "Invalid" }, JsonRequestBehavior.AllowGet);
                }
                        
                timesheet_details td = mP.timesheet_details.Where(x => x.tsd_timesheet_id == id && x.tsd_category_type_id == category_typeID).FirstOrDefault<timesheet_details>();
                Debug.WriteLine(td.tsd_id);

                timesheet_day_details tdd = mP.timesheet_day_details.Where(x => x.tsd_id == td.tsd_id).FirstOrDefault<timesheet_day_details>();
                mP.timesheet_details.Remove(td);
                mP.timesheet_day_details.Remove(tdd);
                mP.SaveChanges();
                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);


            }
        }
        #endregion

        [HttpPost]
        public ActionResult GetCategoryType(string categoryID)
        {
            List<category_type_details> lstctd = new List<category_type_details>();
            int categoryiD = Convert.ToInt32(categoryID);
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;
                lstctd = (mP.category_type_details.Where(x => x.category_id == categoryiD)).ToList<category_type_details>();

                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                string result = javaScriptSerializer.Serialize(lstctd);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }


        [NonAction]
        public static DateTime StartOfWeek( DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        [NonAction]
        public bool IsCategoryTypeExist(int timesheet_id = 0, int category_type_id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                var v = mP.timesheet_details.Where(a => a.tsd_timesheet_id == timesheet_id && a.tsd_category_type_id == category_type_id).FirstOrDefault();
                return v != null;
            }
        }

        [NonAction]
        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }
    }
}