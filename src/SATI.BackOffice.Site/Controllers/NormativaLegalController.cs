using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using SATI.BackOffice.Infraestructura.Entidades;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using SATI.BackOffice.Infraestructura.Entidades.Options;
using SATI.BackOffice.Infraestructura.Helpers;
using SATI.BackOffice.Infraestructura.Intefaces;
using SATI.BackOffice.Site.Core.Servicios.Contratos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace SATI.BackOffice.Site.Controllers
{
    public class NormativaLegalController : ControladorBase
    {
        private readonly ILoggerHelper _logger;
        private readonly AppSettings _appSettings;
        private readonly INormaLegalServicio _normaLegalServicio;
        private readonly ITipoNormaServicio _tipoNormaServicio;

        public NormativaLegalController(ILoggerHelper logger, IOptions<AppSettings> options,
            INormaLegalServicio normaLegalServicio, ITipoNormaServicio tipoNormaServicio) : base(options)
        {
            _appSettings = options.Value;
            _logger = logger;
            _normaLegalServicio = normaLegalServicio;
            _tipoNormaServicio = tipoNormaServicio;
        }

        // GET: dl_documentosController
        public async Task<IActionResult> Index(string buscar, string sortdir = "ASC", string sort = "Orden", int page = 1, int dlt_id = 0)
        {
            try
            {
                //string token = TokenCookie;
                var grilla = await Obtenerdl_documentossAsync(buscar, sortdir, sort, page, dlt_id);
                ViewData["Title"] = "Administración de Normas Legales";
                await CargarCombos(null);
                return View(grilla);
            }
            catch (Exception ex)
            {
                ex = _logger.Log(ex);
                TempData["error"] = ex.Message;
            }
            return RedirectToAction("Index", new RouteValueDictionary(new { area = "", controller = "Home", action = "Index" }));

        }

        private async Task<GridCore<dl_documentos>> Obtenerdl_documentossAsync(string buscar, string sortdir, string sort, int page, int dlt_id)
        {
            #region Invocación de la api


            var respuesta = await _normaLegalServicio.BuscarAsync(new QueryFilters { Search = buscar, PageNumber = page, Sort = sort, SortDir = sortdir, PageSize = _appSettings.DefaultPageSize, IdRef = dlt_id });
            #endregion

            var meta = respuesta.Item2;
            var lista = new StaticPagedList<dl_documentos>(respuesta.Item1, page, meta.PageSize, meta.TotalCount);

            return new GridCore<dl_documentos>()
            {
                ListaDatos = lista,
                CantidadReg = meta.TotalCount,
                PaginaActual = page,
                CantidadPaginas = meta.TotalPages,
                Sort = sort,
                SortDir = sortdir.Equals("ASC") ? "DESC" : "ASC"
            };
        }

        // GET: dl_documentosController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var norma = await _normaLegalServicio.BuscarAsync(id);
                if (norma == null)
                {
                    TempData["warn"] = "La Normativa buscada no se encontró.";
                }
                return View(norma);
            }
            catch (Exception ex)
            {
                ex = _logger.Log(ex);
                TempData["error"] = ex.Message;
            }
            return RedirectToAction("Index", new RouteValueDictionary(new { area = "", controller = "NormativaLegal", action = "Index" }));
        }

        // GET: dl_documentosController/Create
        public async Task<ActionResult> Create()
        {
            await CargarCombos(null);
            ViewBag.Modulo = "C";
            return View(new dl_documentos() { dl_fecha = DateTime.Today, dl_activo = 'S' });
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
                    #region Invoco API y calculo ruta
                    var tipo = Tipos.Where(x => x.dlt_id == datos.dlt_id).Distinct().SingleOrDefault();
                    string ruta;
                    if (tipo == null)
                    {
                        ruta = await _normaLegalServicio.CalcularRuta("XX");
                    }
                    else
                    {
                        ruta = await _normaLegalServicio.CalcularRuta(tipo.dlt_descripcion);
                    }
                    #endregion

                    #region se convierte la imagene en Base64                    
                    MemoryStream ms = new();
                    pdf.CopyTo(ms);
                    var archB64 = Convert.ToBase64String(ms.ToArray());
                    #endregion

                    //cargamos el archivo en B64
                    datos.Pdf = archB64;
                    datos.UbicacionFisica = ruta;


                    #region Subiendo archivo al repositorio a traves de la api
                    var respuesta = await _normaLegalServicio.AgregarAsync(datos);
                    #endregion

                    if (respuesta)
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
            ViewBag.Modulo = "C";
            await CargarCombos(datos);
            return View(datos);
        }

        // GET: dl_documentosController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var norma = await _normaLegalServicio.BuscarAsync(id);
                if (norma == null)
                {
                    TempData["warn"] = "La Normativa buscada no se encontró.";
                }
                await CargarCombos(norma);
                return View(norma);
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
                        #region se convierte la imagene en Base64                    
                        MemoryStream ms = new();
                        pdf.CopyTo(ms);
                        var archB64 = Convert.ToBase64String(ms.ToArray());
                        datos.Pdf = archB64;
                        #endregion
                    }
                    var respuesta = await _normaLegalServicio.ActualizarAsync(datos.dl_id, datos);

                    if (respuesta)
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
            await CargarCombos(datos);
            return View(datos);
        }

        // GET: dl_documentosController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var norma = await _normaLegalServicio.BuscarAsync(id);
                if (norma == null)
                {
                    TempData["warn"] = "La Normativa buscada no se encontró.";
                }
                return View(norma);
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
        public async Task<IActionResult> DeletePpal(int id)
        {
            try
            {
                var respuesta = await _normaLegalServicio.EliminarAsync(id);
                if (respuesta)
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

        private async Task CargarCombos(dl_documentos dl_documentos)
        {
            #region Invocación api tipo norma              
            var respuesta = await _tipoNormaServicio.BuscarAsync();
            #endregion

            if (respuesta.Item1.Count > 0)
            {
                Tipos = respuesta.Item1;
                if (dl_documentos == null || (dl_documentos != null && dl_documentos.dlt_id == default))
                {
                    ViewBag.dlt_id = HelpersMVC<dl_tipos>.ListaGenerica(respuesta.Item1, "dlt_id", "dlt_descripcion");
                }
                else
                {
                    ViewBag.dlt_id = HelpersMVC<dl_tipos>.ListaGenerica(respuesta.Item1, "dlt_id", "dlt_descripcion", dl_documentos.dlt_id);
                }
            }
            else
            {
                ViewBag.dlt_id = HelpersMVC<dl_tipos>.ListaGenerica(new List<dl_tipos>(), "dlt_id", "dlt_descripcion");
            }
        }
    }
}
