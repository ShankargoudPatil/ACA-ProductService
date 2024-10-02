using System;
using Domain.Entities;
using Domain.Persistence;
namespace WebApi.GraphQL;
public class ProductBatchDataLoader : GroupedDataLoader<string, Product>
{
    private readonly IProductRepository _repository;

    public ProductBatchDataLoader(
        IProductRepository repository,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _repository = repository;
    }

    protected override async Task<ILookup<string, Product>> LoadGroupedBatchAsync(
        IReadOnlyList<string> names,
        CancellationToken cancellationToken)
    {
        var persons = await _repository.GetAllAsync(cancellationToken);
        return persons.ToLookup(x => x.SerialNumber);
    }
}
