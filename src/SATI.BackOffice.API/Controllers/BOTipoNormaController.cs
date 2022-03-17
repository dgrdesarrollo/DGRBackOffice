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
    public class BOTipoNormaController : ControllerBase
    {
        private readonly ITipoNormativaServicio _tipoNormativaServicio;
        private readonly ILogger<BOTipoNormaController> _logger;
        private readonly IUriService _uriService;
        private readonly AppSettings _appSettings;

        public BOTipoNormaController(ITipoNormativaServicio tipoNormativaServicio, ILogger<BOTipoNormaController> logger,
            IOptions<AppSettings> options, IUriService uriService)
        {
            _tipoNormativaServicio = tipoNormativaServicio;
            _logger = logger;
            _appSettings = options.Value;
            _uriService = uriService;
        }

        [HttpGet(Name = nameof(GetTiposNorma))]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<dl_tipos>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetTiposNorma()
        {

            _logger.LogInformation($"{this.GetType().Name} - {MethodBase.GetCurrentMethod().Name}");
           
            var resp = _tipoNormativaServicio.TraerTodo();

            var listado = resp.ListaItems;

            //generando metadata
            var listaMetadata = PagedList<dl_tipos>.Create(listado, 1, 99);
            
            var response = new ApiResponse<IEnumerable<dl_tipos>>(listaMetadata);

            return Ok(response);

        }       

    }
}
