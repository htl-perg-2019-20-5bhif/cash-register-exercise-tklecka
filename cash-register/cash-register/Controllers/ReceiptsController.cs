using cash_register.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace cash_register.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptsController : ControllerBase
    {
        private readonly CashRegisterDataContext _context;

        public ReceiptsController(CashRegisterDataContext context)
        {
            _context = context;
        }

        // GET: api/Receipts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Receipt>>> GetReceipts()
        {
            return await _context.Receipts.Include(r => r.Lines).ToListAsync();
        }

        // GET: api/Receipts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Receipt>> GetReceipt(int id)
        {
            var receipt = await _context.Receipts.FindAsync(id);

            if (receipt == null)
            {
                return NotFound();
            }

            return receipt;
        }

        // PUT: api/Receipts/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReceipt(int id, Receipt receipt)
        {
            if (id != receipt.ID)
            {
                return BadRequest();
            }

            _context.Entry(receipt).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReceiptExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Receipts
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        //[HttpPost]
        //public async Task<ActionResult<Receipt>> PostReceipt(Receipt receipt)
        //{
        //    _context.Receipts.Add(receipt);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetReceipt", new { id = receipt.ID }, receipt);
        //}

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]List<ReceiptLineDto> receiptLineDto)
        {
            if (receiptLineDto == null || receiptLineDto.Count == 0)
            {
                return BadRequest("Missing receipt lines");
            }

            // Read product data from DB for incoming product IDs
            var products = new Dictionary<int, Product>();
            foreach (var rl in receiptLineDto)
            {
                products[rl.ProductID] = await _context.Products.FirstOrDefaultAsync(p => p.ID == rl.ProductID);
                if (products[rl.ProductID] == null)
                {
                    return BadRequest($"Unknown product ID {rl.ProductID}");
                }
            }

            // Build receipt from DTO
            var newReceipt = new Receipt
            {
                Timestamp = DateTime.UtcNow,
                Lines = receiptLineDto.Select(rl => new ReceiptLine
                {
                    ID = 0,
                    Product = products[rl.ProductID],
                    Pieces = rl.Amount,
                    TotalPrice = rl.Amount * products[rl.ProductID].Price
                }).ToList()
            };
            newReceipt.TotalPrice = newReceipt.Lines.Sum(rl => rl.TotalPrice);

            await _context.Receipts.AddAsync(newReceipt);
            await _context.SaveChangesAsync();

            return StatusCode((int)HttpStatusCode.Created, newReceipt);
        }

        // DELETE: api/Receipts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Receipt>> DeleteReceipt(int id)
        {
            var receipt = await _context.Receipts.FindAsync(id);
            if (receipt == null)
            {
                return NotFound();
            }

            _context.Receipts.Remove(receipt);
            await _context.SaveChangesAsync();

            return receipt;
        }

        private bool ReceiptExists(int id)
        {
            return _context.Receipts.Any(e => e.ID == id);
        }
    }
}
