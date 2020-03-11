using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CertificatesManager.Database;
using CertificatesManager.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography.X509Certificates;
using CertificatesManager.ViewModels;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http.Internal;
using CertificatesManager.Models.Enum;
using CertificatesManager.EmailSender;
using System.Net.Mail;
using System.Net.Mime;

namespace CertificatesManager.Controllers
{
	public class CertificatesController : Controller
	{
		private readonly CertificatesManagerDBContext _context;
		private readonly IMapper _mapper;
		private IConfiguration _configuration;
		private IEmailJobs _emailJobs;

		public CertificatesController(CertificatesManagerDBContext context, IMapper mapper, IConfiguration Configuration, IEmailJobs emailJobs)
		{
			_context = context;
			_mapper = mapper;
			_configuration = Configuration;
			_emailJobs = emailJobs;
		}

		// GET: Certificates
		public async Task<IActionResult> Index([FromQuery(Name = "filterBy")] string filterBy = "Actives")
		{
			var certificate = await _context.Certificate
				.Include(x => x.Applications)
					.ThenInclude(y => y.Application)
				.Include(x => x.Servers)
					.ThenInclude(y => y.Server)
				.Include(x => x.Groups)
					.ThenInclude(y => y.Group)
				.ToListAsync();
			Enum.TryParse(filterBy.ToUpper(), out Filter filter);
			int dueToExpireDays = Convert.ToInt32(_configuration.GetSection("AppSettings")["DueToExpireDays"]);

			switch (filter)
			{
				case Filter.ACTIVES:
					certificate = certificate.Where(x => x.Active).ToList();
					break;
				case Filter.ALL:
					break;
				case Filter.DUETOEXPIRE:
					certificate = certificate.Where(x => (x.To >= DateTime.Today) && (Convert.ToInt32((x.To - DateTime.Now).TotalDays) <= dueToExpireDays) && x.Active).ToList();
					break;
				case Filter.EXPIRED:
					certificate = certificate.Where(x => (x.To < DateTime.Today) && x.Active).ToList();
					break;
				case Filter.INACTIVES:
					certificate = certificate.Where(x => !x.Active).ToList();
					break;
				case Filter.VALIDS:
					certificate = certificate.Where(x => (Convert.ToInt32((x.To - DateTime.Now).TotalDays) > dueToExpireDays) && x.Active).ToList();
					break;
				default:
					break;

			}
			List<CertificateViewModel> viewModels = _mapper.Map<List<CertificateViewModel>>(certificate);
			return View(viewModels);
		}

		// GET: Certificates/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var certificate = await _context.Certificate
				.Include(x => x.Applications)
					.ThenInclude(y => y.Application)
					.Include(x => x.Servers)
					.ThenInclude(y => y.Server)
					.Include(x => x.Groups)
					.ThenInclude(y => y.Group)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (certificate == null)
			{
				return NotFound();
			}
			CertificateViewModel viewModel = _mapper.Map<CertificateViewModel>(certificate);
			return View(viewModel);
		}

		// GET: Certificates/Create
		public IActionResult Create()
		{
			var viewModel = new CertificateViewModel();
			LoadFieldsViewModel(viewModel);
			return View(viewModel);
		}

