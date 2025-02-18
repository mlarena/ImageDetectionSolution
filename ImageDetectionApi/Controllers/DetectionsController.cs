using ImageDetectionApi.Data;
using ImageDetectionApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImageDetectionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetectionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DetectionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Detection>>> GetDetections()
        {
            return await _context.Detections.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Detection>> GetDetection(int id)
        {
            var detection = await _context.Detections.FindAsync(id);

            if (detection == null)
            {
                return NotFound();
            }

            return detection;
        }

        [HttpPost]
        public async Task<ActionResult<Detection>> PostDetection(Detection detection)
        {
            _context.Detections.Add(detection);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDetection), new { id = detection.Id }, detection);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDetection(int id, Detection detection)
        {
            if (id != detection.Id)
            {
                return BadRequest();
            }

            _context.Entry(detection).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DetectionExists(id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetection(int id)
        {
            var detection = await _context.Detections.FindAsync(id);
            if (detection == null)
            {
                return NotFound();
            }

            _context.Detections.Remove(detection);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DetectionExists(int id)
        {
            return _context.Detections.Any(e => e.Id == id);
        }
    }
}
