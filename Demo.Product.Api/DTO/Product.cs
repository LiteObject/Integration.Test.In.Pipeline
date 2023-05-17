using System.ComponentModel.DataAnnotations;

namespace Demo.Product.Api.DTO
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal UnitPrice { get; set; }
    }
}