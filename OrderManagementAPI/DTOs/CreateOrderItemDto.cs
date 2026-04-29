using System.ComponentModel.DataAnnotations;

namespace OrderManagementAPI.DTOs
{
    public class CreateOrderItemDto
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }
        [Range(1, 9999)]
        public int Quantity { get; set; }
    }
}
