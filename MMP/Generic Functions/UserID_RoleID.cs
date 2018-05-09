using MMP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMP.Generic_Functions
{
    // Login AuthTicket
    // Global.asax  -> GenericPrincipal
    public class UserID_RoleID
    {
        public static int getUserID()
        {
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            using (mmpEntities mE = new mmpEntities())
            {
                user user = mE.users.Where(x => x.user_name == userName).FirstOrDefault<user>();
                return user == null ? 0 : user.user_id;
            }
        }

        public static string getRole(string userName)
        {
            using (mmpEntities mP = new mmpEntities())
            {
                user user = mP.users.Where(x => x.user_name == userName).FirstOrDefault<user>();
                role role = mP.roles.Where(x => x.role_id == user.role_id).FirstOrDefault<role>();
                return role.role_name;                
            }
        }
    }
}