﻿using SATI.BackOffice.Infraestructura.Entidades;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using System.Threading.Tasks;

namespace SATI.BackOffice.Core.Servicios.Contrato
{
    public interface IEArchivoServicio : IServicio<EArchivo>, IReadServicio<EArchivo>, IWriteServicio<EArchivo>
    {
        string CalcularRuta(string codigoSistema);
        Task<int> AgregarAsync(EArchivo entidad, bool esTemporal);
        Task<int> AgregarDBAsync(EArchivo entidad);
        RespuestaGenerica<EArchivo> BuscarPorId(object id, bool generarUrl = true);
        bool ConfirmarArchivos(Confirmacion confirmacion);
    }
}
