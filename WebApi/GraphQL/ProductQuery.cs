using System;
using System.Threading;
using Domain.Entities;
using Domain.Persistence;
using HotChocolate.Authorization;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
namespace WebApi.GraphQL;
public class ProductQuery 
{
    private readonly IProductRepository _productRepository;
    //public ProductQuery(IProductRepository productRepository)
    // {
    //  _productRepository = productRepository;
    //}

    [UsePaging(IncludeTotalCount = true)]
    // [UseFiltering]
   // [Authorize(Policy = "AtLeast21")]
    public async Task<IEnumerable<Product>> GetProducts(ProductRepository productRepository,
        CancellationToken cancellationToken)
    {
        // var result = await pGSqlDbContext.Products.ToListAsync(cancellationToken);
       var result = await productRepository.GetAllAsync(cancellationToken);  
        return result;
    }
    public async Task<Product> GetProductsById(Guid id,CancellationToken cancellationToken)
    {
        var result = await _productRepository.GetProductByIdAsync(id,cancellationToken);
      
        return result;
    }

    public async Task<IEnumerable<Product>> GetPersonBySerialNumber(
        string serialNumber,
        ProductBatchDataLoader dataLoader)
        => await dataLoader.LoadAsync(serialNumber);


}
