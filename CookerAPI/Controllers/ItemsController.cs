using CookerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;


namespace CookerAPI.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class ItemsController : ControllerBase
	{
		private readonly CookerDbContext db;
		private IHostingEnvironment _environment;

		public ItemsController(CookerDbContext context, IHostingEnvironment environment)
		{
			db = context;
			_environment = environment;

		}
		[HttpGet("GetAllItems")]
		public async Task<List<ItemView>> GetAllItems() { 
			var items =await db.ItemViews.ToListAsync();
			for (int i = 0; i < items.Count(); i++)
			{
				items[i].ItmImage = Path.Combine(_environment.WebRootPath, "Image", items[i].ItmImage);

			}
			return items;
		}
		[HttpGet("GetAllItemsByGroup")]
		public async Task<List<ItemView>> GetAllItemsByGroup(int grpId)
		{
			var items = await db.ItemViews.Where(i => i.Gid == grpId).ToListAsync();
			for (int i = 0; i < items.Count(); i++)
			{
				items[i].ItmImage = Path.Combine(_environment.WebRootPath, "Image", items[i].ItmImage);

			}
			return items;
		}

		[HttpGet("GetSearchItems")]
		public async Task<List<ItemView>> GetSearchItemss(string str)
		{

			int.TryParse(str, out int itemId);
			List<ItemView> items = new List<ItemView>();
			items = await db.ItemViews.Where(i => i.Expr1 == itemId || i.ItmName.Contains(str)).ToListAsync();
			for (int i = 0; i < items.Count(); i++)
			{
				items[i].ItmImage = Path.Combine(_environment.WebRootPath, "Image", items[i].ItmImage);

			}
			return items;
		}

		[HttpPost("AddItem")]
		public async Task<IActionResult> AddItem([FromBody] ItemView item)
		{
			var ext = ".Png";
			var Fname = Guid.NewGuid() + ext;


			byte[] bytes = Convert.FromBase64String(item.ItmImage);
			MemoryStream stream = new MemoryStream(bytes);

			IFormFile file = new FormFile(stream, 0, bytes.Length, Fname, Fname);

			var Nfile = Path.Combine(_environment.WebRootPath, "Image/", file.FileName);

			using (var fileStream = new FileStream(Nfile, FileMode.Create))
			{
				await file.CopyToAsync(fileStream);
			}

			item.ItmImage = Fname;
			await db.ItemViews.AddAsync(item);
			var update = await db.SaveChangesAsync();

			if (update != 0)
			{
				return Created();
			}
			return BadRequest();
		}

		[HttpGet("GetItem")]
		public async Task<ItemView> GetItem(int itemId)
		{
			var item = await db.ItemViews.FirstOrDefaultAsync(g => g.Expr1 == itemId);
			if (item.ItmImage != null)
			{
				item.ItmImage = Path.Combine(_environment.WebRootPath, "Image", item.ItmImage);

			}
			return item;
		}

		[HttpGet("EditItem")]
		public async Task<IActionResult> EditItem([FromBody] ItemView item)
		{
			ItemView oldItem = await db.ItemViews.FirstOrDefaultAsync(g => g.Expr1 == item.Expr1);
			if (oldItem != null)
			{
				if (item.ItmName != null)
				{
					oldItem.ItmName = item.ItmName;
				}
				if (item.ItmDesc != null)
				{
					oldItem.ItmDesc = item.ItmDesc;
				}
				if (item.ItmImage != null)
				{
					var ext = ".jpg";
					Random rnd = new Random();
					int num = rnd.Next();

					byte[] bytes = Convert.FromBase64String(item.ItmImage);
					MemoryStream stream = new MemoryStream(bytes);

					var Fname = (num).ToString() + ext;

					IFormFile file = new FormFile(stream, 0, bytes.Length, Fname, Fname);

					var Nfile = Path.Combine(_environment.WebRootPath, "Image/", file.FileName);

					using (var fileStream = new FileStream(Nfile, FileMode.Create))
					{
						await file.CopyToAsync(fileStream);
					}

					oldItem.ItmImage = Fname;
				}
				if (item.ItmPrice != null)
				{
					oldItem.ItmPrice = item.ItmPrice;
				}
				if (item.ItmStatus != null)
				{
					oldItem.ItmStatus = item.ItmStatus;
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
		[HttpGet("ChangeItemStatus")]
		public async Task<IActionResult> ChangeItemStatus(int itemId, bool status)
		{
			var item = db.Items.FirstOrDefault(u => u.Id == itemId);
			if (item != null)
			{
				item.ItmStatus = status;
				var updated = await db.SaveChangesAsync();
				if (updated != 0)
					return Ok();
			}
			return BadRequest();
		}
	}
}
