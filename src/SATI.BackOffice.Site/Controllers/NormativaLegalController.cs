using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using SATI.BackOffice.Core.Servicios.Contrato;
using SATI.BackOffice.Infraestructura.Entidades;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using SATI.BackOffice.Infraestructura.Entidades.Options;
using SATI.BackOffice.Infraestructura.Helpers;
using SATI.BackOffice.Infraestructura.Intefaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using X.PagedList;

namespace SATI.BackOffice.Site.Controllers
{
    public class NormativaLegalController : ControladorBase
    {
        private readonly ILoggerHelper _logger;
        private readonly AppSettings _appSettings;
        private readonly INormativaLegalServicio _normativaLegalServicio;
        private readonly ITipoNormativaServicio _tipoNormativaServicio;
        private readonly IWebHostEnvironment _hosting;

        public NormativaLegalController(INormativaLegalServicio normativaLegalServicio, ILoggerHelper logger,
            IOptions<AppSettings> options, IWebHostEnvironment hosting, ITipoNormativaServicio tipoNormativaServicio) : base(options)
        {
            _appSettings = options.Value;
            _logger = logger;
            _normativaLegalServicio = normativaLegalServicio;
            _hosting = hosting;
            _tipoNormativaServicio = tipoNormativaServicio;
        }

        // GET: dl_documentosController
        public IActionResult Index(string buscar, string sortdir = "ASC", string sort = "Orden", int page = 1)
        {
            try
            {
                //string token = TokenCookie;
                var grilla = Obtenerdl_documentossAsync(buscar, sortdir, sort, page);
                ViewData["Title"] = "Administración de Normas Legales";
                return View(grilla);
            }
            catch (Exception ex)
            {
                ex = _logger.Log(ex);
                TempData["error"] = ex.Message;
            }
            return RedirectToAction("Index", new RouteValueDictionary(new { area = "", controller = "Home", action = "Index" }));

        }

        private GridCore<dl_documentos> Obtenerdl_documentossAsync(string buscar, string sortdir, string sort, int page)
        {
            var dl_documentoss = _normativaLegalServicio.Buscar(new QueryFilters { Search = buscar, PageNumber = page, Sort = sort, SortDir = sortdir, PageSize = _appSettings.DefaultPageSize });
            List<dl_documentos> entidades = dl_documentoss.ListaItems;

            var lista = new StaticPagedList<dl_documentos>(entidades, page, dl_documentoss.CantidadReg, dl_documentoss.TotalRegs);

            return new GridCore<dl_documentos>()
            {
                ListaDatos = lista,
                CantidadReg = dl_documentoss.TotalRegs,
                PaginaActual = page,
                CantidadPaginas = dl_documentoss.TotalRegs < _appSettings.DefaultPageSize ? 1 : dl_documentoss.TotalRegs / _appSettings.DefaultPageSize,
                Sort = sort,
                SortDir = sortdir.Equals("ASC") ? "DESC" : "ASC"
            };
        }

