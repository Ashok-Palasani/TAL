using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UnitWorksCCS;

namespace UnitWorksCCS.Controllers
{
    public class ChangePasswordController : Controller
    {
        // GET: ChangePassword

        public i_facility_talEntities db = new i_facility_talEntities();
        public ActionResult Index()
        {
            int UserID = Convert.ToInt32(Session["UserID"]);
            var UserNamePassword = db.tblusers.Where(m => m.IsDeleted == 0 && m.UserID == UserID).Select(m=> new { m.UserName, m.Password }).FirstOrDefault();
            string UserName = UserNamePassword.UserName;
            string Password = UserNamePassword.Password;
            if (UserName!="" && UserName!=null)
            {
                ViewBag.UserName = UserName;
                //ViewBag.Password = Password;
               // ViewBag.UserName = "reports2";
            }
            //ViewBag.UserName = "reports2";
            return View();
        }


        public string ValidatePassword(string OLDPwd)
        {
            string res = "FAIL";
            int UserID = Convert.ToInt32(Session["UserID"]);
            var UserPWDValidate = db.tblusers.Where(m => m.IsDeleted == 0 && m.UserID == UserID && m.Password==OLDPwd).FirstOrDefault();
            if(UserPWDValidate!=null)
            {
                res = "Success";
            }
            return res;
        }


        public string UpdateNewPassword(string NEWPwd)
        {            
            int UserID = Convert.ToInt32(Session["UserID"]);
            string res = "FAIL";
            var UserDetUpdate = db.tblusers.Where(m => m.IsDeleted == 0 && m.UserID == UserID).FirstOrDefault();
            int RoleID = UserDetUpdate.PrimaryRole;
            if (RoleID!=6)
            {
                string OldPwd = UserDetUpdate.Password;
                if (OldPwd != NEWPwd)
                {
                    UserDetUpdate.Password = NEWPwd;
                    db.Entry(UserDetUpdate).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    res = "Success";
                }
                else
                {
                    res = "Same";
                }
            }
            return res;
        }
    }
}