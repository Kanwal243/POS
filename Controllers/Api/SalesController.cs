using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EyeHospitalPOS.Services;
using EyeHospitalPOS.Models;
using EyeHospitalPOS.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace EyeHospitalPOS.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _salesService;

        public SalesController(ISalesService salesService)
        {
            _salesService = salesService;
        }

        [HttpPost]
        public async Task<ActionResult<Sale>> CreateSale([FromBody] Sale sale)
        {
            if (sale == null) return BadRequest();
            try
            {
                var created = await _salesService.CreateSaleAsync(sale);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating sale: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Sale>> GetById(int id)
        {
            var sale = await _salesService.GetSaleByIdAsync(id);
            if (sale == null) return NotFound();
            return Ok(sale);
        }

        [HttpGet("by-date")]
        public async Task<ActionResult<List<Sale>>> GetByDate([FromQuery] DateTime date)
        {
            return Ok(await _salesService.GetSalesByDateAsync(date));
        }

        [HttpGet("range")]
        public async Task<ActionResult<List<Sale>>> GetByRange([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            return Ok(await _salesService.GetSalesByDateRangeAsync(start, end));
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<Sale>>> GetByUser(string userId)
        {
            // Optional: Check if requesting user matches userId or is admin
            return Ok(await _salesService.GetSalesByUserAsync(userId));
        }

        [HttpGet("today/total")]
        public async Task<ActionResult<decimal>> GetTodayTotal()
        {
            return Ok(await _salesService.GetTodaysTotalSalesAsync());
        }

        [HttpGet("today/count")]
        public async Task<ActionResult<int>> GetTodayCount()
        {
            return Ok(await _salesService.GetTodaysTotalOrdersAsync());
        }

        [HttpGet("invoice-number")]
        public async Task<ActionResult<string>> GetNextInvoice()
        {
            return Ok(_salesService.GenerateInvoiceNumber());
        }
    }
}
