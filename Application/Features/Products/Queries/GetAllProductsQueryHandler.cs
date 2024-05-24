using Application.Features.Products.DTOs;
using Domain.Persistence;
using MediatR;

namespace Application.Features.Products.Queries;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<GetProductsDto>>
{
    private readonly IProductRepository _productRepository;
    public GetAllProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    public async Task<IEnumerable<GetProductsDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _productRepository.GetProductsAsync(cancellationToken);

            return products.Select(p => new GetProductsDto
            {
                Id = p.Id,
                Name = p.Name,
                SerialNumber = p.SerialNumber,
                Description = p.Description,
                Price = p.Price,
                DateTime = p.DateTime
            });
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}