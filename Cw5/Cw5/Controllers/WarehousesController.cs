using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw5.Models;
using Cw5.Services;
using static Cw5.Services.WarehouseSqlServerDatabaseService;

namespace Cw5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly IDatabaseService _dbService;

        public WarehousesController(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductWarehouse(Warehouse warehouse)
        {
            if (await _dbService.ProductExistsAsync(warehouse.IdProduct)
                && await _dbService.WarehouseExistsAsync(warehouse.IdWarehouse)
                && warehouse.Amount > 0)
            {
                if (await _dbService.OrderExistsAsync(warehouse.IdProduct, warehouse.Amount)
                    && await _dbService.CheckDateAsync(warehouse.CreatedAt, warehouse.IdProduct, warehouse.Amount))
                {
                    if (await _dbService.IsOrderCompletedAsync(warehouse.IdProduct, warehouse.Amount))
                    {
                        await _dbService.UpdateFulfilledAtAsync(warehouse.IdProduct, warehouse.Amount);
                        int id = await _dbService.CreateProductWarehouseAsync(warehouse.IdProduct, warehouse.IdWarehouse,
                            warehouse.Amount);
                        return Created("",$"Record with {id} added");
                    }
                    else return BadRequest("Order already completed.");
                }
                else return NotFound("Order was not found.");
            }
            else return NotFound("Product or warehouse doesn't exist");
        }
    }
}