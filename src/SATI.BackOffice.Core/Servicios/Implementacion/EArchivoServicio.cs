using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SATI.BackOffice.Core.Datos.Contratos;
using SATI.BackOffice.Core.Servicios.Contrato;
using SATI.BackOffice.Infraestructura;
using SATI.BackOffice.Infraestructura.Entidades;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using SATI.BackOffice.Infraestructura.Entidades.Constantes;
using SATI.BackOffice.Infraestructura.Entidades.Options;
using SATI.BackOffice.Infraestructura.Exceptions;
using SATI.BackOffice.Infraestructura.Intefaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SATI.BackOffice.Core.Servicios.Implementacion
{
    public class EArchivoServicio : Servicio<EArchivo>, IEArchivoServicio
    {

        public EArchivoServicio(IUnitOfWork uow, IOptions<AppSettings> options, ILoggerHelper logger) : base(uow, options, logger)
        {
        }

        public int Actualizar(object id, EArchivo entidad)
        {
            throw new NotImplementedException();
        }

        public int Agregar(EArchivo entidad)
        {
            throw new NotImplementedException();
        }

        public async Task<int> AgregarAsync(EArchivo entidad, bool esTemporal)
        {
            var entemp = entidad;
            entemp.Archivo = string.Empty;
            _logger.Log(TraceEventType.Information, JsonConvert.SerializeObject(entemp));

            entidad.CodigoSistema = entidad.CodigoSistema.ToUpper();
            entidad.Id = Guid.NewGuid().ToString();
            //inicialmente se procede a calcular la ruta donde se almacenará el archivo
            var ruta = CalcularRuta(entidad.CodigoSistema);
            if (string.IsNullOrWhiteSpace(entidad.NombreArchivo))
            {
                throw new BOException("No se ha especificado el nombre del archivo.");
            }
            var ext = entidad.NombreArchivo.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (ext.Length == 1)
            {
                throw new BOException("El nombre del archivo no especifica la extensión. Verifique. ");
            }

            if (esTemporal) { entidad.EstadoEArchivoId = (int)EstadoArchivo.PENDIENTE; }
            else { entidad.EstadoEArchivoId = (int)EstadoArchivo.CONFIRMADO; }

            //se calcula el nombre del archivo
            var nn = esTemporal ?
                GeneradorNombreTemporalArchivo(entidad.CodigoSistema, entidad.CUIT, entidad.RequerimientoId, ext[^1].ToUpper(), entidad.ClaveTemporal) :
                GeneradorNombreDefinitivoArchivo(entidad.CodigoSistema, entidad.CUIT, entidad.SolicitudId, entidad.TipoSolicitud, entidad.RequerimientoId, ext[^1].ToUpper());

            entidad.NombreArchivo = nn;
            entidad.Ruta = ruta;
            entidad.FechaIndexacion = DateTime.Now;

            //se procede a guardar el archivo en la ruta especificada
            #region Guardar archivo desde B64 a disco

            byte[] archBytes;

            archBytes = Convert.FromBase64String(entidad.Archivo.ToString());


            MemoryStream ms = new MemoryStream(archBytes);
            using (Stream fs = new FileStream($"{ruta}\\{nn}", FileMode.Create))
            {
                await ms.CopyToAsync(fs);
            }

            #endregion

            #region Reacondicionamiento de la ruta - Se quita la porción del servidor 

            entidad.Ruta = entidad.Ruta.Replace(_appSettings.RutaFisica, "");

            #endregion

            var sp = Constantes.StoredProcedures.EARCHIVO_INSERT;
            var excluir = new List<string> { "Archivo" };
            var parametros = _repositorio.InferirParametros(entidad, excluir);
            var res = InvokarNQuery(sp, parametros);
            return res;
        }

        public Task<int> AgregarDBAsync(EArchivo entidad)
        {
            throw new NotImplementedException();
        }

        public RespuestaGenerica<EArchivo> Buscar(QueryFilters filters)
        {

            _logger.Log(TraceEventType.Information, $"Ejecutando: {this.GetType().Name}-{MethodBase.GetCurrentMethod()}");
            filters.PageNumber = filters.PageNumber == 0 ? _appSettings.DefaultPageNumber : filters.PageNumber;
            filters.PageSize = filters.PageSize == 0 ? _appSettings.DefaultPageSize : filters.PageSize;

            if (string.IsNullOrWhiteSpace(filters.Sort)) { filters.Sort = "NombreArchivo"; }
            if (string.IsNullOrWhiteSpace(filters.SortDir)) { filters.SortDir = "ASC"; }

            var sp = Constantes.StoredProcedures.EARCHIVO_GET_COUNT;

            List<SqlParameter> parametros = new();
            { new SqlParameter("@codSistema", string.IsNullOrWhiteSpace(filters.Search) ? DBNull.Value : filters.Search); };

            var res = InvokarSpScalar(sp, parametros);
            var cantidad = res.ToString().ToInt();

            if (cantidad <= 0)
            {
                _logger.Log(TraceEventType.Warning, $"No se encontraron registros - Filtro: {JsonConvert.SerializeObject(filters)} - SP: {sp}");
                return new RespuestaGenerica<EArchivo> { Ok = false, Mensaje = Constantes.MensajesError.CAROUSEL_REGISTROS_NO_ENCONTRADOS.Replace("@codSistema", filters.Search) };
            }


            sp = Constantes.StoredProcedures.EARCHIVO_GET_ALL;

            parametros = new List<SqlParameter>();
            if (filters.Id != default)
            {
                parametros.Add(new SqlParameter("@id", filters.Id));
            }
            if (filters.IdRef != default)
            {
                parametros.Add(new SqlParameter("@IdRef", filters.IdRef));
            }
            if (filters.Date.HasValue)
            {
                parametros.Add(new SqlParameter("@Date", filters.Date.Value));
            }
            if (!string.IsNullOrWhiteSpace(filters.FileName))
            {
                parametros.Add(new SqlParameter("@FileName", filters.FileName));
            }
            if (!string.IsNullOrWhiteSpace(filters.CUIT))
            {
                parametros.Add(new SqlParameter("@CUIT", filters.CUIT));
            }
            if (!string.IsNullOrWhiteSpace(filters.Search) && filters.Search.Trim().Length == 3)
            {
                parametros.Add(new SqlParameter("@CodSistema", filters.Search.Trim()));
            }
            if (!string.IsNullOrWhiteSpace(filters.Search) && filters.Search.Trim().Length != 3)
            {
                parametros.Add(new SqlParameter("@ClaveTemporal", filters.Search.Trim()));
            }
            parametros.Add(new SqlParameter("@PageSize", filters.PageSize));
            parametros.Add(new SqlParameter("@PageNumber", filters.PageNumber));

            var registros = InvokarSp2Lst(sp, parametros);

            if (registros.Count > 0)
            {
                #region se calcula su ubicación
                foreach (var item in registros)
                {
                    item.Ruta = GeneraRutaUrl(item);
                }
                #endregion
                _logger.Log(TraceEventType.Information, $"Resgistros encontrados: {registros.Count} - Filtros: {JsonConvert.SerializeObject(filters)} - SP: {sp}");
                return new RespuestaGenerica<EArchivo> { Ok = true, ListaItems = registros, CantidadReg = registros.Count, TotalRegs = cantidad };
            }
            else
            {
                _logger.Log(TraceEventType.Warning, $"No se encontraron registros - Filtros: {JsonConvert.SerializeObject(filters)} - SP: {sp}");
                return new RespuestaGenerica<EArchivo> { Ok = false, Mensaje = Constantes.MensajesError.EARCHIVO_REGISTROS_NO_ENCONTRADOS.Replace("@codSistema", filters.Search) };
            }
        }



        public RespuestaGenerica<EArchivo> BuscarPorId(object id)
        {
            return BuscarPorId(id, false);
        }

        public RespuestaGenerica<EArchivo> BuscarPorId(object id, bool noUrl = true, bool genB64 = false)
        {
            _logger.Log(TraceEventType.Information, $"Ejecutando: {this.GetType().Name}-{MethodBase.GetCurrentMethod()}");
            var sp = Constantes.StoredProcedures.EARCHIVO_GET_BY_ID;
            var parametros = new List<SqlParameter> { new SqlParameter("@Id", id) };
            var registro = InvokarSp2Lst(sp, parametros);
            if (registro.Count == 0)
            {
                _logger.Log(TraceEventType.Warning, $"No se encontraron registros - Id: {id} - SP: {sp} - Msj: {Constantes.MensajesError.CAROUSEL_REGISTRO_NO_ENCONTRADO}");
                return new RespuestaGenerica<EArchivo> { Ok = false, Mensaje = Constantes.MensajesError.CAROUSEL_REGISTRO_NO_ENCONTRADO };
            }
            else
            {
                var file = registro.First();
                if (!genB64)
                {
                    if (!noUrl)
                    {
                        file.Ruta = GeneraRutaUrl(file);
                    }
                    else
                    {
                        if (_appSettings.RutaFisica.Substring(_appSettings.RutaFisica.Length - 1, 1).Equals("\\"))
                        {
                            file.Ruta = $"{_appSettings.RutaFisica}{file.Ruta}";
                        }
                        else
                        {
                            file.Ruta = $"{_appSettings.RutaFisica}\\{file.Ruta}";
                        }
                    }
                }
                else
                {
                    //se genera el B64
                    string ruta;
                    if (_appSettings.RutaFisica.Substring(_appSettings.RutaFisica.Length - 1, 1).Equals("\\"))
                    {
                        ruta = $"{_appSettings.RutaFisica}{file.Ruta}\\{file.NombreArchivo}";
                    }
                    else
                    {
                        ruta = $"{_appSettings.RutaFisica}\\{file.Ruta}\\{file.NombreArchivo}";
                    }

                    //abrimos el archivo
                    var stream = File.OpenRead(ruta);
                    //generamos un contenedor de información
                    var memory = new MemoryStream();
                    //se copia el contenido del stream a la memoria
                    stream.CopyTo(memory);
                    file.Archivo = Convert.ToBase64String(memory.ToArray());

                }
                
                return new RespuestaGenerica<EArchivo> { Ok = true, DataItem = file };
            }
        }

        public bool ConfirmarArchivos(Confirmacion confirmacion)
        {
            _logger.Log(TraceEventType.Information, $"Ejecutando: {this.GetType().Name}-{MethodBase.GetCurrentMethod()}");
            _logger.Log(TraceEventType.Information, $"Clave Temporal: {confirmacion.ClaveTemporal}");

            List<EArchivo> listaNew = new();

            //se tiene que verificar la clave temporal que tenga el formato correcto. codigo de sistema (3)
            //y un valor long de la longitud de ticks (18)
            bool error = false;
            if (confirmacion.ClaveTemporal.Length != 21)
            {
                error = true;
            }

            //===========================================================
            //se podría generar tabla con los sistemas habilitados y validar si el sistema que viene existe en la base
            // - Plantearselo a Charly
            //===========================================================

            if (error)
            {
                throw new BOException("La clave temporal no es correcta. verifique");
            }

            //1-busco los archivos de la clave temporal
            var sp = Constantes.StoredProcedures.EARCHIVO_GET_ALL;
            var parametros = new List<SqlParameter> {
                //new SqlParameter("@codSistema", DBNull.Value),
                //new SqlParameter("@id", DBNull.Value),
                //new SqlParameter("@fechaDesde", DBNull.Value),
                //new SqlParameter("@fechaHasta", DBNull.Value),
                new SqlParameter("@claveTemporal", confirmacion.ClaveTemporal.Trim()),
                new SqlParameter("@PageSize", 999),
                new SqlParameter("@PageNumber", 1)
            };

            var registros = InvokarSp2Lst(sp, parametros);
            if (registros.Count() == 0)
            {
                throw new BOException("No se encuentran Archivos compatibles a la operación actual.");
            }

            //cargamos en la lista nueva los datos obtenidos y se trabajarn con la nueva lista. Esto es por el resguardo
            //de datos que es necesario tener en caso de que el proceso tenga un fallo.
            foreach (var item in registros)
            {
                listaNew.Add(item);
            }


            //2.0- verifico que existan los archivos
            foreach (var item in listaNew)
            {
                string path;
                if (item.Ruta.Substring(item.Ruta.Length - 1, 1).Equals("\\"))
                {
                    path = $"{item.Ruta}{item.NombreArchivo}";
                }
                else
                {
                    path = $"{item.Ruta}\\{item.NombreArchivo}";
                }

                if (!File.Exists(path))
                {
                    _logger.Log(TraceEventType.Warning, $"No se encontro al menos 1 archivo de los archivos agrupados con la clave temporal: {confirmacion.ClaveTemporal}. Analizar el caso.");
                    throw new BOException("No se encontro al menos 1 de los archivos de la Solicitud. No se puede confirmar la Solicitud. ");
                }
            }

            //2.1- cambio estado y renombrar archivo
            foreach (var file in listaNew)
            {
                file.EstadoEArchivoId = (int)EstadoArchivo.CONFIRMADO;
                var ext = file.NombreArchivo.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                //renombrar archivo
                var nnNew = GeneradorNombreDefinitivoArchivo(file.CodigoSistema, file.CUIT, confirmacion.SolicitudId,
                    file.TipoSolicitud, file.RequerimientoId, ext[^1].ToUpper());

                if (file.Ruta.Substring(file.Ruta.Length - 1, 1).Equals("\\"))
                {
                    File.Move($"{file.Ruta}{file.NombreArchivo}", $"{file.Ruta}{nnNew}");
                }
                else
                {
                    File.Move($"{file.Ruta}\\{file.NombreArchivo}", $"{file.Ruta}\\{nnNew}");
                }
                file.NombreArchivo = nnNew;
                file.ClaveTemporal = string.Empty;
                file.SolicitudId = confirmacion.SolicitudId;
            }
            //3-confirmar registros modificados.
            var cantidad = listaNew.Count;
            var contador = 0;

            try
            {
                foreach (var item in listaNew)
                {
                    contador++;
                    sp = Constantes.StoredProcedures.EARCHIVO_UPDATE;
                    List<string> excluir = new() { "Archivo" };
                    parametros = _repositorio.InferirParametros(item, excluir);
                    var res = InvokarNQuery(sp, parametros, true, contador == cantidad);
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                //se procede a devolver el nombre de los archivos por el error transaccional
                foreach (var file in listaNew)
                {
                    var oldFile = registros.SingleOrDefault(x => x.Id == file.Id);
                    if (oldFile != null)
                    {
                        if (file.Ruta.Substring(file.Ruta.Length - 1, 1).Equals("\\"))
                        {
                            File.Move($"{file.Ruta}{file.NombreArchivo}", $"{file.Ruta}{oldFile.NombreArchivo}");
                        }
                        else
                        {
                            File.Move($"{file.Ruta}\\{file.NombreArchivo}", $"{file.Ruta}\\{oldFile.NombreArchivo}");
                        }
                    }
                }

                throw new BOException("Se produjo un error al intentar grabar en la base. Verifique.");
            }

            return true;
        }

        public bool Quitar(object id)
        {
            _logger.Log(TraceEventType.Information, $"Ejecutando: {this.GetType().Name}-{MethodBase.GetCurrentMethod()}");
            var archivo = BuscarPorId(id, false);
            if (archivo.Ok)
            {
                var sp = Constantes.StoredProcedures.EARCHIVO_DELETE;
                var p = new SqlParameter("@Id", id);
                var res = InvokarNQuery(sp, p);
                if (res > 0)
                {
                    //se procede a eliminar el archivo 
                    if (File.Exists(archivo.DataItem.Ruta))
                    {
                        File.Delete(archivo.DataItem.Ruta);
                        _logger.Log(TraceEventType.Information, $"Se logro eliminar el archivo. Archivo: {JsonConvert.SerializeObject(archivo.DataItem)}");
                    }

                    return true;
                }
                _logger.Log(TraceEventType.Warning, $"No se pudo eliminar el registro del archivo. Archivo: {JsonConvert.SerializeObject(archivo.DataItem)}");
                return false;
            }
            else
            {
                _logger.Log(TraceEventType.Warning, $"El archivo no fue encontrado. ID: {id}");
                return false;
            }

        }

        public RespuestaGenerica<EArchivo> TraerTodo()
        {
            throw new NotImplementedException();
        }
    }
}
