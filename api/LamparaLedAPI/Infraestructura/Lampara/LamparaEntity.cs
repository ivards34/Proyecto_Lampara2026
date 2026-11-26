using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infraestructura.Lampara
{
    [Table("lampara_lecturas")]
    public class LamparaEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime FechaHora { get; set; }

        [MaxLength(100)]
        public string? DispositivoId { get; set; }

        public double TemperaturaSensor { get; set; }

        public double HumedadSensor { get; set; }

        public double TemperaturaApi { get; set; }

        public double HumedadApi { get; set; }

        [MaxLength(10)]
        public string? EstadoLampara { get; set; }
    }
}
