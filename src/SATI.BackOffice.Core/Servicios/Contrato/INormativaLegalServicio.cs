using SATI.BackOffice.Infraestructura.Entidades;
using System.Threading.Tasks;

namespace SATI.BackOffice.Core.Servicios.Contrato
{
    public interface INormativaLegalServicio : IServicio<dl_documentos>, IReadServicio<dl_documentos>, IWriteServicio<dl_documentos>
    {
        string CalcularRuta(string codigoSistema);
        Task<int> AgregarAsync(dl_documentos doc);
    }
}
