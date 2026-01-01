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
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductService _productService;

        public InventoryService(ApplicationDbContext context, IProductService productService)
        {
            _context = context;
            _productService = productService;
        }

        public async Task<InventoryReceiving> CreateInventoryReceivingAsync(InventoryReceiving ir)
        {
            ir.IRNumber = GenerateIRNumber();
            ir.Status = InventoryStatus.Draft;
            ir.TotalAmount = ir.InventoryReceivingItems?.Sum(i => i.Subtotal) ?? 0;

            _context.InventoryReceivings.Add(ir);
            await _context.SaveChangesAsync();
            return ir;
        }

        public async Task<InventoryReceiving> UpdateInventoryReceivingAsync(InventoryReceiving ir)
        {
            ir.TotalAmount = ir.InventoryReceivingItems?.Sum(i => i.Subtotal) ?? 0;
            _context.InventoryReceivings.Update(ir);
            await _context.SaveChangesAsync();
            return ir;
        }

        public async Task<InventoryReceiving> ActivateInventoryReceivingAsync(int irId, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var ir = await _context.InventoryReceivings
                    .Include(i => i.InventoryReceivingItems)
                    .FirstOrDefaultAsync(i => i.Id == irId);

                if (ir == null)
                    throw new Exception("Inventory Receiving not found");

                if (ir.Status == InventoryStatus.Active)
                    throw new Exception("Inventory Receiving already active");

                if (ir.Status == InventoryStatus.Cancelled)
                    throw new Exception("Cannot activate a cancelled record");

                // Update status
                ir.Status = InventoryStatus.Active;
                ir.PostedBy = userId;
                ir.PostedDate = DateTime.Now;

                // Update stock for each item
                if (ir.InventoryReceivingItems != null)
                {
                    foreach (var item in ir.InventoryReceivingItems)
                    {
                        await _productService.UpdateStockAsync(item.ProductId, item.Quantity);
                    }

                    // Update PO Received Quantities if linked
                    if (ir.PurchaseOrderId.HasValue)
                    {
                        var po = await _context.PurchaseOrders
                            .Include(p => p.PurchaseItems)
                            .FirstOrDefaultAsync(p => p.Id == ir.PurchaseOrderId.Value);
                        
                        if (po != null && po.PurchaseItems != null)
                        {
                            foreach (var item in ir.InventoryReceivingItems)
                            {
                                var poItem = po.PurchaseItems.FirstOrDefault(pi => pi.ProductId == item.ProductId);
                                if (poItem != null)
                                {
                                    poItem.ReceivedQuantity += item.Quantity;
                                }
                            }

                            // Check if PO is fully received
                            bool allReceived = po.PurchaseItems.All(pi => pi.ReceivedQuantity >= pi.Quantity);
                            if (allReceived)
                            {
                                po.Status = PurchaseOrderStatus.Completed;
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ir;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<InventoryReceiving> DeactivateInventoryReceivingAsync(int irId, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var ir = await _context.InventoryReceivings
                    .Include(i => i.InventoryReceivingItems)
                    .FirstOrDefaultAsync(i => i.Id == irId);

                if (ir == null)
                    throw new Exception("Inventory Receiving not found");

                if (ir.Status != InventoryStatus.Active)
                    throw new Exception("Only active records can be deactivated");

                // Reverse stock
                if (ir.InventoryReceivingItems != null)
                {
                    foreach (var item in ir.InventoryReceivingItems)
                    {
                        // Pass negative quantity to reduce stock
                        await _productService.UpdateStockAsync(item.ProductId, -item.Quantity);
                    }
                }

                // Update status back to Draft
                ir.Status = InventoryStatus.Draft;
                // Optional: Clear posted info or keep it as history?
                // Keeping it might be confusing if they edit it. Let's clear it or leave it. 
                // The requirement doesn't specify, but usually "Draft" implies not posted.
                ir.PostedBy = null;
                ir.PostedDate = null;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ir;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> CancelInventoryReceivingAsync(int irId, string userId)
        {
            var ir = await _context.InventoryReceivings.FindAsync(irId);
            if (ir == null) throw new Exception("Record not found");

            if (ir.Status != InventoryStatus.Draft)
                throw new Exception("Only draft records can be cancelled");

            ir.Status = InventoryStatus.Cancelled;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<InventoryReceiving>> GetAllInventoryReceivingsAsync()
        {
            return await _context.InventoryReceivings
                .Include(i => i.Supplier)
                .Include(i => i.InventoryReceivingItems)
                .ThenInclude(i => i.Product)
                .OrderByDescending(i => i.ReceivingDate)
                .ToListAsync();
        }

        public async Task<InventoryReceiving?> GetInventoryReceivingByIdAsync(int id)
        {
            return await _context.InventoryReceivings
                .Include(i => i.Supplier)
                .Include(i => i.PostedByUser)
                .Include(i => i.InventoryReceivingItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public string GenerateIRNumber()
        {
            var today = DateTime.Today;
            var prefix = $"IR{today:yyyyMMdd}";

            var lastIR = _context.InventoryReceivings
                .Where(i => i.IRNumber.StartsWith(prefix))
                .OrderByDescending(i => i.IRNumber)
                .FirstOrDefault();

            if (lastIR == null)
            {
                return $"{prefix}001";
            }

            var lastNumber = int.Parse(lastIR.IRNumber.Substring(prefix.Length));
            return $"{prefix}{(lastNumber + 1):D3}";
        }
    }
}
