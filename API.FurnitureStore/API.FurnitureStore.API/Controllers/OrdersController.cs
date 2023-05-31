using API.FurnitureStore.Data;
using API.FurnitureStore.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;

namespace API.FurnitureStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly APIFurnitureStoreContext _context;

        public OrdersController(APIFurnitureStoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task <IEnumerable<Order>> Get()
        {
            //Agrega los detalles de la lista incluidos en el DBSET orderdetails
            return await _context.Orders.Include(o => o.OrderDetails).ToListAsync();
        }

        [HttpGet("{id}")]

        public async Task <IActionResult> GetDetails (int id)
        {
            var order = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.Id == id);
            
            if (order == null) return NotFound();

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Order order)
        {
            if (order == null) return NotFound();


            if (order.OrderDetails == null)
                return BadRequest("El order tiene que tener al menos un detalle.");
            
            await _context.Orders.AddAsync(order);//insertó la orden
            await _context.OrderDetails.AddRangeAsync(order.OrderDetails);//insertó todos los detalles

            await _context.SaveChangesAsync();

            return CreatedAtAction("Post", order.Id, order);
        }

        [HttpPut]

        public async Task <IActionResult> Put(Order order)
        {
            if (order == null) return NotFound();
            if (order.Id <= 0) return NotFound();

            //busco la orden en la db con los detalles
            var existingOrder = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.Id == order.Id);
            if (existingOrder == null) return NotFound();

            //actualizo los datos de la orden
            existingOrder.OrderNumber = order.OrderNumber;
            existingOrder.OrderDate = order.OrderDate;
            existingOrder.DeliveryDate = order.DeliveryDate;
            existingOrder.ClientId = order.ClientId;

            //elimino todos los detalles para que no haya problema
            _context.OrderDetails.RemoveRange(existingOrder.OrderDetails);
            
            //agrega los detalles recibidos por parametro
            _context.Orders.Update(existingOrder);
            _context.OrderDetails.AddRange(order.OrderDetails);

            await _context.SaveChangesAsync();

            return NoContent();

        }

        [HttpDelete]

        public async Task<IActionResult> Delete(Order order)
        {
            if (order == null) return NotFound();

            var existinOrder = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o=> o.Id == order.Id);
            if (existinOrder == null) return NotFound();

            _context.OrderDetails.RemoveRange(existinOrder.OrderDetails);
            _context.Orders.Remove(existinOrder);

            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
