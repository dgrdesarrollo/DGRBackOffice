using System;
using System.Runtime.Serialization;

namespace SATI.BackOffice.Infraestructura.Exceptions
{
    public class BOException : Exception
    {
        public BOException()
        {
        }

        public BOException(string message) : base(message)
        {
        }

        public BOException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BOException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
