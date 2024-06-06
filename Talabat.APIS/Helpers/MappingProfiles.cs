using AutoMapper;
using Talabat.APIS.Dtos;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.APIS.Helpers
{
    public class MappingProfiles : Profile
    {
        

        public MappingProfiles()
        {

			CreateMap<Product, ProductToReturnDto>()
				.ForMember(P => P.Brand, O => O.MapFrom(S => S.Brand.Name))
				.ForMember(P => P.Category, O => O.MapFrom(S => S.Category.Name))
				.ForMember(P => P.PictureUrl, O => O.MapFrom<ProductPictureUrlResolver>());

			CreateMap<CustomerBasketDto, CustomerBasket>();
			CreateMap<BasketItemDto, BasketItem>();

			// Ensure Address mappings are correct
			CreateMap<Core.Entities.Identity.Address, AddressDto>().ReverseMap();
			CreateMap<AddressDto, Core.Entities.Order_Aggregate.Address>();

			CreateMap<Order, OrderToReturnDto>()
				.ForMember(D => D.DeliveryMethod, O => O.MapFrom(S => S.DeliveryMethod.ShortName))
				.ForMember(D => D.DeliveryMethodCost, O => O.MapFrom(S => S.DeliveryMethod.Cost));

			CreateMap<OrderItem, OrderItemDto>()
				.ForMember(D => D.ProductId, O => O.MapFrom(S => S.Product.ProductId))
				.ForMember(D => D.ProductName, O => O.MapFrom(S => S.Product.ProductName))
				.ForMember(D => D.PictureUrl, O => O.MapFrom(S => S.Product.PictureUrl))
				.ForMember(D => D.PictureUrl, O => O.MapFrom<OrderItemPictureUrlResolver>());
		}
    }
}
