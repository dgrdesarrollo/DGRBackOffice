using Microsoft.Extensions.Options;
using SATI.BackOffice.Core.Datos.Contratos;
using SATI.BackOffice.Core.Servicios.Contrato;
using SATI.BackOffice.Infraestructura.Entidades;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using SATI.BackOffice.Infraestructura.Entidades.Options;
using SATI.BackOffice.Infraestructura.Intefaces;
using System;
using System.Threading.Tasks;

namespace SATI.BackOffice.Core.Servicios.Implementacion
{
    public class EArchivoDBServicio:Servicio<EArchivo>,IEArchivoDBServicio
    {
        public EArchivoDBServicio(IUnitOfWork uow, IOptions<AppSettings> options, ILoggerHelper logger) : base(uow, options, logger)
        {
        }

        public int Actualizar(object id, EArchivo entidad)
        {
            throw new NotImplementedException();
        }

        public int Agregar(EArchivo entidad)
        {
            throw new NotImplementedException();
        }

        public Task<int> AgregarAsync(EArchivo entidad, bool esTemporal)
        {
            throw new NotImplementedException();
        }

        public RespuestaGenerica<EArchivo> Buscar(QueryFilters filters)
        {
            throw new NotImplementedException();
        }

        public RespuestaGenerica<EArchivo> BuscarPorId(object id, bool generarUrl = true)
        {
            throw new NotImplementedException();
        }

        public RespuestaGenerica<EArchivo> BuscarPorId(object id)
        {
            throw new NotImplementedException();
        }

        public bool Quitar(object Id)
        {
            throw new NotImplementedException();
        }

        public RespuestaGenerica<EArchivo> TraerTodo()
        {
            throw new NotImplementedException();
        }
    }
}
