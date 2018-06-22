using MMP.Models;
using MMP.Models.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMP.Controllers.Reports
{
    [Authorize(Roles = "admin")]
    public class CategoryReportController : Controller
    {
        // GET: CategoryReport
        [HttpGet]
        public ActionResult USCategoryReport()
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;
                ViewBag.Categories = mP.categories.ToList<category>();

                return View(new USCategoryReport());
            }
        }


        public ActionResult GetCategoryTypeData(int id = 0, string startDate = "", string endDate = "")
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;

                var ret = mP.ReportCategoryTotalHours(id, startDate, endDate).ToList<ReportCategoryTotalHours_Result>();

                return Json(new { data = ret }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetCategoryTypeUserWorkHoursData(int id = 0, string startDate = "", string endDate = "")
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;

                var ret = mP.ReportCategoryUserWorkHours(id, startDate, endDate).ToList<ReportCategoryUserWorkHours_Result>();

                return Json(new { data = ret }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}