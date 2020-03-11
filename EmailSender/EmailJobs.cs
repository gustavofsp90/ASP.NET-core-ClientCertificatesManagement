using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using CertificatesManager.Database;
using CertificatesManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CertificatesManager.EmailSender
{
	public class EmailJobs : IEmailJobs
	{
		private readonly CertificatesManagerDBContext _context;
		private IConfiguration _configuration;
		public EmailJobs(CertificatesManagerDBContext context, IConfiguration Configuration)
		{
			_context = context;
			_configuration = Configuration;
		}
		public async Task SendEmail()
		{
			var settings = await _context.Settings
				.Include(x => x.Group)
				.ToListAsync();
			List<Email> sentEmails = new List<Email>();
			foreach (var setting in settings)
			{
				List<Email> listEmails = GetCertificatesAboutToExpireInDays(setting, sentEmails);
				Send(listEmails, setting);
			}
		}

		private void Send(List<Email> emails, Settings settings)
		{

			foreach (var email in emails)
			{

				DateTime date = DateTime.Now;
				bool hasExpired = (email.Certificate.To - date).TotalDays <= 0;

				//LOGS EMAIL
				//using (StreamWriter writer = new StreamWriter("C:\\Users\\desouza_r\\Desktop\\email.txt", true))
				//{
				//	string testText = "User: " + email.User.Name + ". ( " + email.User.Email + ") Has Expired:" + hasExpired + " Certificate name: " + email.Certificate.Name + email.Certificate.Extension + Environment.NewLine;
				//	testText += "Valid From: " + email.Certificate.From.ToShortDateString() + " to " + email.Certificate.To.ToShortDateString() + Environment.NewLine;
				//	testText += "User group: " + settings.Group.Name + Environment.NewLine;
				//	testText += "ID: " + email.Certificate.Id + Environment.NewLine;
				//	writer.WriteLine(testText);
				//}

				string emailText = emailText = CreateEmailBody(settings, email, hasExpired);
				string subject = settings.EmailSubject.Replace("{certificate}", email.Certificate.Name + email.Certificate.Extension + (hasExpired ? " has expired" : " is due to expire"));
				SMTPSendEmail(email.User.Email, emailText, subject, null);
			}
		}

		public void SMTPSendEmail(string email, string bodyText, string subject, Attachment attachment)
		{
			try
			{
				SmtpClient client = new SmtpClient(_configuration.GetSection("AppSettings")["SMTPClient"]);
				client.Port = Convert.ToInt32(_configuration.GetSection("AppSettings")["SMTPPort"]);
				client.UseDefaultCredentials = true;
				MailMessage mailMessage = new MailMessage();
				mailMessage.From = new MailAddress(_configuration.GetSection("AppSettings")["SMTPEmail"]);
				mailMessage.To.Add(email);
				mailMessage.Body = bodyText;
				mailMessage.IsBodyHtml = true;
				mailMessage.Subject = subject;
				if (attachment != null)
				{
					mailMessage.Attachments.Add(attachment);
				}
				client.Send(mailMessage);
			}
			catch (SmtpException ex)
			{
				throw ex;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private string CreateEmailBody(Settings settings, Email email, bool hasExpired)
		{
			string htmlText = System.IO.File.ReadAllText(_configuration.GetSection("AppSettings")["EmailHTMLLocation"]);
			htmlText = htmlText.Replace("{user}", email.User.Name)
			.Replace("{certificateName}", email.Certificate.Name + email.Certificate.Extension)
			.Replace("{toDate}", email.Certificate.To.ToShortDateString())
			.Replace("{subjectName}", email.Certificate.SubjectName)
			.Replace("{serialNumber}", email.Certificate.SerialNumber)
			.Replace("{issuer}", email.Certificate.Issuer)
			.Replace("{environment}", email.Certificate.Environment.ToString())
			.Replace("{expiryDate}", email.Certificate.From.ToShortDateString() + " to " + email.Certificate.To.ToShortDateString())
			.Replace("{servers}", email.Certificate.Servers.Count > 0 ? String.Join(", ", email.Certificate.Servers.Select(x => x.Server.Name)) : "")
			.Replace("{applications}", email.Certificate.Applications.Count > 0 ? String.Join(", ", email.Certificate.Applications.Select(x => x.Application.Name)) : "")
			.Replace("{group}", settings.Group.Name)
			.Replace("{currentStatus1}", hasExpired ? "has expired" : "is due to expire")
			.Replace("{currentStatus2}", hasExpired ? "It has been expired for <b>" + Convert.ToInt32((email.Certificate.To - DateTime.Now).TotalDays).ToString().Replace("-", "") + "</b> days." : "<b>" + Convert.ToInt32((email.Certificate.To - DateTime.Now).TotalDays).ToString() + "</b> days from now.")
			.Replace("{days}", (email.Certificate.To - DateTime.Now).TotalDays.ToString())
			.Replace("{link}", _configuration.GetSection("AppSettings")["CertificatesURL"] + email.Certificate.Id.ToString());

			return htmlText;
		}

		private List<Email> GetCertificatesAboutToExpireInDays(Settings settings, List<Email> sentEmails)
		{
			List<Email> listEmails = new List<Email>();
			DateTime date = DateTime.Now;
			var certificatesExpired = _context.Certificate
				.Include(x => x.Groups)
					.ThenInclude(y => y.Group)
						.ThenInclude(z => z.Users)
							.ThenInclude(a => a.User)
				.Include(x => x.Servers)
					.ThenInclude(y => y.Server)
				.Include(x => x.Applications)
				.ThenInclude(y => y.Application)
				.Where(x => ((x.To - date).TotalDays <= settings.DaysBeforeEmail) && x.Groups.Any(b => b.GroupId == settings.Group.Id) && x.Active)
				.ToList();
			foreach (var cert in certificatesExpired)
			{
				foreach (var certificateGroup in cert.Groups.Where(x => x.GroupId == settings.Group.Id))
				{
					foreach (var user in certificateGroup.Group.Users)
					{
						Email email = new Email { User = user.User, Certificate = cert };

						if (!sentEmails.Any(x => x.Certificate.Id == cert.Id && x.User.Id == user.UserId))
						{
							listEmails.Add(email);
							sentEmails.Add(email);
						}
					}
				}
			}
			return listEmails;
		}

	}
}
