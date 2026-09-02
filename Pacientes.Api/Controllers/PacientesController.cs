using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pacientes.Api.Data;
using Pacientes.Api.Models;
using Pacientes.Api.Services;

namespace Pacientes.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacientesController : ControllerBase
    {
        private readonly PacientesDBContext _dbContext;
        private readonly RabbitMQPublisher _rabbitMQPublisher;

        public PacientesController(PacientesDBContext dbContext, RabbitMQPublisher rabbitMQPublisher)
        {
            _dbContext = dbContext;
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Paciente>>> GetPacientes()
        {
            var pacientes = await _dbContext.Pacientes
                .AsNoTracking()
                .ToListAsync();
            return Ok(pacientes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Paciente>> GetPaciente(int id)
        {
            var paciente = await _dbContext.Pacientes
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdPaciente == id);

            if (paciente == null)
                return NotFound();

            return Ok(paciente);
        }

        [HttpPost]
        public async Task<ActionResult<Paciente>> CrearPaciente(Paciente paciente)
        {
            _dbContext.Pacientes.Add(paciente);
            await _dbContext.SaveChangesAsync();

            
            _rabbitMQPublisher.PublicarPacienteCreado(paciente); 

            return CreatedAtAction(nameof(GetPaciente),
                new { id = paciente.IdPaciente }, paciente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarPaciente(int id, Paciente paciente)
        {
            if (id != paciente.IdPaciente)
                return BadRequest();

            _dbContext.Entry(paciente).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarPaciente(int id)
        {
            var paciente = await _dbContext.Pacientes.FindAsync(id);

            if (paciente == null)
                return NotFound();

            _dbContext.Pacientes.Remove(paciente);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}