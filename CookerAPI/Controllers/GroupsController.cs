using CookerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace CookerAPI.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class GroupsController : ControllerBase
	{
		private readonly CookerDbContext db;
		private IHostingEnvironment _environment;

		public GroupsController(CookerDbContext context, IHostingEnvironment environment)
		{
			db = context;
			_environment = environment;
		}
		[HttpGet("GetAllGroups")]
		public async Task<List<Group>> GetAllGroups()
		{
			List<Group> Groups = await db.Groups.ToListAsync();

			for (int i = 0; i < Groups.Count(); i++)
			{
				Groups[i].Gimage = Path.Combine(_environment.WebRootPath, "Image", Groups[i].Gimage);

			}
			return Groups;
		}
		[HttpGet("GetSearchGroups")]
		public async Task<List<Group>> GetSearchGroups(string str)
		{

			int.TryParse(str, out int groupId);
			List<Group> groups = new List<Group>();
			groups = await db.Groups.Where(g => g.Id == groupId || g.Gname.Contains(str)).ToListAsync();

			for (int i = 0; i < groups.Count(); i++)
			{
				groups[i].Gimage = Path.Combine(_environment.WebRootPath, "Image", groups[i].Gimage);

			}
			return groups;
		}

		[HttpPost("AddGroup")]
		public async Task<IActionResult> AddGroup([FromBody] Group group)
		{
			//Random rnd = new Random();
			//int num = rnd.Next();
			//var Fname = (num).ToString() + ext;
			var ext = ".Png";
			var Fname = Guid.NewGuid() + ext;


			byte[] bytes = Convert.FromBase64String(group.Gimage);
			MemoryStream stream = new MemoryStream(bytes);

			IFormFile file = new FormFile(stream, 0, bytes.Length, Fname, Fname);

			var Nfile = Path.Combine(_environment.WebRootPath, "Image/", file.FileName);

			using (var fileStream = new FileStream(Nfile, FileMode.Create))
			{
				await file.CopyToAsync(fileStream);
			}

			group.Gimage = Fname;

			await db.Groups.AddAsync(group);
			var update = await db.SaveChangesAsync();

			if (update != 0)
			{
				return Created();
			}
			return BadRequest();
		}

		[HttpGet("GetGroup")]
		public async Task<Group> GetGroup(int gId)
		{
			var groups = await db.Groups.FirstOrDefaultAsync(g => g.Id == gId);
			if (groups.Gimage != null) { 
				groups.Gimage = Path.Combine(_environment.WebRootPath, "Image", groups.Gimage);

			}
			return groups;
		}

		[HttpPost("EditGroup")]
		public async Task<IActionResult> EditGroup([FromBody] Group group)
		{
			Group oldGroup = await db.Groups.FirstOrDefaultAsync(g => g.Id == group.Id);
			if (oldGroup != null)
			{
				if (group.Gname != null)
				{
					oldGroup.Gname = group.Gname;
				}
				if (group.Gimage != null)
				{
					var ext = ".jpg";
					Random rnd = new Random();
					int num = rnd.Next();

					byte[] bytes = Convert.FromBase64String(group.Gimage);
					MemoryStream stream = new MemoryStream(bytes);

					var Fname = (num).ToString() + ext;

					IFormFile file = new FormFile(stream, 0, bytes.Length, Fname, Fname);

					var Nfile = Path.Combine(_environment.WebRootPath, "Image/", file.FileName);

					using (var fileStream = new FileStream(Nfile, FileMode.Create))
					{
						await file.CopyToAsync(fileStream);
					}

					oldGroup.Gimage = Fname;
				}
				if (group.Gstatus != null)
				{
					oldGroup.Gstatus = group.Gstatus;
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
