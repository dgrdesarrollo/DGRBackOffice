using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SATI.BackOffice.Infraestructura.Entidades;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using SATI.BackOffice.Infraestructura.Entidades.Options;
using SATI.BackOffice.Infraestructura.Exceptions;
using SATI.BackOffice.Infraestructura.Helpers;
using SATI.BackOffice.Infraestructura.Intefaces;
using SATI.BackOffice.Site.Core.Servicios.Contratos;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SATI.BackOffice.Site.Core.Servicios.Implementaciones
{
    public class NormaLegalServicio:Servicio<dl_documentos>, INormaLegalServicio
    {
        private readonly static string RUTA_ENTIDAD = "/api/bonorma";
        private readonly static string CALCULAR_RUTA = "/CalcularRuta";
        private readonly AppSettings _appSettings;
        private readonly ILoggerHelper _logger;

        public NormaLegalServicio(IOptions<AppSettings> options, ILoggerHelper logger) : base(options, logger, RUTA_ENTIDAD) 
        {
            _appSettings = options.Value;
            _logger = logger;
        }

        public async Task<string> CalcularRuta(string codigo)
        {
            ApiResponse<string> respuesta;
            try
            {
                HelperAPI helperAPI = new();
                _logger.Log(TraceEventType.Information, $"Por calcular ruta: Código {codigo}");
                HttpClient client = helperAPI.InicializaCliente();

                HttpResponseMessage response;
                var link = $"{_appSettings.URLBase}{RUTA_ENTIDAD}{CALCULAR_RUTA}/{codigo}";
                response = await client.GetAsync(link);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string stringData = await response.Content.ReadAsStringAsync();
                    respuesta = JsonConvert.DeserializeObject<ApiResponse<string>>(stringData);
                    return respuesta.Data;
                }
                else
                {
                    throw new SATIException($"Hubo un error al intentar obtener los datos. Código: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw _logger.Log(ex);
            }
        }
    }
}
