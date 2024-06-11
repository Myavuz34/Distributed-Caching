using System.ComponentModel.DataAnnotations.Schema;

namespace DistributedCaching.Api.Models;

public class Product
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    
    [Column(TypeName = "money")]
    public decimal Price { get; set; }
}