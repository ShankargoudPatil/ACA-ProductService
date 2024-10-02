using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Common;

namespace Domain.Entities;

[Table("Products", Schema = "ProductDb")]
public  class Product:BaseClass
{
    public string Name { get; set; }=string.Empty;
    public string SerialNumber { get; set; }=string.Empty;
    public string Description { get; set; }=string.Empty;
    public decimal Price { get; set; }
    public DateTime DateTime { get; set; }
    public Category? Category { get; set; }
    public Manufacturer? Manufacturer { get; set; }
}

