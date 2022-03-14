using SATI.BackOffice.Infraestructura.Intefaces;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using System;

namespace SATI.BackOffice.Core.Datos.Contratos
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : IEntidades;
        //DatabaseContext Contexto { get; }
        RespuestaGenerica<bool> SaveChanges(bool process);

        RespuestaGenerica<bool> DeshacerCambios();
    }
}
