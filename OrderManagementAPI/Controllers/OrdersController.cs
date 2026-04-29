using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (dto.Items == null || !dto.Items.Any())
                {
                    return BadRequest("Order must contain at least one item.");
                }

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
                await transaction.CommitAsync();

                return Ok(order);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return StatusCode(500, "An error occurred while creating order.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Select(o => new OrderResponseDto
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,

                    Items = o.OrderItems.Select(i => new OrderItemResponseDto
                    {
                        ProductId = i.ProductId,
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.Id == id)
                .Select(o => new OrderResponseDto
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,

                    Items = o.OrderItems.Select(i => new OrderItemResponseDto
                    {
                        ProductId = i.ProductId,
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (order == null)
                return NotFound();

            return Ok(order);
        }
    }
}