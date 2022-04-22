using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SATI.BackOffice.WS.Infra.Entidades
{
    public class EArchivo
    {
        public string Id { get; set; }
        public string CodigoSistema { get; set; }
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
