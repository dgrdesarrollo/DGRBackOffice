using Microsoft.Extensions.Options;
using SATI.BackOffice.Infraestructura.Entidades;
using SATI.BackOffice.Infraestructura.Entidades.Options;
using SATI.BackOffice.Infraestructura.Intefaces;
using SATI.BackOffice.Site.Core.Servicios.Contratos;

namespace SATI.BackOffice.Site.Core.Servicios.Implementaciones
{
    public class TipoNormaServicio:Servicio<dl_tipos>, ITipoNormaServicio
    {
        private readonly static string RUTA_ENTIDAD = "/api/botiponorma";

        public TipoNormaServicio(IOptions<AppSettings> options, ILoggerHelper logger) : base(options, logger, RUTA_ENTIDAD)
        {

        }
    }
}
