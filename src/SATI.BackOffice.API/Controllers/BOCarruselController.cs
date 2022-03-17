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
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class BOCarruselController : ControllerBase
    {
        private readonly ICarouselServicio _carouselServicio;
        private readonly ILogger<BOCarruselController> _logger;
        private readonly IUriService _uriService;
        private readonly AppSettings _appSettings;

        public BOCarruselController(ICarouselServicio carouselServicio, ILogger<BOCarruselController> logger,
            IOptions<AppSettings> options, IUriService uriService)
        {
            _carouselServicio = carouselServicio;
            _logger = logger;
            _appSettings = options.Value;
            _uriService = uriService;
        }

        [HttpGet(Name = nameof(GetCarousel))]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<Carousel>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetCarousel([FromForm] QueryFilters filters)
        {

            _logger.LogInformation($"{this.GetType().Name} - {MethodBase.GetCurrentMethod().Name}");
            filters.PageNumber = filters.PageNumber == 0 ? _appSettings.DefaultPageNumber : filters.PageNumber;
            filters.PageSize = filters.PageSize == 0 ? _appSettings.DefaultPageSize : filters.PageSize;

            var resp = _carouselServicio.Buscar(filters);

            var listado = resp.ListaItems;

            //generando metadata
            var listaMetadata = PagedList<Carousel>.Create(listado, filters.PageNumber, filters.PageSize);

            var metadata = new Metadata
            {
                TotalCount = listaMetadata.TotalCount,
                PageSize = listaMetadata.PageSize,
                CurrentPage = listaMetadata.CurrentPage,
                TatalPages = listaMetadata.TotalPages,
                HasNextPage = listaMetadata.HasNextPage,
                HasPreviousPage = listaMetadata.HasPreviousPage,
                NextPageUrl = _uriService.GetPostPaginationUri(filters, Url.RouteUrl(nameof(GetCarousel))).ToString(),
                PreviousPageUrl = _uriService.GetPostPaginationUri(filters, Url.RouteUrl(nameof(GetCarousel))).ToString()
            };

            var response = new ApiResponse<IEnumerable<Carousel>>(listaMetadata);
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

            var ruta = _carouselServicio.CalcularRuta(codigoSistema);

            var resp = new ApiResponse<string>(ruta);

            return Ok(resp);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            _logger.LogInformation($"{this.GetType().Name} - {MethodBase.GetCurrentMethod().Name}");
            var car = _carouselServicio.BuscarPorId(id);

            var resp = new ApiResponse<Carousel>(car.DataItem);
            return Ok(resp);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Carousel carusel)
        {
            _logger.LogInformation($"{this.GetType().Name} - {MethodBase.GetCurrentMethod().Name}");

            await _carouselServicio.AgregarAsync(carusel);
            return Ok(new ApiResponse<bool>(true));
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id,Carousel carousel)
        {
            _logger.LogInformation($"{this.GetType().Name} - {MethodBase.GetCurrentMethod().Name}");
            _carouselServicio.Actualizar(id, carousel);
            return Ok(new ApiResponse<bool>(true));

        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var res = _carouselServicio.Quitar(id);
            var response = new ApiResponse<bool>(res);
            return Ok(response);
        }
    }
}
