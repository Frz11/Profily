using Microsoft.AspNet.Identity;
using Profily.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Profily.Controllers
{
    [Authorize]
    public class AlbumController : MainController
    {

        public bool IsCommentOwner(int Id)
        {
            var comment = applicationContext.Comments.Find(Id);
            if(User.Identity.GetUserId() == comment.ProfileId)
            {
                return true;
            }
            return true;
        }
        public ActionResult AcceptComment(String id)
        {
            Comment comment = applicationContext.Comments.Find(Convert.ToInt32(id));
            if (TryUpdateModel(comment))
            {
                comment.Accepted = true;
                applicationContext.SaveChanges();
                return RedirectToAction("ShowPhoto", new { id = comment.PhotoId });
            }
            return Content("Eror!");
        }
        public ActionResult RemoveComment(string id)
        {

            Comment comment = applicationContext.Comments.Find(Convert.ToInt32(id));
            if (!IsCommentOwner(comment.Id) && !IsOwner(comment.Photo.Album.ProfileId))
            {
                return Content("No access!");
            }
            if (TryUpdateModel(comment))
            {
                comment.Accepted = false;
                applicationContext.SaveChanges();
                return RedirectToAction("ShowPhoto", new { id = comment.PhotoId });
            }
            return Content("Error!");
        }
        [HttpPost]
        public ActionResult AddComment(Comment comment,String PhotoId)
        {
            int photoId = Convert.ToInt32(PhotoId);
            comment.ProfileId = User.Identity.GetUserId();
            comment.PhotoId = photoId;
            comment.Accepted = false;
            
            applicationContext.Comments.Add(comment);
            applicationContext.SaveChanges();

            return RedirectToAction("ShowPhoto", new { id = PhotoId });
        }

        [AllowAnonymous]
        public ActionResult ShowPhoto(string id)
        {
            int Id = Convert.ToInt32(id);
            var photo = applicationContext.Photos.Find(Id);
            ViewBag.Owner = IsOwner(photo.Album.Profile.UserId);
            if(!ViewBag.Owner && photo.Album.Profile.IsPrivate && Relation(photo.Album.Profile.UserId) != "Friend")
            {
                return Content("No Access!");
            }
            ViewBag.Photo = photo;
            ViewBag.Uid = User.Identity.GetUserId();
            ViewBag.Comments = from cm in applicationContext.Comments where cm.PhotoId == Id where cm.Accepted == true select cm;
            ViewBag.NComments = from cm in applicationContext.Comments where cm.PhotoId == Id where cm.Accepted == false select cm;
            return View();
        }

        public ActionResult RemovePhoto(string id)
        {
            int Id = Convert.ToInt32(id);
            var photo = applicationContext.Photos.Find(Id);
            if (!IsOwner(photo.Album.Profile.UserId))
            {
                return Content("No Access");
            }
            applicationContext.Comments.RemoveRange(photo.Comments);
            applicationContext.SaveChanges();
            applicationContext.Photos.Remove(photo);
            applicationContext.SaveChanges();
            return RedirectToAction("Show", new { id = photo.AlbumId });
            
        }

        public bool SavePhotos(HttpPostedFileBase[] Photos, int AlbumId)
        {
            foreach (var Photo in Photos)
            {
                if (Photo == null || Photo.ContentLength <= 0)
                {
                    return false;
                }
                if (!IsImage(Photo))
                {
                    return false;
                }
            }
            foreach (var Photo in Photos)
            {
                String NewFileName = Guid.NewGuid().ToString("N") + Path.GetExtension(Photo.FileName);
                string path = Path.Combine(Server.MapPath("~/images"), NewFileName);
                Photo.SaveAs(path);
                Image image = Image.FromFile(path);
                Image thumb = image.GetThumbnailImage(250, 250, () => false, IntPtr.Zero);
                thumb.Save(Path.Combine(Server.MapPath("~/images"), Path.GetFileNameWithoutExtension(path) + "250x250" + Path.GetExtension(path)));
                Photo NewPhoto = new Photo();
                NewPhoto.Location = NewFileName;
                NewPhoto.ProfilePhoto = false;
                NewPhoto.BackgroundPhoto = false;
                NewPhoto.AlbumId = AlbumId;
                applicationContext.Photos.Add(NewPhoto);
                applicationContext.SaveChanges();

            }
            return true;
        }


        public ActionResult AddPhoto(HttpPostedFileBase[] Photos, int id)
        {
            var album = applicationContext.Albums.Find(id);
            if (!IsOwner(album.ProfileId))
            {
                return Content("No access!");
            }
            SavePhotos(Photos, id);
            return RedirectToAction("Show", new { id = id });
        }
        [AllowAnonymous]
        // GET: Album
        public ActionResult Show(string id)
        {
            //return Content(id);
            var album = applicationContext.Albums.Find(Convert.ToInt32(id));
            ViewBag.Owner = IsOwner(album.ProfileId);
            if(!ViewBag.Owner && album.Profile.IsPrivate && Relation(album.ProfileId) != "Friend")
            {
                return Content("No Access!");
            }
            ViewBag.Album = album;
 
            return View();
        }


        [HttpGet]
        public ActionResult Create(String Id)
        {
            if (!IsOwner(Id))
            {
                return Content("No Access");
            }
            Profile profile = applicationContext.Profiles.Find(Id);
            ViewBag.Profile = profile;
           
            return View();
        }
        [HttpDelete]
        public ActionResult Delete(int id)
        {

            Album album = applicationContext.Albums.Find(id);
            if (!IsOwner(album.Profile.UserId))
            {
                return Content("No Access");
            }
            foreach (var photo in album.Photos)
            {
                applicationContext.Comments.RemoveRange(photo.Comments);
                applicationContext.SaveChanges();
            }
            applicationContext.Photos.RemoveRange(album.Photos);
            applicationContext.SaveChanges();
            applicationContext.Albums.Remove(album);
            applicationContext.SaveChanges();
            return RedirectToAction("Show", "Profile", new { id = album.ProfileId });
        }
        [HttpPost]
        public ActionResult Create(Album album,HttpPostedFileBase[] Photos,String ProfileId)
        {
            if (!IsOwner(ProfileId))
            {
                return Content("No Access");
            }
            try
            {
        
              
                album.ProfileId = ProfileId;
                album.IsDefault = false;
                applicationContext.Albums.Add(album);
                applicationContext.SaveChanges();
                SavePhotos(Photos, album.Id);
                return RedirectToAction("Show", "Profile", new { id = album.ProfileId });
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }
    }
}