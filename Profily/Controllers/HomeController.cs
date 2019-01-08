using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Profily.Controllers
{
    public class HomeController : MainController
    {
        public ActionResult Index()
        {
            var Photo = from photo in applicationContext.Photos
                        where photo.Album.Profile.IsPrivate == false && photo.Album.IsDefault == false
                        orderby photo.Id descending
                        select photo;
            ViewBag.Photos = Photo;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}