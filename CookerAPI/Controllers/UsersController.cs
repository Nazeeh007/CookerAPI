using CookerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookerAPI.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly CookerDbContext db;

		public UsersController(CookerDbContext context)
		{
			db = context;
		}

		[HttpGet("Login")]
		public IActionResult Login(string username, string password)
		{ 
			var user =db.UserPasses.FirstOrDefault(u => u.UserName == username && u.Pass == password);
			if(user != null)
			{
				//return user.Id;
				return Ok();
			}
			//return 0;
			return BadRequest();
		}
		[HttpGet("ChangePassword")]
		public async Task<IActionResult> ChangePassword(int UserId, string oldPassword, string newPassword)
		{
			if (oldPassword != newPassword)
				return BadRequest("Password doesng match");

			var user = db.UserPasses.FirstOrDefault(u => u.Id == UserId && u.Pass == oldPassword);
			if (user != null)
			{
				user.Pass = newPassword;
				var updated =await db.SaveChangesAsync();
				if (updated != 0)
					return Ok();
			}
			//return 0;
			return BadRequest();
		}

		[HttpPost("AddUser")]
		public async Task<IActionResult> AddUser([FromBody] NewUser user)
		{
			var countUser= db.UserPasses.Count(i => i.UserName==user.UserName && i.Pass==user.Pass);
			if (countUser == 0) { 
				UserPass nwUser = new UserPass();
				nwUser.EmployeeName = user.EmployeeName;
				nwUser.UserName = user.UserName;
				nwUser.Pass = user.Pass;
				nwUser.UserStatus = true;
				db.UserPasses.Add(nwUser);
				var updated = await db.SaveChangesAsync();
				if (updated != 0)
					return CreatedAtAction(nameof(AddUser), new { id = nwUser.Id }, user);
				return BadRequest();
			}
			return BadRequest();
		}

		[HttpGet("GetAllUsers")]
		public async Task<List<UserPass>> GetAllUsers()
		{
			List<UserPass> LUsers = new List<UserPass>();
			LUsers = await db.UserPasses.ToListAsync();
			return LUsers;
		}

		[HttpGet("GeUsers")]
		public async Task<List<UserPass>> GetUsers(int id)
		{
			List<UserPass> LUsers = new List<UserPass>();
			var usr = db.UserPasses.FirstOrDefault(u => u.Id == id);
			if (usr != null)
			{
				LUsers.Add(usr);
			}
			return LUsers;
		}

		[HttpPost("EditUser")]
		public async Task<IActionResult> EditUser([FromBody] UserPass user)
		{
			var oldUser = db.UserPasses.FirstOrDefault(i => i.UserName == user.UserName && i.Pass == user.Pass);
			oldUser.EmployeeName = user.EmployeeName;
			oldUser.UserName = user.UserName;
			oldUser.Pass = user.Pass;
			oldUser.UserStatus = user.UserStatus;

			var updated = await db.SaveChangesAsync();
			if (updated != 0)
				return Ok();
			return BadRequest();

		}
		[HttpGet("ChangeUserStatus")]
		public async Task<IActionResult> ChangeUserStatus(int UserId, bool status)
		{
			var user = db.UserPasses.FirstOrDefault(u => u.Id == UserId);
			if (user != null)
			{
				user.UserStatus = status;
				var updated = await db.SaveChangesAsync();
				if (updated != 0)
					return Ok();
			}
			return BadRequest();
		}
		[HttpGet("GetAllRoles")]
		public async Task<List<Role>> GetAllRoles()
		{
			List<Role> roles = new List<Role>();
			roles = await db.Roles.ToListAsync();
			return roles;
		}

		[HttpGet("GetUserRoles")]
		public async Task<List<UserRole>> GetUserRoles(int id)
		{

			var ListUserRoles = await db.UserRoles.Where(u => u.UserId == id).ToListAsync();
			return ListUserRoles;
		}


		[HttpGet("AddUserRole")]
		public async Task<IActionResult> AddUserRole(int userId, int roleId)
		{

			var CUser = db.UserPasses.Count(i => i.Id == userId);
			var CRole = db.Roles.Count(i => i.Id == roleId);
			if (CUser > 0 && CRole > 0) { 
				var CUserRole = db.UserRoles.Count(i => i.UserId == userId && i.RoleId == roleId);
				if (CUserRole == 0)
				{
					UserRole nwUserRole = new UserRole();
					nwUserRole.UserId = userId;
					nwUserRole.RoleId = roleId;
					await db.UserRoles.AddAsync(nwUserRole);
					var updated = await db.SaveChangesAsync();
					if (updated != 0)
						return Ok();
					return BadRequest();
				}

			}
			return BadRequest();
		}

		[HttpGet("DeleteUserRole")]
		public async Task<IActionResult> DeleteUserRole(int id)
		{
		   var DUser = db.UserRoles.FirstOrDefault(i => i.Id == id);
			if (DUser != null) {
				db.UserRoles.Remove(DUser);
				var updated = await db.SaveChangesAsync();
				if (updated != 0)
					return Ok();
			}
			return BadRequest();
		}

	}
}
