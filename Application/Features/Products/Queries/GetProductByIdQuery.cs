using Application.Features.Products.DTOs;
using Application.Shared.Responses;
using MediatR;
namespace Application.Features.Products.Queries;
public  class GetProductByIdQuery : IRequest<Result<GetProductsDto>>
{
    public Guid Id { get; set; }
    public GetProductByIdQuery(Guid id)
    {
        Id = id;
    }
}
