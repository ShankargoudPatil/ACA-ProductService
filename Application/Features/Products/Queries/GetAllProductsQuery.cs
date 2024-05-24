using Application.Features.Products.DTOs;
using MediatR;

namespace Application.Features.Products.Queries;
public  record GetAllProductsQuery:IRequest<IEnumerable<GetProductsDto>>
{
}
