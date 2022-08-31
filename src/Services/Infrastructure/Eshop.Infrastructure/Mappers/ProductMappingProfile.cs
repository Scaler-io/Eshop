using AutoMapper;
using Eshop.Infrastructure.Commands.Product;
using Eshop.Infrastructure.Events.Product;
using System;

namespace Eshop.Infrastructure.Mappers
{
    public class ProductMappingProfile: Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<CreateProduct, ProductCreated>()
                .ForMember(p => p.CreatedAt, o => o.MapFrom(d => DateTime.UtcNow));
        }
    }
}
