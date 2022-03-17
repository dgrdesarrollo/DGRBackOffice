using Microsoft.Extensions.Options;
using SATI.BackOffice.Core.Datos.Contratos;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using SATI.BackOffice.Infraestructura.Entidades.Options;
using SATI.BackOffice.Infraestructura.Intefaces;
using System;

namespace SATI.BackOffice.Core.Datos.Implementacion
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        #region Properties

        private bool _disposed;
        private DatabaseContext _dbContext;
        private readonly ILoggerHelper _logger;
        //private readonly IOptions<ConnectionStrings> _options;

        //internal DatabaseContext Contexto
        //{
        //    get
        //    {
        //        return _dbContext;
        //    }
        //}

        #endregion

        #region Constructor

        public UnitOfWork(IOptions<ConnectionStrings> options, ILoggerHelper logger)
        {
            _dbContext = new DatabaseContext(options,logger);
            _logger = logger;
        }

        //public UnitOfWork(ILoggerHelper logger)
        //    : this(new DatabaseContext())
        //{
        //}

        #endregion

        #region Public Methods

        public IRepository<T> GetRepository<T>() where T : IEntidades
        {
            IRepository<T> repositorio = new Repository<T>(_dbContext);

            return repositorio;
        }


        public RespuestaGenerica<bool> SaveChanges(bool process)
        {
            var respuesta = new RespuestaGenerica<bool>();
            try
            {
                //debo actualizarlo para que me devuelva la cantidad de instancias que fueron modificadas / agregadas / eliminadas
                //respuesta.NroDeEntidadesModificadas = _dbContext.SaveChanges();
                _dbContext.GrabarCambios();
                respuesta.Mensaje = "Operación Efectuada Correctamente";
                respuesta.Ok = true;
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                respuesta.Mensaje = string.Format("{0} - Hubo error al intentar grabar los datos. Verifique en Log. ", DateTime.Now);
                respuesta.Ok = false;
            }
            return respuesta;
        }


        public RespuestaGenerica<bool> DeshacerCambios()
        {
            var respuesta = new RespuestaGenerica<bool>();
            try
            {
                //debo actualizarlo para que me devuelva la cantidad de instancias que fueron modificadas / agregadas / eliminadas
                //respuesta.NroDeEntidadesModificadas = _dbContext.SaveChanges();
                _dbContext.DeshacerCambios();
                respuesta.Mensaje = "Rollback Efectuado Correctamente";
                respuesta.Ok = true;
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                respuesta.Mensaje = string.Format("{0} - Hubo error al intentar deshacr los datos. Verifique en Log. ", DateTime.Now);
                respuesta.Ok = false;
            }
            return respuesta;
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }

            _disposed = true;
        }


        #endregion
    }
}
