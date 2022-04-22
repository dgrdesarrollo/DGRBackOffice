using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SATI.BackOffice.WS.Infra.Exceptions
{
    public class BOArchivoException : Exception
    {
        public BOArchivoException()
        {
        }

        public BOArchivoException(string message) : base(message)
        {
        }

        public BOArchivoException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BOArchivoException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
