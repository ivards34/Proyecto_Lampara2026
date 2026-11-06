using Infraestructura.Meteorologia;
using System;
using System.Collections.Generic;
using System.Text;

namespace Negocio.Clima
{
    public class DatosMeteorologiaDTO
    {
        public double Temperatura { get; set; }
        public double Humedad { get; set; }
        public double Presion { get; set; }
        public double VelocidadViento { get; set; }
        public string DireccionViento { get; set; }
        public string CondicionDescripcion { get; set; }
        public double GradosViento { get; set; }

        public static DatosMeteorologiaDTO CopiarDatos(MeteorologiaDTO meteorologiaDTO)
        {
            var resultado = new DatosMeteorologiaDTO();
            resultado.CondicionDescripcion = meteorologiaDTO.CondicionDescripcion;
            resultado.DireccionViento = meteorologiaDTO.DireccionViento;
            resultado.Temperatura = meteorologiaDTO.TemperaturaCelsius;
            resultado.Humedad = meteorologiaDTO.Humedad;
            resultado.Presion = meteorologiaDTO.Presion;
            resultado.VelocidadViento = meteorologiaDTO.VelocidadVientoKph;
            resultado.GradosViento = meteorologiaDTO.GradosViento;

            return resultado;
        }
    }
}
