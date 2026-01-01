using EyeHospitalPOS.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EyeHospitalPOS.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrder po);
        Task<PurchaseOrder> UpdatePurchaseOrderAsync(PurchaseOrder po);
        Task<PurchaseOrder> ActivatePurchaseOrderAsync(int poId, string userId);
        Task<PurchaseOrder> CancelPurchaseOrderAsync(int poId, string userId);
        Task<PurchaseOrder> CompletePurchaseOrderAsync(int poId, string userId);
        Task<List<PurchaseOrder>> GetAllPurchaseOrdersAsync();
        Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int id);
        Task<List<PurchaseOrder>> GetPurchaseOrdersBySupplierAsync(int supplierId);
        Task<decimal> GetTotalPurchaseAmountAsync();
        string GeneratePONumber();
    }
}
