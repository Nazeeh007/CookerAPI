using CookerAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookerAPI.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class ItemsController : ControllerBase
	{
		private readonly CookerDbContext db;

		public ItemsController(CookerDbContext context)
		{
			db = context;
		}
		[HttpGet("GetAllItems")]
		public async Task<List<Item>> GetAllItems() { 
			var items =await db.Items.ToListAsync();
			return items;
		}
	}
}
