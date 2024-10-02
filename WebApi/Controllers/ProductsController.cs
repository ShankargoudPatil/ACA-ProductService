using Application.Features.Products.Commands;
using Application.Features.Products.DTOs;
using Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.ProblemResponse;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly HttpContext _httpContext;
    private const string JwtBearerClientCredSchemes = JwtBearerDefaults.AuthenticationScheme + "," +
           "ZitaDelBearer" + "," + "Introspection" + "," + "ClientCredentials";
    public ProductsController(IMediator mediator, IHttpContextAccessor httpContextAccessor)
    {
        _mediator = mediator;
        _httpContext = httpContextAccessor.HttpContext;
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
    [Authorize(Roles = "admin,superadmin")]
    [Authorize(AuthenticationSchemes = "DynamicScheme")]
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

    [HttpGet("{id}")]
    public async Task<IResult> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery(id);

        var result = await _mediator.Send(query, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }
        
        return result.ToProblemDetails();
      
    }
}
