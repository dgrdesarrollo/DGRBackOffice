using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SATI.BackOffice.Core.Datos.Contratos;
using SATI.BackOffice.Core.Servicios.Contrato;
using SATI.BackOffice.Infraestructura;
using SATI.BackOffice.Infraestructura.Entidades;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using SATI.BackOffice.Infraestructura.Entidades.Options;
using SATI.BackOffice.Infraestructura.Intefaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SATI.BackOffice.Core.Servicios.Implementacion
{
    public class NormativaLegalServicio:Servicio<dl_documentos>, INormativaLegalServicio
    {
        private readonly ILoggerHelper _logger;
        public NormativaLegalServicio(IUnitOfWork uow, IOptions<AppSettings> options, ILoggerHelper logger) : base(uow, options)
        {
            _logger = logger;

        }

        public int Actualizar(object id, dl_documentos entidad)
        {
            _logger.Log(TraceEventType.Information, $"Ejecutando: {this.GetType().Name}-{MethodBase.GetCurrentMethod()}");
            string sp = Constantes.StoredProcedures.NORMA_LEGAL_UPDATE;
            List<string> excluir = new List<string> { "Tipo","Pdf" };
            List<SqlParameter> parametros = _repositorio.InferirParametros(entidad, excluir);
            int res = InvokarNQuery(sp, parametros);
            return res;
        }

        public int Agregar(dl_documentos entidad)
        {
            throw new NotImplementedException();
        }

        public async Task<int> AgregarAsync(dl_documentos entidad)
        {
            _logger.Log(TraceEventType.Information, $"Ejecutando: {this.GetType().Name}-{MethodBase.GetCurrentMethod()}");

            #region Guardar archivo desdeB64 a disco
            byte[] archBytes = Convert.FromBase64String(entidad.Pdf.ToString());
            MemoryStream ms = new MemoryStream(archBytes);
            using (Stream fs = new FileStream($"{entidad.UbicacionFisica}\\{entidad.dl_file}", FileMode.Create))
            {
                await ms.CopyToAsync(fs);
            }

            #endregion

            string sp = Constantes.StoredProcedures.NORMA_LEGAL_INSERT;

            List<string> excluir = new List<string> { "dl_id", "Pdf","Tipo" };
            List<SqlParameter> parametros = _repositorio.InferirParametros(entidad, excluir);
            _logger.Log(TraceEventType.Information, $"Excluidos: {JsonConvert.SerializeObject(excluir)}");
            _logger.Log(TraceEventType.Information, $"Parametros: {JsonConvert.SerializeObject(parametros)}");
            int res = InvokarNQuery(sp, parametros);
            return res;

        }

        public RespuestaGenerica<dl_documentos> Buscar(QueryFilters filters)
        {
            _logger.Log(TraceEventType.Information, $"Ejecutando: {this.GetType().Name}-{MethodBase.GetCurrentMethod()}");
            filters.PageNumber = filters.PageNumber == 0 ? _appSettings.DefaultPageNumber : filters.PageNumber;
            filters.PageSize = filters.PageSize == 0 ? _appSettings.DefaultPageSize : filters.PageSize;

            if (string.IsNullOrWhiteSpace(filters.Sort)) { filters.Sort = "Orden"; }
            if (string.IsNullOrWhiteSpace(filters.SortDir)) { filters.SortDir = "ASC"; }

            var sp = Constantes.StoredProcedures.NORMA_LEGAL_GET_COUNT;
            List<SqlParameter> parametros = new List<SqlParameter>();

            var res = InvokarSpScalar(sp, parametros);
            var cantidad = res.ToString().ToInt();

            if (cantidad <= 0)
            {
                _logger.Log(TraceEventType.Warning, $"No se encontraron registros - Filtro: {JsonConvert.SerializeObject(filters)} - SP: {sp}");
                return new RespuestaGenerica<dl_documentos> { Ok = false, Mensaje = Constantes.MensajesError.NORMA_LEGAL_REGISTROS_NO_ENCONTRADOS };
            }

            sp = Constantes.StoredProcedures.NORMA_LEGAL_GET_ALL;
            parametros = new List<SqlParameter> {
                new SqlParameter("@size", filters.PageSize),
                new SqlParameter("@tipo", filters.IdRef),
                new SqlParameter("@Pagina", filters.PageNumber) };
            var registros = InvokarSp2Lst(sp, parametros);
            if (registros.Count > 0)
            {
                #region se calcula su ubicación
                foreach (var item in registros)
                {
                    if (string.IsNullOrWhiteSpace(item.UbicacionFisica))
                    {
                        continue;
                    }
                    var nn = item.dl_file;
                    var ruta = item.UbicacionFisica.Replace(_appSettings.RutaFisica, "").Replace("\\", "/");
                    if (_appSettings.URLRepositorio.Substring(_appSettings.URLRepositorio.Length - 1, 1).Equals("/"))
                    {
                        item.Pdf = $"{_appSettings.URLRepositorio}{ruta}/{nn}";
                    }
                    else
                    {
                        item.Pdf = $"{_appSettings.URLRepositorio}/{ruta}/{nn}";
                    }
                }
                #endregion
                _logger.Log(TraceEventType.Information, $"Resgistros encontrados: {registros.Count} - Filtros: {JsonConvert.SerializeObject(filters)} - SP: {sp}");
                return new RespuestaGenerica<dl_documentos> { Ok = true, ListaItems = registros, CantidadReg = registros.Count, TotalRegs = cantidad };
            }
            else
            {
                _logger.Log(TraceEventType.Warning, $"No se encontraron registros - Filtros: {JsonConvert.SerializeObject(filters)} - SP: {sp}");
                return new RespuestaGenerica<dl_documentos> { Ok = false, Mensaje = Constantes.MensajesError.NORMA_LEGAL_REGISTROS_NO_ENCONTRADOS };
            }
        }

        public RespuestaGenerica<dl_documentos> BuscarPorId(object id)
        {
            _logger.Log(TraceEventType.Information, $"Ejecutando: {this.GetType().Name}-{MethodBase.GetCurrentMethod()}");
            var sp = Constantes.StoredProcedures.NORMA_LEGAL_GET_BY_ID;
            var parametros = new List<SqlParameter> { new SqlParameter("@dl_id", id) };
            var registro = InvokarSp2Lst(sp, parametros);
            if (registro.Count == 0)
            {
                _logger.Log(TraceEventType.Warning, $"No se encontraron registros - Id: {id} - SP: {sp}");
                return new RespuestaGenerica<dl_documentos> { Ok = false, Mensaje = Constantes.MensajesError.CAROUSEL_REGISTRO_NO_ENCONTRADO };
            }
            else
            {
                var car = registro.First();
                _logger.Log(TraceEventType.Information, $"Registro Encontrado - Carousel: {JsonConvert.SerializeObject(car)} - SP: {sp}");
                return new RespuestaGenerica<dl_documentos> { Ok = true, DataItem = car };
            }
        }

        public string CalcularRuta(string codigoSistema)
        {
            var fecha = DateTime.Today;
            string ruta = string.Empty;
            if (string.IsNullOrWhiteSpace(codigoSistema))
            {
                codigoSistema = "XX";
            }
            //se calcula la ruta segun la ruta base y teniendo en cuenta que es carousel
            if (!_appSettings.RutaFisica.Substring(_appSettings.RutaFisica.Length - 1, 1).Equals("\\"))
            {
                ruta = $"{_appSettings.RutaFisica}\\Digesto\\{codigoSistema}\\{fecha.Year}";
            }
            else
            {
                ruta = $"{_appSettings.RutaFisica}Digesto\\{codigoSistema}\\{fecha.Year}";
            }
            //Genera directorio Año
            GeneradorDeRuta(ruta);

            //verificarmos que exista el directorio mes, sino lo crea
            ruta += $"\\{fecha.Month.ToString().PadLeft(2, '0')}";
            //Genera directorio mes
            GeneradorDeRuta(ruta);

            //Verificamos si existe el directiro día, sino lo crea
            ruta += $"\\{fecha.Day.ToString().PadLeft(2, '0')}";
            //Genera directorio dia
            GeneradorDeRuta(ruta);


            return ruta;
        }

        public bool Quitar(object Id)
        {
            throw new NotImplementedException();
        }

        public RespuestaGenerica<dl_documentos> TraerTodo()
        {
            throw new NotImplementedException();
        }
    }
}
