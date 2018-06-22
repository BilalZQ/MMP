using MMP.Models;
using System;
using MMP.Models.ViewModels.Sector;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMP.Controllers
{
    [Authorize(Roles = "admin")]
    public class SectorController : Controller
    {
        [HttpGet]
        // GET: Sector
        public ActionResult SectorDetails()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetData()
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;
                var sectors = from sector in mP.sectors
                              select new
                              {
                                  sector
                              };
                return Json(new { data = sectors.AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult AddSectors()
        {
            List<AddSector> sector = new List<AddSector> { new AddSector { sector_name = "" } };
            return View(sector);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSectors(List<AddSector> sectors)
        {
            if (ModelState.IsValid)
            {
                using (mmpEntities mP = new mmpEntities())
                {
                    mP.Configuration.ProxyCreationEnabled = false;

                    foreach (var i in sectors)
                    {
                        #region Sector Already Exists
                        var isSectorExists = IsSectorExist(i.sector_name);
                        if (isSectorExists)
                        {
                            ModelState.AddModelError("SectorExists", "Sector already exists");
                            return View(sectors);
                        }
                        #endregion
                        sector sector = new sector()
                        {
                            sector_name = i.sector_name
                        };
                        mP.sectors.Add(sector);
                    }
                    mP.SaveChanges();
                    return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return View(sectors);
            }
        }

        [HttpGet]
        public ActionResult EditSector(int id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                var sector = mP.sectors.Where(x => x.sector_id == id).FirstOrDefault<sector>();
                EditSector ae = new EditSector()
                {
                    sector_id = sector.sector_id,
                    sector_name = sector.sector_name
                };
                return View(ae);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSector(EditSector ae)
        {
            if (ModelState.IsValid)
            {
                using (mmpEntities mP = new mmpEntities())
                {
                    sector sector = new sector()
                    {
                        sector_id = ae.sector_id,
                        sector_name = ae.sector_name
                    };
                    mP.Entry(sector).State = EntityState.Modified;
                    mP.SaveChanges();
                    return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                }
            }
            return View();
        }

        [NonAction]
        public bool IsSectorExist(string sectorName)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                var v = mP.sectors.Where(x => x.sector_name == sectorName).FirstOrDefault();
                return v != null;
            }
        }
    }
}