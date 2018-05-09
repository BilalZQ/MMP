using MMP.Models;
using MMP.Models.ViewModels.Category;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMP.Controllers
{
    public class CategoryController : Controller
    {
        [HttpGet]
        // GET: CategoryDetails
        public ActionResult CategoryDetails()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetData()
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;

                var category_type_details = from ctd in mP.category_type_details
                                            join ld in mP.leave_details on ctd.ctd_id equals ld.category_type_id into ldps from ld in ldps.DefaultIfEmpty()
                                            join proj in mP.project_details on ctd.ctd_id equals proj.category_type_id into projps from proj in projps.DefaultIfEmpty()
                                            join category in mP.categories on ctd.category_id equals category.category_id
                                            join sector in mP.sectors on proj.sector_id equals sector.sector_id into sectorps from sector in sectorps.DefaultIfEmpty()
                                            select new
                                            {
                                                ctd,
                                                ld,
                                                proj,
                                                category,
                                                sector,
                                                sector_name = proj == null ? "" : sector.sector_name,
                                                project_model = proj == null ? "" : proj.project_model,
                                                no_of_leaves = ld == null ? "" : ld.no_of_leaves.ToString(),
                                                encashable = ld == null ? "" : ld.encashable,
                                                carry_forward = ld == null ? "" : ld.carry_forward,
                                                project_id = proj == null ? 0 : proj.id,
                                                leave_id = ld == null ? 0 : ld.id
                                            };
                Debug.WriteLine(category_type_details.AsNoTracking().ToList());
                return Json(new { data = category_type_details.AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult SelectCategoryType()
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;
                ViewBag.categories = mP.categories.ToList<category>();
                return View(new category());
            }
        }

        [HttpGet]
        public ActionResult AddorEditDepartments(int id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                if (id == 0)
                {
                    return View(new AddorEditDepartment() { category_id = mP.categories.First(x => x.category_name == "departments").category_id });
                }
                else
                {
                    var query = mP.category_type_details.Where(x => x.ctd_id == id).FirstOrDefault<category_type_details>();
                    AddorEditDepartment aed = new AddorEditDepartment()
                    {
                        ctd_id = query.ctd_id,
                        dept_name = query.ctd_name,
                        dept_code = query.ctd_code,
                        category_id = query.category_id,
                        ctd_created_at = query.ctd_created_at
                    };
                    return View(aed);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddorEditDepartments(AddorEditDepartment aed)
        {
            if (ModelState.IsValid)
            {
                using (mmpEntities mP = new mmpEntities())
                {
                    #region Department Name already exists 
                    var isDepartmentNameAssigned = IsCategoryTypeAssigned(aed.dept_name, aed.ctd_id, 1);
                    if (isDepartmentNameAssigned)
                    {
                        ModelState.AddModelError("DeparmentAssigned", "Department Name is already assigned");
                        return View(aed);
                    }
                    #endregion

                    #region Department Code already assigned
                    var isDepartmentCodeAssigned = IsCategoryTypeAssigned(aed.dept_code, aed.ctd_id, 0);
                    if (isDepartmentCodeAssigned)
                    {
                        ModelState.AddModelError("DepartmentAssigned", "Department Code is already assigned");
                        return View(aed);
                    }
                    #endregion 

                    if (aed.ctd_id == 0)
                    {
                        category_type_details ctd = new category_type_details()
                        {
                            ctd_name = aed.dept_name,
                            ctd_code = aed.dept_code,
                            category_id = aed.category_id,
                            ctd_created_at = DateTime.Now

                        };

                        mP.category_type_details.Add(ctd);
                        mP.SaveChanges();
                        return Json(new { success = true, message = "Saved Successfully " }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        category_type_details ctd = new category_type_details()
                        {
                            ctd_id = aed.ctd_id,
                            ctd_name = aed.dept_name,
                            ctd_code = aed.dept_code,
                            category_id = aed.category_id,
                            ctd_created_at = aed.ctd_created_at,
                            ctd_updated_at = DateTime.Now
                        };
                        mP.Entry(ctd).State = EntityState.Modified;
                        mP.SaveChanges();
                        return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);

                    }
                    
                }
            }
            else
            {
                return View(aed);
            }
        }
        
        [HttpGet]
        public ActionResult AddorEditProjects(int id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                ViewBag.Sectors = mP.sectors.ToList<sector>();
                if (id == 0)
                {
                    return View(new AddorEditProject() { category_id = mP.categories.First(x => x.category_name == "projects").category_id });
                }
                else
                {
                    //ctd = category type details
                    var ctd = mP.category_type_details.Where(x => x.ctd_id == id).FirstOrDefault<category_type_details>();
                    //pd = project details
                    var pd = mP.project_details.Where(x => x.category_type_id == ctd.ctd_id).FirstOrDefault<project_details>();

                    AddorEditProject aep = new AddorEditProject()
                    {
                        ctd_id = ctd.ctd_id,
                        proj_name = ctd.ctd_name,
                        proj_code = ctd.ctd_code,
                        category_id = ctd.category_id,
                        ctd_created_at = ctd.ctd_created_at,
                        proj_details_id = pd.id,
                        sector_id = pd.sector_id,
                        proj_model = pd.project_model
                    };
                    return View(aep);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddorEditProjects(AddorEditProject aep)
        {
            if (ModelState.IsValid)
            {
                using (mmpEntities mP = new mmpEntities())
                {
                    ViewBag.Sectors = mP.sectors.ToList<sector>();
                    #region Project Name is already assigned
                    var isProjectNameAssigned = IsCategoryTypeAssigned(aep.proj_name, aep.ctd_id, 1);
                    if (isProjectNameAssigned)
                    {
                        ModelState.AddModelError("ProjectAssigned", "Project Name is already assigned");
                        return View(aep);
                    }
                    #endregion

                    #region Project Code is already assigned
                    var isProjectCodeAssigned = IsCategoryTypeAssigned(aep.proj_code, aep.ctd_id, 0);
                    if (isProjectCodeAssigned)
                    {
                        ModelState.AddModelError("ProjectAssigned", "Project Code is already assigned");
                        return View(aep);
                    }
                    #endregion

                    if (aep.ctd_id == 0)
                    {
                        category_type_details ctd = new category_type_details()
                        {
                            ctd_name = aep.proj_name,
                            ctd_code = aep.proj_code,
                            category_id = aep.category_id,
                            ctd_created_at = DateTime.Now
                        };
                        
                        mP.category_type_details.Add(ctd);
                        mP.SaveChanges();

                        project_details pd = new project_details()
                        {
                            sector_id = aep.sector_id,
                            project_model = aep.proj_model,
                            category_type_id = ctd.ctd_id
                        };
                        mP.project_details.Add(pd);
                        mP.SaveChanges();
                        return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        category_type_details ctd = new category_type_details()
                        {
                            ctd_id = aep.ctd_id,
                            ctd_name = aep.proj_name,
                            ctd_code = aep.proj_code,
                            category_id = aep.category_id,
                            ctd_created_at = aep.ctd_created_at,
                            ctd_updated_at = DateTime.Now
                        };
                        
                        project_details pd = new project_details()
                        {
                            id = aep.proj_details_id,
                            sector_id = aep.sector_id,
                            project_model = aep.proj_model,
                            category_type_id = aep.ctd_id
                        };
                        Debug.WriteLine(aep.proj_model);
                        mP.Entry(ctd).State = EntityState.Modified;
                        mP.Entry(pd).State = EntityState.Modified;
                        mP.SaveChanges();

                        return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                    }


                }
            }
            else
            {
                return View(aep);
            }

        }

        [HttpGet]
        public ActionResult AddorEditLeaves(int id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                if (id == 0)
                {
                    return View(new AddorEditLeave() { category_id = mP.categories.First(x => x.category_name == "leaves").category_id });
                }
                else
                {
                    //ctd = category type details
                    var ctd = mP.category_type_details.Where(x => x.ctd_id == id).FirstOrDefault<category_type_details>();
                    //ld = leave details 
                    var ld = mP.leave_details.Where(x => x.category_type_id == ctd.ctd_id).FirstOrDefault<leave_details>();

                    AddorEditLeave ael = new AddorEditLeave()
                    {
                        ctd_id = ctd.ctd_id,
                        leave_name = ctd.ctd_name,
                        leave_code = ctd.ctd_code,
                        category_id = ctd.category_id,
                        ctd_created_at = ctd.ctd_created_at,
                        leave_details_id = ld.id,
                        no_of_leaves = ld.no_of_leaves,
                        encashable = ld.encashable,
                        carry_forward = ld.carry_forward
                    };
                    return View(ael);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddorEditLeaves(AddorEditLeave ael)
        {
            if (ModelState.IsValid)
            {
                using (mmpEntities mP = new mmpEntities())
                {
                    #region Leave Name already assigned 
                    var isLeaveNameAssigned = IsCategoryTypeAssigned(ael.leave_name, ael.ctd_id, 1);
                    if (isLeaveNameAssigned)
                    {
                        ModelState.AddModelError("LeaveAssigned", "Leave Name is already assigned");
                        return View(ael);
                    }
                    #endregion

                    #region Leave Code already assigned
                    var isLeaveCodeAssigned = IsCategoryTypeAssigned(ael.leave_code, ael.ctd_id, 0);
                    if (isLeaveCodeAssigned)
                    {
                        ModelState.AddModelError("LeaveAssigned", "Leave Code is already assigned");
                        return View(ael);
                    }
                    #endregion

                    if (ael.ctd_id == 0)
                    {
                        category_type_details ctd = new category_type_details()
                        {
                            ctd_name = ael.leave_name,
                            ctd_code = ael.leave_code,
                            category_id = ael.category_id,
                            ctd_created_at = DateTime.Now
                        };

                        mP.category_type_details.Add(ctd);
                        mP.SaveChanges();

                        leave_details ld = new leave_details()
                        {
                            no_of_leaves = ael.no_of_leaves,
                            encashable = ael.encashable,
                            carry_forward = ael.carry_forward,
                            category_type_id = ctd.ctd_id
                        };

                        mP.leave_details.Add(ld);
                        mP.SaveChanges();
                        return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        category_type_details ctd = new category_type_details()
                        {
                            ctd_id = ael.ctd_id,
                            ctd_name = ael.leave_name,
                            ctd_code = ael.leave_code,
                            category_id = ael.category_id,
                            ctd_created_at = ael.ctd_created_at,
                            ctd_updated_at = DateTime.Now
                        };
                        

                        leave_details ld = new leave_details()
                        {
                            id = ael.leave_details_id,
                            no_of_leaves = ael.no_of_leaves,
                            encashable = ael.encashable,
                            carry_forward = ael.carry_forward,
                            category_type_id = ctd.ctd_id
                        };

                        mP.Entry(ctd).State = EntityState.Modified;
                        mP.Entry(ld).State = EntityState.Modified;
                        mP.SaveChanges();

                        return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                return View(ael);
            }
        }

        
        //Non Acion fuckery
        [NonAction]
        public bool IsCategoryTypeAssigned(string ctd, int ctd_id = 0, int flag = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;
                if (flag == 1)
                {
                    var v = mP.category_type_details.Where(x => (x.ctd_name == ctd) && x.ctd_id != ctd_id).FirstOrDefault();
                    return v != null;
                }
                else
                {
                    var v1 = mP.category_type_details.Where(x => (x.ctd_code == ctd) && x.ctd_id != ctd_id).FirstOrDefault();
                    return v1 != null;
                }
            }
        }

    }
}