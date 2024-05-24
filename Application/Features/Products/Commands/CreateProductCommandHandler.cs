using Application.Shared.Responses;
using Domain.Entities;
using Domain.Persistence;
using MediatR;

namespace Application.Features.Products.Commands;
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                var existingCustomer = await _unitOfWork.Products
                    .GetByConditionAsync(c => c.SerialNumber == request.ProductDto.SerialNumber,cancellationToken);
               
                if (existingCustomer != null)
                {
                    return Result.Failure<Guid>(ProductsErrors.Conflict);
                }

                var product = new Product()
                {
                    Id = Guid.NewGuid(),
                    Name = request.ProductDto.Name,
                    SerialNumber= request.ProductDto.SerialNumber,
                    Description = request.ProductDto.Description,
                    Price = request.ProductDto.Price,
                    DateTime = DateTime.UtcNow
                };

                await _unitOfWork.Products.AddAsync(product, cancellationToken);

                await _unitOfWork.CompleteAsync();

                await transaction.CommitAsync(cancellationToken);

                return Result.Success<Guid>(product.Id);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
