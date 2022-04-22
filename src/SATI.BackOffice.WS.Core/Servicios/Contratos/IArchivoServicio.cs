using SATI.BackOffice.WS.Infra.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SATI.BackOffice.WS.Core.Servicios.Contratos
{
    public interface IArchivoServicio
    {
        RespuestaGenerica<EArchivo> GetArchivos(QueryFilters filters);
        RespuestaGenerica<EArchivo> GetFileB64(Guid id);
        RespuestaGenerica<EArchivo> GetArchivo(Guid id);
        RespuestaGenerica<EArchivo> GetArchivo(Guid id,bool noUrl=true,bool getB64=false);
        RespuestaGenerica<string> GetIdTemp(string codigoSistema);
        RespuestaGenerica<bool> AgregarArchivo(EArchivo eArchivo, bool esTemporal);
        RespuestaGenerica<bool> ConfirmarArchivos(Confirmacion confirmacion);
    }
}
