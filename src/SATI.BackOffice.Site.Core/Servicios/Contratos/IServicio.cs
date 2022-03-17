using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using SATI.BackOffice.Infraestructura.Intefaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SATI.BackOffice.Site.Core.Servicios.Contratos
{
    public interface IServicio<T> where T : IEntidades
    {
        Task<(List<T>, Metadata)> BuscarAsync();
        Task<(List<T>, Metadata)> BuscarAsync(QueryFilters filters);
        Task<T> BuscarAsync(object id);
        Task<T> BuscarUnoAsync();
        Task<bool> AgregarAsync(T entidad);
        Task<bool> ActualizarAsync(object id, T entidad);
        Task<bool> EliminarAsync(object id);
    }
}
