using MMP.Generic_Functions;
using MMP.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

            if (User.Identity.IsAuthenticated && UserID_RoleID.getRole(User.Identity.Name) != "admin")
            {
                //send them to the AuthenticatedIndex page instead of the index page
                return RedirectToAction("UserTimesheets", "TimeSheet");
            }


            List<string> sectors = new List<string>();
            List<int> project_count = new List<int>();

            using (mmpEntities mP = new mmpEntities())
            {
                List<DataPoint> dataPoints = new List<DataPoint>();

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
        
    }
}