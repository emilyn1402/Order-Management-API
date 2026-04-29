using System.ComponentModel.DataAnnotations;

namespace OrderManagementAPI.DTOs
{
    public class ProductCreateDto
    {
        [Required]
        public string Name { get; set; }
        [Range(0.01, 999999)]
        public decimal Price { get; set; }
        [Range(0, 999999)]
        public int Stock {  get; set; }
    }
}
