using SATI.BackOffice.Infraestructura.Intefaces;
using SATI.Entidades;

namespace SATI.BackOffice.Infraestructura.Entidades
{
    public class Carousel : CarouselWS, IEntidades
    {
        public object Archivo { get; set; }
    }
}
