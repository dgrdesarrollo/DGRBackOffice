﻿using log4net;
using log4net.Config;
using log4net.Repository;
using SATI.BackOffice.WS.Infra.Exceptions;
using SATI.BackOffice.WS.Infra.Extensiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SATI.BackOffice.WS.Infra.Helpers
{
    public interface ILoggerHelper
    {
        Exception Log(Exception ex);
        void Log(TraceEventType tipo, string mensaje);
    }
    public class LoggerHelper : ILoggerHelper
    {
        private readonly ILog _log;
        //Quito el constructor.
        public LoggerHelper()
        {
            
            if (_log == null)
            {
                string rutaArch;
                _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

                rutaArch = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.Config.xml");
                ILoggerRepository repository = log4net.LogManager.GetRepository(Assembly.GetCallingAssembly());
                FileInfo arch = new FileInfo(rutaArch);
                XmlConfigurator.Configure(repository, arch);
                _log.Info("INICIALIZANDO LOG4NET");
            }
        }       

        /// <summary>
        /// Carga el archivo de configuración de log4net ( log4net.Config.xml )y obtiene el logger.
        /// </summary>
        /// <param name="nombreLogger">nombre del logger a buscar</param>
        /// <returns>logger encontrado</returns>


        public void Log(TraceEventType tipo, string mensaje)
        {

            switch (tipo)
            {
                case TraceEventType.Error:
                    _log.Error(mensaje);
                    break;
                case TraceEventType.Information:
                    _log.Info(mensaje);
                    break;
                case TraceEventType.Warning:
                    _log.Warn(mensaje);
                    break;
                default:
                    _log.Debug(mensaje);
                    break;
            }
            var t = tipo.ToString(); //Convert.ChangeType(tipo, Enum.GetUnderlyingType(tipo.GetType()));
            Console.WriteLine(string.Format("{0} - {1}", t, mensaje));
        }

        public Exception Log(Exception ex)
        {
            //Verifico si el Administrador de excepciones esta activo
            var enabled = ConfigurationManager.AppSettings["ExceptionManagerEnabled"].ToBoolean();
            var exFormat = new ExceptionFormatterExtension(ex);

            Log(TraceEventType.Error, exFormat.GetValue());

            if (!enabled)
            {
                return ex;
            }

            if (ex is NotImplementedException || ex is BOArchivoException)
            {
                return ex;
            }

            //Las siguientes excepciones son sensitivas. Por lo tanto, se logueará la excepcion original
            //y se lanzara una excepcion genperica para el tipo de excepcion.            
            //if (ex is SmtpException)
            //{
            //    return new InvalidOperationException("Ocurrió un error al intentar enviar correo electrónico.");
            //}

            //if (ex is SqlException)
            //{
            //    return new InvalidOperationException("Ocurrió un error de acceso a datos.");
            //}

            if (ex is IOException)
            {
                return new InvalidOperationException("Ocurrió un error de acceso a archivos.");
            }

            //en caso de que no sea ninguna de las anteriores pondre un mensaje generico.
            return new Exception("Se produjo un error. Intente de nuevo más tarde.");
        }
    }
}
