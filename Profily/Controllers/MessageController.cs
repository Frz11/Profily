using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Profily.Models;

namespace Profily.Controllers
{
    [Authorize]
    public class MessageController : MainController
    {

        public ActionResult Conversation(string id)
        {
            var Messages = applicationContext.Messages.
                SqlQuery("SELECT * FROM Messages WHERE (Receiver = @rId AND Sender = @sId)" +
                " OR (Receiver = @sId AND Sender = @rId) order by SentAt ASC", new SqlParameter("rId", User.Identity.GetUserId()), new SqlParameter("sId", id)).ToArray();
            var Friend = applicationContext.Users.Find(id);
            ViewBag.Messages = Messages;
            ViewBag.Friend = Friend;
            return View();
        }
        [HttpPost]
        public ActionResult Send(String id,String Text)
        {
            var Message = new Message();
            Message.Sender = User.Identity.GetUserId();
            Message.Receiver = id;
            Message.Text = Text;
            Message.SentAt = DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt");
            applicationContext.Messages.Add(Message);
            applicationContext.SaveChanges();
            return RedirectToAction("Conversation", new { id = id });

        }
        // GET: Message
        public ActionResult Index()
        {
            return View();
        }
    }
}