namespace SATI.BackOffice.Infraestructura.Entidades.Options
{
    public class AppSettings
    {
        public int LimiteAvalancha { get; set; }
        public int DefaultPageSize { get; set; }
        public int DefaultPageNumber { get; set; }
        public bool ExceptionManagerEnabled { get; set; }
        public string RutaFiles { get; set; }
        public string FolderCarousel { get; set; }
        public string FolderNormaLeg { get; set; }
        public string URLRepositorio { get; set; }

    }
}
