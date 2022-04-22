using SATI.BackOffice.Infraestructura.Intefaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SATI.BackOffice.Core.Datos.Contratos;
using SATI.BackOffice.Core.Servicios.Contrato;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using SATI.BackOffice.Infraestructura.Entidades.Options;
using System.Collections.Generic;
using System.IO;
using System;
using SATI.BackOffice.Infraestructura.Exceptions;
using System.Diagnostics;
using System.Linq;
using SATI.BackOffice.Infraestructura.Entidades;
using System.Text;

namespace SATI.BackOffice.Core.Servicios.Implementacion
{
    public class Servicio<T> : IServicio<T> where T : IEntidades
    {
        #region Propiedades
        protected readonly ILoggerHelper _logger;
        protected readonly IRepository<T> _repositorio;
        protected readonly IUnitOfWork _uow;
        protected readonly AppSettings _appSettings;
        private List<string> extensiones;

        #endregion

        #region constructor
        public Servicio(IUnitOfWork unitOfWork,IOptions<AppSettings> options, ILoggerHelper logger)
        {
            _uow = unitOfWork;
            _repositorio = _uow.GetRepository<T>();
            _appSettings = options.Value;
            _logger = logger;
            extensiones = _appSettings.Extensiones.Split(new char[] { ',' }).Select(x => x).ToList();
        }
        
        #endregion

        public virtual List<T> InvokarSp2Lst(string sp, List<SqlParameter> parametros)
        {
            return _repositorio.InvokarSp2Lst(sp, parametros);
        }

        public virtual int InvokarNQuery(string sp, List<SqlParameter> parametros, bool esTransaccional = false, bool elUltimo = true)
        {
            return _repositorio.InvokarSpNQuery(sp, parametros, esTransaccional, elUltimo);
        }

        public object InvokarSpScalar(string sp, List<SqlParameter> parametros, bool esTransacciona = false, bool elUltimo = true)
        {
            return _repositorio.InvokarSpScalar(sp, parametros);
        }

        public virtual RespuestaGenerica<bool> Commit()
        {
           return _uow.SaveChanges(true);
        }

        public List<T> InvokarSp2Lst(string sp)
        {
            return _repositorio.InvokarSp2Lst(sp);
        }

        public List<T> InvokarSp2Lst(string sp, SqlParameter parametro)
        {
            return _repositorio.InvokarSp2Lst(sp, parametro);
        }

        public int InvokarNQuery(string sp, SqlParameter parametro, bool esTransaccional = false, bool elUltimo = true)
        {
            return _repositorio.InvokarSpNQuery(sp, parametro, esTransaccional, elUltimo);
        }

        public object InvokarSpScalar(string sp, SqlParameter parametro, bool esTransacciona = false, bool elUltimo = true)
        {
            return _repositorio.InvokarSpScalar(sp, parametro, esTransacciona, elUltimo);

        }

        public string GeneradorNombreTemporalArchivo(string codigoSistema,string cuit,int reqId,string extArchivo, string ClaveTemporal)
        {
            #region Validación Formación del Nombre del Archivo
            StringBuilder texto = new();
            bool first = true;
            bool error = false;
            if(string.IsNullOrWhiteSpace(codigoSistema) || codigoSistema.Length != 3)
            {
                texto.Append("Error en Codigo de Sistema. Debe tener 3 caractes alfanumericos.");
                first = false;
                error = true;
            }
            if (string.IsNullOrWhiteSpace(cuit) || cuit.Length != 11)
            {
                if (first) { first = false; } else { texto.Append('-'); }
                texto.Append("Error en CUIT. 11 digitos debe tener.");
                error = true;
            }
            if (cuit.ToLong().ToString().Length != 11)
            {
                if (first) { first = false; } else { texto.Append('-'); }
                texto.Append("Error en CUIT. Existen caracteres");
                error = true;
            }
            if (reqId==0 || reqId.ToString().Length > 3)
            {
                if (first) { first = false; } else { texto.Append('-'); }
                texto.Append("Error en Requerimiento. No se especificó.");
                error = true;
            }
            if (!extensiones.Contains(extArchivo))
            {
                if (first) { first = false; } else { texto.Append('-'); }
                texto.Append($"La extensión {extArchivo} no es admitida.");
                error = true;
            }
            if (error)
            {
                _logger.Log(TraceEventType.Warning, $"Parámetros: codigoSistema: {codigoSistema}, numeroCuenta: {cuit}, reqId: {reqId}, extArchivo: {extArchivo} ");
                throw new BOException($"Alguno de los parametros detinados a la conformación del nombre del archivo no son validos. {texto}");
            }
            #endregion

            return $"{codigoSistema}_{cuit}_{reqId.ToString().PadLeft(3,'0')}_{ClaveTemporal}.{extArchivo}";
        }

