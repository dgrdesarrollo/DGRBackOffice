using SATI.BackOffice.Infraestructura.Intefaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace SATI.BackOffice.Infraestructura.Entidades
{
    public partial class dl_documentos : IEntidades
    {
       
        [ScaffoldColumn(false)]
        public int dl_id { get; set; }
        [Display(Name ="Tipo Norma")]
        public short? dlt_id { get; set; }
        [Display(Name ="Fecha")]
        public DateTime dl_fecha { get; set; }
        [Display(Name ="Título")]
        public string dl_titulo { get; set; }
        [Display(Name ="Descripción")]
        public string dl_nota { get; set; }
        [Display(Name ="Archivo")]
        public string dl_file { get; set; }
        [Display(Name ="Activo")]
        public char dl_activo { get; set; }     
        public string Tipo { get; set; }
        public object Pdf { get; set; }
        public string UbicacionFisica { get; set; }

    }
}
