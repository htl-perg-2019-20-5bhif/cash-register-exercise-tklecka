using cash_register.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cash_register.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptLinesController : ControllerBase
    {
        private readonly CashRegisterDataContext _context;

        public ReceiptLinesController(CashRegisterDataContext context)
        {
            _context = context;
        }

        // GET: api/ReceiptLines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReceiptLine>>> GetReceiptLines()
        {
            return await _context.ReceiptLines.Include(r => r.Product).ToListAsync();
        }

        // GET: api/ReceiptLines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReceiptLine>> GetReceiptLine(int id)
        {
            var receiptLine = await _context.ReceiptLines.Where(r => r.ID == id).Include(r => r.Product).FirstOrDefaultAsync();

            if (receiptLine == null)
            {
                return NotFound();
            }

            return receiptLine;
        }

        // PUT: api/ReceiptLines/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReceiptLine(int id, ReceiptLine receiptLine)
        {
            if (id != receiptLine.ID)
            {
                return BadRequest();
            }

            _context.Entry(receiptLine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReceiptLineExists(id))
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

        // POST: api/ReceiptLines
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<ReceiptLine>> PostReceiptLine(ReceiptLine receiptLine)
        {
            _context.ReceiptLines.Add(receiptLine);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReceiptLine", new { id = receiptLine.ID }, receiptLine);
        }

        // DELETE: api/ReceiptLines/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ReceiptLine>> DeleteReceiptLine(int id)
        {
            var receiptLine = await _context.ReceiptLines.FindAsync(id);
            if (receiptLine == null)
            {
                return NotFound();
            }

            _context.ReceiptLines.Remove(receiptLine);
            await _context.SaveChangesAsync();

            return receiptLine;
        }

        private bool ReceiptLineExists(int id)
        {
            return _context.ReceiptLines.Any(e => e.ID == id);
        }
    }
}
