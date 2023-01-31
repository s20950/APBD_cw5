using System;
using System.Threading.Tasks;
using Cw5.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cw5.Services
{
    public interface IDatabaseService
    {
        public Task<bool> ProductExistsAsync(int idProduct);
        public Task<bool> WarehouseExistsAsync(int idWarehouse);
        public Task<bool> OrderExistsAsync(int IdProduct, int Amount);
        public Task<bool> CheckDateAsync(DateTime date, int IdProduct, int Amount);
        public Task<bool> IsOrderCompletedAsync(int IdProduct, int Amount);
        public Task<int> UpdateFulfilledAtAsync(int IdProduct, int Amount);
        public Task<int> CreateProductWarehouseAsync(int IdProduct, int IdWarehouse, int Amount);
        public Task<int> CreateProductWarehouseWithProcedureAsync(int IdProduct, int IdWarehouse, int Amount);
        public Task<int> GetProductWarehouseIdAsync(int IdProduct, int IdWarehouse, int Amount);
    }
}