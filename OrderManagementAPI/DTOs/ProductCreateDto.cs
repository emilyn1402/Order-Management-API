using System.ComponentModel.DataAnnotations;

namespace OrderManagementAPI.DTOs
{
    public class ProductCreateDto
    {
        [Required]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock {  get; set; }
    }
}
