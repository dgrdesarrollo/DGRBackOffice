using SATI.BackOffice.Infraestructura.Entidades;
using System.Threading.Tasks;

namespace SATI.BackOffice.Site.Core.Servicios.Contratos
{
    public interface ICarruselServicio:IServicio<Carousel>
    {
        Task<string> CalcularRuta(string codigo);
    }
}
