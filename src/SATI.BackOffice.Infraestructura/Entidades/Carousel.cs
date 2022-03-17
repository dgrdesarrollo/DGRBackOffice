using SATI.BackOffice.Infraestructura.Intefaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace SATI.BackOffice.Infraestructura.Entidades
{
    public class Carousel : IEntidades
    {
        public int Id { get; set; }
        [Display(Name ="Título")]
        [Required(ErrorMessage ="Es necesario que ingrese el {0}")]
        public string Descripcion { get; set; }
        [Display(Name ="Vigencia Desde")]
        [Required(ErrorMessage = "Es necesario que ingrese la {0}")]
        public DateTime VigenciaDesde { get; set; }
        [Display(Name = "Vigencia Hasta")]
        [Required(ErrorMessage = "Es necesario que ingrese la {0}")]
        public DateTime VigenciaHasta { get; set; }
        public string Imagen { get; set; }
        [Display(Name = "Redirección")]
        [Required(ErrorMessage = "Es necesario que ingrese la {0}")]
        public string Url { get; set; }
        [Display(Name = "Texto del Botón")]
        [Required(ErrorMessage = "Es necesario que ingrese el {0}")]
        public string TextoLink { get; set; }
        [Display(Name = "Orden")]
        [Required(ErrorMessage = "Es necesario que ingrese el {0}")]
        [Range(minimum:0,maximum:99,ErrorMessage ="Solo se puede ingresar valores entre el 0 y el 99")]
        public int Orden { get; set; }
        public string UbicacionFisica { get; set; }
        public object Archivo { get; set; }
    }
}
