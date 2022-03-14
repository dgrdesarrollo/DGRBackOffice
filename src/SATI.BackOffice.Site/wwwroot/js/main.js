var gImprimirReporte;

function AbrirWaiting(Mensaje) {
    if (Mensaje != "") {
        $('#lblWaiting').text(Mensaje);
    } else {
        $('#lblWaiting').text("Cargando...");
    }
    $('#wWaiting').fadeIn(0);
}

function onCamino(Camino, TituloPrincipal, TituloSecundario) {

    $('#liCamino').text(Camino);
    $('#h2TituloPrincipal').text(TituloPrincipal);
    $('#emTituloSecundario').text(TituloSecundario);
}

function CerrarWaiting() {
    $('#wWaiting').fadeOut(0);
}

var FunctionCallback = null;
function AbrirMensaje(Titulo, Mensaje, CallBack, EsConfirmacion, Botones, Tipo) {
    if (EsConfirmacion) {
        $("#btnMensajeAceptar").show();
        $("#btnMensajeCancelar").show();
    } else {
        $("#btnMensajeAceptar").show();
        $("#btnMensajeCancelar").hide();
    }
    if (Mensaje != null) {
        $('#msjContenido').html(Mensaje);
    } else {
        $('#msjContenido').html('Error inesperado, intente de nuevo en unos minutos...');
    }
    if (Titulo != null) {
        $('#msjTitulo').text(Titulo);
    } else {
        $('#msjTitulo').text('¡Atención!');
    }
    FunctionCallback = CallBack;
    if (Botones != null) {
        if (Botones.length == 1) {
            $("#btnMensajeAceptar").text(Botones[0]);
        }
        if (Botones.length == 2) {
            $("#btnMensajeAceptar").text(Botones[0]);
            $("#btnMensajeCancelar").text(Botones[1]);
        }
    } else {
        $("#btnMensajeAceptar").text("Aceptar");
        $("#btnMensajeCancelar").text("Cancelar");
    }
    //$('#msjModal').fadeIn(0);
    $("#msjIcono").html("");
    switch (Tipo) {
        case "Info!":
            $("#msjTitulo").prop("class", "text-info");
            $("#msjIcono").html('<i class="fa fa-2x fa-info text-info"></i>');
            break;
        case "Warning!":
            $("#msjTitulo").prop("class", "text-warning");
            $("#msjIcono").html('<i class="fa fa-2x fa-exclamation-triangle text-warning"></i>');
            break;
        case "Error!":
            $("#msjTitulo").prop("class", "text-danger");
            $("#msjIcono").html('<i class="fa fa-2x fa-stop-circle text-danger"></i>');
            break;
        case "success!":
            $("#msjTitulo").prop("class", "text-success");
            $("#msjIcono").html('<i class="fa fa-2x fa-check text-success"></i>');
            break;
        default:
            $("#msjIcono").prop("class", "");
            $("#msjIcono").html('');
            break;
    }
    if (Tipo != null) {
    } else {
    }

    $('#msjModal').modal('show');
}

function CerrarMensaje(Value) {
    //$('#msjModal').fadeOut(0);
    $('#msjModal').modal('hide');
    FunctionCallback(Value);
}

function AceptarMensaje(Value) {
    FunctionCallback(Value);
}

var FunctionCallbackSeleccionDeCuentas = null;
var g_cuit;
function AbrirSeleccionDeCuentas(nro_cuit, CallBack) {
    g_cuit = nro_cuit;
    //alert(g_cuit);
    FunctionCallbackSeleccionDeCuentas = CallBack;
    onIniciaSeleccionDeCuentas();

    $('#wSeleccionDeCuentas').modal('show');
}

function CerrarSeleccionDeCuentas(Value, Datos) {
    $('#wSeleccionDeCuentas').modal('hide');
    FunctionCallbackSeleccionDeCuentas(Value, Datos);
}

function AceptarSeleccionDeCuentas(Value, Datos) {
    $('#wSeleccionDeCuentas').modal('hide');
    FunctionCallbackSeleccionDeCuentas(Value, Datos);
}

var FunctionCallbackImportarRetenciones = null;
var g_tipo_retencion;
function AbrirImportarRetenciones(tipo_retencion, CallBack) {
    g_tipo_retencion = tipo_retencion;
    FunctionCallbackImportarRetenciones = CallBack;
    onIniciaImportarRetenciones();

    $('#sdcTitulo').text("Importación de " + tipo_retencion + "...");
    $('#wImportarRetenciones').modal('show');
}

function CerrarImportarRetenciones(Value, Datos) {
    $('#wImportarRetenciones').modal('hide');
    FunctionCallbackImportarRetenciones(Value, Datos);
}

function AceptarImportarRetenciones(Value, Datos) {
    $('#wImportarRetenciones').modal('hide');
    FunctionCallbackImportarRetenciones(Value, Datos);
}

function FechaToAAAAMMDD(Fecha) {
    if (Fecha == undefined || Fecha == null) { return "10010101" }
    var stFecha = Fecha.split("/");
    return stFecha[2] + stFecha[1] + stFecha[0];
}

const formatterPesos = new Intl.NumberFormat('es-AR', {
    style: 'decimal',
    currency: 'ARS'
});

