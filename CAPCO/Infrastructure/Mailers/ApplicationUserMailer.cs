using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using CAPCO.Infrastructure.Domain;
using Mvc.Mailer;

namespace CAPCO.Infrastructure.Mailers
{
    public interface IApplicationUserMailer
    {
        MailMessage Activation(ApplicationUser customer);
        MailMessage Welcome();
        MailMessage PasswordReset(string firstName,string username, string newPassword, string email);
        MailMessage NewProjectComment(ProjectComment comment, string recipientEmail);
    }

    public class ApplicationUserMailer : MailerBase, IApplicationUserMailer     
	{
        public ApplicationUserMailer() : base()
		{
			MasterName="_MailerLayout";
		}
        		
		public virtual MailMessage Activation(ApplicationUser customer)
		{
			var mailMessage = new MailMessage { Subject = "CAPCO Account Activation" };
            mailMessage.IsBodyHtml = true;
            ViewBag.Title = "CAPCO Account Activation";
            ViewBag.Name = customer.FirstName;
            ViewBag.ActivationKey = customer.ActivationKey;
            mailMessage.To.Add(customer.Email);
            PopulateBody(mailMessage, viewName: "Activation");
            return mailMessage;
		}
        		
		public virtual MailMessage Welcome()
		{
			var mailMessage = new MailMessage{Subject = "Welcome"};
			
			//mailMessage.To.Add("some-email@example.com");
			//ViewBag.Data = someObject;
			PopulateBody(mailMessage, viewName: "Welcome");

			return mailMessage;
		}
        		
		public virtual MailMessage PasswordReset(string firstName, string username, string newPassword, string email)
		{
            var mailMessage = new MailMessage { Subject = "CAPCO Password Reset" };
            mailMessage.IsBodyHtml = true;
            ViewBag.Title = "CAPCO Password Reset";
            ViewBag.Name = firstName;
		    ViewBag.Username = username;
            ViewBag.NewPassword = newPassword;
            mailMessage.To.Add(email);
            PopulateBody(mailMessage, viewName: "PasswordReset");
            return mailMessage;
		}

        public MailMessage NewProjectComment(ProjectComment comment, string recipientEmail)
        {
            var mailMessage = new MailMessage { Subject = "CAPCO Project Comment" };
            mailMessage.IsBodyHtml = true;
            ViewBag.Title = "CAPCO Project Comment";
            ViewBag.Comment = comment;

            mailMessage.To.Add(recipientEmail);
            
            PopulateBody(mailMessage, viewName: "NewProjectComment");
            return mailMessage;
        }

        public virtual MailMessage ProjectInvitation(ProjectInvitation invite)
        {
            var mailMessage = new MailMessage { Subject = "CAPCO Project Invitation" };
            mailMessage.IsBodyHtml = true;
            ViewBag.Title = "CAPCO Project Invitation";
            ViewBag.Invite = invite;
            
            mailMessage.To.Add(invite.Email);
            PopulateBody(mailMessage, viewName: "ProjectInvitation");
            return mailMessage;
        }
    }
}