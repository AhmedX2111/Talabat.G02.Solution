using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Linq.Expressions;
using System.Net;
using Talabat.APIS.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services.Contract;

namespace Talabat.APIS.Controllers
{
	
	public class PaymentController : BaseApiController
	{
		private readonly IPaymentService _paymentService;
		private readonly ILogger<PaymentController> _logger;

		// This is your Stripe CLI webhook secret for testing your endpoint locally.
		private const string WhSecret = "whsec_c51fa7b099e01759d178b7f38aa9cf6d7d6d738531600687041d91357c992e21";
		public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
		{
			_paymentService = paymentService;
			_logger = logger;
		}
		[Authorize]
		[ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		[HttpPost("{basketId}")]
		public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
		{
			var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

			if (basketId is null) return BadRequest(new ApiResponse(400, "An Error with your Basket"));

			return Ok(basket);
		}

		[HttpPost("webhook")]
		public async Task<IActionResult> Webhook()
		{
			var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
			
				var stripeEvent = EventUtility.ConstructEvent(json,
					Request.Headers["Stripe-Signature"], WhSecret);

				var paymentIntent = (PaymentIntent) stripeEvent.Data.Object;

			Order? order;

				// Handle the event
				switch (stripeEvent.Type)
				{
					case Events.PaymentIntentSucceeded:
						order = await _paymentService.UpdateOrderStatus(paymentIntent.Id, true);

					_logger.LogInformation($"Order is succeeded {order?.PaymentIntentId}");
					break;
					case Events.PaymentIntentPaymentFailed:
						order = await _paymentService.UpdateOrderStatus(paymentIntent.Id, false);
					_logger.LogInformation($"Order is failed {order?.PaymentIntentId}");
					break;

				}

				return Ok();
			
			
		}

	}
}
