using ImageDetectionApi.Data;
using ImageDetectionApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<ActionResult<IEnumerable<Detection>>> GetDetections(string videoName = null, string status = null, string className = null, int? criticalLevel = null)
        {
            var query = _context.Detections.AsQueryable();

            if (!string.IsNullOrEmpty(videoName))
            {
                query = query.Where(d => d.VideoName.Contains(videoName));
            }
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(d => d.Status == status);
            }
            if (!string.IsNullOrEmpty(className))
            {
                query = query.Where(d => d.ClassName == className);
            }
            if (criticalLevel.HasValue)
            {
                query = query.Where(d => d.CriticalLevel == criticalLevel.Value);
            }

            var detections = await query.OrderByDescending(d => d.Id).ToListAsync();
            return Ok(detections);
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
