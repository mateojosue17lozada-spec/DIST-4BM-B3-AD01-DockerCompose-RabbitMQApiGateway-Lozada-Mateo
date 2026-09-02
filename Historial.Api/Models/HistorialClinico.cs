using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Historial.Api.Models
{
    [Table("HistorialClinico")]
    public class HistorialClinico
    {
        [Key]
        [Column("IdHistorial")]
        public int IdHistorial { get; set; }

        [Column("IdPaciente")]
        public int IdPaciente { get; set; }

        [Column("NumHistoria")]
        [StringLength(50)]
        public string NumHistoria { get; set; } = string.Empty;

        [Column("Diagnostico")]
        [StringLength(500)]
        public string Diagnostico { get; set; } = string.Empty;

        [Column("Tratamiento")]
        [StringLength(500)]
        public string Tratamiento { get; set; } = string.Empty;

        [Column("Fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}