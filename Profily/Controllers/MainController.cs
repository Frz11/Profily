using Microsoft.AspNet.Identity;
using Profily.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Profily.Controllers
{
    public class MainController : Controller
    {
        // GET: Main
        protected ApplicationDbContext applicationContext = new ApplicationDbContext();


        protected bool IsImage(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" }; // add more if u like...

            // linq from Henrik Stenbæk
            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
        protected bool IsOwner(string Id)
        {
            if (Id == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                return true;
            }
            return false;
        }
       
    }
}