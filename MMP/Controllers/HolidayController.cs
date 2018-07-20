using MMP.Models;
using MMP.Models.ViewModels.Holiday;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMP.Controllers
{
    [Authorize(Roles = "admin")]
    public class HolidayController : Controller
    {
        [HttpGet]
        // GET: Holiday
        public ActionResult Holidays()
        {
            return View();
        }


        [HttpGet]
        public ActionResult GetData()
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;

                DateTime date = DateTime.Now;
                ViewBag.Year = date.Year;
                var year = mP.holiday_year.Where(x => x.year == DateTime.Now.Year).FirstOrDefault<holiday_year>();
                if (year == null)
                {
                    holiday_year hy = new holiday_year()
                    {
                        hy_name = "Holiday list for " + date.Year.ToString(),
                        creation_date = date,
                        year = (Int16)date.Year // Cast int to short
                    };
                    mP.holiday_year.Add(hy);
                    mP.SaveChanges();
                    return Json(new { data = year }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var holidays =  (from hd in mP.holiday_details
                                    join hy in mP.holiday_year on hd.hy_id equals hy.hy_id
                                    select new
                                    {
                                        hd,
                                        year = hy.year
                                    }).OrderByDescending(x => x.year);

                    return Json(new { data = holidays.AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpGet]
        public ActionResult AddHolidays(int id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;

                holiday_year hy = mP.holiday_year.Where(x => x.year == DateTime.Now.Year).OrderByDescending(x => x.hy_id).First<holiday_year>();
                List<AddHolidays> AHD = new List<AddHolidays> { new AddHolidays { hd_name = "", hd_from = DateTime.Now, hd_to = DateTime.Now, hy_id = hy.hy_id } };
                return View(AHD);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddHolidays(List<AddHolidays> AHD)
        {
            if (ModelState.IsValid)
            {
                using (mmpEntities mP = new mmpEntities())
                {
                    mP.Configuration.ProxyCreationEnabled = false;

                    foreach (var i in AHD)
                    {
                        /*#region Holiday Already Exists
                        var isHolidayAssigned = IsHolidayAssigned(i.hd_name, i.hy_id, i.hd_from.Date);
                        if (isHolidayAssigned)
                        {
                            ModelState.AddModelError("HolidayAssigned", "Holiday Already added for the year");
                            return View(AHD);
                        }
                        #endregion
                        #region Validate dates
                        if (i.hd_from.Date > i.hd_to.Date)
                        {
                            ModelState.AddModelError("InvalidDate", "To Date should be equal to or greater than start date");
                            return View(AHD);
                        }
                        #endregion*/
                        holiday_details hd = new holiday_details()
                        {
                            hd_name = i.hd_name,
                            hd_from = i.hd_from,
                            hd_to = i.hd_to,
                            hy_id = i.hy_id,
                            generated_by = 1 // AUTOMATE USER ID 
                        };
                        mP.holiday_details.Add(hd);                        
                    }
                    mP.SaveChanges();
                    ModelState.Clear();
                    return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                }
            }
            return View(AHD);
        }

        [HttpGet]
        public ActionResult EditHolidays(int id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                holiday_details hd = mP.holiday_details.Where(x => x.hd_id == id).FirstOrDefault<holiday_details>();
                return View(hd);
            }
        }

        [HttpPost]
        public ActionResult EditHolidays(holiday_details hd)
        {
            if (ModelState.IsValid)
            {
                using (mmpEntities mP = new mmpEntities())
                {
                    mP.Configuration.ProxyCreationEnabled = false;

                    #region Holiday Already Exists
                    var isHolidayAssigned = IsHolidayAssigned(hd.hd_name, hd.hy_id, hd.hd_from.Date, hd.hd_id);
                    if (isHolidayAssigned)
                    {
                        ModelState.AddModelError("HolidayAssigned", "Holiday Already added for the year");
                        return View(hd);
                    }
                    #endregion
                    #region Validate dates
                    if (hd.hd_from.Date > hd.hd_to.Date)
                    {
                        ModelState.AddModelError("InvalidDate", "To Date should be equal to or greater than start date");
                        return View(hd);
                    }
                    #endregion
                    mP.Entry(hd).State = EntityState.Modified;
                    mP.SaveChanges();
                    return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                }
            }
            return View(hd);
        }

        /*[HttpPost]
        public ActionResult Delete(int id)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                holiday_details hd = mP.holiday_details.Where(x => x.hd_id == id).FirstOrDefault<holiday_details>();
                mP.holiday_details.Remove(hd);
                mP.SaveChanges();
                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }*/
        
        ///
        [NonAction]
        public bool IsHolidayAssigned(string hd_name, int hy_id = 0, DateTime? fromdate = null, int hd_id = 0)
        {
             using (mmpEntities mP = new mmpEntities())
            {
                var v = mP.holiday_details.Where(a => (a.hd_name == hd_name || a.hd_from == fromdate) && a.hy_id == hy_id && a.hd_id != hd_id).FirstOrDefault();
                Debug.WriteLine(v == null ? "empty" : "not empty");
                return v != null;
            }
        }
        
    }
}