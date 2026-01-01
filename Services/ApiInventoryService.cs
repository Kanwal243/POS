using EyeHospitalPOS.Interfaces;
using EyeHospitalPOS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EyeHospitalPOS.Services
{
    /// <summary>
    /// HttpClient-based API client for Inventory operations
    /// </summary>
    public class ApiInventoryService : ApiClientService, IInventoryService
    {
        public ApiInventoryService(HttpClient httpClient, IJSRuntime jsRuntime, IHttpContextAccessor httpContextAccessor, Helper.LoginManager loginManager) 
            : base(httpClient, jsRuntime, httpContextAccessor, loginManager)
        {
        }

        public async Task<InventoryReceiving> CreateInventoryReceivingAsync(InventoryReceiving ir)
        {
            return await PostAsync<InventoryReceiving, InventoryReceiving>("api/Inventory", ir) 
                ?? throw new Exception("Failed to create inventory receiving");
        }

        public async Task<InventoryReceiving> UpdateInventoryReceivingAsync(InventoryReceiving ir)
        {
            return await PutAsync<InventoryReceiving, InventoryReceiving>($"api/Inventory/{ir.Id}", ir) 
                ?? throw new Exception("Failed to update inventory receiving");
        }

        public async Task<InventoryReceiving> ActivateInventoryReceivingAsync(int irId, string userId)
        {
            return await PostAsync<object, InventoryReceiving>($"api/Inventory/{irId}/activate", new { userId }) 
                ?? throw new Exception("Failed to activate inventory receiving");
        }

        public async Task<InventoryReceiving> DeactivateInventoryReceivingAsync(int irId, string userId)
        {
            return await PostAsync<object, InventoryReceiving>($"api/Inventory/{irId}/deactivate", new { userId }) 
                ?? throw new Exception("Failed to deactivate inventory receiving");
        }

        public async Task<bool> CancelInventoryReceivingAsync(int irId, string userId)
        {
            await PostAsync<object, object>($"api/Inventory/{irId}/cancel", new { userId });
            return true;
        }

        public async Task<List<InventoryReceiving>> GetAllInventoryReceivingsAsync()
        {
            return await GetAsync<List<InventoryReceiving>>("api/Inventory") 
                ?? new List<InventoryReceiving>();
        }

        public async Task<InventoryReceiving?> GetInventoryReceivingByIdAsync(int id)
        {
            return await GetAsync<InventoryReceiving>($"api/Inventory/{id}");
        }

        public string GenerateIRNumber() => 
            $"IR-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
    }
}

