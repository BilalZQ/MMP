using MMP.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMP.Models.ViewModels.Region;
using System.Diagnostics;

namespace MMP.Controllers
{
    [Authorize(Roles = "admin")]
    public class RegionController : Controller
    {
        [HttpGet]
        // GET: Region
        public ActionResult RegionDetails()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetData()
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;
                var regions = from region in mP.regions
                              select new
                              {
                                  region
                              };
                return Json(new { data = regions.AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult AddRegions()
        {
            List<AddRegion> region = new List<AddRegion> { new AddRegion { region_name = "" } };
            return View(region);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRegions(List<AddRegion> regions)
        {
            if (ModelState.IsValid)
            {
                using (mmpEntities mP = new mmpEntities())
                {
                    mP.Configuration.ProxyCreationEnabled = false;

                    foreach (var i in regions)
                    {
                        #region Region Already exists
                        var isRegionExists = IsRegionExist(i.region_name);
                        if (isRegionExists)
                        {
                            ModelState.AddModelError("RegionExists", "Region already exists");
                            return View(regions);
                        }
                        #endregion
                        region region = new region()
                        {
                            region_name = i.region_name
                        };
                        mP.regions.Add(region);
                    }
                    mP.SaveChanges();
                    return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return View(regions);
            }
        }
        

        [HttpGet]
        public ActionResult EditRegion(int id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                var region = mP.regions.Where(x => x.region_id == id).FirstOrDefault<region>();
                EditRegion ae = new EditRegion()
                {
                    region_id = region.region_id,
                    region_name = region.region_name
                };
                return View(ae);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRegion(EditRegion ae)
        {
            if (ModelState.IsValid)
            {
                using (mmpEntities mP = new mmpEntities())
                {
                    #region Region Already exists
                    var isRegionExists = IsRegionExist(ae.region_name);
                    if (isRegionExists)
                    {
                        ModelState.AddModelError("RegionExists", "Region already exists");
                        return View(ae);
                    }
                    #endregion

                    region region = new region()
                    {
                        region_id = ae.region_id,
                        region_name = ae.region_name
                    };
                    mP.Entry(region).State = EntityState.Modified;
                    mP.SaveChanges();
                    return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                }
            }
            return View();
        }

        [NonAction]
        public bool IsRegionExist(string regionName)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                var v = mP.regions.Where(x => x.region_name == regionName).FirstOrDefault();
                return v != null;
            }
        }
    }
}