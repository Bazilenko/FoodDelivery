using AutoMapper;
using Catalog.Dal.Entities;

namespace Catalog.Bll.Mapping.Extensions
{
    public static class AutoMapperExtensions{
        public static IMappingExpression<TSource, TDest> IgnoreBaseEntityProperties<TSource, TDest>(
            this IMappingExpression<TSource, TDest> expression) 
            where TDest : BaseEntity
        {
            expression.ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                    .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
            return expression;
        }
        public static IMappingExpression<TSource, TDest> IgnoreAuditProperties<TSource, TDest>(
            this IMappingExpression<TSource, TDest> expression)
            where TDest : BaseEntity
        {
            return expression
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }
    }
}