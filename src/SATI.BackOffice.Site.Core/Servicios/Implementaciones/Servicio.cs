using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using SATI.BackOffice.Infraestructura.Entidades.Options;
using SATI.BackOffice.Infraestructura.Exceptions;
using SATI.BackOffice.Infraestructura.Helpers;
using SATI.BackOffice.Infraestructura.Intefaces;
using SATI.BackOffice.Site.Core.Servicios.Contratos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace SATI.BackOffice.Site.Core.Servicios.Implementaciones
{
    public class Servicio<T> : IServicio<T> where T : IEntidades
    {
        private readonly AppSettings appSettings;
        private readonly ILoggerHelper _logger;
        private string _rutaEntidad;

        public Servicio(IOptions<AppSettings> options, ILoggerHelper logger, string rutaEntidad)
        {
            appSettings = options.Value;
            _logger = logger;
            _rutaEntidad = rutaEntidad;
        }

        public Servicio(IOptions<AppSettings> options, ILoggerHelper logger)
        {
            appSettings = options.Value;
            _logger = logger;
            _rutaEntidad = string.Empty;
        }
        public async Task<(List<T>, Metadata)> BuscarAsync()
        {            
            ApiResponse<List<T>> respuesta;

            HelperAPI helperAPI = new HelperAPI();
            _logger.Log(TraceEventType.Information, $"Buscando todas los coeficientes '{_rutaEntidad}'");
            HttpClient client = helperAPI.InicializaCliente();

            HttpResponseMessage response = client.GetAsync($"{appSettings.URLBase}{_rutaEntidad}").Result;
            _logger.Log(TraceEventType.Information, $"Response: {JsonConvert.SerializeObject(response)}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string stringData = await response.Content.ReadAsStringAsync();
                //_logger.Log(TraceEventType.Information, $"String Response: {stringData}");
                respuesta = JsonConvert.DeserializeObject<ApiResponse<List<T>>>(stringData);
                return (respuesta.Data, respuesta.Meta);
            }           
            else
            {
                //string stringData = await response.Content.ReadAsStringAsync();
                //ErrorExceptionValidation resp = JsonConvert.DeserializeObject<ErrorExceptionValidation>(stringData);
                //var error = resp.Error.First();
                //throw new NegocioException($"Código: {response.StatusCode} - Error: {error.Detail}");
                await ParseoError(response);
                return (null, null);
            }
        }    

        public virtual async Task<(List<T>, Metadata)> BuscarAsync(QueryFilters filters)
        {

            ApiResponse<List<T>> respuesta;
            try
            {
                HelperAPI helperAPI = new HelperAPI();
                _logger.Log(TraceEventType.Information, $"Buscando todos los Items '{_rutaEntidad}' - Filtro: {JsonConvert.SerializeObject(filters)}");
                HttpClient client = helperAPI.InicializaCliente();

                HttpResponseMessage response;
                if (filters.Todo)
                {
                    response = await client.GetAsync($"{appSettings.URLBase}{_rutaEntidad}");
                    _logger.Log(TraceEventType.Information, $"Response: {JsonConvert.SerializeObject(response)}");
                }
                else
                {
                    var link = $"{appSettings.URLBase}{_rutaEntidad}?{EvaluarQueryFilter(filters)}";

                    response = await client.GetAsync(link);
                    _logger.Log(TraceEventType.Information, $"Response: {JsonConvert.SerializeObject(response)}");
                }

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string stringData = await response.Content.ReadAsStringAsync();
                    //_logger.Log(TraceEventType.Information, $"String Response: {stringData}");
                    respuesta = JsonConvert.DeserializeObject<ApiResponse<List<T>>>(stringData);
                    return (respuesta.Data, respuesta.Meta);
                }              
                else
                {
                    //string stringData = await response.Content.ReadAsStringAsync();
                    //ErrorExceptionValidation resp = JsonConvert.DeserializeObject<ErrorExceptionValidation>(stringData);
                    //var error = resp.Error.First();
                    //throw new NegocioException($"Código: {response.StatusCode} - Error: {error.Detail}");
                    await ParseoError(response);
                    return (null, null);
                }

            }
            catch (Exception ex)
            {
                throw _logger.Log(ex);
            }
        }

        public virtual async Task<T> BuscarAsync(object id)
        {

            ApiResponse<T> respuesta;
            try
            {
                HelperAPI helperAPI = new HelperAPI();
                _logger.Log(TraceEventType.Information, $"Buscando todos los Items '{_rutaEntidad}'");
                HttpClient client = helperAPI.InicializaCliente();

                HttpResponseMessage response;
                //string link = $"{appSettings.RutaBase}{_rutaEntidad}?id={id}";
                string link = $"{appSettings.URLBase}{_rutaEntidad}/{id}";

                response = await client.GetAsync(link);
                _logger.Log(TraceEventType.Information, $"Response: {JsonConvert.SerializeObject(response)}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string stringData = await response.Content.ReadAsStringAsync();
                    //_logger.Log(TraceEventType.Information, $"String Response: {stringData}");
                    respuesta = JsonConvert.DeserializeObject<ApiResponse<T>>(stringData);
                    return respuesta.Data;
                }               
                else
                {
                    //string stringData = await response.Content.ReadAsStringAsync();
                    //ErrorExceptionValidation resp = JsonConvert.DeserializeObject<ErrorExceptionValidation>(stringData);
                    //var error = resp.Error.First();
                    //throw new NegocioException($"Código: {response.StatusCode} - Error: {error.Detail}");
                    await ParseoError(response);
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                throw _logger.Log(ex);
            }
        }
        public virtual async Task<T> BuscarUnoAsync()
        {

            ApiResponse<T> respuesta;
            try
            {
                HelperAPI helperAPI = new HelperAPI();
                _logger.Log(TraceEventType.Information, $"Buscando todos los Items '{_rutaEntidad}'");
                HttpClient client = helperAPI.InicializaCliente(); ;

                HttpResponseMessage response;
                string link = $"{appSettings.URLBase}{_rutaEntidad}";

                response = await client.GetAsync(link);
                _logger.Log(TraceEventType.Information, $"Response: {JsonConvert.SerializeObject(response)}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string stringData = await response.Content.ReadAsStringAsync();
                    //_logger.Log(TraceEventType.Information, $"String Response: {stringData}");
                    respuesta = JsonConvert.DeserializeObject<ApiResponse<T>>(stringData);
                    return respuesta.Data;
                }              
                else
                {
                    //string stringData = await response.Content.ReadAsStringAsync();
                    //ErrorExceptionValidation resp = JsonConvert.DeserializeObject<ErrorExceptionValidation>(stringData);
                    //var error = resp.Error.First();
                    //throw new NegocioException($"Código: {response.StatusCode} - Error: {error.Detail}");
                    await ParseoError(response);
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                throw _logger.Log(ex);
            }
        }
        public string EvaluarQueryFilter(QueryFilters filters)
        {
            bool first = true;
            string cadena = string.Empty;
            foreach (var prop in filters.GetType().GetProperties())
            {
                //no evaluo la propiedad Todo
                if (prop.Name.Equals("Todo"))
                { continue; }
                var valor = prop.GetValue(filters, null);
                if (prop.PropertyType == typeof(int))
                {
                    if ((int)valor != 0)
                    {
                        ComponeCadena(ref first, ref cadena, prop, valor);
                        continue;
                    }
                }

                if (prop.PropertyType == typeof(string))
                {
                    if (!string.IsNullOrWhiteSpace((string)valor))
                    {
                        ComponeCadena(ref first, ref cadena, prop, valor);
                        continue;

                    }
                }

                if (prop.PropertyType == typeof(Nullable<DateTime>))
                {
                    if (((DateTime?)valor).HasValue)
                    {
                        ComponeCadena(ref first, ref cadena, prop, valor);
                        continue;

                    }
                }

                if (valor != null)
                {
                    ComponeCadena(ref first, ref cadena, prop, valor);
                    continue;
                }

            }

            return cadena;

        }

        private static void ComponeCadena(ref bool first, ref string cadena, PropertyInfo prop, object valor)
        {
            if (first) { first = false; }
            else { cadena += "&"; }
            cadena += $"{prop.Name}={valor}";
        }

        public virtual async Task<bool> AgregarAsync(T entidad)
        {
            ApiResponse<bool> respuesta;
            try
            {
                HelperAPI helperAPI = new HelperAPI();
                _logger.Log(TraceEventType.Information, "Agregando los datos de la entidad.");
                _logger.Log(TraceEventType.Information, $"A Agregar: {JsonConvert.SerializeObject(entidad)}");

                HttpClient client = helperAPI.InicializaCliente(entidad, out StringContent content);
                client.BaseAddress = new Uri(appSettings.URLBase);
                HttpResponseMessage response;
                string link = $"{_rutaEntidad}";
                response = await client.PostAsync(link, content);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string stringData = await response.Content.ReadAsStringAsync();
                    _logger.Log(TraceEventType.Information, $"stringData: {stringData}");

                    respuesta = JsonConvert.DeserializeObject<ApiResponse<bool>>(stringData);
                    return respuesta.Data;
                }               
                else
                {
                    await ParseoError(response);
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw _logger.Log(ex);
            }
        }

        protected async Task ParseoError(HttpResponseMessage response)
        {
            string stringData = await response.Content.ReadAsStringAsync();
            _logger.Log(TraceEventType.Information, $"stringData: {stringData}");
            ErrorExceptionValidation resp = JsonConvert.DeserializeObject<ErrorExceptionValidation>(stringData);
            if (resp.Error != null)
            {
                var error = resp.Error.First();
                throw new SATIException($"Código: {response.StatusCode} - Error: {error.Detail}");
            }
            else
            {
                var errores = ParseoErroresFluentValidate(stringData);
                string msj = string.Empty;
                foreach (var e in errores)
                {
                    var titulo = e.Titulo;
                    foreach (var m in e.Mensajes)
                    {
                        msj += $"{e.Titulo} - {m}\n";
                    }

                }
                throw new SATIException($"Error(es): {msj} ");
            }
        }

        public virtual async Task<bool> ActualizarAsync(object id, T entidad)
        {
            ApiResponse<bool> respuesta;
            try
            {
                HelperAPI helperAPI = new HelperAPI();
                _logger.Log(TraceEventType.Information, "Actualizando los datos de la entidad");
                _logger.Log(TraceEventType.Information, $"A Modificar: {JsonConvert.SerializeObject(entidad)}");
                HttpClient client = helperAPI.InicializaCliente(entidad,  out StringContent content);
                client.BaseAddress = new Uri(appSettings.URLBase);

                var link = $"{_rutaEntidad}/{id}";

                HttpResponseMessage response = await client.PutAsync(link, content);
                _logger.Log(TraceEventType.Information, $"Response: {JsonConvert.SerializeObject(response)}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string stringData = await response.Content.ReadAsStringAsync();
                    _logger.Log(TraceEventType.Information, $"String Response: {stringData}");
                    respuesta = JsonConvert.DeserializeObject<ApiResponse<bool>>(stringData);
                    return respuesta.Data;
                }
                
                else
                {
                    string stringData = await response.Content.ReadAsStringAsync();
                    _logger.Log(TraceEventType.Information, $"stringData: {stringData}");

                    if (!string.IsNullOrWhiteSpace(stringData))
                    {
                        ErrorExceptionValidation resp = JsonConvert.DeserializeObject<ErrorExceptionValidation>(stringData);
                        if (resp.Error != null)
                        {
                            var error = resp.Error.First();
                            throw new SATIException($"Código: {response.StatusCode} - Error: {error.Detail}");
                        }
                        else
                        {
                            var errores = ParseoErroresFluentValidate(stringData);
                            string msj = string.Empty;
                            foreach (var e in errores)
                            {
                                var titulo = e.Titulo;
                                foreach (var m in e.Mensajes)
                                {
                                    msj += $"{e.Titulo} - {m}\n";
                                }

                            }
                            throw new SATIException($"Error(es): {msj} ");
                        }
                    }
                    else
                    {
                        throw new SATIException($"Código: {response.StatusCode} - Error: Hubo un error. Verifique logs.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw _logger.Log(ex);
            }
        }

        private List<ErroresFluentValidatorVM> ParseoErroresFluentValidate(string stringData)
        {
            var errores = new List<ErroresFluentValidatorVM>();
            var data = stringData.Split(new char[] { '{', '}' })[2];
            var errs = data.Split(new string[] { "]," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in errs)
            {
                var e = item.Split(':');
                var reg = new ErroresFluentValidatorVM();
                reg.Titulo = e[0].Replace("\"", "").Replace("\\", "");
                var msgs = e[1].Split(',');
                reg.Mensajes = new List<string>();
                foreach (var m in msgs)
                {
                    reg.Mensajes.Add(m.Replace("\"", "").Replace("\\", "").Replace("[", "").Replace("]", ""));
                }

                errores.Add(reg);
            }


            return errores;

        }

        public virtual async Task<bool> EliminarAsync(object id)
        {
            ApiResponse<bool> respuesta;
            try
            {
                HelperAPI helperAPI = new HelperAPI();
                _logger.Log(TraceEventType.Information, $"Eliminando datos. Id:{id}");
                HttpClient client = helperAPI.InicializaCliente();
                client.BaseAddress = new Uri(appSettings.URLBase);
                HttpResponseMessage response;
                var link = $"{_rutaEntidad}/{id}";

                response = await client.DeleteAsync(link);
                _logger.Log(TraceEventType.Information, $"Response: {JsonConvert.SerializeObject(response)}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string stringData = await response.Content.ReadAsStringAsync();
                    _logger.Log(TraceEventType.Information, $"String Response: {stringData}");
                    respuesta = JsonConvert.DeserializeObject<ApiResponse<bool>>(stringData);
                    return respuesta.Data;
                }               
                else
                {
                    //string stringData = await response.Content.ReadAsStringAsync();
                    //_logger.Log(TraceEventType.Information, $"stringData: {stringData}");
                    //if (!string.IsNullOrWhiteSpace(stringData))
                    //{
                    //    ErrorExceptionValidation resp = JsonConvert.DeserializeObject<ErrorExceptionValidation>(stringData);
                    //    var error = resp.Error.First();
                    //    throw new NegocioException($"Código: {response.StatusCode} - Error: {error.Detail}");
                    //}
                    //else
                    //{
                    //    throw new NegocioException($"Código: {response.StatusCode} - Error: Hubo un error. Verifique logs.");
                    //}await ParseoError(response);
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw _logger.Log(ex);
            }
        }


    }
}
