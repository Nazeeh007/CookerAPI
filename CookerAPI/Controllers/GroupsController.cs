using CookerAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookerAPI.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class GroupsController : ControllerBase
	{
		private readonly CookerDbContext db;

		public GroupsController(CookerDbContext context)
		{
			db = context;
		}
		[HttpGet("GetAllGroups")]
		public async Task<List<Group>> GetAllGroups()
		{
			var groups =  await db.Groups.ToListAsync();
			return groups;
		}
		[HttpGet("GetSearchGroups")]
		public async Task<List<Group>> GetSearchGroups(string str)
		{

			int.TryParse(str, out int groupId);
			List<Group> groups = new List<Group>();
			groups = await db.Groups.Where(g => g.Id == groupId || g.Gname.Contains(str)).ToListAsync();
			return groups;
		}

		[HttpPost("AddGroup")]
		public async Task<IActionResult> AddGroup([FromBody] Group group)
		{
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
			return groups;
		}

		[HttpGet("EditGroup")]
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
					oldGroup.Gimage = group.Gimage;

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
