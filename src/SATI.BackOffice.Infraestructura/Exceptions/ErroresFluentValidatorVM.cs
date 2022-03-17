using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SATI.BackOffice.Infraestructura.Exceptions
{
    public class ErroresFluentValidatorVM
    {
        public string Titulo { get; set; }
        public List<string> Mensajes { get; set; }
    }
}