        // GET: dl_documentosController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var dl_documentos = _normativaLegalServicio.BuscarPorId(id);
                if (!dl_documentos.Ok)
                {
                    TempData["warn"] = "El dl_documentos buscado no se encontró.";
                }
                return View(dl_documentos.DataItem);
            }
            catch (Exception ex)
            {
                ex = _logger.Log(ex);
                TempData["error"] = ex.Message;
            }
            return RedirectToAction("Index", new RouteValueDictionary(new { area = "", controller = "NormativaLegal", action = "Index" }));
        }

        // GET: dl_documentosController/Create
        public ActionResult Create()
        {
            CargarCombos(null);
            ViewBag.Modulo = "C";
            return View(new dl_documentos() { dl_fecha=DateTime.Today,dl_activo='S' });
        }

        // POST: dl_documentosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile pdf, dl_documentos datos)
        {
            try
            {
                if (pdf == null)
                {
                    ModelState.AddModelError("Archivo", "No ha ingresado un archivo en el formato admitido.");
                }
                else
                {
                    datos.dl_file = pdf.FileName;
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
                    var ruta = Path.Combine(_appSettings.RutaFiles, _appSettings.FolderNormaLeg);
                    ruta += $"\\{datos.dl_file}";
                    using (Stream fs = new FileStream(ruta, FileMode.Create))
                    {
                        await pdf.CopyToAsync(fs);
                    }
                    var res = _normativaLegalServicio.Agregar(datos);
                    if (res > 0)
                    {
                        TempData["info"] = "Se agrego satisfactoriamente la Normalegal";
                        return RedirectToAction("Index", new RouteValueDictionary(new { area = "", controller = "NormativaLegal", action = "Index" }));
                    }
                    else
                    {
                        TempData["warn"] = $"No se pudo agregar la Norma Legal. Si el problema persiste avise al administrador del sistema.";
                    }
                }
            }

            catch (Exception ex)
            {
                ex = _logger.Log(ex);
                TempData["error"] = ex.Message;
            }
            CargarCombos(datos);
            return View(datos);
        }

        // GET: dl_documentosController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var dl_documentos = _normativaLegalServicio.BuscarPorId(id);
                if (!dl_documentos.Ok)
                {
                    TempData["warn"] = "El dl_documentos buscado no se encontró.";
                    return RedirectToAction("Index");
                }
                CargarCombos(dl_documentos.DataItem);
                return View(dl_documentos.DataItem);
            }
            catch (Exception ex)
            {
                ex = _logger.Log(ex);
                TempData["error"] = ex.Message;
            }
            return RedirectToAction("Index", new RouteValueDictionary(new { area = "", controller = "NormativaLegal", action = "Index" }));
        }

        // POST: dl_documentosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormFile pdf, dl_documentos datos)
        {
            try
            {
                if (pdf != null)
                {
                    datos.dl_file = pdf.FileName;
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
                    if (pdf != null)
                    {
                        var ruta = Path.Combine(_appSettings.RutaFiles, _appSettings.FolderCarousel);
                        ruta += $"\\{datos.dl_file}";
                        using (Stream fs = new FileStream(ruta, FileMode.Create))
                        {
                            await pdf.CopyToAsync(fs);
                        }
                    }
                    var res = _normativaLegalServicio.Actualizar(datos.dlt_id, datos);
                    if (res > 0)
                    {
                        TempData["info"] = "Se Actualizo satisfactoriamente el dl_documentos";
                        return RedirectToAction("Index", new RouteValueDictionary(new { area = "", controller = "NormativaLegal", action = "Index" }));
                    }
                    else
                    {
                        TempData["warn"] = $"No se pudo actualizar el dl_documentos. Si el problema persiste avise al administrador del sistema.";
                    }
                }
            }
            catch (Exception ex)
            {
                ex = _logger.Log(ex);
                TempData["error"] = ex.Message;
            }
            CargarCombos(datos);
            return View(datos);
        }

        // GET: dl_documentosController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var dl_documentos = _normativaLegalServicio.BuscarPorId(id);
                if (!dl_documentos.Ok)
                {
                    TempData["warn"] = "La Norma Legal buscada no se encontró.";
                }
                return View(dl_documentos.DataItem);
            }

            catch (Exception ex)
            {
                ex = _logger.Log(ex);
                TempData["error"] = ex.Message;
            }
            return RedirectToAction("Index", new RouteValueDictionary(new { area = "", controller = "NormativaLegal", action = "Index" }));
        }

        // POST: dl_documentosController/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePpal(int id)
        {
            try
            {
                var res = _normativaLegalServicio.Quitar(id);
                if (res)
                {
                    TempData["info"] = "Se Eliminó satisfactoriamente la Norma Legal";
                }
                else
                {
                    TempData["warn"] = $"No se pudo eliminar la Norma Legal. Si el problema persiste avise al administrador del sistema.";
                }
            }
            catch (Exception ex)
            {
                ex = _logger.Log(ex);
                TempData["error"] = ex.Message;
            }
            return RedirectToAction("Index", new RouteValueDictionary(new { area = "", controller = "NormativaLegal", action = "Index" }));
        }

        private void CargarCombos(dl_documentos dl_documentos)
        {
            var dl_tipos = _tipoNormativaServicio.TraerTodo();
            if (dl_tipos.Ok)
            {
                if (dl_documentos == null || (dl_documentos != null && dl_documentos.dlt_id == default))
                {
                    ViewBag.dlt_id = HelpersMVC<dl_tipos>.ListaGenerica(dl_tipos.ListaItems, "dlt_id", "dlt_descripcion");
                }
                else
                {
                    ViewBag.dlt_id = HelpersMVC<dl_tipos>.ListaGenerica(dl_tipos.ListaItems, "dlt_id", "dlt_descripcion", dl_documentos.dlt_id);
                }
            }
            else
            {
                ViewBag.dlt_id = HelpersMVC<dl_tipos>.ListaGenerica(new List<dl_tipos>(), "dlt_id", "dlt_descripcion");
            }
        }
    }
}
