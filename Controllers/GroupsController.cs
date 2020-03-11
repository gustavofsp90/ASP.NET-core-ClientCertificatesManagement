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

namespace CertificatesManager.Controllers
{
	public class GroupsController : Controller
	{
		private readonly CertificatesManagerDBContext _context;
		private readonly IMapper _mapper;


		public GroupsController(CertificatesManagerDBContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		// GET: Groups
		public async Task<IActionResult> Index()
		{
			var group = await _context.Group
				.Include(x => x.Users)
					.ThenInclude(y => y.User)
				.ToListAsync();
			List<GroupViewModel> viewModels = _mapper.Map<List<GroupViewModel>>(group);
			return View(viewModels);
		}

		// GET: Groups/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var @group = await _context.Group
				.Include(x => x.Users)
					.ThenInclude(y => y.User)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (@group == null)
			{
				return NotFound();
			}
			GroupViewModel viewModel = _mapper.Map<GroupViewModel>(@group);
			return View(viewModel);
		}

		// GET: Groups/Create
		public IActionResult Create()
		{
			var viewModel = new GroupViewModel();
			LoadFieldsViewModel(viewModel);
			return View(viewModel);
		}

		// POST: Groups/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Name, UserIds")] GroupViewModel groupViewModel)
		{
			if (ModelState.IsValid)
			{
				Group group = _mapper.Map<Group>(groupViewModel);
				PopulateModelWithSelectedValues(groupViewModel.UserIds, group);
				_context.Add(@group);
				await _context.SaveChangesAsync();
				TempData["Msg"] = groupViewModel.Name + " sucessfully added";
				return RedirectToAction(nameof(Index));
			}
			LoadFieldsViewModel(groupViewModel);
			return View(groupViewModel);
		}

		// GET: Groups/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var @group = await _context.Group
				.Include(x => x.Users)
				.FirstOrDefaultAsync(x => x.Id == id);
			if (@group == null)
			{
				return NotFound();
			}
			GroupViewModel viewModel = _mapper.Map<GroupViewModel>(group);
			LoadFieldsViewModel(viewModel);
			if (group.Users != null)
				viewModel.UserIds = group.Users.Select(x => x.UserId.ToString());
			return View(viewModel);
		}

		// POST: Groups/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Name, UserIds")] GroupViewModel groupViewModel)
		{
			if (id != groupViewModel.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					Group group = _mapper.Map<Group>(groupViewModel);
					PopulateModelWithSelectedValues(groupViewModel.UserIds, group);

					//remove old groupuser dependency
					_context.GroupUser.RemoveRange(_context.GroupUser.Where(x => x.GroupId == id));
					await _context.SaveChangesAsync();

					_context.Update(@group);
					await _context.SaveChangesAsync();
					TempData["Msg"] = groupViewModel.Name + " sucessfully updated";
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!GroupExists(groupViewModel.Id))
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
			LoadFieldsViewModel(groupViewModel);
			return View(groupViewModel);
		}

		// GET: Groups/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var @group = await _context.Group
				.Include(x => x.Users)
					.ThenInclude(y => y.User)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (@group == null)
			{
				return NotFound();
			}
			GroupViewModel viewModel = _mapper.Map<GroupViewModel>(group);
			return View(viewModel);
		}

		// POST: Groups/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var @group = await _context.Group.FindAsync(id);
			_context.Group.Remove(@group);
			await _context.SaveChangesAsync();
			TempData["Msg"] = @group.Name + " sucessfully removed.";
			return RedirectToAction(nameof(Index));
		}

		private bool GroupExists(int id)
		{
			return _context.Group.Any(e => e.Id == id);
		}

		private void LoadFieldsViewModel(GroupViewModel viewModel)
		{
			viewModel.Users = _context.User.Select(x =>
						new SelectListItem
						{
							Value = x.Id.ToString(),
							Text = x.Name
						}).ToList();
		}
		private void PopulateModelWithSelectedValues(IEnumerable<string> userIds, Group group)
		{
			foreach (string item in userIds)
			{
				GroupUser userGroup = new GroupUser();
				userGroup.User = _context.User.FirstOrDefault(x => x.Id == Convert.ToInt32(item));
				group.Users.Add(userGroup);
			}
		}
	}
}
