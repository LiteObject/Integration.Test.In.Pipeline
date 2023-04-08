using System.ComponentModel.DataAnnotations;

namespace Demo.Product.Api.Domain
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}