using System.Threading.Tasks;
using Cw5.Models;
using Cw5.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cw5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Warehouses2Controller : ControllerBase
    {
        private readonly IDatabaseService _dbService;

        public Warehouses2Controller(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductWarehouse(Warehouse warehouse)
        {
            await _dbService.CreateProductWarehouseWithProcedureAsync(warehouse.IdProduct, warehouse.IdWarehouse, warehouse.Amount);
            int id = await _dbService.GetProductWarehouseIdAsync(warehouse.IdProduct, warehouse.IdWarehouse, warehouse.Amount);
            return Created("",$"Record with {id} added");
        }
    }
}