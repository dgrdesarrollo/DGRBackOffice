using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SATI.BackOffice.Infraestructura.Entidades.Options;
using System;

namespace SATI.BackOffice.Site.Controllers
{
    public class ControladorBase : Controller
    {
        private readonly IOptions<AppSettings> _options;

        public ControladorBase(IOptions<AppSettings> options)
        {
            _options = options;
        }

        //public int UltimoRegistro
        //{
        //    get { return HttpContext.Session.GetString("UltimoRegistro").ToInt(); }
        //    set { HttpContext.Session.SetString("UltimoRegistro", value.ToString()); }
        //}

        public string ImagenEnEdicion
        {
            get { return HttpContext.Session.GetString("ImagenEnEdicion"); }
            set { HttpContext.Session.SetString("ImagenEnEdicion", value); }
        }
    }
}
