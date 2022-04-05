using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SATI.BackOffice.Core.Servicios.Contrato;
using SATI.BackOffice.Infraestructura.Entidades;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using SATI.BackOffice.Infraestructura.Entidades.Options;
using SATI.BackOffice.Infraestructura.Intefaces;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace SATI.BackOffice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BONormaController : ControllerBase
    {
        private readonly INormativaLegalServicio _normativaLegalServicio;
        private readonly ILogger<BONormaController> _logger;
        private readonly IUriService _uriService;
        private readonly AppSettings _appSettings;

        public BONormaController(INormativaLegalServicio normativaLegalServicio, ILogger<BONormaController> logger,
            IOptions<AppSettings> options, IUriService uriService)
        {
            _normativaLegalServicio = normativaLegalServicio;
            _logger = logger;
            _appSettings = options.Value;
            _uriService = uriService;
        }

        [HttpGet(Name = nameof(GetNormas))]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<dl_documentos>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetNormas([FromQuery] QueryFilters filters)
        {

            _logger.LogInformation($"{this.GetType().Name} - {MethodBase.GetCurrentMethod().Name}");
            filters.PageNumber = filters.PageNumber == 0 ? _appSettings.DefaultPageNumber : filters.PageNumber;
            filters.PageSize = filters.PageSize == 0 ? _appSettings.DefaultPageSize : filters.PageSize;

            var resp = _normativaLegalServicio.Buscar(filters);

            var listado = resp.ListaItems;

            //generando metadata
            var listaMetadata = PagedList<dl_documentos>.Create(listado, filters.PageNumber, filters.PageSize);

            var metadata = new Metadata
            {
                TotalCount = listaMetadata.TotalCount,
                PageSize = listaMetadata.PageSize,
                CurrentPage = listaMetadata.CurrentPage,
                TotalPages = listaMetadata.TotalPages,
                HasNextPage = listaMetadata.HasNextPage,
                HasPreviousPage = listaMetadata.HasPreviousPage,
                NextPageUrl = _uriService.GetPostPaginationUri(filters, Url.RouteUrl(nameof(GetNormas))).ToString(),
                PreviousPageUrl = _uriService.GetPostPaginationUri(filters, Url.RouteUrl(nameof(GetNormas))).ToString()
            };

            var response = new ApiResponse<IEnumerable<dl_documentos>>(listaMetadata);
            response.Meta = metadata;

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            _logger.LogInformation($"Completado: {JsonConvert.SerializeObject(metadata)}");

            return Ok(response);

        }

        [HttpGet]
        [Route("[action]/{codigoSistema}")]
        public IActionResult CalcularRuta(string codigoSistema)
        {
            _logger.LogInformation($"{this.GetType().Name} - {MethodBase.GetCurrentMethod().Name}");

            string ruta = _normativaLegalServicio.CalcularRuta(codigoSistema);

            var resp = new ApiResponse<string>(ruta);

            return Ok(resp);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            _logger.LogInformation($"{this.GetType().Name} - {MethodBase.GetCurrentMethod().Name}");
            var car = _normativaLegalServicio.BuscarPorId(id);

            var resp = new ApiResponse<dl_documentos>(car.DataItem);
            return Ok(resp);
        }

        [HttpPost]
        public async Task<IActionResult> Post(dl_documentos doc)
        {
            _logger.LogInformation($"{this.GetType().Name} - {MethodBase.GetCurrentMethod().Name}");

            await _normativaLegalServicio.AgregarAsync(doc);
            return Ok(new ApiResponse<bool>(true));
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, dl_documentos doc)
        {
            _logger.LogInformation($"{this.GetType().Name} - {MethodBase.GetCurrentMethod().Name}");
            _normativaLegalServicio.Actualizar(id, doc);
            return Ok(new ApiResponse<bool>(true));

        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var res = _normativaLegalServicio.Quitar(id);
            var response = new ApiResponse<bool>(res);
            return Ok(response);
        }
    }
}
