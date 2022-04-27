using SATI.BackOffice.Infraestructura.Intefaces;
using System;

namespace SATI.BackOffice.Infraestructura.Entidades
{
    public class EArchivo : IEntidades
    {
        public string Id { get; set; }
        public string CodigoSistema { get; set; }
       // public string ClaveRelacion { get; set; }
        public int EstadoEArchivoId { get; set; }
        public int RequerimientoId { get; set; }
        public string SolicitudId { get; set; }
        public string TipoSolicitud { get; set; }
        public string CUIT { get; set; }
        public string NombreArchivo { get; set; }
        public string Descripcion { get; set; }
        public string Ruta { get; set; }
        public DateTime FechaIndexacion { get; set; }
        public string Hash { get; set; }
        public string Archivo { get; set; }
        /// <summary>
        /// Este campo tiene un long de 18 digitos. Corresponde a un Ticks
        /// </summary>
        public string ClaveTemporal { get; set; }

    }
}
