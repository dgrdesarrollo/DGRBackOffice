using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SATI.BackOffice.Site.Controllers
{
    public class IndisponibilidadSitioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
