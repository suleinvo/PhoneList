using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PhonesList.Models;
using Microsoft.AspNet.Identity;
using System.Net.Mail;
using System.Configuration;

namespace PhonesList.Controllers
{
    [Authorize]
    public class ContactsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
     
        private string userId;
        private ApplicationUser currentUser;

        public string UserId
        {
            get
            {
                userId = userId ?? User.Identity.GetUserId();
                return userId;
            }
        }
        
        public ApplicationUser CurrentUser
        {
            get
            {
                currentUser = currentUser ?? db.Users.Find(UserId);
                return currentUser;
            }
        }


        public ContactsController()
        {

        }

        public ActionResult Index()
        {
            try
            {
                return View(CurrentUser.Contacts);
            }
            catch
            {
                return RedirectToAction("Register", "Account");
            }
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Contact contact = CurrentUser.Contacts.FirstOrDefault(t=>t.Id == id);

            if (contact == null)
            {
                return HttpNotFound();
            }

            return View(contact);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Name,Phone,Email")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                CurrentUser.Contacts.Add(contact);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contact);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = CurrentUser.Contacts.FirstOrDefault(t=>t.Id == id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Name,Phone,Email")] Contact contact)
        {
            var oldContact = CurrentUser.Contacts.FirstOrDefault(t => t.Id == contact.Id);

            if (ModelState.IsValid && oldContact != null)
            {
                db.Entry(contact).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contact);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Contact contact = CurrentUser.Contacts.FirstOrDefault(t=> t.Id == id);

            if (contact == null)
            {
                return HttpNotFound();
            }

            return View(contact);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contact contact = CurrentUser.Contacts.FirstOrDefault(t=> t.Id == id);
            db.Contacts.Remove(contact);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult SendMessage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Contact contact = CurrentUser.Contacts.FirstOrDefault(t => t.Id == id);

            if (contact == null)
            {
                return HttpNotFound();
            }

            return View(new EmailSend() { Email = contact.Email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendMessage(EmailSend send)
        {
            try
            {
                sendMail(send.Email, send.Body);
                db.EmailSends.Add(send);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }

        private static void sendMail(string mailto, string message)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("fortestonlyapp@mail.ru");
                mail.To.Add(new MailAddress(mailto));
                mail.Subject = "Phone List";
                mail.Body = message;
                using (SmtpClient client = new SmtpClient())
                {
                    client.Host = ConfigurationManager.AppSettings["smtp"];
                    client.Port = 587;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["pass"]);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Send(mail);
                    mail.Dispose();
                }
            }
        }
    }
}
