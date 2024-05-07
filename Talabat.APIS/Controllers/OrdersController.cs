using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIS.Dtos;
using Talabat.APIS.Errors;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services.Contract;

namespace Talabat.APIS.Controllers
{
	[Authorize]
	public class OrdersController : BaseApiController
	{
		private readonly IOrderService _orderService;
		private readonly IMapper _mapper;
		public OrdersController(IOrderService orderService, IMapper mapper)
		{
			_orderService = orderService;
			_mapper = mapper;
		}
		[ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		[HttpPost]
		public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
		{
			var address = _mapper.Map<AddressDto,Address>(orderDto.ShippingAddress);

			var email = User.FindFirst(ClaimTypes.Email).Value;

			var order = await _orderService.CreateOrderAsync(orderDto.BuyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId, address );

			if (order is null) return BadRequest(new ApiResponse(404));

			return Ok(order);
		}
		[HttpGet]
		public async Task<ActionResult<IReadOnlyList<Order>>> GetOrderForUser()
		{

			var email = User.FindFirst(ClaimTypes.Email).Value;

			var orders = await _orderService.GetOrdersForUserAsync(email);

			return Ok(orders);
		}
	}
}   

