using Microsoft.AspNet.Identity;
using Profily.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        protected String Relation(String id)
        {
            if (Request.IsAuthenticated)
            {
                if (User.Identity.GetUserId() != id)
                {
                    var Request =
                        applicationContext.Database.SqlQuery<FriendRequest>("SELECT * FROM FriendRequests" +
                        " WHERE SenderId = @sId AND ReceiverId = @rId",
                        new SqlParameter("sId", User.Identity.GetUserId()), new SqlParameter("rId", id)).FirstOrDefault();



                    if (Request != null)
                    {
                        if (Request.Accepted)
                        {
                            return "Friend";
                        }
                        else
                        {
                            return "Pending";
                        }
                    }

                    Request =
                      applicationContext.Database.SqlQuery<FriendRequest>("SELECT * FROM FriendRequests" +
                      " WHERE SenderId = @rId AND ReceiverId = @sId",
                      new SqlParameter("sId", User.Identity.GetUserId()), new SqlParameter("rId", id)).FirstOrDefault();
                    if (Request != null)
                    {
                        if (Request.Accepted)
                        {
                            return "Friend";

                        }
                        else
                        {
                            return "Reciever";
                        }
                    }
                }
            }
            return null;
        }
       
    }
}