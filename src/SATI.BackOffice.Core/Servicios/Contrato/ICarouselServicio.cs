using SATI.BackOffice.Infraestructura.Entidades;
using System.Threading.Tasks;

namespace SATI.BackOffice.Core.Servicios.Contrato
{
    public interface ICarouselServicio : IServicio<Carousel>, IReadServicio<Carousel>,IWriteServicio<Carousel>
    {
        string CalcularRuta(string codigoSistema);
        Task<int> AgregarAsync(Carousel entidad);
    }
}
