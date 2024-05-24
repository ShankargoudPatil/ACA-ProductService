namespace Application.Features.Products.DTOs;
public  record GetProductsDto:ProductDto
{  
    public Guid Id { get; init; }
}
