using MMP.Generic_Functions;
using MMP.Models;
using MMP.Models.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MMP.Controllers
{
    
    public class UserController : Controller
    {
        // GET: User
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        //REMOVE REGISTRATION
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(Registration Ruser)
        {
            bool status = false; //ViewBag.status
            string message = ""; //ViewBag.message

            if (ModelState.IsValid)
            {
                #region User Already Exists
                var isUserExists = isUserExist(Ruser.user_name);
                if (isUserExists)
                {
                    ModelState.AddModelError("UserExist", "Username already exists");
                    return View(Ruser);
                }
                #endregion

                #region Email Already Exists
                var isEmailExists = IsEmailExist(Ruser.user_email);
                if (isEmailExists)
                {
                    ModelState.AddModelError("EmailExist", "Email already exists");
                    return View(Ruser);
                }
                #endregion

                #region Password Hashing
                Ruser.user_password = Crypto.Hash(Ruser.user_password);
                Ruser.confirmPassword = Crypto.Hash(Ruser.confirmPassword); //DB validates again on save changes
                #endregion

                //var result = Registration(user);  // implicit
                // conversion from RegisterViewModel to User Model

                //Registration vm = result; // implicit conversion from User model to RegisterViewModel

                #region Save User to Database
                using (mmpEntities mP = new mmpEntities())
                {
                    var user = new user
                    {
                        role_id = mP.roles.First(a => a.role_name == "admin").role_id,
                        user_name = Ruser.user_name,
                        user_email = Ruser.user_email,
                        user_password = Ruser.user_password,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now,
                        region_id = 1

                    };
                    mP.users.Add(user);
                    mP.SaveChanges();
                }
                //Send Email to User
                //--SendEmail(user.email);
                message = "Registration successfully completed. Email for confirmation has been sent to email ID: " + Ruser.user_email;
                status = true;
                #endregion
            }
            else
            {
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = status;
            return View(Ruser);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login login, string ReturnURL = "")
        {
            String message = "";
            using (mmpEntities mP = new mmpEntities())
            {
                var v = mP.users.Where(a => a.user_name == login.user_name).FirstOrDefault();
                string Roles = UserID_RoleID.getRole(v.user_name);
                if (v != null)
                {
                    if (string.Compare(Crypto.Hash(login.user_password), v.user_password) == 0)
                    {
                        int timeout = login.RememberMe ? 525600 : 20; // 525600 => 1 year
                        //var ticket = new FormsAuthenticationTicket(login.user_name, login.RememberMe, timeout);
                        var authTicket = new FormsAuthenticationTicket(v.user_id, v.user_name, DateTime.Now, DateTime.Now.AddMinutes(20), /* expiry */ false, Roles, "/");
                        string encrypted = FormsAuthentication.Encrypt(authTicket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);

                        Debug.WriteLine(Roles);

                        if (Url.IsLocalUrl(ReturnURL))
                        {
                            return Redirect(ReturnURL);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        message = "Password or username is Invalid";
                    }
                }
                else
                {
                    message = "Password or username is Invalid";
                }
            }

            ViewBag.Message = message;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        [Authorize(Roles = "admin")]
        //USER DETAILS  // TODO: Restrict Access to Admin
        public ActionResult UserDetails(int id = 0)
        {
            ViewBag.userID = id;
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult GetData(int id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                mP.Configuration.ProxyCreationEnabled = false;
                var query = from user in mP.users
                            join role in mP.roles on user.role_id equals role.role_id
                            join region in mP.regions on user.region_id equals region.region_id
                            join u in mP.users on user.supervisor equals u.user_id into us from u in us.DefaultIfEmpty()
                            select new
                            {
                                user,
                                user_role = role.role_name,
                                supervisor = u.user_name,
                                region = region.region_name
                            };
                if (id != 0)
                {
                    query = query.Where(x => x.user.user_id == id);
                }
                return Json(new { data = query.AsNoTracking().ToList() }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "admin")]
        #region Add new user
        [HttpGet]
        public ActionResult AddUser()
        {
            using (mmpEntities mP = new mmpEntities())
            {
                ViewBag.roles = mP.roles.ToList<role>();
                ViewBag.regions = mP.regions.ToList<region>();
                ViewBag.supervisors = mP.users.Where(a => a.role_id == mP.roles.FirstOrDefault(x => x.role_name == "supervisor").role_id).ToList<user>();
                return View(new AddUser());
            }

        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(AddUser newUser)
        {
            if (ModelState.IsValid)
            {
                #region User Already Exists
                var isUserExists = isUserExist(newUser.user_name);
                if (isUserExists)
                {
                    ModelState.AddModelError("UserExist", "User already exists");
                    return View(newUser);
                }
                #endregion

                #region Email Already exists
                var isEmailExists = IsEmailExist(newUser.user_email);
                if (isEmailExists)
                {
                    ModelState.AddModelError("EmailExist", "Email already exists");
                    return View(newUser);
                }
                #endregion

                #region Password Hashing
                newUser.user_password = Crypto.Hash(newUser.user_password);
                newUser.confirmPassword = Crypto.Hash(newUser.confirmPassword);
                #endregion

                user user = new user()
                {
                    role_id = newUser.role_id,
                    user_name = newUser.user_name,
                    user_email = newUser.user_email,
                    user_password = newUser.user_password,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now,
                    supervisor = newUser.supervisor,
                    region_id = newUser.region_id

                };

                #region Save to Database 
                using (mmpEntities mP = new mmpEntities())
                {
                    mP.users.Add(user);
                    mP.SaveChanges();
                }
                #endregion

                return Json(new { success = true, message = "Saved Successfully" });
            }
            else
            {
                return Json(new { success = true, message = "Invalid Request" });
            }

        }
        #endregion

        [Authorize(Roles = "admin")]
        #region EDIT USER
        [HttpGet]
        public ActionResult EditUser(int id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                ViewBag.roles = mP.roles.ToList<role>();
                ViewBag.regions = mP.regions.ToList<region>();
                ViewBag.supervisors = mP.users.Where(a => a.role_id == mP.roles.FirstOrDefault(x => x.role_name == "supervisor" & a.user_id != id).role_id).ToList<user>();
                if (id != 0)
                {
                    user u = mP.users.Where(x => x.user_id == id).FirstOrDefault<user>();
                    EditUser editUser = new EditUser()
                    {
                        user_id = u.user_id,
                        user_name = u.user_name,
                        role_id = u.role_id,
                        user_email = u.user_email,
                        supervisor = u.supervisor,
                        region_id = u.region_id
                    };
                    return View(editUser);
                }
                else
                {
                    return View();
                }
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(EditUser editUser)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                ViewBag.roles = mP.roles.ToList<role>();
                ViewBag.regions = mP.regions.ToList<region>();
                ViewBag.supervisors = mP.users.Where(a => a.user_id == mP.roles.FirstOrDefault(x => x.role_name == "supervisor").role_id & a.user_id != editUser.user_id).ToList<user>();

                if (ModelState.IsValid)
                {
                    #region User Already Exists
                    var isUserExists = isUserExist(editUser.user_name, editUser.user_id);
                    if (isUserExists)
                    {
                        ModelState.AddModelError("UserExist", "Username already exists");
                        return View(editUser);
                    }
                    #endregion

                    #region Email Already Exists
                    var isEmailExists = IsEmailExist(editUser.user_email, editUser.user_id);
                    if (isEmailExists)
                    {
                        ModelState.AddModelError("EmailExists", "Email already exists");
                        return View(editUser);
                    }
                    #endregion

                    user user = mP.users.Where(x => x.user_id == editUser.user_id).First();
                    user.user_name = editUser.user_name;
                    user.role_id = editUser.role_id;
                    user.user_email = editUser.user_email;
                    user.supervisor = editUser.supervisor;
                    user.region_id = editUser.region_id;
                    mP.Entry(user).State = EntityState.Modified;
                    mP.SaveChanges();

                    return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                }
            }

            return View(editUser);
        }
        #endregion

        [Authorize(Roles = "admin")]
        #region Change Password
        [HttpGet]
        public ActionResult ChangePassword(int id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                if (id != 0)
                {
                    user v = mP.users.Where(x => x.user_id == id).FirstOrDefault<user>();
                    ChangePasswordVM userpassVM = new ChangePasswordVM();
                    userpassVM.user_id = v.user_id;
                    return View(userpassVM);
                }
                else
                {
                    return View();
                }
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordVM userpassVM)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                user user = mP.users.Where(x => x.user_id == userpassVM.user_id).FirstOrDefault();
                #region Password Hashing
                user.user_password = Crypto.Hash(userpassVM.password);
                #endregion
                mP.SaveChanges();

                return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion


        [Authorize(Roles = "admin")]
        #region Delete User
        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                user userDetails = mP.users.Where(x => x.user_id == id).FirstOrDefault<user>();
                mP.users.Remove(userDetails);
                mP.SaveChanges();
                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion




















        //Non Action
        [NonAction]
        public bool isUserExist(string userName, int user_id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                var v = mP.users.Where(a => a.user_name == userName && a.user_id != user_id).FirstOrDefault();
                return v != null;
            }
        }

        [NonAction]
        public bool IsEmailExist(string email, int user_id = 0)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                var v = mP.users.Where(a => a.user_email == email && a.user_id != user_id).FirstOrDefault();
                //return v == null ? false : true;
                return v != null;
            }
        }

        [NonAction]
        public void SendEmail(string email)
        {

            var URL = "/User/";
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, URL);

            var fromEmail = new MailAddress("bilal@gmail.com", "MEW");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "********"; // Replace with Actual Password
            String subject = "Your account is successfully created.";

            String body = "<br></br>Your account of mew was successfully created. Click on the link below to get redirected to the website<br></br>" +
                "<a href='" + link + "'>" + link + "</a>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            }) smtp.Send(message);
        }
    }
}