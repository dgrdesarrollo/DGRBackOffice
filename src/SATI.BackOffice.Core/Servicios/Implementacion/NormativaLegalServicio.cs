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
using System.Linq;
using System.Reflection;

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
            var sp = Constantes.StoredProcedures.NORMA_LEGAL_UPDATE;
            var excluir = new List<string> { "Tipo","Pdf" };
            var parametros = _repositorio.InferirParametros(entidad, excluir);
            var res = InvokarNQuery(sp, parametros);
            return res;
        }

        public int Agregar(dl_documentos entidad)
        {
            _logger.Log(TraceEventType.Information, $"Ejecutando: {this.GetType().Name}-{MethodBase.GetCurrentMethod()}");
            var sp = Constantes.StoredProcedures.NORMA_LEGAL_INSERT;
            var excluir = new List<string> { "dlt_id", "Tipo", "Pdf" };
            var parametros = _repositorio.InferirParametros(entidad, excluir);
            var res = InvokarNQuery(sp, parametros);
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
                new SqlParameter("@Pagina", filters.PageNumber) };
            var registros = InvokarSp2Lst(sp, parametros);
            if (registros.Count > 0)
            {
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
