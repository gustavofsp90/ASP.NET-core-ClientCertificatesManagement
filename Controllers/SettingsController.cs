using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CertificatesManager.Database;
using CertificatesManager.Models;
using AutoMapper;
using CertificatesManager.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace CertificatesManager.Controllers
{
	public class SettingsController : Controller
	{
		private readonly CertificatesManagerDBContext _context;
		private readonly IMapper _mapper;

		public SettingsController(CertificatesManagerDBContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		// GET: Settings
		public async Task<IActionResult> Index()
		{
			var settings = await _context.Settings
				.Include(x => x.Group)
				.ToListAsync();
			List<SettingsViewModel> viewModels = _mapper.Map<List<SettingsViewModel>>(settings);
			return View(viewModels);
		}

		// GET: Settings/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var settings = await _context.Settings
				.Include(x => x.Group)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (settings == null)
			{
				return NotFound();
			}

			SettingsViewModel viewModel = _mapper.Map<SettingsViewModel>(settings);
			return View(viewModel);
		}

		// GET: Settings/Create
		public IActionResult Create()
		{
			var viewModel = new SettingsViewModel();
			LoadFieldsViewModel(viewModel);
			return View(viewModel);
		}

		// POST: Settings/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("File,Id,EmailSubject,EmailText,DaysBeforeEmail, Group")] SettingsViewModel settingsViewModel)
		{
			Settings settings = _mapper.Map<Settings>(settingsViewModel);
			PopulateModelWithSelectedValues(settingsViewModel.Group, settings);
			var hasSettingsCreated = _context.Settings.Any(x => x.Group.Id.ToString() == settingsViewModel.Group);
			if (hasSettingsCreated)
				ModelState.AddModelError("Group", "Setting already created with this group. Please select another group.");
			if (ModelState.IsValid)
			{
				_context.Settings.Add(settings);
				await _context.SaveChangesAsync();
				TempData["Msg"] = "Settings for " + settings.Group.Name + " sucessfully added";
				return RedirectToAction(nameof(Index));
			}
			else
			{
				//ValidationResult result = new ValidationResult("Please select a user.");
				LoadFieldsViewModel(settingsViewModel);
				return View(settingsViewModel);
			}
		}

		// GET: Settings/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var settings = await _context.Settings
				.Include(x => x.Group)
				.FirstOrDefaultAsync(x => x.Id == id);
			if (settings == null)
			{
				return NotFound();
			}

			SettingsViewModel viewModel = _mapper.Map<SettingsViewModel>(settings);
			LoadFieldsViewModel(viewModel);
			if (settings.Group != null)
				viewModel.Group = settings.Group.Id.ToString();


			return View(viewModel);
		}

		// POST: Settings/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,EmailSubject,EmailText,DaysBeforeEmail, Group")] SettingsViewModel settingsViewModel)
		{
			if (id != settingsViewModel.Id)
			{
				return NotFound();
			}
			var hasSettingsCreated = _context.Settings.Any(x => x.Group.Id.ToString() == settingsViewModel.Group && x.Id != settingsViewModel.Id);
			if (hasSettingsCreated)
				ModelState.AddModelError("Group", "Setting already created with this group. Please select another group.");
			if (ModelState.IsValid)
			{
				try
				{
					Settings settings = _mapper.Map<Settings>(settingsViewModel);
					PopulateModelWithSelectedValues(settingsViewModel.Group, settings);
					_context.Update(settings);
					await _context.SaveChangesAsync();
					TempData["Msg"] = "Setting sucessfully updated";
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!SettingsExists(settingsViewModel.Id))
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
			LoadFieldsViewModel(settingsViewModel);
			return View(settingsViewModel);
		}

		// GET: Settings/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var settings = await _context.Settings
				.Include(x => x.Group)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (settings == null)
			{
				return NotFound();
			}

			SettingsViewModel viewModel = _mapper.Map<SettingsViewModel>(settings);
			return View(viewModel);
		}

		// POST: Settings/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var settings = await _context.Settings.FindAsync(id);
			_context.Settings.Remove(settings);
			await _context.SaveChangesAsync();
			TempData["Msg"] = "Settings sucessfully removed.";
			return RedirectToAction(nameof(Index));
		}

		private bool SettingsExists(int id)
		{
			return _context.Settings.Any(e => e.Id == id);
		}

		private void LoadFieldsViewModel(SettingsViewModel viewModel)
		{
			viewModel.Groups = _context.Group.Select(x =>
						new SelectListItem
						{
							Value = x.Id.ToString(),
							Text = x.Name
						}).ToList();
		}
		private void PopulateModelWithSelectedValues(string groupId, Settings settings)
		{
			settings.Group = _context.Group.FirstOrDefault(x => x.Id == Convert.ToInt32(groupId));
		}
	}
}
