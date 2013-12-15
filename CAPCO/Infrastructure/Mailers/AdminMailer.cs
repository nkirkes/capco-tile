using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Configuration;
using CAPCO.Infrastructure.Domain;

using Mvc.Mailer;

namespace CAPCO.Infrastructure.Mailers
{
    public interface IAdminMailer
    {
        MailMessage AccountRequest(AccountRequest request);
        MailMessage ContactRequest(ContactRequest request);
        MailMessage NewAccountNotification(ApplicationUser user);
    }

    public class AdminMailer : MailerBase, IAdminMailer     
	{
		public AdminMailer() : base()
		{
			MasterName="_Layout";
		}

		public virtual MailMessage AccountRequest(AccountRequest request)
		{
			var mailMessage = new MailMessage{Subject = "CAPCO Account Request"};
            mailMessage.IsBodyHtml = true;
            ViewBag.Title = "CAPCO Account Request";
            ViewBag.AccountRequest = request;
            mailMessage.To.Add(ConfigurationManager.AppSettings["AccountRequestRecipient"]);
			PopulateBody(mailMessage, viewName: "AccountRequest");
            return mailMessage;
		}

        public virtual MailMessage NewAccountNotification(ApplicationUser user)
        {
            var mailMessage = new MailMessage { Subject = "CAPCO New Profile Notification" };
            mailMessage.IsBodyHtml = true;
            ViewBag.Title = "CAPCO New Profile Notification";
            ViewBag.NewProfile = user;
            mailMessage.To.Add(ConfigurationManager.AppSettings["NewAccountActivationRecipient"]);
            PopulateBody(mailMessage, viewName: "NewAccountNotification");
            return mailMessage;
        }
		
		public virtual MailMessage ContactRequest(ContactRequest request)
		{
            var mailMessage = new MailMessage { Subject = "CAPCO Contact Request" };
            mailMessage.IsBodyHtml = true;
            ViewBag.Title = "CAPCO Contact Request";
            ViewBag.ContactRequest = request;
            mailMessage.To.Add(ConfigurationManager.AppSettings["WebContactRecipient"]);
            PopulateBody(mailMessage, viewName: "ContactRequest");
            return mailMessage;
		}

		
	}
}