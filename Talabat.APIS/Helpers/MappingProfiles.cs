﻿using AutoMapper;
using Talabat.APIS.Dtos;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;

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
			CreateMap<Core.Entities.Order_Aggregate.Address, AddressDto>().ReverseMap();
			CreateMap<AddressDto, Core.Entities.Order_Aggregate.Address>();
		}
    }
}
