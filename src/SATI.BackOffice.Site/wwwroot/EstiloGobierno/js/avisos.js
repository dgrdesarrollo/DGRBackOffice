$(document).ready(function () {
    $(".notice").click(function () {
        /*CAMBIO DE ICONO LEER MAS AVISOS*/
        var icono = $(this).children(".readMore");
        if(icono.children('svg').hasClass("fa-angle-down")){
          icono.children('svg').removeClass("fa-angle-down");
          icono.children('svg').addClass("fa-angle-up");
        }else{
          icono.children('svg').removeClass("fa-angle-up");
          icono.children('svg').addClass("fa-angle-down");
        }
        /*FIN CAMDIO DE ICONO LEER MAS AVISOS*/

        var noticia = $(this).children('.desc');
        noticia.fadeToggle();
          
        /*$(this).next().toggle(function () {
          if ($(this).text() == "Read") 
            $(this).text("Hide");
          else 
            $(this).text("Read");
        });*/
      });
})