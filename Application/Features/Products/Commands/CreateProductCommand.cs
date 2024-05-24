using Application.Features.Products.DTOs;
using Application.Shared.Responses;
using MediatR;

namespace Application.Features.Products.Commands;
public record  CreateProductCommand(ProductDto ProductDto) : IRequest<Result<Guid>>
{

}

