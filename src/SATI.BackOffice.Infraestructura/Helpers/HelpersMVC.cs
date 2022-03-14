using Microsoft.AspNetCore.Mvc.Rendering;
using SATI.BackOffice.Infraestructura.Intefaces;
using System.Collections.Generic;

namespace SATI.BackOffice.Infraestructura.Helpers
{
    public class HelpersMVC<T> where T: IEntidades
    {
        public static SelectList ListaGenerica(IEnumerable<T> listado, string textValor, string textDato, object valorSeleccionado = null)
        {
            if (valorSeleccionado != null)
                return new SelectList(listado, textValor, textDato, valorSeleccionado);
            else
                return new SelectList(listado, textValor, textDato);
        }

        public static SelectList ListaGenerica(IEnumerable<T> listado, object valorSeleccionado = null)
        {
            return ListaGenerica(listado, "Id", "Descripcion", valorSeleccionado);
        }

        public static SelectList ListaGenerica(IEnumerable<T> listado)
        {
            return ListaGenerica(listado, "Id", "Descripcion");
        }
    }
}
