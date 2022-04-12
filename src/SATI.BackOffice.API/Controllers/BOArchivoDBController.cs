using Microsoft.AspNetCore.Http;
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
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace SATI.BackOffice.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class BOArchivoDBController : ControllerBase
    {
        private readonly IEArchivoServicio _eArchivoServicio;
        private readonly ILogger<BOArchivoDBController> _logger;
        private readonly IUriService _uriService;
        private readonly AppSettings _appSettings;

        public BOArchivoDBController(IEArchivoServicio eArchivoServicio, ILogger<BOArchivoDBController> logger,
            IOptions<AppSettings> options, IUriService uriService)
        {
            _eArchivoServicio = eArchivoServicio;
            _logger = logger;
            _uriService = uriService;
            _appSettings = options.Value;
        }

        [HttpGet(Name = nameof(GetArchivosDB))]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<EArchivo>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetArchivosDB([FromQuery] QueryFilters filters)
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
                NextPageUrl = _uriService.GetPostPaginationUri(filters, Url.RouteUrl(nameof(GetArchivosDB))).ToString(),
                PreviousPageUrl = _uriService.GetPostPaginationUri(filters, Url.RouteUrl(nameof(GetArchivosDB))).ToString()
            };


            var response = new ApiResponse<IEnumerable<EArchivo>>(listaMetadata)
            {
                Meta = metadata
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            _logger.LogInformation($"Completado: {JsonConvert.SerializeObject(metadata)}");

            return Ok(response);

        }

        [HttpGet("{id:Guid}")]
        public IActionResult Get(Guid id)
        {
            _logger.LogInformation($"{this.GetType().Name} - {MethodBase.GetCurrentMethod().Name}");
            RespuestaGenerica<EArchivo> car = _eArchivoServicio.BuscarPorId(id);

            ApiResponse<EArchivo> resp = new(car.DataItem);
            return Ok(resp);
        }

        [HttpPost]
        public async Task<IActionResult> Post(EArchivo archivo)
        {
            _logger.LogInformation($"{this.GetType().Name} - {MethodBase.GetCurrentMethod().Name}");

            await _eArchivoServicio.AgregarAsync(archivo, false); //no es un archivo temporal
            return Ok(new ApiResponse<bool>(true));
        }

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            var res = _eArchivoServicio.Quitar(id);
            var response = new ApiResponse<bool>(res);
            return Ok(response);
        }
    }
}
