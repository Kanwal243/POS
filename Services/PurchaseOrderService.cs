using EyeHospitalPOS.Data;
using EyeHospitalPOS.Models;
using EyeHospitalPOS.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EyeHospitalPOS.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly ApplicationDbContext _context;

        public PurchaseOrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrder po)
        {
            po.PONumber = GeneratePONumber();
            po.Status = PurchaseOrderStatus.Draft;
            po.TotalAmount = po.PurchaseItems?.Sum(i => i.Quantity * i.UnitPrice) ?? 0;

            _context.PurchaseOrders.Add(po);
            await _context.SaveChangesAsync();
            return po;
        }

        public async Task<PurchaseOrder> UpdatePurchaseOrderAsync(PurchaseOrder po)
        {
            po.TotalAmount = po.PurchaseItems?.Sum(i => i.Quantity * i.UnitPrice) ?? 0;
            _context.PurchaseOrders.Update(po);
            await _context.SaveChangesAsync();
            return po;
        }

        public async Task<PurchaseOrder> ActivatePurchaseOrderAsync(int poId, string userId)
        {
            var po = await _context.PurchaseOrders.FindAsync(poId);
            if (po == null) throw new Exception("Purchase Order not found");

            po.Status = PurchaseOrderStatus.Active;
            po.ApprovedBy = userId;
            po.ApprovedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return po;
        }

        public async Task<PurchaseOrder> CancelPurchaseOrderAsync(int poId, string userId)
        {
            var po = await _context.PurchaseOrders.FindAsync(poId);
            if (po == null) throw new Exception("Purchase Order not found");

            po.Status = PurchaseOrderStatus.Cancelled;
            po.IsCancelled = true;
            
            await _context.SaveChangesAsync();
            return po;
        }

        public async Task<PurchaseOrder> CompletePurchaseOrderAsync(int poId, string userId)
        {
            var po = await _context.PurchaseOrders.FindAsync(poId);
            if (po == null) throw new Exception("Purchase Order not found");

            po.Status = PurchaseOrderStatus.Completed;
            
            await _context.SaveChangesAsync();
            return po;
        }

        public async Task<List<PurchaseOrder>> GetAllPurchaseOrdersAsync()
        {
            return await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Product)
                .OrderByDescending(p => p.OrderDate)
                .ToListAsync();
        }

        public async Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int id)
        {
            return await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.ApprovedByUser)
                .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Product)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<PurchaseOrder>> GetPurchaseOrdersBySupplierAsync(int supplierId)
        {
            return await _context.PurchaseOrders
                .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Product)
                .Where(p => p.SupplierId == supplierId)
                .OrderByDescending(p => p.OrderDate)
                .ToListAsync();
        }

        public string GeneratePONumber()
        {
            var today = DateTime.Today;
            var prefix = $"PO{today:yyyyMMdd}";

            var lastPO = _context.PurchaseOrders
                .Where(p => p.PONumber.StartsWith(prefix))
                .OrderByDescending(p => p.PONumber)
                .FirstOrDefault();

            if (lastPO == null)
            {
                return $"{prefix}001";
            }

            var lastNumber = int.Parse(lastPO.PONumber.Substring(prefix.Length));
            return $"{prefix}{(lastNumber + 1):D3}";
        }

        public async Task<decimal> GetTotalPurchaseAmountAsync()
        {
            return await _context.PurchaseOrders
                .Where(p => p.Status == PurchaseOrderStatus.Active) // Only approved POs? Or all? Prototype says "Purchase Amt". Let's assume all or approved. Approved is safer.
                .SumAsync(p => p.TotalAmount);
        }
    }
}
