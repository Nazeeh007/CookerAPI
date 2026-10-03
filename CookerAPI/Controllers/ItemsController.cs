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

		public async Task<List<Group>> GetAllGroups()
		{
			var groups = await db.Groups.ToListAsync();
			return groups;
		}
		[HttpGet("GetSearchItemss")]
		public async Task<List<Item>> GetSearchItemss(string str)
		{

			int.TryParse(str, out int itemId);
			List<Item> items = new List<Item>();
			items = await db.Items.Where(i => i.Id == itemId || i.ItmName.Contains(str)).ToListAsync();
			return items;
		}

		[HttpGet("AddItem")]
		public async Task<IActionResult> AddItem([FromBody] Item item)
		{
			await db.Items.AddAsync(item);
			var update = await db.SaveChangesAsync();

			if (update != 0)
			{
				return Created();
			}
			return BadRequest();
		}

		[HttpGet("GetItem")]
		public async Task<Item> GetItem(int itemId)
		{
			var item = await db.Items.FirstOrDefaultAsync(g => g.Id == itemId);
			return item;
		}

		[HttpGet("EditItem")]
		public async Task<IActionResult> EditItem([FromBody] Item item)
		{
			Item oldItem = await db.Items.FirstOrDefaultAsync(g => g.Id == item.Id);
			if (oldItem != null)
			{
				if (item.ItmName != null)
				{
					item.ItmName = item.ItmName;
				}
				if (item.ItmDesc != null)
				{
					item.ItmDesc = item.ItmDesc;
				}
				if (item.ItmImage != null)
				{
					item.ItmImage = item.ItmImage;
				}
				if (item.ItmPrice != null)
				{
					item.ItmPrice = item.ItmPrice;
				}
				if (item.ItmStatus != null)
				{
					item.ItmStatus = item.ItmStatus;
				}

				var update = await db.SaveChangesAsync();
				if (update != 0)
				{
					return Ok();
				}
				return BadRequest();

			}
			return BadRequest();
		}
		[HttpGet("ChangeGroupStatus")]
		public async Task<IActionResult> ChangeGroupStatus(int groupId, bool status)
		{
			var group = db.Groups.FirstOrDefault(u => u.Id == groupId);
			if (group != null)
			{
				group.Gstatus = status;
				var updated = await db.SaveChangesAsync();
				if (updated != 0)
					return Ok();
			}
			return BadRequest();
		}
	}
}
