using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SATI.BackOffice.Infraestructura.Entidades;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using SATI.BackOffice.Infraestructura.Entidades.Options;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace SATI.BackOffice.Site.Controllers
{
    public class ControladorBase : Controller
    {
        private readonly IOptions<AppSettings> _options;

        public ControladorBase(IOptions<AppSettings> options)
        {
            _options = options;
        }

        public List<dl_tipos> Tipos
        {
            get
            {
                string lista = HttpContext.Session.GetString("Tipos");
                return JsonConvert.DeserializeObject<List<dl_tipos>>(lista);
            }
            set
            {
                string lista = JsonConvert.SerializeObject(value);
                HttpContext.Session.SetString("Tipos", lista);
            }
        }
    }
}
