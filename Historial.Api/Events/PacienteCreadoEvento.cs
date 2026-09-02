namespace Historial.Api.Events
{
    public class PacienteCreadoEvento
    {
        public int IdPaciente { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Cedula { get; set; }
    }
}