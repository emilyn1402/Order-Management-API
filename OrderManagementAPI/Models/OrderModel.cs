namespace OrderManagementAPI.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public List<OrderItemModel> OrderItems { get; set; } = new();
    }
}