		// POST: Certificates/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(IFormFile file, [Bind("Id,File,Guid,Extension,Name,Password,GroupIds,ApplicationIds,ServerIds,SubjectName,SerialNumber,Issuer,Location,Purpose,Environment,From,To,InstallationLink,OtherInfo,Active")] CertificateViewModel certificateViewModel)
		{
			if (ModelState.IsValid)
			{
				var filePath = "";
				try
				{
					var outputFolder = _configuration.GetSection("AppSettings")["CertificatesPath"];

					// Extract file name from whatever was posted by browser
					var fileName = Guid.NewGuid().ToString();
					var fileExtension = System.IO.Path.GetExtension(file.FileName);
					filePath = System.IO.Path.GetFullPath(outputFolder + fileName + fileExtension);

					if (fileExtension != ".p12" && fileExtension != ".pfx" && fileExtension != ".cer")
						throw new Exception("Unsupported file");

					// If file with same name exists delete it
					if (System.IO.File.Exists(filePath))
						System.IO.File.Delete(filePath);

					// Create new local file and copy contents of uploaded file
					using (var localFile = System.IO.File.OpenWrite(filePath))
					using (var uploadedFile = file.OpenReadStream())
						uploadedFile.CopyTo(localFile);

					switch (fileExtension)
					{
						case ".p12":
						case ".pfx":
							X509Certificate2 certificatep12 = new X509Certificate2(@filePath, certificateViewModel.Password, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
							certificateViewModel.From = Convert.ToDateTime(certificatep12.GetEffectiveDateString());
							certificateViewModel.To = Convert.ToDateTime(certificatep12.GetExpirationDateString());
							certificateViewModel.SerialNumber = certificatep12.SerialNumber;
							certificateViewModel.SubjectName = certificatep12.Subject;
							certificateViewModel.Issuer = certificatep12.Issuer;
							break;
						case ".cer":
							// Load the certificate into an X509Certificate object.
							X509Certificate certificateCer = X509Certificate.CreateFromCertFile(filePath);
							certificateViewModel.From = Convert.ToDateTime(certificateCer.GetEffectiveDateString());
							certificateViewModel.To = Convert.ToDateTime(certificateCer.GetExpirationDateString());
							certificateViewModel.SerialNumber = certificateCer.GetSerialNumberString();
							certificateViewModel.SubjectName = certificateCer.Subject;
							certificateViewModel.Issuer = certificateCer.Issuer;
							break;
						default:
							throw new Exception("Unsupported file");
					}
					certificateViewModel.Guid = fileName;
					certificateViewModel.Extension = fileExtension;

					Certificate certificate = _mapper.Map<Certificate>(certificateViewModel);
					PopulateModelWithSelectedValues(certificateViewModel.ApplicationIds, certificateViewModel.ServerIds, certificateViewModel.GroupIds, certificate);
					_context.Certificate.Add(certificate);
					await _context.SaveChangesAsync();
					TempData["Msg"] = certificateViewModel.Name + " sucessfully added";
				}
				catch (Exception ex)
				{
					if (System.IO.File.Exists(filePath))
						System.IO.File.Delete(filePath);

					if (ex.Message.ToLower().Contains("password"))
						TempData["Msg"] = "Error: Invalid certificate password";
					else
						TempData["Msg"] = "Error: " + ex.Message;
					return RedirectToAction(nameof(Create));
				}

				return RedirectToAction(nameof(Index));
			}
			LoadFieldsViewModel(certificateViewModel);
			return View(certificateViewModel);
		}

		// GET: Certificates/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var certificate = await _context.Certificate
				.Include(x => x.Applications)
				.Include(x => x.Servers)
				.Include(x => x.Groups)
				.FirstOrDefaultAsync(x => x.Id == id);

			if (certificate == null)
			{
				return NotFound();
			}
			CertificateViewModel viewModel = _mapper.Map<CertificateViewModel>(certificate);
			LoadFieldsViewModel(viewModel);
			if (certificate.Applications != null)
				viewModel.ApplicationIds = certificate.Applications.Select(x => x.ApplicationId.ToString());
			if (certificate.Servers != null)
				viewModel.ServerIds = certificate.Servers.Select(x => x.ServerId.ToString());
			if (certificate.Groups != null)
				viewModel.GroupIds = certificate.Groups.Select(x => x.GroupId.ToString());
			return View(viewModel);
		}

