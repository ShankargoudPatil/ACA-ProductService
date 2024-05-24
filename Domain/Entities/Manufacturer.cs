using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Common;

namespace Domain.Entities;

[Table("Manufacturers", Schema = "ProductDb")]
public  class Manufacturer:BaseClass
{
    [Required]
    public string Name { get; set; }=string.Empty;

    public string Description { get; set; } = string.Empty;
    public List<Product> Products { get; set; }
    public Manufacturer()
    {
        Products = new List<Product>();
    }
}
