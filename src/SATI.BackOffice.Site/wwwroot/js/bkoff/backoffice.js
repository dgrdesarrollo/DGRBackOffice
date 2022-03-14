$(document).ready(function () {

    $("#Archivo").change(function () {
       
        var archivo = $(this).val();
        var extensiones = archivo.substring(archivo.lastIndexOf("."));
        if (extensiones != ".jpg" && extensiones != ".gif" && extensiones != ".jpeg" && extensiones != ".png" && extensiones != ".bmp") {
            $("#file").html("<p class='alert alert-danger'><i class='fa fa-exclamation-circle'></i> El archivo de tipo <span class='text-uppercase'>" + extensiones + "</span> no es válido</p>");
            $(this).val("");
        }
        else {
            $("#file").html("");
        }
    });

    $("#Pdf").change(function () {

        var archivo = $(this).val();
        var extensiones = archivo.substring(archivo.lastIndexOf("."));
        if (extensiones != ".pdf") {
            $("#file").html("<p class='alert alert-danger'><i class='fa fa-exclamation-circle'></i> El archivo de tipo <span class='text-uppercase'>" + extensiones + "</span> no es válido</p>");
            $(this).val("");
        }
        else {
            $("#file").html("");
        }
    });
});