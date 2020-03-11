using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CertificatesManager.EmailSender
{
	public interface IEmailJobs
	{
		Task SendEmail();
		void SMTPSendEmail(string email, string bodyText, string subject, Attachment attachment);
	}
}
