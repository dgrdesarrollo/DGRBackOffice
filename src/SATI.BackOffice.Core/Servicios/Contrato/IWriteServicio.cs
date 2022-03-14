using SATI.BackOffice.Infraestructura.Intefaces;

namespace SATI.BackOffice.Core.Servicios.Contrato
{
    public interface IWriteServicio<T> where T:IEntidades
    {
        int Agregar(T entidad);
        int Actualizar(object id, T entidad);
        bool Quitar(object Id);
    }
}
