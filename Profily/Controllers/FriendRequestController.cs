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
    public class FriendRequestController : MainController
    {
        public ActionResult Friends()
        {
            List<ApplicationUser> Users = new List<ApplicationUser>();
            var Friends = applicationContext.FriendRequests.SqlQuery("SELECT * FROM FriendRequests WHERE ReceiverId = @rId AND Accepted = 'True'", new SqlParameter("rId", User.Identity.GetUserId())).ToArray();Friends = applicationContext.FriendRequests.SqlQuery("SELECT * FROM FriendRequests WHERE ReceiverId = @rId AND Accepted = 'True'", new SqlParameter("rId", User.Identity.GetUserId())).ToArray();
            foreach(var friend in Friends)
            {
                Users.Add(applicationContext.Users.Find(friend.SenderId));
            }
            Friends = applicationContext.FriendRequests.SqlQuery("SELECT * FROM FriendRequests WHERE SenderId = @rId AND Accepted = 'True'", new SqlParameter("rId", User.Identity.GetUserId())).ToArray();
            foreach(var friend in Friends)
            {
                Users.Add(applicationContext.Users.Find(friend.ReceiverId));
            }
            ViewBag.Friends = Users;
            return View();
        }
        // GET: FriendRequest
        public ActionResult Index()
        {
            var Requests = applicationContext.FriendRequests.SqlQuery("SELECT * FROM FriendRequests WHERE ReceiverId = @rId AND Accepted = 'False'",new SqlParameter("rId",User.Identity.GetUserId())).ToArray();
            List<Profile> Profiles = new List<Profile>();
            foreach(var Request in Requests)
            {
                var profile = applicationContext.Profiles.Find(Request.SenderId);
                Profiles.Add(profile);
            }
            ViewBag.Profiles = Profiles;
            return View();
        }
        public ActionResult SendRequest(String id)
        {
            FriendRequest friendRequest = new FriendRequest();
            friendRequest.SenderId = User.Identity.GetUserId();
            friendRequest.ReceiverId = id;
            friendRequest.Accepted = false;

            applicationContext.FriendRequests.Add(friendRequest);
            applicationContext.SaveChanges();
            return RedirectToAction("Show","Profile" ,new { id = id });

        }
        public ActionResult AcceptRequest(String id)
        {
            FriendRequest friendRequest = applicationContext.FriendRequests.SqlQuery("SELECT * FROM FriendRequests" +
                  " WHERE SenderId = @rId AND ReceiverId = @sId",
                  new SqlParameter("sId", User.Identity.GetUserId()), new SqlParameter("rId", id)).FirstOrDefault();
            if(friendRequest == null)
            {
                return Content("Error!" + id);
            }
            
               
            friendRequest.Accepted = true;
            applicationContext.SaveChanges();
            return RedirectToAction("Show","Profile",new { id = id });
          
        }
        public ActionResult RemoveRequest(String id)
        {
            FriendRequest friendRequest = applicationContext.FriendRequests.SqlQuery("SELECT * FROM FriendRequests" +
                  " WHERE SenderId = @rId AND ReceiverId = @sId",
                  new SqlParameter("sId", User.Identity.GetUserId()), new SqlParameter("rId", id)).FirstOrDefault();
            if (friendRequest == null)
            {
                friendRequest = applicationContext.FriendRequests.SqlQuery("SELECT * FROM FriendRequests" +
                  " WHERE SenderId = @sId AND ReceiverId = @rId",
                  new SqlParameter("sId", User.Identity.GetUserId()), new SqlParameter("rId", id)).FirstOrDefault();
                if(friendRequest == null)
                {
                    return Content("Error!");
                }
            }

            applicationContext.FriendRequests.Remove(friendRequest);
            applicationContext.SaveChanges();
            return RedirectToAction("Show", "Profile");
        }
    }
}