        public string GeneradorNombreDefinitivoArchivo(string codigoSistema,string cuit,string solicitudId,string tipoSolicitudId,int reqId, string extArchivo)
        {

            #region Validación Formación del Nombre del Archivo
            StringBuilder texto = new();
            bool first = true;
            bool error = false;
            if (string.IsNullOrWhiteSpace(codigoSistema) || codigoSistema.Length != 3)
            {
                texto.Append("Error en Codigo de Sistema. Debe tener 3 caractes alfanumericos.");
                first = false;
                error = true;
            }
            if (string.IsNullOrWhiteSpace(cuit) || cuit.Length != 11)
            {
                if (first) { first = false; } else { texto.Append('-'); }
                texto.Append("Error en CUIT. 11 digitos debe tener.");
                error = true;
            }
            if (cuit.ToLong().ToString().Length != 11)
            {
                if (first) { first = false; } else { texto.Append('-'); }
                texto.Append("Error en CUIT. Existen caracteres");
                error = true;
            }
            if (string.IsNullOrWhiteSpace(solicitudId) || solicitudId.Length > 10)
            {
                if (first) { first = false; } else { texto.Append('-'); }
                texto.Append("Error en Solicitud. Longitud de la solicitud mayor a 10 digitos");
                error = true;
            }
            if (solicitudId.Length < 10)
            {
                solicitudId = solicitudId.PadLeft(10, '0');
            }
            if (string.IsNullOrWhiteSpace(tipoSolicitudId) || tipoSolicitudId.Length != 3)
            {
                if (first) { first = false; } else { texto.Append('-'); }
                texto.Append("Error en tipoSolicitudId.");
                error = true;
            }
            if (reqId == 0 || reqId.ToString().Length > 3)
            {
                if (first) { first = false; } else { texto.Append('-'); }
                texto.Append("Error en Requerimiento. No se especificó.");
                error = true;
            }        
            if (error)
            {
                _logger.Log(TraceEventType.Warning, $"Parámetros: codigoSistema: {codigoSistema}, numeroCuenta: {cuit}, reqId: {reqId}, extArchivo: {extArchivo} ");
                throw new BOException($"Alguno de los parametros detinados a la conformación del nombre del archivo no son validos. {texto}");
            }
            #endregion

            return $"{codigoSistema}_{cuit}_{solicitudId}_{tipoSolicitudId}_{reqId.ToString().PadLeft(3, '0')}.{extArchivo}";
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
                ruta = $"{_appSettings.RutaFisica}\\{codigoSistema}\\{fecha.Year}";
            }
            else
            {
                ruta = $"{_appSettings.RutaFisica}{codigoSistema}\\{fecha.Year}";
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

        internal void GeneradorDeRuta(string ruta)
        {
            if (!Directory.Exists(ruta))
            {
                Directory.CreateDirectory(ruta);
            }
        }

        internal string GeneraRutaUrl(EArchivo eArchivo)
        {
            var nn = eArchivo.NombreArchivo;
            var ruta = eArchivo.Ruta.Replace(_appSettings.RutaFisica, "").Replace("\\", "/");
            if (ruta.Substring(0, 1).Equals("/")) { ruta = ruta.Substring(1, ruta.Length - 1); }
            if (_appSettings.URLRepositorio.Substring(_appSettings.URLRepositorio.Length - 1, 1).Equals("/"))
            {
                ruta = $"{_appSettings.URLRepositorio}{ruta}/{nn}";
            }
            else
            {
                ruta = $"{_appSettings.URLRepositorio}/{ruta}/{nn}";
            }

            return ruta;
        }
    }
}
