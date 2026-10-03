using CookerAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookerAPI.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class OrdersController : ControllerBase
	{
		private readonly CookerDbContext db;

		public OrdersController(CookerDbContext context)
		{
			db = context;
		}

		[HttpGet("GetAllItemsByGroup")]
		public async Task<List<Item>> GetAllItemsByGroup(int groupId)
		{
		
			var items = await db.Items.Where(i => i.Gid == groupId).ToListAsync();
			return items;
		}

		//[HttpGet("AddOrder")]
		//public async Task<List<Item>> AddOrder([FromBody] Order order)
		//{

		//	if (order.OrderBillNo == 0) { 
		//		order.OrderBillNo = await db.Orders.MaxAsync(o => o.OrderBillNo) + 1;
		//	}																		 
		//	order.OrderDate = DateTime.Now;
		//	order.Quntity = 1;
		//	if (order.OrderPrice == 0) { 
		//		order.OrderPrice= await db.Items.Where(i => i.Id == order.ItmId).Select(i => i.ItmPrice).FirstOrDefaultAsync();
		//	}

		//}

	}
}
