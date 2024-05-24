
namespace Application.Features.Products.DTOs
{
    public record ProductDto
    {
        public string Name { get; init; }=string.Empty;
        public string SerialNumber { get; init; }=string.Empty;
        public string Description { get; init; }=string.Empty;
        public decimal Price { get; init; }
        public DateTime DateTime { get; init; }

    }
}
