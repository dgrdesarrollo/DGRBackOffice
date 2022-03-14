using SATI.BackOffice.Infraestructura.Intefaces;
using Microsoft.Data.SqlClient;
using SATI.BackOffice.Core.Datos.Contratos;
using SATI.BackOffice.Infraestructura.Helpers;
using System.Collections.Generic;
using System.Data;

namespace SATI.BackOffice.Core.Datos.Implementacion
{
    public class Repository<T> : IRepository<T> where T : IEntidades
    {
        private readonly DatabaseContext _dbContext;
        //protected string EntitySetName { get; private set; }

        public Repository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public DatabaseContext GetContext()
        {
            return _dbContext;
        }



        public List<T> InvokarSp2Lst(string sp, List<SqlParameter> parametros)
        {
            int contador = 0;
            List<T> resultado = null;

            using (var cnn = _dbContext.ObtenerConexionSql())
            {
                var cmd = _dbContext.ObtenerCommandSql(cnn, CommandType.StoredProcedure);
                cmd.CommandText = sp;
                foreach (var p in parametros)
                {
                    if(p==null)
                    { continue; }
                    cmd.Parameters.Add(p);
                }
                cnn.Open();
                using (var dr = _dbContext.ObtenerDatosDelCommand(cmd))
                {
                    var mapper = new MapeoGenerico<T>();
                    resultado = new List<T>();
                    while (dr.Read())
                    {
                        contador++;
                        resultado.Add(mapper.Map(dr));
                    }
                }
            }
            return resultado;
        }

       public int InvokarSpNQuery(string sp, List<SqlParameter> parametros, bool esTransacciona = false, bool elUltimo = true)
        {
            int resultado = 0;

            var cnn = _dbContext.ObtenerConexionSql(esTransacciona);

            var cmd = _dbContext.ObtenerCommandSql(cnn, CommandType.StoredProcedure);
            cmd.CommandText = sp;
            foreach (var p in parametros)
            {
                cmd.Parameters.Add(p);
            }
            //si es TRANSACCIONAL la conexion ya fue abierta al momento de generar la conexion y definir la transaccion para la operacion actual.
            if (!esTransacciona)
            {
                cnn.Open();
            }
            resultado = cmd.ExecuteNonQuery();
            if (esTransacciona && elUltimo)
            {
                _dbContext.GrabarCambios();
            }
            //en caso de ser el ultimo item de ejecución se procederá a cerrar la conexión
            if (elUltimo)
            {
                _dbContext.CerrarConexion();
            }

            return resultado;
        }

        public object InvokarSpScalar(string sp, List<SqlParameter> parametros, bool esTransacciona = false, bool elUltimo = true)
        {
            object resultado = null;
            var cnn = _dbContext.ObtenerConexionSql(esTransacciona);

            var cmd = _dbContext.ObtenerCommandSql(cnn, CommandType.StoredProcedure);
            cmd.CommandText = sp;
            foreach (var p in parametros)
            {
                cmd.Parameters.Add(p);
            }
            //si es TRANSACCIONAL la conexion ya fue abierta al momento de generar la conexion y definir la transaccion para la operacion actual.
            if (!esTransacciona)
            {
                cnn.Open();
            }
            resultado = cmd.ExecuteScalar();
            if (esTransacciona && elUltimo)
            {
                _dbContext.GrabarCambios();
            }
            //en caso de ser el ultimo item de ejecución se procederá a cerrar la conexión
            if (elUltimo)
            {
                _dbContext.CerrarConexion();
            }

            return resultado;
        }

        public List<T> InvokarSp2Lst(string sp)
        {
            return InvokarSp2Lst(sp, new List<SqlParameter>());
        }

        public List<T> InvokarSp2Lst(string sp, SqlParameter parametro)
        {
            return InvokarSp2Lst(sp, new List<SqlParameter> { parametro });
        }

        public int InvokarSpNQuery(string sp, SqlParameter parametro, bool esTransaccional = false, bool elUltimo = true)
        {
            return InvokarSpNQuery(sp, new List<SqlParameter> { parametro }, esTransaccional, elUltimo);
        }

        public object InvokarSpScalar(string sp, SqlParameter parametro, bool esTransacciona = false, bool elUltimo = true)
        {
            return InvokarSpScalar(sp, new List<SqlParameter> { parametro }, esTransacciona, elUltimo);
        }

        public List<SqlParameter> InferirParametros(T entidad, IEnumerable<string> excluir = null)
        {
            return _dbContext.InferirParametros(entidad, excluir);
        }
    }
}
