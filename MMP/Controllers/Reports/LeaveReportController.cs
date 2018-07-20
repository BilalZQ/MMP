using MMP.Models;
using MMP.Models.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMP.Controllers.Reports
{
    public class LeaveReportController : Controller
    {
        [Authorize(Roles = "admin")]
        // GET: Leave
        public ActionResult USLeaveReport()
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;
                ViewBag.Users = mP.users.ToList<user>();

                return View(new USLeaveReport());
            }
        }

        public JsonResult GetUser(string Areas, string term = "")
        {
            using (mmpEntities mP = new mmpEntities())
            {
                // Use Stored Procedure
                var objCustomerlist = mP.users.Where(c => c.user_status == "active" && c.employee_id.ToUpper()
                            .Contains(term.ToUpper()))
                            .Select(c => new { Name = c.employee_id, ID = c.user_id })
                            .Distinct().ToList();
                return Json(objCustomerlist, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTotalLeaves(int id = 0, string startDate = "", string endDate = "")
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;

                var ret = mP.ReportLeavesTotal(id, startDate, endDate).ToList<ReportLeavesTotal_Result>();

                return Json(new { data = ret }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetUserLeaves(int id = 0, string startDate = "", string endDate = "")
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;

                var ret = mP.ReportLeaves(id, startDate, endDate).ToList<ReportLeaves_Result>();

                return Json(new { data = ret }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}