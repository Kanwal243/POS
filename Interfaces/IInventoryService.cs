using EyeHospitalPOS.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EyeHospitalPOS.Interfaces
{
    public interface IInventoryService
    {
        Task<InventoryReceiving> CreateInventoryReceivingAsync(InventoryReceiving ir);
        Task<InventoryReceiving> UpdateInventoryReceivingAsync(InventoryReceiving ir);
        Task<InventoryReceiving> ActivateInventoryReceivingAsync(int irId, string userId);
        Task<InventoryReceiving> DeactivateInventoryReceivingAsync(int irId, string userId);
        Task<bool> CancelInventoryReceivingAsync(int irId, string userId);
        Task<List<InventoryReceiving>> GetAllInventoryReceivingsAsync();
        Task<InventoryReceiving?> GetInventoryReceivingByIdAsync(int id);
        string GenerateIRNumber();
    }
}
