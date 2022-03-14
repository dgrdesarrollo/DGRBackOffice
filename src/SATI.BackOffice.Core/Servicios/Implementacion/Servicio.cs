using SATI.BackOffice.Infraestructura.Intefaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SATI.BackOffice.Core.Datos.Contratos;
using SATI.BackOffice.Core.Servicios.Contrato;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using SATI.BackOffice.Infraestructura.Entidades.Options;
using System.Collections.Generic;

namespace SATI.BackOffice.Core.Servicios.Implementacion
{
    public class Servicio<T> : IServicio<T> where T : IEntidades
    {
        #region Propiedades

        protected readonly IRepository<T> _repositorio;
        protected readonly IUnitOfWork _uow;
        protected readonly AppSettings _appSettings;

        #endregion

        #region constructor
        public Servicio(IUnitOfWork unitOfWork,IOptions<AppSettings> options)
        {

            _uow = unitOfWork;
            _repositorio = _uow.GetRepository<T>();
            _appSettings = options.Value;
        }
        
        #endregion

        public virtual List<T> InvokarSp2Lst(string sp, List<SqlParameter> parametros)
        {
            return _repositorio.InvokarSp2Lst(sp, parametros);
        }

        public virtual int InvokarNQuery(string sp, List<SqlParameter> parametros, bool esTransaccional = false, bool elUltimo = true)
        {
            return _repositorio.InvokarSpNQuery(sp, parametros, esTransaccional, elUltimo);
        }

        public object InvokarSpScalar(string sp, List<SqlParameter> parametros, bool esTransacciona = false, bool elUltimo = true)
        {
            return _repositorio.InvokarSpScalar(sp, parametros);
        }

        public virtual RespuestaGenerica<bool> Commit()
        {
           return _uow.SaveChanges(true);
        }

        public List<T> InvokarSp2Lst(string sp)
        {
            return _repositorio.InvokarSp2Lst(sp);
        }

        public List<T> InvokarSp2Lst(string sp, SqlParameter parametro)
        {
            return _repositorio.InvokarSp2Lst(sp, parametro);
        }

        public int InvokarNQuery(string sp, SqlParameter parametro, bool esTransaccional = false, bool elUltimo = true)
        {
            return _repositorio.InvokarSpNQuery(sp, parametro, esTransaccional, elUltimo);
        }

        public object InvokarSpScalar(string sp, SqlParameter parametro, bool esTransacciona = false, bool elUltimo = true)
        {
            return _repositorio.InvokarSpScalar(sp, parametro, esTransacciona, elUltimo);

        }

     
    }
}
