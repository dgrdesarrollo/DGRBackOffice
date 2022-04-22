using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SATI.BackOffice.Core.Servicios.Contrato;
using SATI.BackOffice.Infraestructura.Entidades;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using SATI.BackOffice.Infraestructura.Entidades.Options;
using SATI.BackOffice.Infraestructura.Intefaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace SATI.BackOffice.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class BOArchivoController : ControllerBase
    {
        private readonly IEArchivoServicio _eArchivoServicio;
        private readonly ILogger<BOArchivoController> _logger;
        private readonly IUriService _uriService;
        private readonly AppSettings _appSettings;

        public BOArchivoController(IEArchivoServicio eArchivoServicio, ILogger<BOArchivoController> logger,
            IOptions<AppSettings> options, IUriService uriService)
        {
            _logger = logger;
            _uriService = uriService;
            _appSettings = options.Value;
            _eArchivoServicio = eArchivoServicio;
        }

        [HttpGet(Name = nameof(GetArchivos))]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<EArchivo>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetArchivos([FromQuery] QueryFilters filters)
        {

            _logger.LogInformation($"{this.GetType().Name} - {MethodBase.GetCurrentMethod().Name}");
            filters.PageNumber = filters.PageNumber == 0 ? _appSettings.DefaultPageNumber : filters.PageNumber;
            filters.PageSize = filters.PageSize == 0 ? _appSettings.DefaultPageSize : filters.PageSize;

            PagedList<EArchivo> listaMetadata;
            var resp = _eArchivoServicio.Buscar(filters);

            if (!resp.Ok) { resp.ListaItems = new List<EArchivo>(); }

            var listado = resp.ListaItems;

            //generando metadata
            listaMetadata = PagedList<EArchivo>.Create(listado, filters.PageNumber, filters.PageSize);

            var metadata = new Metadata
            {
                TotalCount = listaMetadata.TotalCount,
                PageSize = listaMetadata.PageSize,
                CurrentPage = listaMetadata.CurrentPage,
                TotalPages = listaMetadata.TotalPages,
                HasNextPage = listaMetadata.HasNextPage,
                HasPreviousPage = listaMetadata.HasPreviousPage,
                NextPageUrl = _uriService.GetPostPaginationUri(filters, Url.RouteUrl(nameof(GetArchivos))).ToString(),
                PreviousPageUrl = _uriService.GetPostPaginationUri(filters, Url.RouteUrl(nameof(GetArchivos))).ToString()
            };


            var response = new ApiResponse<IEnumerable<EArchivo>>(listaMetadata)
            {
                Meta = metadata
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            _logger.LogInformation($"Completado: {JsonConvert.SerializeObject(metadata)}");

            return Ok(response);

        }

        [HttpGet]
        [Route("[action]/{codigoSistema}")]
        public IActionResult GetIdTemp(string codigoSistema)
        {
            _logger.LogInformation($"{this.GetType().Name} - {MethodBase.GetCurrentMethod().Name}");

            if (string.IsNullOrWhiteSpace(codigoSistema) || codigoSistema.Length != 3)
            {
                return BadRequest("El código de sistema no es valido.");
            }
            string tempCod = $"{codigoSistema.ToUpper()}{DateTime.Now.Ticks}";

            ApiResponse<string> resp = new ApiResponse<string>(tempCod);

            return Ok(resp);
        }

        [HttpGet("{id:Guid}")]
        public IActionResult Get(Guid id)
        {
            _logger.LogInformation($"{this.GetType().Name} - {MethodBase.GetCurrentMethod().Name}");
            RespuestaGenerica<EArchivo> car = _eArchivoServicio.BuscarPorId(id,false);

            ApiResponse<EArchivo> resp = new ApiResponse<EArchivo>(car.DataItem);
            return Ok(resp);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetNoUrl(Guid id, bool noUrl)
        {
            _logger.LogInformation($"{this.GetType().Name} - {MethodBase.GetCurrentMethod().Name}");
            RespuestaGenerica<EArchivo> car = _eArchivoServicio.BuscarPorId(id, noUrl);

            ApiResponse<EArchivo> resp = new ApiResponse<EArchivo>(car.DataItem);
            return Ok(resp);
        }

        [HttpGet]
        [Route("[action]/{id:Guid}")]
        public IActionResult GetFileB64(Guid id)
        {
            _logger.LogInformation($"{this.GetType().Name} - {MethodBase.GetCurrentMethod().Name}");
            RespuestaGenerica<EArchivo> car = _eArchivoServicio.BuscarPorId(id, true, true);

            ApiResponse<EArchivo> resp = new ApiResponse<EArchivo>(car.DataItem);
            return Ok(resp);
        }

        [HttpPost]
        public async Task<IActionResult> Post(EArchivo archivo, bool esTemporal)
        {
            _logger.LogInformation($"{this.GetType().Name} - {MethodBase.GetCurrentMethod().Name}");

            await _eArchivoServicio.AgregarAsync(archivo, esTemporal);
            return Ok(new ApiResponse<bool>(true));
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult ConfirmarArchivos(Confirmacion confirmacion)
        {
            _logger.LogInformation($"{this.GetType().Name} - {MethodBase.GetCurrentMethod().Name}");
            _eArchivoServicio.ConfirmarArchivos(confirmacion);
            return Ok(new ApiResponse<bool>(true));
        }

        //[HttpPut("{id:Guid}")]
        //public IActionResult Put(Guid id, EArchivo archivo)
        //{
        //    _logger.LogInformation($"{this.GetType().Name} - {MethodBase.GetCurrentMethod().Name}");
        //    _eArchivoServicio.Actualizar(id, archivo);
        //    return Ok(new ApiResponse<bool>(true));

        //}

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            var res = _eArchivoServicio.Quitar(id);
            var response = new ApiResponse<bool>(res);
            return Ok(response);
        }
    }
}
