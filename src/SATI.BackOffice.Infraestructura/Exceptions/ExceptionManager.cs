using SATI.BackOffice.Infraestructura.Intefaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SATI.BackOffice.Infraestructura.Exceptions
{
    public class ExceptionManager : IExceptionManager
    {

        public Exception HandleException(Exception ex)
        {

            //These exceptions are considered as non-sensitive. 
            //So, we returns the same exception.
            if (ex is SATIException || ex is InvalidOperationException
                || ex is ArgumentException || ex is NotFoundException)
            {
                return ex;
            }

            //The following exceptions are sensitive.
            //So, we logs the original exception and throw a new exception message.

            if (ex is SmtpException)
            {
                return new InvalidOperationException("Ocurrió un error al intentar enviar correo electrónico.");
            }

            //if (ex is SqlException)
            //{
            //    return new InvalidOperationException("Ocurrió un error de acceso a datos.");
            //}

            if (ex is IOException)
            {
                return new InvalidOperationException("Ocurrió un error de acceso a archivos.");
            }


            return new InvalidOperationException("Ocurrió un error al realizar la operación.");
        }

    }
}
