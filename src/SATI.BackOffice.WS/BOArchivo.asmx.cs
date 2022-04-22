using Newtonsoft.Json;
using SATI.BackOffice.WS.Core.Servicios.Contratos;
using SATI.BackOffice.WS.Core.Servicios.Implementaciones;
using SATI.BackOffice.WS.Infra.Entidades;
using SATI.BackOffice.WS.Infra.Helpers;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Web.Services;

namespace SATI.BackOffice.WS
{
    /// <summary>
    /// Descripción breve de BOArchivo
    /// </summary>
    [WebService(Namespace = "http://boarchivo.dgr.gov.ar/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class BOArchivo : System.Web.Services.WebService
    {
        private readonly IArchivoServicio _archivoServicio;
        private readonly LoggerHelper _logger;
        public BOArchivo(IArchivoServicio archivoServicio)
        {
            _logger = new LoggerHelper();
            _archivoServicio = archivoServicio;
        }

        public BOArchivo():this(new ArchivoServicio())
        {

        }


        [WebMethod]
        public RespuestaGenerica<EArchivo> GetArchivos(QueryFilters filters)
        {
            try
            {
                _logger.Log(TraceEventType.Information,  $"{this.GetType().Name} - {MethodBase.GetCurrentMethod()}");
                _logger.Log(TraceEventType.Information,$"Parametros: {JsonConvert.SerializeObject(filters)}");
                return _archivoServicio.GetArchivos(filters);
            }
            catch (Exception ex)
            {
                _logger.Log(TraceEventType.Information,$"{this.GetType().Name}-{MethodBase.GetCurrentMethod()}");
                _logger.Log(ex);
                return new RespuestaGenerica<EArchivo>
                {
                    Ok = false,
                    Mensaje = "Se produjo un error al intentar recuperar los archivos.",
                    MensajeOriginal = ex.Message
                };
            }
        }

        [WebMethod(Description = "Permite obtener el archivo solicitado según el Id que se envíe")]
        public RespuestaGenerica<EArchivo> GetArchivo(Guid id)
        {
            try
            {
                _logger.Log(TraceEventType.Information,$"{this.GetType().Name} - {MethodBase.GetCurrentMethod()}");
                _logger.Log(TraceEventType.Information,$"Parametro: id:{id}");
                return _archivoServicio.GetArchivo(id);
            }
            catch (Exception ex)
            {
                _logger.Log(TraceEventType.Information,$"{this.GetType().Name}-{MethodBase.GetCurrentMethod()}");
                _logger.Log(ex);
                return new RespuestaGenerica<EArchivo>
                {
                    Ok = false,
                    Mensaje = "Se produjo un error al intentar recuperar los archivos.",
                    MensajeOriginal = ex.Message
                };
            }
        }

        [WebMethod(Description = "Permite obtener el archivo solicitado según el Id que se envíe. Se puede definir adicionalmente si se desea que la ruta sea una URL o una ruta física.")]
        public RespuestaGenerica<EArchivo> GetArchivoNoUrl(Guid id, bool noUrl)
        {
            try
            {
                _logger.Log(TraceEventType.Information,$"{this.GetType().Name} - {MethodBase.GetCurrentMethod()}");
                _logger.Log(TraceEventType.Information,$"Parametros: id:{id} noUrl: {noUrl}");
                return _archivoServicio.GetArchivo(id, noUrl);
            }
            catch (Exception ex)
            {
                _logger.Log(TraceEventType.Information,$"{this.GetType().Name}-{MethodBase.GetCurrentMethod()}");
                _logger.Log(ex);
                return new RespuestaGenerica<EArchivo>
                {
                    Ok = false,
                    Mensaje = "Se produjo un error al intentar recuperar los archivos.",
                    MensajeOriginal = ex.Message
                };
            }
        }

        [WebMethod(Description = "Permite obtener el archivo solicitado según el Id que se envíe. Este metodo devolverá el archivo en B64.")]
        public RespuestaGenerica<EArchivo> GetFileB64(Guid id)
        {
            try
            {
                _logger.Log(TraceEventType.Information,$"{this.GetType().Name} - {MethodBase.GetCurrentMethod()}");
                _logger.Log(TraceEventType.Information,$"Parametros: Id: {id}");

                return _archivoServicio.GetFileB64(id);
            }
            catch (Exception ex)
            {
                _logger.Log(TraceEventType.Information,$"{this.GetType().Name}-{MethodBase.GetCurrentMethod()}");
                _logger.Log(ex);
                return new RespuestaGenerica<EArchivo>
                {
                    Ok = false,
                    Mensaje = "Se produjo un error al intentar recuperar los archivos.",
                    MensajeOriginal = ex.Message
                };
            }
        }


        [WebMethod(Description = "Alta de un archivo")]
        public RespuestaGenerica<bool> AgregarArchivo(EArchivo entidad, bool esTemporal)
        {
            try
            {
                _logger.Log(TraceEventType.Information,$"{this.GetType().Name} - {MethodBase.GetCurrentMethod()}");
                _logger.Log(TraceEventType.Information,$"Parametros: Entidad: {JsonConvert.SerializeObject(entidad)} - esTemporal: {esTemporal}");
                return _archivoServicio.AgregarArchivo(entidad, esTemporal);
            }
            catch (Exception ex)
            {
                _logger.Log(TraceEventType.Information,$"{this.GetType().Name}-{MethodBase.GetCurrentMethod()}");
                _logger.Log(ex);
                return new RespuestaGenerica<bool>
                {
                    Ok = false,
                    Mensaje = "Se produjo un error al intentar recuperar los archivos.",
                    MensajeOriginal = ex.Message
                };
            }
        }

        [WebMethod(Description = "Se confirman los archivos upload temporalmente.")]
        public RespuestaGenerica<bool> ConfirmarArchivos(Confirmacion entidad)
        {
            try
            {
                _logger.Log(TraceEventType.Information,$"{this.GetType().Name} - {MethodBase.GetCurrentMethod()}");
                _logger.Log(TraceEventType.Information,$"Parametros: Confirmacion: {JsonConvert.SerializeObject(entidad)}");
                return _archivoServicio.ConfirmarArchivos(entidad);
            }
            catch (Exception ex)
            {
                _logger.Log(TraceEventType.Information,$"{this.GetType().Name}-{MethodBase.GetCurrentMethod()}");
                _logger.Log(ex);
                return new RespuestaGenerica<bool>
                {
                    Ok = false,
                    Mensaje = "Se produjo un error al intentar recuperar los archivos.",
                    MensajeOriginal = ex.Message
                };
            }
        }

        [WebMethod(Description = "Se obtienen una clave temporal para el alta de archivos.")]
        public RespuestaGenerica<string> GetIdTemp(string codSistema)
        {
            try
            {
                _logger.Log(TraceEventType.Information,$"{this.GetType().Name} - {MethodBase.GetCurrentMethod()}");
                _logger.Log(TraceEventType.Information,$"Parametros: codSistema: {codSistema}");

                return _archivoServicio.GetIdTemp(codSistema);
            }
            catch (Exception ex)
            {
                _logger.Log(TraceEventType.Information,$"{this.GetType().Name}-{MethodBase.GetCurrentMethod()}");
                _logger.Log(ex);
                return new RespuestaGenerica<string>
                {
                    Ok = false,
                    Mensaje = "Se produjo un error al intentar recuperar los archivos.",
                    MensajeOriginal = ex.Message
                };
            }
        }
    }
}
