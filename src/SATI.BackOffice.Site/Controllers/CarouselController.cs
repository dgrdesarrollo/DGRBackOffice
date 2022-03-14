using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using SATI.BackOffice.Core.Servicios.Contrato;
using SATI.BackOffice.Infraestructura.Entidades;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using SATI.BackOffice.Infraestructura.Entidades.Options;
using SATI.BackOffice.Infraestructura.Intefaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace SATI.BackOffice.Site.Controllers
{
    public class CarouselController : ControladorBase
    {
        private readonly ILoggerHelper _logger;
        private readonly AppSettings _appSettings;
        private readonly ICarouselServicio _carouselServicio;
        private readonly IWebHostEnvironment _hosting;

        public CarouselController(ICarouselServicio carouselServicio, ILoggerHelper logger,
            IOptions<AppSettings> options, IWebHostEnvironment hosting) : base(options)
        {
            _appSettings = options.Value;
            _logger = logger;
            _carouselServicio = carouselServicio;
            _hosting = hosting;
        }

        // GET: CarouselController
        public IActionResult Index(string buscar, string sortdir = "ASC", string sort = "Orden", int page = 1)
        {
            try
            {
                //string token = TokenCookie;
                var grilla = ObtenerCarouselsAsync(buscar, sortdir, sort, page);
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

        private GridCore<Carousel> ObtenerCarouselsAsync(string buscar, string sortdir, string sort, int page)
        {
            var carousels = _carouselServicio.Buscar(new QueryFilters { Search = buscar, PageNumber = page, Sort = sort, SortDir = sortdir, PageSize = _appSettings.DefaultPageSize });
            List<Carousel> entidades = carousels.ListaItems;

            var lista = new StaticPagedList<Carousel>(entidades, page, carousels.CantidadReg, carousels.TotalRegs);

            return new GridCore<Carousel>()
            {
                ListaDatos = lista,
                CantidadReg = carousels.TotalRegs,
                PaginaActual = page,
                CantidadPaginas = carousels.TotalRegs < _appSettings.DefaultPageSize ? 1 : carousels.TotalRegs / _appSettings.DefaultPageSize,
                Sort = sort,
                SortDir = sortdir.Equals("ASC") ? "DESC" : "ASC"
            };
        }

        // GET: CarouselController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var carousel = _carouselServicio.BuscarPorId(id);
                if (!carousel.Ok)
                {
                    TempData["warn"] = "El Carousel buscado no se encontró.";
                }
                return View(carousel.DataItem);
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
                    var ruta = Path.Combine(_appSettings.RutaFiles,_appSettings.FolderCarousel);
                    ruta += $"\\{datos.Imagen}";
                    using (Stream fs = new FileStream(ruta, FileMode.Create))
                    {
                        await archivo.CopyToAsync(fs);
                    }
                    datos.Imagen = $"{_appSettings.URLRepositorio}/{_appSettings.FolderCarousel}/{datos.Imagen}";
                    datos.Url = $"{_appSettings.URLRepositorio}//{_appSettings.FolderCarousel}";
                    var res = _carouselServicio.Agregar(datos);
                    if (res > 0)
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
        public ActionResult Edit(int id)
        {
            try
            {
                var carousel = _carouselServicio.BuscarPorId(id);
                if (!carousel.Ok)
                {
                    TempData["warn"] = "El Carousel buscado no se encontró.";
                    return RedirectToAction("Index");
                }
                ImagenEnEdicion = carousel.DataItem.Imagen;
                return View(carousel.DataItem);
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
                        var ruta = Path.Combine(_appSettings.RutaFiles, _appSettings.FolderCarousel);
                        ruta += $"\\{datos.Imagen}";
                        using (Stream fs = new FileStream(ruta, FileMode.Create))
                        {
                            await archivo.CopyToAsync(fs);
                        }
                    }
                    datos.Imagen = $"{_appSettings.URLRepositorio}/{_appSettings.FolderCarousel}/{datos.Imagen}";
                    datos.Url = $"{_appSettings.URLRepositorio}//{_appSettings.FolderCarousel}";
                    var res = _carouselServicio.Actualizar(datos.Id, datos);
                    if (res > 0)
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
        public ActionResult Delete(int id)
        {
            try
            {
                var carousel = _carouselServicio.BuscarPorId(id);
                if (!carousel.Ok)
                {
                    TempData["warn"] = "El Carousel buscado no se encontró.";
                }
                return View(carousel.DataItem);
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
        public IActionResult DeletePpal(int id)
        {
            try
            {
                var res = _carouselServicio.Quitar(id);
                if (res)
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
