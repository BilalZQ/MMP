using MMP.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Security;


namespace MMP.Generic_Functions
{
    // Login AuthTicket
    // Global.asax  -> GenericPrincipal
    public class UserID_RoleID
    {
        public static int getUserID()
        {
           
            var employeeID = System.Web.HttpContext.Current.User.Identity.Name;
            using (mmpEntities mE = new mmpEntities())
            {
                user user = mE.users.Where(x => x.employee_id == employeeID).FirstOrDefault<user>();
                return user == null ? 0 : user.user_id;
            }
        }

        public static string getRole(string employeeID)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                //USE SUB QUERY
                user user = mP.users.Where(x => x.employee_id == employeeID).FirstOrDefault<user>();
                role role = mP.roles.Where(x => x.role_id == user.role_id).FirstOrDefault<role>();
                return role.role_name;                
            }
        }

        public static string getUserName()
        {
            var employeeID = System.Web.HttpContext.Current.User.Identity.Name;
            using (mmpEntities mE = new mmpEntities())
            {
                user user = mE.users.Where(x => x.employee_id == employeeID).FirstOrDefault<user>();
                return user == null ? "" : user.user_name;
            }
        }
    }
}