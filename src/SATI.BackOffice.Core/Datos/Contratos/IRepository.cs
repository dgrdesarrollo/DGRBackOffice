using SATI.BackOffice.Infraestructura.Intefaces;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace SATI.BackOffice.Core.Datos.Contratos
{
    public interface IRepository<T> where T : IEntidades
    {
        /// <summary>
        /// Método ejecuta un Stored Procedure para realizar alguna consulta y con la misma
        /// poder devolver, si es posible, una lista de datos de un tipo especifico. Si no se 
        /// encuentran datos devuelve una lista vacia.
        /// </summary>
        /// <param name="sp">nombre del Stored Procedure</param>
        /// <param name="parametros">Listado de parametros</param>
        /// <returns>Devuelve lista de datos de tipo predefinido</returns>
        List<T> InvokarSp2Lst(string sp);
        List<T> InvokarSp2Lst(string sp, SqlParameter parametro);
        List<T> InvokarSp2Lst(string sp, List<SqlParameter> parametros);

        /// <summary>
        /// Método ejecuta un Stored Procedure para realizar alguna actualización
        /// o inserción de datos en una base de datos 
        /// </summary>
        /// <param name="sp">nombre del Stored Procedure</param>
        /// <param name="parametros">Listado de parametros</param>
        /// <returns>devuelve cantidad de modificaciones</returns>
        int InvokarSpNQuery(string sp, SqlParameter parametro, bool esTransaccional = false, bool elUltimo = true);
        int InvokarSpNQuery(string sp, List<SqlParameter> parametros, bool esTransaccional = false, bool elUltimo = true);

        /// <summary>
        /// Método ejecuta un Stored Procedure para realizar alguna consulta y con la misma
        /// poder devolver, si es posible, una valor especifico. 
        /// </summary>
        /// <param name="sp">nombre del Stored Procedure</param>
        /// <param name="parametros">Listado de parametros</param>
        /// <returns>Devuelve un valor</returns>
        object InvokarSpScalar(string sp, SqlParameter parametro, bool esTransacciona = false, bool elUltimo = true);
        object InvokarSpScalar(string sp, List<SqlParameter> parametros, bool esTransacciona = false, bool elUltimo = true);

        List<SqlParameter> InferirParametros(T entidad, IEnumerable<string> excluir = null);
    }

}