		// POST: Certificates/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Location,Purpose,GroupIds,ServerIds,ApplicationIds,Environment,InstallationLink,OtherInfo,Active")] CertificateViewModel certificateViewModel)
		{
			if (id != certificateViewModel.Id)
			{
				return NotFound();
			}

			var certificateSaved = await _context.Certificate.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
			certificateViewModel.From = certificateSaved.From;
			certificateViewModel.To = certificateSaved.To;
			certificateViewModel.SerialNumber = certificateSaved.SerialNumber;
			certificateViewModel.SubjectName = certificateSaved.SubjectName;
			certificateViewModel.Issuer = certificateSaved.Issuer;
			certificateViewModel.Guid = certificateSaved.Guid;
			certificateViewModel.Extension = certificateSaved.Extension;
			ModelState.Remove("File");
			if (ModelState.IsValid)
			{
				try
				{
					Certificate certificate = _mapper.Map<Certificate>(certificateViewModel);
					PopulateModelWithSelectedValues(certificateViewModel.ApplicationIds, certificateViewModel.ServerIds, certificateViewModel.GroupIds, certificate);
					//remove old certificateapplication dependency
					_context.CertificateApplication.RemoveRange(_context.CertificateApplication.Where(x => x.CertificateId == id));
					//remove old certificateserver dependency
					_context.CertificateServer.RemoveRange(_context.CertificateServer.Where(x => x.CertificateId == id));
					//remove old certificategroup dependency
					_context.CertificateGroup.RemoveRange(_context.CertificateGroup.Where(x => x.CertificateId == id));
					await _context.SaveChangesAsync();

					_context.Update(certificate);
					await _context.SaveChangesAsync();
					TempData["Msg"] = certificateViewModel.Name + " sucessfully updated";
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!CertificateExists(certificateViewModel.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			LoadFieldsViewModel(certificateViewModel);
			return View(certificateViewModel);
		}

		// GET: Certificates/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var certificate = await _context.Certificate
							.Include(x => x.Applications)
							.Include(x => x.Servers)
							.Include(x => x.Groups)
							.FirstOrDefaultAsync(x => x.Id == id);
			if (certificate == null)
			{
				return NotFound();
			}
			CertificateViewModel viewModel = _mapper.Map<CertificateViewModel>(certificate);
			return View(viewModel);
		}

		// POST: Certificates/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var certificate = await _context.Certificate.FindAsync(id);
			_context.Certificate.Remove(certificate);
			await _context.SaveChangesAsync();
			var path = _configuration.GetSection("AppSettings")["CertificatesPath"];
			try
			{
				System.IO.File.Delete(System.IO.Path.Combine(path, certificate.Guid + certificate.Extension));
			}
			catch (Exception ex)
			{
				throw ex;
			}
			TempData["Msg"] = certificate.Name + " sucessfully removed.";
			return RedirectToAction(nameof(Index));
		}

		private bool CertificateExists(int id)
		{
			return _context.Certificate.Any(e => e.Id == id);
		}

		[HttpGet]
		public FileResult Download(int id)
		{
			Certificate certificate = _context.Certificate.FirstOrDefault(x => x.Id == id);
			var net = new System.Net.WebClient();
			var path = _configuration.GetSection("AppSettings")["CertificatesPath"];
			var data = net.DownloadData(path + certificate.Guid + certificate.Extension);
			var content = new System.IO.MemoryStream(data);
			var contentType = "APPLICATION/octet-stream";
			var fileName = certificate.Name + certificate.Extension;
			return File(content, contentType, fileName);
		}

		[HttpPost]
		public void SendEmail(int id, string emails)
		{
			Certificate certificate = _context.Certificate
				.Include(x => x.Servers)
					.ThenInclude(y => y.Server)
				.Include(x => x.Applications)
				.ThenInclude(y => y.Application)
				.FirstOrDefault(x => x.Id == id);

			string[] emailsArray = emails.Split(';');
			string htmlText = System.IO.File.ReadAllText(_configuration.GetSection("AppSettings")["EmailDetailsLocation"]);
			htmlText = htmlText.Replace("{certificateName}", certificate.Name + certificate.Extension)
			.Replace("{subjectName}", certificate.SubjectName)
			.Replace("{serialNumber}", certificate.SerialNumber)
			.Replace("{issuer}", certificate.Issuer)
			.Replace("{environment}", certificate.Environment.ToString())
			.Replace("{expiryDate}", certificate.From.ToShortDateString() + " to " + certificate.To.ToShortDateString())
			.Replace("{servers}", certificate.Servers.Count > 0 ? String.Join(", ", certificate.Servers.Select(x => x.Server.Name)) : "")
			.Replace("{applications}", certificate.Applications.Count > 0 ? String.Join(", ", certificate.Applications.Select(x => x.Application.Name)) : "")
			.Replace("{link}", _configuration.GetSection("AppSettings")["CertificatesURL"] + certificate.Id.ToString());

			Attachment attachment = new Attachment(_configuration.GetSection("AppSettings")["CertificatesPath"] + certificate.Guid + certificate.Extension, MediaTypeNames.Application.Octet);
			attachment.ContentDisposition.FileName = certificate.Name + certificate.Extension;
			foreach (var email in emailsArray)
				_emailJobs.SMTPSendEmail(email, htmlText, "Certificate " + certificate.Name + certificate.Extension, attachment);

		}

		[HttpPost]
		public string ShowPassword(int id, string password)
		{
			Certificate certificate = _context.Certificate.FirstOrDefault(x => x.Id == id);
			if (_configuration.GetSection("AppSettings")["PasswordValidation"] == password)
				return certificate.Password;
			return "N/A";
		}

		private void LoadFieldsViewModel(CertificateViewModel viewModel)
		{
			viewModel.Applications = _context.Application.Select(x =>
				new SelectListItem
				{
					Value = x.Id.ToString(),
					Text = x.Name
				}).ToList();

			viewModel.Servers = _context.Server.Select(x =>
				new SelectListItem
				{
					Value = x.Id.ToString(),
					Text = x.Name
				}).ToList();

			viewModel.Groups = _context.Group.Select(x =>
				new SelectListItem
				{
					Value = x.Id.ToString(),
					Text = x.Name
				}).ToList();
		}

		private void PopulateModelWithSelectedValues(IEnumerable<string> applicationIds, IEnumerable<string> serverIds, IEnumerable<string> groupIds, Certificate certificate)
		{
			foreach (string item in applicationIds)
			{
				CertificateApplication certificateApplication = new CertificateApplication();
				certificateApplication.Application = _context.Application.FirstOrDefault(x => x.Id == Convert.ToInt32(item));
				certificate.Applications.Add(certificateApplication);
			}

			foreach (string item in serverIds)
			{
				CertificateServer certificateServer = new CertificateServer();
				certificateServer.Server = _context.Server.FirstOrDefault(x => x.Id == Convert.ToInt32(item));
				certificate.Servers.Add(certificateServer);
			}

			foreach (string item in groupIds)
			{
				CertificateGroup certificateGroup = new CertificateGroup();
				certificateGroup.Group = _context.Group.FirstOrDefault(x => x.Id == Convert.ToInt32(item));
				certificate.Groups.Add(certificateGroup);
			}
		}

	}
}
