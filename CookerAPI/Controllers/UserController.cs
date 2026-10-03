using CookerAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CookerAPI.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		CookerDbContext	db =new CookerDbContext();

		[HttpGet("Login")]
		public int Login(string username, string password)
		{ 
			var user =db.UserPasses.FirstOrDefault(u => u.UserName == username && u.Pass == password);
			if(user != null)
			{
				return user.Id;
			}
			return 0;
		}
	}
}
