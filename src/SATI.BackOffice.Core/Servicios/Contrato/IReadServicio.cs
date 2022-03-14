using SATI.BackOffice.Infraestructura.Intefaces;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;

namespace SATI.BackOffice.Core.Servicios.Contrato
{
    public interface IReadServicio<T> where T:IEntidades
    {
        RespuestaGenerica<T> TraerTodo();
        RespuestaGenerica<T> BuscarPorId(object id);
        RespuestaGenerica<T> Buscar(QueryFilters filters);
    }
}
