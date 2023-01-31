using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw5.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cw5.Services
{
    public class WarehouseSqlServerDatabaseService : IDatabaseService
    {
            private readonly IConfiguration _configuration;
            private readonly string _connString;

            public WarehouseSqlServerDatabaseService(IConfiguration configuration)
            {
                _configuration = configuration;
                _connString = _configuration.GetConnectionString("ProductionDb");
            }

            public async Task<bool> ProductExistsAsync(int idProduct)
            {
                using (SqlConnection con = new SqlConnection(_connString))
                {
                    SqlCommand com = new SqlCommand();
                    await con.OpenAsync();
                    com.Connection = con;
                    com.CommandText = "SELECT COUNT(idProduct) FROM Product WHERE idProduct =@idProduct";
                    com.Parameters.AddWithValue("@idProduct", idProduct);
                    int exists = (int)(await com.ExecuteScalarAsync())!;
                    if (exists == 0) return false; else return true;
                }
            }
            public async Task<bool> WarehouseExistsAsync(int idWarehouse)
            {
                using (SqlConnection con = new SqlConnection(_connString))
                {
                    SqlCommand com = new SqlCommand();
                    await con.OpenAsync();
                    com.Connection = con;
                    com.CommandText = "SELECT COUNT(idWarehouse) FROM Warehouse WHERE idWarehouse =@idWarehouse";
                    com.Parameters.AddWithValue("idWarehouse", idWarehouse);
                    var result = await com.ExecuteScalarAsync();
                    return result != null;
                }
            }
            public async Task<bool> OrderExistsAsync(int IdProduct, int Amount)
            {
                using (SqlConnection con = new SqlConnection(_connString))
                {
                    SqlCommand com = new SqlCommand();
                    await con.OpenAsync();
                    com.Connection = con;
                    com.CommandText = "SELECT COUNT(idOrder) FROM [Order] WHERE IdProduct =@IdProduct AND Amount =@Amount";
                    com.Parameters.AddWithValue("IdProduct", IdProduct);
                    com.Parameters.AddWithValue("Amount", Amount);
                    int result = (int)(await com.ExecuteScalarAsync())!;
                    return result > 0;
                }
            }
            public async Task<bool> CheckDateAsync(DateTime date, int IdProduct, int Amount)
            {
                using (SqlConnection con = new SqlConnection(_connString))
                {
                    SqlCommand com = new SqlCommand();
                    await con.OpenAsync();
                    com.Connection = con;
                    com.CommandText = "SELECT CreatedAt FROM [Order] WHERE IdProduct =@IdProduct AND Amount =@Amount";
                    com.Parameters.AddWithValue("IdProduct", IdProduct);
                    com.Parameters.AddWithValue("Amount", Amount);
                    return (DateTime)(await com.ExecuteScalarAsync())! > date;
                }
            }
            public async Task<bool> IsOrderCompletedAsync(int IdProduct, int Amount)
            {
                using (SqlConnection con = new SqlConnection(_connString))
                {
                    SqlCommand com = new SqlCommand();
                    com.Connection = con;
                    con.OpenAsync();
                    com.CommandText = "SELECT count(IdProductWarehouse) FROM Product_Warehouse WHERE IdOrder = (SELECT IdOrder FROM [Order] WHERE IdProduct =@IdProduct AND Amount =@Amount)";
                    com.Parameters.AddWithValue("IdProduct", IdProduct);
                    com.Parameters.AddWithValue("Amount", Amount);
                    int result = (int)await com.ExecuteScalarAsync();
                    return result > 0;
                }
            }
            public async Task<int> UpdateFulfilledAtAsync(int IdProduct, int Amount)
            {
                using (SqlConnection con = new SqlConnection(_connString))
                {
                    SqlCommand com = new SqlCommand();
                    await con.OpenAsync();
                    com.Connection = con;
                    com.CommandText = "UPDATE [Order] SET FulfilledAt = GETDATE() WHERE IdOrder = (SELECT idOrder FROM [Order] WHERE IdProduct =@IdProduct AND Amount =@Amount)";
                    com.Parameters.AddWithValue("IdProduct", IdProduct);
                    com.Parameters.AddWithValue("Amount", Amount);
                    int rowsAffected = (int)await com.ExecuteNonQueryAsync();
                    return rowsAffected;
                }
            }
            public async Task<int> CreateProductWarehouseAsync(int IdProduct, int IdWarehouse, int Amount)
            {
                using (SqlConnection con = new SqlConnection(_connString))
                {
                    SqlCommand com = new SqlCommand();
                    await con.OpenAsync();
                    com.Connection = con;
                    com.CommandText = "SET IDENTITY_INSERT Product_Warehouse ON;INSERT INTO Product_Warehouse (IdProductWarehouse, IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)VALUES (((SELECT ISNULL(Max(IdProductWarehouse),0) FROM Product_Warehouse) + 1), @IdWarehouse, @IdProduct,(SELECT idOrder FROM [Order] WHERE IdProduct =@IdProduct AND Amount =@Amount),@Amount, ((SELECT Price FROM Product WHERE IdProduct = @IdProduct) * @Amount), GETDATE());SET IDENTITY_INSERT Product_Warehouse OFF";
                    com.Parameters.AddWithValue("IdProduct", IdProduct);
                    com.Parameters.AddWithValue("Amount", Amount);
                    com.Parameters.AddWithValue("IdWarehouse", IdWarehouse);
                    com.ExecuteNonQuery();
                    com.CommandText = "SELECT IdProductWarehouse FROM Product_Warehouse WHERE IdProduct = @IdProduct AND IdWarehouse = @IdWarehouse AND Amount = @Amount AND Price = ((SELECT Price FROM Product WHERE IdProduct = @IdProduct) * @Amount)";
                    int id = (int)await com.ExecuteScalarAsync();
                    return id;
                }
            }
            public async Task<int> CreateProductWarehouseWithProcedureAsync(int IdProduct, int IdWarehouse, int Amount)
            {
                using (SqlConnection con = new SqlConnection(_connString))
                {
                    SqlCommand com = new SqlCommand("AddProductToWarehouse", con);
                    await con.OpenAsync();
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("IdProduct", IdProduct);
                    com.Parameters.AddWithValue("Amount", Amount);
                    com.Parameters.AddWithValue("IdWarehouse", IdWarehouse);
                    com.Parameters.AddWithValue("CreatedAt", DateTime.Now);
                    int rowsAffected = (int) await com.ExecuteNonQueryAsync();
                    return rowsAffected;
                }
            }
            public async Task<int> GetProductWarehouseIdAsync(int IdProduct, int IdWarehouse, int Amount)
            {
                using (SqlConnection con = new SqlConnection(_connString))
                {
                    SqlCommand com = new SqlCommand();
                    com.Connection = con;
                    await  con.OpenAsync();
                    com.CommandText = "SELECT IdProductWarehouse FROM Product_Warehouse WHERE IdProduct = @IdProduct AND IdWarehouse = @IdWarehouse AND Amount = @Amount AND Price = ((SELECT Price FROM Product WHERE IdProduct = @IdProduct) * @Amount)";
                    com.Parameters.AddWithValue("IdProduct", IdProduct);
                    com.Parameters.AddWithValue("Amount", Amount);
                    com.Parameters.AddWithValue("IdWarehouse", IdWarehouse);
                    int id = (int)(await com.ExecuteScalarAsync())!;
                    return id;
                }
            }
    }
}