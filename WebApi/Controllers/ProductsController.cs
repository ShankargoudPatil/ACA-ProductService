using Application.Features.Products.Commands;
using Application.Features.Products.DTOs;
using Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.ProblemResponse;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IResult> CreateProduct([FromBody] ProductDto product,CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(product);

        var result = await _mediator.Send(command, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Results.Created("", result.Value);
        }

        return result.ToProblemDetails();
    }

    [HttpGet]
    public async Task<IResult> GetAllProducts(CancellationToken cancellationToken)
    {
        var query = new GetAllProductsQuery();

        var result= await _mediator.Send(query, cancellationToken);
        if(!result.Any())
        {
            return Results.NoContent();
        }
        return Results.Ok(result);
    }
}
