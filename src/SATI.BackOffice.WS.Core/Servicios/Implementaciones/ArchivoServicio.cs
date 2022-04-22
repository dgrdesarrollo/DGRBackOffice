using log4net;
using Newtonsoft.Json;
using SATI.BackOffice.WS.Core.Servicios.Contratos;
using SATI.BackOffice.WS.Infra.Entidades;
using SATI.BackOffice.WS.Infra.Exceptions;
using SATI.BackOffice.WS.Infra.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace SATI.BackOffice.WS.Core.Servicios.Implementaciones
{
    public class ArchivoServicio : IArchivoServicio
    {
        private string _urlBase;
        private const string API_ROUTE = "/api/boarchivo";
        private const string ROUTE_GETBYID_NOURL = "/GetNoUrl";
        private const string ROUTE_GETFILE_B64 = "/GetFileB64";
        private const string ROUTE_GET_ID_TEMP = "/GetIdTemp";
        private const string ROUTE_CONFIRMAR = "/ConfirmarArchivos";
        private LoggerHelper _logger;

        public ArchivoServicio()
        {
            _logger = new LoggerHelper();
            _urlBase = ConfigurationManager.AppSettings["APIBase"];
        }    

        public RespuestaGenerica<bool> AgregarArchivo(EArchivo entidad, bool esTemporal)
        {
            ApiResponse<bool> respuesta;
            try
            {
                if (entidad.Archivo == null)
                {
                    throw new BOArchivoException("No se ha encontrado el archivo");
                }
                //string texto = string.Empty;
                //XmlDocument xml = new XmlDocument();
                //XmlSerializer ser = new XmlSerializer(entidad.Archivo.GetType());
                //using (MemoryStream ms = new MemoryStream())
                //{
                //    ser.Serialize(ms, entidad.Archivo);
                //    ms.Position = 0;
                //    xml.Load(ms);
                //    texto = xml.GetElementsByTagName("XmlNode").Item(0).InnerText;
                //}

                //entidad.Archivo = texto;
                HelperMapper<EArchivo, EArchivo> map = new HelperMapper<EArchivo, EArchivo>();

                EArchivo enTemp = map.Map(entidad);
                enTemp.Archivo = null;

                _logger.Log(TraceEventType.Information, "Agregando los datos de la entidad.");
                _logger.Log(TraceEventType.Information, $"A Agregar: {JsonConvert.SerializeObject(enTemp)}");
                HelperAPI helperAPI = new HelperAPI();
                HttpClient client = helperAPI.InicializaCliente(entidad, out StringContent content);
                //client.BaseAddress = new Uri(_urlBase);
                HttpResponseMessage response;
                string link = $"{_urlBase}{API_ROUTE}?esTemporal={esTemporal}";
                response = client.PostAsync(link, content).GetAwaiter().GetResult();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string stringData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    _logger.Log(TraceEventType.Information, $"stringData: {stringData}");

                    respuesta = JsonConvert.DeserializeObject<ApiResponse<bool>>(stringData);
                    return new RespuestaGenerica<bool> { Ok = true, DataItem = true, Mensaje = "Archivo agregado satisfactoriamente!!" };
                }
                else
                {
                    var stringData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    _logger.Log(TraceEventType.Warning, JsonConvert.SerializeObject(stringData));
                    return new RespuestaGenerica<bool>
                    {
                        Ok = false,
                        Mensaje = "Hubo un problema intentar agregar el archivo. Intente nuevamente",
                        MensajeOriginal = $"StatusCode: {response.StatusCode} - Data: {stringData}"
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public RespuestaGenerica<bool> ConfirmarArchivos(Confirmacion confirmacion)
        {
            ApiResponse<bool> respuesta;
            try
            {
                HelperAPI helperAPI = new HelperAPI();
                _logger.Log(TraceEventType.Information, "Confirmando el Tramite o Solicitud");
                _logger.Log(TraceEventType.Information, $"A Agregar: {JsonConvert.SerializeObject(confirmacion)}");

                HttpClient client = helperAPI.InicializaCliente(confirmacion, out StringContent content);
                //client.BaseAddress = new Uri(_urlBase);
                HttpResponseMessage response;
                string link = $"{_urlBase}{API_ROUTE}{ROUTE_CONFIRMAR}";
                response = client.PostAsync(link, content).GetAwaiter().GetResult();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string stringData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    _logger.Log(TraceEventType.Information, $"stringData: {stringData}");

                    respuesta = JsonConvert.DeserializeObject<ApiResponse<bool>>(stringData);
                    return new RespuestaGenerica<bool> { Ok = true, DataItem = true, Mensaje = "Confirmación de Trámite satisfactoriamente!!" };
                }
                else
                {
                    var stringData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    _logger.Log(TraceEventType.Warning, JsonConvert.SerializeObject(stringData));
                    return new RespuestaGenerica<bool>
                    {
                        Ok = false,
                        Mensaje = "Hubo un problema intentar agregar el archivo. Intente nuevamente",
                        MensajeOriginal = $"StatusCode: {response.StatusCode} - Data: {stringData}"
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RespuestaGenerica<EArchivo> GetArchivo(Guid id)
        {
            return GetArchivo(id, false);
        }

        public RespuestaGenerica<EArchivo> GetFileB64(Guid id)
        {
            return GetArchivo(id, false, true);
        }
        public RespuestaGenerica<EArchivo> GetArchivo(Guid id, bool noUrl = true, bool getB64 = false)
        {
            ApiResponse<EArchivo> respuesta;
            try
            {
                HelperAPI helperAPI = new HelperAPI();
                _logger.Log(TraceEventType.Information, $"{this.GetType().Name} - {MethodBase.GetCurrentMethod()}");
                _logger.Log(TraceEventType.Information, $"Parametros: id: {id} - noUrl: {noUrl}");
                HttpClient client = helperAPI.InicializaCliente();

                HttpResponseMessage response;
                string link;
                if (getB64)
                {
                    link = $"{_urlBase}{API_ROUTE}{ROUTE_GETFILE_B64}/{id}";
                }
                else
                if (noUrl)
                {
                    link = $"{_urlBase}{API_ROUTE}{ROUTE_GETBYID_NOURL}?id={id}&nourl={noUrl}";
                }
                else
                {
                    link = $"{_urlBase}{API_ROUTE}/{id}";
                }

                response = client.GetAsync(link).GetAwaiter().GetResult();
                _logger.Log(TraceEventType.Information, $"Response: {JsonConvert.SerializeObject(response)}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string stringData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    //_logger.Log(TraceEventType.Information, $"String Response: {stringData}");
                    respuesta = JsonConvert.DeserializeObject<ApiResponse<EArchivo>>(stringData);
                    return new RespuestaGenerica<EArchivo> { Ok = true, DataItem = respuesta.Data };
                }
                else
                {
                    var stringData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return new RespuestaGenerica<EArchivo>
                    {
                        Ok = false,
                        Mensaje = "Hubo un problema para recuperar los archivos solicitados. Intente nuevamente",
                        MensajeOriginal = $"StatusCode: {response.StatusCode} - Data: {stringData}"
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RespuestaGenerica<EArchivo> GetArchivos(QueryFilters filters)
        {
            ApiResponse<List<EArchivo>> respuesta;
            try
            {
                _logger.Log(TraceEventType.Information, $"{this.GetType().Name} - {MethodBase.GetCurrentMethod()}");
                HelperAPI helperAPI = new HelperAPI();
                HttpClient cliente = helperAPI.InicializaCliente();

                HttpResponseMessage response;
                var link = $"{_urlBase}{API_ROUTE}?{helperAPI.EvaluarQueryFilter(filters)}";
                response = cliente.GetAsync(link).GetAwaiter().GetResult();
                _logger.Log(TraceEventType.Information, $"Response: {JsonConvert.SerializeObject(response)}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string stringData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    //_logger.Log(TraceEventType.Information, $"String Response: {stringData}");
                    respuesta = JsonConvert.DeserializeObject<ApiResponse<List<EArchivo>>>(stringData);
                    return new RespuestaGenerica<EArchivo>
                    {
                        ListaItems = respuesta.Data,
                        Ok = true,
                        CantidadReg = respuesta.Data.Count,
                        TotalRegs = respuesta.Meta.TotalCount,
                        Pagina = respuesta.Meta.CurrentPage
                    };
                }
                else
                {
                    var stringData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return new RespuestaGenerica<EArchivo>
                    {
                        Ok = false,
                        Mensaje = "Hubo un problema para recuperar los archivos solicitados. Intente nuevamente",
                        MensajeOriginal = $"StatusCode: {response.StatusCode} - Data: {stringData}"
                    };
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RespuestaGenerica<string> GetIdTemp(string codigoSistema)
        {
            ApiResponse<string> respuesta;
            try
            {
                HelperAPI helperAPI = new HelperAPI();
                _logger.Log(TraceEventType.Information, $"{this.GetType().Name} - {MethodBase.GetCurrentMethod()}");
                _logger.Log(TraceEventType.Information, $"Parametros: codigo de Sistema: {codigoSistema}");
                HttpClient client = helperAPI.InicializaCliente();

                HttpResponseMessage response;
                string link;

                link = $"{_urlBase}{API_ROUTE}{ROUTE_GET_ID_TEMP}/{codigoSistema}";

                response = client.GetAsync(link).GetAwaiter().GetResult();
                _logger.Log(TraceEventType.Information, $"Response: {JsonConvert.SerializeObject(response)}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string stringData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    //_logger.Log(TraceEventType.Information, $"String Response: {stringData}");
                    respuesta = JsonConvert.DeserializeObject<ApiResponse<string>>(stringData);
                    return new RespuestaGenerica<string> { Ok = true, DataItem = respuesta.Data };
                }
                else
                {
                    var stringData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return new RespuestaGenerica<string>
                    {
                        Ok = false,
                        Mensaje = "Hubo un problema para recuperar los archivos solicitados. Intente nuevamente",
                        MensajeOriginal = $"StatusCode: {response.StatusCode} - Data: {stringData}"
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
