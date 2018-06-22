using MMP.Models;
using MMP.Models.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMP.Controllers.Reports
{
    public class SectorProjectsController : Controller
    {
        // GET: SectorProjects
        public ActionResult SSProjectReport()
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;
                ViewBag.Sectors = mP.sectors.ToList<sector>();

                return View(new SSProjectReport());
            }
        }

        public ActionResult GetSectorProjectsTotalHours(int id = 0, string startDate = "", string endDate = "")
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;

                var ret = mP.ReportSectorProjectTotalHours(id, startDate, endDate).ToList<ReportSectorProjectTotalHours_Result>();

                return Json(new { data = ret }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}