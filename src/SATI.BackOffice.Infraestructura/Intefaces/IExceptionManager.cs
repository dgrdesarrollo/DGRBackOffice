using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SATI.BackOffice.Infraestructura.Intefaces
{
    public interface IExceptionManager
    {
        Exception HandleException(Exception ex);
    }
}
