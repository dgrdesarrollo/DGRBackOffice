using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SATI.BackOffice.Infraestructura.Exceptions
{
    public class SATIException : Exception
    {
        public SATIException()
        {
        }

        public SATIException(string message) : base(message)
        {
        }

        public SATIException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SATIException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
