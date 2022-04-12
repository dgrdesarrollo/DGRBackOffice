using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SATI.BackOffice.Core.Datos.Contratos;
using SATI.BackOffice.Core.Servicios.Contrato;
using SATI.BackOffice.Infraestructura;
using SATI.BackOffice.Infraestructura.Entidades;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using SATI.BackOffice.Infraestructura.Entidades.Options;
using SATI.BackOffice.Infraestructura.Intefaces;
using System.Diagnostics;
using System.Reflection;

namespace SATI.BackOffice.Core.Servicios.Implementacion
{
    public class TipoNormativaServicio : Servicio<dl_tipos>, ITipoNormativaServicio
    {
        public TipoNormativaServicio(IUnitOfWork uow, IOptions<AppSettings> options, ILoggerHelper logger) : base(uow, options, logger)
        {
        }

        public RespuestaGenerica<dl_tipos> Buscar(QueryFilters filters)
        {
            throw new System.NotImplementedException();
        }

        public RespuestaGenerica<dl_tipos> BuscarPorId(object id)
        {
            throw new System.NotImplementedException();
        }

        public RespuestaGenerica<dl_tipos> TraerTodo()
        {
            _logger.Log(TraceEventType.Information, $"Ejecutando: {this.GetType().Name}-{MethodBase.GetCurrentMethod()}");

            var sp = Constantes.StoredProcedures.TIPO_NORMATIVA_GET_ALL;
            SqlParameter parametro = null;
            var tipos = InvokarSp2Lst(sp, parametro);
            if (tipos.Count > 0)
            {
                _logger.Log(TraceEventType.Information, $"Resgistros encontrados: {tipos.Count} - SP: {sp}");
                return new RespuestaGenerica<dl_tipos> { Ok = true, ListaItems = tipos, CantidadReg = tipos.Count, TotalRegs = tipos.Count };
            }
            else
            {
                _logger.Log(TraceEventType.Warning, $"No se encontraron registros - SP: {sp}");
                return new RespuestaGenerica<dl_tipos> { Ok = false, Mensaje = Constantes.MensajesError.TIPO_NORMATIVA_REGISTROS_NO_ENCONTRADOS };
            }
        }
    }
}
