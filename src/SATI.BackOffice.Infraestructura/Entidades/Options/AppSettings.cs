namespace SATI.BackOffice.Infraestructura.Entidades.Options
{
    public class AppSettings
    {
        public int LimiteAvalancha { get; set; }
        public int DefaultPageSize { get; set; }
        public int DefaultPageNumber { get; set; }
        public bool ExceptionManagerEnabled { get; set; }
        public string URLRepositorio { get; set; }
        public string URLBase { get; set; }
        public string RutaFisica { get; set; }
        public int FileSize { get; set; }
        public string Extensiones { get; set; }
    }
}
