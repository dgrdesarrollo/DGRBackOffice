using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using SATI.BackOffice.Infraestructura.Entidades;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using SATI.BackOffice.Infraestructura.Entidades.Options;
using SATI.BackOffice.Infraestructura.Intefaces;
using SATI.BackOffice.Site.Core.Servicios.Contratos;
using System;
using System.IO;
using System.Threading.Tasks;
using X.PagedList;

namespace SATI.BackOffice.Site.Controllers
{
    public class CarouselController : ControladorBase
    {
        private readonly ILoggerHelper _logger;
        private readonly AppSettings _appSettings;
        private readonly ICarruselServicio _carruselServicio;



        public CarouselController(ILoggerHelper logger, ICarruselServicio carruselServicio,
            IOptions<AppSettings> options, IWebHostEnvironment hosting) : base(options)
        {
            _appSettings = options.Value;
            _logger = logger;
            _carruselServicio = carruselServicio;
        }

        // GET: CarouselController
        public IActionResult Index(string buscar, string sortdir = "ASC", string sort = "Orden", int page = 1)
        {
            try
            {
                //string token = TokenCookie;
                var grilla = ObtenerCarouselsAsync(buscar, sortdir, sort, page).Result;
                ViewData["Title"] = "Administración de Carousel";
                return View(grilla);
            }
            catch (Exception ex)
            {
                ex = _logger.Log(ex);
                TempData["error"] = ex.Message;
            }
            return RedirectToAction("Index", new RouteValueDictionary(new { area = "", controller = "Home", action = "Index" }));

        }

        private async Task<GridCore<Carousel>> ObtenerCarouselsAsync(string buscar, string sortdir, string sort, int page)
        {


            var respuesta = await _carruselServicio.BuscarAsync(new QueryFilters { Search = buscar, PageNumber = page, Sort = sort, SortDir = sortdir, PageSize = _appSettings.DefaultPageSize });

            //var carousels = _carouselServicio.Buscar(new QueryFilters { Search = buscar, PageNumber = page, Sort = sort, SortDir = sortdir, PageSize = _appSettings.DefaultPageSize });
            //List<Carousel> entidades = carousels.ListaItems;
            var meta = respuesta.Item2;
            var lista = new StaticPagedList<Carousel>(respuesta.Item1, page, meta.PageSize, meta.TotalCount);

            return new GridCore<Carousel>()
            {
                ListaDatos = lista,
                CantidadReg = meta.TotalCount,
                PaginaActual = page,
                CantidadPaginas = meta.TatalPages,
                Sort = sort,
                SortDir = sortdir.Equals("ASC") ? "DESC" : "ASC"
            };
        }

