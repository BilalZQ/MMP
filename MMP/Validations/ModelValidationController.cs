using MMP.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMP.Validations
{
    public class ModelValidationController : Controller
    {
        [AllowAnonymous]
        [HttpPost]
        public JsonResult CheckExistingEmail(string user_email, int user_id = 0)
        {
            try
            {
                return Json(!IsEmailExist(user_email, user_id));
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult CheckTimeSheetExtention(DateTime tsmr_extension, DateTime tsmr_valid_till)
        {
            try
            {
                return Json(!IsTimeSheetExtentionValid(tsmr_extension, tsmr_valid_till));
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult CheckExistingEmployeeID(string employee_id, int user_id = 0)
        {
            try
            {
                return Json(!IsEmployeeIDExist(employee_id, user_id));
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult CheckExistingCategoryTypeCode(string code, int ctd_id = 0)
        {
            try
            {
                return Json(!IsCategoryTypeAssigned(code, ctd_id));
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        

        [AllowAnonymous]
        [HttpPost]
        public JsonResult CheckExistingRegions(string region_name, int region_id = 0)
        {
            try
            {
                return Json(!IsRegionExist(region_name, region_id));
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult CheckExistingSectors(string sector_name, int sector_id = 0)
        {
            try
            {
                return Json(!IsSectorExist(sector_name, sector_id));
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }


        [AllowAnonymous]
        [HttpPost]
        public JsonResult CheckExistingTimeSheetCategory(int tsd_category_type_id = 0, int tsd_timesheet_id = 0)
        {
            try
            {
                return Json(!IsTimeSheetCategoryTypeExist(tsd_category_type_id, tsd_timesheet_id));
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        ///***
        [AllowAnonymous]
        [HttpPost]
        public JsonResult CheckExistingHolidays(string hd_name, int hy_id)
        {
            try
            {

                Debug.WriteLine("CheckExistingHolidays here");
                return Json(!IsHolidayAssigned(hd_name, hy_id));
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult CheckHolidayDateRange(DateTime hd_from, DateTime hd_to)
        {
            try
            {
                if (hd_from.Date > hd_to.Date)
                {
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
                
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }





        //=========================================================================================================================================================

        private bool IsEmployeeIDExist(string employee_id, int user_id)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                var v = mP.users.Where(a => a.employee_id == employee_id && a.user_id != user_id).FirstOrDefault();
                //return v == null ? false : true;
                return v != null;
            }
        }

        private bool IsEmailExist(string email, int user_id)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                var v = mP.users.Where(a => a.user_email == email && a.user_id != user_id).FirstOrDefault();
                //return v == null ? false : true;
                return v != null;
            }
        }


        private bool IsTimeSheetExtentionValid(DateTime tsmr_extension, DateTime tsmr_valid_till)
        {
            using (mmpEntities mP = new mmpEntities())
            {

                if (tsmr_extension >= tsmr_valid_till && tsmr_extension > DateTime.Now && tsmr_extension < tsmr_valid_till.AddMonths(6))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }


        private bool IsCategoryTypeAssigned(string ctd, int ctd_id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;
                var v = mP.category_type_details.Where(x => (x.ctd_code == ctd) && x.ctd_id != ctd_id).FirstOrDefault();
                return v != null;
            }
        }

        private bool IsHolidayAssigned(string hd_name, int hy_id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                var v = mP.holiday_details.Where(a => (a.hd_name == hd_name) && a.hy_id == hy_id).FirstOrDefault();
                //Debug.WriteLine(v == null ? "empty" : "not empty");
                return v != null;
            }
        }

        private bool IsRegionExist(string regionName, int region_id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                var v = mP.regions.Where(x => x.region_name == regionName && x.region_id != region_id).FirstOrDefault();
                return v != null;
            }
        }

        private bool IsSectorExist(string sectorName, int sector_id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                var v = mP.sectors.Where(x => x.sector_name == sectorName && x.sector_id != sector_id).FirstOrDefault();
                return v != null;
            }
        }

        private bool IsTimeSheetCategoryTypeExist(int category_type_id = 0, int timesheet_id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                var v = mP.timesheet_details.Where(a => a.tsd_timesheet_id == timesheet_id && a.tsd_category_type_id == category_type_id).FirstOrDefault();
                return v != null;
            }
        }

    }
}