using System.Diagnostics.Eventing.Reader;
using CookerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace CookerAPI.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class OrdersController : ControllerBase
	{
		private readonly CookerDbContext db;
		private readonly IHubContext<ChatHub> _chatHub;
		private IHostingEnvironment _environment;


		public OrdersController(CookerDbContext context, IHostingEnvironment environment, IHubContext<ChatHub> hubContext)
		{
			db = context;
			_environment = environment;
			_chatHub = hubContext;
		}

		[HttpGet("GetAllItemsByGroup")]
		public async Task<List<Item>> GetAllItemsByGroup(int groupId)
		{
		
			var items = await db.Items.Where(i => i.Gid == groupId).ToListAsync();
			return items;
		}

		[HttpPost("AddOrder")]
		public async Task<IActionResult> AddOrder([FromBody] NewOrder newOrder)
		{
			Order nOrder = new Order();
			nOrder.OrderBillNo = newOrder.OrderBillNo;
			nOrder.OrderTableNo = newOrder.OrderTableNo;
			nOrder.ItmId = newOrder.ItmId;


			if (nOrder.OrderBillNo == 0)
			{
				if (db.Orders.Count() == 0)
				{
					nOrder.OrderBillNo = 1;
				}
				else
				{
					nOrder.OrderBillNo = await db.Orders.MaxAsync(o => o.OrderBillNo) + 1;
				}
			}
			nOrder.OrderDate = DateTime.Now;
			nOrder.Quntity = 1;
			
			//nOrder.OrderPrice = await db.Items.Where(i => i.Id == nOrder.ItmId).Select(i => i.ItmPrice).FirstOrDefaultAsync();
			Item item = await db.Items.FindAsync(nOrder.ItmId);

			if (item != null)
			{
				//vale.Date to get the date
				Order oldOrder = db.Orders.FirstOrDefault(i => i.OrderTableNo == nOrder.OrderTableNo && i.OrderBillNo == nOrder.OrderBillNo && i.ItmId == item.Id && i.OrderDate.Value.Date == DateTime.Now.Date);
				if (oldOrder == null)
				{

					nOrder.OrderPrice = item.ItmPrice;
					nOrder.OrderTotal = nOrder.OrderPrice * nOrder.Quntity;
					nOrder.ConfirmFlag = false;
					db.Orders.Add(nOrder);
				}
				else {
					oldOrder.Quntity++;
					oldOrder.OrderTotal = oldOrder.Quntity * item.ItmPrice;
				}
				var update = await db.SaveChangesAsync();
				if (update != 0)
				{
					calcTotal(nOrder.OrderBillNo, nOrder.OrderTableNo);
					return Ok();
				}
			}
			return BadRequest();
		}

		[HttpGet("ChangeQuantity")]
		public async Task<IActionResult> ChangeQuantity(int orderId, float quantity)
		{
			var order = await db.Orders.FindAsync(orderId);
			if (order == null)
				return NotFound();
			order.Quntity = quantity;
			order.OrderTotal = order.OrderPrice * order.Quntity;
			db.Orders.Update(order);
			var update = await db.SaveChangesAsync();
			if (update != 0) {
				calcTotal(order.OrderBillNo, order.OrderTableNo);
				return Ok();

			}
			return BadRequest();
		}

		[HttpGet("DeleteOrder")]
		public async Task<IActionResult> DeleteOrder(int orderId)
		{
			var order = await db.Orders.FindAsync(orderId);
			if (order == null)
				return NotFound();
			
			db.Orders.Remove(order);
			var update = await db.SaveChangesAsync();
			if (update != 0) {
				calcTotal(order.OrderBillNo, order.OrderTableNo);
				return Ok();
			}
			return BadRequest();
		}

		[HttpGet("ConfirmOrder")]
		public async Task<IActionResult> ConfirmOrder(int orderBillNo)
		{
			List<Order> lOrders = await db.Orders.Where(o => o.OrderBillNo == orderBillNo).ToListAsync();
			if (lOrders.Count == 0)
				return NotFound();
			for (int i = 0; i < lOrders.Count; i++) { 
				lOrders[i].ConfirmFlag = true;
			}
			await _chatHub.Clients.All.SendAsync("MessageReceived", orderBillNo);
			

			var update = await db.SaveChangesAsync();
			if (update != 0)
				return Ok();
			return BadRequest();
		}	  

		[HttpGet("GetOrder")]
		public async Task<List<OrdersView>> GetOrder(int orderBillNo)
		{
			List<OrdersView> lOrders = await db.OrdersViews.Where(o => o.OrderBillNo == orderBillNo).ToListAsync();
			for (int i = 0; i < lOrders.Count(); i++)
			{
				if (lOrders[i].ItmImage != null)
				{
					lOrders[i].ItmImage = Path.Combine(_environment.WebRootPath, "Image", lOrders[i].ItmImage);
				}
			}
			return lOrders;

		}

		[HttpGet("OrderHeader")]
		public async Task<OrderHeader> OrderHeader(int orderTableNo)
		{
			OrderHeader orderHeader = new OrderHeader();
			orderHeader.CCount = await db.OrdersViews.Where(o => o.OrderTableNo == orderTableNo && o.PayFlag ==false).CountAsync();
			orderHeader.Total = await db.OrdersViews.Where(o => o.OrderTableNo == orderTableNo && o.PayFlag == false).SumAsync(o => o.OrderTotal);
			var tttoalOrder = await db.TotalOrders.FirstOrDefaultAsync(o => o.ToTableNo == orderTableNo && o.PayFlag == false);
			if (tttoalOrder == null) { orderHeader.BillNo = 0; }
			else { orderHeader.BillNo = tttoalOrder.ToBillNo; }
			return orderHeader;

		}

		[HttpGet("TotalOrderPay")]
		public async Task<IActionResult> TotalOrderPay(int orderBillNo)
		{
			TotalOrder oldOrder = await db.TotalOrders.FirstOrDefaultAsync(o => o.ToBillNo == orderBillNo);
			if (oldOrder == null)
				return NotFound();
			oldOrder.PayFlag = true;
			var update = await db.SaveChangesAsync();
			if(update != 0)
				return Ok();
			return BadRequest(); 
		}

		[HttpGet("GetAllOrders")]
		public async Task<List<TotalOrder>> GetAllOrders()
		{

			List<TotalOrder> LtotalOrder = await db.TotalOrders.Where(I => I.PayFlag != null).ToListAsync();

			return LtotalOrder;
		}

		[HttpGet("GetAllOrdersReports")]
		public async Task<List<TotalOrder>> GetAllOrdersReports(string strOrderDate)
		{
			DateTime orderDate = Convert.ToDateTime(strOrderDate);

			List<TotalOrder> LtotalOrder = await db.TotalOrders.Where(I => I.PayFlag == true && I.ToDate.Value.Date == orderDate).ToListAsync();

			return LtotalOrder;
		}

		private void calcTotal(int? billNo, int? tableNo)
		{
			var total = db.Orders.Where(i => i.OrderBillNo == billNo).Sum(i => i.OrderTotal);
			TotalOrder totalorder = db.TotalOrders.FirstOrDefault(i => i.ToBillNo == billNo);
			if (totalorder == null)
			{
				TotalOrder nTotalOrder = new TotalOrder();
				nTotalOrder.ToBillNo = billNo;
				nTotalOrder.ToTableNo = tableNo;
				nTotalOrder.ToDate = DateTime.Now;
				nTotalOrder.ToTotal = total;
				nTotalOrder.PayFlag = false;
				db.TotalOrders.Add(nTotalOrder);
			}
			else { 
				totalorder.ToTotal = total;
			}
			db.SaveChanges();
		}

		
	}
}