        // GET: CarouselController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var carousel = await _carruselServicio.BuscarAsync(id);
                if (carousel == null)
                {
                    TempData["warn"] = "La Imagen buscada no se encontró.";
                }
                return View(carousel);
            }
            catch (Exception ex)
            {
                ex = _logger.Log(ex);
                TempData["error"] = ex.Message;
            }
            return RedirectToAction("Index", new RouteValueDictionary(new { area = "", controller = "Carousel", action = "Index" }));
        }


        // GET: CarouselController/Create
        public ActionResult Create()
        {
            return View(new Carousel() { VigenciaDesde = DateTime.Today, VigenciaHasta = DateTime.Today.AddDays(14) });
        }

        // POST: CarouselController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile archivo, Carousel datos)
        {
            try
            {
                if (archivo == null)
                {
                    ModelState.AddModelError("Archivo", "No ha ingresado un archivo en el formato admitido.");
                }
                else
                {
                    if (archivo.Length > _appSettings.FileSize)
                    {
                    ModelState.AddModelError("Archivo", "El archivo es demasiado grande. Tamaño máximo 5Mb.");
                    }
                    datos.Imagen = archivo.FileName;
                }

                if (datos is null)
                {
                    throw new ArgumentNullException(nameof(datos));
                }
                if (datos.VigenciaDesde >= datos.VigenciaHasta)
                {
                    ModelState.AddModelError("VigenciaDesde", "Se definieron fechas erroneas. Verifique.");
                }
                if (!ModelState.IsValid)
                {
                    return View(datos);
                }
                else
                {
                    #region Invoco API y calculo ruta

                    string ruta = await _carruselServicio.CalcularRuta("XX");

                    #endregion

                    #region se convierte la imagene en Base64                    
                    MemoryStream ms = new();
                    archivo.CopyTo(ms);
                    var archB64 = Convert.ToBase64String(ms.ToArray());
                    #endregion

                    //cargamos el archivo en B64
                    datos.Archivo = archB64;
                    datos.UbicacionFisica = ruta;


                    #region Subiendo archivo al repositorio a traves de la api
                    var respuesta = await _carruselServicio.AgregarAsync(datos);
                    #endregion

                    if (respuesta)
                    {
                        TempData["info"] = "Se agrego satisfactoriamente el Carousel";
                        return RedirectToAction("Index", new RouteValueDictionary(new { area = "", controller = "Carousel", action = "Index" }));
                    }
                    else
                    {
                        TempData["warn"] = $"No se pudo agregar el Carousel. Si el problema persiste avise al administrador del sistema.";
                    }
                }
            }

            catch (Exception ex)
            {
                ex = _logger.Log(ex);
                TempData["error"] = ex.Message;
            }
            return View(datos);
        }

        // GET: CarouselController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var carousel = await _carruselServicio.BuscarAsync(id);
                if (carousel == null)
                {
                    TempData["warn"] = "La Imagen buscada no se encontró.";
                }
                return View(carousel);
            }
            catch (Exception ex)
            {
                ex = _logger.Log(ex);
                TempData["error"] = ex.Message;
            }
            return RedirectToAction("Index", new RouteValueDictionary(new { area = "", controller = "Carousel", action = "Index" }));
        }

        // POST: CarouselController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormFile archivo, Carousel datos)
        {
            try
            {
                if (archivo != null)
                {
                    datos.Imagen = archivo.FileName;
                }
                if (datos is null)
                {
                    throw new ArgumentNullException(nameof(datos));
                }

                if (!ModelState.IsValid)
                {
                    return View(datos);
                }
                else
                {
                    if (archivo != null)
                    {
                        #region se convierte la imagene en Base64                    
                        MemoryStream ms = new();
                        archivo.CopyTo(ms);
                        var archB64 = Convert.ToBase64String(ms.ToArray());
                        datos.Archivo = archB64;
                        #endregion
                    }
                    #region Subiendo archivo al repositorio a traves de la api
                    var respuesta = await _carruselServicio.ActualizarAsync(datos.Id, datos);
                    #endregion
                    if (respuesta)
                    {
                        TempData["info"] = "Se Actualizo satisfactoriamente el Carousel";
                        return RedirectToAction("Index", new RouteValueDictionary(new { area = "", controller = "Carousel", action = "Index" }));
                    }
                    else
                    {
                        TempData["warn"] = $"No se pudo actualizar el Carousel. Si el problema persiste avise al administrador del sistema.";
                    }
                }
            }
            catch (Exception ex)
            {
                ex = _logger.Log(ex);
                TempData["error"] = ex.Message;
            }
            return View(datos);
        }

        // GET: CarouselController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var carousel = await _carruselServicio.BuscarAsync(id);
                if (carousel == null)
                {
                    TempData["warn"] = "La Imagen buscada no se encontró.";
                }
                return View(carousel);
            }
            catch (Exception ex)
            {
                ex = _logger.Log(ex);
                TempData["error"] = ex.Message;
            }
            return RedirectToAction("Index", new RouteValueDictionary(new { area = "", controller = "Carousel", action = "Index" }));
        }

        // POST: CarouselController/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePpal(int id)
        {
            try
            {
                var respuesta = await _carruselServicio.EliminarAsync(id);

                if (respuesta)
                {
                    TempData["info"] = "Se Eliminó satisfactoriamente el Carousel";
                }
                else
                {
                    TempData["warn"] = $"No se pudo eliminar el Carousel. Si el problema persiste avise al administrador del sistema.";
                }
            }
            catch (Exception ex)
            {
                ex = _logger.Log(ex);
                TempData["error"] = ex.Message;
            }
            return RedirectToAction("Index", new RouteValueDictionary(new { area = "", controller = "Carousel", action = "Index" }));
        }
    }

}
