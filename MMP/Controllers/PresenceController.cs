using MMP.Generic_Functions;
using MMP.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMP.Controllers
{
    
    public class PresenceController : Controller
    {
        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult UserPresence(int id = 0)
        {
            ViewBag.user_id = id;
            return View();
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult GetData(int id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;
                
                var presence = (from p in mP.presences 
                               join u in mP.users on p.user_id equals u.user_id
                               select new
                               {
                                   p,
                                   u
                               }).Where(x => x.u.user_id == id);
                

                return Json(new { data = presence.AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "supervisor")]
        [HttpGet]
        public ActionResult SuperVisorUserPresence(int id = 0)
        {
            ViewBag.user_id = id;
            Debug.WriteLine(id);
            return View();
        }
        [Authorize(Roles = "supervisor")]
        [HttpGet]
        public ActionResult GetSupervisorUserData(int id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;
                int supervisor_id = UserID_RoleID.getUserID();
                var presence = (from p in mP.presences
                                join u in mP.users on p.user_id equals u.user_id
                                select new
                                {
                                    p,
                                    u
                                }).Where(x => x.u.user_id == id && x.u.supervisor == supervisor_id);
 

                return Json(new { data = presence.AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}