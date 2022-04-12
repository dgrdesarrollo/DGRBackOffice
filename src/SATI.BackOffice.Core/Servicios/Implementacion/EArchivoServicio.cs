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
            entidad.CodigoSistema = entidad.CodigoSistema.ToUpper();
            entidad.Id = Guid.NewGuid();
            //inicialmente se procede a calcular la ruta donde se almacenará el archivo
            var ruta = CalcularRuta(entidad.CodigoSistema);
            var ext = entidad.NombreArchivo.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

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
            var archBytes = Convert.FromBase64String(entidad.Archivo.ToString());
            MemoryStream ms = new MemoryStream(archBytes);
            using (Stream fs = new FileStream($"{ruta}\\{nn}", FileMode.Create))
            {
                await ms.CopyToAsync(fs);
            }

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
            parametros = new List<SqlParameter> {
                new SqlParameter("@codSistema", string.IsNullOrWhiteSpace(filters.Search)?DBNull.Value:filters.Search),
                new SqlParameter("@id", filters.Id!=default?filters.Id:DBNull.Value),
                new SqlParameter("@fechaDesde", filters.Date.HasValue?filters.Date.Value:DBNull.Value),
                new SqlParameter("@fechaHasta", filters.Date.HasValue?filters.Date.Value.AddDays(1):DBNull.Value),
                new SqlParameter("@claveTemporal", DBNull.Value),
                new SqlParameter("@size", filters.PageSize),
                new SqlParameter("@Pagina", filters.PageNumber) };

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
            return BuscarPorId(id, true);
        }

        public RespuestaGenerica<EArchivo> BuscarPorId(object id, bool generarUrl = true)
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
                if (generarUrl)
                {
                    file.Ruta = GeneraRutaUrl(file);
                }
                _logger.Log(TraceEventType.Information, $"Registro Encontrado - Carousel: {JsonConvert.SerializeObject(file)} - SP: {sp}");
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
                new SqlParameter("@size", 200),
                new SqlParameter("@Pagina", 1) };

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
            catch(Exception ex)
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
                            File.Move($"{file.Ruta}\\{file.NombreArchivo}", $"{file.Ruta}\\{oldFile.no}");
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
