using Historial.Api.Data;
using Historial.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Historial.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialController : ControllerBase
    {
        private readonly HistorialDBContext _dbContext;

        public HistorialController(HistorialDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistorialClinico>>> GetHistorial()
        {
            var historial = await _dbContext.HistorialClinico
                .AsNoTracking()
                .ToListAsync();
            return Ok(historial);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HistorialClinico>> GetHistorialPorId(int id)
        {
            var historial = await _dbContext.HistorialClinico
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.IdHistorial == id);

            if (historial == null)
                return NotFound();

            return Ok(historial);
        }

        [HttpGet("paciente/{idPaciente}")]
        public async Task<ActionResult<IEnumerable<HistorialClinico>>> GetHistorialPorPaciente(int idPaciente)
        {
            var historial = await _dbContext.HistorialClinico
                .AsNoTracking()
                .Where(h => h.IdPaciente == idPaciente)
                .ToListAsync();

            return Ok(historial);
        }

        [HttpPost]
        public async Task<ActionResult<HistorialClinico>> CrearHistorial(HistorialClinico historial)
        {
            _dbContext.HistorialClinico.Add(historial);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHistorialPorId),
                new { id = historial.IdHistorial }, historial);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarHistorial(int id, HistorialClinico historial)
        {
            if (id != historial.IdHistorial)
                return BadRequest();

            _dbContext.Entry(historial).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarHistorial(int id)
        {
            var historial = await _dbContext.HistorialClinico.FindAsync(id);

            if (historial == null)
                return NotFound();

            _dbContext.HistorialClinico.Remove(historial);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}