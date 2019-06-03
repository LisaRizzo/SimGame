using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimGame.Models;

namespace SimGame.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Player account)
        {
            if (ModelState.IsValid)
            {
                using (SimGameEntities db = new SimGameEntities())
                {
                    db.Players.Add(account);
                    db.SaveChanges();
                }
                ModelState.Clear();
                ViewBag.Message = "Succesfully registered.";
            }
            return RedirectToAction("Login");
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Player user)
        {
            using (SimGameEntities db = new SimGameEntities())
            {
                var usr = db.Players.FirstOrDefault(u => u.username == user.username && u.password == user.password);
                if (usr != null)
                {
                    Session["UserID"] = usr.Id.ToString();
                    Session["Username"] = usr.username.ToString();
                    return RedirectToAction("Game","Home");
                }
                else
                {
                    ModelState.AddModelError("", "User name or password is wrong!");
                }
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            Session.RemoveAll();
            return RedirectToAction("Login");
        }
    }
}