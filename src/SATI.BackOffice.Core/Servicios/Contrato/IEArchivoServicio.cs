using SATI.BackOffice.Infraestructura.Entidades;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using System;
using System.Threading.Tasks;

namespace SATI.BackOffice.Core.Servicios.Contrato
{
    public interface IEArchivoServicio : IServicio<EArchivo>, IReadServicio<EArchivo>, IWriteServicio<EArchivo>
    {
        string CalcularRuta(string codigoSistema);
        Task<(string, string)> AgregarAsync(EArchivo entidad, bool esTemporal);
        Task<int> AgregarDBAsync(EArchivo entidad);
        RespuestaGenerica<EArchivo> BuscarPorId(object id, bool generarUrl = true,bool getB64=false);
        bool ConfirmarArchivos(Confirmacion confirmacion);
    }
}
