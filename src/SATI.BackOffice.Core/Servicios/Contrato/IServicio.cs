using SATI.BackOffice.Infraestructura.Intefaces;
using Microsoft.Data.SqlClient;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using System.Collections.Generic;

namespace SATI.BackOffice.Core.Servicios.Contrato
{
    public interface IServicio<T> where T : IEntidades
    {
        List<T> InvokarSp2Lst(string sp, List<SqlParameter> parametros);
        List<T> InvokarSp2Lst(string sp, SqlParameter parametro);
        List<T> InvokarSp2Lst(string sp);
        int InvokarNQuery(string sp, List<SqlParameter> parametros, bool esTransaccional = false, bool elUltimo = true);
        int InvokarNQuery(string sp, SqlParameter parametro, bool esTransaccional = false, bool elUltimo = true);
        object InvokarSpScalar(string sp, List<SqlParameter> parametros, bool esTransacciona = false, bool elUltimo = true);
        object InvokarSpScalar(string sp, SqlParameter parametro, bool esTransacciona = false, bool elUltimo = true);
        RespuestaGenerica<bool> Commit();
    }
}
