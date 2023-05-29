using API.FurnitureStore.Data;
using API.FurnitureStore.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.FurnitureStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        //Dependencia del context, dbcontext
        private readonly APIFurnitureStoreContext _context;

        //Constructor de context para injectar la dependencia
        public ClientsController(APIFurnitureStoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        //Se crea el endpoint asincrono porque puede llegar a demorar
        //cuando quiere ingresar a la base de datos
        public async Task<IEnumerable<Client>> Get()
        {
            return await _context.Clients.ToListAsync();
        }
        //Si no encuentra el cliente con firstordefault devuelve = null
        //con single o default devuelve un error
        //IActionResult permite devolver respuestas http responses
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails (int id)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
            //not found es un http response devuelve un 404
            if (client == null) return NotFound();

            return Ok(client);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Client client)
        {

            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();

            //Produce un 201 que es un CREATED
            return CreatedAtAction("Post", client.Id, client);
        }

        [HttpPut]

        public async Task<IActionResult> Put(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]

        public async Task<IActionResult> Delete(Client client)
        {
            if (client == null) return NotFound();

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
