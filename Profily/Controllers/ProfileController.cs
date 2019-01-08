using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Profily.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Profily.Controllers
{

    [Authorize]
    public class ProfileController : MainController
    {

        [AllowAnonymous]
        public ActionResult Search(String id)
        {
            String[] words = id.Split(' ');
            String word1 = words[0].ToLower();
            String word2 = " ";
            if (words.Length == 2)
            {
                word2 = words[1].ToLower();
            }
            List<ApplicationUser> Profiles = new List<ApplicationUser>();
            foreach (var user in applicationContext.Users)
            {
                if ((user.FirstName.ToLower().Contains(word1) || user.LastName.ToLower().Contains(word2)
                || user.FirstName.ToLower().Contains(word2) || user.LastName.ToLower().Contains(word1)) && (!user.Profile.IsPrivate || User.IsInRole("Administrator") || Relation(user.Id) == "Friend"))
                {
                    Profiles.Add(user);
                }
            }
            ViewBag.Profiles = Profiles;
            ViewBag.Query = id;
            return View();
        }



        // GET: Profile
        [AllowAnonymous]
        public ActionResult Show(String id)
        {
            if (id == "" || id == null)
            {
                id = User.Identity.GetUserId();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Account");
                }
            }

            var user = applicationContext.Users.Find(id);
            ViewBag.User = user;
            var album = applicationContext.Albums.SqlQuery("Select * from Albums where ProfileId=@pId And IsDefault = 'True'", new SqlParameter("pId", user.Id)).FirstOrDefault();
            var albums = applicationContext.Albums.SqlQuery("Select * from Albums where ProfileId=@pId And IsDefault = 'False'", new SqlParameter("pId", user.Id)).ToArray();
            var photos = from photo in album.Photos select photo;
            ViewBag.ProfilePhoto = "no-photo.png";
            ViewBag.BackgroundPhoto = "default.jpg";
            ViewBag.Owner = IsOwner(id);
            ViewBag.CurrentUser = User.Identity.GetUserId();
            ViewBag.Albums = albums;
            ViewBag.Friend = false;
            ViewBag.Pending = false;
            ViewBag.Receiver = false;

            if (Relation(id) == "Friend")
            {
                ViewBag.Friend = true;
            }
            else if (Relation(id) == "Pending")
            {
                ViewBag.Pending = true;

            }
            else if (Relation(id) == "Receiver")
            {
                ViewBag.Receiver = true;
            }

            if (!ViewBag.Owner && (user.Profile.IsPrivate && !ViewBag.Friend))
            {
                return Content("No Access!");
            }
            /*
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
                            ViewBag.Friend = true;
                        }
                        else
                        {
                            ViewBag.Pending = true;
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
                            ViewBag.Friend = true;

                        }
                        else
                        {
                            ViewBag.Receiver = true;
                        }
                    }
                }
            }*/

            foreach (var photo in photos)
            {
                if (photo.ProfilePhoto)
                {
                    ViewBag.ProfilePhoto = Path.GetFileNameWithoutExtension(photo.Location) + "200x200" + Path.GetExtension(photo.Location);

                }
                if (photo.BackgroundPhoto)
                {
                    ViewBag.BackgroundPhoto = photo.Location;
                }
            }

            return View();
        }
        [HttpPost]
        public ActionResult ChangeBackground(String Id, HttpPostedFileBase backgroundPhoto)
        {
            //return Content(Id);
            var user = applicationContext.Users.Find(Id);

            if (!IsOwner(Id) && !User.IsInRole("Administrator"))
            {
                return Content("No access!");
            }
            if (backgroundPhoto != null && backgroundPhoto.ContentLength > 0)
            {
                if (IsImage(backgroundPhoto))
                {
                    string NewFileName = Guid.NewGuid().ToString("N") + Path.GetExtension(backgroundPhoto.FileName);
                    string path = Path.Combine(Server.MapPath("~/images"), NewFileName);
                    backgroundPhoto.SaveAs(path);
                    var album = applicationContext.Albums.SqlQuery("Select * from Albums where ProfileId=@pId And IsDefault = 'True'", new SqlParameter("pId", user.Id)).FirstOrDefault();
                    var profile = applicationContext.Users.Find(Id).Profile;
                    foreach (var ph in album.Photos)
                    {
                        if (ph.BackgroundPhoto)
                        {
                            applicationContext.Photos.Remove(ph);
                            break;
                        }
                    }
                    Photo photo = new Photo();
                    photo.Location = NewFileName;
                    photo.ProfilePhoto = false;
                    photo.BackgroundPhoto = true;
                    photo.AlbumId = album.Id;
                    //applicationContext.Profiles.Find(profile.UserId).Photos.Add(photo);
                    applicationContext.Photos.Add(photo);
                    applicationContext.SaveChanges();

                    return RedirectToAction("Show", new { id = profile.UserId });
                }
                else
                {
                    return Content("File is not an image!");
                }
            }
            return Content("No image uploaded!");
        }
        public ActionResult ChangeProfilePhoto(String Id, HttpPostedFileBase profilePhoto)
        {
            if (!IsOwner(Id) && !User.IsInRole("Administrator"))
            {
                return Content("No Access!");
            }
            if (profilePhoto != null && profilePhoto.ContentLength > 0)
            {
                if (IsImage(profilePhoto))
                {
                    string NewFileName = Guid.NewGuid().ToString("N") + Path.GetExtension(profilePhoto.FileName);
                    string path = Path.Combine(Server.MapPath("~/images"), NewFileName);
                    profilePhoto.SaveAs(path);
                    //create thumb
                    Image image = Image.FromFile(path);
                    Image thumb = image.GetThumbnailImage(200, 200, () => false, IntPtr.Zero);
                    thumb.Save(Path.Combine(Server.MapPath("~/images"), Path.GetFileNameWithoutExtension(path) + "200x200" + Path.GetExtension(path)));

                    var profile = applicationContext.Users.Find(Id).Profile;
                    var album = applicationContext.Albums.SqlQuery("Select * from Albums where ProfileId=@pId And IsDefault = 'True'", new SqlParameter("pId", Id)).FirstOrDefault();

                    foreach (var ph in album.Photos)
                    {
                        if (ph.ProfilePhoto)
                        {
                            applicationContext.Photos.Remove(ph);
                            break;
                        }
                    }
                    Photo photo = new Photo();
                    photo.Location = NewFileName;
                    photo.ProfilePhoto = true;
                    photo.BackgroundPhoto = false;
                    photo.AlbumId = album.Id;
                    //applicationContext.Profiles.Find(profile.UserId).Photos.Add(photo);
                    applicationContext.Photos.Add(photo);
                    applicationContext.SaveChanges();

                    return RedirectToAction("Show", new { id = profile.UserId });
                }
                else
                {
                    return Content("File is not an image!");
                }
            }
            else
            {
                return Content("No image uploaded!");
            }
        }

        public ActionResult MakeAdmin(String Id)
        {
            var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            UserManager.AddToRole(Id, "Administrator");
            return RedirectToAction("Show", new { id = Id });
        }

        [HttpGet]
        public ActionResult Edit(String Id)
        {

            if (!IsOwner(Id) && !User.IsInRole("Administrator"))
            {
                return Content("No Access");
            }
            if (User.IsInRole("Administrator"))
                ViewBag.isAdmin = true;
            else
                ViewBag.isAdmin = false;

            var profile = applicationContext.Profiles.Find(Id);

            ViewBag.Profile = profile;

            return View();

        }
        [HttpPut]
        public ActionResult Edit(String FirstName, String LastName, Profile requestProfile)
        {
            try
            {
                Profile profile = applicationContext.Profiles.Find(requestProfile.UserId);
                if (TryUpdateModel(profile))
                {

                    profile.ApplicationUser.FirstName = FirstName;
                    profile.ApplicationUser.LastName = LastName;
                    profile.Work = requestProfile.Work;
                    profile.Birthday = requestProfile.Birthday;
                    profile.Description = requestProfile.Description;
                    profile.School = requestProfile.School;
                    applicationContext.SaveChanges();

                }

                return RedirectToAction("Show", new { id = requestProfile.UserId });

            }
            catch (Exception e)
            {
                return View();
            }
        }
        public ActionResult SetPrivate(String id)
        {
            Profile profile = applicationContext.Profiles.Find(id);
            profile.IsPrivate = true;
            applicationContext.SaveChanges();

            return RedirectToAction("Show", new { id = id });
        }
        public ActionResult SetPublic(String id)
        {
            Profile profile = applicationContext.Profiles.Find(id);
            profile.IsPrivate = false;
            applicationContext.SaveChanges();

            return RedirectToAction("Show", new { id = id });
        }


        [HttpDelete]
        public ActionResult Delete(String id)
        {
            ApplicationUser applicationUser = applicationContext.Users.Find(id);
            if (!IsOwner(applicationUser.Profile.UserId))
            {
                return Content("No Access");
            }
            foreach (var album in applicationUser.Profile.Albums)
            {
                foreach (var photo in album.Photos)
                {
                    applicationContext.Comments.RemoveRange(photo.Comments);
                    applicationContext.SaveChanges();

                }
                applicationContext.Photos.RemoveRange(album.Photos);
                applicationContext.SaveChanges();
            }
            applicationContext.Albums.RemoveRange(applicationUser.Profile.Albums);
            foreach (var comment in applicationUser.Profile.Comments)
            {
                applicationContext.Comments.Remove(comment);
                applicationContext.SaveChanges();
            }
            applicationContext.Profiles.Remove(applicationUser.Profile);
            applicationContext.SaveChanges();
            applicationContext.Users.Remove(applicationUser);
            applicationContext.SaveChanges();
            applicationContext.Database.ExecuteSqlCommand("DELETE FROM FriendRequests WHERE ReceiverId = @id OR SenderId = @id", new SqlParameter("id", id));
            var AuthenticationManager = HttpContext.GetOwinContext().Authentication;
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");

        }

    }


}