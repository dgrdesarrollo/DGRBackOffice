using SATI.BackOffice.Infraestructura.Entidades;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using System.Threading.Tasks;

namespace SATI.BackOffice.Core.Servicios.Contrato
{
    public interface IEArchivoDBServicio : IServicio<EArchivo>, IReadServicio<EArchivo>, IWriteServicio<EArchivo>
    {
        Task<int> AgregarAsync(EArchivo entidad, bool esTemporal);
        RespuestaGenerica<EArchivo> BuscarPorId(object id, bool generarUrl = true);
    }
}
