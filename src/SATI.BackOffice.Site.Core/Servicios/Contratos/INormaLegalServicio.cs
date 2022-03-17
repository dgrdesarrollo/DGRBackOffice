using SATI.BackOffice.Infraestructura.Entidades;
using System.Threading.Tasks;

namespace SATI.BackOffice.Site.Core.Servicios.Contratos
{
    public interface INormaLegalServicio : IServicio<dl_documentos>
    {
        Task<string> CalcularRuta(string codigo);

    }
}
