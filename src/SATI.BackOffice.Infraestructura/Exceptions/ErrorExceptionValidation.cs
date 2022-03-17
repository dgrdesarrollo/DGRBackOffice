using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SATI.BackOffice.Infraestructura.Exceptions
{
    public class ErrorExceptionValidation
    {
        public ExceptionValidation[] Error { get; set; }
    }

    public class ExceptionValidation
    {
        public int Status { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
    }
}
