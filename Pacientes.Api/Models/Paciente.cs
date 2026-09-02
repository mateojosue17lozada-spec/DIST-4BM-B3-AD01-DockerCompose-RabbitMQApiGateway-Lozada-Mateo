using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pacientes.Api.Models
{
    [Table("Pacientes")]
    public class Paciente
    {
        [Key]
        [Column("IdPaciente")]
        public int IdPaciente { get; set; }

        [Column("Cedula")]
        [StringLength(20)]
        public string Cedula { get; set; } = string.Empty;

        [Column("Nombre")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Column("Apellido")]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Column("Direccion")]
        [StringLength(200)]
        public string Direccion { get; set; } = string.Empty;
    }
}