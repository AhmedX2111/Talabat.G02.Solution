using AutoMapper;
using Talabat.APIS.Dtos;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.APIS.Helpers
{
	public class OrderItemPictureUrlResolver : IValueResolver<OrderItem, OrderItemDto, string>
	{
		private readonly IConfiguration _configuration;

		public OrderItemPictureUrlResolver(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
		{
			return $"{_configuration["ApiBaseUrl"]}/{source.Product.PictureUrl}";
		}
	}
}
