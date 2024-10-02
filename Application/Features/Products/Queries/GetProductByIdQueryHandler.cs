using Application.Features.Products.DTOs;
using Application.Shared.Responses;
using Domain.Persistence;
using MediatR;
namespace Application.Features.Products.Queries;

internal class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<GetProductsDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    public async Task<Result<GetProductsDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductByIdAsync(request.Id,cancellationToken);
       
        if (product == null)
            return Result.Failure<GetProductsDto>(ProductsErrors.NotFound);

        return  new GetProductsDto
        {
            Id = product.Id,
            Name = product.Name,
            SerialNumber = product.SerialNumber,
            Description = product.Description,
            Price = product.Price,
            DateTime = product.DateTime
        };
    }
}
