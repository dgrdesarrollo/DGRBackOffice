using SATI.BackOffice.Infraestructura.Intefaces;

namespace SATI.BackOffice.Infraestructura.Entidades
{
    public partial class dl_tipos : IEntidades
    {     
        public short dlt_id { get; set; }
        public string dlt_descripcion { get; set; }

    }
}
