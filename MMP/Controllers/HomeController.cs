using MMP.Generic_Functions;
using MMP.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMP.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            using (mmpEntities mP = new mmpEntities())
            {

                if (User.Identity.IsAuthenticated && UserID_RoleID.getRole(User.Identity.Name) != "admin")
                {
                    //send them to the AuthenticatedIndex page instead of the index page
                    int user_id = UserID_RoleID.getUserID();
                    var ts_id = mP.timesheets.OrderByDescending(x => x.time_my).FirstOrDefault(x => x.timesheet_user == user_id).timesheet_id;
                    //return RedirectToAction("UserTimesheets", "TimeSheet");
                    if (ts_id > 0)
                    {
                        return RedirectToAction("TimeSheetEditView", "TimeSheet", new { id = ts_id });
                    }
                    else
                    {
                        return RedirectToAction("UserTimesheets", "TimeSheet");
                    }
                    //'@Url.Action("TimeSheetEditView", "TimeSheet")/'+id
                }


                List<string> sectors = new List<string>();
                List<int> project_count = new List<int>();

                List<DataPoint> dataPoints = new List<DataPoint>();


                List<DataPoint> timeSheet_status_count = new List<DataPoint>();

                mP.Configuration.ProxyCreationEnabled = false;
                var usersPerRole = from user in mP.users
                                   group user by user.role into userGroup
                                   select new
                                   {
                                       value = userGroup.Key.role_name,
                                       count = userGroup.Count(),
                                   };

                foreach (var item in usersPerRole)
                {
                    //Debug.WriteLine(item.value);
                    //Debug.WriteLine(item.count);
                    dataPoints.Add(new DataPoint(item.value.ToString().ToUpper(), item.count));
                }

                //var timeSheetCountByStatus = from 


                var projectsPerSector = from pd in mP.project_details
                                        group pd by pd.sector into projectGroup
                                        select new
                                        {
                                            value = projectGroup.Key.sector_name,
                                            count = projectGroup.Count()
                                        };
                foreach (var item in projectsPerSector)
                {
                    sectors.Add(item.value.ToString().ToUpper());
                    project_count.Add(item.count);
                }

                ViewBag.DoughnutDataPoints = JsonConvert.SerializeObject(dataPoints);
                ViewBag.Sectors = JsonConvert.SerializeObject(sectors);
                ViewBag.ProjecsCount = JsonConvert.SerializeObject(project_count);

                //Debug.WriteLine(JsonConvert.SerializeObject(dataPoints));
                //Debug.WriteLine(JsonConvert.SerializeObject(sectors));
                //Debug.WriteLine(JsonConvert.SerializeObject(project_count));

                return View();
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult GetTimeSheetCountByStatus()
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;

                var query = (from tsmr in mP.timesheet_mr
                             join ts in mP.timesheets on tsmr.tsmr_id equals ts.timesheet_caller
                             select new
                             {
                                 tsmr,
                                 ts
                             }).Where(x => x.tsmr.tsmr_valid_till >= DateTime.Now).GroupBy(x => new {
                                                                                                x.ts.timesheet_status
                                                                                                })
                                                                                   .Select(x => new
                                                                                   {
                                                                                       timeSheet_Status = x.Key.timesheet_status,
                                                                                       timeSheet_Count = x.Count()
                                                                                   });

                return Json(new { data = query.AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
            }
        }
        
    }
}