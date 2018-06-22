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
    public class PresenceController : Controller
    {
        [HttpGet]
        public ActionResult UserPresence(int id = 0)
        {
            ViewBag.user_id = id;
            return View();
        }

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
    }
}