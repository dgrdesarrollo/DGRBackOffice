using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SATI.BackOffice.Infraestructura.Entidades.Options;
using SATI.BackOffice.Infraestructura.Intefaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace SATI.BackOffice.Core.Datos.Implementacion
{
    public class DatabaseContext : IDisposable
    {
        private readonly ManejadorDeTransacciones _manejador;
        private readonly IOptions<ConnectionStrings> _options;
        private readonly ILoggerHelper _logger;
        public DatabaseContext(IOptions<ConnectionStrings> options, ILoggerHelper logger)
        {
            _options = options;
            _logger = logger;
            _manejador = new ManejadorDeTransacciones();
        }

        public SqlConnection ObtenerConexionSql(bool esTransaccional = false)
        {
            if (esTransaccional)
            {
                //verifico si se creo la transacción
                if (_manejador.sqlTransaction == null)
                {
                    ///no existe la transacción por lo que se debe crear la conexion. Si existe una conexion previa
                    ///se debe verificar si se encuentra cerrada para generar la transacción.
                    ///
                    GenerarConexion();

                    //debo generar la transaccion por lo que debo abrir la conexion para instanciar la transacción
                    _manejador.sqlConnection.Open();
                    _manejador.sqlTransaction = _manejador.sqlConnection.BeginTransaction();
                }
            }
            else
            {
                GenerarConexion();
            }

            return _manejador.sqlConnection;
        }

        private void GenerarConexion()
        {
            if (_manejador.sqlConnection == null || string.IsNullOrWhiteSpace(_manejador.sqlConnection.ConnectionString))
            {
                _manejador.sqlConnection = new SqlConnection(_options.Value.SATIKey);
            }
            if (_manejador.sqlConnection.State == ConnectionState.Open)
            {
                _manejador.sqlConnection.Close();
            }
        }

        public void CerrarConexion(bool grabarCambios = false)
        {
            if (grabarCambios)
            {
                GrabarCambios();
            }
            if (_manejador.sqlTransaction != null)
            {
                _manejador.sqlTransaction = null;
            }
            if (_manejador.sqlConnection.State == ConnectionState.Open)
            {
                _manejador.sqlConnection.Close();
            }
        }

        public SqlCommand ObtenerCommandSql(SqlConnection cnn, CommandType commandType)
        {
            SqlCommand cmd = new SqlCommand()
            {
                Connection = cnn,
                CommandType = commandType
            };
            if (_manejador.sqlTransaction != null)
                cmd.Transaction = _manejador.sqlTransaction;

            return cmd;
        }

        public SqlDataReader ObtenerDatosDelCommand(SqlCommand cmd)
        {
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public void GrabarCambios()
        {
            try
            {
                if (_manejador.sqlTransaction != null)
                {
                    _manejador.sqlTransaction.Commit();
                }
            }
            catch (Exception ex)
            {

                _logger.Log(TraceEventType.Error, string.Format("DatabaseContext - {1} Error al grabar los datos: {0}", ex.Message, DateTime.Now));
                if (_manejador.sqlTransaction != null)
                {
                    _manejador.sqlTransaction.Rollback();
                }
                throw;
            }
        }

        public void DeshacerCambios()
        {
            try
            {
                if (_manejador.sqlTransaction != null)
                {
                    _manejador.sqlTransaction.Rollback();
                }
            }
            catch (Exception ex)
            {
                _logger.Log(TraceEventType.Error, string.Format("DatabaseContext - {1} Error al Deshacer los datos: {0}", ex.Message, DateTime.Now));
                throw;
            }
        }

        public List<SqlParameter> InferirParametros<T>(T entidad, IEnumerable<string> excluir = null)
        {
            var parametros = new List<SqlParameter>();

            if (excluir == null) excluir = new List<string>();

            var t = typeof(T);

            var properties = t.GetProperties().Where(p => !excluir.Contains(p.Name));

            foreach (var prop in properties)
            {
                var value = prop.GetValue(entidad, null);
                if (value == null) { continue; }
                var name = "@" + prop.Name;

                parametros.Add(new SqlParameter(name, value));
            }

            return parametros;
        }

        public void Dispose()
        {

            GC.SuppressFinalize(this);
        }
    }

    public class ManejadorDeTransacciones
    {
        public SqlConnection sqlConnection = null;
        public SqlTransaction sqlTransaction = null;

        public bool HasConectionOpen()
        {
            if (sqlConnection == null)
                return false;
            else
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                return true;
            else
                return false;
        }
    }
}
