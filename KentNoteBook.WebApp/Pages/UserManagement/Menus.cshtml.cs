﻿using System;
using System.Linq;
using System.Threading.Tasks;
using KentNoteBook.Data;
using KentNoteBook.Infrastructure.Html.Grid;
using KentNoteBook.Infrastructure.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace KentNoteBook.WebApp.Pages.UserManagement
{
	public class MenusModel : PageModel
	{
		public MenusModel(KentNoteBookDbContext db, IDistributedCache cache) {
			this._db = db;
			this._cache = cache;
		}

		readonly KentNoteBookDbContext _db;
		readonly IDistributedCache _cache;

		[BindProperty(SupportsGet = true)]
		public Guid[] IdArray { get; set; }

		public class SystemMenuModel
		{
			public Guid Id { get; set; }
			public Guid? ParentId { get; set; }
			public string Name { get; set; }
		}

		public void OnGet() {
		}

		public async Task<IActionResult> OnPostMenusAsync([FromForm] GridCriteria criteria) {
			return await _db.Menus
				.AsNoTracking()
				.Select(x => new SystemMenuModel {
					Id = x.Id,
					ParentId = x.ParentId,
					Name = x.Name
				}).ToDataSourceJsonResultAsync(criteria);
		}

		public async Task<IActionResult> OnPostRemoveAsync() {

			if ( IdArray == null || IdArray.Length == 0 ) {
				return new CustomResult(0, "Please select one item to remove.");
			}

			var deletes = await _db.Menus
				.Where(x => IdArray.Contains(x.Id))
				.ToListAsync();

			_db.RemoveRange(deletes);

			await _db.SaveChangesAsync();

			return new SuccessResult();
		}
	}
}