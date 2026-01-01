using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EyeHospitalPOS.Services;
using EyeHospitalPOS.Models;
using EyeHospitalPOS.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Security.Claims;

namespace EyeHospitalPOS.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<InventoryReceiving>>> GetAll()
        {
            try
            {
                var items = await _inventoryService.GetAllInventoryReceivingsAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryReceiving>> Get(int id)
        {
            try
            {
                var item = await _inventoryService.GetInventoryReceivingByIdAsync(id);
                if (item == null)
                {
                    return NotFound();
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<InventoryReceiving>> Create([FromBody] InventoryReceiving ir)
        {
            if (ir == null) return BadRequest();
            try
            {
                var created = await _inventoryService.CreateInventoryReceivingAsync(ir);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating inventory receiving: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<InventoryReceiving>> Update(int id, [FromBody] InventoryReceiving ir)
        {
            if (ir == null || ir.Id != id) return BadRequest();
            try
            {
                var updated = await _inventoryService.UpdateInventoryReceivingAsync(ir);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating inventory receiving: {ex.Message}");
            }
        }

        [HttpPost("{id}/activate")]
        public async Task<ActionResult<InventoryReceiving>> Activate(int id, [FromBody] dynamic request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                var result = await _inventoryService.ActivateInventoryReceivingAsync(id, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error activating inventory receiving: {ex.Message}");
            }
        }

        [HttpPost("{id}/deactivate")]
        public async Task<ActionResult<InventoryReceiving>> Deactivate(int id, [FromBody] dynamic request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                var result = await _inventoryService.DeactivateInventoryReceivingAsync(id, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deactivating inventory receiving: {ex.Message}");
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> Cancel(int id, [FromBody] dynamic request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                await _inventoryService.CancelInventoryReceivingAsync(id, userId);
                return Ok(new { message = "Inventory receiving cancelled successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error cancelling inventory receiving: {ex.Message}");
            }
        }
    }
}

