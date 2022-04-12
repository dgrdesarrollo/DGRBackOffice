using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SATI.BackOffice.Infraestructura.Entidades.Constantes
{
    public enum EstadoArchivo
    {
        CONFIRMADO = 1,
        TRANSFERIDO = 2,
        INSERTADO = 3,
        EXPORTADO = 4,
        PENDIENTE = 5,
        DESECHADO = 6,
    }
}
