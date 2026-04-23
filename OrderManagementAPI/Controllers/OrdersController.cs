using Microsoft.AspNetCore.Mvc;
using OrderManagementAPI.Data;
using OrderManagementAPI.DTOs;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
        {
            decimal total = 0;

            var order = new OrderModel
            {
                OrderDate = DateTime.Now,
                OrderItems = new List<OrderItemModel>()
            };

            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);

                if (product == null)
                    return BadRequest($"Product {item.ProductId} not found.");

                if (product.Stock < item.Quantity)
                    return BadRequest($"Not enough stock for {product.Name}");

                product.Stock -= item.Quantity;

                total += product.Price * item.Quantity;

                order.OrderItems.Add(new OrderItemModel
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    Price = product.Price
                });
            }

            order.TotalAmount = total;

            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            return Ok(order);
        }
    }
}