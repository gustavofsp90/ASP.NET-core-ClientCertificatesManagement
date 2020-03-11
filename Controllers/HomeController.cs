using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CertificatesManager.Models;
using CertificatesManager.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CertificatesManager.Controllers
{
	public class HomeController : Controller
	{
		private readonly CertificatesManagerDBContext _context;
		private IConfiguration _configuration;
		public HomeController(CertificatesManagerDBContext context, IConfiguration Configuration)
		{
			_context = context;
			_configuration = Configuration;
		}

		public async Task<IActionResult> Index()
		{
			var certificate = await _context.Certificate
				.Include(x => x.Applications)
					.ThenInclude(y => y.Application)
				.Include(x => x.Servers)
					.ThenInclude(y => y.Server)
				.Include(x => x.Groups)
					.ThenInclude(y => y.Group)
				.ToListAsync();

			int dueToExpireDays = Convert.ToInt32(_configuration.GetSection("AppSettings")["DueToExpireDays"]);
			ViewData["TotalValid"] = certificate.Where(x => (Convert.ToInt32((x.To - DateTime.Now).TotalDays) > dueToExpireDays) && x.Active).Count();
			ViewData["TotalDueToExpire"] = certificate.Where(x => (x.To >= DateTime.Today) && (Convert.ToInt32((x.To - DateTime.Now).TotalDays) <= dueToExpireDays) && x.Active).Count();
			ViewData["TotalExpired"] = certificate.Where(x => (x.To < DateTime.Today) && x.Active).Count();
			ViewData["DueToExpireDays"] = dueToExpireDays;

			return View();
		}

		public IActionResult About()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		public IActionResult Contact()
		{
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