function MensajeEmergente(titulo, mensaje, imagen, sonido) {
    if (sonido && imagen != null) {
        $.gritter.add({
            // (string | mandatory) the heading of the notification
            title: titulo,
            // (string | mandatory) the text inside the notification
            text: mensaje,
            image: imagen,
            after_open: function (e) {
                $(".gritter-with-image").append('<audio id="audioMensajeEmergente" > <source src="' + document.URL + 'bootstrap/audio/flash-message.mp3" type="audio/mpeg">Su browser no soporta audio.</audio>');
                $("#audioMensajeEmergente").trigger("play");
            }
        });
        return;
    }
    if (!sonido && imagen != null) {
        $.gritter.add({
            // (string | mandatory) the heading of the notification
            title: titulo,
            // (string | mandatory) the text inside the notification
            text: mensaje,
            image: imagen
        });
        return;
    }
    if (!sonido && imagen == null) {
        $.gritter.add({
            // (string | mandatory) the heading of the notification
            title: titulo,
            // (string | mandatory) the text inside the notification
            text: mensaje
        });
        return;
    }

    $.gritter.add({
        // (string | mandatory) the heading of the notification
        title: titulo,
        // (string | mandatory) the text inside the notification
        text: mensaje,
        after_open: function (e) {
            $(".gritter-without-image").append('<audio id="audioMensajeEmergente" > <source src="' + document.URL + 'bootstrap/audio/flash-message.mp3" type="audio/mpeg">Su browser no soporta audio.</audio>');
            $("#audioMensajeEmergente").trigger("play");
        }
    });
}

function AgregarCeros(str, max) {
    str = str.toString();
    return str.length < max ? AgregarCeros("0" + str, max) : str;
}

function validaEmail(email) {
    //var expr = /^([\w-\.]+)@@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    //return expr.test(email);

    var expr = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    if (!expr.test(email)) {
        return false;
        //Correo incorrecto.
    }
    return true;
};

function validaCuit(sCUIT) {
    var aMult = '5432765432';
    var aMult = aMult.split('');

    if (sCUIT && sCUIT.length == 11) {
        aCUIT = sCUIT.split('');
        var iResult = 0;
        for (i = 0; i <= 9; i++) {
            iResult += aCUIT[i] * aMult[i];
        }
        iResult = (iResult % 11);
        iResult = 11 - iResult;

        if (iResult == 11) iResult = 0;
        if (iResult == 10) iResult = 9;

        if (iResult == aCUIT[10]) {
            return true;
        }
    }
    return false;
}

function formato_numerico(number, decimals, dec_point, thousands_sep) {
    // Strip all characters but numerical ones.
    number = (number + '').replace(/[^0-9+\-Ee.]/g, '');
    var n = !isFinite(+number) ? 0 : +number,
        prec = !isFinite(+decimals) ? 0 : Math.abs(decimals),
        sep = (typeof thousands_sep === 'undefined') ? ',' : thousands_sep,
        dec = (typeof dec_point === 'undefined') ? '.' : dec_point,
        s = '',
        toFixedFix = function (n, prec) {
            var k = Math.pow(10, prec);
            return '' + Math.round(n * k) / k;
        };
    // Fix for IE parseFloat(0.55).toFixed(0) = 0;
    s = (prec ? toFixedFix(n, prec) : '' + Math.round(n)).split('.');
    if (s[0].length > 3) {
        s[0] = s[0].replace(/\B(?=(?:\d{3})+(?!\d))/g, sep);
    }
    if ((s[1] || '').length < prec) {
        s[1] = s[1] || '';
        s[1] += new Array(prec - s[1].length + 1).join('0');
    }
    return s.join(dec);
}


function FormatoFecha(Fecha, Formato) {
    var monthNames = ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"];

    var todayDate = Fecha;

    var date = todayDate.getDate().toString();
    var month = (todayDate.getMonth() + 1).toString();
    var year = todayDate.getFullYear().toString();
    var formattedMonth = (todayDate.getMonth() < 10) ? "0" + month : month;
    var formattedDay = (todayDate.getDate() < 10) ? "0" + date : date;
    var result = "";

    switch (Formato) {
        case "M/d/yyyy":
            formattedMonth = formattedMonth.indexOf("0") == 0 ? formattedMonth.substring(1, 2) : formattedMonth;
            formattedDay = formattedDay.indexOf("0") == 0 ? formattedDay.substring(1, 2) : formattedDay;

            result = formattedMonth + '/' + formattedDay + '/' + year;
            break;
        case "M/d/yy":
            formattedMonth = formattedMonth.indexOf("0") == 0 ? formattedMonth.substring(1, 2) : formattedMonth;
            formattedDay = formattedDay.indexOf("0") == 0 ? formattedDay.substring(1, 2) : formattedDay;
            result = formattedMonth + '/' + formattedDay + '/' + year.substr(2);
            break;
        case "MM/dd/yy":
            result = formattedMonth + '/' + formattedDay + '/' + year.substr(2);
            break;
        case "MM/dd/yyyy":
            result = formattedMonth + '/' + formattedDay + '/' + year;
            break;
        case "yy/MM/dd":
            result = year.substr(2) + '/' + formattedMonth.substr(-2) + '/' + formattedDay;
            break;
        case "yyyy-MM-dd":
            result = year + '-' + formattedMonth.substr(-2) + '-' + formattedDay.substr(-2);
            break;
        case "dd-MMM-yy":
            result = formattedDay.substr(-2) + '-' + monthNames[todayDate.getMonth()].substr(3) + '-' + year.substr(2);
            break;
        case "dd/MM/yyyy":
            result = formattedDay.substr(-2) + '/' + formattedMonth.substr(-2) + '/' + year;
            break;
        case "MMMM d, yyyy":
            result = todayDate.toLocaleDateString("es-ar", { day: 'numeric', month: 'long', year: 'numeric' });
            break;
    }

    return result;
}