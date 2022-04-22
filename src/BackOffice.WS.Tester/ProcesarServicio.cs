using BackOffice.WS.Tester.ArchivoSv;
using Newtonsoft.Json;
using SATI.BackOffice.WS.Infra.Helpers;
using System;
using System.Diagnostics;
using System.Reflection;

namespace BackOffice.WS.Tester
{
    internal class ProcesarServicio
    {
        private ILoggerHelper _logger;
        public ProcesarServicio(ILoggerHelper logger)
        {
            _logger = logger;
        }

        public ProcesarServicio():this(new LoggerHelper())
        {

        }

        internal void Inicio()
        {
            BOArchivo ws = new BOArchivo();
            _logger.Log(TraceEventType.Information, "Se instancia servicio");
            var request = new QueryFilters
            {
                Id = "581795ae-e2e5-4ab5-adf4-0ae0125ccbf5".ToGuid()
            };
            _logger.Log(TraceEventType.Information, $"Datos: {JsonConvert.SerializeObject(request)}");
            var res = ws.GetArchivos(request);
            _logger.Log(TraceEventType.Information, $"Response: {res.ListaItems}");

        }
    }
}