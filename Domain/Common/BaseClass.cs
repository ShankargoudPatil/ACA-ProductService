using System.ComponentModel.DataAnnotations;
namespace Domain.Entities.Common;
public  class BaseClass
{
    [Key]
    public required Guid Id { get; set; }
}

//strongly typed primary key
//public record PrimaryKeyId(Guid value);
